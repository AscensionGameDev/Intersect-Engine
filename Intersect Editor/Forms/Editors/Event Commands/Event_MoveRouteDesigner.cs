
using System;
using System.Linq;
using System.Windows.Forms;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect.Localization;
using System.Collections.Generic;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class Event_MoveRouteDesigner : UserControl
    {
        private EventMoveRoute _editingRoute;
        private EventMoveRoute _tmpMoveRoute;
        private FrmEvent _eventEditor;
        private readonly EventBase _editingEvent;
        private EventCommand _editingCommand;
        private MapBase _currentMap;
        private MoveRouteAction _lastAddedAction;
        private List<TreeNode> _origItems = new List<TreeNode>();
        public Event_MoveRouteDesigner(FrmEvent eventEditor, MapBase currentMap, EventBase currentEvent, EventMoveRoute editingRoute, EventCommand editingCommand = null)
        {
            InitializeComponent();
            InitLocalization();

            foreach (var item in lstCommands.Nodes)
            {
                var parentNode = new TreeNode(((TreeNode)item).Text)
                {
                    Name = ((TreeNode)item).Name,
                    Tag = ((TreeNode)item).Tag
                };
                foreach (var childItem in ((TreeNode)item).Nodes)
                {
                    var childNode = new TreeNode(((TreeNode)childItem).Text)
                    {
                        Name = ((TreeNode)childItem).Name,
                        Tag = ((TreeNode)childItem).Tag
                    };
                    parentNode.Nodes.Add(childNode);
                }
                _origItems.Add(parentNode);
            }

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
                if (_editingCommand != null)
                {
                    cmbTarget.Items.Add(Strings.Get("eventmoveroute", "player"));
                    if (_editingCommand.Route.Target == -1) cmbTarget.SelectedIndex = 0;
                }
                foreach (var evt in _currentMap.Events)
                {
                    cmbTarget.Items.Add(evt.Key == _editingEvent.MyIndex ? Strings.Get("eventmoveroute", "thisevent") : "" + evt.Value.Name);
                    if (_editingCommand != null)
                    {
                        if (_editingCommand.Route.Target == evt.Key) cmbTarget.SelectedIndex = cmbTarget.Items.Count;
                    }
                    else
                    {
                        if (_editingRoute.Target == evt.Key || _editingRoute.Target == -1)
                            cmbTarget.SelectedIndex = cmbTarget.Items.Count - 1;
                    }
                }
            }
            if (cmbTarget.SelectedIndex == -1 && cmbTarget.Items.Count > 0)
            {
                cmbTarget.SelectedIndex = 0;
            }

            ListMoveRoute();
            lstCommands.ExpandAll();
        }

        private void InitLocalization()
        {
            grpMoveRoute.Text = Strings.Get("eventmoveroute", "title");
            grpCommands.Text = Strings.Get("eventmoveroute", "commands");
            chkIgnoreIfBlocked.Text = Strings.Get("eventmoveroute", "ignoreblocked");
            chkRepeatRoute.Text = Strings.Get("eventmoveroute", "repeatroute");
            btnOkay.Text = Strings.Get("eventmoveroute", "okay");
            btnCancel.Text = Strings.Get("eventmoveroute", "cancel");
            for (int i = 0; i < lstCommands.Nodes.Count; i++)
            {
                lstCommands.Nodes[i].Text = Strings.Get("eventmoveroute", lstCommands.Nodes[i].Name);
                for (int x = 0; x < lstCommands.Nodes[i].Nodes.Count; x++)
                {
                    lstCommands.Nodes[i].Nodes[x].Text = Strings.Get("eventmoveroute", lstCommands.Nodes[i].Nodes[x].Name);
                }
            }
        }

        private void ListMoveRoute()
        {
            lstActions.Items.Clear();
            foreach (MoveRouteAction action in _tmpMoveRoute.Actions)
            {
                for (int i = 0; i < lstCommands.Nodes.Count; i++)
                {
                    for (int x = 0; x < lstCommands.Nodes[i].Nodes.Count; x++)
                    {
                        if (Convert.ToInt32(lstCommands.Nodes[i].Nodes[x].Tag) == (int)action.Type)
                        {
                            lstActions.Items.Add(Strings.Get("eventmoveroute", lstCommands.Nodes[i].Nodes[x].Name));
                        }
                    }
                }
            }
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
                    Event_GraphicSelector graphicSelector = new Event_GraphicSelector(_tmpMoveRoute.Actions[lstActions.SelectedIndex].Graphic, _eventEditor, this, false);
                    _eventEditor.Controls.Add(graphicSelector);
                    graphicSelector.BringToFront();
                    graphicSelector.Size = ClientSize;
                }
                else if (_tmpMoveRoute.Actions[lstActions.SelectedIndex].Type == MoveRouteEnum.SetAnimation)
                {
                    //Open the animation selector
                    Event_MoveRouteAnimationSelector animationSelector = new Event_MoveRouteAnimationSelector(this, _tmpMoveRoute.Actions[lstActions.SelectedIndex], true);
                    Controls.Add(animationSelector);
                    animationSelector.BringToFront();
                    animationSelector.Size = ClientSize;
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
                    if (cmbTarget.SelectedIndex == 0)
                    {
                        _editingRoute.Target = -1;
                    }
                    else
                    {
                        _editingRoute.Target = _currentMap.Events.Keys.ToList()[cmbTarget.SelectedIndex - 1];
                    }
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

        private void lstCommands_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lstCommands_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag == null)
            {
                return;
            }
            var action = new MoveRouteAction()
            {
                Type = (MoveRouteEnum)Convert.ToInt32(e.Node.Tag)
            };
            if (action.Type == MoveRouteEnum.SetGraphic)
            {
                action.Graphic = new EventGraphic();
                //Open the graphic editor....
                Event_GraphicSelector graphicSelector = new Event_GraphicSelector(action.Graphic, _eventEditor, this, true);
                _eventEditor.Controls.Add(graphicSelector);
                graphicSelector.BringToFront();
                graphicSelector.Size = ClientSize;
            }
            else if (action.Type == MoveRouteEnum.SetAnimation)
            {
                //Open the animation selector
                Event_MoveRouteAnimationSelector animationSelector = new Event_MoveRouteAnimationSelector(this, action, true);
                Controls.Add(animationSelector);
                animationSelector.BringToFront();
                animationSelector.Size = ClientSize;
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

        private void cmbTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstCommands.Nodes.Clear();
            foreach (var item in _origItems)
            {
                var parentNode = new TreeNode(((TreeNode)item).Text)
                {
                    Name = ((TreeNode)item).Name,
                    Tag = ((TreeNode)item).Tag
                };
                foreach (var childItem in item.Nodes)
                {
                    var childNode = new TreeNode(((TreeNode)childItem).Text)
                    {
                        Name = ((TreeNode)childItem).Name,
                        Tag = ((TreeNode)childItem).Tag
                    };
                    parentNode.Nodes.Add(childNode);
                }
                lstCommands.Nodes.Add(parentNode);
            }
            lstCommands.ExpandAll();
            if (!_editingEvent.CommonEvent && _editingCommand != null && cmbTarget.SelectedIndex == 0)
            {
                var hideNodes = new string[]{ "movetowardplayer", "moveawayfromplayer", "turntowardplayer", "turnawayfromplayer", "setspeed", "setmovementfrequency", "setattribute", "setgraphic", "setanimation" };
                var nodesToRemove = new List<TreeNode>();
                foreach (var parentNode in lstCommands.Nodes)
                {
                    if (parentNode != null)
                    {
                        if (hideNodes.Contains(((TreeNode)parentNode).Name))
                        {
                            nodesToRemove.Add(((TreeNode)parentNode));
                        }
                        foreach (var childNode in ((TreeNode)parentNode).Nodes)
                        {
                            if (childNode != null)
                            {
                                if (hideNodes.Contains(((TreeNode)childNode).Name))
                                {
                                    nodesToRemove.Add((TreeNode)childNode);
                                }
                            }
                        }
                    }
                }
                foreach (var node in nodesToRemove)
                {
                    node.Remove();
                }
            }
        }
    }
}
