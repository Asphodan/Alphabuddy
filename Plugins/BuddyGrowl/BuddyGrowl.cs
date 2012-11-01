//!CompilerOption:AddRef:System.Design.dll
using System;
using System.Text;
using System.Windows.Media;

using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.Plugins;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System.Diagnostics;
using BuddyGrowl;
using System.Collections.Generic;
using System.Net;

namespace BuddyGrowl
{
    class BuddyGrowl : HBPlugin
    {
        public override string Name { get { return "BuddyGrowl"; } }
        public override string Author { get { return "Jimmy06"; } }
        public override Version Version { get { return new Version(1, 5, 0, 0); } }

        public override bool WantButton { get { return true; } }
        public override string ButtonText { get { return "Settings"; } }

        private static LocalPlayer intMe = StyxWoW.Me;
        private BuddyGrowlForm mainForm;

        public static bool Whisper = false;
        public static bool Officer = false;
        public static bool Battleground = false;
        public static bool Guild = false;
        public static bool Party = false;
        public static bool Yell = false;
        public static bool Raid = false;
        public static bool Say = false;
        public static bool Death = false;
        public static string trigger = null;
        public static string growlpath = null;

        public BuddyGrowl()
        {
            mainForm = new BuddyGrowlForm();

            Chat.Say += Chat_Say;
            Chat.Yell += Chat_Yell;
            Chat.Whisper += Chat_Whisper;
            Chat.Party += Chat_Party;
            Chat.PartyLeader += Chat_Party;
            Chat.Guild += Chat_Guild;
            Chat.Emote += Chat_Emote;
            Chat.Battleground += Chat_BG;
            Chat.BattlegroundLeader += Chat_BG;
            Chat.Raid += Chat_Raid;
            Chat.RaidLeader += Chat_Raid;
            Chat.Officer += Chat_Officer;

            var settings = BuddyGrowlSettings.Instance;

            BuddyGrowl.Whisper = settings.Whisper;
            BuddyGrowl.Officer = settings.Officer;
            BuddyGrowl.Battleground = settings.Battleground;
            BuddyGrowl.Guild = settings.Guild;
            BuddyGrowl.Party = settings.Party;
            BuddyGrowl.Yell = settings.Yell;
            BuddyGrowl.Raid = settings.Raid;
            BuddyGrowl.Death = settings.Death;
            BuddyGrowl.trigger = settings.trigger;
            BuddyGrowl.growlpath = settings.growlpath;

            Lua.Events.AttachEvent("PLAYER_LEVEL_UP", Level);
            Lua.Events.AttachEvent("CHAT_MSG_BN_WHISPER", BNET);
            Lua.Events.AttachEvent("PLAYER_DEAD", DEATH);

            Logging.Write("Loaded " + Name + " v" + Version.ToString() + " by " + Author);
            
        }

        public override void Pulse()
        {
            if (!StyxWoW.IsInGame)
            {
                Logging.WriteVerbose(string.Format("[BuddyGrowl] Sent Disconnect Growl"));
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = growlpath + @"\growlnotify.exe";
                startInfo.Arguments = "/t:\"" + Styx.StyxWoW.Me.Name + "\" /id:12345 /s:false /p:0 /c:ABCDEF /a:\"BuddyGrowl\" /ai:\"c:\\myapp.png\" " +
                    "/r:\"BuddyGrowl\" /n:\"BuddyGrowl\" /host:localhost /port:23053 " +
                    "/enc:DES /hash:SHA256 /silent:true \"You have been Disconnected \"";

                Process.Start(startInfo);
                return;
            }
        }
        
        public override void OnButtonPress()
        {
            mainForm.ShowDialog();
        }

