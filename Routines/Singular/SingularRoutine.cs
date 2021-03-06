﻿#region Revision Info

// This file is part of Singular - A community driven Honorbuddy CC
// $Author: bobby53 $
// $Date: 2012-10-04 04:30:19 +0000 (Do, 04 Okt 2012) $
// $HeadURL: https://subversion.assembla.com/svn/singular_v3/trunk/Singular/SingularRoutine.cs $
// $LastChangedBy: bobby53 $
// $LastChangedDate: 2012-10-04 04:30:19 +0000 (Do, 04 Okt 2012) $
// $LastChangedRevision: 864 $
// $Revision: 864 $

#endregion

using System;
using System.Reflection;
using System.Windows.Forms;
using Singular.GUI;
using Singular.Helpers;
using Singular.Managers;
using Singular.Utilities;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Routines;
using Styx.Helpers;
using Styx.WoWInternals.WoWObjects;

namespace Singular
{
    public partial class SingularRoutine : CombatRoutine
    {
        public SingularRoutine()
        {
            Instance = this;

            // Do this now, so we ensure we update our context when needed.
            BotEvents.Player.OnMapChanged += e =>
                {
                    // Don't run this handler if we're not the current routine!
                    if (RoutineManager.Current.Name != Name)
                        return;

                    // Only ever update the context. All our internal handlers will use the context changed event
                    // so we're not reliant on anything outside of ourselves for updates.
                    UpdateContext();
                };
        }

        public static SingularRoutine Instance { get; private set; }

        public override string Name { get { return "Singular v3"; } }

        public override WoWClass Class { get { return StyxWoW.Me.Class; } }

        public override bool WantButton { get { return true; } }

        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        private static bool IsMounted
        {
            get
            {
                switch (StyxWoW.Me.Shapeshift)
                {
                    case ShapeshiftForm.FlightForm:
                    case ShapeshiftForm.EpicFlightForm:
                        return true;
                }
                return StyxWoW.Me.Mounted;
            }
        }

        public override void OnButtonPress()
        {
            DialogResult dr = new ConfigurationForm().ShowDialog();
            if (dr == DialogResult.OK || dr == DialogResult.Yes)
            {
                Logger.WriteDebug("Settings saved, rebuilding behaviors...");
                Initialize();
            }
        }

        public override void Pulse()
        {
            // No pulsing if we're loading or out of the game.
            if (!StyxWoW.IsInGame || !StyxWoW.IsInWorld)
                return;

            // Update the current context, check if we need to rebuild any behaviors.
            UpdateContext();

            // Double cast shit
            Spell.DoubleCastPreventionDict.RemoveAll(t => DateTime.UtcNow.Subtract(t).TotalMilliseconds >= 2500);

            //Only pulse for classes with pets
            switch (StyxWoW.Me.Class)
            {
                case WoWClass.Hunter:
                case WoWClass.DeathKnight:
                case WoWClass.Warlock:
                case WoWClass.Mage:
                    PetManager.Pulse();
                    break;
            }

            if (HealerManager.NeedHealTargeting)
                HealerManager.Instance.Pulse();

            if (Group.MeIsTank && CurrentWoWContext != WoWContext.Battlegrounds &&
                (Me.GroupInfo.IsInParty || Me.GroupInfo.IsInRaid))
                TankManager.Instance.Pulse();
        }

        public override void Initialize()
        {
            Logger.Write("Starting Singular v" + Assembly.GetExecutingAssembly().GetName().Version);
            Logger.Write("Determining talent spec.");
            try
            {
                TalentManager.Update();
            }
            catch (Exception e)
            {
                StopBot(e.ToString());
            }
            Logger.Write("Current spec is " + TalentManager.CurrentSpec.ToString().CamelToSpaced());

            // Update the current WoWContext, and fire an event for the change.
            UpdateContext();

            // NOTE: Hook these events AFTER the context update.
            OnWoWContextChanged += (orig, ne) =>
                {
                    Logger.Write("Context changed, re-creating behaviors");
                    RebuildBehaviors();
                };
            RoutineManager.Reloaded += (s, e) =>
                {
                    Logger.Write("Routines were reloaded, re-creating behaviors");
                    RebuildBehaviors();
                };

            if (!RebuildBehaviors())
            {
                return;
            }
            Logger.Write("Behaviors created!");

            // When we actually need to use it, we will.
            EventHandlers.Init();
            MountManager.Init();
            //Logger.Write("Combat log event handler started.");

            Instance.RebuildBehaviors();
        }

        private static void StopBot(string reason)
        {
            Logger.Write(reason);
            TreeRoot.Stop();
        }
    }
}