using System;
using System.Windows.Forms;
using Intersect.GameObjects.Events;
using Intersect.Localization;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandSelfSwitch : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EventCommand mMyCommand;

        public EventCommandSelfSwitch(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            cmbSetSwitch.SelectedIndex = mMyCommand.Ints[0];
            cmbSetSwitchVal.SelectedIndex = mMyCommand.Ints[1];
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
            mMyCommand.Ints[0] = cmbSetSwitch.SelectedIndex;
            mMyCommand.Ints[1] = cmbSetSwitchVal.SelectedIndex;
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }
    }
}