        public override void Dispose()
        {
            base.Dispose();

            Chat.Say -= Chat_Say;
            Chat.Yell -= Chat_Yell;
            Chat.Whisper -= Chat_Whisper;
            Chat.Party -= Chat_Party;
            Chat.PartyLeader -= Chat_Party;
            Chat.Guild -= Chat_Guild;
            Chat.Emote -= Chat_Emote;
            Chat.Battleground -= Chat_BG;
            Chat.BattlegroundLeader -= Chat_BG;
            Chat.Raid -= Chat_Raid;
            Chat.RaidLeader -= Chat_Raid;
            Chat.Officer -= Chat_Officer;

            Lua.Events.DetachEvent("PLAYER_LEVEL_UP", Level);
            Lua.Events.DetachEvent("CHAT_MSG_BN_WHISPER", BNET);
            Lua.Events.DetachEvent("PLAYER_DEAD", DEATH);

            Logging.Write("Stopped " + Name + " v" + Version.ToString() + " by " + Author);
        }

        #region LUA

        private void Level(object sender, LuaEventArgs args)
        {
            Logging.WriteVerbose(string.Format("[BuddyGrowl]: Level: {0}", args.Args[0]));
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = growlpath + @"\growlnotify.exe";
            startInfo.Arguments = "/t:\"" + Styx.StyxWoW.Me.Name + "\" /id:12345 /s:false /p:0 /c:ABCDEF /a:\"BuddyGrowl\" /ai:\"c:\\myapp.png\" " +
                "/r:\"BuddyGrowl\" /n:\"BuddyGrowl\" /host:localhost /port:23053 " +
                "/enc:DES /hash:SHA256 /silent:true \"Level Up: " + args.Args[0] + "\"";

            Process.Start(startInfo);
        }

        private void DEATH(object sender, LuaEventArgs args)
        {
            if (Death)
            {
                Logging.WriteVerbose("[BuddyGrowl] Sent Death Growl");

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = growlpath + @"\growlnotify.exe";
                startInfo.Arguments = "/t:\"" + Styx.StyxWoW.Me.Name + "\" /id:12345 /s:false /p:0 /c:ABCDEF /a:\"BuddyGrowl\" /ai:\"c:\\myapp.png\" " +
                    "/r:\"BuddyGrowl\" /n:\"BuddyGrowl\" /host:localhost /port:23053 " +
                    "/enc:DES /hash:SHA256 /silent:true \"I Died Count: " + GameStats.Deaths + " Per Hour: " + GameStats.DeathsPerHour + " \"";

                Process.Start(startInfo);
            }
        }

        private void BNET(object sender, LuaEventArgs args)
        {
            object[] Args = args.Args;
            string message = Args[0].ToString();
            string presenceId = Args[12].ToString();
            string senderName = Lua.GetReturnVal<string>(string.Format("return BNGetFriendInfoByID({0})", presenceId), 4);
            Logging.WriteVerbose(string.Format("[BuddyGrowl]: [BNET] [{0}]: {1}", senderName, message));
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = growlpath + @"\growlnotify.exe";
            startInfo.Arguments = "/t:\"Unset\" /id:12345 /s:false /p:0 /c:ABCDEF /a:\"BuddyGrowl\" /ai:\"c:\\myapp.png\" " +
                "/r:\"BuddyGrowl\" /n:\"BuddyGrowl\" /host:localhost /port:23053 " +
                "/enc:DES /hash:SHA256 /silent:true \"[BNET] [" + senderName + "]: " + message + "\"";

            Process.Start(startInfo);
        }

        #endregion LUA

        #region chat

