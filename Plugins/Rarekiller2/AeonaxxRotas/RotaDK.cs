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
    class RotaDK
    {
        public static LocalPlayer Me = StyxWoW.Me;

        public void DKKill(WoWUnit Unit)
        {

			if (SpellManager.HasSpell("Bone Shield") && !SpellManager.Spells["Bone Shield"].Cooldown && !Me.ActiveAuras.ContainsKey("Bone Shield"))
			{
				RarekillerSpells.CastSafe("Bone Shield", Me, true);
				Thread.Sleep(100);
			}
			
			
			if (SpellManager.HasSpell("Frost Strike") && !SpellManager.Spells["Frost Strike"].Cooldown && !Unit.IsDead)
			{
				if((Me.RunicPowerPercent > 16) && (Me.CurrentRunicPower > SpellManager.Spells["Frost Strike"].PowerCost))
				{
					RarekillerSpells.CastSafe("Frost Strike", Unit, true);
					Thread.Sleep(100);
				}
			}
			
			if (SpellManager.HasSpell("Outbreak") && !SpellManager.Spells["Outbreak"].Cooldown && !Unit.IsDead)
			{
				if(!Unit.ActiveAuras.ContainsKey("Blood Plague") && !Unit.ActiveAuras.ContainsKey("Frost Fever"))
				{
					RarekillerSpells.CastSafe("Outbreak", Unit, true);
					Thread.Sleep(100);
				}
			}
			
			if (SpellManager.HasSpell("BloodTap") && !SpellManager.Spells["BloodTap"].Cooldown && !Unit.IsDead)
			{
				if((Me.BloodRuneCount == 0) && (Me.DeathRuneCount == 0))
				{
					RarekillerSpells.CastSafe("BloodTap", Me, true);
					Thread.Sleep(100);
				}
			}
			
			if (SpellManager.HasSpell("Death Strike") && !SpellManager.Spells["Death Strike"].Cooldown && Me.HealthPercent < 80 && !Unit.IsDead)
			{                    
				if (!(Me.UnholyRuneCount < 1 && Me.FrostRuneCount < 1 && Me.DeathRuneCount < 2)  && !(Me.UnholyRuneCount < 1 && Me.DeathRuneCount < 1) 
						&& !(Me.FrostRuneCount < 1 && Me.DeathRuneCount < 1))
				{
					RarekillerSpells.CastSafe("Death Strike", Unit, true);
					Thread.Sleep(100);
				}
			}
			
			if (SpellManager.HasSpell("Festering Strike") && !SpellManager.Spells["Festering Strike"].Cooldown && !Unit.IsDead)
			{                    
				if (!(Me.BloodRuneCount < 1 && Me.FrostRuneCount < 1 && Me.DeathRuneCount < 2)
						&& !(Me.BloodRuneCount < 1 && Me.DeathRuneCount < 1)
						&& !(Me.FrostRuneCount < 1 && Me.DeathRuneCount < 1)
						&& !(!Unit.ActiveAuras.ContainsKey("Frost Fever"))
						&& !(!Unit.ActiveAuras.ContainsKey("Blood Plague"))
						&& !(Unit.Auras["Frost Fever"].TimeLeft.TotalSeconds > 5 && Unit.Auras["Blood Plague"].TimeLeft.TotalSeconds > 5))
				{
					RarekillerSpells.CastSafe("Festering Strike", Unit, true);
					Thread.Sleep(100);
				}
			}
			
			if (SpellManager.HasSpell("Rune Tap") && !SpellManager.Spells["Rune Tap"].Cooldown && Me.HealthPercent < 35 && !Unit.IsDead)
			{                    
				if (!(Me.BloodRuneCount < 1 && Me.DeathRuneCount < 1))
				{				
					RarekillerSpells.CastSafe("Rune Tap", Me, true);
					Thread.Sleep(100);
				}
			}
			
			if (SpellManager.HasSpell("Heart Strike") && !SpellManager.Spells["Heart Strike"].Cooldown && !Unit.IsDead)
			{
				if (!(Me.BloodRuneCount < 1 && Me.DeathRuneCount < 1))
				{				
					RarekillerSpells.CastSafe("Heart Strike", Unit, true);
					Thread.Sleep(100);
				}
			}

			if (SpellManager.HasSpell("Howling Blast") && !SpellManager.Spells["Howling Blast"].Cooldown && !Unit.IsDead)
			{
				if (!(Me.FrostRuneCount < 1 && Me.DeathRuneCount < 1))
				{
					RarekillerSpells.CastSafe("Howling Blast", Unit, true);
					Thread.Sleep(100);
				}
			}
			
			if (SpellManager.HasSpell("Death Coil") && !SpellManager.Spells["Death Coil"].Cooldown && !Unit.IsDead)
			{
				if((Me.RunicPowerPercent > 80) && (Me.CurrentRunicPower > SpellManager.Spells["Frost Strike"].PowerCost))
				{
					RarekillerSpells.CastSafe("Death Coil", Unit, true);
					Thread.Sleep(100);
				}
			}
			
			if (SpellManager.HasSpell("Frost Strike") && !SpellManager.Spells["Frost Strike"].Cooldown && !Unit.IsDead)
			{
				if((Me.RunicPowerPercent > 16) && (Me.CurrentRunicPower > SpellManager.Spells["Frost Strike"].PowerCost))
				{
					RarekillerSpells.CastSafe("Frost Strike", Unit, true);
					Thread.Sleep(100);
				}
			}
			
			if (SpellManager.HasSpell("Obliterate") && !SpellManager.Spells["Obliterate"].Cooldown && !Unit.IsDead)
			{
				if (!(Me.UnholyRuneCount < 1 && Me.FrostRuneCount < 1 && Me.DeathRuneCount < 2)
						&& !(Me.UnholyRuneCount < 1 && Me.DeathRuneCount < 1)
						&& !(Me.FrostRuneCount < 1 && Me.DeathRuneCount < 1))
				{
					RarekillerSpells.CastSafe("Obliterate", Unit, true);
					Thread.Sleep(100);
				}
			}
			

			if (SpellManager.HasSpell("Plague Strike") && !SpellManager.Spells["Plague Strike"].Cooldown && Unit.ActiveAuras.ContainsKey("Blood Plague") && !Unit.IsDead)
			{
				if (!(Me.UnholyRuneCount < 1 && Me.DeathRuneCount < 1))
				{
					RarekillerSpells.CastSafe("Plague Strike", Unit, true);
					Thread.Sleep(100);
				}
			}

			if (SpellManager.HasSpell("Frost Strike") && !SpellManager.Spells["Frost Strike"].Cooldown && !Unit.IsDead)
			{
				if((Me.RunicPowerPercent > 16) && (Me.CurrentRunicPower > SpellManager.Spells["Frost Strike"].PowerCost))
				{
					RarekillerSpells.CastSafe("Frost Strike", Unit, true);
					Thread.Sleep(100);
				}
			}
			
			if (SpellManager.HasSpell("Icy Touch") && !SpellManager.Spells["Icy Touch"].Cooldown && !Unit.IsDead)
			{
				if (!(Me.FrostRuneCount < 1 && Me.DeathRuneCount < 1))
				{
					RarekillerSpells.CastSafe("Icy Touch", Unit, true);
					Thread.Sleep(100);
				}
			}
			
			if (SpellManager.HasSpell("Scourge Strike") && !SpellManager.Spells["Scourge Strike"].Cooldown && !Unit.IsDead)
			{
				if (!(Me.UnholyRuneCount < 1 && Me.DeathRuneCount < 1))
				{
					RarekillerSpells.CastSafe("Scourge Strike", Unit, true);
					Thread.Sleep(100);
				}
			}
			
			if (SpellManager.HasSpell("Blood Strike") && !SpellManager.Spells["Blood Strike"].Cooldown && !Unit.IsDead)
			{
				if (!(Me.BloodRuneCount < 1 && Me.DeathRuneCount < 1))
				{				
					RarekillerSpells.CastSafe("Blood Strike", Unit, true);
					Thread.Sleep(100);
				}
			}
        }
    }    
}