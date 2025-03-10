using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;
using Intersect.GameObjects;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

public partial class ConditionControl_TimeBetween : UserControl
{
    public ConditionControl_TimeBetween()
    {
        InitializeComponent();
        InitLocalization();
    }

    public void InitLocalization()
    {
        grpTime.Text = Strings.EventConditional.time;
        lblStartRange.Text = Strings.EventConditional.startrange;
        lblEndRange.Text = Strings.EventConditional.endrange;
        lblAnd.Text = Strings.EventConditional.and;
    }

    public void SetupFormValues(TimeBetweenCondition condition)
    {
        cmbTime1.SelectedIndex = Math.Min(condition.Ranges[0], cmbTime1.Items.Count - 1);
        cmbTime2.SelectedIndex = Math.Min(condition.Ranges[1], cmbTime2.Items.Count - 1);

        if (cmbTime1.SelectedIndex == -1 && cmbTime1.Items.Count > 0)
        {
            cmbTime1.SelectedIndex = 0;
        }

        if (cmbTime2.SelectedIndex == -1 && cmbTime2.Items.Count > 0)
        {
            cmbTime2.SelectedIndex = 0;
        }
    }

    public void SaveFormValues(TimeBetweenCondition condition)
    {
        condition.Ranges[0] = cmbTime1.SelectedIndex;
        condition.Ranges[1] = cmbTime2.SelectedIndex;
    }

    public new void Show()
    {
        cmbTime1.Items.Clear();
        cmbTime2.Items.Clear();
        var time = new DateTime(2000, 1, 1, 0, 0, 0);
        for (var i = 0; i < 1440; i += DaylightCycleDescriptor.Instance.RangeInterval)
        {
            var addRange = time.ToString("h:mm:ss tt") + " " + Strings.EventConditional.to + " ";
            time = time.AddMinutes(DaylightCycleDescriptor.Instance.RangeInterval);
            addRange += time.ToString("h:mm:ss tt");
            cmbTime1.Items.Add(addRange);
            cmbTime2.Items.Add(addRange);
        }

        base.Show();
    }
}
