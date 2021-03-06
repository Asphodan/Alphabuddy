﻿#region Revision info
/*
 * $Author$
 * $Date$
 * $ID$
 * $Revision$
 * $URL$
 * $LastChangedBy$
 * $ChangesMade$
 */
#endregion

// This file was part of Singular - A community driven Honorbuddy CC

using System.ComponentModel;
using Styx.Helpers;
using CLU.Base;
using DefaultValue = Styx.Helpers.DefaultValueAttribute;


namespace CLU.Settings
{

    internal class MonkSettings : Styx.Helpers.Settings
    {
        public MonkSettings()
        : base(CLUSettings.SettingsPath + "_Monk.xml")
        {
        }

        #region Common

        [Setting]
        [DefaultValue(MonkLegacy.Emperor)]
        [Category("Common")]
        [DisplayName("Monk Legacy Selector")]
        [Description("Choose a Monk Legacy. This is on applicable to solo play (ie: not in party or raid.)")]
        public MonkLegacy LegacySelection
        {
            get;
            set;
        }

        [Setting]
        [DefaultValue(true)]
        [Category("WindWalker Spec")]
        [DisplayName("Fists Of Fury Enable/Disable")]
        [Description("This setting should be used in conjunction with the FistsKeybind to enable disable FoF during movement Phases.")]
        public bool EnableFists
        {
            get;
            set;
        }

        #endregion
    }
}