using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;
using Intersect.Framework.Core.GameObjects.Dungeons;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

public partial class ConditionControl_InDungeon : UserControl
{
    public ConditionControl_InDungeon()
    {
        InitializeComponent();
        InitLocalization();
    }

    public void InitLocalization()
    {
        grpDungeon.Text = Strings.EventConditional.InDungeon;
        lblDungeon.Text = Strings.EventConditional.InDungeonLabel;
        cmbDungeon.Items.Clear();
        // First entry: any dungeon.
        cmbDungeon.Items.Add(Strings.EventConditional.AnyDungeon);
        cmbDungeon.Items.AddRange(DungeonDescriptor.Names);
    }

    public void SetupFormValues(InDungeonCondition condition)
    {
        cmbDungeon.SelectedIndex = condition.DungeonId == Guid.Empty ? 0 : DungeonDescriptor.ListIndex(condition.DungeonId) + 1;
        if (cmbDungeon.SelectedIndex < 0)
        {
            cmbDungeon.SelectedIndex = 0;
        }
    }

    public void SaveFormValues(InDungeonCondition condition)
    {
        condition.DungeonId = cmbDungeon.SelectedIndex > 0 ? DungeonDescriptor.IdFromList(cmbDungeon.SelectedIndex - 1) : Guid.Empty;
    }
}
