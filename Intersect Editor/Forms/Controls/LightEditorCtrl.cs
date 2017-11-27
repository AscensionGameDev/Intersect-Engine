using System;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.GameObjects;
using Intersect.Editor.Classes.Localization;

namespace Intersect.Editor.Forms.Controls
{
    public partial class LightEditorCtrl : UserControl
    {
        private LightBase mBackupLight;
        private LightBase mEditingLight;
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
            mEditingLight = tmpLight;
            mBackupLight = new LightBase(tmpLight);
            nudIntensity.Value = tmpLight.Intensity;
            nudSize.Value = tmpLight.Size;
            nudOffsetX.Value = tmpLight.OffsetX;
            nudOffsetY.Value = tmpLight.OffsetY;
            nudExpand.Value = (int) tmpLight.Expand;
            pnlLightColor.BackColor = System.Drawing.Color.FromArgb(tmpLight.Color.A, tmpLight.Color.R,
                tmpLight.Color.G,
                tmpLight.Color.B);
            if (!CanClose) btnOkay.Hide();
            InitLocalization();
        }

        private void InitLocalization()
        {
            grpLightEditor.Text = Strings.lighteditor.title;
            lblOffsetX.Text = Strings.lighteditor.xoffset;
            lblOffsetY.Text = Strings.lighteditor.yoffset;
            lblColor.Text = Strings.lighteditor.color;
            btnSelectLightColor.Text = Strings.lighteditor.selectcolor;
            lblIntensity.Text = Strings.lighteditor.intensity;
            lblSize.Text = Strings.lighteditor.size;
            lblExpandAmt.Text = Strings.lighteditor.expandamt;
            btnOkay.Text = Strings.lighteditor.save;
            btnCancel.Text = Strings.lighteditor.revert;
        }

        //Lights Tab
        private void btnLightEditorClose_Click(object sender, EventArgs e)
        {
            if (CanClose) Visible = false;
            if (mEditingLight == Globals.EditingLight) Globals.EditingLight = null;
        }

        private void btnLightEditorRevert_Click(object sender, EventArgs e)
        {
            if (mEditingLight != null)
            {
                mEditingLight.Intensity = mBackupLight.Intensity;
                mEditingLight.Size = mBackupLight.Size;
                mEditingLight.OffsetX = mBackupLight.OffsetX;
                mEditingLight.OffsetY = mBackupLight.OffsetY;
                if (mEditingLight == Globals.EditingLight) Globals.EditingLight = null;
            }
            EditorGraphics.TilePreviewUpdated = true;
            if (CanClose) Visible = false;
        }

        private void btnSelectLightColor_Click(object sender, EventArgs e)
        {
            colorDialog.Color = System.Drawing.Color.White;
            colorDialog.ShowDialog();
            pnlLightColor.BackColor = colorDialog.Color;
            mEditingLight.Color = Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G,
                colorDialog.Color.B);
            EditorGraphics.TilePreviewUpdated = true;
        }

        public void Cancel()
        {
            btnLightEditorClose_Click(null, null);
        }

        private void nudOffsetX_ValueChanged(object sender, EventArgs e)
        {
            if (mEditingLight == null)
            {
                return;
            }
            mEditingLight.OffsetX = (int) nudOffsetX.Value;
            EditorGraphics.TilePreviewUpdated = true;
        }

        private void nudOffsetY_ValueChanged(object sender, EventArgs e)
        {
            if (mEditingLight == null)
            {
                return;
            }
            mEditingLight.OffsetY = (int) nudOffsetY.Value;
            EditorGraphics.TilePreviewUpdated = true;
        }

        private void nudSize_ValueChanged(object sender, EventArgs e)
        {
            if (mEditingLight == null)
            {
                return;
            }
            mEditingLight.Size = (int) nudSize.Value;
            EditorGraphics.TilePreviewUpdated = true;
        }

        private void nudIntensity_ValueChanged(object sender, EventArgs e)
        {
            if (mEditingLight == null)
            {
                return;
            }
            mEditingLight.Intensity = (byte) nudIntensity.Value;
            EditorGraphics.TilePreviewUpdated = true;
        }

        private void nudExpand_ValueChanged(object sender, EventArgs e)
        {
            if (mEditingLight == null)
            {
                return;
            }
            mEditingLight.Expand = (int) nudExpand.Value;
            EditorGraphics.TilePreviewUpdated = true;
        }
    }
}