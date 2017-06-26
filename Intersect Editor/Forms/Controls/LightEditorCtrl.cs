using System;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.GameObjects;
using Intersect.Localization;

namespace Intersect.Editor.Forms.Controls
{
    public partial class LightEditorCtrl : UserControl
    {
        private LightBase _backupLight;
        private LightBase _editingLight;
        public bool CanClose = true;

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
            nudIntensity.Value = tmpLight.Intensity;
            nudSize.Value = tmpLight.Size;
            nudOffsetX.Value = tmpLight.OffsetX;
            nudOffsetY.Value = tmpLight.OffsetY;
            nudExpand.Value = (int) tmpLight.Expand;
            pnlLightColor.BackColor = System.Drawing.Color.FromArgb(tmpLight.Color.A, tmpLight.Color.R, tmpLight.Color.G,
                tmpLight.Color.B);
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
        private void btnLightEditorClose_Click(object sender, EventArgs e)
        {
            if (CanClose) Visible = false;
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
            EditorGraphics.TilePreviewUpdated = true;
            if (CanClose) Visible = false;
        }

        private void btnSelectLightColor_Click(object sender, EventArgs e)
        {
            colorDialog.Color = System.Drawing.Color.White;
            colorDialog.ShowDialog();
            pnlLightColor.BackColor = colorDialog.Color;
            _editingLight.Color = Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G,
                colorDialog.Color.B);
            EditorGraphics.TilePreviewUpdated = true;
        }

        public void Cancel()
        {
            btnLightEditorClose_Click(null, null);
        }

        private void nudOffsetX_ValueChanged(object sender, EventArgs e)
        {
            if (_editingLight == null)
            {
                return;
            }
            _editingLight.OffsetX = (int) nudOffsetX.Value;
            EditorGraphics.TilePreviewUpdated = true;
        }

        private void nudOffsetY_ValueChanged(object sender, EventArgs e)
        {
            if (_editingLight == null)
            {
                return;
            }
            _editingLight.OffsetY = (int) nudOffsetY.Value;
            EditorGraphics.TilePreviewUpdated = true;
        }

        private void nudSize_ValueChanged(object sender, EventArgs e)
        {
            if (_editingLight == null)
            {
                return;
            }
            _editingLight.Size = (int) nudSize.Value;
            EditorGraphics.TilePreviewUpdated = true;
        }

        private void nudIntensity_ValueChanged(object sender, EventArgs e)
        {
            if (_editingLight == null)
            {
                return;
            }
            _editingLight.Intensity = (byte) nudIntensity.Value;
            EditorGraphics.TilePreviewUpdated = true;
        }

        private void nudExpand_ValueChanged(object sender, EventArgs e)
        {
            if (_editingLight == null)
            {
                return;
            }
            _editingLight.Expand = (int) nudExpand.Value;
            EditorGraphics.TilePreviewUpdated = true;
        }
    }
}