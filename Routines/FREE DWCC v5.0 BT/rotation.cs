using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Styx;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Pathing;
using CommonBehaviors.Actions;
using Styx.TreeSharp;
using Action = Styx.TreeSharp.Action;
using Styx.Common;
using Styx.CommonBot;

namespace DWCC5Free
{
    public partial class Warrior
    {

        protected Composite CreateDWCCBehavior()
        {
            return new PrioritySelector(
                new Decorator(ret => StyxWoW.Me.CurrentTarget == null || Me.CurrentTarget.IsDead || !Me.CurrentTarget.Attackable || Me.IsDead || Me.Mounted || !Me.CurrentTarget.IsWithinMeleeRange,
                new ActionIdle()),
            #region HealthRegen
 new Decorator(ret => (DunatanksSettings.Instance.useHealthStone && Me.HealthPercent <= DunatanksSettings.Instance.HealthStonePercent && HaveHealthStone() && HealthStoneNotCooldown()),
            new Action(ret => UseHealthStone())),
            new Decorator(ret => (DunatanksSettings.Instance.usePotion && Me.HealthPercent <= DunatanksSettings.Instance.PotionPercent && HaveHealthPotion() && HealthPotionReady()),
            new Action(ret => UseHealthPotion())),
            #endregion
            FaceTarget(ret => !DunatanksSettings.Instance.DisableMovement && !AreaSpells()),
            HeroicLeap(ret => SpellManager.HasSpell("Heroic Leap") && Navigator.CanNavigateFully(Me.Location, Me.CurrentTarget.Location) && !DunatanksSettings.Instance.DisableMovement && !Me.CurrentTarget.IsWithinMeleeRange && !AreaSpells()),
            MoveToTarget(),
            CreateAutoAttack(),
            AoEPummel(),
            UseTrinketOne(),
            UseTrinketTwo(),
            CreateSpellCheckAndCast("Pummel", ret => DunatanksSettings.Instance.usePummel && Me.CurrentTarget.IsCasting /*&& Me.CurrentTarget.CanInterruptCurrentSpellCast*/), // CICSC does not seem to be working properly
            #region 1-9
            CreateSpellCheckAndCast("Execute", ret => Me.Level < 10 && Me.CurrentTarget.HealthPercent < 20),
            CreateSpellCheckAndCast("Victory Rush", ret => (Me.Level < 89 || !SpellManager.HasSpell("Impending Victory")) && Me.HasAura("Victorious")),
            #endregion
            #region Prot 10-89
                // CDs
                CreateSpellCheckAndCast("Spell Reflection", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useProt && Me.CurrentTarget.IsCasting && !Me.CurrentTarget.CastingSpell.IsFunnel && Me.CurrentTarget.CurrentTargetGuid == Me.Guid),
                CreateSpellCheckAndCast("Impending Victory", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useProt && Me.HealthPercent <= 80),
                CreateSpellCheckAndCast("Demoralizing Shout", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useProt && !Me.CurrentTarget.HasAura("Demoralizing Shout")),
                CreateSpellCheckAndCast("Last Stand", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useProt && Me.HealthPercent <= 30),
                CreateSpellCheckAndCast("Recklessness", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useProt),
                CreateSpellCheckAndCast("Deadly Calm", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useProt && Me.CurrentRage < 30),
                // AoE
                CreateSpellCheckAndCast("Thunderclap", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useProt && detectAdds().Count > 1),
                CreateSpellCheckAndCast("Cleave", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useProt && detectAdds().Count > 1 && (Me.HasAura("Ultimatum") || Me.CurrentRage > 90)),
                // single target
                CreateSpellCheckAndCast("Taunt", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useProt && DunatanksSettings.Instance.useTaunt && (!Me.CurrentTarget.Aggro && Me.CurrentTarget.GotTarget && !IsTank())),
                CreateSpellCheckAndCast("Shield Slam", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useProt),
                CreateSpellCheckAndCast("Revenge", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useProt),
                CreateSpellCheckAndCast("Devastate", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useProt),
                CreateSpellCheckAndCast("Thunder Clap", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useProt && (!Me.CurrentTarget.HasAura("Weakened Blows") || Me.CurrentTarget.ActiveAuras["Weakened Blows"].TimeLeft.TotalSeconds < 2)),
                CreateSpellCheckAndCast("Heroic Strike", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useProt && (Me.HasAura("Ultimatum") || Me.CurrentRage > 90)),
                CreateSpellCheckAndCast("Commanding Shout", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useProt && Me.CurrentRage <= 30),
            #endregion
            #region Fury 10-89
                // Fury MoP T13 HC
            CreateSpellCheckAndCast("Recklessness", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useFury && SpellManager.HasSpell("Colossus Smash") && (Me.CurrentTarget.HasAura("Colossus Smash") && (Me.CurrentTarget.Auras["Colossus Smash"].TimeLeft.TotalSeconds >= 5) || SpellManager.Spells["Colossus Smash"].CooldownTimeLeft.TotalSeconds >= 4) && Me.CurrentTarget.HealthPercent <= 20),
                // Add 2 charges check to RB condition
            CreateSpellCheckAndCast("Berserker Rage", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useFury && (!IsEnraged(Me) || /*SpellManager.Spells["Raging Blow"].Charges == 2 && */Me.CurrentTarget.HealthPercent >= 20)),
                // MISSING: Heroic Leap
            CreateSpellCheckAndCast("Deadly Calm", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useFury && Me.CurrentRage >= 40),
            CreateSpellCheckAndCast("Heroic Strike", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useFury && (((Me.CurrentTarget.HasAura("Colossus Smash") && Me.CurrentRage >= 40) || (Me.HasAura("Deadly Calm") && Me.CurrentRage >= 30)) && Me.CurrentTarget.HealthPercent >= 20) || Me.CurrentRage >= 110),
            CreateSpellCheckAndCast("Bloodthirst", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useFury && Me.CurrentTarget.HealthPercent >= 20 && !Me.CurrentTarget.HasAura("Colossus Smash")/* && Me.CurrentRage <= 30*/), //more dps without rage check
            CreateSpellCheckAndCast("Wild Strike", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useFury && Bloodsurge != null && Me.CurrentTarget.HealthPercent >= 20 && SpellManager.Spells["Bloodthirst"].CooldownTimeLeft.TotalSeconds <= 1),
            CreateSpellCheckAndCast("Colossus Smash", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useFury),
            CreateSpellCheckAndCast("Execute", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useFury && Me.CurrentTarget.HealthPercent < 20),
            CreateSpellCheckAndCast("Raging Blow", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useFury && Me.HasAura("Raging Blow!")),
            CreateSpellCheckAndCast("Shockwave", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useFury),
            CreateSpellCheckAndCast("Dragon Roar", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useFury),
            CreateSpellCheckAndCast("Battle Shout", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useFury && Me.CurrentRage < 70 && !Me.CurrentTarget.HasAura("Colossus Smash")),
            CreateSpellCheckAndCast("Bladestorm", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useFury && SpellManager.HasSpell("Colossus Smash") && SpellManager.Spells["Colossus Smash"].CooldownTimeLeft.TotalSeconds >= 5 && !Me.CurrentTarget.HasAura("Colossus Smash") && SpellManager.Spells["Bloodthirst"].CooldownTimeLeft.TotalSeconds >= 2 && Me.CurrentTarget.HealthPercent >= 20),
            CreateSpellCheckAndCast("Wild Strike", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useFury && Me.CurrentTarget.HasAura("Colossus Smash") && Me.CurrentTarget.HealthPercent <= 20),
            CreateSpellCheckAndCast("Impending Victory", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useFury && Me.CurrentTarget.HealthPercent >= 20),
            CreateSpellCheckAndCast("Wild Strike", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useFury && SpellManager.HasSpell("Colossus Smash") && SpellManager.Spells["Colossus Smash"].CooldownTimeLeft.TotalSeconds >= 1 && Me.CurrentRage >= 60 && Me.CurrentTarget.HealthPercent >= 20),
            CreateSpellCheckAndCast("Battle Shout", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useFury && Me.CurrentTarget.HealthPercent < 70),
            #endregion
            #region Arms 10-89
 CreateSpellCheckAndCast("Recklessness", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useArms && SpellManager.HasSpell("Colossus Smash") && (Me.CurrentTarget.HasAura("Colossus Smash") && (Me.CurrentTarget.Auras["Colossus Smash"].TimeLeft.TotalSeconds >= 5) || SpellManager.Spells["Colossus Smash"].CooldownTimeLeft.TotalSeconds >= 4) && Me.CurrentTarget.HealthPercent <= 20),
            CreateSpellCheckAndCast("Berserker Rage", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useArms && !IsEnraged(Me)),
                // Heroic Leap missing
            CreateSpellCheckAndCast("Deadly Calm", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useArms && Me.CurrentRage >= 40),
            CreateSpellCheckAndCast("Heroic Strike", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useArms && (((Me.CurrentTarget.HasAura("Colossus Smash") && Me.CurrentRage >= 40) || (Me.HasAura("Deadly Calm") && Me.CurrentRage >= 30)) && Me.CurrentTarget.HealthPercent >= 20) || Me.CurrentRage >= 110),
            CreateSpellCheckAndCast("Mortal Strike", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useArms),
            CreateSpellCheckAndCast("Colossus Smash", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useArms && SpellManager.HasSpell("Colossus Smash") && (!Me.CurrentTarget.HasAura("Colossus Smash") || (Me.CurrentTarget.HasAura("Colossus Smash") && Me.CurrentTarget.Auras["Colossus Smash"].TimeLeft.TotalMilliseconds <= 1500))),
            CreateSpellCheckAndCast("Execute", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useArms),
            CreateSpellCheckAndCast("Overpower", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useArms),
            CreateSpellCheckAndCast("Shockwave", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useArms),
            CreateSpellCheckAndCast("Dragon Roar", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useArms),
            CreateSpellCheckAndCast("Slam", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useArms && (Me.CurrentRage > 70 || Me.CurrentTarget.HasAura("Colossus Smash")) && Me.CurrentTarget.HealthPercent >= 20),
            CreateSpellCheckAndCast("Heroic Throw", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useArms),
            CreateSpellCheckAndCast("Battle Shout", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useArms && Me.CurrentRage < 70 && !Me.CurrentTarget.HasAura("Colossus Smash")),
            CreateSpellCheckAndCast("Bladestorm", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useArms && SpellManager.HasSpell("Colossus Smash") && SpellManager.Spells["Colossus Smash"].CooldownTimeLeft.TotalSeconds >= 5 && !Me.CurrentTarget.HasAura("Colossus Smash") && SpellManager.HasSpell("Mortal Strike") && SpellManager.Spells["Mortal Strike"].CooldownTimeLeft.TotalSeconds >= 2 && Me.CurrentTarget.HealthPercent >= 20),
            CreateSpellCheckAndCast("Slam", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useArms && Me.CurrentTarget.HealthPercent >= 20),
            CreateSpellCheckAndCast("Impending Victory", ret => Me.Level >= 10 && Me.Level < 90 && DunatanksSettings.Instance.useArms && Me.CurrentTarget.HealthPercent >= 20),
            CreateSpellCheckAndCast("Battle Shout", ret => Me.CurrentRage < 70),
            #endregion
            #region HeroStrike
 CreateSpellCheckAndCast("Heroic Strike", ret => Me.Level < 81),
            #endregion
            #region Prot 90
 CreateSpellCheckAndCast("Taunt", ret => Me.Level == 90 && DunatanksSettings.Instance.useProt && DunatanksSettings.Instance.useTaunt && (!Me.CurrentTarget.Aggro && Me.CurrentTarget.GotTarget && !IsTank())),
            CreateSpellCheckAndCast("Last Stand", ret => Me.Level == 90 && DunatanksSettings.Instance.useProt && Me.CurrentHealth < 130000),
            CreateSpellCheckAndCast("Avatar", ret => Me.Level == 90 && DunatanksSettings.Instance.useProt),
            CreateSpellCheckAndCast("Shockwave", ret => Me.Level == 90 && DunatanksSettings.Instance.useProt && detectAdds().Count > 2),
            CreateSpellCheckAndCast("Berserker Rage", ret => Me.Level == 90 && DunatanksSettings.Instance.useProt),
            CreateSpellCheckAndCast("Heroic Strike", ret => Me.Level == 90 && DunatanksSettings.Instance.useProt && Me.HasAura("Ultimatum")),
            CreateSpellCheckAndCast("Shield Slam", ret => Me.Level == 90 && DunatanksSettings.Instance.useProt && Me.CurrentRage < 90),
            CreateSpellCheckAndCast("Revenge", ret => Me.Level == 90 && DunatanksSettings.Instance.useProt && Me.CurrentRage < 100),
            CreateSpellCheckAndCast("Battle Shout", ret => Me.Level == 90 && DunatanksSettings.Instance.useProt && Me.CurrentRage < 100),
            CreateSpellCheckAndCast("Shield Block", ret => Me.Level == 90 && DunatanksSettings.Instance.useProt),
            CreateSpellCheckAndCast("Shield Barrier", ret => Me.Level == 90 && DunatanksSettings.Instance.useProt && !Me.HasAura("Shield Barrier") && Me.CurrentRage > 80),
            CreateSpellCheckAndCast("Shield Wall", ret => Me.Level == 90 && DunatanksSettings.Instance.useProt && !Me.HasAura("Shield Block")),
            CreateSpellCheckAndCast("Demoralizing Shout", ret => Me.Level == 90 && DunatanksSettings.Instance.useProt),
            CreateSpellCheckAndCast("Thunder Clap", ret => Me.Level == 90 && DunatanksSettings.Instance.useProt && !Me.CurrentTarget.HasAura("Weakened Blows") && detectAdds().Count > 1),
            CreateSpellCheckAndCast("Devastate", ret => Me.Level == 90 && DunatanksSettings.Instance.useProt),
            #endregion
            #region Fury 90
 CreateSpellCheckAndCast("Impending Victory", ret => Me.Level == 90 && DunatanksSettings.Instance.useFury && Me.HealthPercent < 60),
                //CD's
            CreateSpellCheckAndCast("Recklessness", ret => Me.Level == 90 && DunatanksSettings.Instance.useFury && Me.CurrentTarget.HealthPercent > 80 || Me.CurrentTarget.Name.Contains("Training Dummy") || Me.CurrentTarget.IsBoss && Me.CurrentTarget.HasAura("Colossus Smash") && SpellManager.CanCast("Raging Blow")),
            CreateSpellCheckAndCast("Avatar", ret => Me.Level == 90 && DunatanksSettings.Instance.useFury && Me.HasAura("Recklessness")),
            CreateSpellCheckAndCast("Skull Banner", ret => Me.Level == 90 && DunatanksSettings.Instance.useFury && Me.HasAura("Recklessness")),
            CreateSpellCheckAndCast("Bloodbath", ret => Me.Level == 90 && DunatanksSettings.Instance.useFury && Me.CurrentTarget.HasAura(86346) && SpellManager.CanCast("Raging Blow")),
            CreateSpellCheckAndCast("Berserker Rage", ret => Me.Level == 90 && DunatanksSettings.Instance.useFury && !IsEnraged(Me) && Me.CurrentTarget.HasAura(86346) || (Me.HasAura("Recklessness") && !Me.HasAura("Raging Blow!"))),
                //Rotation
            CreateSpellCheckAndCast("Heroic Strike", ret => Me.Level == 90 && DunatanksSettings.Instance.useFury && Me.PowerPercent >= 95 && Me.CurrentTarget.HealthPercent > 20),
            CreateSpellCheckAndCast("Bloodthirst", ret => Me.Level == 90 && DunatanksSettings.Instance.useFury),
            CreateSpellCheckAndCast("Colossus Smash", ret => Me.Level == 90 && DunatanksSettings.Instance.useFury && !Me.CurrentTarget.HasAura(86346)),
            CreateSpellCheckAndCast("Execute", ret => Me.Level == 90 && DunatanksSettings.Instance.useFury),
            CreateSpellCheckAndCast("Raging Blow", ret => Me.Level == 90 && DunatanksSettings.Instance.useFury && Me.CurrentTarget.HealthPercent > 20),
            CreateSpellCheckAndCast("Shockwave", ret => Me.Level == 90 && DunatanksSettings.Instance.useFury && Me.CurrentTarget.HealthPercent > 20),
            CreateSpellCheckAndCast("Dragon Roar", ret => Me.Level == 90 && DunatanksSettings.Instance.useFury && Me.CurrentTarget.HealthPercent > 20),
            CreateSpellCheckAndCast("Wild Strike", ret => Me.Level == 90 && DunatanksSettings.Instance.useFury && Me.HasAura(46916) && Me.CurrentTarget.HealthPercent > 20),       //46916 = Bloodsurge
            CreateSpellCheckAndCast("Heroic Strike", ret => Me.Level == 90 && DunatanksSettings.Instance.useFury && (Me.PowerPercent >= 67 || (Me.HasAura("Deadly Calm") && Me.PowerPercent > 40)) && Me.CurrentTarget.HealthPercent > 20),
            CreateSpellCheckAndCast("Impending Victory", ret => Me.Level == 90 && DunatanksSettings.Instance.useFury && Me.CurrentTarget.HealthPercent > 20 || Me.HealthPercent < 40),
            CreateSpellCheckAndCast("Heroic Throw", ret => Me.Level == 90 && DunatanksSettings.Instance.useFury && Me.CurrentTarget.Distance > 10 && Me.CurrentTarget.HealthPercent > 20),
            CreateSpellCheckAndCast("Deadly Calm", ret => Me.Level == 90 && DunatanksSettings.Instance.useFury && Me.PowerPercent > 50 && Me.CurrentTarget.HealthPercent > 20),
            CreateSpellCheckAndCast("Battle Shout", ret => Me.Level == 90 && DunatanksSettings.Instance.useFury && Me.CurrentRage < 70),
            #endregion
            #region Arms 90
 CreateSpellCheckAndCast("Recklessness", ret => Me.Level == 90 && DunatanksSettings.Instance.useArms && ((Me.CurrentTarget.HasAura("Colossus Smash") && Me.CurrentTarget.Debuffs["Colossus Smash"].TimeLeft.TotalSeconds >= 5) || (SpellManager.HasSpell("Colossus Smash") && SpellManager.Spells["Colossus Smash"].CooldownTimeLeft.TotalSeconds <= 4)) && Me.CurrentTarget.HealthPercent < 20),
            CreateSpellCheckAndCast("Avatar", ret => Me.Level == 90 && DunatanksSettings.Instance.useArms && SpellManager.Spells["Recklessness"].CooldownTimeLeft.TotalSeconds >= 180 || Me.HasAura("Recklessness")),
            CreateSpellCheckAndCast("Bloodbath", ret => Me.Level == 90 && DunatanksSettings.Instance.useArms && (SpellManager.Spells["Recklessness"].CooldownTimeLeft.TotalSeconds >= 10 || Me.CurrentTarget.HealthPercent >= 20)),
            CreateSpellCheckAndCast("Berserker Rage", ret => Me.Level == 90 && DunatanksSettings.Instance.useArms && !IsEnraged(Me)),
                // MISSING: HEROIC LEAP
            CreateSpellCheckAndCast("Deadly Calm", ret => Me.Level == 90 && DunatanksSettings.Instance.useArms && Me.CurrentRage >= 40),
            CreateSpellCheckAndCast("Heroic Strike", ret => Me.Level == 90 && DunatanksSettings.Instance.useArms && ((Me.HasAura("Taste for Blood") && Me.ActiveAuras["Taste for Blood"].TimeLeft.TotalSeconds <= 2) || SpellManager.CanCast("Overpower") || Me.HasAura("Taste for Blood") && !SpellManager.Spells["Colossus Smash"].Cooldown || Me.HasAura("Deadly Calm") || Me.CurrentRage > 110) && Me.CurrentTarget.HealthPercent > 20 && Me.CurrentTarget.HasAura("Colossus Smash")),
            CreateSpellCheckAndCast("Mortal Strike", ret => Me.Level == 90 && DunatanksSettings.Instance.useArms),
            CreateSpellCheckAndCast("Colossus Smash", ret => Me.Level == 90 && DunatanksSettings.Instance.useArms && (!Me.CurrentTarget.HasAura("Colossus Smash") || (Me.CurrentTarget.HasAura("Colossus Smash") && Me.CurrentTarget.ActiveAuras["Colossus Smash"].TimeLeft.TotalSeconds <= 1.5))),
            CreateSpellCheckAndCast("Execute", ret => Me.Level == 90 && DunatanksSettings.Instance.useArms),
            CreateSpellCheckAndCast("Storm Bolt", ret => Me.Level == 90 && DunatanksSettings.Instance.useArms),
            CreateSpellCheckAndCast("Overpower", ret => Me.Level == 90 && DunatanksSettings.Instance.useArms),
            CreateSpellCheckAndCast("Shockwave", ret => Me.Level == 90 && DunatanksSettings.Instance.useArms),
            CreateSpellCheckAndCast("Dragon Roar", ret => Me.Level == 90 && DunatanksSettings.Instance.useArms),
            CreateSpellCheckAndCast("Slam", ret => Me.Level == 90 && DunatanksSettings.Instance.useArms && (Me.CurrentRage >= 70 || Me.CurrentTarget.HasAura("Colossus Smash")) && Me.CurrentTarget.HealthPercent >= 20),
            CreateSpellCheckAndCast("Heroic Throw", ret => Me.Level == 90 && DunatanksSettings.Instance.useArms),
            CreateSpellCheckAndCast("Battle Shout", ret => Me.Level == 90 && DunatanksSettings.Instance.useArms && Me.CurrentRage < 70 && !Me.CurrentTarget.HasAura("Colossus Smash")),
            CreateSpellCheckAndCast("Bladestorm", ret => Me.Level == 90 && DunatanksSettings.Instance.useArms && SpellManager.HasSpell("Colossus Smash") && SpellManager.Spells["Colossus Smash"].CooldownTimeLeft.TotalSeconds >= 5 && !Me.CurrentTarget.HasAura("Colossus Smash") && SpellManager.Spells["Mortal Strike"].CooldownTimeLeft.TotalSeconds >= 2 && Me.CurrentTarget.HealthPercent >= 20),
            CreateSpellCheckAndCast("Slam", ret => Me.Level == 90 && DunatanksSettings.Instance.useArms && Me.CurrentTarget.HealthPercent >= 20),
            CreateSpellCheckAndCast("Impending Victory", ret => Me.Level == 90 && DunatanksSettings.Instance.useArms && Me.CurrentTarget.HealthPercent >= 20),
            CreateSpellCheckAndCast("Battle Shout", ret => Me.Level == 90 && DunatanksSettings.Instance.useArms && Me.CurrentRage < 70),
            #endregion
            MoveToTargetProper()
);
        }
    }
}