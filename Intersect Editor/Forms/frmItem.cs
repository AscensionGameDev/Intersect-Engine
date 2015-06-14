using System;
using System.Windows.Forms;
using System.Drawing;
using Intersect_Editor.Classes;

namespace Intersect_Editor.Forms
{
    public partial class FrmItem : Form
    {
        private ByteBuffer[] _itemsBackup;
        private bool[] _changed;
        private int _editorIndex;

        public FrmItem()
        {
            InitializeComponent();
        }

        private void frmItem_Load(object sender, EventArgs e)
        {
            lstItems.SelectedIndex = 0;
            cmbPic.Items.Clear();
            cmbPic.Items.Add("None");
            for (int i = 0; i < Intersect_Editor.Classes.Graphics.ItemNames.Length; i++)
            {
                cmbPic.Items.Add(Intersect_Editor.Classes.Graphics.ItemNames[i]);
            }
            UpdateEditor();
        }

        public void InitEditor()
        {
            _itemsBackup = new ByteBuffer[Constants.MaxItems];
            _changed = new bool[Constants.MaxItems];
            for (var i = 0; i < Constants.MaxItems; i++)
            {
                _itemsBackup[i] = new ByteBuffer();
                _itemsBackup[i].WriteBytes(Globals.Items[i].ItemData());
                lstItems.Items.Add((i + 1) + ") " + Globals.Items[i].Name);
                _changed[i] = false;
            }
        }

        private void UpdateEditor()
        {
            _editorIndex = lstItems.SelectedIndex;

            txtName.Text = Globals.Items[_editorIndex].Name;
            cmbType.SelectedIndex = Globals.Items[_editorIndex].Type;
            cmbPic.SelectedIndex = cmbPic.FindString(Globals.Items[_editorIndex].Pic);
            scrlPrice.Value = Globals.Items[_editorIndex].Price;
            scrlAnim.Value = Globals.Items[_editorIndex].Animation;
            scrlLevel.Value = Globals.Items[_editorIndex].LevelReq;
            cmbClass.SelectedIndex = Globals.Items[_editorIndex].ClassReq;
            scrlStrReq.Value = Globals.Items[_editorIndex].StatsReq[0];
            scrlMagReq.Value = Globals.Items[_editorIndex].StatsReq[1];
            scrlDefReq.Value = Globals.Items[_editorIndex].StatsReq[2];
            scrlMRReq.Value = Globals.Items[_editorIndex].StatsReq[3];
            scrlSpdReq.Value = Globals.Items[_editorIndex].StatsReq[4];
            scrlStr.Value = Globals.Items[_editorIndex].StatsGiven[0];
            scrlMag.Value = Globals.Items[_editorIndex].StatsGiven[1];
            scrlDef.Value = Globals.Items[_editorIndex].StatsGiven[2];
            scrlMR.Value = Globals.Items[_editorIndex].StatsGiven[3];
            scrlSpd.Value = Globals.Items[_editorIndex].StatsGiven[4];
            scrlDmg.Value = Globals.Items[_editorIndex].Damage;
            ScrlAtkSpd.Value = Globals.Items[_editorIndex].Speed;
            scrlRange.Value = Globals.Items[_editorIndex].StatGrowth;
            scrlTool.Value = Globals.Items[_editorIndex].Tool;
            cmbPaperdoll.SelectedIndex = cmbPaperdoll.FindString(Globals.Items[_editorIndex].Paperdoll);
            if (cmbPic.SelectedIndex > 0) { picItem.BackgroundImage = Bitmap.FromFile("Resources/Items/" + cmbPic.Text); }
            else { picItem.BackgroundImage = null; }
            _changed[_editorIndex] = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < Constants.MaxItems; i++)
            {
                if (_changed[i])
                {
                    PacketSender.SendItem(i, Globals.Items[i].ItemData());
                }
            }

            Hide();
            Dispose();
        }

        private void scrlInterval_Scroll(object sender, EventArgs e)
        {
            lblInterval.Text = @"Interval: " + scrlInterval.Value;
            Globals.Items[_editorIndex].Data2 = scrlInterval.Value;
        }

        private void lstItems_Click(object sender, EventArgs e)
        {
            UpdateEditor();
        }

        private void scrlLevel_Scroll(object sender, EventArgs e)
        {
            lblLevel.Text = @"Level: " + scrlLevel.Value;
            Globals.Items[_editorIndex].LevelReq = scrlLevel.Value;
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            gbConsumable.Visible = false;
            gbSpell.Visible = false;
            gbEquipment.Visible = false;

            if (cmbType.SelectedIndex == cmbType.Items.IndexOf("Consumable"))
            {
                cmbConsume.SelectedIndex = Globals.Items[_editorIndex].Data1;
                scrlInterval.Value = Globals.Items[_editorIndex].Data2;
                gbConsumable.Visible = true;
            }
            else if (cmbType.SelectedIndex == cmbType.Items.IndexOf("Spell"))
            {
                scrlSpell.Value = Globals.Items[_editorIndex].Data1;
                gbSpell.Visible = true;
            }
            else if (cmbType.SelectedIndex >= cmbType.Items.IndexOf("Weapon") && cmbType.SelectedIndex <= cmbType.Items.IndexOf("Shield"))
            {
                gbEquipment.Visible = true;
            }

            Globals.Items[_editorIndex].Type = cmbType.SelectedIndex;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var tempItem = new ItemStruct();
            var tempBuff = new ByteBuffer();
            tempBuff.WriteBytes(tempItem.ItemData());
            Globals.Items[_editorIndex].LoadItem(tempBuff);
            UpdateEditor();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < Constants.MaxItems; i++)
            {
                Globals.Items[i].LoadItem(_itemsBackup[i]);
            }

