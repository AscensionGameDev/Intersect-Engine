using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Events.Commands;
using Intersect.GameObjects;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands;


public partial class EventCommandEndQuest : UserControl
{

    private readonly FrmEvent mEventEditor;

    private EndQuestCommand mMyCommand;

    public EventCommandEndQuest(EndQuestCommand refCommand, FrmEvent editor)
    {
        InitializeComponent();
        mMyCommand = refCommand;
        mEventEditor = editor;
        InitLocalization();
        cmbQuests.Items.Clear();
        cmbQuests.Items.AddRange(QuestDescriptor.Names);
        cmbQuests.SelectedIndex = QuestDescriptor.ListIndex(refCommand.QuestId);
        chkSkipCompletionEvent.Checked = refCommand.SkipCompletionEvent;
    }

    private void InitLocalization()
    {
        grpEndQuest.Text = Strings.EventEndQuest.title;
        lblQuest.Text = Strings.EventEndQuest.label;
        chkSkipCompletionEvent.Text = Strings.EventEndQuest.skipcompletion;
        btnSave.Text = Strings.EventEndQuest.okay;
        btnCancel.Text = Strings.EventEndQuest.cancel;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        mMyCommand.QuestId = QuestDescriptor.IdFromList(cmbQuests.SelectedIndex);
        mMyCommand.SkipCompletionEvent = chkSkipCompletionEvent.Checked;
        mEventEditor.FinishCommandEdit();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        mEventEditor.CancelCommandEdit();
    }

}
