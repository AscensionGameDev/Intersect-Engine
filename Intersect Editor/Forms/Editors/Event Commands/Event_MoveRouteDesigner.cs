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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Intersect_Editor.Classes;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class Event_MoveRouteDesigner : UserControl
    {
        private EventMoveRoute _editingRoute;
        private EventMoveRoute _tmpMoveRoute;
        private FrmEvent _eventEditor;
        private readonly EventStruct _editingEvent;
        private EventCommand _editingCommand;
        private MapStruct _currentMap;
        private List<int> _targetIndicies = new List<int>();
        private MoveRouteAction _lastAddedAction;
        public Event_MoveRouteDesigner(FrmEvent eventEditor, MapStruct currentMap, EventStruct currentEvent, EventMoveRoute editingRoute, EventCommand editingCommand = null)
        {
            InitializeComponent();

            //Grab event editor reference
            _eventEditor = eventEditor;
            _editingEvent = currentEvent;
            _editingCommand = editingCommand;
            _currentMap = currentMap;

            //Generate temp route to edit
            _tmpMoveRoute = new EventMoveRoute();
            _tmpMoveRoute.CopyFrom(editingRoute);
            _editingRoute = editingRoute;

            //Setup form
            chkIgnoreIfBlocked.Checked = _tmpMoveRoute.IgnoreIfBlocked;
            chkRepeatRoute.Checked = _tmpMoveRoute.RepeatRoute;

            cmbTarget.Items.Clear();

            if (!_editingEvent.CommonEvent)
            {
                for (int i = 0; i < currentMap.Events.Count; i++)
                {
                    if (i == _editingEvent.MyIndex)
                    {
                        cmbTarget.Items.Add((i + 1) + ". [THIS EVENT] " + currentMap.Events[i].MyName);
                        _targetIndicies.Add(i);
                        if (_editingCommand == null)
                        {
                            cmbTarget.SelectedIndex = cmbTarget.Items.Count-1;
                            cmbTarget.Enabled = false;
                        }
                        else
                        {
                            if (_editingRoute.Target == i || _editingRoute.Target == -1)
                            {
                                cmbTarget.SelectedIndex = cmbTarget.Items.Count - 1;
                            }
                        }
                    }
                    else
                    {
                        if (currentMap.Events[i].Deleted == 0)
                        {
                            cmbTarget.Items.Add((i + 1) + ". " + currentMap.Events[i].MyName);
                            _targetIndicies.Add(i);
                            if (_editingRoute.Target == i)
                            {
                                cmbTarget.SelectedIndex = cmbTarget.Items.Count - 1;
                            }
                        }
                    }
                }
            }

            ListMoveRoute();
        }

        private void ListMoveRoute()
        {
            lstActions.Items.Clear();
            foreach (MoveRouteAction action in _tmpMoveRoute.Actions)
            {
                switch (action.Type)
                {
                    case MoveRouteEnum.MoveUp:
                        lstActions.Items.Add("Move Up");
                        break;
                    case MoveRouteEnum.MoveDown:
                        lstActions.Items.Add("Move Down");
                        break;
                    case MoveRouteEnum.MoveLeft:
                        lstActions.Items.Add("Move Left");
                        break;
                    case MoveRouteEnum.MoveRight:
                        lstActions.Items.Add("Move Right");
                        break;
                    case MoveRouteEnum.MoveRandomly:
                        lstActions.Items.Add("Move Randomly");
                        break;
                    case MoveRouteEnum.MoveTowardsPlayer:
                        lstActions.Items.Add("Move Towards Player");
                        break;
                    case MoveRouteEnum.MoveAwayFromPlayer:
                        lstActions.Items.Add("Move Away From Player");
                        break;
                    case MoveRouteEnum.StepForward:
                        lstActions.Items.Add("Step Forward");
                        break;
                    case MoveRouteEnum.StepBack:
                        lstActions.Items.Add("Step Back");
                        break;
                    case MoveRouteEnum.FaceUp:
                        lstActions.Items.Add("Face Up");
                        break;
                    case MoveRouteEnum.FaceDown:
                        lstActions.Items.Add("Face Down");
                        break;
                    case MoveRouteEnum.FaceLeft:
                        lstActions.Items.Add("Face Left");
                        break;
                    case MoveRouteEnum.FaceRight:
                        lstActions.Items.Add("Face Right");
                        break;
                    case MoveRouteEnum.Turn90Clockwise:
                        lstActions.Items.Add("Turn 90* Clockwise");
                        break;
                    case MoveRouteEnum.Turn90CounterClockwise:
                        lstActions.Items.Add("Turn 90* Counter Clockwise");
                        break;
                    case MoveRouteEnum.Turn180:
                        lstActions.Items.Add("Turn 180*");
                        break;
                    case MoveRouteEnum.TurnRandomly:
                        lstActions.Items.Add("Turn Randomly");
                        break;
                    case MoveRouteEnum.FacePlayer:
                        lstActions.Items.Add("Face Player");
                        break;
                    case MoveRouteEnum.FaceAwayFromPlayer:
                        lstActions.Items.Add("Face Away from Player");
                        break;
                    case MoveRouteEnum.SetSpeedSlowest:
                        lstActions.Items.Add("Set Speed: Slowest");
                        break;
                    case MoveRouteEnum.SetSpeedSlower:
                        lstActions.Items.Add("Set Speed: Slower");
                        break;
                    case MoveRouteEnum.SetSpeedNormal:
                        lstActions.Items.Add("Set Speed: Normal");
                        break;
                    case MoveRouteEnum.SetSpeedFaster:
                        lstActions.Items.Add("Set Speed: Faster");
                        break;
                    case MoveRouteEnum.SetSpeedFastest:
                        lstActions.Items.Add("Set Speed: Fastest");
                        break;
                    case MoveRouteEnum.SetFreqLowest:
                        lstActions.Items.Add("Set Frequency: Lowest");
                        break;
                    case MoveRouteEnum.SetFreqLower:
                        lstActions.Items.Add("Set Frequency: Lower");
                        break;
                    case MoveRouteEnum.SetFreqNormal:
                        lstActions.Items.Add("Set Frequency: Normal");
                        break;
                    case MoveRouteEnum.SetFreqHigher:
                        lstActions.Items.Add("Set Frequency: Higher");
                        break;
                    case MoveRouteEnum.SetFreqHighest:
                        lstActions.Items.Add("Set Frequency: Highest");
                        break;
                    case MoveRouteEnum.WalkingAnimOn:
                        lstActions.Items.Add("Walking Animaton: On");
                        break;
                    case MoveRouteEnum.WalkingAnimOff:
                        lstActions.Items.Add("Walking Animaton: Off");
                        break;
                    case MoveRouteEnum.DirectionFixOn:
                        lstActions.Items.Add("Direction Fix: On");
                        break;
                    case MoveRouteEnum.DirectionFixOff:
                        lstActions.Items.Add("Direction Fix: Off");
                        break;
                    case MoveRouteEnum.WalkthroughOn:
                        lstActions.Items.Add("Walkthrough: On");
                        break;
                    case MoveRouteEnum.WalkthroughOff:
                        lstActions.Items.Add("Walkthrough: Off");
                        break;
                    case MoveRouteEnum.ShowName:
                        lstActions.Items.Add("Show Name");
                        break;
                    case MoveRouteEnum.HideName:
                        lstActions.Items.Add("Hide Name");
                        break;
                    case MoveRouteEnum.SetLevelBelow:
                        lstActions.Items.Add("Set Level: Below Player");
                        break;
                    case MoveRouteEnum.SetLevelNormal:
                        lstActions.Items.Add("Set Level: With Player");
                        break;
                    case MoveRouteEnum.SetLevelAbove:
                        lstActions.Items.Add("Set Level: Above Player");
                        break;
                    case MoveRouteEnum.Wait100:
                        lstActions.Items.Add("Wait 100ms");
                        break;
                    case MoveRouteEnum.Wait500:
                        lstActions.Items.Add("Wait 500ms");
                        break;
                    case MoveRouteEnum.Wait1000:
                        lstActions.Items.Add("Wait 1000ms");
                        break;
                    case MoveRouteEnum.SetGraphic:
                        lstActions.Items.Add("Set Graphic....");
                        break;
                    case MoveRouteEnum.SetAnimation:
                        lstActions.Items.Add("Set Animation....");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void lstCommands_ItemActivate(object sender, EventArgs e)
        {
            if (lstCommands.SelectedItems.Count == 0) { return; }

            var action = new MoveRouteAction();
            action.Type = (MoveRouteEnum)lstCommands.SelectedItems[0].Index + 1;
            if (action.Type == MoveRouteEnum.SetGraphic)
            {
                action.Graphic = new EventGraphic();
                //Open the graphic editor....
                Event_GraphicSelector graphicSelector = new Event_GraphicSelector(action.Graphic, _eventEditor, this, true);
                _eventEditor.Controls.Add(graphicSelector);
                graphicSelector.BringToFront();
                graphicSelector.Size = this.ClientSize;
            }
            else if (action.Type == MoveRouteEnum.SetAnimation)
            {
                //Open the animation selector
                Event_MoveRouteAnimationSelector animationSelector = new Event_MoveRouteAnimationSelector(this,action,true);
                Controls.Add(animationSelector);
                animationSelector.BringToFront();
                animationSelector.Size = this.ClientSize;
            }
            if (lstActions.SelectedIndex == -1)
            {
                _tmpMoveRoute.Actions.Add(action);
            }
            else
            {
                _tmpMoveRoute.Actions.Insert(lstActions.SelectedIndex, action);
            }
            _lastAddedAction = action;
            ListMoveRoute();
        }

        private void lstActions_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (lstActions.SelectedIndex > -1)
                {
                    _tmpMoveRoute.Actions.RemoveAt(lstActions.SelectedIndex);
                    ListMoveRoute();
                }
            }
        }

        private void lstActions_MouseDown(object sender, MouseEventArgs e)
        {
            if (lstActions.IndexFromPoint(new System.Drawing.Point(e.X, e.Y)) == -1)
            {
                lstActions.SelectedIndex = -1;
            }
        }

        private void lstActions_DoubleClick(object sender, EventArgs e)
        {
            if (lstActions.SelectedIndex > -1)
            {
                if (_tmpMoveRoute.Actions[lstActions.SelectedIndex].Type == MoveRouteEnum.SetGraphic)
                {
                    //Open the graphic editor....
                    Event_GraphicSelector graphicSelector = new Event_GraphicSelector(_tmpMoveRoute.Actions[lstActions.SelectedIndex].Graphic, _eventEditor,this,false);
                    _eventEditor.Controls.Add(graphicSelector);
                    graphicSelector.BringToFront();
                    graphicSelector.Size = this.ClientSize;
                }
                else if (_tmpMoveRoute.Actions[lstActions.SelectedIndex].Type == MoveRouteEnum.SetAnimation)
                {
                    //Open the animation selector
                    Event_MoveRouteAnimationSelector animationSelector = new Event_MoveRouteAnimationSelector(this, _tmpMoveRoute.Actions[lstActions.SelectedIndex], true);
                    Controls.Add(animationSelector);
                    animationSelector.BringToFront();
                    animationSelector.Size = this.ClientSize;
                }
            }
        }

        public void RemoveLastAction()
        {
            if (_tmpMoveRoute.Actions.Count > 0)
            {
                _tmpMoveRoute.Actions.Remove(_lastAddedAction);
                ListMoveRoute();
            }
        }

        private void chkIgnoreIfBlocked_CheckedChanged(object sender, EventArgs e)
        {
            _tmpMoveRoute.IgnoreIfBlocked = chkIgnoreIfBlocked.Checked;
        }

        private void chkRepeatRoute_CheckedChanged(object sender, EventArgs e)
        {
            _tmpMoveRoute.RepeatRoute = chkRepeatRoute.Checked;
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            _editingRoute.CopyFrom(_tmpMoveRoute);
            _editingRoute.Target = -1;
            if (_editingCommand != null)
            {
                if (!_editingEvent.CommonEvent)
                {
                    _editingRoute.Target = _targetIndicies[cmbTarget.SelectedIndex];
                }
                _eventEditor.FinishCommandEdit(true);
            }
            _eventEditor.CloseMoveRouteDesigner(this);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_editingCommand != null)
            {
                _eventEditor.CancelCommandEdit(true);
            }
            _eventEditor.CloseMoveRouteDesigner(this);
        }
    }
}
