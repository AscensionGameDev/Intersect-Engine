using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Intersect_Editor
{
    public partial class frmItem : Form
    {
        private ByteBuffer[] ItemsBackup;
        private bool[] Changed;
        private int EditorIndex;

        public frmItem()
        {
            InitializeComponent();
        }

        public void initEditor()
        {
            ItemsBackup = new ByteBuffer[Constants.MAX_ITEMS];
            Changed = new bool[Constants.MAX_ITEMS];
            for (int i = 0; i < Constants.MAX_ITEMS; i++)
            {
                ItemsBackup[i] = new ByteBuffer();
                ItemsBackup[i].WriteBytes(Globals.Items[i].ItemData());
                lstItems.Items.Add((i + 1) + ") " + Globals.Items[i].Name);
                Changed[i] = false;
            }
        }

        private void UpdateEditor()
        {
            EditorIndex = lstItems.SelectedIndex;

            txtName.Text = Globals.Items[EditorIndex].Name;
            cmbType.SelectedIndex = Globals.Items[EditorIndex].Type;
            cmbPic.SelectedIndex = cmbPic.FindString(Globals.Items[EditorIndex].Pic);
            scrlPrice.Value = Globals.Items[EditorIndex].Price;
            scrlAnim.Value = Globals.Items[EditorIndex].Animation;
            scrlLevel.Value = Globals.Items[EditorIndex].LevelReq;
            cmbClass.SelectedIndex = Globals.Items[EditorIndex].ClassReq;
            scrlStrReq.Value = Globals.Items[EditorIndex].StatsReq[0];
            scrlMagReq.Value = Globals.Items[EditorIndex].StatsReq[1];
            scrlDefReq.Value = Globals.Items[EditorIndex].StatsReq[2];
            scrlMRReq.Value = Globals.Items[EditorIndex].StatsReq[3];
            scrlSpdReq.Value = Globals.Items[EditorIndex].StatsReq[4];
            scrlStr.Value = Globals.Items[EditorIndex].StatsGiven[0];
            scrlMag.Value = Globals.Items[EditorIndex].StatsGiven[1];
            scrlDef.Value = Globals.Items[EditorIndex].StatsGiven[2];
            scrlMR.Value = Globals.Items[EditorIndex].StatsGiven[3];
            scrlSpd.Value = Globals.Items[EditorIndex].StatsGiven[4];
            scrlDmg.Value = Globals.Items[EditorIndex].Damage;
            ScrlAtkSpd.Value = Globals.Items[EditorIndex].Speed;
            scrlRange.Value = Globals.Items[EditorIndex].StatGrowth;
            scrlTool.Value = Globals.Items[EditorIndex].Tool;
            cmbPaperdoll.SelectedIndex = cmbPaperdoll.FindString(Globals.Items[EditorIndex].Paperdoll);

            Changed[EditorIndex] = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Constants.MAX_ITEMS; i++)
            {
                if (Changed[i] == true)
                {
                    PacketSender.SendItem(i, Globals.Items[i].ItemData());
                }
            }

            Hide();
            this.Dispose();
        }

        private void scrlInterval_Scroll(object sender, EventArgs e)
        {
            lblInterval.Text = "Interval: " + scrlInterval.Value;
            Globals.Items[EditorIndex].Data2 = scrlInterval.Value;
        }

        private void lstItems_Click(object sender, EventArgs e)
        {
            UpdateEditor();
        }

        private void scrlLevel_Scroll(object sender, EventArgs e)
        {
            lblLevel.Text = "Level: " + scrlLevel.Value;
            Globals.Items[EditorIndex].LevelReq = scrlLevel.Value;
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            gbConsumable.Visible = false;
            gbSpell.Visible = false;
            gbEquipment.Visible = false;

            if (cmbType.SelectedIndex == cmbType.Items.IndexOf("Consumable"))
            {
                cmbConsume.SelectedIndex = Globals.Items[EditorIndex].Data1;
                scrlInterval.Value = Globals.Items[EditorIndex].Data2;
                gbConsumable.Visible = true;
            }
            else if (cmbType.SelectedIndex == cmbType.Items.IndexOf("Spell"))
            {
                scrlSpell.Value = Globals.Items[EditorIndex].Data1;
                gbSpell.Visible = true;
            }
            else if (cmbType.SelectedIndex >= cmbType.Items.IndexOf("Weapon") && cmbType.SelectedIndex <= cmbType.Items.IndexOf("Shield"))
            {
                gbEquipment.Visible = true;
            }

            Globals.Items[EditorIndex].Type = cmbType.SelectedIndex;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            Item TempItem = new Item();
            ByteBuffer TempBuff = new ByteBuffer();
            TempBuff.WriteBytes(TempItem.ItemData());
            Globals.Items[EditorIndex].LoadItem(TempBuff);
            UpdateEditor();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Constants.MAX_ITEMS; i++)
            {
                Globals.Items[i].LoadItem(ItemsBackup[i]);
            }

            Hide();
            this.Dispose();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            Globals.Items[EditorIndex].Name = txtName.Text;
        }

        private void cmbPic_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.Items[EditorIndex].Pic = cmbPic.Text;
        }

        private void scrlPrice_Scroll(object sender, EventArgs e)
        {
            lblPrice.Text = "Price: " + scrlPrice.Value;
            Globals.Items[EditorIndex].Price = scrlPrice.Value;
        }

        private void scrlAnim_Scroll(object sender, EventArgs e)
        {
            lblAnim.Text = "Animation: " + scrlAnim.Value + " None";
            Globals.Items[EditorIndex].Animation = scrlAnim.Value;
        }

        private void scrlStrReq_Scroll(object sender, EventArgs e)
        {
            lblStrReq.Text = "Strength: " + scrlStrReq.Value;
            Globals.Items[EditorIndex].StatsReq[0] = scrlStrReq.Value;
        }

        private void scrlMagReq_Scroll(object sender, EventArgs e)
        {
            lblMagReq.Text = "Magic: " + scrlMagReq.Value;
            Globals.Items[EditorIndex].StatsReq[1] = scrlMagReq.Value;
        }

        private void scrlDefReq_Scroll(object sender, EventArgs e)
        {
            lblDefReq.Text = "Armor: " + scrlDefReq.Value;
            Globals.Items[EditorIndex].StatsReq[2] = scrlDefReq.Value;
        }

        private void scrlMRReq_Scroll(object sender, EventArgs e)
        {
            lblMRReq.Text = "Magic Resist: " + scrlMRReq.Value;
            Globals.Items[EditorIndex].StatsReq[3] = scrlMRReq.Value;
        }

        private void scrlSpdReq_Scroll(object sender, EventArgs e)
        {
            lblSpdReq.Text = "Move Speed: " + scrlSpdReq.Value;
            Globals.Items[EditorIndex].StatsReq[4] = scrlSpdReq.Value;
        }

        private void cmbClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.Items[EditorIndex].ClassReq = cmbClass.SelectedIndex;
        }

        private void scrlSpell_Scroll(object sender, EventArgs e)
        {
            lblSpell.Text = "Spell: " + scrlSpell.Value + " None";
            Globals.Items[EditorIndex].Data1 = scrlSpell.Value;
        }

        private void cmbConsume_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.Items[EditorIndex].Data1 = cmbConsume.SelectedIndex;
        }

        private void scrlStr_Scroll(object sender, EventArgs e)
        {
            lblStr.Text = "Strength: " + scrlStr.Value;
            Globals.Items[EditorIndex].StatsGiven[0] = scrlStr.Value;
        }

        private void scrlMag_Scroll(object sender, EventArgs e)
        {
            lblMag.Text = "Magic: " + scrlMag.Value;
            Globals.Items[EditorIndex].StatsGiven[1] = scrlMag.Value;
        }

        private void scrlDef_Scroll(object sender, EventArgs e)
        {
            lblDef.Text = "Armor: " + scrlDef.Value;
            Globals.Items[EditorIndex].StatsGiven[2] = scrlDef.Value;
        }

        private void scrlMR_Scroll(object sender, EventArgs e)
        {
            lblMR.Text = "Magic Resist: " + scrlMR.Value;
            Globals.Items[EditorIndex].StatsGiven[3] = scrlMR.Value;
        }

        private void scrlSpd_Scroll(object sender, EventArgs e)
        {
            lblSpd.Text = "Move Speed: " + scrlSpd.Value;
            Globals.Items[EditorIndex].StatsGiven[4] = scrlSpd.Value;
        }

        private void scrlDmg_Scroll(object sender, EventArgs e)
        {
            lblDmg.Text = "Damage: " + scrlDmg.Value;
            Globals.Items[EditorIndex].Damage = scrlDmg.Value;
        }

        private void ScrlAtkSpd_Scroll(object sender, EventArgs e)
        {
            lblAtkSpd.Text = "Attack Speed: " + ((decimal)ScrlAtkSpd.Value / 10) + " Sec";
            Globals.Items[EditorIndex].Speed = ScrlAtkSpd.Value;
        }

        private void scrlTool_Scroll(object sender, EventArgs e)
        {
            lblTool.Text = "Tool Index: " + scrlTool.Value;
            Globals.Items[EditorIndex].Tool = scrlTool.Value;
        }

        private void scrlRange_Scroll(object sender, EventArgs e)
        {
            lblRange.Text = "Stat Bonus Range: +- " + scrlRange.Value;
            Globals.Items[EditorIndex].StatGrowth = scrlRange.Value;
        }

        private void cmbPaperdoll_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.Items[EditorIndex].Paperdoll = cmbPaperdoll.Text;
        }

        private void frmItem_Load(object sender, EventArgs e)
        {
            lstItems.SelectedIndex = 0;
            UpdateEditor();
        }
    }
}
