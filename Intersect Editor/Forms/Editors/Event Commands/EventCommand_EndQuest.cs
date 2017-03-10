
using System;
using System.Windows.Forms;
using Intersect_Editor.Classes;
using Intersect_Editor.Classes.Core;
using Intersect_Library;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.Localization;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_EndQuest : UserControl
    {
        private EventCommand _myCommand;
        private readonly FrmEvent _eventEditor;
        public EventCommand_EndQuest(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            InitLocalization();
            cmbQuests.Items.Clear();
            cmbQuests.Items.AddRange(Database.GetGameObjectList(GameObject.Quest));
            cmbQuests.SelectedIndex = Database.GameObjectListIndex(GameObject.Quest, refCommand.Ints[0]);
            chkRunCompletionTask.Checked = Convert.ToBoolean(refCommand.Ints[1]);
        }

        private void InitLocalization()
        {
            grpEndQuest.Text = Strings.Get("eventendquest", "title");
            lblQuest.Text = Strings.Get("eventendquest", "label");
            chkRunCompletionTask.Text = Strings.Get("eventendquest", "skipcompletion");
            btnSave.Text = Strings.Get("eventendquest", "okay");
            btnCancel.Text = Strings.Get("eventendquest", "cancel");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Ints[0] = Database.GameObjectIdFromList(GameObject.Quest, cmbQuests.SelectedIndex);
            _myCommand.Ints[1] = Convert.ToInt32(chkRunCompletionTask.Checked);
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }
    }
}
