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
using Styx.Pathing;

namespace BadWolf
{
    class RarekillerTamer
    {
        public static LocalPlayer Me = StyxWoW.Me;
        private static Stopwatch BlacklistTimer = new Stopwatch();
        bool ForceGround = false;
        Int64 Place = 0;
        
        public void findAndTameMob()
        {
            if (Rarekiller.Settings.DeveloperLogs)
                Logging.Write("Rarekiller: Scan for Tamer");
// ----------------- Generate a List with all wanted Rares found in Object Manager ---------------------		
            ObjectManager.Update();
            List<WoWUnit> objList = ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(o => (
                ((Rarekiller.Settings.TameDefault && Rarekiller.TameableMobsList.ContainsKey(Convert.ToInt32(o.Entry)))
                || (Rarekiller.Settings.TameByID && (o.Entry == Convert.ToInt64(Rarekiller.Settings.TameMobID))))
                && !o.IsPet && o.IsTameable))
                .OrderBy(o => o.Distance).ToList();
            foreach (WoWUnit o in objList)
            {
                if (!o.IsDead && !Blacklist.Contains(o.Guid))
                {
                    Logging.Write(Colors.MediumPurple, "Rarekiller Part Tamer: Found a new Pet {0} ID {1}", o.Name, o.Entry);
// Don't tame the Rare if ...
                    if (Rarekiller.inCombat)
                    {
                        Logging.Write("Rarekiller Part Tamer: ... but I'm in another Combat :( !!!");
                        return;
                    }
                    if (Me.Level < o.Level)
                    {
                        Logging.Write("Rarekiller Part Tamer: Mob Level is higher then mine, can't tame the Mob.");
                        Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                        Logging.Write(Colors.DarkOrange, "Rarekiller Part Tamer: Blacklist Mob for 60 Minutes.");
                        return;
                    }
                    if (Me.IsOnTransport)
                    {
                        Logging.Write("Rarekiller Part Tamer: ... but I'm on a Transport.");
                        return;
                    }
                    while (Me.IsCasting)
                    {
                        Thread.Sleep(100);
                    }
					if(Rarekiller.BlacklistMobsList.ContainsKey(Convert.ToInt32(o.Entry)))
                    {
                        Logging.Write("Rarekiller Part Tamer: {0} is Member of the BlacklistedMobs.xml", o.Name);
                        Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist15));
                        Logging.Write(Colors.DarkOrange, "Rarekiller Part Tamer: Blacklist Mob for 15 Minutes.");
                        return;
					}

//Dismiss Pet
					SpellManager.Cast("Dismiss Pet");
                    Thread.Sleep(3000);
					
// ----------------- Alert ---------------------
                    if (Rarekiller.Settings.Alert)
                    {
                        if (File.Exists(Rarekiller.Settings.SoundfileFoundRare))
                            new SoundPlayer(Rarekiller.Settings.SoundfileFoundRare).Play();
                        else if (File.Exists(Rarekiller.Soundfile))
                            new SoundPlayer(Rarekiller.Soundfile).Play();
                        else
                            Logging.Write(Colors.Red, "Rarekiller Part Tamer: playing Soundfile failes");
                    }
// ----------------- Move to Mob Part ---------------------	

                    WoWPoint newPoint = WoWMovement.CalculatePointFrom(o.Location, (float)Rarekiller.Settings.Tamedistance);

                    if (o.Entry == 49822)
                        ForceGround = true;

                    Logging.Write("Rarekiller Part MoveTo: Move to target");
                    BlacklistTimer.Reset();
                    BlacklistTimer.Start();
                    Place = Me.ZoneId;
					while (newPoint.Distance(Me.Location) > Rarekiller.Settings.Tamedistance)
					{
						if (Rarekiller.Settings.GroundMountMode || ForceGround)
							Navigator.MoveTo(newPoint);
						else
							Flightor.MoveTo(newPoint);
						Thread.Sleep(100);
						// ----------------- Security  ---------------------
						if (Rarekiller.inCombat) return;
                        if (Rarekiller.Settings.BlacklistCheck && (BlacklistTimer.Elapsed.TotalSeconds > (Convert.ToInt32(Rarekiller.Settings.BlacklistTime))))
                        {
                            Logging.Write(Colors.Red, "Rarekiller Part Tamer: Can't reach Mob {0}, Blacklist and Move on", o.Name);
                            Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                            BlacklistTimer.Reset();
                            WoWMovement.MoveStop();
                            return;
                        }
						// ----------------- Security  ---------------------
                        if (Rarekiller.Settings.ZoneSave && (Me.ZoneId != Place))
                        {
                            Logging.Write(Colors.Red, "Rarekiller Part MoveTo: Left Zone while move to {0} is not allowed", o.Name);
                            Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                            Logging.Write(Colors.DarkOrange, "Rarekiller: Blacklist Mob for 5 Minutes.");
                            BlacklistTimer.Reset();
                            WoWMovement.MoveStop();
                            return;
                        }
                    }
                    BlacklistTimer.Reset();
					Thread.Sleep(300);
					WoWMovement.MoveStop();
//Dismount
                    if (Me.Auras.ContainsKey("Flight Form"))
                        Lua.DoString("CancelShapeshiftForm()");
                    else if (Me.Mounted)
                        Lua.DoString("Dismount()");

                    Thread.Sleep(150);
					o.Target();
					if(Rarekiller.Settings.ScreenTamer && Rarekiller.Settings.ScreenFound)
					{
						Lua.DoString("TakeScreenshot()");
						Thread.Sleep(300);
                        Logging.Write("Rarekiller Part Tamer: Take Screenshot before tame");
					}
// Tame it
                    while (!o.IsPet)
                    {
                        if (o.IsDead)
                        {
                            Logging.Write("Rarekiller Part Tamer: Mob was dying, I'm so Sorry !!! ");
                            return;
                        }
                        if (Me.HealthPercent < 10)
                        {
                            Logging.Write("Rarekiller Part Tamer: Health < 10% , Use Feign Death !!! ");
							SpellManager.Cast("Feign Death");
                            return;
                        }

                        if (!Me.IsCasting)
                        {
                            WoWMovement.MoveStop();
                            SpellManager.Cast("Tame Beast");
                            Logging.Write("Plugin Part The Tamer: Try to tame Beast {0}", o.Name);
                            Thread.Sleep(1500);
                        }
                    }
                    Logging.Write("Rarekiller Part Tamer: Sucessfully tamed Beast {0}", o.Name);
                    if (Rarekiller.Settings.ScreenTamer && Rarekiller.Settings.ScreenSuccess)
                    {
                        Lua.DoString("TakeScreenshot()");
                        Thread.Sleep(500);
                        Logging.Write("Rarekiller Part Tamer: Take Screenshot after tame");
                    }
                }
                else if (o.IsPet)
                    return;
                else
                {
                    Logging.Write("Rarekiller Part Tamer: Find a Mob, but sadly he's dead, blacklistet or not tameable: {0}", o.Name);
                    Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                    Logging.Write(Colors.DarkOrange, "Rarekiller Part Tamer: Blacklist Mob for 5 Minutes.");
                    return;
                }
            }
        }
    }
}
