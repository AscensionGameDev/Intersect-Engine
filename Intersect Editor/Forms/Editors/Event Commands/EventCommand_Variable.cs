using System;
using System.Windows.Forms;
using Intersect;
using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.Localization;
using Intersect_Editor.Classes;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_Variable : UserControl
    {
        private readonly FrmEvent _eventEditor;
        private bool _loading = false;
        private EventCommand _myCommand;

        public EventCommand_Variable(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            _loading = true;
            InitLocalization();
            if (_myCommand.Ints[0] == (int) SwitchVariableTypes.ServerVariable)
            {
                rdoGlobalVariable.Checked = true;
            }
            else
            {
                rdoPlayerVariable.Checked = true;
            }
            _loading = false;
            InitEditor();
        }

        private void InitLocalization()
        {
            grpSetVariable.Text = Strings.Get("eventsetvariable", "title");
            grpVariableSelection.Text = Strings.Get("eventsetvariable", "label");
            lblVariable.Text = Strings.Get("eventsetvariable", "label");
            rdoGlobalVariable.Text = Strings.Get("eventsetvariable", "global");
            rdoPlayerVariable.Text = Strings.Get("eventsetvariable", "player");
            optSet.Text = Strings.Get("eventsetvariable", "set");
            optAdd.Text = Strings.Get("eventsetvariable", "add");
            optSubtract.Text = Strings.Get("eventsetvariable", "subtract");
            optRandom.Text = Strings.Get("eventsetvariable", "random");
            lblRandomLow.Text = Strings.Get("eventsetvariable", "randomlow");
            lblRandomHigh.Text = Strings.Get("eventsetvariable", "randomhigh");
            btnSave.Text = Strings.Get("eventsetvariable", "okay");
            btnCancel.Text = Strings.Get("eventsetvariable", "cancel");
        }

        private void InitEditor()
        {
            cmbVariable.Items.Clear();
            int varCount = 0;
            if (rdoPlayerVariable.Checked)
            {
                cmbVariable.Items.AddRange(Database.GetGameObjectList(GameObjectType.PlayerVariable));
                cmbVariable.SelectedIndex = Database.GameObjectListIndex(GameObjectType.PlayerVariable, _myCommand.Ints[1]);
            }
            else
            {
                cmbVariable.Items.AddRange(Database.GetGameObjectList(GameObjectType.ServerVariable));
                cmbVariable.SelectedIndex = Database.GameObjectListIndex(GameObjectType.ServerVariable, _myCommand.Ints[1]);
            }
            switch (_myCommand.Ints[2])
            {
                case 0:
                    optSet.Checked = true;
                    nudSet.Value = _myCommand.Ints[3];
                    break;
                case 1:
                    optAdd.Checked = true;
                    nudAdd.Value = _myCommand.Ints[3];
                    break;
                case 2:
                    optSubtract.Checked = true;
                    nudSubtract.Value = _myCommand.Ints[3];
                    break;
                case 3:
                    optRandom.Checked = true;
                    nudLow.Value = _myCommand.Ints[3];
                    nudHigh.Value = _myCommand.Ints[4];
                    break;
            }
            UpdateFormElements();
        }

        private void UpdateFormElements()
        {
            nudSet.Enabled = optSet.Checked;
            nudAdd.Enabled = optAdd.Checked;
            nudSubtract.Enabled = optSubtract.Checked;
            nudLow.Enabled = optRandom.Checked;
            nudHigh.Enabled = optRandom.Checked;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int n;
            if (rdoPlayerVariable.Checked)
            {
                _myCommand.Ints[0] = (int) SwitchVariableTypes.PlayerVariable;
                _myCommand.Ints[1] = Database.GameObjectIdFromList(GameObjectType.PlayerVariable,
                    cmbVariable.SelectedIndex);
            }
            if (rdoGlobalVariable.Checked)
            {
                _myCommand.Ints[0] = (int) SwitchVariableTypes.ServerVariable;
                _myCommand.Ints[1] = Database.GameObjectIdFromList(GameObjectType.ServerVariable,
                    cmbVariable.SelectedIndex);
            }
            if (optSet.Checked)
            {
                _myCommand.Ints[2] = 0;
                _myCommand.Ints[3] = (int) nudSet.Value;
            }
            else if (optAdd.Checked)
            {
                _myCommand.Ints[2] = 1;
                _myCommand.Ints[3] = (int) nudAdd.Value;
            }
            else if (optSubtract.Checked)
            {
                _myCommand.Ints[2] = 2;
                _myCommand.Ints[3] = (int) nudSubtract.Value;
            }
            else if (optRandom.Checked)
            {
                _myCommand.Ints[2] = 3;
                _myCommand.Ints[3] = (int) nudLow.Value;
                _myCommand.Ints[4] = (int) nudHigh.Value;
                if (_myCommand.Ints[4] < _myCommand.Ints[3])
                {
                    n = _myCommand.Ints[3];
                    _myCommand.Ints[3] = _myCommand.Ints[4];
                    _myCommand.Ints[4] = n;
                }
            }
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }

        private void optSet_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFormElements();
        }

        private void optAdd_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFormElements();
        }

        private void optSubtract_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFormElements();
        }

        private void optRandom_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFormElements();
        }

        private void rdoPlayerVariable_CheckedChanged(object sender, EventArgs e)
        {
            InitEditor();
            if (!_loading && cmbVariable.Items.Count > 0) cmbVariable.SelectedIndex = 0;
            if (!_loading) optSet.Checked = true;
            if (!_loading) nudSet.Value = 0;
        }

        private void rdoGlobalVariable_CheckedChanged(object sender, EventArgs e)
        {
            InitEditor();
            if (!_loading && cmbVariable.Items.Count > 0) cmbVariable.SelectedIndex = 0;
            if (!_loading) optSet.Checked = true;
            if (!_loading) nudSet.Value = 0;
        }
    }
}