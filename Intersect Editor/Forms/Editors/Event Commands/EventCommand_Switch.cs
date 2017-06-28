using System;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.Localization;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_Switch : UserControl
    {
        private readonly FrmEvent _eventEditor;
        private bool _loading;
        private EventCommand _myCommand;

        public EventCommand_Switch(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            _loading = true;
            InitLocalization();
            if (_myCommand.Ints[0] == (int) SwitchVariableTypes.ServerSwitch)
            {
                rdoGlobalSwitch.Checked = true;
            }
            _loading = false;
            InitEditor();
        }

        private void InitLocalization()
        {
            grpSetSwitch.Text = Strings.Get("eventsetswitch", "title");
            lblSwitch.Text = Strings.Get("eventsetswitch", "label");
            rdoGlobalSwitch.Text = Strings.Get("eventsetswitch", "global");
            rdoPlayerSwitch.Text = Strings.Get("eventsetswitch", "player");
            lblTo.Text = Strings.Get("eventsetswitch", "to");
            cmbSetSwitchVal.Items.Clear();
            cmbSetSwitchVal.Items.Add(Strings.Get("eventsetswitch", "false"));
            cmbSetSwitchVal.Items.Add(Strings.Get("eventsetswitch", "true"));
            btnSave.Text = Strings.Get("eventsetswitch", "okay");
            btnCancel.Text = Strings.Get("eventsetswitch", "cancel");
        }

        private void InitEditor()
        {
            cmbSetSwitch.Items.Clear();
            int switchCount = 0;
            if (rdoPlayerSwitch.Checked)
            {
                cmbSetSwitch.Items.AddRange(Database.GetGameObjectList(GameObjectType.PlayerSwitch));
                cmbSetSwitch.SelectedIndex = Database.GameObjectListIndex(GameObjectType.PlayerSwitch, _myCommand.Ints[1]);
            }
            else
            {
                cmbSetSwitch.Items.AddRange(Database.GetGameObjectList(GameObjectType.ServerSwitch));
                cmbSetSwitch.SelectedIndex = Database.GameObjectListIndex(GameObjectType.ServerSwitch, _myCommand.Ints[1]);
            }
            cmbSetSwitchVal.SelectedIndex = _myCommand.Ints[2];
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (rdoPlayerSwitch.Checked)
            {
                _myCommand.Ints[0] = (int) SwitchVariableTypes.PlayerSwitch;
                _myCommand.Ints[1] = Database.GameObjectIdFromList(GameObjectType.PlayerSwitch, cmbSetSwitch.SelectedIndex);
            }
            if (rdoGlobalSwitch.Checked)
            {
                _myCommand.Ints[0] = (int) SwitchVariableTypes.ServerSwitch;
                _myCommand.Ints[1] = Database.GameObjectIdFromList(GameObjectType.ServerSwitch, cmbSetSwitch.SelectedIndex);
            }
            _myCommand.Ints[2] = cmbSetSwitchVal.SelectedIndex;
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }

        private void rdoPlayerSwitch_CheckedChanged(object sender, EventArgs e)
        {
            InitEditor();
            if (!_loading && cmbSetSwitch.Items.Count > 0) cmbSetSwitch.SelectedIndex = 0;
            if (!_loading) cmbSetSwitchVal.SelectedIndex = 0;
        }

        private void rdoGlobalSwitch_CheckedChanged(object sender, EventArgs e)
        {
            InitEditor();
            if (!_loading && cmbSetSwitch.Items.Count > 0) cmbSetSwitch.SelectedIndex = 0;
            if (!_loading) cmbSetSwitchVal.SelectedIndex = 0;
        }
    }
}