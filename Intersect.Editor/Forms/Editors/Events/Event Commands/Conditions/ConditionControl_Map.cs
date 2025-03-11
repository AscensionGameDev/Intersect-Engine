using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

public partial class ConditionControl_Map : UserControl
{
    public ConditionControl_Map()
    {
        InitializeComponent();
        InitLocalization();
    }

    public void InitLocalization()
    {
        grpMapIs.Text = Strings.EventConditional.mapis;
        btnSelectMap.Text = Strings.EventConditional.selectmap;
    }

    public void SetupFormValues(MapIsCondition condition)
    {
        btnSelectMap.Tag = condition.MapId;
    }

    public void SaveFormValues(MapIsCondition condition)
    {
        condition.MapId = btnSelectMap.Tag as Guid? ?? Guid.Empty;
    }

    private void btnSelectMap_Click(object sender, EventArgs e)
    {
        var frmWarpSelection = new FrmWarpSelection();
        frmWarpSelection.InitForm(false, null);
        frmWarpSelection.SelectTile(btnSelectMap.Tag as Guid? ?? Guid.Empty, 0, 0);
        frmWarpSelection.TopMost = true;
        frmWarpSelection.ShowDialog();
        if (frmWarpSelection.GetResult())
        {
            btnSelectMap.Tag = frmWarpSelection.GetMap();
        }
    }
}
