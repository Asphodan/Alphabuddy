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
    class RotaShaman
    {
        public static LocalPlayer Me = StyxWoW.Me;

        public void ShamanKill(WoWUnit Unit)
        {   
            if (Me.HealthPercent < 10 && SpellManager.HasSpell("Healing Surge"))
            {
                while (Me.HealthPercent < 30 && SpellManager.HasSpell("Healing Surge"))
                {
                    RarekillerSpells.CastSafe("Healing Surge", Me, true);
                    Thread.Sleep(100);
                }
            }
            if (Me.HealthPercent < 30 && SpellManager.HasSpell("Greater Healing Wave"))
            {
                while (Me.HealthPercent < 50 && SpellManager.HasSpell("Greater Healing Wave"))
                {
                    RarekillerSpells.CastSafe("Greater Healing Wave", Me, true);
                    Thread.Sleep(100);
                }
            }
// -------- Damage
            if (SpellManager.HasSpell("Flame Shock") && !SpellManager.Spells["Flame Shock"].Cooldown && !Unit.ActiveAuras.ContainsKey("Flame Shock"))
            {
                RarekillerSpells.CastSafe("Flame Shock", Unit, true);
                Thread.Sleep(100);
				return;
            }
            if (SpellManager.HasSpell("Stormstrike") && !SpellManager.Spells["Stormstrike"].Cooldown)
            {
                RarekillerSpells.CastSafe("Stormstrike", Unit, true);
                Thread.Sleep(100);
                return;
            }
            if (SpellManager.HasSpell("Lava Lash") && !SpellManager.Spells["Lava Lash"].Cooldown)
            {
                RarekillerSpells.CastSafe("Lava Lash", Unit, true);
                Thread.Sleep(100);
                return;
            }
            if (!SpellManager.HasSpell("Lava Lash") && SpellManager.HasSpell("Lava Burst") && !SpellManager.Spells["Lava Burst"].Cooldown)
            {
                RarekillerSpells.CastSafe("Lava Burst", Unit, true);
                Thread.Sleep(100);
                return;
            }
            if (SpellManager.HasSpell("Lightning Bolt") && !SpellManager.Spells["Lightning Bolt"].Cooldown)
            {
                RarekillerSpells.CastSafe("Lightning Bolt", Unit, true);
                Thread.Sleep(100);
                return;
            }
        }
    }    
}