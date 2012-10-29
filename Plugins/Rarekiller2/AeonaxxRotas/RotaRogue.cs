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
    class RotaRogue
    {
        public static LocalPlayer Me = StyxWoW.Me;

        public void RogueKill(WoWUnit Unit)
        {
			if (SpellManager.HasSpell("Revealing Strike") && !SpellManager.Spells["Revealing Strike"].Cooldown && !Unit.IsDead && (Me.ComboPoints > 4))
			{
				RarekillerSpells.CastSafe("Revealing Strike", Unit, true);
				Thread.Sleep(100);
				return;
			}
			if (SpellManager.HasSpell("Eviscerate") && !SpellManager.Spells["Eviscerate"].Cooldown && !Unit.IsDead && (Me.ComboPoints > 4))
			{
				RarekillerSpells.CastSafe("Eviscerate", Unit, true);
				Thread.Sleep(100);
				return;
			}
			
			if (SpellManager.HasSpell("Sinister Strike") && !SpellManager.Spells["Sinister Strike"].Cooldown && !Unit.IsDead)
			{
				RarekillerSpells.CastSafe("Sinister Strike", Unit, true);
				Thread.Sleep(100);
				return;
			}
        }
    }    
}