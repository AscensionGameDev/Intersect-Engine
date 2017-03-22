
using System;
using System.Windows.Forms;
using Intersect.GameObjects.Events;
using Intersect.Localization;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_SelfSwitch : UserControl
    {
        private EventCommand _myCommand;
        private readonly FrmEvent _eventEditor;
        public EventCommand_SelfSwitch(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            InitLocalization();
            cmbSetSwitch.SelectedIndex = _myCommand.Ints[0];
            cmbSetSwitchVal.SelectedIndex = _myCommand.Ints[1];
        }

        private void InitLocalization()
        {
            grpSelfSwitch.Text = Strings.Get("eventselfswitch", "title");
            lblSelfSwitch.Text = Strings.Get("eventselfswitch", "label");
            cmbSetSwitch.Items.Clear();
            for (int i = 0; i < 4; i++)
            {
                cmbSetSwitch.Items.Add(Strings.Get("eventselfswitch", "selfswitch" + i));
            }
            cmbSetSwitchVal.Items.Clear();
            cmbSetSwitchVal.Items.Add(Strings.Get("eventselfswitch", "false"));
            cmbSetSwitchVal.Items.Add(Strings.Get("eventselfswitch", "true"));
            btnSave.Text = Strings.Get("eventselfswitch", "okay");
            btnCancel.Text = Strings.Get("eventselfswitch", "cancel");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Ints[0] = cmbSetSwitch.SelectedIndex;
            _myCommand.Ints[1] = cmbSetSwitchVal.SelectedIndex;
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }
    }
}
