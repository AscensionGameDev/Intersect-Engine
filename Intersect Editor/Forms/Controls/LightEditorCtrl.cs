using System;
using System.Windows.Forms;
using DarkUI.Controls;
using Intersect_Editor.Classes;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.Localization;

namespace Intersect_Editor.Forms.Controls
{
    public partial class LightEditorCtrl : UserControl
    {
        public bool CanClose = true;
        private LightBase _editingLight;
        private LightBase _backupLight;

        public LightEditorCtrl()
        {
            InitializeComponent();
            if (!CanClose)
            {
                btnOkay.Visible = false;
            }
        }

        public void LoadEditor(LightBase tmpLight)
        {
            _editingLight = tmpLight;
            _backupLight = new LightBase(tmpLight);
            txtLightIntensity.Text = "" + tmpLight.Intensity;
            txtLightRange.Text = "" + tmpLight.Size;
            txtLightOffsetX.Text = "" + tmpLight.OffsetX;
            txtLightOffsetY.Text = "" + tmpLight.OffsetY;
            scrlLightIntensity.Value = tmpLight.Intensity;
            txtLightExpandAmt.Text = "" + tmpLight.Expand;
            scrlLightSize.Value = tmpLight.Size;
            pnlLightColor.BackColor = System.Drawing.Color.FromArgb(tmpLight.Color.A, tmpLight.Color.R, tmpLight.Color.G, tmpLight.Color.B);
            if (!CanClose) btnOkay.Hide();
            InitLocalization();
        }

        private void InitLocalization()
        {
            grpLightEditor.Text = Strings.Get("lighteditor", "title");
            lblOffsetX.Text = Strings.Get("lighteditor", "xoffset");
            lblOffsetY.Text = Strings.Get("lighteditor", "yoffset");
            lblColor.Text = Strings.Get("lighteditor", "color");
            btnSelectLightColor.Text = Strings.Get("lighteditor", "selectcolor");
            lblIntensity.Text = Strings.Get("lighteditor", "intensity");
            lblSize.Text = Strings.Get("lighteditor", "size");
            lblExpandAmt.Text = Strings.Get("lighteditor", "expandamt");
            btnOkay.Text = Strings.Get("lighteditor", "save");
            btnCancel.Text = Strings.Get("lighteditor", "revert");
        }

        //Lights Tab
        private void txtLightOffsetY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (_editingLight == null)
                {
                    return;
                }
                int offsetY = 0;
                if (Int32.TryParse(txtLightOffsetY.Text, out offsetY))
                {
                    _editingLight.OffsetY = offsetY;
                }
                Classes.EditorGraphics.TilePreviewUpdated = true;
            }
            catch (Exception)
            {
                //ignore
            }
        }
        private void txtLightOffsetX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (_editingLight == null)
                {
                    return;
                }
                int offsetX = 0;
                if (Int32.TryParse(txtLightOffsetX.Text, out offsetX))
                {
                    _editingLight.OffsetX = offsetX;
                }
                Classes.EditorGraphics.TilePreviewUpdated = true;
            }
            catch (Exception)
            {
                //ignore
            }
        }
        private void scrlLightSize_Scroll(object sender, ScrollValueEventArgs e)
        {
            if (_editingLight == null) { return; }
            txtLightRange.Text = "" + _editingLight.Size;
            _editingLight.Size = scrlLightSize.Value;
            Classes.EditorGraphics.TilePreviewUpdated = true;
        }
        private void scrlLightIntensity_Scroll(object sender, ScrollValueEventArgs e)
        {
            if (_editingLight == null) { return; }
            _editingLight.Intensity = (byte)scrlLightIntensity.Value;
            txtLightIntensity.Text = "" + _editingLight.Intensity;
            Classes.EditorGraphics.TilePreviewUpdated = true;
        }
        private void txtLightIntensity_TextChanged(object sender, EventArgs e)
        {
            if (_editingLight == null) { return; }
            try
            {
                var intensity = Convert.ToByte(txtLightIntensity.Text);
                _editingLight.Intensity = intensity;
                txtLightIntensity.Text = "" + _editingLight.Intensity;
                scrlLightIntensity.Value = intensity;
                Classes.EditorGraphics.TilePreviewUpdated = true;
            }
            catch (Exception)
            {
                //ignore
            }
        }
        private void txtLightRange_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (_editingLight == null)
                {
                    return;
                }
                var range = Convert.ToInt32(txtLightRange.Text);
                if (range < 0)
                {
                    range = 0;
                }
                if (range > scrlLightSize.Maximum)
                {
                    range = scrlLightSize.Maximum;
                }
                _editingLight.Size = range;
                Classes.EditorGraphics.TilePreviewUpdated = true;
                txtLightRange.Text = "" + range;
            }
            catch (Exception)
            {
                //ignore
            }
        }
        private void btnLightEditorClose_Click(object sender, EventArgs e)
        {
            if (CanClose) this.Visible = false;
            if (_editingLight == Globals.EditingLight) Globals.EditingLight = null;
        }
        private void btnLightEditorRevert_Click(object sender, EventArgs e)
        {
            if (_editingLight != null)
            {
                _editingLight.Intensity = _backupLight.Intensity;
                _editingLight.Size = _backupLight.Size;
                _editingLight.OffsetX = _backupLight.OffsetX;
                _editingLight.OffsetY = _backupLight.OffsetY;
                if (_editingLight == Globals.EditingLight) Globals.EditingLight = null;
            }
            Classes.EditorGraphics.TilePreviewUpdated = true;
            if (CanClose) this.Visible = false;
        }
        private void btnSelectLightColor_Click(object sender, EventArgs e)
        {
            colorDialog.Color = System.Drawing.Color.White;
            colorDialog.ShowDialog();
            pnlLightColor.BackColor = colorDialog.Color;
            _editingLight.Color = Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
            Classes.EditorGraphics.TilePreviewUpdated = true;
        }

        public void Cancel()
        {
            btnLightEditorClose_Click(null, null);
        }

        private void scrlLightExpand_Scroll(object sender, ScrollValueEventArgs e)
        {
            if (_editingLight == null) { return; }
            _editingLight.Expand = scrlLightExpand.Value;
            txtLightExpandAmt.Text = "" + _editingLight.Expand;
            Classes.EditorGraphics.TilePreviewUpdated = true;
        }

        private void txtLightExpandAmt_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (_editingLight == null)
                {
                    return;
                }
                var expand = Convert.ToInt32(txtLightExpandAmt.Text);
                if (expand < 0)
                {
                    expand = 0;
                }
                if (expand > scrlLightExpand.Maximum)
                {
                    expand = scrlLightExpand.Maximum;
                }
                _editingLight.Expand = expand;
                Classes.EditorGraphics.TilePreviewUpdated = true;
                txtLightExpandAmt.Text = "" + expand;
            }
            catch (Exception)
            {
                //ignore
            }
        }

        private void scrlLightIntensity_Click(object sender, EventArgs e)
        {

        }
    }
}
