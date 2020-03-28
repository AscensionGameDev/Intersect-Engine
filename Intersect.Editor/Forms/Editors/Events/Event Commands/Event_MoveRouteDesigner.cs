using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;
using Intersect.GameObjects.Maps;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventMoveRouteDesigner : UserControl
    {

        private readonly EventBase mEditingEvent;

        private MapBase mCurrentMap;

        private SetMoveRouteCommand mEditingCommand;

        private EventMoveRoute mEditingRoute;

        private FrmEvent mEventEditor;

        private MoveRouteAction mLastAddedAction;

        private List<TreeNode> mOrigItems = new List<TreeNode>();

        private EventMoveRoute mTmpMoveRoute;

        public EventMoveRouteDesigner(
            FrmEvent eventEditor,
            MapBase currentMap,
            EventBase currentEvent,
            EventMoveRoute editingRoute,
            SetMoveRouteCommand editingCommand = null
        )
        {
            InitializeComponent();
            InitLocalization();

            foreach (var item in lstCommands.Nodes)
            {
                var parentNode = new TreeNode(((TreeNode) item).Text)
                {
                    Name = ((TreeNode) item).Name,
                    Tag = ((TreeNode) item).Tag
                };

                foreach (var childItem in ((TreeNode) item).Nodes)
                {
                    var childNode = new TreeNode(((TreeNode) childItem).Text)
                    {
                        Name = ((TreeNode) childItem).Name,
                        Tag = ((TreeNode) childItem).Tag
                    };

                    parentNode.Nodes.Add(childNode);
                }

                mOrigItems.Add(parentNode);
            }

            //Grab event editor reference
            mEventEditor = eventEditor;
            mEditingEvent = currentEvent;
            mEditingCommand = editingCommand;
            mCurrentMap = currentMap;

            //Generate temp route to edit
            mTmpMoveRoute = new EventMoveRoute();
            mTmpMoveRoute.CopyFrom(editingRoute);
            mEditingRoute = editingRoute;

            //Setup form
            chkIgnoreIfBlocked.Checked = mTmpMoveRoute.IgnoreIfBlocked;
            chkRepeatRoute.Checked = mTmpMoveRoute.RepeatRoute;

            cmbTarget.Items.Clear();
            if (!mEditingEvent.CommonEvent)
            {
                if (mEditingCommand != null)
                {
                    cmbTarget.Items.Add(Strings.EventMoveRoute.player);
                    if (mEditingCommand.Route.Target == Guid.Empty)
                    {
                        cmbTarget.SelectedIndex = 0;
                    }
                }

                foreach (var evt in mCurrentMap.LocalEvents)
                {
                    cmbTarget.Items.Add(
                        evt.Key == mEditingEvent.Id ? Strings.EventMoveRoute.thisevent.ToString() : "" + evt.Value.Name
                    );

                    if (mEditingCommand != null)
                    {
                        if (mEditingCommand.Route.Target == evt.Key)
                        {
                            cmbTarget.SelectedIndex = cmbTarget.Items.Count - 1;
                        }
                    }
                    else
                    {
                        if (mEditingRoute.Target == evt.Key || mEditingRoute.Target == Guid.Empty)
                        {
                            cmbTarget.SelectedIndex = cmbTarget.Items.Count - 1;
                        }
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
            grpMoveRoute.Text = Strings.EventMoveRoute.title;
            grpCommands.Text = Strings.EventMoveRoute.command;
            chkIgnoreIfBlocked.Text = Strings.EventMoveRoute.ignoreblocked;
            chkRepeatRoute.Text = Strings.EventMoveRoute.repeatroute;
            btnOkay.Text = Strings.EventMoveRoute.okay;
            btnCancel.Text = Strings.EventMoveRoute.cancel;
            for (var i = 0; i < lstCommands.Nodes.Count; i++)
            {
                lstCommands.Nodes[i].Text = Strings.EventMoveRoute.commands[lstCommands.Nodes[i].Name];
                for (var x = 0; x < lstCommands.Nodes[i].Nodes.Count; x++)
                {
                    lstCommands.Nodes[i].Nodes[x].Text =
                        Strings.EventMoveRoute.commands[lstCommands.Nodes[i].Nodes[x].Name];
                }
            }
        }

        private void ListMoveRoute()
        {
            lstActions.Items.Clear();
            foreach (var action in mTmpMoveRoute.Actions)
            {
                for (var i = 0; i < lstCommands.Nodes.Count; i++)
                {
                    for (var x = 0; x < lstCommands.Nodes[i].Nodes.Count; x++)
                    {
                        if (Convert.ToInt32(lstCommands.Nodes[i].Nodes[x].Tag) == (int) action.Type)
                        {
                            lstActions.Items.Add(Strings.EventMoveRoute.commands[lstCommands.Nodes[i].Nodes[x].Name]);
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
                    mTmpMoveRoute.Actions.RemoveAt(lstActions.SelectedIndex);
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
                if (mTmpMoveRoute.Actions[lstActions.SelectedIndex].Type == MoveRouteEnum.SetGraphic)
                {
                    //Open the graphic editor....
                    var graphicSelector = new EventGraphicSelector(
                        mTmpMoveRoute.Actions[lstActions.SelectedIndex].Graphic, mEventEditor, this, false
                    );

                    mEventEditor.Controls.Add(graphicSelector);
                    graphicSelector.BringToFront();
                    graphicSelector.Size = ClientSize;
                }
                else if (mTmpMoveRoute.Actions[lstActions.SelectedIndex].Type == MoveRouteEnum.SetAnimation)
                {
                    //Open the animation selector
                    var animationSelector = new EventMoveRouteAnimationSelector(
                        this, mTmpMoveRoute.Actions[lstActions.SelectedIndex], false
                    );

                    Controls.Add(animationSelector);
                    animationSelector.BringToFront();
                    animationSelector.Size = ClientSize;
                }
            }
        }

        public void RemoveLastAction()
        {
            if (mTmpMoveRoute.Actions.Count > 0)
            {
                mTmpMoveRoute.Actions.Remove(mLastAddedAction);
                ListMoveRoute();
            }
        }

        private void chkIgnoreIfBlocked_CheckedChanged(object sender, EventArgs e)
        {
            mTmpMoveRoute.IgnoreIfBlocked = chkIgnoreIfBlocked.Checked;
        }

        private void chkRepeatRoute_CheckedChanged(object sender, EventArgs e)
        {
            mTmpMoveRoute.RepeatRoute = chkRepeatRoute.Checked;
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            mEditingRoute.CopyFrom(mTmpMoveRoute);
            mEditingRoute.Target = Guid.Empty;
            if (mEditingCommand != null)
            {
                if (!mEditingEvent.CommonEvent)
                {
                    if (cmbTarget.SelectedIndex == 0)
                    {
                        mEditingRoute.Target = Guid.Empty;
                    }
                    else
                    {
                        mEditingRoute.Target = mCurrentMap.LocalEvents.Keys.ToList()[cmbTarget.SelectedIndex - 1];
                    }
                }

                mEventEditor.FinishCommandEdit(true);
            }

            mEventEditor.CloseMoveRouteDesigner(this);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (mEditingCommand != null)
            {
                mEventEditor.CancelCommandEdit(true);
            }

            mEventEditor.CloseMoveRouteDesigner(this);
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
                Type = (MoveRouteEnum) Convert.ToInt32(e.Node.Tag)
            };

            if (action.Type == MoveRouteEnum.SetGraphic)
            {
                action.Graphic = new EventGraphic();

                //Open the graphic editor....
                var graphicSelector = new EventGraphicSelector(action.Graphic, mEventEditor, this, true);
                mEventEditor.Controls.Add(graphicSelector);
                graphicSelector.BringToFront();
                graphicSelector.Size = ClientSize;
            }
            else if (action.Type == MoveRouteEnum.SetAnimation)
            {
                //Open the animation selector
                var animationSelector = new EventMoveRouteAnimationSelector(this, action, true);
                Controls.Add(animationSelector);
                animationSelector.BringToFront();
                animationSelector.Size = ClientSize;
            }

            if (lstActions.SelectedIndex == -1)
            {
                mTmpMoveRoute.Actions.Add(action);
            }
            else
            {
                mTmpMoveRoute.Actions.Insert(lstActions.SelectedIndex, action);
            }

            mLastAddedAction = action;
            ListMoveRoute();
        }

        private void cmbTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstCommands.Nodes.Clear();
            foreach (var item in mOrigItems)
            {
                var parentNode = new TreeNode(((TreeNode) item).Text)
                {
                    Name = ((TreeNode) item).Name,
                    Tag = ((TreeNode) item).Tag
                };

                foreach (var childItem in item.Nodes)
                {
                    var childNode = new TreeNode(((TreeNode) childItem).Text)
                    {
                        Name = ((TreeNode) childItem).Name,
                        Tag = ((TreeNode) childItem).Tag
                    };

                    parentNode.Nodes.Add(childNode);
                }

                lstCommands.Nodes.Add(parentNode);
            }

            lstCommands.ExpandAll();
            if (!mEditingEvent.CommonEvent && mEditingCommand != null && cmbTarget.SelectedIndex == 0)
            {
                var hideNodes = new string[]
                {
                    "movetowardplayer", "moveawayfromplayer", "turntowardplayer", "turnawayfromplayer", "setspeed",
                    "setmovementfrequency", "setattribute", "setgraphic", "setanimation"
                };

                var nodesToRemove = new List<TreeNode>();
                foreach (var parentNode in lstCommands.Nodes)
                {
                    if (parentNode != null)
                    {
                        if (hideNodes.Contains(((TreeNode) parentNode).Name))
                        {
                            nodesToRemove.Add((TreeNode) parentNode);
                        }

                        foreach (var childNode in ((TreeNode) parentNode).Nodes)
                        {
                            if (childNode != null)
                            {
                                if (hideNodes.Contains(((TreeNode) childNode).Name))
                                {
                                    nodesToRemove.Add((TreeNode) childNode);
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
