﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Styx.Helpers;
using Styx.Common;
using System.IO;
using System.Windows.Forms;
using Styx;

namespace MrItemRemover2
{
    
        public class MrItemRemover2Settings : Settings
        {
            public static readonly MrItemRemover2Settings Instance = new MrItemRemover2Settings();
            private MrItemRemover2Settings() : base(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Settings/MrItemRemover2_Settings_{0}.xml", StyxWoW.Me.Name.ToString())) { }

            [Setting, Styx.Helpers.DefaultValue(5)]
            public int Time { get; set; }

            [Setting, Styx.Helpers.DefaultValue(false)]
            public bool EnableRemove { get; set; }

            [Setting, Styx.Helpers.DefaultValue(false)]
            public bool EnableOpen { get; set; }

            [Setting, Styx.Helpers.DefaultValue(false)]
            public bool DeleteQuestItems { get; set; }

            [Setting, Styx.Helpers.DefaultValue(false)]
            public bool EnableSell { get; set; }

            [Setting, Styx.Helpers.DefaultValue(true)]
            public bool EnableGray { get; set; }

            [Setting, Styx.Helpers.DefaultValue(false)]
            public bool SellWhite { get; set; }

            [Setting, Styx.Helpers.DefaultValue(false)]
            public bool SellGreen { get; set; }

            [Setting, Styx.Helpers.DefaultValue(true)]
            public bool SellGray { get; set; }

            [Setting, Styx.Helpers.DefaultValue(true)]
            public bool DeleteAllGray { get; set; }

            [Setting, Styx.Helpers.DefaultValue(1)]
            public int GoldGrays { get; set; }

            [Setting, Styx.Helpers.DefaultValue(22)]
            public int SilverGrays { get; set; }

            [Setting, Styx.Helpers.DefaultValue(95)]
            public int CopperGrays { get; set; }
        }
    
}
