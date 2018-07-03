using System;
using System.Windows.Forms;
using Intersect.Editor.Core;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
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
            cmbQuests.Items.AddRange(QuestBase.Names);
            cmbQuests.SelectedIndex = QuestBase.ListIndex(refCommand.QuestId);
            chkRunCompletionTask.Checked = !refCommand.SkipCompletionEvent;
        }

        private void InitLocalization()
        {
            grpEndQuest.Text = Strings.EventEndQuest.title;
            lblQuest.Text = Strings.EventEndQuest.label;
            chkRunCompletionTask.Text = Strings.EventEndQuest.skipcompletion;
            btnSave.Text = Strings.EventEndQuest.okay;
            btnCancel.Text = Strings.EventEndQuest.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.QuestId = QuestBase.IdFromList(cmbQuests.SelectedIndex);
            mMyCommand.SkipCompletionEvent = !chkRunCompletionTask.Checked;
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }
    }
}