            Hide();
            Dispose();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            Globals.Items[_editorIndex].Name = txtName.Text;
            lstItems.Items[_editorIndex] = (_editorIndex + 1) + ") " + txtName.Text;
        }

        private void cmbPic_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.Items[_editorIndex].Pic = cmbPic.Text;
            if (cmbPic.SelectedIndex > 0) { picItem.BackgroundImage = Bitmap.FromFile("Resources/Items/" + cmbPic.Text); }
            else { picItem.BackgroundImage = null; }
        }

        private void scrlPrice_Scroll(object sender, EventArgs e)
        {
            lblPrice.Text = @"Price: " + scrlPrice.Value;
            Globals.Items[_editorIndex].Price = scrlPrice.Value;
        }

        private void scrlAnim_Scroll(object sender, EventArgs e)
        {
            lblAnim.Text = @"Animation: " + scrlAnim.Value + @" None";
            Globals.Items[_editorIndex].Animation = scrlAnim.Value;
        }

        private void scrlStrReq_Scroll(object sender, EventArgs e)
        {
            lblStrReq.Text = @"Strength: " + scrlStrReq.Value;
            Globals.Items[_editorIndex].StatsReq[0] = scrlStrReq.Value;
        }

        private void scrlMagReq_Scroll(object sender, EventArgs e)
        {
            lblMagReq.Text = @"Magic: " + scrlMagReq.Value;
            Globals.Items[_editorIndex].StatsReq[1] = scrlMagReq.Value;
        }

        private void scrlDefReq_Scroll(object sender, EventArgs e)
        {
            lblDefReq.Text = @"Armor: " + scrlDefReq.Value;
            Globals.Items[_editorIndex].StatsReq[2] = scrlDefReq.Value;
        }

        private void scrlMRReq_Scroll(object sender, EventArgs e)
        {
            lblMRReq.Text = @"Magic Resist: " + scrlMRReq.Value;
            Globals.Items[_editorIndex].StatsReq[3] = scrlMRReq.Value;
        }

        private void scrlSpdReq_Scroll(object sender, EventArgs e)
        {
            lblSpdReq.Text = @"Move Speed: " + scrlSpdReq.Value;
            Globals.Items[_editorIndex].StatsReq[4] = scrlSpdReq.Value;
        }

        private void cmbClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.Items[_editorIndex].ClassReq = cmbClass.SelectedIndex;
        }

        private void scrlSpell_Scroll(object sender, EventArgs e)
        {
            lblSpell.Text = @"Spell: " + scrlSpell.Value + @" None";
            Globals.Items[_editorIndex].Data1 = scrlSpell.Value;
        }

        private void cmbConsume_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.Items[_editorIndex].Data1 = cmbConsume.SelectedIndex;
        }

        private void scrlStr_Scroll(object sender, EventArgs e)
        {
            lblStr.Text = @"Strength: " + scrlStr.Value;
            Globals.Items[_editorIndex].StatsGiven[0] = scrlStr.Value;
        }

        private void scrlMag_Scroll(object sender, EventArgs e)
        {
            lblMag.Text = @"Magic: " + scrlMag.Value;
            Globals.Items[_editorIndex].StatsGiven[1] = scrlMag.Value;
        }

        private void scrlDef_Scroll(object sender, EventArgs e)
        {
            lblDef.Text = @"Armor: " + scrlDef.Value;
            Globals.Items[_editorIndex].StatsGiven[2] = scrlDef.Value;
        }

        private void scrlMR_Scroll(object sender, EventArgs e)
        {
            lblMR.Text = @"Magic Resist: " + scrlMR.Value;
            Globals.Items[_editorIndex].StatsGiven[3] = scrlMR.Value;
        }

        private void scrlSpd_Scroll(object sender, EventArgs e)
        {
            lblSpd.Text = @"Move Speed: " + scrlSpd.Value;
            Globals.Items[_editorIndex].StatsGiven[4] = scrlSpd.Value;
        }

        private void scrlDmg_Scroll(object sender, EventArgs e)
        {
            lblDmg.Text = @"Damage: " + scrlDmg.Value;
            Globals.Items[_editorIndex].Damage = scrlDmg.Value;
        }

        private void ScrlAtkSpd_Scroll(object sender, EventArgs e)
        {
            lblAtkSpd.Text = @"Attack Speed: " + ((decimal)ScrlAtkSpd.Value / 10) + @" Sec";
            Globals.Items[_editorIndex].Speed = ScrlAtkSpd.Value;
        }

        private void scrlTool_Scroll(object sender, EventArgs e)
        {
            lblTool.Text = @"Tool Index: " + scrlTool.Value;
            Globals.Items[_editorIndex].Tool = scrlTool.Value;
        }

        private void scrlRange_Scroll(object sender, EventArgs e)
        {
            lblRange.Text = @"Stat Bonus Range: +- " + scrlRange.Value;
            Globals.Items[_editorIndex].StatGrowth = scrlRange.Value;
        }

        private void cmbPaperdoll_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.Items[_editorIndex].Paperdoll = cmbPaperdoll.Text;
        }
    }
}
