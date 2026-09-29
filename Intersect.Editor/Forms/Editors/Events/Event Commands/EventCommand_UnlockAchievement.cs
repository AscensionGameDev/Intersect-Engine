using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Events.Commands;
using Intersect.Framework.Core.GameObjects.Achievements;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands;


public partial class EventCommandUnlockAchievement : UserControl
{

    private readonly FrmEvent mEventEditor;

    private UnlockAchievementCommand mMyCommand;

    public EventCommandUnlockAchievement(UnlockAchievementCommand refCommand, FrmEvent editor)
    {
        InitializeComponent();
        mMyCommand = refCommand;
        mEventEditor = editor;
        InitLocalization();

        // The achievements defined in the Achievement Editor; no "none" entry since unlocking is additive.
        cmbAchievement.Items.Clear();
        cmbAchievement.Items.AddRange(AchievementDescriptor.Names);
        cmbAchievement.SelectedIndex = Math.Max(0, AchievementDescriptor.ListIndex(mMyCommand.AchievementId));
    }

    private void InitLocalization()
    {
        grpAchievement.Text = Strings.EventUnlockAchievement.title;
        lblAchievement.Text = Strings.EventUnlockAchievement.label;
        btnSave.Text = Strings.EventUnlockAchievement.okay;
        btnCancel.Text = Strings.EventUnlockAchievement.cancel;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        mMyCommand.AchievementId = cmbAchievement.SelectedIndex >= 0 ? AchievementDescriptor.IdFromList(cmbAchievement.SelectedIndex) : Guid.Empty;

        mEventEditor.FinishCommandEdit();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        mEventEditor.CancelCommandEdit();
    }

}
