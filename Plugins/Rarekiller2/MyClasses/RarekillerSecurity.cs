//=================================================================
//
//				      Rarekiller - Plugin
//						Autor: katzerle
//			Honorbuddy Plugin - www.thebuddyforum.com
//    Credits to highvoltz, bloodlove, SMcCloud, Lofi, ZapMan 
//                and all the brave Testers
//
//==================================================================
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.Windows.Media;
using System.Net;
using System.Globalization;

using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Plugins;

namespace BadWolf
{
    class RarekillerSecurity
    {
        bool LeftRight = true;
        
        public static LocalPlayer Me = StyxWoW.Me; 

        public void Movearound()
        {
            Logging.Write("Rarekiller Part Keypresser: Move around");
            if (LeftRight)
            {
                //KeyboardManager.PressKey('A');
                WoWMovement.Move(WoWMovement.MovementDirection.TurnLeft);
				Thread.Sleep(100);
				WoWMovement.MoveStop();
                //KeyboardManager.ReleaseKey('A');
                LeftRight = false;
            }
            else
            {
                //KeyboardManager.PressKey('D');
				WoWMovement.Move(WoWMovement.MovementDirection.TurnRight);
                Thread.Sleep(100);
				WoWMovement.MoveStop();
                //KeyboardManager.ReleaseKey('D');
                LeftRight = true;
            }
        }

