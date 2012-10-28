using System.IO;
using Styx;
using Styx.Helpers;
using Styx.Common;

namespace DWCC5Free
{
    public class DunatanksSettings : Settings
    {
        public static readonly DunatanksSettings Instance = new DunatanksSettings();

        public DunatanksSettings()
            : base(Path.Combine(Styx.Common.Utilities.AssemblyDirectory, "Settings", string.Format(@"DWCC-{0}-{1}.xml", StyxWoW.Me.Name, StyxWoW.Me.RealmName)))
        {
        }

        #region Specc
        [Setting, DefaultValue(true)]
        public bool useArms { get; set; }

        [Setting, DefaultValue(false)]
        public bool useFury { get; set; }

        [Setting, DefaultValue(false)]
        public bool useProt { get; set; }
        #endregion
        #region Taunt
        [Setting, DefaultValue(false)]
        public bool useTaunt { get; set; }
        #endregion
        #region HealthStone
        [Setting, DefaultValue(false)]
        public bool useHealthStone { get; set; }

        [Setting, DefaultValue(15)]
        public int HealthStonePercent { get; set; }
        #endregion
        #region Stances
        [Setting, DefaultValue(false)]
        public bool AutoSwitchBattleStance { get; set; }

        [Setting, DefaultValue(false)]
        public bool AutoSwitchBerserkerStance { get; set; }

        [Setting, DefaultValue(false)]
        public bool AutoSwitchDefensiveStance { get; set; }
        #endregion
        #region Pummel
        [Setting, DefaultValue(false)]
        public bool usePummel { get; set; }
        #endregion
        #region Disarm Prot
        [Setting, DefaultValue(false)]
        public bool useDisarmProt { get; set; }
        #endregion
        #region Vigilance
        [Setting, DefaultValue(false)]
        public bool useVigilanceProt { get; set; }

        [Setting, DefaultValue(true)]
        public bool useVigilanceOnRandom { get; set; }

        [Setting, DefaultValue(false)]
        public bool useVigilanceOnSpecific { get; set; }

        [Setting, DefaultValue("")]
        public string VigilanceSpecificName { get; set; }
        #endregion
        #region Potion
        [Setting, DefaultValue(false)]
        public bool usePotion { get; set; }

        [Setting, DefaultValue(20)]
        public int PotionPercent { get; set; }
        #endregion
        #region TrinketOne
        [Setting, DefaultValue(false)]
        public bool UseTrinketOneOnCd { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseTrinketOneHero { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseTrinketOneBelow20 { get; set; }

        [Setting, DefaultValue(true)]
        public bool DoNotUseTrinketOne { get; set; }

        [Setting, DefaultValue(false)]
        public bool useTrinketOneCC { get; set; }
        #endregion
        #region TrinketTwo
        [Setting, DefaultValue(false)]
        public bool UseTrinketTwoOnCd { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseTrinketTwoHero { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseTrinketTwoBelow20 { get; set; }

        [Setting, DefaultValue(true)]
        public bool DoNotUseTrinketTwo { get; set; }

        [Setting, DefaultValue(false)]
        public bool useTrinketTwoCC { get; set; }
        #endregion
        #region Movement
        [Setting, DefaultValue(false)]
        public bool DisableMovement { get; set; }
        #endregion
        #region Pull
        [Setting, DefaultValue(true)]
        public bool usePullBehaviour { get; set; }

        [Setting, DefaultValue(20)]
        public int PullRange { get; set; }
        #endregion
        #region CombatDistance
        [Setting, DefaultValue(10)]
        public int CombatDistance { get; set; }
        #endregion
        #region TankTargeting
        [Setting, DefaultValue(false)]
        public bool useAutoTargetProt { get; set; }
        #endregion
        #region AoEPummel
        [Setting, DefaultValue(false)]
        public bool usePummelAoEAuto { get; set; }
        #endregion
        #region Rest
        [Setting, DefaultValue(true)]
        public bool UseRest { get; set; }

        [Setting, DefaultValue(25)]
        public int RestPercent { get; set; }
        #endregion
        #region Racials
        [Setting, DefaultValue(true)]
        public bool UseRacials { get; set; }
        #endregion
    }
}