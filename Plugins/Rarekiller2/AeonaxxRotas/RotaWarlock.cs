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
    class RotaWarlock
    {
        public static LocalPlayer Me = StyxWoW.Me;

        public void WarlockKill(WoWUnit Unit)
        {
			if (Me.ManaPercent < 10 && SpellManager.HasSpell("Shoot") && SpellManager.CanCast("Shoot"))
            {
				while (Me.ManaPercent < 30 && SpellManager.HasSpell("Shoot"))
                {
					RarekillerSpells.CastSafe("Shoot", Unit, true);
					Thread.Sleep(6500);
					return;
				}
            }
			
            // Affliction Warlock
            if (SpellManager.HasSpell("Unstable Affliction"))
            {
                if (SpellManager.HasSpell("Demon Soul") && !SpellManager.Spells["Demon Soul"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Demon Soul", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Haunt") && !SpellManager.Spells["Haunt"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Haunt", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Soulburn") && !SpellManager.Spells["Soulburn"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Soulburn", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Soul Fire") && !SpellManager.Spells["Soul Fire"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Soul Fire", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Bane of Agony") && !SpellManager.Spells["Bane of Agony"].Cooldown && !Unit.ActiveAuras.ContainsKey("Bane of Agony") && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Bane of Agony", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Corruption") && !SpellManager.Spells["Corruption"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Corruption", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Unstable Affliction") && !SpellManager.Spells["Unstable Affliction"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Unstable Affliction", Unit, true);
                    Thread.Sleep(100);
                }
            }

            //Destruction Warlock
            else if (SpellManager.HasSpell("Conflagrate"))
            {
                if (SpellManager.HasSpell("Soulburn") && !SpellManager.Spells["Soulburn"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Soulburn", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Soul Fire") && !SpellManager.Spells["Soul Fire"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Soul Fire", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Bane of Doom") && !SpellManager.Spells["Bane of Doom"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Bane of Doom", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Immolate") && !SpellManager.Spells["Immolate"].Cooldown && !Unit.ActiveAuras.ContainsKey("Immolate") && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Immolate", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Conflagrate") && !SpellManager.Spells["Conflagrate"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Conflagrate", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Corruption") && !SpellManager.Spells["Corruption"].Cooldown && !Unit.ActiveAuras.ContainsKey("Corruption") && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Corruption", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Shadowflame") && !SpellManager.Spells["Shadowflame"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Shadowflame", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Chaos Bolt") && !SpellManager.Spells["Chaos Bolt"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Chaos Bolt", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Incinerate") && !SpellManager.Spells["Incinerate"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Incinerate", Unit, true);
                    Thread.Sleep(100);
                }
            }
			//Demonologie
			else
			{
				if (SpellManager.HasSpell("Immolate") && !SpellManager.Spells["Immolate"].Cooldown && !Unit.ActiveAuras.ContainsKey("Immolate") && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Immolate", Unit, true);
                    Thread.Sleep(100);
                }
				if (SpellManager.HasSpell("Corruption") && !SpellManager.Spells["Corruption"].Cooldown && !Unit.ActiveAuras.ContainsKey("Corruption") && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Corruption", Unit, true);
                    Thread.Sleep(100);
                }
				if (SpellManager.HasSpell("Bane of Agony") && !SpellManager.Spells["Bane of Agony"].Cooldown && !Unit.ActiveAuras.ContainsKey("Bane of Agony") && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Bane of Agony", Unit, true);
                    Thread.Sleep(100);
                }
				if (SpellManager.HasSpell("Incinerate") && !SpellManager.Spells["Incinerate"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Incinerate", Unit, true);
                    Thread.Sleep(100);
                }
				if (SpellManager.HasSpell("Shadow Bolt") && !SpellManager.Spells["Shadow Bolt"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Shadow Bolt", Unit, true);
                    Thread.Sleep(100);
                }
			}
        }
    }    
}