﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Routines;
using Styx.WoWInternals.DBC;

namespace Singular
{
    #region Nested type: WoWContextEventArg

    public class WoWContextEventArg : EventArgs
    {
        public readonly WoWContext CurrentContext;
        public readonly WoWContext PreviousContext;

        public WoWContextEventArg(WoWContext currentContext, WoWContext prevContext)
        {
            CurrentContext = currentContext;
            PreviousContext = prevContext;
        }
    }

    #endregion
    partial class SingularRoutine
    {
        public static event EventHandler<WoWContextEventArg> OnWoWContextChanged;
        private static WoWContext _lastContext;

        internal static WoWContext CurrentWoWContext
        {
            get
            {
                if(!StyxWoW.IsInGame)
                    return WoWContext.None;

                Map map = StyxWoW.Me.CurrentMap;

                if (map.IsBattleground || map.IsArena)
                {
                    return WoWContext.Battlegrounds;
                }

                if (map.IsDungeon)
                {
                    return WoWContext.Instances;
                }

                return WoWContext.Normal;
            }
        }

        private bool _contextEventSubscribed;
        private void UpdateContext()
        {
            // Subscribe to the map change event, so we can automatically update the context.
            if(!_contextEventSubscribed)
            {
                // Subscribe to OnBattlegroundEntered. Just 'cause.
                BotEvents.Battleground.OnBattlegroundEntered += e => UpdateContext();
                _contextEventSubscribed = true;
            }

            var current = CurrentWoWContext;

            // Can't update the context when it doesn't exist.
            if (current == WoWContext.None)
                return;


            if(current != _lastContext && OnWoWContextChanged!=null)
            {
                try
                {
                    OnWoWContextChanged(this, new WoWContextEventArg(current, _lastContext));
                }
                catch
                {
                    // Eat any exceptions thrown.
                }
                _lastContext = current;
            }
                
        }


        public static string GetBotName()
        {
            BotBase bot = GetBot();
            string sFoundName = "[null]";

            if (bot != null)
                sFoundName = bot.Name;

            return sFoundName;
        }

        public static BotBase GetBot()
        {
            BotBase bot = null;

            if (TreeRoot.Current != null)
            {
                if (!(TreeRoot.Current is NewMixedMode.MixedModeEx))
                    bot = TreeRoot.Current;
                else
                {
                    NewMixedMode.MixedModeEx mmb = (NewMixedMode.MixedModeEx)TreeRoot.Current;
                    if (mmb != null)
                    {
                        string sPrimary = mmb.PrimaryBot != null ? mmb.PrimaryBot.Name : "[primary null]";
                        string sSecondary = mmb.SecondaryBot != null ? mmb.SecondaryBot.Name : "[secondary null]";
                        if (CurrentWoWContext == WoWContext.Battlegrounds || CurrentWoWContext == WoWContext.Instances)
                            bot = mmb.SecondaryBot;
                        else
                            bot = mmb.PrimaryBot;
                    }
                }
            }

            return bot;
        }

        public static bool IsBotInUse(string botNameContains)
        {
            return GetBotName().ToUpper().Contains(botNameContains.ToUpper());
        }


    }
}
