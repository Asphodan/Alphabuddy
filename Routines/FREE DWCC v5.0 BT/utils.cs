using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Diagnostics;
using Styx.XmlEngine;

using Styx;
using Styx.TreeSharp;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.CommonBot;
using Styx.Common;
using Styx.Resources;
using Styx.Loaders;
using Styx.Common.Helpers;
using Styx.Pathing;
using CommonBehaviors.Actions;

using Action = Styx.TreeSharp.Action;

namespace DWCC5Free
{
    public partial class Warrior
    {
        #region SpellCaster
        public Composite CreateSpellCheckAndCast(string name, CanRunDecoratorDelegate extra)
        {
            return new Decorator(ret => extra(ret) && SpellManager.CanCast(name),
                                 new Action(delegate
                                 {
                                     SpellManager.Cast(name);
                                     Logging.Write("[DWCC]: " + name + ".");
                                     Logging.WriteDiagnostic("[DWCC]: Attempting to cast " + name + " on " + Me.CurrentTarget.Name.Remove(3, Me.CurrentTarget.Name.Length - 3) + "*** @ " + Me.CurrentTarget.CurrentHealth + "/" + Me.CurrentTarget.MaxHealth + " (" + Math.Round(Me.CurrentTarget.HealthPercent, 2) + "%)");
                                     Logging.WriteDiagnostic("[DWCC]: Target: IsCasting: " + Me.CurrentTarget.IsCasting + " | IsPlayer: " + Me.CurrentTarget.IsPlayer + " | Distance: " + Math.Round(Me.CurrentTarget.Distance, 2) + " | Level: " + Me.CurrentTarget.Level + " | IsElite: " + Me.CurrentTarget.Elite + " | Adds: " + detectAdds().Count);
                                     Logging.WriteDiagnostic("[DWCC]: We are in: " + Me.ZoneText + " | Instance: " + Me.IsInInstance + " | Outdoors: " + Me.IsOutdoors + " | Battleground: " + Styx.WoWInternals.Battlegrounds.IsInsideBattleground + " | Indoors: " + Me.IsIndoors + " | Party: " + Me.GroupInfo.IsInParty + " | Raid: " + Me.GroupInfo.IsInRaid + " | Members: " + Me.PartyMembers.Count + "/" + Me.RaidMembers.Count + " | Health: " + Me.CurrentHealth + "/" + Me.MaxHealth + " (" + Math.Round(Me.HealthPercent, 2) + "%) | BattleStance: " + BattleStance + " | DefStance: " + DefStance + " | BerserkerStance: " + BersiStance);
                                 }
                                     ));
        }
        #endregion
        #region Enrage
        public bool IsEnraged(WoWUnit unit)
        {
            return Me.GetAllAuras().Any(
                a => a.Spell.Mechanic == WoWSpellMechanic.Enraged
                      );
        }
        #endregion
        #region Add Detection
        //Credit to CodeNameGamma for detectAdds code
        private List<WoWUnit> detectAdds()
        {
            List<WoWUnit> addList = ObjectManager.GetObjectsOfType<WoWUnit>(false).FindAll(unit =>
                        unit.Guid != Me.Guid &&
                        unit.Distance < DunatanksSettings.Instance.CombatDistance &&
                        unit.IsAlive &&
                        (unit.Combat || unit.Name == "Training Dummy" || unit.Name == "Raider's Training Dummy" || unit.IsTotem || unit.IsPlayer) &&
                        !unit.IsPet &&
                        !unit.IsFriendly &&
                        unit.Attackable &&
                        !Styx.CommonBot.Blacklist.Contains(unit.Guid));
            //Logging.Write(addList.Count.ToString());


            return addList;
        }
        #endregion
        #region Bloodsurge
        public static WoWAura Bloodsurge
        {
            get
            {
                return StyxWoW.Me.GetAuraById(46916);
            }
        }
        #endregion
        #region Stances
        public bool BattleStance
        {
            get
            {
                if (StyxWoW.Me.HasAura(2457))
                {
                    return true;
                }
                return false;
            }
        }

        public bool DefStance
        {
            get
            {
                if (StyxWoW.Me.HasAura(71))
                {
                    return true;
                }
                return false;
            }
        }

