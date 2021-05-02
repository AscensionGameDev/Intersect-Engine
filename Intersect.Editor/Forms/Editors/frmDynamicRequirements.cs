using System;
using System.Drawing;
using System.Windows.Forms;

using Intersect.Editor.Forms.Editors.Events.Event_Commands;
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

        Quest,

        NpcFriend,

        NpcAttackOnSight,

        NpcDontAttackOnSight,

        NpcCanBeAttacked,

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

            this.Icon = Properties.Resources.Icon;
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
                case RequirementType.NpcFriend:
                    lblInstructions.Text = Strings.DynamicRequirements.instructionsnpcfriend;

                    break;
                case RequirementType.NpcAttackOnSight:
                    lblInstructions.Text = Strings.DynamicRequirements.instructionsnpcattackonsight;

                    break;
                case RequirementType.NpcDontAttackOnSight:
                    lblInstructions.Text = Strings.DynamicRequirements.instructionsnpcdontattackonsight;

                    break;
                case RequirementType.NpcCanBeAttacked:
                    lblInstructions.Text = Strings.DynamicRequirements.instructionsnpccanbeattacked;

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            btnAddList.Text = Strings.DynamicRequirements.addlist;
            btnRemoveList.Text = Strings.DynamicRequirements.removelist;
            btnSave.Text = Strings.DynamicRequirements.save;
            btnCancel.Text = Strings.DynamicRequirements.cancel;
            grpConditionList.Text = Strings.DynamicRequirements.conditionlist;
            lblListName.Text = Strings.DynamicRequirements.listname;
            btnAddCondition.Text = Strings.DynamicRequirements.addcondition;
            btnRemoveCondition.Text = Strings.DynamicRequirements.removecondition;
        }

        private void UpdateLists()
        {
            grpConditionLists.Show();
            grpConditionList.Hide();
            lstConditionLists.Items.Clear();
            for (var i = 0; i < mEdittingLists.Lists.Count; i++)
            {
                lstConditionLists.Items.Add(mEdittingLists.Lists[i].Name);
            }
        }

        private void UpdateConditions(ConditionList list)
        {
            grpConditionList.Show();
            lstConditions.Items.Clear();
            if (list != mEdittingList)
            {
                mSourceList = list;
                mEdittingList = mSourceList;
            }

            txtListName.Text = list.Name;
            for (var i = 0; i < list.Conditions.Count; i++)
            {
                if (list.Conditions[i].Negated)
                {
                    lstConditions.Items.Add(
                        Strings.EventConditionDesc.negated.ToString(
                            Strings.GetEventConditionalDesc((dynamic) list.Conditions[i])
                        )
                    );
                }
                else
                {
                    lstConditions.Items.Add(Strings.GetEventConditionalDesc((dynamic) list.Conditions[i]));
                }
            }
        }

        private void btnAddList_Click(object sender, EventArgs e)
        {
            var newList = new ConditionList();
            mEdittingLists.Lists.Add(newList);
            UpdateLists();
            lstConditionLists.SelectedIndex = lstConditionLists.Items.Count - 1;
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
                if (mEdittingLists.Lists.IndexOf(mEdittingList) > -1)
                {
                    lstConditionLists.Items[mEdittingLists.Lists.IndexOf(mEdittingList)] = txtListName.Text;
                }
            }
        }

        private void lstConditions_DoubleClick(object sender, EventArgs e)
        {
            if (lstConditions.SelectedIndex > -1)
            {
                var condition = OpenConditionEditor(mEdittingList.Conditions[lstConditions.SelectedIndex]);
                if (condition != null)
                {
                    mEdittingList.Conditions[lstConditions.SelectedIndex] = condition;
                    UpdateConditions(mEdittingList);
                }
            }
        }

        private void btnAddCondition_Click(object sender, EventArgs e)
        {
            var condition = OpenConditionEditor(new VariableIsCondition());
            if (condition != null)
            {
                mEdittingList.Conditions.Add(condition);
                UpdateConditions(mEdittingList);
            }
        }

        private Condition OpenConditionEditor(Condition condition)
        {
            var cmdWindow = new EventCommandConditionalBranch(condition, null, null, null);
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
            frm.TopMost = true;
            frm.ShowDialog();
            if (cmdWindow.Cancelled)
            {
                return null;
            }

            return cmdWindow.Condition;
        }

        private void btnRemoveCondition_Click(object sender, EventArgs e)
        {
            if (lstConditions.SelectedIndex > -1)
            {
                mEdittingList.Conditions.RemoveAt(lstConditions.SelectedIndex);
                UpdateConditions(mEdittingList);
            }
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
            if (e.KeyCode == Keys.Delete)
            {
                btnRemoveList_Click(null, null);
            }
        }

        private void lstConditions_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                btnRemoveCondition_Click(null, null);
            }
        }

        private void lstConditionLists_Click(object sender, EventArgs e)
        {
            if (lstConditionLists.SelectedIndex > -1)
            {
                UpdateConditions(mEdittingLists.Lists[lstConditionLists.SelectedIndex]);
            }
        }

    }

}
