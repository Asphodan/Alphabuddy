using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using BuddyGrowl;

namespace BuddyGrowl
{
    public partial class BuddyGrowlForm : Form
    {
        private static LocalPlayer intMe = StyxWoW.Me;

        public BuddyGrowlForm()
        {
            InitializeComponent();
        }

        private void BuddyGrowlForm_Load(object sender, EventArgs e)
        {
            pgSettings.SelectedObject = BuddyGrowlSettings.Instance;
        }

        private void pgSettings_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (pgSettings.SelectedObject != null && pgSettings.SelectedObject is BuddyGrowlSettings)
                ((BuddyGrowlSettings)pgSettings.SelectedObject).Save();
            var settings = BuddyGrowlSettings.Instance;
            BuddyGrowl.Whisper = settings.Whisper;
            BuddyGrowl.Officer = settings.Officer;
            BuddyGrowl.Battleground = settings.Battleground;
            BuddyGrowl.Guild = settings.Guild;
            BuddyGrowl.Party = settings.Party;
            BuddyGrowl.Yell = settings.Yell;
            BuddyGrowl.Raid = settings.Raid;
            BuddyGrowl.Say = settings.Say;
            BuddyGrowl.Death = settings.Death;
        }
    }
}
