using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Drawing;
using System.Text;

using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.TreeSharp;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Pathing;
using Styx.CommonBot.Routines;

using CommonBehaviors.Actions;

using Action = Styx.TreeSharp.Action;

namespace DWCC5Free
{
    public partial class Warrior : CombatRoutine
    {
        public override sealed string Name { get { return "[FREE] DWCC v5.0 Alpha BT"; } }
        public override WoWClass Class { get { return WoWClass.Warrior; } }
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize
        public override void Initialize()
        {
            Logging.Write("[DWCC]: [FREE] DWCC v5.0 Alpha BT will do the job for you!");
            Logging.Write("[DWCC]: created by Wownerds Dev Team!");
            CharacterSettings.Instance.PullDistance = 24;
            Logging.WriteDiagnostic("[DWCC]: ###Character Information###");
            Logging.WriteDiagnostic("[DWCC]: Level: " + Me.Level);
            Logging.WriteDiagnostic("[DWCC]: Alliance/Horde: " + Me.IsAlliance + " || " + Me.IsHorde);
            Logging.WriteDiagnostic("[DWCC]: Health: " + Me.MaxHealth);
            Logging.WriteDiagnostic("[DWCC]: Movement Disabled: " + DunatanksSettings.Instance.DisableMovement);
            Logging.WriteDiagnostic("[DWCC]: Pull: " + !DunatanksSettings.Instance.DisableMovement);
            Logging.WriteDiagnostic("[DWCC]: CC Specc: " + DunatanksSettings.Instance.useArms + " || " + DunatanksSettings.Instance.useFury + " || " + DunatanksSettings.Instance.useProt);
            Logging.WriteDiagnostic("[DWCC]: ###Character Information###");
        }
        #endregion
        #region Pulse
        public override void Pulse()
        {
            StanceCheck();
            LifeSpiritRegen();
            Vigilance();
            AutoTankTargeting();
            if (NeedTankTargeting && (Me.GroupInfo.IsInParty || Me.GroupInfo.IsInRaid))
            {
                TankManager.Instance.Pulse();
            }
            RacialAbilities();
            MoveToTarget();
            MoveToCastingTarget();
        }
        #endregion
        #region Buttons
        public override bool WantButton
        {
            get
            {
                return true;
            }
        }
        public override void OnButtonPress()
        {
            DWCC5Free.cfg cfg = new DWCC5Free.cfg();
            cfg.ShowDialog();
        }
        #endregion
        #region Rest
        public override bool NeedRest
        {
            get
            {
                if (Me.HealthPercent <= DunatanksSettings.Instance.RestPercent && DunatanksSettings.Instance.UseRest)
                {
                    return true;
                }
                else
                    return false;
            }
        }

        public override void Rest()
        {
            if (Me.HealthPercent <= DunatanksSettings.Instance.RestPercent && DunatanksSettings.Instance.UseRest && !Me.IsFlying && !Me.IsSwimming)
            {
                Styx.CommonBot.Rest.Feed();
            }

        }
        #endregion
        #region Combat
        private Composite _combatBehavior;

        public override Composite CombatBehavior
        {
            get { if (_combatBehavior == null) { Logging.Write("[DWCC]: Creating DWCC MoP Combat Behavior."); _combatBehavior = CreateCombatBehavior(); } return _combatBehavior; }
        }

