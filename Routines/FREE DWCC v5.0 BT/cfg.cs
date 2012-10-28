using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DWCC5Free
{
    public partial class cfg : Form
    {
        public cfg()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.thebuddyforum.com/honorbuddy-forum/classes/warrior/68956-dunatanks-warrior-cc-arms-fury-protection-cc.html#post711931");
        }

        private void cfg_Load(object sender, EventArgs e)
        {
            pictureBox1.ImageLocation = Styx.Common.Utilities.AssemblyDirectory + @"\Routines\FREE DWCC v5.0 BT\utils\cfg.png"; ;
        }
    }
}
