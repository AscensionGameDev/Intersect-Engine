using System;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.Editor.Classes.Localization;

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
            grpSetSwitch.Text = Strings.eventsetswitch.title;
            lblSwitch.Text = Strings.eventsetswitch.label;
            rdoGlobalSwitch.Text = Strings.eventsetswitch.global;
            rdoPlayerSwitch.Text = Strings.eventsetswitch.player;
            lblTo.Text = Strings.eventsetswitch.to;
            cmbSetSwitchVal.Items.Clear();
            cmbSetSwitchVal.Items.Add(Strings.eventsetswitch.False);
            cmbSetSwitchVal.Items.Add(Strings.eventsetswitch.True);
            btnSave.Text = Strings.eventsetswitch.okay;
            btnCancel.Text = Strings.eventsetswitch.cancel;
        }

        private void InitEditor()
        {
            cmbSetSwitch.Items.Clear();
            int switchCount = 0;
            if (rdoPlayerSwitch.Checked)
            {
                cmbSetSwitch.Items.AddRange(Database.GetGameObjectList(GameObjectType.PlayerSwitch));
                cmbSetSwitch.SelectedIndex =
                    Database.GameObjectListIndex(GameObjectType.PlayerSwitch, mMyCommand.Ints[1]);
            }
            else
            {
                cmbSetSwitch.Items.AddRange(Database.GetGameObjectList(GameObjectType.ServerSwitch));
                cmbSetSwitch.SelectedIndex =
                    Database.GameObjectListIndex(GameObjectType.ServerSwitch, mMyCommand.Ints[1]);
            }
            cmbSetSwitchVal.SelectedIndex = mMyCommand.Ints[2];
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (rdoPlayerSwitch.Checked)
            {
                mMyCommand.Ints[0] = (int) SwitchVariableTypes.PlayerSwitch;
                mMyCommand.Ints[1] =
                    Database.GameObjectIdFromList(GameObjectType.PlayerSwitch, cmbSetSwitch.SelectedIndex);
            }
            if (rdoGlobalSwitch.Checked)
            {
                mMyCommand.Ints[0] = (int) SwitchVariableTypes.ServerSwitch;
                mMyCommand.Ints[1] =
                    Database.GameObjectIdFromList(GameObjectType.ServerSwitch, cmbSetSwitch.SelectedIndex);
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