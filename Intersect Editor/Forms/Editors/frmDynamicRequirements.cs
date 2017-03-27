using System;
using System.Drawing;
using System.Windows.Forms;
using Intersect.GameObjects.Conditions;
using Intersect.GameObjects.Events;
using Intersect.Localization;
using Intersect_Editor.Forms.Editors.Event_Commands;

namespace Intersect_Editor.Forms.Editors
{
    public enum RequirementType
    {
        Item,
        Resource,
        Spell,
        Event,
        Quest
    }

    public partial class frmDynamicRequirements : Form
    {
        private ConditionList _edittingList;
        private ConditionLists _edittingLists;
        private ConditionList _sourceList;
        private ConditionLists _sourceLists;

        public frmDynamicRequirements(ConditionLists lists, RequirementType type)
        {
            InitializeComponent();
            _sourceLists = lists;
            _edittingLists = new ConditionLists(lists.Data());
            UpdateLists();
            InitLocalization(type);
        }

        private void InitLocalization(RequirementType type)
        {
            Text = Strings.Get("dynamicrequirements", "title");
            grpConditionLists.Text = Strings.Get("dynamicrequirements", "conditionlists");
            switch (type)
            {
                case RequirementType.Item:
                    lblInstructions.Text = Strings.Get("dynamicrequirements", "instructionsitem");
                    break;
                case RequirementType.Resource:
                    lblInstructions.Text = Strings.Get("dynamicrequirements", "instructionsresource");
                    break;
                case RequirementType.Spell:
                    lblInstructions.Text = Strings.Get("dynamicrequirements", "instructionsspell");
                    break;
                case RequirementType.Event:
                    lblInstructions.Text = Strings.Get("dynamicrequirements", "instructionsevent");
                    break;
                case RequirementType.Quest:
                    lblInstructions.Text = Strings.Get("dynamicrequirements", "instructionsquest");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            btnAddList.Text = Strings.Get("dynamicrequirements", "addlist");
            btnRemoveList.Text = Strings.Get("dynamicrequirements", "removelist");
            btnSave.Text = Strings.Get("dynamicrequirements", "savelists");
            btnCancel.Text = Strings.Get("dynamicrequirements", "cancellists");
            grpConditionList.Text = Strings.Get("dynamicrequirements", "conditionlist");
            lblListName.Text = Strings.Get("dynamicrequirements", "listname");
            btnAddCondition.Text = Strings.Get("dynamicrequirements", "addcondition");
            btnRemoveCondition.Text = Strings.Get("dynamicrequirements", "removecondition");
            btnConditionsOkay.Text = Strings.Get("dynamicrequirements", "saveconditions");
            btnConditionsCancel.Text = Strings.Get("dynamicrequirements", "cancelconditions");
        }

        private void UpdateLists()
        {
            grpConditionLists.Show();
            grpConditionList.Hide();
            lstConditionLists.Items.Clear();
            for (int i = 0; i < _edittingLists.Lists.Count; i++)
            {
                lstConditionLists.Items.Add(_edittingLists.Lists[i].Name);
            }
        }

        private void UpdateConditions(ConditionList list)
        {
            grpConditionLists.Hide();
            grpConditionList.Show();
            lstConditions.Items.Clear();
            if (list != _edittingList)
            {
                _sourceList = list;
                _edittingList = new ConditionList(_sourceList.Data());
            }
            txtListName.Text = list.Name;
            for (int i = 0; i < list.Conditions.Count; i++)
            {
                lstConditions.Items.Add(list.Conditions[i].GetConditionalDesc());
            }
        }

        private void lstConditionLists_DoubleClick(object sender, EventArgs e)
        {
            if (lstConditionLists.SelectedIndex > -1)
            {
                UpdateConditions(_edittingLists.Lists[lstConditionLists.SelectedIndex]);
            }
        }

        private void btnAddList_Click(object sender, EventArgs e)
        {
            var newList = new ConditionList();
            _edittingLists.Lists.Add(newList);
            UpdateConditions(newList);
        }

        private void btnRemoveList_Click(object sender, EventArgs e)
        {
            if (lstConditionLists.SelectedIndex > -1)
            {
                _edittingLists.Lists.RemoveAt(lstConditionLists.SelectedIndex);
                UpdateLists();
            }
        }

        private void txtListName_TextChanged(object sender, EventArgs e)
        {
            if (txtListName.Text.Trim().Length > 0)
            {
                _edittingList.Name = txtListName.Text;
            }
        }

        private void lstConditions_DoubleClick(object sender, EventArgs e)
        {
            if (lstConditions.SelectedIndex > -1)
            {
                if (OpenConditionEditor(_edittingList.Conditions[lstConditions.SelectedIndex]))
                {
                    UpdateConditions(_edittingList);
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
                _edittingList.Conditions.Add(evtCommand);
                UpdateConditions(_edittingList);
            }
        }

        private bool OpenConditionEditor(EventCommand cmd)
        {
            var cmdWindow = new EventCommand_ConditionalBranch(cmd, null, null);
            var frm = new Form
            {
                Text = Strings.Get("dynamicrequirements", "conditioneditor"),
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
                _edittingList.Conditions.RemoveAt(lstConditions.SelectedIndex);
                UpdateConditions(_edittingList);
            }
        }

        private void btnConditionsOkay_Click(object sender, EventArgs e)
        {
            _sourceList.Load(_edittingList.Data());
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
            _sourceLists.Load(_edittingLists.Data());
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