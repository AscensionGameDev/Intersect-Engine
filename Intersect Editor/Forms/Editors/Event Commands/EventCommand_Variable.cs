using System;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.Localization;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandVariable : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private bool mLoading;
        private EventCommand mMyCommand;

        public EventCommandVariable(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            mLoading = true;
            InitLocalization();
            if (mMyCommand.Ints[0] == (int) SwitchVariableTypes.ServerVariable)
            {
                rdoGlobalVariable.Checked = true;
            }
            else
            {
                rdoPlayerVariable.Checked = true;
            }
            mLoading = false;
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
                cmbVariable.SelectedIndex =
                    Database.GameObjectListIndex(GameObjectType.PlayerVariable, mMyCommand.Ints[1]);
            }
            else
            {
                cmbVariable.Items.AddRange(Database.GetGameObjectList(GameObjectType.ServerVariable));
                cmbVariable.SelectedIndex =
                    Database.GameObjectListIndex(GameObjectType.ServerVariable, mMyCommand.Ints[1]);
            }
            switch (mMyCommand.Ints[2])
            {
                case 0:
                    optSet.Checked = true;
                    nudSet.Value = mMyCommand.Ints[3];
                    break;
                case 1:
                    optAdd.Checked = true;
                    nudAdd.Value = mMyCommand.Ints[3];
                    break;
                case 2:
                    optSubtract.Checked = true;
                    nudSubtract.Value = mMyCommand.Ints[3];
                    break;
                case 3:
                    optRandom.Checked = true;
                    nudLow.Value = mMyCommand.Ints[3];
                    nudHigh.Value = mMyCommand.Ints[4];
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
                mMyCommand.Ints[0] = (int) SwitchVariableTypes.PlayerVariable;
                mMyCommand.Ints[1] = Database.GameObjectIdFromList(GameObjectType.PlayerVariable,
                    cmbVariable.SelectedIndex);
            }
            if (rdoGlobalVariable.Checked)
            {
                mMyCommand.Ints[0] = (int) SwitchVariableTypes.ServerVariable;
                mMyCommand.Ints[1] = Database.GameObjectIdFromList(GameObjectType.ServerVariable,
                    cmbVariable.SelectedIndex);
            }
            if (optSet.Checked)
            {
                mMyCommand.Ints[2] = 0;
                mMyCommand.Ints[3] = (int) nudSet.Value;
            }
            else if (optAdd.Checked)
            {
                mMyCommand.Ints[2] = 1;
                mMyCommand.Ints[3] = (int) nudAdd.Value;
            }
            else if (optSubtract.Checked)
            {
                mMyCommand.Ints[2] = 2;
                mMyCommand.Ints[3] = (int) nudSubtract.Value;
            }
            else if (optRandom.Checked)
            {
                mMyCommand.Ints[2] = 3;
                mMyCommand.Ints[3] = (int) nudLow.Value;
                mMyCommand.Ints[4] = (int) nudHigh.Value;
                if (mMyCommand.Ints[4] < mMyCommand.Ints[3])
                {
                    n = mMyCommand.Ints[3];
                    mMyCommand.Ints[3] = mMyCommand.Ints[4];
                    mMyCommand.Ints[4] = n;
                }
            }
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
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
            if (!mLoading && cmbVariable.Items.Count > 0) cmbVariable.SelectedIndex = 0;
            if (!mLoading) optSet.Checked = true;
            if (!mLoading) nudSet.Value = 0;
        }

        private void rdoGlobalVariable_CheckedChanged(object sender, EventArgs e)
        {
            InitEditor();
            if (!mLoading && cmbVariable.Items.Count > 0) cmbVariable.SelectedIndex = 0;
            if (!mLoading) optSet.Checked = true;
            if (!mLoading) nudSet.Value = 0;
        }
    }
}