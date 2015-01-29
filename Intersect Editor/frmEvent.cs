using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Intersect_Editor
{
    public partial class frmEvent : Form
    {
        public Event myEvent;
        public int currentPageIndex;
        public EventPage currentPage;
        private ByteBuffer eventBackup = new ByteBuffer();
        private List<CommandListProperties> commandProperties = new List<CommandListProperties>();
        private int currentCommand = -1;
        private EventCommand editingCommand;
        private bool isInsert;
        private bool isEdit;
        public Map myMap;
        public bool newEvent;
        public frmEvent()
        {
            InitializeComponent();
        }

        private void frmEvent_Load(object sender, EventArgs e)
        {
            grpNewCommands.BringToFront();
            grpCreateCommands.BringToFront();
        }

        private void frmEvent_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (MessageBox.Show("Do you want to save changes to this event?", "Save Event?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    btnSave_Click(null, null);
                }
                else
                {
                    btnCancel_Click(null, null);
                }
            }
        }

        public void initEditor()
        {
            eventBackup = new ByteBuffer();
            eventBackup.WriteBytes(myEvent.EventData());
            txtEventname.Text = myEvent.myName;
            cmbCond1.Items.Clear();
            cmbCond1.Items.Add("None");
            cmbCond2.Items.Clear();
            cmbCond2.Items.Add("None");
            for (int i = 0; i < Constants.SWITCH_COUNT; i++)
            {
                cmbCond1.Items.Add("Switch " + (i + 1));
                cmbCond2.Items.Add("Switch " + (i + 1));
            }
            loadPage(0);
        }

        public void loadPage(int pageNum)
        {
            currentPageIndex = pageNum;
            currentPage = myEvent.myPages[0];
            cmbMoveType.SelectedIndex = currentPage.movementType;
            cmbEventSpeed.SelectedIndex = currentPage.movementSpeed;
            cmbEventFreq.SelectedIndex = currentPage.movementFreq;
            chkWalkThrough.Checked = Convert.ToBoolean(currentPage.passable);
            cmbLayering.SelectedIndex = currentPage.layer;
            cmbTrigger.SelectedIndex = currentPage.trigger;
            cmbCond1.SelectedIndex = currentPage.myConditions.switch1;
            cmbCond2.SelectedIndex = currentPage.myConditions.switch2;
            cmbCond1Val.SelectedIndex = Convert.ToInt32(currentPage.myConditions.switch1val);
            cmbCond2Val.SelectedIndex = Convert.ToInt32(currentPage.myConditions.switch2val);
            txtPageGraphic.Text = currentPage.graphic;
            cmbEventDir.SelectedIndex = currentPage.graphicy;
            chkHideName.Checked = Convert.ToBoolean(currentPage.hideName);
            ListPageCommands();
        }

        private void ListPageCommands()
        {
            lstEventCommands.Items.Clear();
            commandProperties.Clear();
            PrintCommandList(currentPage.commandLists[0], " ");
        }

        private void PrintCommandList(CommandList commandList, string indent)
        {
            CommandListProperties clp;
            if (commandList.commands.Count > 0)
            {
                for (int i = 0; i < commandList.commands.Count; i++)
                {
                    switch (commandList.commands[i].type)
                    {
                        case 1:
                            lstEventCommands.Items.Add(indent + "@> " + GetCommandText(commandList.commands[i]));
                            clp = new CommandListProperties();
                            clp.editable = true;
                            clp.myIndex = i;
                            clp.myList = commandList;
                            clp.cmd = commandList.commands[i];
                            clp.type = commandList.commands[i].type;
                            commandProperties.Add(clp);
                            for (int x = 1; x < 5; x++)
                            {
                                if (commandList.commands[i].strs[x].Trim().Length > 0)
                                {
                                    lstEventCommands.Items.Add(indent + "      : When [" + Truncate(commandList.commands[i].strs[x], 20) + "]");
                                    clp = new CommandListProperties();
                                    clp.editable = false;
                                    clp.myIndex = i;
                                    clp.myList = commandList;
                                    clp.type = commandList.commands[i].type;
                                    clp.cmd = commandList.commands[i];
                                    commandProperties.Add(clp);
                                    PrintCommandList(currentPage.commandLists[commandList.commands[i].ints[x - 1]], indent + "          ");
                                }
                            }
                            lstEventCommands.Items.Add(indent + "      : End Options");
                            clp = new CommandListProperties();
                            clp.editable = false;
                            clp.myIndex = i;
                            clp.myList = commandList;
                            clp.type = commandList.commands[i].type;
                            clp.cmd = commandList.commands[i];

                            commandProperties.Add(clp);
                            break;
                        case 4:
                            lstEventCommands.Items.Add(indent + "@> " + GetCommandText(commandList.commands[i]));
                            clp = new CommandListProperties();
                            clp.editable = true;
                            clp.myIndex = i;
                            clp.myList = commandList;
                            clp.cmd = commandList.commands[i];
                            clp.type = commandList.commands[i].type;
                            commandProperties.Add(clp);

                            PrintCommandList(currentPage.commandLists[commandList.commands[i].ints[0]], indent + "          ");

                            lstEventCommands.Items.Add(indent + "      : Else");
                            clp = new CommandListProperties();
                            clp.editable = false;
                            clp.myIndex = i;
                            clp.myList = commandList;
                            clp.type = commandList.commands[i].type;
                            clp.cmd = commandList.commands[i];
                            commandProperties.Add(clp);

                            PrintCommandList(currentPage.commandLists[commandList.commands[i].ints[1]], indent + "          ");




                            lstEventCommands.Items.Add(indent + "      : End Branch");
                            clp = new CommandListProperties();
                            clp.editable = false;
                            clp.myIndex = i;
                            clp.myList = commandList;
                            clp.type = commandList.commands[i].type;
                            clp.cmd = commandList.commands[i];

                            commandProperties.Add(clp);
                            break;
                        default:
                            lstEventCommands.Items.Add(indent + "@> " + GetCommandText(commandList.commands[i]));
                            clp = new CommandListProperties();
                            clp.editable = true;
                            clp.myIndex = i;
                            clp.myList = commandList;
                            clp.type = commandList.commands[i].type;
                            clp.cmd = commandList.commands[i];
                            commandProperties.Add(clp);
                            break;
                    }

                }
            }
            lstEventCommands.Items.Add(indent + "@> ");
            clp = new CommandListProperties();
            clp.editable = true;
            clp.myIndex = -1;
            clp.myList = commandList;
            clp.type = -1;
            commandProperties.Add(clp);
        }

        private string GetCommandText(EventCommand command)
        {
            switch (command.type)
            {
                case 0:
                    return "Show Text: " + Truncate(command.strs[0], 30);
                case 1:
                    return "Show Options: " + Truncate(command.strs[0], 30);
                case 2:
                    return "Set Switch #" + command.ints[0] + " to " + Convert.ToBoolean(command.ints[1]);
                case 4:
                    return "Conditional Branch:";
                case 5:
                    return "Warp Player [Map: " + command.ints[0] + " X: " + command.ints[1] + " Y: " + command.ints[2] + " Dir: " + command.ints[3] + "]";
                default:
                    return "Unknown Command";
            }
        }

        private string Truncate(string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + " ..";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (grpCreateCommands.Visible)
            {
                btnCreateCancel_Click(null, null);
            }
            if (newEvent)
            {
                myMap.Events.Remove(myEvent);
            }
            else
            {
                myEvent = new Event(eventBackup);
            }
            Hide();
            this.Dispose();
        }

        private void txtEventname_TextChanged(object sender, EventArgs e)
        {
            myEvent.myName = txtEventname.Text;
        }

        private void cmbMoveType_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPage.movementType = cmbMoveType.SelectedIndex;
        }

        private void cmbEventSpeed_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPage.movementSpeed = cmbEventSpeed.SelectedIndex;
        }

        private void cmbEventFreq_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPage.movementFreq = cmbEventFreq.SelectedIndex;
        }

        private void chkWalkThrough_CheckedChanged(object sender, EventArgs e)
        {
            currentPage.passable = Convert.ToInt32(chkWalkThrough.Checked);
        }

        private void cmbLayering_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPage.layer = cmbLayering.SelectedIndex;
        }

        private void cmbTrigger_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPage.trigger = cmbTrigger.SelectedIndex;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (grpCreateCommands.Visible)
            {
                btnCreateCancel_Click(null, null);
            }
            Hide();
            this.Dispose();
        }

        private void lstEventCommands_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentCommand = lstEventCommands.SelectedIndex;
        }

        private void lstEventCommands_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (currentCommand > -1)
                {
                    if (commandProperties[currentCommand].editable)
                    {
                        commandProperties[currentCommand].myList.commands.Remove(commandProperties[currentCommand].cmd);
                        ListPageCommands();
                    }
                }
            }
        }

        private void lstEventCommands_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                int i = lstEventCommands.IndexFromPoint(e.Location);
                if (i > -1 && i < lstEventCommands.Items.Count)
                {
                    if (commandProperties[i].editable)
                    {
                        lstEventCommands.SelectedIndex = i;
                        commandMenu.Show((ListBox)sender, e.Location);
                        if (commandProperties[i].type > -1)
                        {
                            btnEdit.Enabled = true;
                            btnDelete.Enabled = true;
                        }
                        else
                        {
                            btnEdit.Enabled = false;
                            btnDelete.Enabled = false;
                        }
                    }
                }
            }
        }

        private void lstEventCommands_DoubleClick(object sender, EventArgs e)
        {
            if (currentCommand > -1)
            {
                if (commandProperties[currentCommand].editable)
                {
                    if (commandProperties[currentCommand].type == -1)
                    {
                        grpNewCommands.Show();
                        isInsert = false;
                    }
                    else
                    {
                        grpNewCommands.Show();
                        isInsert = true;
                    }
                }
            }
        }

        private void lstCommands_ItemActivated(object sender, EventArgs e)
        {
            if (lstCommands.SelectedItems.Count == 0) { return; }
            EventCommand tmpCommand = new EventCommand();
            grpNewCommands.Hide();
            tmpCommand.type = Convert.ToInt32(lstCommands.SelectedItems[0].Tag);
            if (isInsert)
            {
                commandProperties[currentCommand].myList.commands.Insert(commandProperties[currentCommand].myList.commands.IndexOf(commandProperties[currentCommand].cmd), tmpCommand);
            }
            else
            {
                commandProperties[currentCommand].myList.commands.Add(tmpCommand);
            }
            OpenEditCommand(tmpCommand);
            isEdit = false;
        }

        private void OpenEditCommand(EventCommand command)
        {
            grpCreateCommands.Show();
            grpShowText.Hide();
            grpShowOptions.Hide();
            grpSetSwitch.Hide();
            grpNewCondition.Hide();
            grpCreateWarp.Hide();
            switch (command.type)
            {
                case 0:
                    grpShowText.Show();
                    txtShowText.Text = command.strs[0];
                    break;
                case 1:
                    grpShowOptions.Show();
                    txtShowOptions.Text = command.strs[0];
                    txtShowOptionsOpt1.Text = command.strs[1];
                    txtShowOptionsOpt2.Text = command.strs[2];
                    txtShowOptionsOpt3.Text = command.strs[3];
                    txtShowOptionsOpt4.Text = command.strs[4];
                    break;
                case 2:
                    grpSetSwitch.Show();
                    if (cmbSetSwitch.Items.Count == 0)
                    {
                        for (int i = 0; i < Constants.SWITCH_COUNT; i++)
                        {
                            cmbSetSwitch.Items.Add("Switch " + (i + 1));
                        }
                    }
                    if (command.ints[0] > 0)
                    {
                        cmbSetSwitch.SelectedIndex = command.ints[0] - 1;
                    }
                    else
                    {
                        cmbSetSwitch.SelectedIndex = 0;
                        command.ints[0] = 1;
                    }
                    cmbSetSwitchVal.SelectedIndex = command.ints[1];
                    break;
                case 4:
                    grpNewCondition.Show();
                    if (cmbNewCond1.Items.Count == 0)
                    {
                        cmbNewCond1.Items.Add("None");
                        cmbNewCond2.Items.Add("None");
                        for (int i = 0; i < Constants.SWITCH_COUNT; i++)
                        {
                            cmbNewCond1.Items.Add("Switch " + (i + 1));
                            cmbNewCond2.Items.Add("Switch " + (i + 1));
                        }
                    }
                    cmbNewCond1.SelectedIndex = command.myConditions.switch1;
                    cmbNewCond2.SelectedIndex = command.myConditions.switch2;
                    cmbNewCond1Val.SelectedIndex = Convert.ToInt32(command.myConditions.switch1val);
                    cmbNewCond2Val.SelectedIndex = Convert.ToInt32(command.myConditions.switch2val);
                    break;
                case 5:
                    grpCreateWarp.Show();
                    txtNewWarpMap.Text = command.ints[0] + "";
                    txtNewWarpX.Text = command.ints[1] + "";
                    txtNewWarpY.Text = command.ints[2] + "";
                    txtNewWarpDir.Text = command.ints[3] + "";
                    break;
            }
            editingCommand = command;
            btnSave.Enabled = false;
        }

        private void lstCommands_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnCreateOkay_Click(object sender, EventArgs e)
        {
            switch (editingCommand.type)
            {
                case 0:
                    editingCommand.strs[0] = txtShowText.Text;
                    break;
                case 1:
                    editingCommand.strs[0] = txtShowOptions.Text;
                    editingCommand.strs[1] = txtShowOptionsOpt1.Text;
                    editingCommand.strs[2] = txtShowOptionsOpt2.Text;
                    editingCommand.strs[3] = txtShowOptionsOpt3.Text;
                    editingCommand.strs[4] = txtShowOptionsOpt4.Text;
                    if (editingCommand.ints[0] == 0)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            currentPage.commandLists.Add(new CommandList());
                            editingCommand.ints[i] = currentPage.commandLists.Count - 1;
                        }
                    }
                    break;
                case 2:
                    editingCommand.ints[0] = cmbSetSwitch.SelectedIndex + 1;
                    editingCommand.ints[1] = cmbSetSwitchVal.SelectedIndex;
                    break;
                case 4:
                    editingCommand.myConditions.switch1 = cmbNewCond1.SelectedIndex;
                    editingCommand.myConditions.switch2 = cmbNewCond2.SelectedIndex;
                    editingCommand.myConditions.switch1val = Convert.ToBoolean(cmbNewCond1Val.SelectedIndex);
                    editingCommand.myConditions.switch2val = Convert.ToBoolean(cmbNewCond2Val.SelectedIndex);
                    if (editingCommand.ints[0] == 0)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            currentPage.commandLists.Add(new CommandList());
                            editingCommand.ints[i] = currentPage.commandLists.Count - 1;
                        }
                    }
                    break;
                case 5:
                    int tmp = Convert.ToInt32(txtNewWarpMap.Text);
                    if (tmp > -1 && tmp < Globals.MapRefs.Count())
                    {
                        editingCommand.ints[0] = tmp;
                    }
                    tmp = Convert.ToInt32(txtNewWarpX.Text);
                    if (tmp > -1 && tmp < 30)
                    {
                        editingCommand.ints[1] = tmp;
                    }
                    tmp = Convert.ToInt32(txtNewWarpY.Text);
                    if (tmp > -1 && tmp < 30)
                    {
                        editingCommand.ints[2] = tmp;
                    }
                    tmp = Convert.ToInt32(txtNewWarpDir.Text);
                    if (tmp > -1 && tmp < 4)
                    {
                        editingCommand.ints[3] = tmp;
                    }
                    break;
            }
            grpCreateCommands.Hide();
            ListPageCommands();
            btnSave.Enabled = true;
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (currentCommand > -1)
            {
                if (commandProperties[currentCommand].editable)
                {
                    if (commandProperties[currentCommand].type == -1)
                    {
                        grpNewCommands.Show();
                        isInsert = false;
                        isEdit = false;
                    }
                    else
                    {
                        grpNewCommands.Show();
                        isInsert = true;
                        isEdit = false;
                    }
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (currentCommand > -1)
            {
                if (commandProperties[currentCommand].editable)
                {
                    OpenEditCommand(commandProperties[currentCommand].myList.commands[commandProperties[currentCommand].myIndex]);
                    isEdit = true;
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (currentCommand > -1)
            {
                if (commandProperties[currentCommand].editable)
                {
                    commandProperties[currentCommand].myList.commands.Remove(commandProperties[currentCommand].cmd);
                    ListPageCommands();
                }
            }
        }

        private void btnCreateCancel_Click(object sender, EventArgs e)
        {
            if (!isEdit)
            {
                if (isInsert)
                {
                    commandProperties[currentCommand].myList.commands.RemoveAt(commandProperties[currentCommand].myList.commands.IndexOf(commandProperties[currentCommand].cmd) - 1);
                }
                else
                {
                    commandProperties[currentCommand].myList.commands.RemoveAt(commandProperties[currentCommand].myList.commands.Count - 1);
                }
            }
            grpCreateCommands.Hide();
            ListPageCommands();
            btnSave.Enabled = true;
        }

        private void cmbCond1_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPage.myConditions.switch1 = cmbCond1.SelectedIndex;
        }

        private void cmbCond2_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPage.myConditions.switch2 = cmbCond2.SelectedIndex;
        }

        private void cmbCond1Val_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPage.myConditions.switch1val = Convert.ToBoolean(cmbCond1Val.SelectedIndex);
        }

        private void cmbCond2Val_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPage.myConditions.switch2val = Convert.ToBoolean(cmbCond2Val.SelectedIndex);
        }

        private void txtPageGraphic_TextChanged(object sender, EventArgs e)
        {
            currentPage.graphic = txtPageGraphic.Text;
        }

        private void chkHideName_CheckedChanged(object sender, EventArgs e)
        {
            currentPage.hideName = Convert.ToInt32(chkHideName.Checked);
        }

        private void cmbEventDir_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPage.graphicy = cmbEventDir.SelectedIndex;
        }



    }

    public class CommandListProperties
    {
        public CommandList myList;
        public int myIndex;
        public bool editable;
        public EventCommand cmd;
        public int type;
    }
}
