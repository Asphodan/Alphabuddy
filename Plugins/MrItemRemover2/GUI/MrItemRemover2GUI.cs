using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using Styx.Plugins;
using Styx;
using Styx.CommonBot;
using Styx.Common.Helpers;
using Styx.CommonBot.POI;
using Styx.CommonBot.Profiles;
using Styx.CommonBot.Database;
using Styx.Common;
using Styx.CommonBot.Frames;
using System.Diagnostics;
using Styx.WoWInternals.WoWObjects;
using Styx.WoWInternals;
using System.IO;


namespace MrItemRemover2.GUI
{
    public partial class MrItemRemover2GUI : Form
    {
        MrItemRemover2 Base = new MrItemRemover2();
        public static void slog(string format, params object[] args)
        { Logging.Write("[Mr.ItemRemover]:" + format, args); }

        public MrItemRemover2GUI()
        {
            InitializeComponent();

        }

        private string refreshImangePathName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format(@"Plugins/MrItemRemover2/ref.bmp"));
        private string GoldImangePathName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format(@"Plugins/MrItemRemover2/Gold2.bmp"));
        private void MrItemRemover2GUI_Load(object sender, EventArgs e)
        {
            Bitmap refresh = new Bitmap(refreshImangePathName);
            Bitmap GoldImg = new Bitmap(GoldImangePathName);
            GoldBox.Image = GoldImg;
            resf.Image = refresh;
            Base.MIRLoad();
            MrItemRemover2Settings.Instance.Load();
            SellList.Items.Clear();
            RemoveList.Items.Clear();
            ProtectedList.Items.Clear();
            GrayItems.Checked = MrItemRemover2Settings.Instance.DeleteAllGray;
            SellGray.Checked = MrItemRemover2Settings.Instance.SellGray;
            SellWhite.Checked = MrItemRemover2Settings.Instance.SellWhite;
            SellGreen.Checked = MrItemRemover2Settings.Instance.SellGreen;
            EnableRemove.Checked = MrItemRemover2Settings.Instance.EnableRemove;
            EnableOpen.Checked = MrItemRemover2Settings.Instance.EnableOpen;
            EnableSell.Checked = MrItemRemover2Settings.Instance.EnableSell;
            RemoveQItems.Checked = MrItemRemover2Settings.Instance.DeleteQuestItems;
            GoldGrays.Text = MrItemRemover2Settings.Instance.GoldGrays.ToString();
            SilverGrays.Text = MrItemRemover2Settings.Instance.SilverGrays.ToString();
            CopperGrays.Text = MrItemRemover2Settings.Instance.CopperGrays.ToString();
            Time.Value = MrItemRemover2Settings.Instance.Time;
            foreach (string itm in Base._ItemName)
            {
                RemoveList.Items.Add(itm);
            }
            foreach (string itm in Base._ItemNameSell)
            {
                SellList.Items.Add(itm);
            }
            foreach (string itm in Base._KeepList)
            {
                ProtectedList.Items.Add(itm);
            }
            foreach (string itm in Base._OpnList)
            {
                OpnList.Items.Add(itm);
            }
            ObjectManager.Update();
            foreach (WoWItem BagItem in StyxWoW.Me.BagItems)
            {
                if (BagItem.IsValid && BagItem.BagSlot != -1 && !MyBag.Items.Contains(BagItem.Name))
                {
                    MyBag.Items.Add(BagItem.Name);
                }
            }

        }

        private void AddToBagList_Click(object sender, EventArgs e)
        {
            if (InputAddToBagItem.Text != null)
            {

                MyBag.Items.Add(InputAddToBagItem.Text);
                slog("Added {0} to Inventory List", InputAddToBagItem.Text);
            }
        }

        private void RefreshBagItems_Click(object sender, EventArgs e)
        {
            MyBag.Items.Clear();
            ObjectManager.Update();
            foreach (WoWItem BagItem in Styx.StyxWoW.Me.BagItems)
            {
                if (BagItem.BagSlot != -1 && !MyBag.Items.Contains(BagItem.Name))
                {
                    MyBag.Items.Add(BagItem.Name);
                }
            }
        }

        private void AddToSellList_Click(object sender, EventArgs e)
        {
            if (MyBag.SelectedItems[0] != null)
            {
                SellList.Items.Add(MyBag.SelectedItem);
                Base._ItemNameSell.Add(MyBag.SelectedItem.ToString());
            }
        }

        private void AddToRemoveList_Click(object sender, EventArgs e)
        {
            if (MyBag.SelectedItems[0] != null)
            {
                RemoveList.Items.Add(MyBag.SelectedItem);
                Base._ItemName.Add(MyBag.SelectedItem.ToString());
            }
        }

        private void AddToProtList_Click(object sender, EventArgs e)
        {
            if (MyBag.SelectedItems[0] != null)
            {
                ProtectedList.Items.Add(MyBag.SelectedItem);
                Base._KeepList.Add(MyBag.SelectedItem.ToString());
            }
        }

        private void RemoveSellItem_Click(object sender, EventArgs e)
        {
            if (SellList.SelectedItem != null)
            {

                slog("{0} Removed", SellList.SelectedItem.ToString());
                Base._ItemNameSell.Remove(SellList.SelectedItem.ToString());
                SellList.Items.Remove(SellList.SelectedItem);
            }
        }

        private void RemoveRemoveItem_Click(object sender, EventArgs e)
        {
            if (RemoveList.SelectedItem != null)
            {

                slog("{0} Removed", RemoveList.SelectedItem.ToString());
                Base._ItemName.Remove(RemoveList.SelectedItem.ToString());
                RemoveList.Items.Remove(RemoveList.SelectedItem);
            }
        }

        private void RemoveProtectedItem_Click(object sender, EventArgs e)
        {
            if (ProtectedList.SelectedItem != null)
            {
                slog("{0} Removed", ProtectedList.SelectedItem.ToString());
                Base._KeepList.Remove(ProtectedList.SelectedItem.ToString());
                ProtectedList.Items.Remove(ProtectedList.SelectedItem);
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            Base.MIRSave();
            MrItemRemover2Settings.Instance.Save();
        }

        private void GrayItems_CheckedChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.DeleteAllGray = GrayItems.Checked;
        }

        private void RemoveQItems_CheckedChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.DeleteQuestItems = RemoveQItems.Checked;
        }

        private void SellGray_CheckedChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.SellGray = SellGray.Checked;
        }

        private void SellGreen_CheckedChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.SellGreen = SellGreen.Checked;
        }

        private void SellWhite_CheckedChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.SellWhite = SellWhite.Checked;
        }

        private void EnableSell_CheckedChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.EnableSell = EnableSell.Checked;
        }

        private void EnableRemove_CheckedChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.EnableRemove = EnableRemove.Checked;
        }

        private void Time_ValueChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.Time = int.Parse(Time.Value.ToString());
        }

        private void GoldGrays_TextChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.GoldGrays = int.Parse(GoldGrays.Text);
        }

        private void SilverGrays_TextChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.SilverGrays = int.Parse(SilverGrays.Text);
        }

        private void CopperGrays_TextChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.CopperGrays = int.Parse(CopperGrays.Text);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
           
        }

        private void RemoveOpenItem_Click(object sender, EventArgs e)
        {
            if (OpnList.SelectedItem != null)
            {

                slog("{0} Removed", OpnList.SelectedItem.ToString());
                Base._ItemName.Remove(OpnList.SelectedItem.ToString());
                OpnList.Items.Remove(OpnList.SelectedItem);
            }
        }

        private void EnableOpen_CheckedChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.EnableOpen = EnableOpen.Checked;
        }

        private void resf_Click(object sender, EventArgs e)
        {
            MyBag.Items.Clear();
            ObjectManager.Update();
            foreach (WoWItem BagItem in Styx.StyxWoW.Me.BagItems)
            {
                if (BagItem.BagSlot != -1 && !MyBag.Items.Contains(BagItem.Name))
                {
                    MyBag.Items.Add(BagItem.Name);
                }
            }
        }

        private void AddToOpnList_Click(object sender, EventArgs e)
        {
            if (MyBag.SelectedItems[0] != null)
            {
                OpnList.Items.Add(MyBag.SelectedItem);
                Base._OpnList.Add(MyBag.SelectedItem.ToString());
            }
        }

        private void Run_Click(object sender, EventArgs e)
        {
            Logging.Write("Checking Bag Items Manually");
            Base.CheckForItems();
            Base.OpenBagItems();
        }

      
     

    

    }
}