        private Composite CreateCombatBehavior()
        {
            return CreateDWCCBehavior();
        }
        #endregion
        #region Pull
        private Composite _pullBehavior;
        public override Composite PullBehavior
        {
            get
            {
                if (_pullBehavior == null)
                {
                    Logging.Write("[DWCC]: Creating 'Pull' behavior");
                    _pullBehavior = CreatePullBehavior();
                }
                _pullBehavior = CreatePullBehavior();
                return _pullBehavior;
            }
        }
        private PrioritySelector CreatePullBehavior()
        {
            return new PrioritySelector(

                 new Decorator(ret => DunatanksSettings.Instance.DisableMovement && !Me.GotTarget && !Me.CurrentTarget.IsFriendly && !Me.CurrentTarget.IsDead && Me.CurrentTarget.Attackable,
                    new Action(ctx => RunStatus.Success)),

                // Use leaders target
                new Decorator(
                    ret =>
                    !DunatanksSettings.Instance.DisableMovement && Me.GroupInfo.IsInParty && RaFHelper.Leader != null && RaFHelper.Leader.GotTarget && Me.GotTarget &&
                    Me.CurrentTargetGuid != RaFHelper.Leader.CurrentTargetGuid,
                    new Action(ret =>
                               RaFHelper.Leader.CurrentTarget.Target())),

                // Clear target and return failure if it's tagged by someone else
                new Decorator(ret => !DunatanksSettings.Instance.DisableMovement && !Me.GroupInfo.IsInParty && Me.GotTarget && Me.CurrentTarget.TaggedByOther,
                              new Action(delegate
                              {
                                  SpellManager.StopCasting();
                                  Logging.Write("[DWCC]: Target tagged, blacklisting!");
                                  Styx.CommonBot.Blacklist.Add(Me.CurrentTarget, TimeSpan.FromMinutes(30));
                                  Me.ClearTarget();
                                  return RunStatus.Failure;
                              })
                    ),

                // If we are casting we assume we are already pulling so let it 'return' smoothly. 
                // if we are in combat pull suceeded and the combat behavior should run
                new Decorator(ret => !DunatanksSettings.Instance.DisableMovement && (Me.IsCasting || Me.Combat) && Me.CurrentTarget.Distance < PullDistance + 3,
                              new Action(delegate { return RunStatus.Success; })),

                // Make sure we got a proper target
                new Decorator(ret => !DunatanksSettings.Instance.DisableMovement && !Me.GotTarget && !Me.GroupInfo.IsInParty,
                              new Action(delegate
                              {
                                  Targeting.Instance.TargetList[0].Target();
                                  WoWMovement.Face();
                                  Thread.Sleep(100);
                                  return RunStatus.Success;
                              })),

                // Blacklist target's we can't move to
                new Decorator(ret => !DunatanksSettings.Instance.DisableMovement && Navigator.GeneratePath(Me.Location, Me.CurrentTarget.Location).Length <= 0,
                              new Action(delegate
                              {
                                  Blacklist.Add(Me.CurrentTargetGuid, TimeSpan.FromDays(365));
                                  Logging.Write("[DWCC]: Can't move to: {0} blacklisted!",
                                      Me.CurrentTarget.Name);
                                  return RunStatus.Success;
                              })
                    ),

                // Move closer to the target if we are too far away or in !Los
                new Decorator(ret => !DunatanksSettings.Instance.DisableMovement && Me.GotTarget && (Me.CurrentTarget.Distance > PullDistance || !Me.CurrentTarget.InLineOfSight),
                              new Action(delegate
                              {
                                  if (!DunatanksSettings.Instance.DisableMovement)
                                  {
                                      Logging.Write("[DWCC]: Approaching:{0}", Me.CurrentTarget);
                                      Navigator.MoveTo(Me.CurrentTarget.Location);
                                  }
                              })),

                // Stop moving if we are moving
                new Decorator(ret => DunatanksSettings.Instance.DisableMovement && Me.IsMoving,
                              new Action(ret => WoWMovement.MoveStop())),

                // Face the target if we aren't
                new Decorator(ret => !DunatanksSettings.Instance.DisableMovement && Me.GotTarget && !Me.IsFacing(Me.CurrentTarget),
                              new Action(ret => WoWMovement.Face())
                    ),

                new PrioritySelector(
                                CreateAutoAttack(),
                                FaceTarget(ret => !DunatanksSettings.Instance.DisableMovement),
                                CastCharge(ret => Me.Level >= 3 && Me.IsFacing(Me.CurrentTarget.Location) && SpellManager.CanCast("Charge") && Me.CurrentTarget.Distance < 20 /*&& Navigator.CanNavigateFully(Me.Location, Me.CurrentTarget.Location)*/),
                                CreateSpellCheckAndCast("Heroic Throw", ret => Me.Level >= 20 && !SpellManager.Spells["Heroic Throw"].Cooldown && Me.CurrentTarget.Distance <= 30),
                                CreateSpellCheckAndCast("Intercept", ret => Me.Level >= 50 && Styx.WoWInternals.Battlegrounds.IsInsideBattleground && Me.CurrentTarget.Distance <= 25),
                                CreateSpellCheckAndCast("Throw", ret => Me.Inventory.Equipped.Ranged != null && SpellManager.CanCast("Throw") && !Styx.WoWInternals.Battlegrounds.IsInsideBattleground && Me.CurrentTarget.Distance <= 30 && Me.CurrentTarget.Distance > 15),
                                MoveToTarget(),
                new Decorator(ret => Me.IsMoving,
                              new Action(ret => WoWMovement.MoveStop()))


                          ));
        }
        #endregion
    }
}