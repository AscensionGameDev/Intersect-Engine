using System;
using System.Drawing;
using System.Windows.Forms;
using Intersect.Editor.Forms.Editors.Event_Commands;
using Intersect.Editor.Localization;
using Intersect.GameObjects.Conditions;
using Intersect.GameObjects.Events;

namespace Intersect.Editor.Forms.Editors
{
    public enum RequirementType
    {
        Item,
        Resource,
        Spell,
        Event,
        Quest
    }

    public partial class FrmDynamicRequirements : Form
    {
        private ConditionList mEdittingList;
        private ConditionLists mEdittingLists;
        private ConditionList mSourceList;
        private ConditionLists mSourceLists;

        public FrmDynamicRequirements(ConditionLists lists, RequirementType type)
        {
            InitializeComponent();
            mSourceLists = lists;
            mEdittingLists = new ConditionLists(lists.Data());
            UpdateLists();
            InitLocalization(type);
        }

        private void InitLocalization(RequirementType type)
        {
            Text = Strings.DynamicRequirements.title;
            grpConditionLists.Text = Strings.DynamicRequirements.conditionlists;
            switch (type)
            {
                case RequirementType.Item:
                    lblInstructions.Text = Strings.DynamicRequirements.instructionsitem;
                    break;
                case RequirementType.Resource:
                    lblInstructions.Text = Strings.DynamicRequirements.instructionsresource;
                    break;
                case RequirementType.Spell:
                    lblInstructions.Text = Strings.DynamicRequirements.instructionsspell;
                    break;
                case RequirementType.Event:
                    lblInstructions.Text = Strings.DynamicRequirements.instructionsevent;
                    break;
                case RequirementType.Quest:
                    lblInstructions.Text = Strings.DynamicRequirements.instructionsquest;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            btnAddList.Text = Strings.DynamicRequirements.addlist;
            btnRemoveList.Text = Strings.DynamicRequirements.removelist;
            btnSave.Text = Strings.DynamicRequirements.savelists;
            btnCancel.Text = Strings.DynamicRequirements.cancellists;
            grpConditionList.Text = Strings.DynamicRequirements.conditionlist;
            lblListName.Text = Strings.DynamicRequirements.listname;
            btnAddCondition.Text = Strings.DynamicRequirements.addcondition;
            btnRemoveCondition.Text = Strings.DynamicRequirements.removecondition;
            btnConditionsOkay.Text = Strings.DynamicRequirements.saveconditions;
            btnConditionsCancel.Text = Strings.DynamicRequirements.cancelconditions;
        }

        private void UpdateLists()
        {
            grpConditionLists.Show();
            grpConditionList.Hide();
            lstConditionLists.Items.Clear();
            for (int i = 0; i < mEdittingLists.Lists.Count; i++)
            {
                lstConditionLists.Items.Add(mEdittingLists.Lists[i].Name);
            }
        }

        private void UpdateConditions(ConditionList list)
        {
            grpConditionLists.Hide();
            grpConditionList.Show();
            lstConditions.Items.Clear();
            if (list != mEdittingList)
            {
                mSourceList = list;
                mEdittingList = new ConditionList(mSourceList.Data());
            }
            txtListName.Text = list.Name;
            for (int i = 0; i < list.Conditions.Count; i++)
            {
                lstConditions.Items.Add(Strings.GetEventConditionalDesc(list.Conditions[i]));
            }
        }

        private void lstConditionLists_DoubleClick(object sender, EventArgs e)
        {
            if (lstConditionLists.SelectedIndex > -1)
            {
                UpdateConditions(mEdittingLists.Lists[lstConditionLists.SelectedIndex]);
            }
        }

        private void btnAddList_Click(object sender, EventArgs e)
        {
            var newList = new ConditionList();
            mEdittingLists.Lists.Add(newList);
            UpdateConditions(newList);
        }

        private void btnRemoveList_Click(object sender, EventArgs e)
        {
            if (lstConditionLists.SelectedIndex > -1)
            {
                mEdittingLists.Lists.RemoveAt(lstConditionLists.SelectedIndex);
                UpdateLists();
            }
        }

        private void txtListName_TextChanged(object sender, EventArgs e)
        {
            if (txtListName.Text.Trim().Length > 0)
            {
                mEdittingList.Name = txtListName.Text;
            }
        }

        private void lstConditions_DoubleClick(object sender, EventArgs e)
        {
            if (lstConditions.SelectedIndex > -1)
            {
                if (OpenConditionEditor(mEdittingList.Conditions[lstConditions.SelectedIndex]))
                {
                    UpdateConditions(mEdittingList);
                }
            }
        }

        private void btnAddCondition_Click(object sender, EventArgs e)
        {
            var evtCommand = new EventCommand()
            {
                Type = EventCommandType.ConditionalBranch
            };
            if (OpenConditionEditor(evtCommand))
            {
                mEdittingList.Conditions.Add(evtCommand);
                UpdateConditions(mEdittingList);
            }
        }

        private bool OpenConditionEditor(EventCommand cmd)
        {
            var cmdWindow = new EventCommandConditionalBranch(cmd, null, null);
            var frm = new Form
            {
                Text = Strings.DynamicRequirements.conditioneditor,
                FormBorderStyle = FormBorderStyle.FixedSingle,
                Size = new Size(0, 0),
                AutoSize = true,
                ControlBox = false,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = cmdWindow.BackColor
            };
            frm.Controls.Add(cmdWindow);
            cmdWindow.BringToFront();
            frm.ShowDialog();
            return !cmdWindow.Cancelled;
        }

        private void btnRemoveCondition_Click(object sender, EventArgs e)
        {
            if (lstConditions.SelectedIndex > -1)
            {
                mEdittingList.Conditions.RemoveAt(lstConditions.SelectedIndex);
                UpdateConditions(mEdittingList);
            }
        }

        private void btnConditionsOkay_Click(object sender, EventArgs e)
        {
            mSourceList.Load(mEdittingList.Data());
            UpdateLists();
        }

        private void btnConditionsCancel_Click(object sender, EventArgs e)
        {
            UpdateLists();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mSourceLists.Load(mEdittingLists.Data());
            Close();
        }

        private void lstConditionLists_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete) btnRemoveList_Click(null, null);
        }

        private void lstConditions_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete) btnRemoveCondition_Click(null, null);
        }
    }
}