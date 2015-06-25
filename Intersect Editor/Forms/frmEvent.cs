/*
    Intersect Game Engine (Server)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Intersect_Editor.Classes;

namespace Intersect_Editor.Forms
{
    public partial class FrmEvent : Form
    {
        public EventStruct MyEvent;
        public int CurrentPageIndex;
        public EventPage CurrentPage;
        private ByteBuffer _eventBackup = new ByteBuffer();
        private readonly List<CommandListProperties> _commandProperties = new List<CommandListProperties>();
        private int _currentCommand = -1;
        private EventCommand _editingCommand;
        private bool _isInsert;
        private bool _isEdit;
        public MapStruct MyMap;
        public bool NewEvent;
        public FrmEvent()
        {
            InitializeComponent();
        }

        private void frmEvent_Load(object sender, EventArgs e)
        {
            grpNewCommands.BringToFront();
            grpCreateCommands.BringToFront();
            cmbEventSprite.Items.Clear();
            cmbEventSprite.Items.Add("None");
            for (int i = 0; i < Graphics.EntityFileNames.Length; i++)
            {
                cmbEventSprite.Items.Add(Graphics.EntityFileNames[i]);
            }
            cmbPreviewFace.Items.Clear();
            cmbPreviewFace.Items.Add("None");
            for (int i = 0; i < Graphics.FaceFileNames.Count; i++)
            {
                cmbPreviewFace.Items.Add(Graphics.FaceFileNames[i]);
            }
        }

        private void frmEvent_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing) return;
            if (MessageBox.Show(@"Do you want to save changes to this event?", @"Save Event?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                btnSave_Click(null, null);
            }
            else
            {
                btnCancel_Click(null, null);
            }
        }

        public void InitEditor()
        {
            _eventBackup = new ByteBuffer();
            _eventBackup.WriteBytes(MyEvent.EventData());
            txtEventname.Text = MyEvent.MyName;
            cmbCond1.Items.Clear();
            cmbCond1.Items.Add("None");
            cmbCond2.Items.Clear();
            cmbCond2.Items.Add("None");
            for (var i = 0; i < Constants.SwitchCount; i++)
            {
                cmbCond1.Items.Add("Switch " + (i + 1));
                cmbCond2.Items.Add("Switch " + (i + 1));
            }
            LoadPage(0);
        }

        public void LoadPage(int pageNum)
        {
            CurrentPageIndex = pageNum;
            CurrentPage = MyEvent.MyPages[0];
            cmbMoveType.SelectedIndex = CurrentPage.MovementType;
            cmbEventSpeed.SelectedIndex = CurrentPage.MovementSpeed;
            cmbEventFreq.SelectedIndex = CurrentPage.MovementFreq;
            chkWalkThrough.Checked = Convert.ToBoolean(CurrentPage.Passable);
            cmbLayering.SelectedIndex = CurrentPage.Layer;
            cmbTrigger.SelectedIndex = CurrentPage.Trigger;
            cmbCond1.SelectedIndex = CurrentPage.MyConditions.Switch1;
            cmbCond2.SelectedIndex = CurrentPage.MyConditions.Switch2;
            cmbCond1Val.SelectedIndex = Convert.ToInt32(CurrentPage.MyConditions.Switch1Val);
            cmbCond2Val.SelectedIndex = Convert.ToInt32(CurrentPage.MyConditions.Switch2Val);
            cmbEventSprite.SelectedIndex = cmbEventSprite.Items.IndexOf(CurrentPage.Graphic);
            cmbPreviewFace.SelectedIndex = cmbPreviewFace.Items.IndexOf(CurrentPage.FaceGraphic);
            cmbEventDir.SelectedIndex = CurrentPage.Graphicy;
            chkHideName.Checked = Convert.ToBoolean(CurrentPage.HideName);
            chkDisablePreview.Checked = Convert.ToBoolean(CurrentPage.DisablePreview);
            txtDesc.Text = CurrentPage.Desc;
            ListPageCommands();
        }

        private void ListPageCommands()
        {
            lstEventCommands.Items.Clear();
            _commandProperties.Clear();
            PrintCommandList(CurrentPage.CommandLists[0], " ");
        }

        private void PrintCommandList(CommandList commandList, string indent)
        {
            CommandListProperties clp;
            if (commandList.Commands.Count > 0)
            {
                for (var i = 0; i < commandList.Commands.Count; i++)
                {
                    switch (commandList.Commands[i].Type)
                    {
                        case 1:
                            lstEventCommands.Items.Add(indent + "@> " + GetCommandText(commandList.Commands[i]));
                            clp = new CommandListProperties
                            {
                                Editable = true,
                                MyIndex = i,
                                MyList = commandList,
                                Cmd = commandList.Commands[i],
                                Type = commandList.Commands[i].Type
                            };
                            _commandProperties.Add(clp);
                            for (var x = 1; x < 5; x++)
                            {
                                if (commandList.Commands[i].Strs[x].Trim().Length <= 0) continue;
                                lstEventCommands.Items.Add(indent + "      : When [" + Truncate(commandList.Commands[i].Strs[x], 20) + "]");
                                clp = new CommandListProperties
                                {
                                    Editable = false,
                                    MyIndex = i,
                                    MyList = commandList,
                                    Type = commandList.Commands[i].Type,
                                    Cmd = commandList.Commands[i]
                                };
                                _commandProperties.Add(clp);
                                PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[x - 1]], indent + "          ");
                            }
                            lstEventCommands.Items.Add(indent + "      : End Options");
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };

                            _commandProperties.Add(clp);
                            break;
                        case 4:
                            lstEventCommands.Items.Add(indent + "@> " + GetCommandText(commandList.Commands[i]));
                            clp = new CommandListProperties
                            {
                                Editable = true,
                                MyIndex = i,
                                MyList = commandList,
                                Cmd = commandList.Commands[i],
                                Type = commandList.Commands[i].Type
                            };
                            _commandProperties.Add(clp);

                            PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[0]], indent + "          ");

                            lstEventCommands.Items.Add(indent + "      : Else");
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };
                            _commandProperties.Add(clp);

                            PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[1]], indent + "          ");
                            
                            lstEventCommands.Items.Add(indent + "      : End Branch");
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };

                            _commandProperties.Add(clp);
                            break;
                        default:
                            lstEventCommands.Items.Add(indent + "@> " + GetCommandText(commandList.Commands[i]));
                            clp = new CommandListProperties
                            {
                                Editable = true,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };
                            _commandProperties.Add(clp);
                            break;
                    }

                }
            }
            lstEventCommands.Items.Add(indent + "@> ");
            clp = new CommandListProperties {Editable = true, MyIndex = -1, MyList = commandList, Type = -1};
            _commandProperties.Add(clp);
        }

        private string GetCommandText(EventCommand command)
        {
            switch (command.Type)
            {
                case 0:
                    return "Show Text: " + Truncate(command.Strs[0], 30);
                case 1:
                    return "Show Options: " + Truncate(command.Strs[0], 30);
                case 2:
                    return "Set Switch #" + command.Ints[0] + " to " + Convert.ToBoolean(command.Ints[1]);
                case 4:
                    return "Conditional Branch:";
                case 5:
                    return "Warp Player [Map: " + command.Ints[0] + " X: " + command.Ints[1] + " Y: " + command.Ints[2] + " Dir: " + command.Ints[3] + "]";
                default:
                    return "Unknown Command";
            }
        }

        private static string Truncate(string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + " ..";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (grpCreateCommands.Visible)
            {
                btnCreateCancel_Click(null, null);
            }
            if (NewEvent)
            {
                MyMap.Events.Remove(MyEvent);
            }
            else
            {
                MyEvent = new EventStruct(_eventBackup);
            }
            Hide();
            Dispose();
        }

        private void txtEventname_TextChanged(object sender, EventArgs e)
        {
            MyEvent.MyName = txtEventname.Text;
        }

        private void cmbMoveType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage.MovementType = cmbMoveType.SelectedIndex;
        }

        private void cmbEventSpeed_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage.MovementSpeed = cmbEventSpeed.SelectedIndex;
        }

        private void cmbEventFreq_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage.MovementFreq = cmbEventFreq.SelectedIndex;
        }

        private void chkWalkThrough_CheckedChanged(object sender, EventArgs e)
        {
            CurrentPage.Passable = Convert.ToInt32(chkWalkThrough.Checked);
        }

        private void cmbLayering_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage.Layer = cmbLayering.SelectedIndex;
        }

        private void cmbTrigger_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage.Trigger = cmbTrigger.SelectedIndex;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (grpCreateCommands.Visible)
            {
                btnCreateCancel_Click(null, null);
            }
            Hide();
            Dispose();
        }

        private void lstEventCommands_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentCommand = lstEventCommands.SelectedIndex;
        }

        private void lstEventCommands_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Delete) return;
            if (_currentCommand <= -1) return;
            if (!_commandProperties[_currentCommand].Editable) return;
            _commandProperties[_currentCommand].MyList.Commands.Remove(_commandProperties[_currentCommand].Cmd);
            ListPageCommands();
        }

        private void lstEventCommands_Click(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            var i = lstEventCommands.IndexFromPoint(e.Location);
            if (i <= -1 || i >= lstEventCommands.Items.Count) return;
            if (!_commandProperties[i].Editable) return;
            lstEventCommands.SelectedIndex = i;
            commandMenu.Show((ListBox)sender, e.Location);
            if (_commandProperties[i].Type > -1)
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

        private void lstEventCommands_DoubleClick(object sender, EventArgs e)
        {
            if (_currentCommand <= -1) return;
            if (!_commandProperties[_currentCommand].Editable) return;
            if (_commandProperties[_currentCommand].Type == -1)
            {
                grpNewCommands.Show();
                _isInsert = false;
            }
            else
            {
                grpNewCommands.Show();
                _isInsert = true;
            }
        }

        private void lstCommands_ItemActivated(object sender, EventArgs e)
        {
            if (lstCommands.SelectedItems.Count == 0) { return; }
            var tmpCommand = new EventCommand();
            grpNewCommands.Hide();
            tmpCommand.Type = Convert.ToInt32(lstCommands.SelectedItems[0].Tag);
            if (_isInsert)
            {
                _commandProperties[_currentCommand].MyList.Commands.Insert(_commandProperties[_currentCommand].MyList.Commands.IndexOf(_commandProperties[_currentCommand].Cmd), tmpCommand);
            }
            else
            {
                _commandProperties[_currentCommand].MyList.Commands.Add(tmpCommand);
            }
            OpenEditCommand(tmpCommand);
            _isEdit = false;
        }

        private void OpenEditCommand(EventCommand command)
        {
            grpCreateCommands.Show();
            grpShowText.Hide();
            grpShowOptions.Hide();
            grpSetSwitch.Hide();
            grpNewCondition.Hide();
            grpCreateWarp.Hide();
            switch (command.Type)
            {
                case 0:
                    grpShowText.Show();
                    txtShowText.Text = command.Strs[0];
                    break;
                case 1:
                    grpShowOptions.Show();
                    txtShowOptions.Text = command.Strs[0];
                    txtShowOptionsOpt1.Text = command.Strs[1];
                    txtShowOptionsOpt2.Text = command.Strs[2];
                    txtShowOptionsOpt3.Text = command.Strs[3];
                    txtShowOptionsOpt4.Text = command.Strs[4];
                    break;
                case 2:
                    grpSetSwitch.Show();
                    if (cmbSetSwitch.Items.Count == 0)
                    {
                        for (var i = 0; i < Constants.SwitchCount; i++)
                        {
                            cmbSetSwitch.Items.Add("Switch " + (i + 1));
                        }
                    }
                    if (command.Ints[0] > 0)
                    {
                        cmbSetSwitch.SelectedIndex = command.Ints[0] - 1;
                    }
                    else
                    {
                        cmbSetSwitch.SelectedIndex = 0;
                        command.Ints[0] = 1;
                    }
                    cmbSetSwitchVal.SelectedIndex = command.Ints[1];
                    break;
                case 4:
                    grpNewCondition.Show();
                    if (cmbNewCond1.Items.Count == 0)
                    {
                        cmbNewCond1.Items.Add("None");
                        cmbNewCond2.Items.Add("None");
                        for (var i = 0; i < Constants.SwitchCount; i++)
                        {
                            cmbNewCond1.Items.Add("Switch " + (i + 1));
                            cmbNewCond2.Items.Add("Switch " + (i + 1));
                        }
                    }
                    cmbNewCond1.SelectedIndex = command.MyConditions.Switch1;
                    cmbNewCond2.SelectedIndex = command.MyConditions.Switch2;
                    cmbNewCond1Val.SelectedIndex = Convert.ToInt32(command.MyConditions.Switch1Val);
                    cmbNewCond2Val.SelectedIndex = Convert.ToInt32(command.MyConditions.Switch2Val);
                    break;
                case 5:
                    grpCreateWarp.Show();
                    txtNewWarpMap.Text = command.Ints[0] + "";
                    txtNewWarpX.Text = command.Ints[1] + "";
                    txtNewWarpY.Text = command.Ints[2] + "";
                    txtNewWarpDir.Text = command.Ints[3] + "";
                    break;
            }
            _editingCommand = command;
            btnSave.Enabled = false;
        }

        private void lstCommands_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnCreateOkay_Click(object sender, EventArgs e)
        {
            switch (_editingCommand.Type)
            {
                case 0:
                    _editingCommand.Strs[0] = txtShowText.Text;
                    break;
                case 1:
                    _editingCommand.Strs[0] = txtShowOptions.Text;
                    _editingCommand.Strs[1] = txtShowOptionsOpt1.Text;
                    _editingCommand.Strs[2] = txtShowOptionsOpt2.Text;
                    _editingCommand.Strs[3] = txtShowOptionsOpt3.Text;
                    _editingCommand.Strs[4] = txtShowOptionsOpt4.Text;
                    if (_editingCommand.Ints[0] == 0)
                    {
                        for (var i = 0; i < 4; i++)
                        {
                            CurrentPage.CommandLists.Add(new CommandList());
                            _editingCommand.Ints[i] = CurrentPage.CommandLists.Count - 1;
                        }
                    }
                    break;
                case 2:
                    _editingCommand.Ints[0] = cmbSetSwitch.SelectedIndex + 1;
                    _editingCommand.Ints[1] = cmbSetSwitchVal.SelectedIndex;
                    break;
                case 4:
                    _editingCommand.MyConditions.Switch1 = cmbNewCond1.SelectedIndex;
                    _editingCommand.MyConditions.Switch2 = cmbNewCond2.SelectedIndex;
                    _editingCommand.MyConditions.Switch1Val = Convert.ToBoolean(cmbNewCond1Val.SelectedIndex);
                    _editingCommand.MyConditions.Switch2Val = Convert.ToBoolean(cmbNewCond2Val.SelectedIndex);
                    if (_editingCommand.Ints[0] == 0)
                    {
                        for (var i = 0; i < 2; i++)
                        {
                            CurrentPage.CommandLists.Add(new CommandList());
                            _editingCommand.Ints[i] = CurrentPage.CommandLists.Count - 1;
                        }
                    }
                    break;
                case 5:
                    var tmp = Convert.ToInt32(txtNewWarpMap.Text);
                    if (tmp > -1 && tmp < Globals.MapRefs.Count())
                    {
                        _editingCommand.Ints[0] = tmp;
                    }
                    tmp = Convert.ToInt32(txtNewWarpX.Text);
                    if (tmp > -1 && tmp < 30)
                    {
                        _editingCommand.Ints[1] = tmp;
                    }
                    tmp = Convert.ToInt32(txtNewWarpY.Text);
                    if (tmp > -1 && tmp < 30)
                    {
                        _editingCommand.Ints[2] = tmp;
                    }
                    tmp = Convert.ToInt32(txtNewWarpDir.Text);
                    if (tmp > -1 && tmp < 4)
                    {
                        _editingCommand.Ints[3] = tmp;
                    }
                    break;
            }
            grpCreateCommands.Hide();
            ListPageCommands();
            btnSave.Enabled = true;
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (_currentCommand <= -1) return;
            if (!_commandProperties[_currentCommand].Editable) return;
            if (_commandProperties[_currentCommand].Type == -1)
            {
                grpNewCommands.Show();
                _isInsert = false;
                _isEdit = false;
            }
            else
            {
                grpNewCommands.Show();
                _isInsert = true;
                _isEdit = false;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (_currentCommand <= -1) return;
            if (!_commandProperties[_currentCommand].Editable) return;
            OpenEditCommand(_commandProperties[_currentCommand].MyList.Commands[_commandProperties[_currentCommand].MyIndex]);
            _isEdit = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_currentCommand <= -1) return;
            if (!_commandProperties[_currentCommand].Editable) return;
            _commandProperties[_currentCommand].MyList.Commands.Remove(_commandProperties[_currentCommand].Cmd);
            ListPageCommands();
        }

        private void btnCreateCancel_Click(object sender, EventArgs e)
        {
            if (!_isEdit)
            {
                if (_isInsert)
                {
                    _commandProperties[_currentCommand].MyList.Commands.RemoveAt(_commandProperties[_currentCommand].MyList.Commands.IndexOf(_commandProperties[_currentCommand].Cmd) - 1);
                }
                else
                {
                    _commandProperties[_currentCommand].MyList.Commands.RemoveAt(_commandProperties[_currentCommand].MyList.Commands.Count - 1);
                }
            }
            grpCreateCommands.Hide();
            ListPageCommands();
            btnSave.Enabled = true;
        }

        private void cmbCond1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage.MyConditions.Switch1 = cmbCond1.SelectedIndex;
        }

        private void cmbCond2_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage.MyConditions.Switch2 = cmbCond2.SelectedIndex;
        }

        private void cmbCond1Val_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage.MyConditions.Switch1Val = Convert.ToBoolean(cmbCond1Val.SelectedIndex);
        }

        private void cmbCond2Val_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage.MyConditions.Switch2Val = Convert.ToBoolean(cmbCond2Val.SelectedIndex);
        }

        private void chkHideName_CheckedChanged(object sender, EventArgs e)
        {
            CurrentPage.HideName = Convert.ToInt32(chkHideName.Checked);
        }

        private void cmbEventDir_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage.Graphicy = cmbEventDir.SelectedIndex;
        }

        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            CurrentPage.Desc = txtDesc.Text;
        }

        private void chkDisablePreview_CheckedChanged(object sender, EventArgs e)
        {
            CurrentPage.DisablePreview = Convert.ToInt32(chkDisablePreview.Checked);
        }

        private void cmbEventSprite_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage.Graphic = cmbEventSprite.Text;
        }

        private void cmbPreviewFace_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage.FaceGraphic = cmbPreviewFace.Text;
        }



    }

    public class CommandListProperties
    {
        public CommandList MyList;
        public int MyIndex;
        public bool Editable;
        public EventCommand Cmd;
        public int Type;
    }
}
