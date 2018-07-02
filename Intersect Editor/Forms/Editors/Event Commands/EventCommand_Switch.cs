using System;
using System.Windows.Forms;
using Intersect.Editor.Core;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandSwitch : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private bool mLoading;
        private EventCommand mMyCommand;

        public EventCommandSwitch(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            mLoading = true;
            InitLocalization();
            if (mMyCommand.Ints[0] == (int) SwitchVariableTypes.ServerSwitch)
            {
                rdoGlobalSwitch.Checked = true;
            }
            mLoading = false;
            InitEditor();
        }

        private void InitLocalization()
        {
            grpSetSwitch.Text = Strings.EventSetSwitch.title;
            lblSwitch.Text = Strings.EventSetSwitch.label;
            rdoGlobalSwitch.Text = Strings.EventSetSwitch.global;
            rdoPlayerSwitch.Text = Strings.EventSetSwitch.player;
            lblTo.Text = Strings.EventSetSwitch.to;
            cmbSetSwitchVal.Items.Clear();
            cmbSetSwitchVal.Items.Add(Strings.EventSetSwitch.False);
            cmbSetSwitchVal.Items.Add(Strings.EventSetSwitch.True);
            btnSave.Text = Strings.EventSetSwitch.okay;
            btnCancel.Text = Strings.EventSetSwitch.cancel;
        }

        private void InitEditor()
        {
            cmbSetSwitch.Items.Clear();
            int switchCount = 0;
            if (rdoPlayerSwitch.Checked)
            {
                cmbSetSwitch.Items.AddRange(PlayerSwitchBase.Names);
                cmbSetSwitch.SelectedIndex = PlayerSwitchBase.ListIndex(mMyCommand.Guids[1]);
            }
            else
            {
                cmbSetSwitch.Items.AddRange(ServerSwitchBase.Names);
                cmbSetSwitch.SelectedIndex =ServerSwitchBase.ListIndex(mMyCommand.Guids[1]);
            }
            cmbSetSwitchVal.SelectedIndex = mMyCommand.Ints[2];
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (rdoPlayerSwitch.Checked)
            {
                mMyCommand.Ints[0] = (int) SwitchVariableTypes.PlayerSwitch;
                mMyCommand.Guids[1] = PlayerSwitchBase.IdFromList(cmbSetSwitch.SelectedIndex);
            }
            if (rdoGlobalSwitch.Checked)
            {
                mMyCommand.Ints[0] = (int) SwitchVariableTypes.ServerSwitch;
                mMyCommand.Guids[1] = ServerSwitchBase.IdFromList(cmbSetSwitch.SelectedIndex);
            }
            mMyCommand.Ints[2] = cmbSetSwitchVal.SelectedIndex;
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

        private void rdoPlayerSwitch_CheckedChanged(object sender, EventArgs e)
        {
            InitEditor();
            if (!mLoading && cmbSetSwitch.Items.Count > 0) cmbSetSwitch.SelectedIndex = 0;
            if (!mLoading) cmbSetSwitchVal.SelectedIndex = 0;
        }

        private void rdoGlobalSwitch_CheckedChanged(object sender, EventArgs e)
        {
            InitEditor();
            if (!mLoading && cmbSetSwitch.Items.Count > 0) cmbSetSwitch.SelectedIndex = 0;
            if (!mLoading) cmbSetSwitchVal.SelectedIndex = 0;
        }
    }
}