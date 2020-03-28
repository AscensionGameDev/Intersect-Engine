using System;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandSelfSwitch : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private SetSelfSwitchCommand mMyCommand;

        public EventCommandSelfSwitch(SetSelfSwitchCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            cmbSetSwitch.SelectedIndex = mMyCommand.SwitchId;
            cmbSetSwitchVal.SelectedIndex = Convert.ToInt32(mMyCommand.Value);
        }

        private void InitLocalization()
        {
            grpSelfSwitch.Text = Strings.EventSelfSwitch.title;
            lblSelfSwitch.Text = Strings.EventSelfSwitch.label;
            cmbSetSwitch.Items.Clear();
            for (var i = 0; i < Strings.EventSelfSwitch.selfswitches.Count; i++)
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
            mMyCommand.SwitchId = cmbSetSwitch.SelectedIndex;
            mMyCommand.Value = Convert.ToBoolean(cmbSetSwitchVal.SelectedIndex);
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

    }

}