        private void Chatter(string message, string author = "", string type = "")
        {
            if (!String.IsNullOrEmpty(message))
            {
                string myname = Styx.StyxWoW.Me.Name;

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = growlpath + @"\growlnotify.exe";

                if (type == "CHAT_MSG_SAY" && myname != author && Say)
                {
                    startInfo.Arguments = "/t:\"" + myname + "\" /id:12345 /s:false /p:0 /c:ABCDEF /a:\"BuddyGrowl\" /ai:\"c:\\myapp.png\" " +
                        "/r:\"BuddyGrowl\" /n:\"BuddyGrowl\" /host:localhost /port:23053 " +
                        "/enc:DES /hash:SHA256 /silent:true \"[" + author + "] says:" + message + "\"";

                    Process.Start(startInfo);
                }

                if (type == "CHAT_MSG_RAID" && myname != author && Raid || type == "CHAT_MSG_RAID_LEADER" && myname != author && Whisper)
                {
                    startInfo.Arguments = "/t:\"" + myname + "\" /id:12345 /s:false /p:0 /c:ABCDEF /a:\"BuddyGrowl\" /ai:\"c:\\myapp.png\" " +
                        "/r:\"BuddyGrowl\" /n:\"BuddyGrowl\" /host:localhost /port:23053 " +
                        "/enc:DES /hash:SHA256 /silent:true \"[RAID] [" + author + "] says: " + message + "\"";

                    Process.Start(startInfo);
                }

                if (type == "CHAT_MSG_YELL" && myname != author && Yell)
                {
                    startInfo.Arguments = "/t:\"" + myname + "\" /id:12345 /s:false /p:0 /c:ABCDEF /a:\"BuddyGrowl\" /ai:\"c:\\myapp.png\" " +
                        "/r:\"BuddyGrowl\" /n:\"BuddyGrowl\" /host:localhost /port:23053 " +
                        "/enc:DES /hash:SHA256 /silent:true \"[YELL] [" + author + "] says: " + message + "\"";

                    Process.Start(startInfo);
                }

                if (type == "CHAT_MSG_PARTY" && myname != author && Party || type == "CHAT_MSG_PARTY_LEADER" && myname != author && Party)
                {
                    startInfo.Arguments = "/t:\"" + myname + "\" /id:12345 /s:false /p:0 /c:ABCDEF /a:\"BuddyGrowl\" /ai:\"c:\\myapp.png\" " +
                        "/r:\"BuddyGrowl\" /n:\"BuddyGrowl\" /host:localhost /port:23053 " +
                        "/enc:DES /hash:SHA256 /silent:true \"[PARTY] [" + author + "] says: " + message + "\"";

                    Process.Start(startInfo);
                }

                if (type == "CHAT_MSG_GUILD" && myname != author && Guild)
                {
                    startInfo.Arguments = "/t:\"" + myname + "\" /id:12345 /s:false /p:0 /c:ABCDEF /a:\"BuddyGrowl\" /ai:\"c:\\myapp.png\" " +
                        "/r:\"BuddyGrowl\" /n:\"BuddyGrowl\" /host:localhost /port:23053 " +
                        "/enc:DES /hash:SHA256 /silent:true \"[GUILD] [" + author + "] says: " + message + "\"";

                    Process.Start(startInfo);
                }

                if (type == "CHAT_MSG_BATTLEGROUND" && myname != author && Battleground)
                {
                    startInfo.Arguments = "/t:\"" + myname + "\" /id:12345 /s:false /p:0 /c:ABCDEF /a:\"BuddyGrowl\" /ai:\"c:\\myapp.png\" " +
                        "/r:\"BuddyGrowl\" /n:\"BuddyGrowl\" /host:localhost /port:23053 " +
                        "/enc:DES /hash:SHA256 /silent:true \"[BG] [" + author + "] says: " + message + "\"";

                    Process.Start(startInfo);
                }

                if (type == "CHAT_MSG_OFFICER" && Officer && myname != author)
                {
                    startInfo.Arguments = "/t:\"" + myname + "\" /id:12345 /s:false /p:0 /c:ABCDEF /a:\"BuddyGrowl\" /ai:\"c:\\myapp.png\" " +
                        "/r:\"BuddyGrowl\" /n:\"BuddyGrowl\" /host:localhost /port:23053 " +
                        "/enc:DES /hash:SHA256 /silent:true \"[OFFICER] [" + author + "] says: " + message + "\"";

                    Process.Start(startInfo);
                }

                if (type == "CHAT_MSG_WHISPER" && myname != author)
                {
                    if (message.Contains(trigger))
                    {
                        Styx.WoWInternals.Lua.DoString( "GuildInvite(\"" + author +"\")" );

                        startInfo.Arguments = "/t:\"" + myname + "\" /id:12345 /s:false /p:0 /c:ABCDEF /a:\"BuddyGrowl\" /ai:\"c:\\myapp.png\" " +
                            "/r:\"BuddyGrowl\" /n:\"BuddyGrowl\" /host:localhost /port:23053 " +
                            "/enc:DES /hash:SHA256 /silent:true \"[WHISPER+Ginv] [" + author + "] says: " + message + "\"";

                        Process.Start(startInfo);

                    }
                    else
                    {
                        if (Whisper)
                        {
                            startInfo.Arguments = "/t:\"" + myname + "\" /id:12345 /s:false /p:0 /c:ABCDEF /a:\"BuddyGrowl\" /ai:\"c:\\myapp.png\" " +
                                "/r:\"BuddyGrowl\" /n:\"BuddyGrowl\" /host:localhost /port:23053 " +
                                "/enc:DES /hash:SHA256 /silent:true \"[WHISPER] [" + author + "] says: " + message + "\"";

                            Process.Start(startInfo);
                        }
                    }
                }

                if (message.IndexOf(StyxWoW.Me.Name, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
                {
                    startInfo.Arguments = "/t:\"" + myname + "\" /id:12345 /s:false /p:0 /c:ABCDEF /a:\"BuddyGrowl\" /ai:\"c:\\myapp.png\" " +
                        "/r:\"BuddyGrowl\" /n:\"BuddyGrowl\" /host:localhost /port:23053 " +
                        "/enc:DES /hash:SHA256 /silent:true \"[ALERT] [" + author + "] says: " + message + "\"";

                    Process.Start(startInfo);
                }

                Logging.WriteVerbose(string.Format("[BuddyGrowl]: Chat: From: {1}  Message: {0} Type: {2}", message, author, type));
            }
        }

        private void Chat_Raid(Styx.CommonBot.Chat.ChatLanguageSpecificEventArgs e)
        {
            Chatter(e.Message, e.Author, e.EventName);
        }

        private void Chat_BG(Styx.CommonBot.Chat.ChatLanguageSpecificEventArgs e)
        {
            Chatter(e.Message, e.Author, e.EventName);
        }

        private void Chat_Emote(Styx.CommonBot.Chat.ChatAuthoredEventArgs e)
        {
            Chatter(e.Message, e.Author, e.EventName);
        }

        private void Chat_Party(Styx.CommonBot.Chat.ChatLanguageSpecificEventArgs e)
        {
            Chatter(e.Message, e.Author, e.EventName);
        }

        private void Chat_Whisper(Styx.CommonBot.Chat.ChatWhisperEventArgs e)
        {
            Chatter(e.Message, e.Author, e.EventName);
        }

        private void Chat_Yell(Styx.CommonBot.Chat.ChatLanguageSpecificEventArgs e)
        {
            Chatter(e.Message, e.Author, e.EventName);
        }

        private void Chat_Say(Styx.CommonBot.Chat.ChatLanguageSpecificEventArgs e)
        {
            Chatter(e.Message, e.Author, e.EventName);
        }

        private void Chat_Guild(Styx.CommonBot.Chat.ChatGuildEventArgs e)
        {
            Chatter(e.Message, e.Author, e.EventName);
        }

        private void Chat_Officer(Styx.CommonBot.Chat.ChatLanguageSpecificEventArgs e)
        {
            Chatter(e.Message, e.Author, e.EventName);
        }

        private void Chat_BN(Styx.CommonBot.Chat.ChatLanguageSpecificEventArgs e)
        {
            Chatter(e.Message, e.Author, e.EventName);
        }

        #endregion chat

        #region settings

        #endregion settings

    }
}
