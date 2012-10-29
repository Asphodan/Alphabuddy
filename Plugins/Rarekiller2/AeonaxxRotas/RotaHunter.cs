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
    class RotaHunter
    {
        public static LocalPlayer Me = StyxWoW.Me;

        public void HunterKill(WoWUnit Unit)
        {
            //Survival Hunter
            if (SpellManager.HasSpell("Explosive Shot"))
            {
                if (SpellManager.HasSpell("Kill Shot") && !SpellManager.Spells["Kill Shot"].Cooldown && Unit.HealthPercent < 20 && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Kill Shot", Unit, true);
                    Thread.Sleep(100);
                }
				if (SpellManager.HasSpell("Rapid Fire") && !SpellManager.Spells["Rapid Fire"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Rapid Fire", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Serpent Sting") && !SpellManager.Spells["Serpent Sting"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Serpent Sting", Unit, true);
                    Thread.Sleep(100);
                }
				if (SpellManager.HasSpell("Black Arrow") && !SpellManager.Spells["Black Arrow"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Black Arrow", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Explosive Shot") && !SpellManager.Spells["Explosive Shot"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Explosive Shot", Unit, true);
                    Thread.Sleep(100);
                }				
			}
			else
			{
				if (SpellManager.HasSpell("Kill Shot") && !SpellManager.Spells["Kill Shot"].Cooldown && Unit.HealthPercent < 20 && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Kill Shot", Unit, true);
                    Thread.Sleep(100);
                }
				if (SpellManager.HasSpell("Rapid Fire") && !SpellManager.Spells["Rapid Fire"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Rapid Fire", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Arcane Shot") && !SpellManager.Spells["Arcane Shot"].Cooldown && !Unit.IsDead)
                {
                    RarekillerSpells.CastSafe("Arcane Shot", Unit, true);
                    Thread.Sleep(100);
                }
			}
        }
    }    
}