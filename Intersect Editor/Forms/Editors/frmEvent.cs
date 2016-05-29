/*
    Intersect Game Engine (Editor)
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
using Intersect_Editor.Forms.Editors.Event_Commands;
using System.Drawing;
using System.IO;
using Intersect_Editor.Classes.Core;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.GameObjects.Maps;
using Intersect_Library.GameObjects.Maps.MapList;
using Color = System.Drawing.Color;

namespace Intersect_Editor.Forms
{
    public partial class FrmEvent : Form
    {
        private readonly MapBase _currentMap;
        public EventBase MyEvent;
        public int CurrentPageIndex;
        public EventPage CurrentPage;
        private ByteBuffer _eventBackup = new ByteBuffer();
        private readonly List<CommandListProperties> _commandProperties = new List<CommandListProperties>();
        private int _currentCommand = -1;
        private EventCommand _editingCommand;
        private bool _isInsert;
        private bool _isEdit;
        public MapBase MyMap;
        public bool NewEvent;
        private ByteBuffer _pageCopy;

        #region "Form Events"
        public FrmEvent(MapBase currentMap)
        {
            InitializeComponent();
            _currentMap = currentMap;
        }
        private void frmEvent_Load(object sender, EventArgs e)
        {
            grpNewCommands.BringToFront();
            grpCreateCommands.BringToFront();
        }
        /// <summary>
        /// Intercepts the form closing event to ask the user if they want to save their changes or not.
        /// </summary>
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
        private void FrmEvent_FormClosed(object sender, FormClosedEventArgs e)
        {
            Globals.CurrentEditor = -1;
        }
        private void FrmEvent_VisibleChanged(object sender, EventArgs e)
        {
            //btnCancel_Click(null, null);
        }
        #endregion

        #region "Loading/Initialization/Saving"
        /// <summary>
        /// This function creates a backup of the event we are editting just in case we canel our revisions. 
        /// It also populates general lists in our editor (ie. switches/variables) for event spawning conditions.
        /// If the event is a common event (not a map entity) we hide the entity options on the form.
        /// </summary>
        public void InitEditor()
        {
            _eventBackup = new ByteBuffer();
            _eventBackup.WriteBytes(MyEvent.EventData());
            txtEventname.Text = MyEvent.MyName;
            cmbPreviewFace.Items.Clear();
            cmbPreviewFace.Items.Add("None");
            cmbPreviewFace.Items.AddRange(GameContentManager.GetTextureNames(GameContentManager.TextureType.Face));
            cmbAnimation.Items.Clear();
            cmbAnimation.Items.Add("None");
            cmbAnimation.Items.AddRange(Database.GetGameObjectList(GameObject.Animation));
            if (MyEvent.CommonEvent)
            {
                grpEntityOptions.Hide();
            }
            chkIsGlobal.Checked = Convert.ToBoolean(MyEvent.IsGlobal);
            if (MyEvent.CommonEvent) chkIsGlobal.Hide();
            tabControl.TabPages.Clear();
            for (int i = 0; i < MyEvent.MyPages.Count; i++)
                tabControl.TabPages.Add((i + 1).ToString());
            LoadPage(0);
        }
        /// <summary>
        /// Initializes all of the form controls with values from the passed event page.
        /// </summary>
        /// <param name="pageNum">The index of the page to load.</param>
        public void LoadPage(int pageNum)
        {
            this.Text = @"Event Editor - Event #" + MyEvent.MyIndex + @": " + txtEventname.Text;
            CurrentPageIndex = pageNum;
            CurrentPage = MyEvent.MyPages[pageNum];
            tabControl.SelectedIndex = CurrentPageIndex;
            cmbMoveType.SelectedIndex = CurrentPage.MovementType;
            if (CurrentPage.MovementType == 2)
            {
                btnSetRoute.Enabled = true;
            }
            else
            {
                btnSetRoute.Enabled = false;
            }
            cmbEventSpeed.SelectedIndex = CurrentPage.MovementSpeed;
            cmbEventFreq.SelectedIndex = CurrentPage.MovementFreq;
            chkWalkThrough.Checked = Convert.ToBoolean(CurrentPage.Passable);
            cmbLayering.SelectedIndex = CurrentPage.Layer;
            cmbTrigger.SelectedIndex = CurrentPage.Trigger;
            cmbPreviewFace.SelectedIndex = cmbPreviewFace.Items.IndexOf(CurrentPage.FaceGraphic);
            if (cmbPreviewFace.SelectedIndex == -1)
            {
                cmbPreviewFace.SelectedIndex = 0;
                UpdateFacePreview();
            }
            cmbAnimation.SelectedIndex = Database.GameObjectListIndex(GameObject.Animation, CurrentPage.Animation) + 1;
            chkHideName.Checked = Convert.ToBoolean(CurrentPage.HideName);
            chkDisablePreview.Checked = Convert.ToBoolean(CurrentPage.DisablePreview);
            chkDirectionFix.Checked = Convert.ToBoolean(CurrentPage.DirectionFix);
            chkWalkingAnimation.Checked = Convert.ToBoolean(CurrentPage.WalkingAnimation);
            txtDesc.Text = CurrentPage.Desc;
            ListPageCommands();
            ListPageConditions();
            UpdateEventPreview();
            EnableButtons();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (grpCreateCommands.Visible)
            {
                CancelCommandEdit();
            }
            if (NewEvent)
            {
                if (MyMap.EventIndex == MyEvent.MyIndex + 1) MyMap.EventIndex--;
                MyMap.Events.Remove(MyEvent.MyIndex);
            }
            else
            {
                MyEvent = new EventBase(MyEvent.MyIndex, _eventBackup);
            }
            Hide();
            Dispose();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (grpCreateCommands.Visible)
            {
                CancelCommandEdit();
            }
            if (MyEvent.CommonEvent)
            {
                PacketSender.SendSaveObject(MyEvent);
            }
            Hide();
            Dispose();
        }
        #endregion

        #region "CommandList Events/Functions"
        /// <summary>
        /// Clears the listbox that displays event commands and re-populates it.
        /// </summary>
        private void ListPageCommands()
        {
            lstEventCommands.Items.Clear();
            _commandProperties.Clear();
            PrintCommandList(CurrentPage.CommandLists[0], " ");
        }

        /// <summary>
        /// Recursively prints the referenced command list and all of it's children.
        /// </summary>
        /// <param name="commandList">The command list to print.</param>
        /// <param name="indent">The starting indent of commands in this list.</param>
        private void PrintCommandList(CommandList commandList, string indent)
        {
            CommandListProperties clp;
            if (commandList.Commands.Count > 0)
            {
                for (var i = 0; i < commandList.Commands.Count; i++)
                {
                    switch (commandList.Commands[i].Type)
                    {
                        case EventCommandType.ShowOptions:
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
                        case EventCommandType.ConditionalBranch:
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

                            PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[4]], indent + "          ");

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

                            PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[5]], indent + "          ");

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
                        case EventCommandType.ChangeSpells:
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

                            //When the spell was successfully taught:
                            lstEventCommands.Items.Add(indent + "      : Spell Taught/Removed Successfully");
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };
                            _commandProperties.Add(clp);
                            PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[4]], indent + "          ");

                            //When the spell failed to be taught:
                            lstEventCommands.Items.Add(indent + "      : Spell Not Taught/Removed (Already Knew/Spellbook full/Didn't know)");
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };
                            _commandProperties.Add(clp);
                            PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[5]], indent + "          ");


                            lstEventCommands.Items.Add(indent + "      : End Spell Change");
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
                        case EventCommandType.ChangeItems:
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

                            //When the item(s) were successfully given/taken:
                            lstEventCommands.Items.Add(indent + "      : Item(s) Given/Taken Successfully");
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };
                            _commandProperties.Add(clp);
                            PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[4]], indent + "          ");

                            //When the items failed to be given/taken:
                            lstEventCommands.Items.Add(indent + "      : Item(s) Not Given/Taken (Doesn't have/Inventory full)");
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };
                            _commandProperties.Add(clp);
                            PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[5]], indent + "          ");


                            lstEventCommands.Items.Add(indent + "      : End Item Change");
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
            clp = new CommandListProperties { Editable = true, MyIndex = -1, MyList = commandList };
            _commandProperties.Add(clp);
        }

        /// <summary>
        /// Given a command, this returns the string that should be displayed for it on the form's command list.
        /// </summary>
        /// <param name="command">The command to generate a summary for.</param>
        /// <returns></returns>
        private string GetCommandText(EventCommand command)
        {
            string output = "";
            switch (command.Type)
            {
                case EventCommandType.ShowText:
                    return "Show Text: " + Truncate(command.Strs[0], 30);
                case EventCommandType.ShowOptions:
                    return "Show Options: " + Truncate(command.Strs[0], 30);
                case EventCommandType.AddChatboxText:
                    output = "Show Chatbox Text [Channel: ";
                    switch (command.Ints[0])
                    {
                        case 0:
                            output += "Player";
                            break;
                        case 1:
                            output += "Local";
                            break;
                        case 2:
                            output += "Global";
                            break;
                    }
                    output += ", Color: " + command.Strs[1] + "] - ";
                    output += Truncate(command.Strs[0], 20);
                    return output;
                case EventCommandType.SetSwitch:
                    if (command.Ints[0] == (int)SwitchVariableTypes.PlayerSwitch)
                    {
                        return "Set Player Switch " + PlayerSwitchBase.GetName(command.Ints[1]) + " to " + Convert.ToBoolean(command.Ints[2]);
                    }
                    else if (command.Ints[0] == (int)SwitchVariableTypes.ServerSwitch)
                    {
                        return "Set Global Switch " + ServerSwitchBase.GetName(command.Ints[1]) + " to " + Convert.ToBoolean(command.Ints[2]);
                    }
                    else
                    {
                        return "Invalid Command";
                    }
                case EventCommandType.SetVariable:
                    if (command.Ints[0] == (int)SwitchVariableTypes.PlayerVariable)
                    {
                        output = "Set Player Variable " + PlayerVariableBase.GetName(command.Ints[1]) + " (";
                    }
                    else if (command.Ints[0] == (int)SwitchVariableTypes.ServerVariable)
                    {
                        output = "Set Global Variable " + ServerVariableBase.GetName(command.Ints[1]) + " (";
                    }
                    else
                    {
                        return "Invalid Command";
                    }
                    switch (command.Ints[2])
                    {
                        case 0:
                            output += "Set to " + command.Ints[3];
                            break;
                        case 1:
                            output += "Add " + command.Ints[3];
                            break;

                        case 2:
                            output += "Subtract " + command.Ints[3];
                            break;

                        case 3:
                            output += "Random Number " + command.Ints[3] + " to " + command.Ints[4] + "]";
                            break;
                    }
                    output += ")";
                    return output;
                case EventCommandType.SetSelfSwitch:
                    return "Set Self Switch " + (char)('A' + command.Ints[0]) + " to " + Convert.ToBoolean(command.Ints[1]);
                case EventCommandType.ConditionalBranch:
                    return "Conditional Branch: [" + GetConditionalDesc(command) + "]";
                case EventCommandType.ExitEventProcess:
                    return "Exit event processing";
                case EventCommandType.Label:
                    return "Label: " + command.Strs[0];
                case EventCommandType.GoToLabel:
                    return "Go to Label: " + command.Strs[0];
                case EventCommandType.StartCommonEvent:
                    return "Start Common Event: " + EventBase.GetName(command.Ints[0]);
                case EventCommandType.RestoreHp:
                    return "Restore Player HP";
                case EventCommandType.RestoreMp:
                    return "Restore Player MP";
                case EventCommandType.LevelUp:
                    return "Level up Player";
                case EventCommandType.GiveExperience:
                    return "Give player " + command.Ints[0] + " experience.";
                case EventCommandType.ChangeLevel:
                    return "Set player level to: " + command.Ints[0];
                case EventCommandType.ChangeSpells:
                    output = "Change Player Spells [";
                    if (command.Ints[0] == 0)
                    {
                        output += "Teach: ";
                    }
                    else
                    {
                        output += "Remove: ";
                    }
                    output += "Spell " + SpellBase.GetName(command.Ints[1]) + "]";
                    return output;
                case EventCommandType.ChangeItems:
                    output = "Change Player Items [";
                    if (command.Ints[0] == 0)
                    {
                        output += "Give: ";
                    }
                    else
                    {
                        output += "Take: ";
                    }
                    output += "Item " + ItemBase.GetName(command.Ints[1]) + "]";
                    return output;
                case EventCommandType.ChangeSprite:
                    return "Set Player Sprite to " + command.Strs[0];
                case EventCommandType.ChangeFace:
                    return "Set Player Face to " + command.Strs[0];
                case EventCommandType.ChangeGender:
                    if (command.Ints[0] == 0)
                    {
                        return "Set Player Gender to Male";
                    }
                    else
                    {
                        return "Set Player Gender to Female";
                    }
                case EventCommandType.SetAccess:
                    switch (command.Ints[0])
                    {
                        case 0:
                            return "Set Player Access to a Regular User";
                        case 1:
                            return "Set Player Access to a In-Game Moderator";
                        case 2:
                            return "Set Player Access to a Owner/Developer";
                    }
                    return "Set Player Access: Unknown Access";
                case EventCommandType.WarpPlayer:
                    output = "Warp Player [Map: ";
                    for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
                    {
                        if (MapList.GetOrderedMaps()[i].MapNum == command.Ints[0])
                        {
                            output += "#" + command.Ints[0] + " " + MapList.GetOrderedMaps()[i].Name + " X: " +
                                      command.Ints[1] + " Y: " + command.Ints[2] + " Dir: ";
                            switch (command.Ints[3])
                            {
                                case 0:
                                    return output + "Retain Direction]";
                                case 1:
                                    return output + "Up]";
                                case 2:
                                    return output + "Down]";
                                case 3:
                                    return output + "Left]";
                                case 4:
                                    return output + "Right]";
                            }
                            break;
                        }
                    }
                    return output + "NOT FOUND]";
                case EventCommandType.SetMoveRoute:
                    output += "Set Move Route for ";
                    if (MyMap.Events.ContainsKey(command.Route.Target))
                    {
                        return output + "Event #" + (command.Route.Target) + " " + MyMap.Events[command.Route.Target].MyName;
                    }
                    else
                    {
                        return output + "Deleted Event!";
                    }
                case EventCommandType.WaitForRouteCompletion:
                    output += "Wait for Move Route Completion of ";
                    if (MyMap.Events.ContainsKey(command.Ints[0]))
                    {
                        return output + "Event #" + (command.Ints[0]) + " " + MyMap.Events[command.Ints[0]].MyName;
                    }
                    else
                    {
                        return output + "Deleted Event!";
                    }
                case EventCommandType.HoldPlayer:
                    return "Hold Player";
                case EventCommandType.ReleasePlayer:
                    return "Release Player";
                case EventCommandType.SpawnNpc:
                    output += "Spawn Npc " + NpcBase.GetName(command.Ints[0]) + " ";
                    switch (command.Ints[1])
                    {
                        case 0: //On Map/Tile Selection
                            for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
                            {
                                if (MapList.GetOrderedMaps()[i].MapNum == command.Ints[2])
                                {
                                    output += "[On Map #" + command.Ints[2] + " " + MapList.GetOrderedMaps()[i].Name + " X: " +
                                              command.Ints[3] + " Y: " + command.Ints[4] + " Dir: ";
                                    switch (command.Ints[5])
                                    {
                                        case 0:
                                            return output + "Up]";
                                        case 1:
                                            return output + "Down]";
                                        case 2:
                                            return output + "Left]";
                                        case 3:
                                            return output + "Right]";
                                    }
                                    break;
                                }
                            }
                            break;
                        case 1: //On/Around Entity
                            if (command.Ints[2] == -1)
                            {
                                return output += "On Player" + " [X Offset: " + command.Ints[3] + " Y Offset: " + command.Ints[4] + " Retain Direction: " + Convert.ToBoolean(command.Ints[5]).ToString() + "]";
                            }
                            else
                            {
                                if (MyMap.Events.ContainsKey(command.Ints[2]))
                                {
                                    return output + "On Event #" + (command.Ints[2] + 1) + " " + MyMap.Events[command.Ints[2]].MyName + " [X Offset: " + command.Ints[3] + " Y Offset: " + command.Ints[4] + " Retain Direction: " + Convert.ToBoolean(command.Ints[5]).ToString() + "]";
                                }
                                else
                                {
                                    return output + "On Deleted Event!" + " [X Offset: " + command.Ints[3] + " Y Offset: " + command.Ints[4] + " Retain Direction: " + Convert.ToBoolean(command.Ints[5]).ToString() + "]";
                                }
                            }
                    }
                    return output;
                case EventCommandType.PlayAnimation:
                    output += "Play Animation " + AnimationBase.GetName(command.Ints[0]) + " ";
                    switch (command.Ints[1])
                    {
                        case 0: //On Map/Tile Selection
                            for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
                            {
                                if (MapList.GetOrderedMaps()[i].MapNum == command.Ints[2])
                                {
                                    output += "[On Map #" + command.Ints[2] + " " + MapList.GetOrderedMaps()[i].Name + " X: " +
                                              command.Ints[3] + " Y: " + command.Ints[4] + " Dir: ";
                                    switch (command.Ints[5])
                                    {
                                        case 0:
                                            return output + "Up]";
                                        case 1:
                                            return output + "Down]";
                                        case 2:
                                            return output + "Left]";
                                        case 3:
                                            return output + "Right]";
                                    }
                                    break;
                                }
                            }
                            break;
                        case 1: //On/Around Entity
                            if (command.Ints[2] == -1)
                            {
                                output += "On Player" + " [X Offset: " + command.Ints[3] + " Y Offset: " + command.Ints[4];
                            }
                            else
                            {
                                if (MyMap.Events.ContainsKey(command.Ints[2]))
                                {
                                    output += "On Event #" + (command.Ints[2] + 1) + " " + MyMap.Events[command.Ints[2]].MyName + " [X Offset: " + command.Ints[3] + " Y Offset: " + command.Ints[4];
                                }
                                else
                                {
                                    output += "On Deleted Event!" + " [X Offset: " + command.Ints[3] + " Y Offset: " + command.Ints[4];
                                }
                            }
                            switch (command.Ints[5])
                            {
                                //0 does not adhere to direction, 1 is Spawning Relative to Direction, 2 is Rotating Relative to Direction, and 3 is both.
                                case 1:
                                    output += " (Spawn Relative To Direction)";
                                    break;
                                case 2:
                                    output += " (Rotate Relative To Direction)";
                                    break;
                                case 3:
                                    output += " (Spawn and Rotate Relative To Direction)";
                                    break;
                            }
                            output += "]";
                            return output;
                    }
                    return output;
                case EventCommandType.PlayBgm:
                    return "Play BGM [File: " + command.Strs[0] + "]";
                case EventCommandType.FadeoutBgm:
                    return "Fadeout BGM";
                case EventCommandType.PlaySound:
                    return "Play Sound [File: " + command.Strs[0] + "]";
                case EventCommandType.StopSounds:
                    return "Stop Sounds";
                case EventCommandType.Wait:
                    return "Wait " + command.Ints[0] + "ms";
                case EventCommandType.OpenBank:
                    return "Open Bank";
                case EventCommandType.OpenShop:
                    return "Open Shop [" + ShopBase.GetName(command.Ints[0]) + "]";
                default:
                    return "Unknown Command";
            }
        }

        private string GetConditionalDesc(EventCommand command)
        {
            if (command.Type != EventCommandType.ConditionalBranch) return "";
            string output = "";
            switch (command.Ints[0])
            {
                case 0: //Player Switch
                    return "Player Switch " + PlayerSwitchBase.GetName(command.Ints[1]) + " is " + Convert.ToBoolean(command.Ints[2]);
                case 1: //Player Variables
                    output = "Player Variable " + PlayerVariableBase.GetName(command.Ints[1]);
                    switch (command.Ints[2])
                    {
                        case 0:
                            output += " is equal to ";
                            break;
                        case 1:
                            output += " is greater than or equal to ";
                            break;
                        case 2:
                            output += " is less than or equal to ";
                            break;
                        case 3:
                            output += " is greater than ";
                            break;
                        case 4:
                            output += " is less than ";
                            break;
                        case 5:
                            output += " does not equal ";
                            break;
                    }
                    output += command.Ints[3];
                    return output;
                case 2: //Global Switch
                    return "Global Switch " + ServerSwitchBase.GetName(command.Ints[1]) + " is " + Convert.ToBoolean(command.Ints[2]);
                case 3: //Global Variables
                    output = "Global Variable " + ServerVariableBase.GetName(command.Ints[1]);
                    switch (command.Ints[2])
                    {
                        case 0:
                            output += " is equal to ";
                            break;
                        case 1:
                            output += " is greater than or equal to ";
                            break;
                        case 2:
                            output += " is less than or equal to ";
                            break;
                        case 3:
                            output += " is greater than ";
                            break;
                        case 4:
                            output += " is less than ";
                            break;
                        case 5:
                            output += " does not equal ";
                            break;
                    }
                    output += command.Ints[3];
                    return output;
                case 4: //Has Item
                    return "Player has at least " + command.Ints[2] + " of Item " + ItemBase.GetName(command.Ints[1]);
                case 5: //Class Is
                    return "Player's class is " + ClassBase.GetName(command.Ints[1]); ;
                case 6: //Knows spell
                    return "Player knows Spell " + SpellBase.GetName(command.Ints[1]); ;
                case 7: //Level is
                    output = "Player's level";
                    switch (command.Ints[1])
                    {
                        case 0:
                            output += " is equal to ";
                            break;
                        case 1:
                            output += " is greater than or equal to ";
                            break;
                        case 2:
                            output += " is less than or equal to ";
                            break;
                        case 3:
                            output += " is greater than ";
                            break;
                        case 4:
                            output += " is less than ";
                            break;
                        case 5:
                            output += " does not equal ";
                            break;
                    }
                    output += command.Ints[2];
                    return output;
                case 8: //Self Switch
                    return "Self Switch " + (char)('A' + command.Ints[1]) + " is " + Convert.ToBoolean(command.Ints[2]);
                case 9: //Power is
                    output = "Player's Power is";
                    switch (command.Ints[1])
                    {
                        case 0:
                            output += " Mod or Admin";
                            break;
                        case 1:
                            output += " Admin";
                            break;
                    }
                    return output;
            }
            return "";
        }

        private void lstCommands_ItemActivated(object sender, EventArgs e)
        {
            if (lstCommands.SelectedItems.Count == 0)
            {
                return;
            }
            var tmpCommand = new EventCommand();
            grpNewCommands.Hide();
            tmpCommand.Type = (EventCommandType)lstCommands.SelectedItems[0].Index + 1;
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

        /// <summary>
        /// Given a new or existing command this creates the window to select to edit it's values.
        /// </summary>
        /// <param name="command">The command that will be modified.</param>
        private void OpenEditCommand(EventCommand command)
        {
            UserControl cmdWindow = null;
            switch (command.Type)
            {
                case EventCommandType.Null:
                    break;
                case EventCommandType.ShowText:
                    cmdWindow = new EventCommand_Text(command, this);
                    break;
                case EventCommandType.ShowOptions:
                    cmdWindow = new EventCommand_Options(command, CurrentPage, this);
                    break;
                case EventCommandType.AddChatboxText:
                    cmdWindow = new EventCommand_ChatboxText(command, this);
                    break;
                case EventCommandType.SetSwitch:
                    cmdWindow = new EventCommand_Switch(command, this);
                    break;
                case EventCommandType.SetVariable:
                    cmdWindow = new EventCommand_Variable(command, this);
                    break;
                case EventCommandType.SetSelfSwitch:
                    cmdWindow = new EventCommand_SelfSwitch(command, this);
                    break;
                case EventCommandType.ConditionalBranch:
                    cmdWindow = new EventCommand_ConditionalBranch(command, CurrentPage, this);
                    break;
                case EventCommandType.ExitEventProcess:
                    //No editor
                    break;
                case EventCommandType.Label:
                    cmdWindow = new EventCommand_Label(command, this);
                    break;
                case EventCommandType.GoToLabel:
                    cmdWindow = new EventCommand_GotoLabel(command, this);
                    break;
                case EventCommandType.StartCommonEvent:
                    cmdWindow = new EventCommand_StartCommonEvent(command, this);
                    break;
                case EventCommandType.RestoreHp:
                    //No editor
                    break;
                case EventCommandType.RestoreMp:
                    //No editor
                    break;
                case EventCommandType.LevelUp:
                    //No editor
                    break;
                case EventCommandType.GiveExperience:
                    cmdWindow = new EventCommand_GiveExperience(command, this);
                    break;
                case EventCommandType.ChangeLevel:
                    cmdWindow = new EventCommand_ChangeLevel(command, this);
                    break;
                case EventCommandType.ChangeSpells:
                    cmdWindow = new EventCommand_ChangeSpells(command, CurrentPage, this);
                    break;
                case EventCommandType.ChangeItems:
                    cmdWindow = new EventCommand_ChangeItems(command, CurrentPage, this);
                    break;
                case EventCommandType.ChangeSprite:
                    cmdWindow = new EventCommand_ChangeSprite(command, this);
                    break;
                case EventCommandType.ChangeFace:
                    cmdWindow = new EventCommand_ChangeFace(command, this);
                    break;
                case EventCommandType.ChangeGender:
                    cmdWindow = new EventCommand_ChangeGender(command, this);
                    break;
                case EventCommandType.SetAccess:
                    cmdWindow = new EventCommand_SetAccess(command, this);
                    break;
                case EventCommandType.WarpPlayer:
                    cmdWindow = new EventCommand_Warp(command, this);
                    break;
                case EventCommandType.SetMoveRoute:
                    if (command.Route == null)
                    {
                        command.Route = new EventMoveRoute();
                    }
                    cmdWindow = new Event_MoveRouteDesigner(this, _currentMap, MyEvent, command.Route, command);
                    break;
                case EventCommandType.WaitForRouteCompletion:
                    cmdWindow = new EventCommand_WaitForRouteCompletion(command, this, _currentMap, MyEvent);
                    break;
                case EventCommandType.HoldPlayer:
                    break;
                case EventCommandType.ReleasePlayer:
                    break;
                case EventCommandType.SpawnNpc:
                    cmdWindow = new EventCommand_SpawnNpc(this, _currentMap, MyEvent, command);
                    break;
                case EventCommandType.PlayAnimation:
                    cmdWindow = new EventCommand_PlayAnimation(this, _currentMap, MyEvent, command);
                    break;
                case EventCommandType.PlayBgm:
                    cmdWindow = new EventCommand_PlayBgm(command, this);
                    break;
                case EventCommandType.FadeoutBgm:
                    break;
                case EventCommandType.PlaySound:
                    cmdWindow = new EventCommand_PlayBgs(command, this);
                    break;
                case EventCommandType.StopSounds:
                    break;
                case EventCommandType.Wait:
                    cmdWindow = new EventCommand_Wait(command, this);
                    break;
                case EventCommandType.OpenBank:
                    break;
                case EventCommandType.OpenShop:
                    cmdWindow = new EventCommand_OpenShop(command, this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (cmdWindow != null)
            {
                if (cmdWindow.GetType() == typeof(Event_MoveRouteDesigner))
                {
                    this.Controls.Add(cmdWindow);
                    cmdWindow.Width = this.ClientSize.Width;
                    cmdWindow.Height = this.ClientSize.Height;
                    cmdWindow.BringToFront();
                    _editingCommand = command;
                }
                else
                {
                    grpCreateCommands.Show();
                    grpCreateCommands.Controls.Add(cmdWindow);
                    cmdWindow.Left = (grpCreateCommands.Width / 2) - cmdWindow.Width / 2;
                    cmdWindow.Top = (grpCreateCommands.Height / 2) - cmdWindow.Height / 2;
                    _editingCommand = command;
                    DisableButtons();
                }
            }
            else //Added a command with no editor
            {
                ListPageCommands();
            }
        }

        /// <summary>
        /// Resets the form when a user saves a command they are creating or editting.
        /// </summary>
        public void FinishCommandEdit(bool moveRoute = false)
        {
            if (!moveRoute)
            {
                grpCreateCommands.Hide();
                grpCreateCommands.Controls.RemoveAt(0);
                //Remove the only control which should be the last editing window
            }
            ListPageCommands();
            ListPageConditions();
            EnableButtons();
        }

        /// <summary>
        /// Resets the form when a user cancels editting or creating a new command for the event.
        /// </summary>
        public void CancelCommandEdit(bool moveRoute = false)
        {
            if (_currentCommand > -1 && _commandProperties[_currentCommand].MyList.Commands.Count > _currentCommand)
            {
                if (!_isEdit)
                {
                    if (_isInsert)
                    {
                        _commandProperties[_currentCommand].MyList.Commands.RemoveAt(
                            _commandProperties[_currentCommand].MyList.Commands.IndexOf(
                                _commandProperties[_currentCommand].Cmd) - 1);
                    }
                    else
                    {
                        _commandProperties[_currentCommand].MyList.Commands.RemoveAt(
                            _commandProperties[_currentCommand].MyList.Commands.Count - 1);
                    }
                }
            }
            if (!moveRoute)
            {
                grpCreateCommands.Hide();
                grpCreateCommands.Controls.RemoveAt(0);


            }
            ListPageCommands();
            EnableButtons();
        }

        /// <summary>
        /// Opens the 'Add Command' window in order to insert a command at the select location in the command list.
        /// </summary>
        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (_currentCommand <= -1) return;
            if (!_commandProperties[_currentCommand].Editable) return;
            if (_commandProperties[_currentCommand].Type == EventCommandType.Null)
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
        #endregion


        /// <summary>
        /// Takes a string and a length value. If the string is longer than the length it will cut the string and add a ..., otherwise it will return the original string.
        /// </summary>
        /// <param name="value">String to process.</param>
        /// <param name="maxChars">Max length allowed for the string.</param>
        /// <returns></returns>
        private static string Truncate(string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + " ..";
        }



        private void txtEventname_TextChanged(object sender, EventArgs e)
        {
            MyEvent.MyName = txtEventname.Text;
            this.Text = @"Event Editor - Event #" + MyEvent.MyIndex + @": " + txtEventname.Text;
        }

        #region "Movement Options"
        private void cmbMoveType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage.MovementType = cmbMoveType.SelectedIndex;
            if (CurrentPage.MovementType == 2)
            {
                btnSetRoute.Enabled = true;
            }
            else
            {
                btnSetRoute.Enabled = false;
            }
        }
        private void cmbEventSpeed_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage.MovementSpeed = cmbEventSpeed.SelectedIndex;
        }
        private void cmbEventFreq_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage.MovementFreq = cmbEventFreq.SelectedIndex;
        }
        private void btnSetRoute_Click(object sender, EventArgs e)
        {
            Event_MoveRouteDesigner moveRouteDesigner = new Event_MoveRouteDesigner(this, _currentMap, MyEvent, CurrentPage.MoveRoute);
            this.Controls.Add(moveRouteDesigner);
            moveRouteDesigner.BringToFront();
            moveRouteDesigner.Size = this.ClientSize;
        }
        #endregion

        #region "Extra Options"
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
        private void chkHideName_CheckedChanged(object sender, EventArgs e)
        {
            CurrentPage.HideName = Convert.ToInt32(chkHideName.Checked);
        }
        private void chkDirectionFix_CheckedChanged(object sender, EventArgs e)
        {
            CurrentPage.DirectionFix = Convert.ToInt32(chkDirectionFix.Checked);
        }
        private void chkWalkingAnimation_CheckedChanged(object sender, EventArgs e)
        {
            CurrentPage.WalkingAnimation = Convert.ToInt32(chkWalkingAnimation.Checked);
        }
        #endregion

        #region "Inspector Options"
        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            CurrentPage.Desc = txtDesc.Text;
        }
        private void chkDisablePreview_CheckedChanged(object sender, EventArgs e)
        {
            CurrentPage.DisablePreview = Convert.ToInt32(chkDisablePreview.Checked);
            if (chkDisablePreview.Checked)
            {
                cmbPreviewFace.Enabled = false;
                txtDesc.Enabled = false;
            }
            else
            {
                cmbPreviewFace.Enabled = true;
                txtDesc.Enabled = true;
            }
            UpdateFacePreview();
        }
        private void UpdateFacePreview()
        {
            if (CurrentPage.DisablePreview == 1 || cmbPreviewFace.SelectedIndex < 1)
            {
                pnlFacePreview.BackgroundImage = null;
                return;
            }
            if (File.Exists("resources/faces/" + cmbPreviewFace.Text))
            {
                pnlFacePreview.BackgroundImage = new Bitmap("resources/faces/" + cmbPreviewFace.Text);
            }
        }
        private void cmbPreviewFace_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage.FaceGraphic = cmbPreviewFace.Text;
            UpdateFacePreview();
        }
        #endregion

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
            btnEdit.Enabled = true;
            btnDelete.Enabled = true;
        }
        private void lstEventCommands_DoubleClick(object sender, EventArgs e)
        {
            if (_currentCommand <= -1) return;
            if (!_commandProperties[_currentCommand].Editable) return;
            if (_commandProperties[_currentCommand].Type == EventCommandType.Null)
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







        /// <summary>
        /// Opens the graphic selector window to pick the default graphic for this event page.
        /// </summary>
        private void pnlPreview_DoubleClick(object sender, EventArgs e)
        {
            Event_GraphicSelector graphicSelector = new Event_GraphicSelector(CurrentPage.Graphic, this);
            this.Controls.Add(graphicSelector);
            graphicSelector.BringToFront();
            graphicSelector.Size = this.ClientSize;
        }
        private void UpdateEventPreview()
        {
            System.Drawing.Graphics graphics;
            Bitmap sourceBitmap = null;
            Bitmap destBitmap = null;
            destBitmap = new Bitmap(pnlPreview.Width, pnlPreview.Height);
            graphics = System.Drawing.Graphics.FromImage(destBitmap);
            graphics.Clear(Color.Black);

            if (CurrentPage.Graphic.Type == 1) //Sprite
            {
                if (File.Exists("resources/entities/" + CurrentPage.Graphic.Filename))
                {
                    sourceBitmap = new Bitmap("resources/entities/" + CurrentPage.Graphic.Filename);
                }
            }
            else if (CurrentPage.Graphic.Type == 2) //Tileset
            {
                if (File.Exists("resources/tilesets/" + CurrentPage.Graphic.Filename))
                {
                    sourceBitmap = new Bitmap("resources/tilesets/" + CurrentPage.Graphic.Filename);
                }
            }
            if (sourceBitmap != null)
            {
                if (CurrentPage.Graphic.Type == 1)
                {
                    graphics.DrawImage(sourceBitmap, new Rectangle(pnlPreview.Width / 2 - (sourceBitmap.Width / 4) / 2, pnlPreview.Height / 2 - (sourceBitmap.Height / 4) / 2, sourceBitmap.Width / 4, sourceBitmap.Height / 4), new Rectangle(CurrentPage.Graphic.X * sourceBitmap.Width / 4, CurrentPage.Graphic.Y * sourceBitmap.Height / 4, sourceBitmap.Width / 4, sourceBitmap.Height / 4), GraphicsUnit.Pixel);
                }
                else if (CurrentPage.Graphic.Type == 2)
                {
                    graphics.DrawImage(sourceBitmap, new Rectangle(pnlPreview.Width / 2 - (Options.TileWidth + (CurrentPage.Graphic.Width * Options.TileWidth)) / 2, pnlPreview.Height / 2 - (Options.TileHeight + (CurrentPage.Graphic.Height * Options.TileHeight)) / 2, Options.TileWidth + (CurrentPage.Graphic.Width * Options.TileWidth), Options.TileHeight + (CurrentPage.Graphic.Height * Options.TileHeight)), new Rectangle(CurrentPage.Graphic.X * Options.TileWidth, CurrentPage.Graphic.Y * Options.TileHeight, Options.TileWidth + (CurrentPage.Graphic.Width * Options.TileWidth), Options.TileHeight + (CurrentPage.Graphic.Height * Options.TileHeight)), GraphicsUnit.Pixel);
                }
                sourceBitmap.Dispose();
            }
            graphics.Dispose();
            pnlPreview.BackgroundImage = destBitmap;
        }



        public void CloseMoveRouteDesigner(Event_MoveRouteDesigner moveRouteDesigner)
        {
            Controls.Remove(moveRouteDesigner);
        }
        public void CloseGraphicSelector(Event_GraphicSelector graphicSelector)
        {
            Controls.Remove(graphicSelector);
            UpdateEventPreview();
        }


        private void lstCommands_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void ListPageConditions()
        {
            lstConditions.Items.Clear();
            for (int i = 0; i < CurrentPage.Conditions.Count(); i++)
            {
                lstConditions.Items.Add((i + 1) + ". " + GetConditionalDesc(CurrentPage.Conditions[i]));
            }
        }

        private void btnAddCondition_Click(object sender, EventArgs e)
        {
            if (!grpCreateCommands.Visible && !grpNewCommands.Visible)
            {
                EventCommand newCondition = new EventCommand();
                newCondition.Type = EventCommandType.ConditionalBranch;
                CurrentPage.Conditions.Add(newCondition);
                OpenEditCommand(newCondition);
            }
        }

        private void btnRemoveCondition_Click(object sender, EventArgs e)
        {
            if (!grpCreateCommands.Visible)
            {
                if (lstConditions.SelectedIndex > -1)
                {
                    CurrentPage.Conditions.RemoveAt(lstConditions.SelectedIndex);
                    ListPageConditions();
                }
            }
        }

        private void btnNewPage_Click(object sender, EventArgs e)
        {
            MyEvent.MyPages.Add(new EventPage());
            UpdateTabControl();
            LoadPage(MyEvent.MyPages.Count - 1);
        }

        private void UpdateTabControl()
        {
            tabControl.TabPages.Clear();
            for (int i = 0; i < MyEvent.MyPages.Count(); i++)
            {
                tabControl.TabPages.Add((i + 1).ToString());
            }
        }

        private void EnableButtons()
        {
            //Enable Actions
            btnNewPage.Enabled = true;
            btnCopyPage.Enabled = true;
            if (_pageCopy != null)
            {
                btnPastePage.Enabled = true;
            }
            else
            {
                btnPastePage.Enabled = false;
            }
            if (MyEvent.MyPages.Count > 1)
            {
                btnDeletePage.Enabled = true;
            }
            else
            {
                btnDeletePage.Enabled = false;
            }
            btnClearPage.Enabled = true;
            btnSave.Enabled = true;
            btnCancel.Enabled = true;
        }

        private void DisableButtons()
        {
            //Disable Actions
            btnNewPage.Enabled = false;
            btnCopyPage.Enabled = false;
            btnPastePage.Enabled = false;
            btnDeletePage.Enabled = false;
            btnClearPage.Enabled = false;
            btnSave.Enabled = false;
            btnCancel.Enabled = false;
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex > -1)
            {
                if (grpCreateCommands.Visible)
                {
                    tabControl.SelectedIndex = CurrentPageIndex;
                }
                else
                {
                    LoadPage(tabControl.SelectedIndex);
                }
            }
        }

        private void FrmEvent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (grpNewCommands.Visible)
                {
                    grpNewCommands.Hide();
                }
            }
        }

        private void btnDeletePage_Click(object sender, EventArgs e)
        {
            if (MyEvent.MyPages.Count > 1)
            {
                MyEvent.MyPages.RemoveAt(CurrentPageIndex);
                UpdateTabControl();
                LoadPage(0);
            }
        }

        private void btnClearPage_Click(object sender, EventArgs e)
        {
            MyEvent.MyPages[CurrentPageIndex] = new EventPage();
            LoadPage(CurrentPageIndex);
        }

        private void btnCopyPage_Click(object sender, EventArgs e)
        {
            _pageCopy = new ByteBuffer();
            CurrentPage.WriteBytes(_pageCopy);
            EnableButtons();
        }

        private void btnPastePage_Click(object sender, EventArgs e)
        {
            if (_pageCopy != null)
            {
                _pageCopy.Readpos = 0;
                MyEvent.MyPages[CurrentPageIndex] = new EventPage(_pageCopy);
                LoadPage(CurrentPageIndex);
            }
        }

        private void cmbAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage.Animation = Database.GameObjectIdFromList(GameObject.Animation,cmbAnimation.SelectedIndex - 1);
        }

        private void chkIsGlobal_CheckedChanged(object sender, EventArgs e)
        {
            MyEvent.IsGlobal = Convert.ToByte(chkIsGlobal.Checked);
        }
    }

    public class CommandListProperties
    {
        public CommandList MyList;
        public int MyIndex;
        public bool Editable;
        public EventCommand Cmd;
        public EventCommandType Type = EventCommandType.Null;
    }
}
