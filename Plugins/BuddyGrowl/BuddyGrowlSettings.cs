using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms.Design;
using Styx;
using Styx.Common;
using Styx.Helpers;
using DefaultValue = Styx.Helpers.DefaultValueAttribute;

namespace BuddyGrowl
{
    public class BuddyGrowlSettings : Settings
    {
        private static BuddyGrowlSettings _instance;

        public BuddyGrowlSettings()
            : base(Path.Combine(Path.Combine(Utilities.AssemblyDirectory, "Settings"), string.Format("BuddyGrowlSettings_{0}.xml", StyxWoW.Me.Name)))
        {

        }

        public static BuddyGrowlSettings Instance { get { return _instance ?? (_instance = new BuddyGrowlSettings()); } }

        [Setting]
        [DefaultValue(@"C:\Program Files (x86)\Growl for Windows")]
        [Category("Growl")]
        [DisplayName("Buddy Growl Path")]
        [Description("Buddy Growl Path")]
        [EditorAttribute(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string growlpath { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Chat Types")]
        [DisplayName("Whisper")]
        [Description("")]
        public bool Whisper { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Chat Types")]
        [DisplayName("Officer")]
        [Description("")]
        public bool Officer { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Chat Types")]
        [DisplayName("Battleground")]
        [Description("")]
        public bool Battleground { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Chat Types")]
        [DisplayName("Guild")]
        [Description("")]
        public bool Guild { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Chat Types")]
        [DisplayName("Party")]
        [Description("")]
        public bool Party { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Chat Types")]
        [DisplayName("Yell")]
        [Description("")]
        public bool Yell { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Chat Types")]
        [DisplayName("Raid")]
        [Description("")]
        public bool Raid { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Chat Types")]
        [DisplayName("Say")]
        [Description("")]
        public bool Say { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Death Types")]
        [DisplayName("Died")]
        [Description("Send Growl Events On Death")]
        public bool Death { get; set; }

        [Setting]
        [DefaultValue("guild")]
        [Category("Guild Invites")]
        [DisplayName("Trigger")]
        [Description("Guild invite trigger")]
        public string trigger { get; set; }

    }
}