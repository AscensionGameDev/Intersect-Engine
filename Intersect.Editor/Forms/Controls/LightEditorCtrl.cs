using System;
using System.Windows.Forms;

using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.GameObjects;

namespace Intersect.Editor.Forms.Controls
{

    public partial class LightEditorCtrl : UserControl
    {

        public bool CanClose = true;

        private LightBase mBackupLight;

        private LightBase mEditingLight;

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
            pnlLightColor.BackColor = System.Drawing.Color.FromArgb(
                tmpLight.Color.A, tmpLight.Color.R, tmpLight.Color.G, tmpLight.Color.B
            );

            if (!CanClose)
            {
                btnOkay.Hide();
            }

            InitLocalization();
        }

        private void InitLocalization()
        {
            grpLightEditor.Text = Strings.LightEditor.title;
            lblOffsetX.Text = Strings.LightEditor.xoffset;
            lblOffsetY.Text = Strings.LightEditor.yoffset;
            lblColor.Text = Strings.LightEditor.color;
            btnSelectLightColor.Text = Strings.LightEditor.selectcolor;
            lblIntensity.Text = Strings.LightEditor.intensity;
            lblSize.Text = Strings.LightEditor.size;
            lblExpandAmt.Text = Strings.LightEditor.expandamt;
            btnOkay.Text = Strings.LightEditor.save;
            btnCancel.Text = Strings.LightEditor.revert;
        }

        //Lights Tab
        private void btnLightEditorClose_Click(object sender, EventArgs e)
        {
            if (CanClose)
            {
                Visible = false;
            }

            if (mEditingLight == Globals.EditingLight)
            {
                Globals.EditingLight = null;
            }
        }

        private void btnLightEditorRevert_Click(object sender, EventArgs e)
        {
            if (mEditingLight != null)
            {
                mEditingLight.Intensity = mBackupLight.Intensity;
                mEditingLight.Size = mBackupLight.Size;
                mEditingLight.OffsetX = mBackupLight.OffsetX;
                mEditingLight.OffsetY = mBackupLight.OffsetY;
                if (mEditingLight == Globals.EditingLight)
                {
                    Globals.EditingLight = null;
                }
            }

            Graphics.TilePreviewUpdated = true;
            if (CanClose)
            {
                Visible = false;
            }
        }

        private void btnSelectLightColor_Click(object sender, EventArgs e)
        {
            colorDialog.Color = System.Drawing.Color.White;
            colorDialog.ShowDialog();
            pnlLightColor.BackColor = colorDialog.Color;
            mEditingLight.Color = Color.FromArgb(
                colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B
            );

            Graphics.TilePreviewUpdated = true;
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
            Graphics.TilePreviewUpdated = true;
        }

        private void nudOffsetY_ValueChanged(object sender, EventArgs e)
        {
            if (mEditingLight == null)
            {
                return;
            }

            mEditingLight.OffsetY = (int) nudOffsetY.Value;
            Graphics.TilePreviewUpdated = true;
        }

        private void nudSize_ValueChanged(object sender, EventArgs e)
        {
            if (mEditingLight == null)
            {
                return;
            }

            mEditingLight.Size = (int) nudSize.Value;
            Graphics.TilePreviewUpdated = true;
        }

        private void nudIntensity_ValueChanged(object sender, EventArgs e)
        {
            if (mEditingLight == null)
            {
                return;
            }

            mEditingLight.Intensity = (byte) nudIntensity.Value;
            Graphics.TilePreviewUpdated = true;
        }

        private void nudExpand_ValueChanged(object sender, EventArgs e)
        {
            if (mEditingLight == null)
            {
                return;
            }

            mEditingLight.Expand = (int) nudExpand.Value;
            Graphics.TilePreviewUpdated = true;
        }

    }

}