        public void newWhisper(Chat.ChatWhisperEventArgs arg)
        {
            bool IsGM = Lua.GetReturnVal<bool>("if(_G.GMChatFrame_IsGM and _G.GMChatFrame_IsGM("+ arg.Author + ")) then return true; else return false; end", 0); // from WIM Addon; WIM.lua Z:449 - Needs some Work !!
			if (Rarekiller.Settings.Wisper)
            {
				if (File.Exists(Rarekiller.Settings.SoundfileWisper))
                    new SoundPlayer(Rarekiller.Settings.SoundfileWisper).Play();
                else if (File.Exists(Rarekiller.Soundfile))
                    new SoundPlayer(Rarekiller.Soundfile).Play();
                else
                    Logging.Write(Colors.Red, "Rarekiller Part Alert: playing Soundfile failes");
				if(IsGM) //doesn't work !!! 
                    Logging.Write(Colors.DarkOrange, "Rarekiller Part Alert: You got a GM Wisper: {0}: {1} - Timestamp: {2}: {3}", arg.Author, arg.Message, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
				else
                    Logging.Write(Colors.Pink, "Rarekiller Part Alert: You got a Wisper: {0}: {1} - Timestamp: {2}: {3}", arg.Author, arg.Message, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
            }
        }
        public void BNWhisper(object sender, LuaEventArgs args)
        {
            object[] Args = args.Args;
            string Message = Args[0].ToString();
            string presenceId = Args[12].ToString();
            string Author = Lua.GetReturnValues(String.Format("return BNGetFriendInfoByID({0})", presenceId))[3];

            if (File.Exists(Rarekiller.Settings.SoundfileWisper))
                new SoundPlayer(Rarekiller.Settings.SoundfileWisper).Play();
            else if (File.Exists(Rarekiller.Soundfile))
                new SoundPlayer(Rarekiller.Soundfile).Play();
            else
                Logging.Write(Colors.Red, "Rarekiller Part Alert: playing Soundfile failes");
            Logging.Write(Colors.Aqua, "Rarekiller Part Alert: You got a BN Wisper: {0}: {1} - Timestamp: {2}: {3}", Author, Message, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
        }

        public void newGuild(Chat.ChatGuildEventArgs arg)
        {
            if (Rarekiller.Settings.Guild)
            {
                if (File.Exists(Rarekiller.Settings.SoundfileGuild))
                    new SoundPlayer(Rarekiller.Settings.SoundfileGuild).Play();
                else if (File.Exists(Rarekiller.Soundfile))
                    new SoundPlayer(Rarekiller.Soundfile).Play();
                else
                    Logging.Write(Colors.Red, "Rarekiller Part Alert: playing Soundfile failes");
                Logging.Write(Colors.Lime, "Rarekiller Part Alert: Guildmessage: {0}: {1} - Timestamp: {2}: {3}", arg.Author, arg.Message, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
            }
        }
        public void newOfficer(Chat.ChatLanguageSpecificEventArgs arg)
        {
            if (Rarekiller.Settings.Guild)
            {
                if (File.Exists(Rarekiller.Settings.SoundfileGuild))
                    new SoundPlayer(Rarekiller.Settings.SoundfileGuild).Play();
                else if (File.Exists(Rarekiller.Soundfile))
                    new SoundPlayer(Rarekiller.Soundfile).Play();
                else
                    Logging.Write(Colors.Red, "Rarekiller Part Alert: playing Soundfile failes");
                Logging.Write(Colors.Lime, "Rarekiller Part Alert: Officermessage: {0}: {1} - Timestamp: {2}: {3}", arg.Author, arg.Message, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
            }
        }
		
		public void LogoutForItem()
		{
			ObjectManager.Update();
            List<WoWItem> objList = ObjectManager.GetObjectsOfType<WoWItem>()
                .Where(o => ((o.Entry == 63042) || (o.Entry == 67151) || (o.Entry == 63046) || (o.Entry == 44168) || ((o.Entry == 40768) && Rarekiller.Settings.TestLogoutItem)))
                .OrderBy(o => o.Distance).ToList();
			foreach (WoWItem o in objList)
			{
				if(Me.BagItems.Contains(o))
				{
					Logging.Write(Colors.MediumPurple, "Successfully Lootet {0}", o.Name);
					WoWMovement.MoveStop();
					Logging.Write(Colors.Red, "Rarekiller Part Log Out: Logout - to interrupt this, stop the Bot");
					Logging.Write("Rarekiller Part Log Out: Log Out in");
					Logging.Write("5 sec");
					Thread.Sleep(1000);
					Logging.Write("4 sec");
					Thread.Sleep(1000);
					Logging.Write("3 sec");
					Thread.Sleep(1000);
					Logging.Write("2 sec");
					Thread.Sleep(1000);
					Logging.Write("1 sec");
					Thread.Sleep(1000);
					Lua.DoString("ForceQuit()");
					return;
				}
			}
		}
		
        public void HearthMe(string Message)
        {
            if (Rarekiller.Settings.PlayerFollowsLogout && (Message.Contains("Followed for too long by player")))
            {
                Logging.Write(Colors.Red, "Rarekiller Part Log Out: {0} - Kill WoW Now, bb", Message);
                StyxWoW.Memory.Process.Kill();
                return;
            }
            Logging.Write("Use Heartstone! " + Message);
            ObjectManager.Update();
            List<WoWItem> objList = ObjectManager.GetObjectsOfType<WoWItem>()
                .Where(o => ((o.Entry == 6948) || (o.Entry == 64488)))
                .OrderBy(o => o.Distance).ToList();

            if (File.Exists(Rarekiller.Soundfile))
                new SoundPlayer(Rarekiller.Soundfile).Play();
            else
                Logging.Write(Colors.Red, "Rarekiller Part Log Out: playing Soundfile failes");

			//Descend to Land
			//KeyboardManager.PressKey('X');
			if(Me.IsFlying && !Rarekiller.inCombat)
			{
				WoWMovement.MoveStop();
				Thread.Sleep(1000);
				WoWMovement.Move(WoWMovement.MovementDirection.Descend);
				Thread.Sleep(1000);
			}
			if(Me.IsFlying && !Rarekiller.inCombat)
				Thread.Sleep(1000);
			if(Me.IsFlying && !Rarekiller.inCombat)
				Thread.Sleep(1000);
			if(Me.IsFlying && !Rarekiller.inCombat)
				Thread.Sleep(1000);
			if(Me.IsFlying && !Rarekiller.inCombat)
				Thread.Sleep(1000);
			//KeyboardManager.ReleaseKey('X');
			WoWMovement.MoveStop();
			//Dismount
            if (Me.Auras.ContainsKey("Flight Form"))
                Lua.DoString("CancelShapeshiftForm()");
            else if (Me.Mounted)
                Lua.DoString("Dismount()");

            Thread.Sleep(300);

            Logging.Write(Colors.Red, "Rarekiller Part Log Out: Use Heartstone, to interrupt this, stop the Bot");
            Logging.Write("Rarekiller Part Log Out: Use Heartstone in");
            Logging.Write("5 sec");
			Thread.Sleep(1000);
            Logging.Write("4 sec");
			Thread.Sleep(1000);
			if (Rarekiller.Settings.Message1 != "" && (Message == "Continent Change"))
				Lua.DoString(string.Format("RunMacroText(\"{0} {1}?\")", Rarekiller.Settings.Message1, Me.RealZoneText), 0);
            Logging.Write("3 sec");
			Thread.Sleep(1000);
            Logging.Write("2 sec");
			Thread.Sleep(1000);
            Logging.Write("1 sec");
			Thread.Sleep(1000);
            foreach (WoWItem o in objList)
			{
                if(SpellManager.HasSpell("Astral Recall") && SpellManager.CanCast("Astral Recall"))
					SpellManager.Cast("Astral Recall");
				else
					o.Use();
				Thread.Sleep(12000);
				Thread.Sleep(1000);
				while (!StyxWoW.IsInGame)
				{
					Thread.Sleep(1000);
				}
				Thread.Sleep(5000);
				
				if ((Me.MapId != Rarekiller.Settings.ContinentID) && (Message != "Continent Change"))
				{
                    Logging.Write(Colors.Red, "Rarekiller Part Log Out: Location of Heartstone is not your camping area, can't fly back camping. Stopping Bot and Exit");
					Thread.Sleep(1000);
					Lua.DoString("ForceQuit()");
					return;
				}
				else
				{
                    Logging.Write("Rarekiller Part Log Out: Hang around a little bit and go camping again");
					Thread.Sleep(900000);
					Movearound();
					Thread.Sleep(900000);
					Movearound();
					return;					
				}
			}
        }

        public void LogMeOut(string Message)
        {
            int Alert = 0;
			Lua.DoString("TakeScreenshot()");
            Logging.Write("Rarekiller Part Log Out: Take Screenshot");
            Thread.Sleep(500);

            if (File.Exists(Rarekiller.Soundfile))
                new SoundPlayer(Rarekiller.Soundfile).Play();
            else
                Logging.Write(Colors.Red, "Rarekiller Part Log Out: playing Soundfile failes");

			if (Message == "Continent Change")
                Logging.Write(Colors.Red, "Rarekiller Part Log Out: Unautorized Continent Change from {0} Continent {1} to {2} Continent {3}", Rarekiller.Settings.Zone, Rarekiller.Settings.Continent, Me.RealZoneText, Me.MapName);
			WoWMovement.MoveStop();

            if (Rarekiller.Settings.JustAlert && (Message == "Continent Change"))
            {
                while (Alert < 50)
                {
                    Thread.Sleep(10000);
                    new SoundPlayer(Rarekiller.Soundfile).Play();
                    Alert = Alert + 1;
                }
                Rarekiller.Settings.KillWoW = true;
            }
            
            if (Rarekiller.Settings.KillWoW && (Message == "Continent Change"))
            {
                Logging.Write(Colors.Red, "Rarekiller Part Log Out: Kill WoW Now, bb");
                StyxWoW.Memory.Process.Kill();
                return;
            }

            HearthMe(Message);
            Logging.Write(Colors.Red, "Rarekiller Part Log Out: Logout - to interrupt this, stop the Bot");
            Logging.Write("Rarekiller Part Log Out: Log Out in");
            Logging.Write("5 sec");
            Thread.Sleep(1000);
            Logging.Write("4 sec");
            Thread.Sleep(1000);
            if (Rarekiller.Settings.Message2 != "")
                Lua.DoString(string.Format("RunMacroText(\"{0}\")", Rarekiller.Settings.Message2), 0);
            Logging.Write("3 sec");
            Thread.Sleep(1000);
            Logging.Write("2 sec");
            Thread.Sleep(1000);
            Logging.Write("1 sec");
            Thread.Sleep(1000);
            Lua.DoString("ForceQuit()");
        }
    }
}
