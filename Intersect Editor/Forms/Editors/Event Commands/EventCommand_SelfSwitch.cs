using System;
using System.Windows.Forms;
using Intersect.Editor.Localization;
using Intersect.GameObjects.Events;

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
            grpSelfSwitch.Text = Strings.EventSelfSwitch.title;
            lblSelfSwitch.Text = Strings.EventSelfSwitch.label;
            cmbSetSwitch.Items.Clear();
            for (int i = 0; i < Strings.EventSelfSwitch.selfswitches.Length; i++)
            {
                cmbSetSwitch.Items.Add(Strings.EventSelfSwitch.selfswitches[i]);
            }
            cmbSetSwitchVal.Items.Clear();
            cmbSetSwitchVal.Items.Add(Strings.EventSelfSwitch.False);
            cmbSetSwitchVal.Items.Add(Strings.EventSelfSwitch.True);
            btnSave.Text = Strings.EventSelfSwitch.okay;
            btnCancel.Text = Strings.EventSelfSwitch.cancel;
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