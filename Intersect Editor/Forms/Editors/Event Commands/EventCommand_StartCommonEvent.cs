using System;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.Localization;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_StartCommonEvent : UserControl
    {
        private readonly FrmEvent _eventEditor;
        private EventCommand _myCommand;

        public EventCommand_StartCommonEvent(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            InitLocalization();
            cmbEvent.Items.Clear();
            cmbEvent.Items.AddRange(Database.GetGameObjectList(GameObjectType.CommonEvent));
            cmbEvent.SelectedIndex = Database.GameObjectListIndex(GameObjectType.CommonEvent, refCommand.Ints[0]);
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
            _myCommand.Ints[0] = Database.GameObjectIdFromList(GameObjectType.CommonEvent, cmbEvent.SelectedIndex);
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }
    }
}