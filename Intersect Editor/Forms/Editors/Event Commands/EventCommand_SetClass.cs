using System;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.Localization;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_SetClass : UserControl
    {
        private readonly FrmEvent _eventEditor;
        private EventCommand _myCommand;

        public EventCommand_SetClass(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            InitLocalization();
            cmbClass.Items.Clear();
            cmbClass.Items.AddRange(Database.GetGameObjectList(GameObjectType.Class));
            cmbClass.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Class, _myCommand.Ints[0]);
        }

        private void InitLocalization()
        {
            grpSetClass.Text = Strings.Get("eventsetclass", "title");
            lblClass.Text = Strings.Get("eventsetclass", "label");
            btnSave.Text = Strings.Get("eventsetclass", "okay");
            btnCancel.Text = Strings.Get("eventsetclass", "cancel");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbClass.SelectedIndex > -1)
                _myCommand.Ints[0] = Database.GameObjectIdFromList(GameObjectType.Class, cmbClass.SelectedIndex);
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }
    }
}