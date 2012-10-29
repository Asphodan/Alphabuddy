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
    class RotaPaladin
    {
        public static LocalPlayer Me = StyxWoW.Me;

        public void PaladinKill(WoWUnit Unit)
        {
// -------- Heal
            if (Me.HealthPercent < 10 && SpellManager.HasSpell("Flash of Light"))
            {
                while (Me.HealthPercent < 30 && SpellManager.HasSpell("Flash of Light"))
                {
                    RarekillerSpells.CastSafe("Flash of Light", Me, true);
                    Thread.Sleep(100);
                }
            }
            if (Me.HealthPercent < 30 && SpellManager.HasSpell("Divine Light"))
            {
                while (Me.HealthPercent < 50 && SpellManager.HasSpell("Divine Light"))
                {
                    RarekillerSpells.CastSafe("Divine Light", Me, true);
                    Thread.Sleep(100);
                }
            }
            //Retribution Paladin
            if (SpellManager.HasSpell("Templar's Verdict"))
            {
                if (SpellManager.HasSpell("Seal of Truth") && !SpellManager.Spells["Seal of Truth"].Cooldown && !Me.ActiveAuras.ContainsKey("Seal of Truth") && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Seal of Truth", Me, true);
                    Thread.Sleep(100);
                }
				
				if (SpellManager.HasSpell("Guardian of Ancient Kings") && !SpellManager.Spells["Guardian of Ancient Kings"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Guardian of Ancient Kings", Unit, true);
                    Thread.Sleep(100);
                }	
                if (SpellManager.HasSpell("Hammer of Wrath") && !SpellManager.Spells["Hammer of Wrath"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Hammer of Wrath", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Exorcism") && !SpellManager.Spells["Exorcism"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Exorcism", Unit, true);
                    Thread.Sleep(100);
                }
				if (SpellManager.HasSpell("Judgement") && !SpellManager.Spells["Judgement"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Judgement", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Holy Wrath") && !SpellManager.Spells["Holy Wrath"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Holy Wrath", Unit, true);
                    Thread.Sleep(100);
                }				
			}
			else
			{
				if (SpellManager.HasSpell("Seal of Truth") && !SpellManager.Spells["Seal of Truth"].Cooldown && !Me.ActiveAuras.ContainsKey("Seal of Truth") && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Seal of Truth", Me, true);
                    Thread.Sleep(100);
                }
				
				if (SpellManager.HasSpell("Exorcism") && !SpellManager.Spells["Exorcism"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Exorcism", Unit, true);
                    Thread.Sleep(100);
                }
				if (SpellManager.HasSpell("Judgement") && !SpellManager.Spells["Judgement"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Judgement", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Holy Wrath") && !SpellManager.Spells["Holy Wrath"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Holy Wrath", Unit, true);
                    Thread.Sleep(100);
                }		
				if (SpellManager.HasSpell("Crusader Strike") && !SpellManager.Spells["Crusader Strike"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Crusader Strike", Unit, true);
                    Thread.Sleep(100);
                }
			}
        }
    }    
}