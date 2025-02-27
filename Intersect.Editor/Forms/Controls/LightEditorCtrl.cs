using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Lighting;
using Intersect.GameObjects;
using Graphics = Intersect.Editor.Core.Graphics;

namespace Intersect.Editor.Forms.Controls;


public partial class LightEditorCtrl : UserControl
{

    public bool CanClose = true;

    private LightDescriptor mBackupLight;

    private LightDescriptor mEditingLight;

    private ToolTip _tooltip = new ToolTip();

    public LightEditorCtrl()
    {
        InitializeComponent();
        if (!CanClose)
        {
            btnOkay.Visible = false;
        }
    }

    public void LoadEditor(LightDescriptor tmpLight)
    {
        mEditingLight = tmpLight;
        mBackupLight = new LightDescriptor(tmpLight);
        nudIntensity.Value = tmpLight.Intensity;
        nudSize.Value = tmpLight.Size;
        nudOffsetX.Value = tmpLight.OffsetX;
        nudOffsetY.Value = tmpLight.OffsetY;
        nudExpand.Value = (int)tmpLight.Expand;
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
        lblIntensity.Text = Strings.LightEditor.intensity;
        lblSize.Text = Strings.LightEditor.size;
        lblExpandAmt.Text = Strings.LightEditor.expandamt;
        _tooltip.SetToolTip(btnCancel, Strings.LightEditor.revert);
        _tooltip.SetToolTip(btnOkay, Strings.LightEditor.save);
        _tooltip.SetToolTip(btnSelectLightColor, Strings.LightEditor.SelectColor);
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
            LoadEditor(mEditingLight);
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

        mEditingLight.OffsetX = (int)nudOffsetX.Value;
        Graphics.TilePreviewUpdated = true;
    }

    private void nudOffsetY_ValueChanged(object sender, EventArgs e)
    {
        if (mEditingLight == null)
        {
            return;
        }

        mEditingLight.OffsetY = (int)nudOffsetY.Value;
        Graphics.TilePreviewUpdated = true;
    }

    private void nudSize_ValueChanged(object sender, EventArgs e)
    {
        if (mEditingLight == null)
        {
            return;
        }

        mEditingLight.Size = (int)nudSize.Value;
        Graphics.TilePreviewUpdated = true;
    }

    private void nudIntensity_ValueChanged(object sender, EventArgs e)
    {
        if (mEditingLight == null)
        {
            return;
        }

        mEditingLight.Intensity = (byte)nudIntensity.Value;
        Graphics.TilePreviewUpdated = true;
    }

    private void nudExpand_ValueChanged(object sender, EventArgs e)
    {
        if (mEditingLight == null)
        {
            return;
        }

        mEditingLight.Expand = (int)nudExpand.Value;
        Graphics.TilePreviewUpdated = true;
    }

}
