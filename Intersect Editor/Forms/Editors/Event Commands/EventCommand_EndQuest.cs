using System;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.Editor.Classes.Localization;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandEndQuest : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EventCommand mMyCommand;

        public EventCommandEndQuest(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            cmbQuests.Items.Clear();
            cmbQuests.Items.AddRange(Database.GetGameObjectList(GameObjectType.Quest));
            cmbQuests.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Quest, refCommand.Ints[0]);
            chkRunCompletionTask.Checked = Convert.ToBoolean(refCommand.Ints[1]);
        }

        private void InitLocalization()
        {
            grpEndQuest.Text = Strings.eventendquest.title;
            lblQuest.Text = Strings.eventendquest.label;
            chkRunCompletionTask.Text = Strings.eventendquest.skipcompletion;
            btnSave.Text = Strings.eventendquest.okay;
            btnCancel.Text = Strings.eventendquest.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Ints[0] = Database.GameObjectIdFromList(GameObjectType.Quest, cmbQuests.SelectedIndex);
            mMyCommand.Ints[1] = Convert.ToInt32(chkRunCompletionTask.Checked);
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }
    }
}