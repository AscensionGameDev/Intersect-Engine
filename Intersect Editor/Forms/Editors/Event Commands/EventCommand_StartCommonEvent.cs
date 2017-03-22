
using System;
using System.Windows.Forms;
using Intersect_Editor.Classes;
using Intersect;
using Intersect.GameObjects.Events;
using Intersect.Localization;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_StartCommonEvent : UserControl
    {
        private EventCommand _myCommand;
        private readonly FrmEvent _eventEditor;
        public EventCommand_StartCommonEvent(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            InitLocalization();
            cmbEvent.Items.Clear();
            cmbEvent.Items.AddRange(Database.GetGameObjectList(GameObject.CommonEvent));
            cmbEvent.SelectedIndex = Database.GameObjectListIndex(GameObject.CommonEvent,refCommand.Ints[0]);
        }

        private void InitLocalization()
        {
            grpCommonEvent.Text = Strings.Get("eventstartcommonevent", "title");
            lblCommonEvent.Text = Strings.Get("eventstartcommonevent", "label");
            btnSave.Text = Strings.Get("eventstartcommonevent", "okay");
            btnCancel.Text = Strings.Get("eventstartcommonevent", "cancel");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Ints[0] = Database.GameObjectIdFromList(GameObject.CommonEvent,cmbEvent.SelectedIndex);
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }
    }
}