        public bool BersiStance
        {
            get
            {
                if (StyxWoW.Me.HasAura(2458))
                {
                    return true;
                }
                return false;
            }
        }
        #endregion
        #region IsTank
        public bool IsTank()
        {
            WoWPlayer PotentialTank = null;
            if (Me.CurrentTarget.CurrentTarget.IsPlayer && Me.CurrentTarget.CurrentTarget.CurrentTarget != Me)
            {
                PotentialTank = Me.CurrentTarget.CurrentTarget.ToPlayer();
                if (PotentialTank.Type.Equals(SpecType.Tank))
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        #endregion
        #region AutoAttack
        public static Composite CreateAutoAttack()
        {

            return new PrioritySelector(
                new Decorator(
                    ret => !StyxWoW.Me.IsAutoAttacking && StyxWoW.Me.CurrentTarget.IsWithinMeleeRange,
                    new Action(ret => StyxWoW.Me.ToggleAttack()))
            );
        }
        #endregion
        #region UnitSelection
        public delegate WoWUnit UnitSelectDelegate(object context);
        #endregion
        #region TankTargetingClass
        internal class TankManager : Targeting
        {
            static TankManager()
            {
                Instance = new TankManager { NeedToTaunt = new List<WoWUnit>() };
            }

            #region CrowdControlled
            // Credits to Apoc and Singular devs
            public bool IsCrowdControlled(WoWUnit unit)
            {
                return Me.GetAllAuras().Any(
                    a => a.IsHarmful &&
                         (a.Spell.Mechanic == WoWSpellMechanic.Shackled ||
                          a.Spell.Mechanic == WoWSpellMechanic.Polymorphed ||
                          a.Spell.Mechanic == WoWSpellMechanic.Horrified ||
                          a.Spell.Mechanic == WoWSpellMechanic.Rooted ||
                          a.Spell.Mechanic == WoWSpellMechanic.Frozen ||
                          a.Spell.Mechanic == WoWSpellMechanic.Stunned ||
                          a.Spell.Mechanic == WoWSpellMechanic.Fleeing ||
                          a.Spell.Mechanic == WoWSpellMechanic.Banished ||
                          a.Spell.Mechanic == WoWSpellMechanic.Sapped
                          ));
            }
            #endregion

            public new static TankManager Instance { get; set; }
            public List<WoWUnit> NeedToTaunt { get; private set; }

            public static readonly WaitTimer TargetingTimer = new WaitTimer(TimeSpan.FromSeconds(1));

            protected override List<WoWObject> GetInitialObjectList()
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>(false, false).Cast<WoWObject>().ToList();
            }

            protected override void DefaultRemoveTargetsFilter(List<WoWObject> units)
            {
                for (int i = units.Count - 1; i >= 0; i--)
                {
                    if (!units[i].IsValid)
                    {
                        units.RemoveAt(i);
                        continue;
                    }

                    WoWUnit u = units[i].ToUnit();

                    if (u.IsFriendly || u.IsDead || u.IsPet || !u.Combat || IsCrowdControlled(u))
                    {
                        units.RemoveAt(i);
                        continue;
                    }

                    if (u.DistanceSqr > 40 * 40)
                    {
                        units.RemoveAt(i);
                        continue;
                    }

                    if (u.CurrentTarget == null)
                        continue;

                    WoWUnit tar = u.CurrentTarget;
                    if (!tar.IsPlayer || !tar.IsHostile)
                        continue;

                    units.RemoveAt(i);
                }
            }

            protected override void DefaultIncludeTargetsFilter(List<WoWObject> incomingUnits, HashSet<WoWObject> outgoingUnits)
            {
                foreach (WoWObject i in incomingUnits)
                {
                    var unit = i as WoWUnit;

                    outgoingUnits.Add(i);
                }
            }

            protected override void DefaultTargetWeight(List<TargetPriority> units)
            {
                NeedToTaunt.Clear();
                List<WoWPlayer> members = StyxWoW.Me.GroupInfo.IsInRaid ? StyxWoW.Me.RaidMembers : StyxWoW.Me.PartyMembers;
                foreach (TargetPriority p in units)
                {
                    WoWUnit u = p.Object.ToUnit();
                    int aggroDiff = GetAggroDifferenceFor(u, members);
                    p.Score -= aggroDiff;
                    if (aggroDiff < 0 && !u.Fleeing)
                    {
                        NeedToTaunt.Add(u);
                    }
                }
            }

            private static int GetAggroDifferenceFor(WoWUnit unit, IEnumerable<WoWPlayer> partyMembers)
            {
                uint myThreat = unit.ThreatInfo.ThreatValue;
                uint highestParty = (from p in partyMembers
                                     let tVal = unit.GetThreatInfoFor(p).ThreatValue
                                     orderby tVal descending
                                     select tVal).FirstOrDefault();

                int result = (int)myThreat - (int)highestParty;
                return result;
            }
        }
        #endregion
        #region TankPrio
        #region targeting
        //credits to Singular devs
        public bool NeedTankTargeting { get; set; }

        private static readonly WaitTimer targetingTimer = new WaitTimer(TimeSpan.FromSeconds(2));

            protected Composite CreateEnsureTarget()
        {
            return
                new PrioritySelector(
                    new Decorator(
                        ret => NeedTankTargeting && targetingTimer.IsFinished && Me.Combat &&
                               TankManager.Instance.FirstUnit != null && Me.CurrentTarget != TankManager.Instance.FirstUnit,
                        new Action(
                            ret =>
                            {
                                Logging.WriteDiagnostic("Targeting first unit of TankTargeting");
                                TankManager.Instance.FirstUnit.Target();
                                StyxWoW.SleepForLagDuration();
                                targetingTimer.Reset();
                            })),
                    new Decorator(
                        ret => Me.CurrentTarget == null || Me.CurrentTarget.IsDead || Me.CurrentTarget.IsFriendly,
                        new PrioritySelector(
                            ctx =>
                            {
                                if (Me.IsInInstance)
                                    return null;
                                if (RaFHelper.Leader != null && RaFHelper.Leader.Combat)
                                {
                                    return RaFHelper.Leader.CurrentTarget;
                                }
                                if (Targeting.Instance.FirstUnit != null && Me.Combat)
                                {
                                    return Targeting.Instance.FirstUnit;
                                }
                                var units =
                                    ObjectManager.GetObjectsOfType<WoWUnit>(false, false).Where(
                                        p => p.IsHostile && !p.IsOnTransport && !p.IsDead && p.DistanceSqr <= 70 * 70 && p.Combat);

                                if (Me.Combat && units.Any())
                                {
                                    return units.OrderBy(u => u.DistanceSqr).FirstOrDefault();
                                }

                                return null;
                            },
                            new Decorator(
                                ret => ret != null,
                                new Sequence(
                                    new Action(ret => Logging.Write("Target is invalid. Switching target!")),
                                    new Action(ret => ((WoWUnit)ret).Target()))),
                            new Decorator(
                                ret => Me.CurrentTarget != null,
                                new Action(
                                    ret =>
                                    {
                                        Me.ClearTarget();
                                        return RunStatus.Failure;
                                    })))));
        }



        #endregion
        #region movement
        //credits to Singular devs


        protected Composite CreateMoveToAndFace(float maxRange, float coneDegrees, UnitSelectDelegate unit, bool noMovement)
        {
            return new Decorator(
                ret => !DunatanksSettings.Instance.DisableMovement && unit(ret) != null,
                new PrioritySelector(
                    new Decorator(
                        ret => (!unit(ret).InLineOfSight || (!noMovement && unit(ret).DistanceSqr > maxRange * maxRange)),
                        new Action(ret => Navigator.MoveTo(unit(ret).Location))),
                //Returning failure for movestop for smoother movement
                //Rest should return success !
                    new Decorator(
                        ret => Me.IsMoving && unit(ret).DistanceSqr <= maxRange * maxRange,
                        new Action(delegate
                        {
                            Navigator.PlayerMover.MoveStop();
                            return RunStatus.Failure;
                        })),
                    new Decorator(
                        ret => Me.CurrentTarget != null && Me.CurrentTarget.IsAlive && !Me.IsSafelyFacing(Me.CurrentTarget, coneDegrees),
                        new Action(ret => Me.CurrentTarget.Face()))
                    ));
        }

        //edited singular code
        protected Composite MoveToTarget()
        {
            return new Decorator(
                ret => !DunatanksSettings.Instance.DisableMovement && Me.CurrentTarget != null,
                new PrioritySelector(
                    new Decorator(
                        ret => (!Me.CurrentTarget.InLineOfSight || Me.CurrentTarget.DistanceSqr > 5 * 5 || !Me.CurrentTarget.IsWithinMeleeRange),
                        new Action(ret => Navigator.MoveTo(Me.CurrentTarget.Location))),
                //Returning failure for movestop for smoother movement
                //Rest should return success !
                    new Decorator(
                        ret => Me.CurrentTarget.IsWithinMeleeRange,
                        new Action(delegate
                        {
                            CreateAutoAttack();
                            Navigator.PlayerMover.MoveStop();
                            return RunStatus.Failure;
                        })))
                    );
        }

        protected Composite MoveToCastingTarget()
        {
            return new Decorator(
                ret => !DunatanksSettings.Instance.DisableMovement && !AreaSpells() && Me.CurrentTarget != null && Me.CurrentTarget.IsCasting && Me.CurrentTarget.CurrentTarget == Me,
                new PrioritySelector(
                    new Decorator(
                        ret => (!Me.CurrentTarget.InLineOfSight || Me.CurrentTarget.DistanceSqr > 5 * 5 || !Me.CurrentTarget.IsWithinMeleeRange),
                        new Action(ret => Navigator.MoveTo(Me.CurrentTarget.Location + 2f))),
                //Returning failure for movestop for smoother movement
                //Rest should return success !
                    new Decorator(
                        ret => Me.CurrentTarget.IsWithinMeleeRange,
                        new Action(delegate
                        {
                            CreateAutoAttack();
                            Navigator.PlayerMover.MoveStop();
                            return RunStatus.Failure;
                        })))
                    );
        }

        public Composite MoveToTargetProper()
        {
            if (!DunatanksSettings.Instance.DisableMovement && Me.CurrentTarget != null)
            {
                if (!Me.CurrentTarget.InLineOfSight || !Me.CurrentTarget.IsWithinMeleeRange)
                {
                    return new Action(delegate
                    {
                        Navigator.MoveTo(Me.CurrentTarget.Location);
                        return RunStatus.Failure;
                    });
                }
                else if (Me.IsMoving && Me.CurrentTarget.IsWithinMeleeRange)
                {
                    return new Action(delegate
                    {
                        Navigator.PlayerMover.MoveStop();
                        CreateAutoAttack();
                        return RunStatus.Failure;
                    });
                }
            }
            return new Action(delegate
            {
                return RunStatus.Failure;
            });
        }

        public Composite MoveToTargetProper2()
        {
            if (!DunatanksSettings.Instance.DisableMovement && Me.CurrentTarget != null && !Me.Mounted && Me.Combat)
            {
                if (Me.CurrentTarget.InLineOfSight || !Me.CurrentTarget.IsWithinMeleeRange)
                {
                    return new Action(delegate
                    {
                        Navigator.MoveTo(Me.CurrentTarget.Location);
                        return RunStatus.Success;
                    });
                }
                else if (Me.IsMoving && Me.CurrentTarget.IsWithinMeleeRange)
                {
                    return new Action(delegate
                    {
                        Navigator.PlayerMover.MoveStop();
                        CreateAutoAttack();
                        return RunStatus.Success;
                    });
                }
            }
            return new Action(delegate
            {
                return RunStatus.Failure;
            });
        }


        public Composite movetobeta()
        {
            return new Decorator(
                ret => !DunatanksSettings.Instance.DisableMovement && Me.CurrentTarget != null && !Me.CurrentTarget.IsWithinMeleeRange,
                new Action(ret => Navigator.MoveTo(Me.CurrentTarget.Location
                    )));
        }

        public Composite stopmoving()
        {
            return new Decorator(
                ret => !DunatanksSettings.Instance.DisableMovement && Me.CurrentTarget != null && Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.Distance < 8 && Me.IsMoving,
                new Action(ret => Navigator.PlayerMover.MoveStop()));
        }

        public Composite MTT()
        {
            if (!DunatanksSettings.Instance.DisableMovement && Me.CurrentTarget != null)
            {
                if (Me.CurrentTarget.InLineOfSight || !Me.CurrentTarget.IsWithinMeleeRange)
                {
                    return new Action(delegate
                    {
                        Navigator.MoveTo(Me.CurrentTarget.Location);
                        return RunStatus.Success;
                    });
                }
                else if (Me.IsMoving && Me.CurrentTarget.IsWithinMeleeRange)
                {
                    return new Action(delegate
                    {
                        Navigator.PlayerMover.MoveStop();
                        return RunStatus.Success;
                    });
                }
            }
            return new Action(delegate
            {
                return RunStatus.Failure;
            });
        }

        protected Composite CreateMoveToAndFace(float maxRange, float coneDegrees, UnitSelectDelegate unit)
        {
            return CreateMoveToAndFace(maxRange, coneDegrees, unit, false);
        }

        protected Composite CreateMoveToAndFace(float maxRange, UnitSelectDelegate distanceFrom)
        {
            return CreateMoveToAndFace(maxRange, 70, distanceFrom);
        }
        protected Composite CreateMoveToAndFace(UnitSelectDelegate unitToCheck)
        {
            return CreateMoveToAndFace(5f, unitToCheck);
        }

        protected Composite CreateMoveToAndFace()
        {
            return CreateMoveToAndFace(5f, ret => Me.CurrentTarget);
        }

        protected Composite CreateFaceUnit(UnitSelectDelegate unitToCheck)
        {
            return CreateMoveToAndFace(5f, 70, unitToCheck, true);
        }

        protected Composite CreateFaceUnit()
        {
            return CreateFaceUnit(ret => Me.CurrentTarget);
        }
        #endregion
        #endregion
        #region Charge
        public Composite CastCharge(CanRunDecoratorDelegate extra)
        {
            return new Decorator(ret => extra(ret),
                                 new Action(delegate
                                 {
                                     SpellManager.Cast("Charge");
                                     Logging.Write("[DWCC]: Charge.");
                                     Logging.WriteDiagnostic("[DWCC]: Attempting to cast Charge on " + Me.CurrentTarget.Name.Remove(3, Me.CurrentTarget.Name.Length - 3) + "*** @ " + Me.CurrentTarget.CurrentHealth + "/" + Me.CurrentTarget.MaxHealth + " (" + Math.Round(Me.CurrentTarget.HealthPercent, 2) + "%)");
                                     Logging.WriteDiagnostic("[DWCC]: Target: IsCasting: " + Me.CurrentTarget.IsCasting + " | IsPlayer: " + Me.CurrentTarget.IsPlayer + " | Distance: " + Math.Round(Me.CurrentTarget.Distance, 2) + " | Level: " + Me.CurrentTarget.Level + " | IsElite: " + Me.CurrentTarget.Elite + " | Adds: " + detectAdds().Count);
                                     Logging.WriteDiagnostic("[DWCC]: We are in: " + Me.ZoneText + " | Instance: " + Me.IsInInstance + " | Outdoors: " + Me.IsOutdoors + " | Battleground: " + Styx.WoWInternals.Battlegrounds.IsInsideBattleground + " | Indoors: " + Me.IsIndoors + " | Party: " + Me.GroupInfo.IsInParty + " | Raid: " + Me.GroupInfo.IsInRaid + " | Members: " + Me.PartyMembers.Count + "/" + Me.RaidMembers.Count + " | Health: " + Me.CurrentHealth + "/" + Me.MaxHealth + " (" + Math.Round(Me.HealthPercent, 2) + "%) | BattleStance: " + BattleStance + " | DefStance: " + DefStance + " | BerserkerStance: " + BersiStance);
                                     MoveToTarget();
                                 }
                                     ));
        }
        #endregion
        #region Heroic Leap

        public delegate WoWUnit UnitSelection(object context);

        public static Composite HeroicLeap(CanRunDecoratorDelegate cond)
        {
            return new Decorator(ret => Me.CurrentTarget != null && SpellManager.CanCast("Heroic Leap") && !Me.CurrentTarget.HasAura("Charge Stun") && Me.CurrentTarget.Attackable && cond(ret) && !Me.CurrentTarget.IsFlying && Me.CurrentTarget.Distance < 40 && Me.CurrentTarget.Distance >= 8,
            new Sequence(
                new Action(a => Logging.Write("[DWCC]: Heroic Leap.")),
                new Action(a => SpellManager.Cast("Heroic Leap")),
                new Action(ret => SpellManager.ClickRemoteLocation(Me.CurrentTarget.Location))));
        }
        #endregion
        #region FaceTarget
        public Composite FaceTarget(Styx.TreeSharp.CanRunDecoratorDelegate extra)
        {
            return new Decorator(ret => extra(ret) && (!Me.IsFacing(Me.CurrentTarget) || !Me.IsSafelyFacing(Me.CurrentTarget, 5f)),
                                 new Action(delegate
                                 {
                                     Me.CurrentTarget.Face();
                                 }
                                     ));
        }
        #endregion
        #region AutoTankTarget
        public void AutoTankTargeting()
        {
            if (DunatanksSettings.Instance.useProt == true && NeedTankTargeting == false && DunatanksSettings.Instance.useAutoTargetProt == true)
            {
                NeedTankTargeting = true;
            }
            else if (DunatanksSettings.Instance.useProt == true && DunatanksSettings.Instance.useAutoTargetProt == false)
            {
                NeedTankTargeting = false;
            }
        }
        #endregion
        #region StanceCheck
        public bool StanceCheck()
        {
            if (DunatanksSettings.Instance.useProt && DunatanksSettings.Instance.AutoSwitchDefensiveStance == true)
            {
                if (!Me.HasAura("Defensive Stance")) //If we don't have Defensive Stance active, we will switch
                {
                    if (SpellManager.Cast("Defensive Stance") == true)
                    {
                        Logging.Write("[DWCC]: Defensive Stance.");
                    }
                    return true;
                }
                return false;
            }
            else if (DunatanksSettings.Instance.useFury && DunatanksSettings.Instance.AutoSwitchBerserkerStance == true)
            {
                if (!Me.HasAura("Battle Stance")) //If we don't have Berserker Stance active, we will switch
                {
                    if (SpellManager.Cast("Battle Stance") == true)
                    {
                        Logging.Write("[DWCC]: Battle Stance.");
                    }
                    return true;
                }
                return false;
            }
            else if (DunatanksSettings.Instance.useArms && DunatanksSettings.Instance.AutoSwitchBattleStance == true)
            {
                if (!Me.HasAura("Battle Stance")) //If we don't have Battle Stance active, we will switch
                {
                    if (SpellManager.Cast("Battle Stance") == true)
                    {
                        Logging.Write("[DWCC]: Battle Stance.");
                    }
                    return true;
                }
                return false;
            }
            return false;
        }
        #endregion
        #region Vigilance
        public string playername;
        public WoWUnit bufftarget;
        public bool Vigilance()
        {
            if (DunatanksSettings.Instance.useProt == true)
            {
                if (DunatanksSettings.Instance.useVigilanceProt == true)
                {
                    if (Me.GroupInfo.IsInParty || Me.GroupInfo.IsInRaid) //Trying to buff Vigilance on partymember #2
                    {
                        if (DunatanksSettings.Instance.useVigilanceOnRandom)
                        {
                            if (Me.PartyMembers.FirstOrDefault() != null)
                            {
                                if (!Me.PartyMembers.FirstOrDefault().HasAura("Vigilance") && Me.PartyMembers.FirstOrDefault().IsDead == false && Me.PartyMembers.FirstOrDefault().Distance <= 20 && !Me.Mounted)
                                {
                                    if (SpellManager.CanBuff("Vigilance", Me.PartyMembers.FirstOrDefault()) == true)
                                    {
                                        if (SpellManager.Buff("Vigilance", Me.PartyMembers.FirstOrDefault()) == true)
                                        {
                                            Logging.Write("[DWCC]: Vigilance on " + Me.PartyMembers.FirstOrDefault().Name);
                                        }
                                        return false;
                                    }
                                }
                                return true;
                            }
                            else
                            {
                                if (Me.PartyMembers.FirstOrDefault() != null && Me.PartyMembers.FirstOrDefault().HasAura("Vigilance") && Me.PartyMembers.FirstOrDefault().IsDead == false && Me.PartyMembers.FirstOrDefault().Distance <= 20 && !Me.Mounted)
                                {
                                    if (SpellManager.CanBuff("Vigilance", Me.PartyMembers.FirstOrDefault()) == true)
                                    {
                                        if (SpellManager.Buff("Vigilance", Me.PartyMembers.FirstOrDefault()) == true)
                                        {
                                            Logging.Write("[DWCC]: Vigilance on " + Me.PartyMembers.FirstOrDefault().Name);
                                        }
                                        return false;
                                    }
                                }
                                return true;
                            }
                        }
                        else
                        {
                            playername = DunatanksSettings.Instance.VigilanceSpecificName;
                            if (playername != "")
                            {
                                bufftarget = ObjectManager.GetObjectsOfType<WoWPlayer>().Where(Player => Player.Name == playername).FirstOrDefault();
                                if (bufftarget != null)
                                {
                                    if (bufftarget.IsFriendly && bufftarget.IsPlayer)
                                    {
                                        if (!bufftarget.HasAura("Vigilance") && bufftarget.IsDead == false && bufftarget.Distance <= 20 && !Me.Mounted)
                                        {
                                            if (SpellManager.CanBuff("Vigilance", bufftarget))
                                            {
                                                if (SpellManager.Buff("Vigilance", bufftarget) == true)
                                                {
                                                    Logging.Write("[DWCC]: Vigilance on " + playername);
                                                }
                                                return false;
                                            }
                                            return true;
                                        }
                                        return false;
                                    }
                                    return false;
                                }
                                else
                                {
                                    Logging.Write("WARNING! Your desired --> Vigilance <-- target \"" + playername + "\" could not be found! Are you sure it is correct?");
                                }
                            }

                            else
                            {
                                Logging.Write("WARNING: You did not define a target to buff Vigilance on! Can't buff Vigilance!");
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            return true;
        }
        #endregion
        #region PvPCC
        public bool IsPvPCrowdControlled(WoWUnit unit)
        {
            return Me.GetAllAuras().Any(
                a => a.IsHarmful &&
                     (a.Spell.Mechanic == WoWSpellMechanic.Shackled ||
                      a.Spell.Mechanic == WoWSpellMechanic.Polymorphed ||
                      a.Spell.Mechanic == WoWSpellMechanic.Horrified ||
                      a.Spell.Mechanic == WoWSpellMechanic.Rooted ||
                      a.Spell.Mechanic == WoWSpellMechanic.Frozen ||
                      a.Spell.Mechanic == WoWSpellMechanic.Stunned ||
                      a.Spell.Mechanic == WoWSpellMechanic.Fleeing ||
                      a.Spell.Mechanic == WoWSpellMechanic.Banished ||
                      a.Spell.Mechanic == WoWSpellMechanic.Sapped
                      ));
        }
        #endregion
        #region StoneFormCheck
        public bool StoneFormCheck(WoWUnit unit)
        {
            return Me.GetAllAuras().Any(
                a => a.IsHarmful &&
                     (a.Spell.Mechanic == WoWSpellMechanic.Bleeding ||
                      a.Spell.Mechanic == WoWSpellMechanic.Infected
                      ));
        }
        #endregion
        #region EscapeArtistCheck
        public bool EscapeArtistCheck(WoWUnit unit)
        {
            return Me.GetAllAuras().Any(
                a => a.IsHarmful &&
                     (a.Spell.Mechanic == WoWSpellMechanic.Rooted ||
                      a.Spell.Mechanic == WoWSpellMechanic.Slowed
                      ));
        }
        #endregion
        #region ForsakenArtistCheck
        public bool ForsakenCheck(WoWUnit unit)
        {
            return Me.GetAllAuras().Any(
                a => a.IsHarmful &&
                     (a.Spell.Mechanic == WoWSpellMechanic.Charmed ||
                      a.Spell.Mechanic == WoWSpellMechanic.Horrified ||
                      a.Spell.Mechanic == WoWSpellMechanic.Asleep
                      ));
        }
        #endregion
        #region FleeingCheck
        public bool IsFleeing(WoWUnit unit)
        {
            return Me.CurrentTarget.GetAllAuras().Any(
                a => a.Spell.Mechanic == WoWSpellMechanic.Fleeing
                );
        }
        #endregion
        #region Trinkets
        public bool CheckTrinketOne()
        {
            if (Me.Inventory.Equipped.Trinket1 != null)
            {
                if (StyxWoW.Me.Inventory.Equipped.Trinket1.Usable == true)
                {
                    WoWItem TrinketOne = StyxWoW.Me.Inventory.Equipped.Trinket1;
                    //Logging.Write(StyxWoW.Me.Inventory.Equipped.Trinket1.Name + "check 2");
                    return true;
                }
                //Logging.Write(StyxWoW.Me.Inventory.Equipped.Trinket1.Name + "check 1");
                return false;
            }
            return false;
        }

        WoWItem TrinketOne = null;

        public Composite UseTrinketOne()
        {
            return new Decorator(ret => CheckTrinketOne() && StyxWoW.Me.Inventory.Equipped.Trinket1.Cooldown == 0 && (DunatanksSettings.Instance.UseTrinketOneOnCd) || (DunatanksSettings.Instance.UseTrinketOneBelow20 && Me.CurrentTarget.HealthPercent < 20) || (IsPvPCrowdControlled(Me) && DunatanksSettings.Instance.useTrinketOneCC) || ((Me.HasAura("Bloodlust") || Me.HasAura("Heroism") || Me.HasAura("Time Warp")) && DunatanksSettings.Instance.UseTrinketOneHero),
                                 new Action(delegate
                                 {
                                     StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                                     Logging.Write("[DWCC]: Using " + TrinketOne.Name + " <--");
                                 }
                                     ));
        }

        WoWItem TrinketTwo;

        public bool CheckTrinketTwo()
        {
            if (Me.Inventory.Equipped.Trinket2 != null)
            {
                if (StyxWoW.Me.Inventory.Equipped.Trinket2.Usable == true)
                {
                    WoWItem TrinketTwo = StyxWoW.Me.Inventory.Equipped.Trinket2;
                    //Logging.Write(TrinketTwo.Name + "check 2");
                    return true;
                }
                //Logging.Write(StyxWoW.Me.Inventory.Equipped.Trinket2.Name + "check 1");
                return false;
            }
            return false;
        }



        public Composite UseTrinketTwo()
        {

            return new Decorator(ret => CheckTrinketTwo() && StyxWoW.Me.Inventory.Equipped.Trinket2.Cooldown == 0 && (DunatanksSettings.Instance.UseTrinketTwoOnCd) || (DunatanksSettings.Instance.UseTrinketTwoBelow20 && Me.CurrentTarget.HealthPercent < 20) || (IsPvPCrowdControlled(Me) && DunatanksSettings.Instance.useTrinketTwoCC) || ((Me.HasAura("Bloodlust") || Me.HasAura("Heroism") || Me.HasAura("Time Warp")) && DunatanksSettings.Instance.UseTrinketTwoHero),
                                 new Action(delegate
                                 {
                                     StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                                     Logging.Write("[DWCC]: Using " + TrinketTwo.Name + " <--");
                                 }
                                     ));
        }

        public bool TrinketOneReady()
        {
            if (StyxWoW.Me.Inventory.GetItemBySlot(12) != null && StyxWoW.Me.Inventory.GetItemBySlot(12).BaseAddress != null)
            {
                if (StyxWoW.Me.Inventory.GetItemBySlot(12).Cooldown == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool TrinketTwoReady()
        {
            if (StyxWoW.Me.Inventory.GetItemBySlot(13) != null && StyxWoW.Me.Inventory.GetItemBySlot(13).BaseAddress != null)
            {
                if (StyxWoW.Me.Inventory.GetItemBySlot(13).Cooldown == 0)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
        #region AoEPummel
        public WoWUnit AoEPummelLastTarget = null;

        public Composite AoEPummel()
        {
            SetCurrentTarget();
            return new Decorator(ret => DunatanksSettings.Instance.usePummelAoEAuto && SpellManager.CanCast("Pummel", (WoWUnit)AoECastingAdds().FirstOrDefault()) && (WoWUnit)AoECastingAdds().FirstOrDefault() != null,
                                 new Sequence(
                                     TargetAoEPummel(ret => Me.CurrentTarget != ((WoWUnit)AoECastingAdds().FirstOrDefault()) && (WoWUnit)AoECastingAdds().FirstOrDefault() != null),
                                     FaceAoEPummel(ret => !Me.IsFacing((WoWUnit)AoECastingAdds().FirstOrDefault()) && (WoWUnit)AoECastingAdds().FirstOrDefault() != null),
                                     new Action(ret => SpellManager.Cast("Pummel", (WoWUnit)AoECastingAdds().FirstOrDefault())),
                                     new Action(ret => AoEPummelLastTarget.Target()),
                                     new Action(ret => AoEPummelLastTarget.Face())));
        }

        public void SetCurrentTarget()
        {
            AoEPummelLastTarget = Me.CurrentTarget;
        }

        public Composite FaceAoEPummel(CanRunDecoratorDelegate extra)
        {
            return new Decorator(ret => extra(ret) && !Me.IsFacing((WoWUnit)AoECastingAdds().FirstOrDefault()),
                                 new Action(delegate
                                 {
                                     ((WoWUnit)AoECastingAdds().FirstOrDefault()).Face();
                                     Logging.Write("[DWCC]: Facing AoE Pummel target.");
                                 }
                                     ));
        }

        public Composite TargetAoEPummel(CanRunDecoratorDelegate extra)
        {
            return new Decorator(ret => extra(ret) && Me.CurrentTarget != ((WoWUnit)AoECastingAdds().FirstOrDefault()),
                                 new Action(delegate
                                 {
                                     ((WoWUnit)AoECastingAdds().FirstOrDefault()).Target();
                                     Logging.Write("[DWCC]: Targeting AoE Pummel target.");
                                 }
                                     ));
        }
        #endregion
        #region AoECastingAdds
        private List<WoWUnit> AoECastingAdds()
        {
                List<WoWUnit> addList = ObjectManager.GetObjectsOfType<WoWUnit>(false).FindAll(unit =>
                            unit.Guid != Me.Guid &&
                            unit.Distance < DunatanksSettings.Instance.CombatDistance &&
                            unit.IsAlive &&
                            unit.Combat &&
                            unit.IsCasting &&
                            !unit.IsFriendly &&
                            !unit.IsPet &&
                            !Styx.CommonBot.Blacklist.Contains(unit.Guid));

                return addList;
        }
        #endregion
        #region HealthRegen
        public static ulong LastTarget;
        public static ulong LastTargetHPPot;
        WoWItem CurrentHealthPotion;
        public bool HaveHealthPotion()
        {
            //whole idea is to make sure CurrentHealthPotion is not null, and to check once every battle. 
            if (CurrentHealthPotion == null)
            {
                if (LastTargetHPPot == null || Me.CurrentTarget.Guid != LastTargetHPPot) //Meaning they are not the same. 
                {
                    LastTarget = Me.CurrentTarget.Guid; // set guid to current target. 
                    List<WoWItem> HPPot =
                    (from obj in
                         Me.BagItems.Where(
                             ret => ret != null && ret.BaseAddress != null &&
                             (ret.ItemInfo.ItemClass == WoWItemClass.Consumable) &&
                             (ret.ItemInfo.ContainerClass == WoWItemContainerClass.Potion) &&
                             (ret.ItemSpells[0].ActualSpell.SpellEffect1.EffectType == WoWSpellEffectType.Heal))
                     select obj).ToList();
                    if (HPPot.Count > 0)
                    {

                        //on first check, set CurrentHealthPotion so we dont keep running the list looking for one, 
                        CurrentHealthPotion = HPPot.FirstOrDefault();
                        Logging.Write("Potion Found {0}", HPPot.FirstOrDefault().Name);
                        return true;

                    }
                }


                return false;
            }
            else
            {
                return true;
            }
        }
        public bool HealthPotionReady()
        {
            if (CurrentHealthPotion != null && CurrentHealthPotion.BaseAddress != null)
            {
                if (CurrentHealthPotion.Cooldown == 0)
                {
                    return true;
                }
            }
            return false;
        }
        public void UseHealthPotion()
        {
            if (CurrentHealthPotion != null && CurrentHealthPotion.BaseAddress != null)
            {
                if (CurrentHealthPotion.Cooldown == 0)
                {
                    Logging.Write("[DWCC]: Low HP! Using --> {0} <--!", CurrentHealthPotion.Name.ToString());
                    CurrentHealthPotion.Use();
                }
            }
        }

        // credits to CodenameG
        public static WoWItem HealthStone;
        public static bool HaveHealthStone()
        {

            if (HealthStone == null)
            {
                foreach (WoWItem item in Me.BagItems)
                {
                    if (item.Entry == 5512)
                    {
                        HealthStone = item;
                        return true;
                    }

                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool HealthStoneNotCooldown()
        {
            if (HealthStone != null && HealthStone.BaseAddress != null)
            {

                if (HealthStone.Cooldown == 0)
                {
                    return true;
                }

            }
            return false;
        }
        public static void UseHealthStone()
        {
            if (HealthStone != null && HealthStoneNotCooldown())
            {
                Logging.Write("[DWCC]: Swallowing the green pill! Using --> Healthstone <--");
                HealthStone.Use();
            }
        }
        #endregion
        #region LifeSpirit
        public bool LifeSpiritRegen()
        {
            if ((Me.CurrentHealth <= (Me.MaxHealth - 60000)) && HaveLifeSpirit() && LifeSpiritNotCooldown() && !Me.Mounted && !Me.OnTaxi && !Me.IsFlying)
            {
                UseLifeSpirit();
                return true;
            }
            return false;
        }

        public static WoWItem LifeSpirit;
        public static bool HaveLifeSpirit()
        {

            if (LifeSpirit == null)
            {
                foreach (WoWItem item in Me.BagItems)
                {
                    if (item.Entry == 89640)
                    {
                        LifeSpirit = item;
                        return true;
                    }

                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool LifeSpiritNotCooldown()
        {
            if (LifeSpirit != null && LifeSpirit.BaseAddress != null)
            {

                if (LifeSpirit.Cooldown == 0)
                {
                    return true;
                }

            }
            return false;
        }
        public static void UseLifeSpirit()
        {
            if (LifeSpirit != null && LifeSpiritNotCooldown())
            {
                Logging.Write("[DWCC]: Using --> Life Spirit <--");
                LifeSpirit.Use();
            }
        }
        #endregion
        #region Racials
        #region UseRacials
        public bool RacialAbilities()
        {
            if (!Me.Mounted && !Me.OnTaxi && !Me.IsFlying && DunatanksSettings.Instance.UseRacials)
            {
                #region Alliance
                HumanRacial();
                NightElfRacial();
                DwarfRacial();
                GnomeRacial();
                WorgenRacial();
                DraeneiRacial();
                #endregion

                #region Horde
                OrcRacial();
                TaurenRacial();
                UndeadRacial();
                TrollRacial();
                GoblinRacial();
                BloodElfRacial();
                #endregion
                return true;
            }
            return false;
        }
        #endregion
        #region Alliance
        #region Human
        public bool HumanRacial()
        {
            if (Me.Race == WoWRace.Human && IsPvPCrowdControlled(Me) && SpellManager.HasSpell("Every Man for Himself") && !SpellManager.Spells["Every Man for Himself"].Cooldown && SpellManager.CanCast("Every Man for Himself"))
            {
                SpellManager.Cast("Every Man for Himself");
                return true;
            }
            return false;
        }
        #endregion
        #region NightElf
        public bool NightElfRacial()
        {
            if (Me.Race == WoWRace.NightElf && Me.IsInMyPartyOrRaid && (DunatanksSettings.Instance.useArms || DunatanksSettings.Instance.useFury) && Me.CurrentTarget.Aggro && SpellManager.HasSpell("Shadowmeld") && !SpellManager.Spells["Shadowmeld"].Cooldown && SpellManager.CanCast("Shadowmeld"))
            {
                SpellManager.Cast("Shadowmeld");
                return true;
            }
            return false;
        }
        #endregion
        #region Dwarf
        public bool DwarfRacial()
        {
            if (Me.Race == WoWRace.Dwarf && StoneFormCheck(Me) && SpellManager.HasSpell("Stoneform") && !SpellManager.Spells["Stoneform"].Cooldown && SpellManager.CanCast("Stoneform"))
            {
                SpellManager.Cast("Stoneform");
                return true;
            }
            return false;
        }
        #endregion
        #region Gnome
        public bool GnomeRacial()
        {
            if (Me.Race == WoWRace.Gnome && EscapeArtistCheck(Me) && SpellManager.HasSpell("Escape Artist") && !SpellManager.Spells["Escape Artist"].Cooldown && SpellManager.CanCast("Escape Artist"))
            {
                SpellManager.Cast("Escape Artist");
                return true;
            }
            return false;
        }
        #endregion
        #region Worgen
        public bool WorgenRacial()
        {
            if (Me.Race == WoWRace.Worgen && (Me.CurrentTarget.Distance > 35 || (Me.CurrentTarget.Distance > 20 && IsFleeing(Me.CurrentTarget))) && SpellManager.HasSpell("Darkflight") && !SpellManager.Spells["Darkflight"].Cooldown && SpellManager.CanCast("Darkflight"))
            {
                SpellManager.Cast("Darkflight");
                return true;
            }
            return false;
        }
        #endregion
        #region Draenei
        public bool DraeneiRacial()
        {
            if (Me.Race == WoWRace.Draenei && Me.HealthPercent < 70 && SpellManager.HasSpell("Gift of the Naaru") && !SpellManager.Spells["Gift of the Naaru"].Cooldown && SpellManager.CanCast("Gift of the Naaru"))
            {
                SpellManager.Cast("Gift of the Naaru");
                return true;
            }
            return false;
        }
        #endregion
        #endregion
        #region Horde
        #region Orc
        public bool OrcRacial()
        {
            if (Me.Race == WoWRace.Orc && Me.Combat && SpellManager.HasSpell("Blood Fury") && !SpellManager.Spells["Blood Fury"].Cooldown && SpellManager.CanCast("Blood Fury"))
            {
                SpellManager.Cast("Blood Fury");
                return true;
            }
            return false;
        }
        #endregion
        #region Tauren
        public bool TaurenRacial()
        {
            if (Me.Race == WoWRace.Tauren && Me.Combat && AoECastingAdds().Count > 0 && SpellManager.HasSpell("War Stomp") && !SpellManager.Spells["War Stomp"].Cooldown && SpellManager.CanCast("War Stomp"))
            {
                SpellManager.Cast("War Stomp");
                return true;
            }
            return false;
        }
        #endregion
        #region Undead
        public bool UndeadRacial()
        {
            if (Me.Race == WoWRace.Undead && ForsakenCheck(Me) && SpellManager.HasSpell("Will of the Forsaken") && !SpellManager.Spells["Will of the Forsaken"].Cooldown && SpellManager.CanCast("Will of the Forsaken"))
            {
                SpellManager.Cast("Will of the Forsaken");
                return true;
            }
            return false;
        }
        #endregion
        #region Troll
        public bool TrollRacial()
        {
            if (Me.Race == WoWRace.Troll && Me.Combat && SpellManager.HasSpell("Berserking") && !SpellManager.Spells["Berserking"].Cooldown && SpellManager.CanCast("Berserking"))
            {
                SpellManager.Cast("Berserking");
                return true;
            }
            return false;
        }
        #endregion
        #region Goblin
        public bool GoblinRacial()
        {
            if (Me.Race == WoWRace.Goblin && Me.Combat && Me.CurrentTarget.Attackable && Me.CurrentTarget.IsHostile && SpellManager.HasSpell("Rocket Barrage") && !SpellManager.Spells["Rocket Barrage"].Cooldown && SpellManager.CanCast("Rocket Barrage"))
            {
                SpellManager.Cast("Rocket Barrage", Me.CurrentTarget);
                return true;
            }
            return false;
        }
        #endregion
        #region BloodElf
        public bool BloodElfRacial()
        {
            if (Me.Race == WoWRace.BloodElf && Me.Combat && AoECastingAdds().Count > 0 && SpellManager.HasSpell("Arcane Torrent") && !SpellManager.Spells["Arcane Torrent"].Cooldown && SpellManager.CanCast("Arcane Torrent"))
            {
                SpellManager.Cast("Arcane Torrent");
                return true;
            }
            return false;
        }
        #endregion
        #endregion
        #region Neutral
        #region Pandaren
        // Currently, there are no advantages for using Pandaren racial skills
        #endregion
        #endregion
        #endregion
        #region AreaSpells
        #region Bool
        public bool AreaSpells()
        {
            if (StyxWoW.Me.HasAura(130774) ||
        StyxWoW.Me.HasAura(116040) ||
        StyxWoW.Me.HasAura(116583) ||
        StyxWoW.Me.HasAura(116586) ||
        StyxWoW.Me.HasAura(116924) ||
        StyxWoW.Me.HasAura(119610) ||
        StyxWoW.Me.HasAura(13810) ||
        StyxWoW.Me.HasAura(43265))
            {
                Logging.WriteDiagnostic("[DWCC]: Standing in AoE: true.");
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        #region FindHealers
        public List<WoWPlayer> Healers
        {
            get
            {
                if (!StyxWoW.Me.GroupInfo.IsInParty)
                    return new List<WoWPlayer>(); ;

                return StyxWoW.Me.GroupInfo.RaidMembers.Where(p => p.HasRole(WoWPartyMember.GroupRole.Healer))
                    .Select(p => p.ToPlayer())
                    .Where(p => p != null && p.IsAlive && p.IsFriendly && (!p.HasAura(130774) || !p.HasAura(116040) || !p.HasAura(116583) || !p.HasAura(116586) || !p.HasAura(116924) || !p.HasAura(119610) || !p.HasAura(13810) || !p.HasAura(43265)) && Navigator.CanNavigateFully(Me.Location, p.Location)).ToList();
            }
        }
        #endregion
        #region MoveOut
        public Composite MoveOutOfAoE()
        {
            return new Decorator(ret => AreaSpells() && !DunatanksSettings.Instance.DisableMovement,
                                 new Action(delegate
                                 {
                                     Logging.Write("[DWCC]: Moving out of AoE.");
                                     //Logging.Write("[DWCC]: Healer: " + Healers.FirstOrDefault().ToString() + " | CanNav: " + Navigator.CanNavigateFully(Me.Location, Healers.FirstOrDefault().Location));
                                     Navigator.PlayerMover.MoveTowards(Healers.FirstOrDefault().Location);
                                 }
                                ));
        }

        public Composite StopAoEMovement()
        {
            {
                return new Decorator(ret => !AreaSpells() && !DunatanksSettings.Instance.DisableMovement && MoveOutOfAoE().IsRunning && Me.IsMoving,
                                     new Action(delegate
                                     {
                                         Navigator.PlayerMover.MoveStop();
                                     }
                                    ));
            }
        }
        #endregion
        #endregion
    }
}