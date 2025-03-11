using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;
using Intersect.Framework.Core.GameObjects.Maps;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

public partial class ConditionControl_MapZone : UserControl
{
    public ConditionControl_MapZone()
    {
        InitializeComponent();
        InitLocalization();
    }

    public void InitLocalization()
    {
        grpMapZoneType.Text = Strings.EventConditional.MapZoneTypeIs;
        lblMapZoneType.Text = Strings.EventConditional.MapZoneTypeLabel;
        cmbMapZoneType.Items.Clear();
        for (var i = 0; i < Strings.MapProperties.zones.Count; i++)
        {
            cmbMapZoneType.Items.Add(Strings.MapProperties.zones[i]);
        }
    }

    public void SetupFormValues(MapZoneTypeIs condition)
    {
        if (cmbMapZoneType.Items.Count > 0)
        {
            cmbMapZoneType.SelectedIndex = (int)condition.ZoneType;
        }
    }

    public void SaveFormValues(MapZoneTypeIs condition)
    {
        if (cmbMapZoneType.Items.Count > 0)
        {
            condition.ZoneType = (MapZone)cmbMapZoneType.SelectedIndex;
        }
    }
}
