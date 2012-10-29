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
    class RotaWarrior
    {
        public static LocalPlayer Me = StyxWoW.Me;

        public void WarriorKill(WoWUnit Unit)
        {
            //Arms warrior: rend, colossus smash, mortal strike, overpower, slam
            if (SpellManager.HasSpell("Mortal Strike"))
            {
                if (SpellManager.HasSpell("Battle Stance") && !SpellManager.Spells["Battle Stance"].Cooldown && !Me.ActiveAuras.ContainsKey("Battle Stance") && !Unit.IsDead)
				{
					RarekillerSpells.CastSafe("Battle Stance", Me, true);
					Thread.Sleep(100);
					return;
				}
				if (SpellManager.HasSpell("Rend") && !SpellManager.Spells["Rend"].Cooldown && !Unit.IsDead)
                {
					if (SpellManager.HasSpell("Battle Stance") && !SpellManager.Spells["Battle Stance"].Cooldown && !Me.ActiveAuras.ContainsKey("Battle Stance") && !Unit.IsDead)
					{
						RarekillerSpells.CastSafe("Battle Stance", Me, true);
						Thread.Sleep(100);
						return;
					}
					RarekillerSpells.CastSafe("Rend", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Colossus Smash") && !SpellManager.Spells["Colossus Smash"].Cooldown && !Unit.IsDead)
                {
                    if (SpellManager.HasSpell("Battle Stance") && !SpellManager.Spells["Battle Stance"].Cooldown && !Me.ActiveAuras.ContainsKey("Battle Stance") && !Me.ActiveAuras.ContainsKey("Berserker Stance") && !Unit.IsDead)
					{
						RarekillerSpells.CastSafe("Battle Stance", Me, true);
						Thread.Sleep(100);
						return;
					}
					RarekillerSpells.CastSafe("Colossus Smash", Unit, true);
                    Thread.Sleep(100);
                }
                if (!SpellManager.Spells["Mortal Strike"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Mortal Strike", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Overpower") && !SpellManager.Spells["Overpower"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Overpower", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Slam") && !SpellManager.Spells["Slam"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Slam", Unit, true);
                    Thread.Sleep(100);
                }
            }

            //Fury warrior: Colossus smash, raging blow, bloodthirst, heroic strike
            else if (SpellManager.HasSpell("Bloodthirst"))
            {
                if (SpellManager.HasSpell("Berserker Stance") && !SpellManager.Spells["Berserker Stance"].Cooldown && !Me.ActiveAuras.ContainsKey("Berserker Stance") && !Unit.IsDead)
				{
					RarekillerSpells.CastSafe("Berserker Stance", Me, true);
					Thread.Sleep(100);
					return;
				}
				if (SpellManager.HasSpell("Colossus Smash") && !SpellManager.Spells["Colossus Smash"].Cooldown && !Unit.IsDead)
                {
                    if (SpellManager.HasSpell("Berserker Stance") && !SpellManager.Spells["Berserker Stance"].Cooldown && !Me.ActiveAuras.ContainsKey("Battle Stance") && !Me.ActiveAuras.ContainsKey("Berserker Stance") && !Unit.IsDead)
                    {
						RarekillerSpells.CastSafe("Berserker Stance", Me, true);
						Thread.Sleep(100);
						return;
					}
					RarekillerSpells.CastSafe("Colossus Smash", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Raging Blow") && !SpellManager.Spells["Raging Blow"].Cooldown && !Unit.IsDead)
                {
                    if (SpellManager.HasSpell("Berserker Stance") && !SpellManager.Spells["Berserker Stance"].Cooldown && !Me.ActiveAuras.ContainsKey("Battle Stance") && !Me.ActiveAuras.ContainsKey("Berserker Stance") && !Unit.IsDead)
                    {
						RarekillerSpells.CastSafe("Berserker Stance", Me, true);
						Thread.Sleep(100);
						return;
					}
					RarekillerSpells.CastSafe("Raging Blow", Unit, true);
                    Thread.Sleep(100);
                }
                if (!SpellManager.Spells["Bloodthirst"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Bloodthirst", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Heroic Strike") && !SpellManager.Spells["Heroic Strike"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Heroic Strike", Unit, true);
                    Thread.Sleep(100);
                }
            }
            //Prot Warrior: Thunder Clap, Revenge, Shield Slam, Shockwave, Devastate
            else if (SpellManager.HasSpell("Shield Slam"))
            {
				if (SpellManager.HasSpell("Defensive Stance") && !SpellManager.Spells["Defensive Stance"].Cooldown && !Me.ActiveAuras.ContainsKey("Defensive Stance") && !Unit.IsDead)
				{
					RarekillerSpells.CastSafe("Defensive Stance", Me, true);
					Thread.Sleep(100);
					return;
				}
				
				if (SpellManager.HasSpell("Thunder Clap") && !SpellManager.Spells["Thunder Clap"].Cooldown && !Unit.IsDead)
                {
                    if (SpellManager.HasSpell("Defensive Stance") && !SpellManager.Spells["Defensive Stance"].Cooldown && !Me.ActiveAuras.ContainsKey("Battle Stance") && !Me.ActiveAuras.ContainsKey("Defensive Stance") && !Unit.IsDead)
					{
						RarekillerSpells.CastSafe("Defensive Stance", Me, true);
						Thread.Sleep(100);
						return;
					}
					RarekillerSpells.CastSafe("Thunder Clap", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Revenge") && !SpellManager.Spells["Revenge"].Cooldown && !Unit.IsDead)
                {
                    if (SpellManager.HasSpell("Defensive Stance") && !SpellManager.Spells["Defensive Stance"].Cooldown && !Me.ActiveAuras.ContainsKey("Defensive Stance") && !Unit.IsDead)
					{
						RarekillerSpells.CastSafe("Defensive Stance", Me, true);
						Thread.Sleep(100);
						return;
					}
					RarekillerSpells.CastSafe("Revenge", Unit, true);
                    Thread.Sleep(100);
                }
                if (!SpellManager.Spells["Shield Slam"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Shield Slam", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Shockwave") && !SpellManager.Spells["Shockwave"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Shockwave", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Devastate") && !SpellManager.Spells["Devastate"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Devastate", Unit, true);
                    Thread.Sleep(100);
                }
            }
        }
    }    
}