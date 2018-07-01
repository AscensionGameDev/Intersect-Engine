using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DarkUI.Controls;
using DarkUI.Forms;
using Intersect.Editor.ContentManagement;
using Intersect.Editor.Core;
using Intersect.Editor.Forms.Editors.Event_Commands;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Utilities;

namespace Intersect.Editor.Forms.Editors
{
    public partial class FrmEvent : Form
    {
        private readonly List<CommandListProperties> mCommandProperties = new List<CommandListProperties>();
        private readonly MapBase mCurrentMap;
        private int mCurrentCommand = -1;
        private EventCommand mEditingCommand;
        private string mEventBackup = null;
        private bool mIsEdit;
        private bool mIsInsert;
        private ByteBuffer mPageCopy;
        private List<DarkButton> mPageTabs = new List<DarkButton>();
        public EventPage CurrentPage;
        public int CurrentPageIndex;
        public EventBase MyEvent;
        public MapBase MyMap;
        public bool NewEvent;

        /// <summary>
        ///     Takes a string and a length value. If the string is longer than the length it will cut the string and add a ...,
        ///     otherwise it will return the original string.
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
            MyEvent.Name = txtEventname.Text;
            Text = Strings.EventEditor.title.ToString( MyEvent.Index, txtEventname.Text);
        }

        private void lstEventCommands_SelectedIndexChanged(object sender, EventArgs e)
        {
            mCurrentCommand = lstEventCommands.SelectedIndex;
        }

        private void lstEventCommands_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Delete) return;
            if (mCurrentCommand <= -1) return;
            if (!mCommandProperties[mCurrentCommand].Editable) return;
            mCommandProperties[mCurrentCommand].MyList.Commands.Remove(mCommandProperties[mCurrentCommand].Cmd);
            ListPageCommands();
        }

        private void lstEventCommands_Click(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            var i = lstEventCommands.IndexFromPoint(e.Location);
            if (i <= -1 || i >= lstEventCommands.Items.Count) return;
            if (!mCommandProperties[i].Editable) return;
            lstEventCommands.SelectedIndex = i;

            if (mCurrentCommand < 0 || mCurrentCommand >= mCommandProperties.Count) return;

            commandMenu.Show((ListBox) sender, e.Location);
            btnEdit.Enabled = true;
            if (!mCommandProperties[mCurrentCommand].Editable) btnEdit.Enabled = false;
            if (mCommandProperties[mCurrentCommand].MyIndex < 0 || mCommandProperties[mCurrentCommand].MyIndex >= mCommandProperties[mCurrentCommand].MyList.Commands.Count) btnEdit.Enabled = false;
            btnDelete.Enabled = true;
        }

        private void lstEventCommands_DoubleClick(object sender, EventArgs e)
        {
            if (mCurrentCommand <= -1) return;
            if (!mCommandProperties[mCurrentCommand].Editable) return;
            if (mCommandProperties[mCurrentCommand].Type == EventCommandType.Null)
            {
                grpNewCommands.Show();
                DisableButtons();
                mIsInsert = false;
            }
            else
            {
                grpNewCommands.Show();
                DisableButtons();
                mIsInsert = true;
            }
        }

        /// <summary>
        ///     Opens the graphic selector window to pick the default graphic for this event page.
        /// </summary>
        private void pnlPreview_DoubleClick(object sender, EventArgs e)
        {
            EventGraphicSelector graphicSelector = new EventGraphicSelector(CurrentPage.Graphic, this);
            Controls.Add(graphicSelector);
            graphicSelector.BringToFront();
            graphicSelector.Size = ClientSize;
        }

        private void UpdateEventPreview()
        {
            Graphics graphics;
            Bitmap sourceBitmap = null;
            Bitmap destBitmap = null;
            destBitmap = new Bitmap(pnlPreview.Width, pnlPreview.Height);
            graphics = Graphics.FromImage(destBitmap);
            graphics.Clear(System.Drawing.Color.FromArgb(60, 63, 65));

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
                    graphics.DrawImage(sourceBitmap,
                        new Rectangle(pnlPreview.Width / 2 - (sourceBitmap.Width / 4) / 2,
                            pnlPreview.Height / 2 - (sourceBitmap.Height / 4) / 2, sourceBitmap.Width / 4,
                            sourceBitmap.Height / 4),
                        new Rectangle(CurrentPage.Graphic.X * sourceBitmap.Width / 4,
                            CurrentPage.Graphic.Y * sourceBitmap.Height / 4, sourceBitmap.Width / 4,
                            sourceBitmap.Height / 4), GraphicsUnit.Pixel);
                }
                else if (CurrentPage.Graphic.Type == 2)
                {
                    graphics.DrawImage(sourceBitmap,
                        new Rectangle(
                            pnlPreview.Width / 2 -
                            (Options.TileWidth + (CurrentPage.Graphic.Width * Options.TileWidth)) / 2,
                            pnlPreview.Height / 2 -
                            (Options.TileHeight + (CurrentPage.Graphic.Height * Options.TileHeight)) / 2,
                            Options.TileWidth + (CurrentPage.Graphic.Width * Options.TileWidth),
                            Options.TileHeight + (CurrentPage.Graphic.Height * Options.TileHeight)),
                        new Rectangle(CurrentPage.Graphic.X * Options.TileWidth,
                            CurrentPage.Graphic.Y * Options.TileHeight,
                            Options.TileWidth + (CurrentPage.Graphic.Width * Options.TileWidth),
                            Options.TileHeight + (CurrentPage.Graphic.Height * Options.TileHeight)),
                        GraphicsUnit.Pixel);
                }
                sourceBitmap.Dispose();
            }
            graphics.Dispose();
            pnlPreview.BackgroundImage = destBitmap;
        }

        public void CloseMoveRouteDesigner(EventMoveRouteDesigner moveRouteDesigner)
        {
            Controls.Remove(moveRouteDesigner);
        }

        public void CloseGraphicSelector(EventGraphicSelector graphicSelector)
        {
            Controls.Remove(graphicSelector);
            UpdateEventPreview();
        }

        private void btnNewPage_Click(object sender, EventArgs e)
        {
            MyEvent.Pages.Add(new EventPage());
            UpdateTabControl();
            LoadPage(MyEvent.Pages.Count - 1);
        }

        private void UpdateTabControl()
        {
            foreach (var page in mPageTabs)
            {
                pnlTabs.Controls.Remove(page);
            }
            mPageTabs.Clear();
            for (int i = 0; i < MyEvent.Pages.Count; i++)
            {
                var btn = new DarkButton()
                {
                    Text = (i + 1).ToString()
                };
                btn.Click += TabBtn_Click;
                mPageTabs.Add(btn);
            }
            pnlTabs.Controls.AddRange(mPageTabs.ToArray());
            for (int i = 0; i < MyEvent.Pages.Count; i++)
            {
                var btn = mPageTabs[i];
                btn.Size = new Size(0, 0);
                btn.MaximumSize = new Size(100, 22);
                btn.AutoSize = true;
                if (i > 0)
                {
                    btn.Left = mPageTabs[i - 1].Right - 1;
                }
                btn.Top = 0;
                btn.SendToBack();
            }
            pnlTabs.SetBounds(0, 0, pnlTabs.Width, pnlTabs.Height);
            btnTabsRight.Visible = pnlTabs.Right > pnlTabsContainer.Width;
            btnTabsLeft.Visible = pnlTabs.Left < 0;
        }

        private void TabBtn_Click(object sender, EventArgs e)
        {
            LoadPage(mPageTabs.IndexOf((DarkButton) sender));
        }

        private void EnableButtons()
        {
            //Enable Actions
            btnNewPage.Enabled = true;
            btnCopyPage.Enabled = true;
            if (mPageCopy != null)
            {
                btnPastePage.Enabled = true;
            }
            else
            {
                btnPastePage.Enabled = false;
            }
            if (MyEvent.Pages.Count > 1)
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

        private void FrmEvent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (grpNewCommands.Visible)
                {
                    grpNewCommands.Hide();
                    EnableButtons();
                }
            }
        }

        private void btnDeletePage_Click(object sender, EventArgs e)
        {
            if (MyEvent.Pages.Count > 1)
            {
                MyEvent.Pages.RemoveAt(CurrentPageIndex);
                UpdateTabControl();
                LoadPage(0);
            }
        }

        private void btnClearPage_Click(object sender, EventArgs e)
        {
            MyEvent.Pages[CurrentPageIndex] = new EventPage();
            LoadPage(CurrentPageIndex);
        }

        private void btnCopyPage_Click(object sender, EventArgs e)
        {
            mPageCopy = new ByteBuffer();
            CurrentPage.WriteBytes(mPageCopy);
            EnableButtons();
        }

        private void btnPastePage_Click(object sender, EventArgs e)
        {
            if (mPageCopy != null)
            {
                mPageCopy.Readpos = 0;
                MyEvent.Pages[CurrentPageIndex] = new EventPage(mPageCopy);
                LoadPage(CurrentPageIndex);
            }
        }

        private void cmbAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage.Animation =
                Database.GameObjectIdFromList(GameObjectType.Animation, cmbAnimation.SelectedIndex - 1);
        }

        private void chkIsGlobal_CheckedChanged(object sender, EventArgs e)
        {
            MyEvent.IsGlobal = Convert.ToByte(chkIsGlobal.Checked);
        }

        private void lblCloseCommands_Click(object sender, EventArgs e)
        {
            if (grpNewCommands.Visible)
            {
                grpNewCommands.Hide();
                EnableButtons();
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
        }

        private void btnTabsRight_Click(object sender, EventArgs e)
        {
            if (pnlTabs.Right > pnlTabsContainer.Width)
            {
                pnlTabs.SetBounds(pnlTabs.Bounds.Left - 200, 0, pnlTabs.Width,
                    pnlTabs.Height);
                if (pnlTabs.Right < pnlTabsContainer.Width)
                {
                    pnlTabs.SetBounds(pnlTabs.Bounds.Left + pnlTabsContainer.Width - pnlTabs.Right, 0, pnlTabs.Width,
                        pnlTabs.Height);
                }
            }
            btnTabsRight.Visible = pnlTabs.Right > pnlTabsContainer.Width;
            btnTabsLeft.Visible = pnlTabs.Left < 0;
        }

        private void btnTabsLeft_Click(object sender, EventArgs e)
        {
            if (pnlTabs.Left < 0)
            {
                pnlTabs.SetBounds(pnlTabs.Bounds.Left + 200, 0, pnlTabs.Width, pnlTabs.Height);
                if (pnlTabs.Left > 0)
                {
                    pnlTabs.SetBounds(0, 0, pnlTabs.Width,
                        pnlTabs.Height);
                }
            }
            btnTabsRight.Visible = pnlTabs.Right > pnlTabsContainer.Width;
            btnTabsLeft.Visible = pnlTabs.Left < 0;
        }

        private void lstCommands_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag == null)
            {
                return;
            }
            var tmpCommand = new EventCommand();
            grpNewCommands.Hide();
            tmpCommand.Type = (EventCommandType) int.Parse(e.Node.Tag.ToString());
            if (tmpCommand.Type == EventCommandType.SetSwitch || tmpCommand.Type == EventCommandType.SetSelfSwitch)
            {
                tmpCommand.Ints[2] = 1;
            }
            if ((tmpCommand.Type == EventCommandType.SetMoveRoute ||
                 tmpCommand.Type == EventCommandType.WaitForRouteCompletion) && MyEvent.CommonEvent)
            {
                DarkMessageBox.ShowWarning(Strings.EventCommandList.notcommon,
                    Strings.EventCommandList.notcommoncaption, DarkDialogButton.Ok,
                    Properties.Resources.Icon);
                EnableButtons();
                return;
            }
            if (tmpCommand.Type == EventCommandType.WaitForRouteCompletion)
            {
                tmpCommand.Ints[0] = -1;
            }
            if (mIsInsert)
            {
                mCommandProperties[mCurrentCommand].MyList.Commands.Insert(
                    mCommandProperties[mCurrentCommand].MyList.Commands
                        .IndexOf(mCommandProperties[mCurrentCommand].Cmd),
                    tmpCommand);
            }
            else
            {
                mCommandProperties[mCurrentCommand].MyList.Commands.Add(tmpCommand);
            }
            OpenEditCommand(tmpCommand);
            mIsEdit = false;
        }

        private void btnEditConditions_Click(object sender, EventArgs e)
        {
            var editForm = new FrmDynamicRequirements(CurrentPage.ConditionLists, RequirementType.Event);
            editForm.ShowDialog();
        }

        private void txtCommand_TextChanged(object sender, EventArgs e)
        {
            CurrentPage.TriggerCommand = txtCommand.Text;
        }

        #region "Form Events"

        public FrmEvent(MapBase currentMap)
        {
            InitializeComponent();
            mCurrentMap = currentMap;
        }

        private void frmEvent_Load(object sender, EventArgs e)
        {
            grpNewCommands.BringToFront();
            grpCreateCommands.BringToFront();
            InitLocalization();
            lstCommands.ExpandAll();
        }

        /// <summary>
        ///     Intercepts the form closing event to ask the user if they want to save their changes or not.
        /// </summary>
        private void frmEvent_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing) return;
            if (btnSave.Enabled == false)
            {
                e.Cancel = true;
                return;
            }
            if (
                DarkMessageBox.ShowWarning(Strings.EventEditor.savedialogue,
                    Strings.EventEditor.savecaption, DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                DialogResult.Yes)
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
        }

        private void FrmEvent_VisibleChanged(object sender, EventArgs e)
        {
            //btnCancel_Click(null, null);
        }

        private void InitLocalization()
        {
            grpGeneral.Text = Strings.EventEditor.general;
            lblName.Text = Strings.EventEditor.name;
            chkIsGlobal.Text = Strings.EventEditor.global;

            grpPageOptions.Text = Strings.EventEditor.pageoptions;
            btnNewPage.Text = Strings.EventEditor.newpage;
            btnCopyPage.Text = Strings.EventEditor.copypage;
            btnPastePage.Text = Strings.EventEditor.pastepage;
            btnDeletePage.Text = Strings.EventEditor.deletepage;
            btnClearPage.Text = Strings.EventEditor.clearpage;

            grpEventConditions.Text = Strings.EventEditor.conditions;
            btnEditConditions.Text = Strings.EventEditor.editconditions;

            grpEntityOptions.Text = Strings.EventEditor.entityoptions;
            grpPreview.Text = Strings.EventEditor.eventpreview;
            lblAnimation.Text = Strings.EventEditor.animation;

            grpMovement.Text = Strings.EventEditor.movement;
            lblType.Text = Strings.EventEditor.movementtype;
            cmbMoveType.Items.Clear();
            for (int i = 0; i < Strings.EventEditor.movementtypes.Length; i++)
            {
                cmbMoveType.Items.Add(Strings.EventEditor.movementtypes[i]);
            }
            btnSetRoute.Text = Strings.EventEditor.setroute;
            lblSpeed.Text = Strings.EventEditor.speed;
            cmbEventSpeed.Items.Clear();
            for (int i = 0; i < Strings.EventEditor.speeds.Length; i++)
            {
                cmbEventSpeed.Items.Add(Strings.EventEditor.speeds[i]);
            }
            lblFreq.Text = Strings.EventEditor.frequency;
            cmbEventFreq.Items.Clear();
            for (int i = 0; i < Strings.EventEditor.frequencies.Length; i++)
            {
                cmbEventFreq.Items.Add(Strings.EventEditor.frequencies[i]);
            }
            lblLayer.Text = Strings.EventEditor.layer;
            cmbLayering.Items.Clear();
            for (int i = 0; i < Strings.EventEditor.layers.Length; i++)
            {
                cmbLayering.Items.Add(Strings.EventEditor.layers[i]);
            }
            grpInspector.Text = Strings.EventEditor.inspector;
            chkDisableInspector.Text = Strings.EventEditor.disableinspector;
            lblInspectorDesc.Text = Strings.EventEditor.inspectordesc;
            lblFace.Text = Strings.EventEditor.face;
            grpExtra.Text = Strings.EventEditor.extras;
            chkWalkThrough.Text = Strings.EventEditor.passable;
            chkHideName.Text = Strings.EventEditor.hidename;
            chkDirectionFix.Text = Strings.EventEditor.directionfix;
            chkWalkingAnimation.Text = Strings.EventEditor.walkinganim;
            chkInteractionFreeze.Text = Strings.EventEditor.interactionfreeze;
            grpTriggers.Text = Strings.EventEditor.trigger;
            grpNewCommands.Text = Strings.EventEditor.addcommand;
            grpEventCommands.Text = Strings.EventEditor.commandlist;
            btnInsert.Text = Strings.EventEditor.insertcommand;
            btnEdit.Text = Strings.EventEditor.editcommand;
            btnDelete.Text = Strings.EventEditor.deletecommand;
            btnSave.Text = Strings.EventEditor.save;
            btnCancel.Text = Strings.EventEditor.cancel;

            for (int i = 0; i < lstCommands.Nodes.Count; i++)
            {
                lstCommands.Nodes[i].Text = Strings.EventCommands.commands[lstCommands.Nodes[i].Name];
                for (int x = 0; x < lstCommands.Nodes[i].Nodes.Count; x++)
                {
                    lstCommands.Nodes[i].Nodes[x].Text =  Strings.EventCommands.commands[lstCommands.Nodes[i].Nodes[x].Name];
                }
            }
        }

        #endregion

        #region "Loading/Initialization/Saving"

        /// <summary>
        ///     This function creates a backup of the event we are editting just in case we canel our revisions.
        ///     It also populates General lists in our editor (ie. switches/variables) for event spawning conditions.
        ///     If the event is a common event (not a map entity) we hide the entity Options on the form.
        /// </summary>
        public void InitEditor()
        {
            mEventBackup = MyEvent.JsonData;
            txtEventname.Text = MyEvent.Name;
            if (MyEvent.Id != Guid.Empty)
            {
                txtEventname.Enabled = false;
                grpTriggers.Hide();
            }
            cmbPreviewFace.Items.Clear();
            cmbPreviewFace.Items.Add(Strings.General.none);
            cmbPreviewFace.Items.AddRange(GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Face));
            cmbAnimation.Items.Clear();
            cmbAnimation.Items.Add(Strings.General.none);
            cmbAnimation.Items.AddRange(Database.GetGameObjectList(GameObjectType.Animation));
            if (MyEvent.CommonEvent)
            {
                grpEntityOptions.Hide();
                cmbTrigger.Items.Clear();
                for (int i = 0; i < Strings.EventEditor.commontriggers.Length; i++)
                {
                    cmbTrigger.Items.Add(Strings.EventEditor.commontriggers[i]);
                }
            }
            else
            {
                cmbTrigger.Items.Clear();
                for (int i = 0; i < Strings.EventEditor.triggers.Length; i++)
                {
                    cmbTrigger.Items.Add(Strings.EventEditor.triggers[i]);
                }
                cmbTriggerVal.Items.Clear();
                cmbTriggerVal.Items.Add(Strings.General.none);
                cmbTriggerVal.Items.AddRange(Database.GetGameObjectList(GameObjectType.Projectile));
            }
            chkIsGlobal.Checked = Convert.ToBoolean(MyEvent.IsGlobal);
            if (MyEvent.CommonEvent) chkIsGlobal.Hide();
            UpdateTabControl();
            LoadPage(0);
        }

        /// <summary>
        ///     Initializes all of the form controls with values from the passed event page.
        /// </summary>
        /// <param name="pageNum">The index of the page to load.</param>
        public void LoadPage(int pageNum)
        {
            Text = Strings.EventEditor.title.ToString( MyEvent.Index, txtEventname.Text);
            CurrentPageIndex = pageNum;
            if (MyEvent.Pages.Count == 0) MyEvent.Pages.Add(new EventPage());
            CurrentPage = MyEvent.Pages[pageNum];
            for (int i = 0; i < mPageTabs.Count; i++)
            {
                if (i == CurrentPageIndex)
                {
                    mPageTabs[i].BackColor = System.Drawing.Color.FromArgb(90, 90, 90);
                }
                else
                {
                    mPageTabs[i].BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
                }
            }
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
            cmbTriggerVal.Hide();
            lblTriggerVal.Hide();
            if (!MyEvent.CommonEvent)
            {
                if (cmbTrigger.SelectedIndex == (int) EventPage.EventTriggers.ProjectileHit)
                {
                    lblTriggerVal.Show();
                    lblTriggerVal.Text = Strings.EventEditor.projectile;
                    cmbTriggerVal.Show();
                    cmbTriggerVal.SelectedIndex =
                        Database.GameObjectListIndex(GameObjectType.Projectile, CurrentPage.TriggerVal) + 1;
                }
            }
            else
            {
                if (cmbTrigger.SelectedIndex == (int) EventPage.CommonEventTriggers.Command)
                {
                    txtCommand.Show();
                    txtCommand.Text = CurrentPage.TriggerCommand;
                    lblCommand.Show();
                    lblCommand.Text = Strings.EventEditor.command;
                }
            }
            cmbPreviewFace.SelectedIndex = cmbPreviewFace.Items.IndexOf(TextUtils.NullToNone(CurrentPage.FaceGraphic));
            if (cmbPreviewFace.SelectedIndex == -1)
            {
                cmbPreviewFace.SelectedIndex = 0;
                UpdateFacePreview();
            }
            cmbAnimation.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Animation, CurrentPage.Animation) +
                                         1;
            chkHideName.Checked = Convert.ToBoolean(CurrentPage.HideName);
            chkDisableInspector.Checked = Convert.ToBoolean(CurrentPage.DisablePreview);
            chkDirectionFix.Checked = Convert.ToBoolean(CurrentPage.DirectionFix);
            chkWalkingAnimation.Checked = Convert.ToBoolean(CurrentPage.WalkingAnimation);
            chkInteractionFreeze.Checked = Convert.ToBoolean(CurrentPage.InteractionFreeze);
            txtDesc.Text = CurrentPage.Desc;
            ListPageCommands();
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
                MyMap.LocalEvents.Remove(MyEvent.Id);
            }
            else
            {
                MyEvent.Load(mEventBackup);
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
            if (MyEvent.CommonEvent && MyEvent.Id != Guid.Empty)
            {
                PacketSender.SendSaveObject(MyEvent);
            }
            Hide();
            Dispose();
        }

        #endregion

        #region "CommandList Events/Functions"

        /// <summary>
        ///     Clears the listbox that displays event commands and re-populates it.
        /// </summary>
        private void ListPageCommands()
        {
            lstEventCommands.Items.Clear();
            mCommandProperties.Clear();
            PrintCommandList(CurrentPage.CommandLists[0], " ");
        }

        /// <summary>
        ///     Recursively prints the referenced command list and all of it's children.
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
                            lstEventCommands.Items.Add(indent + Strings.EventCommandList.linestart +
                                                       GetCommandText(commandList.Commands[i]));
                            clp = new CommandListProperties
                            {
                                Editable = true,
                                MyIndex = i,
                                MyList = commandList,
                                Cmd = commandList.Commands[i],
                                Type = commandList.Commands[i].Type
                            };
                            mCommandProperties.Add(clp);
                            for (var x = 1; x < 5; x++)
                            {
                                if (commandList.Commands[i].Strs[x].Trim().Length <= 0) continue;
                                lstEventCommands.Items.Add(indent + "      : " +
                                                           Strings.EventCommandList.whenoption.ToString(
                                                               Truncate(commandList.Commands[i].Strs[x], 20)));
                                clp = new CommandListProperties
                                {
                                    Editable = false,
                                    MyIndex = i,
                                    MyList = commandList,
                                    Type = commandList.Commands[i].Type,
                                    Cmd = commandList.Commands[i]
                                };
                                mCommandProperties.Add(clp);
                                PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[x - 1]],
                                    indent + "          ");
                            }
                            lstEventCommands.Items.Add(indent + "      : " +
                                                       Strings.EventCommandList.endoptions);
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };

                            mCommandProperties.Add(clp);
                            break;
                        case EventCommandType.ConditionalBranch:
                            lstEventCommands.Items.Add(indent + Strings.EventCommandList.linestart +
                                                       GetCommandText(commandList.Commands[i]));
                            clp = new CommandListProperties
                            {
                                Editable = true,
                                MyIndex = i,
                                MyList = commandList,
                                Cmd = commandList.Commands[i],
                                Type = commandList.Commands[i].Type
                            };
                            mCommandProperties.Add(clp);

                            PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[4]],
                                indent + "          ");

                            lstEventCommands.Items.Add(indent + "      : " +
                                                       Strings.EventCommandList.conditionalelse);
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };
                            mCommandProperties.Add(clp);

                            PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[5]],
                                indent + "          ");

                            lstEventCommands.Items.Add(indent + "      : " +
                                                       Strings.EventCommandList.conditionalend);
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };

                            mCommandProperties.Add(clp);
                            break;
                        case EventCommandType.ChangeSpells:
                            lstEventCommands.Items.Add(indent + Strings.EventCommandList.linestart +
                                                       GetCommandText(commandList.Commands[i]));
                            clp = new CommandListProperties
                            {
                                Editable = true,
                                MyIndex = i,
                                MyList = commandList,
                                Cmd = commandList.Commands[i],
                                Type = commandList.Commands[i].Type
                            };
                            mCommandProperties.Add(clp);

                            //When the spell was successfully taught:
                            lstEventCommands.Items.Add(indent + "      : " +
                                                       Strings.EventCommandList.spellsucceeded);
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };
                            mCommandProperties.Add(clp);
                            PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[4]],
                                indent + "          ");

                            //When the spell failed to be taught:
                            lstEventCommands.Items.Add(indent + "      : " +
                                                       Strings.EventCommandList.spellfailed);
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };
                            mCommandProperties.Add(clp);
                            PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[5]],
                                indent + "          ");

                            lstEventCommands.Items.Add(
                                indent + "      : " + Strings.EventCommandList.endspell);
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };

                            mCommandProperties.Add(clp);
                            break;
                        case EventCommandType.ChangeItems:
                            lstEventCommands.Items.Add(indent + Strings.EventCommandList.linestart +
                                                       GetCommandText(commandList.Commands[i]));
                            clp = new CommandListProperties
                            {
                                Editable = true,
                                MyIndex = i,
                                MyList = commandList,
                                Cmd = commandList.Commands[i],
                                Type = commandList.Commands[i].Type
                            };
                            mCommandProperties.Add(clp);

                            //When the item(s) were successfully given/taken:
                            lstEventCommands.Items.Add(indent + "      : " +
                                                       Strings.EventCommandList.itemschanged);
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };
                            mCommandProperties.Add(clp);
                            PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[4]],
                                indent + "          ");

                            //When the items failed to be given/taken:
                            lstEventCommands.Items.Add(indent + "      : " +
                                                       Strings.EventCommandList.itemnotchanged);
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };
                            mCommandProperties.Add(clp);
                            PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[5]],
                                indent + "          ");

                            lstEventCommands.Items.Add(indent + "      : " +
                                                       Strings.EventCommandList.enditemchange);
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };

                            mCommandProperties.Add(clp);
                            break;

                        case EventCommandType.StartQuest:
                            lstEventCommands.Items.Add(indent + Strings.EventCommandList.linestart +
                                                       GetCommandText(commandList.Commands[i]));
                            clp = new CommandListProperties
                            {
                                Editable = true,
                                MyIndex = i,
                                MyList = commandList,
                                Cmd = commandList.Commands[i],
                                Type = commandList.Commands[i].Type
                            };
                            mCommandProperties.Add(clp);

                            //When the quest is accepted/started successfully:
                            lstEventCommands.Items.Add(indent + "      : " +
                                                       Strings.EventCommandList.queststarted);
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };
                            mCommandProperties.Add(clp);
                            PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[4]],
                                indent + "          ");

                            //When the quest was declined or requirements not met:
                            lstEventCommands.Items.Add(indent + "      : " +
                                                       Strings.EventCommandList.questnotstarted);
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };
                            mCommandProperties.Add(clp);
                            PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[5]],
                                indent + "          ");

                            lstEventCommands.Items.Add(indent + "      : " +
                                                       Strings.EventCommandList.endstartquest);
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };

                            mCommandProperties.Add(clp);
                            break;
                        default:
                            lstEventCommands.Items.Add(indent + Strings.EventCommandList.linestart +
                                                       GetCommandText(commandList.Commands[i]));
                            clp = new CommandListProperties
                            {
                                Editable = true,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };
                            mCommandProperties.Add(clp);
                            break;
                    }
                }
            }
            lstEventCommands.Items.Add(indent + Strings.EventCommandList.linestart);
            clp = new CommandListProperties {Editable = true, MyIndex = -1, MyList = commandList};
            mCommandProperties.Add(clp);
        }

        /// <summary>
        ///     Given a command, this returns the string that should be displayed for it on the form's command list.
        /// </summary>
        /// <param name="command">The command to generate a summary for.</param>
        /// <returns></returns>
        private string GetCommandText(EventCommand command)
        {
            string output = "";
            switch (command.Type)
            {
                case EventCommandType.ShowText:
                    return Strings.EventCommandList.showtext.ToString( Truncate(command.Strs[0], 30));
                case EventCommandType.ShowOptions:
                    return Strings.EventCommandList.showoptions.ToString( Truncate(command.Strs[0], 30));
                case EventCommandType.AddChatboxText:
                    var channel = "";
                    switch (command.Ints[0])
                    {
                        case 0:
                            channel += Strings.EventCommandList.chatplayer;
                            break;
                        case 1:
                            channel += Strings.EventCommandList.chatlocal;
                            break;
                        case 2:
                            channel += Strings.EventCommandList.chatglobal;
                            break;
                    }
                    return Strings.EventCommandList.chatboxtext.ToString( channel, command.Strs[1],
                        Truncate(command.Strs[0], 20));
                case EventCommandType.SetSwitch:
                    var value = "";
                    value = Strings.EventCommandList.False;
                    if (Convert.ToBoolean(command.Ints[2]))
                    {
                        value = Strings.EventCommandList.True;
                    }
                    if (command.Ints[0] == (int) SwitchVariableTypes.PlayerSwitch)
                    {
                        return Strings.EventCommandList.playerswitch.ToString(
                            PlayerSwitchBase.GetName(command.Ints[1]),
                            value);
                    }
                    else if (command.Ints[0] == (int) SwitchVariableTypes.ServerSwitch)
                    {
                        return Strings.EventCommandList.globalswitch.ToString(
                            ServerSwitchBase.GetName(command.Ints[1]),
                            value);
                    }
                    else
                    {
                        return Strings.EventCommandList.invalid;
                    }
                case EventCommandType.SetVariable:
                    var varvalue = "";
                    switch (command.Ints[2])
                    {
                        case 0:
                            varvalue = Strings.EventCommandList.setvariable.ToString( command.Ints[3]);
                            break;
                        case 1:
                            varvalue = Strings.EventCommandList.addvariable.ToString( command.Ints[3]);
                            break;

                        case 2:
                            varvalue = Strings.EventCommandList.subtractvariable.ToString( command.Ints[3]);
                            break;

                        case 3:
                            varvalue = Strings.EventCommandList.randvariable.ToString( command.Ints[3],
                                command.Ints[4]);
                            break;
                    }
                    if (command.Ints[0] == (int) SwitchVariableTypes.PlayerVariable)
                    {
                        return Strings.EventCommandList.playervariable.ToString(
                            PlayerVariableBase.GetName(command.Ints[1]), varvalue);
                    }
                    else if (command.Ints[0] == (int) SwitchVariableTypes.ServerVariable)
                    {
                        return Strings.EventCommandList.globalvariable.ToString(
                            ServerVariableBase.GetName(command.Ints[1]), varvalue);
                    }
                    else
                    {
                        return Strings.EventCommandList.invalid;
                    }
                case EventCommandType.SetSelfSwitch:
                    var selfvalue = "";
                    selfvalue = Strings.EventCommandList.False;
                    if (Convert.ToBoolean(command.Ints[1]))
                    {
                        selfvalue = Strings.EventCommandList.True;
                    }
                    return Strings.EventCommandList.selfswitch.ToString(Strings.EventCommandList.selfswitches[command.Ints[0]], selfvalue);
                case EventCommandType.ConditionalBranch:
                    return Strings.EventCommandList.conditionalbranch.ToString(Strings.GetEventConditionalDesc(command));
                case EventCommandType.ExitEventProcess:
                    return Strings.EventCommandList.exitevent;
                case EventCommandType.Label:
                    return Strings.EventCommandList.label.ToString( command.Strs[0]);
                case EventCommandType.GoToLabel:
                    return Strings.EventCommandList.gotolabel.ToString( command.Strs[0]);
                case EventCommandType.StartCommonEvent:
                    return Strings.EventCommandList.commonevent.ToString( EventBase.GetName(command.Ints[0]));
                case EventCommandType.RestoreHp:
                    return Strings.EventCommandList.restorehp;
                case EventCommandType.RestoreMp:
                    return Strings.EventCommandList.restoremp;
                case EventCommandType.LevelUp:
                    return Strings.EventCommandList.levelup;
                case EventCommandType.GiveExperience:
                    return Strings.EventCommandList.giveexp.ToString( command.Ints[0]);
                case EventCommandType.ChangeLevel:
                    return Strings.EventCommandList.setlevel.ToString( command.Ints[0]);
                case EventCommandType.ChangeSpells:
                    if (command.Ints[0] == 0)
                    {
                        return Strings.EventCommandList.changespells.ToString(
                            Strings.EventCommandList.teach.ToString( SpellBase.GetName(command.Ints[1])));
                    }
                    else
                    {
                        return Strings.EventCommandList.changespells.ToString(
                            Strings.EventCommandList.forget.ToString( SpellBase.GetName(command.Ints[1])));
                    }
                case EventCommandType.ChangeItems:
                    if (command.Ints[0] == 0)
                    {
                        return Strings.EventCommandList.changeitems.ToString(
                            Strings.EventCommandList.give.ToString( ItemBase.GetName(command.Ints[1])));
                    }
                    else
                    {
                        return Strings.EventCommandList.changeitems.ToString(
                            Strings.EventCommandList.take.ToString( ItemBase.GetName(command.Ints[1])));
                    }
                case EventCommandType.ChangeSprite:
                    return Strings.EventCommandList.setsprite.ToString( command.Strs[0]);
                case EventCommandType.ChangeFace:
                    return Strings.EventCommandList.setface.ToString( command.Strs[0]);
                case EventCommandType.ChangeGender:
                    if (command.Ints[0] == 0)
                    {
                        return Strings.EventCommandList.setgender.ToString( Strings.EventCommandList.male);
                    }
                    else
                    {
                        return Strings.EventCommandList.setgender.ToString( Strings.EventCommandList.female);
                    }
                case EventCommandType.SetAccess:
                    switch (command.Ints[0])
                    {
                        case 0:
                            return Strings.EventCommandList.setaccess.ToString(
                                Strings.EventCommandList.regularuser);
                        case 1:
                            return Strings.EventCommandList.setaccess.ToString(
                                Strings.EventCommandList.moderator);
                        case 2:
                            return Strings.EventCommandList.setaccess.ToString(
                                Strings.EventCommandList.admin);
                    }
                    return Strings.EventCommandList.setaccess.ToString(
                        Strings.EventCommandList.unknownrole);
                case EventCommandType.WarpPlayer:
                    var mapName = Strings.EventCommandList.mapnotfound;
                    for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
                    {
                        if (MapList.GetOrderedMaps()[i].MapNum == command.Ints[0])
                        {
                            mapName = MapList.GetOrderedMaps()[i].Name;
                        }
                    }
                    return Strings.EventCommandList.warp.ToString( mapName, command.Ints[1], command.Ints[2], Strings.Directions.dir[command.Ints[3] - 1]);
                case EventCommandType.SetMoveRoute:
                    if (command.Route.Target == Guid.Empty)
                    {
                        return Strings.EventCommandList.moveroute.ToString(
                            Strings.EventCommandList.moverouteplayer);
                    }
                    else
                    {
                        if (MyMap.LocalEvents.ContainsKey(command.Route.Target))
                        {
                            return Strings.EventCommandList.moveroute.ToString(
                                Strings.EventCommandList.moverouteevent.ToString( (command.Route.Target),
                                    MyMap.LocalEvents[command.Route.Target].Name));
                        }
                        else
                        {
                            return Strings.EventCommandList.moveroute.ToString(
                                Strings.EventCommandList.deletedevent);
                        }
                    }
                case EventCommandType.WaitForRouteCompletion:
                    if (command.Ints[0] == -1)
                    {
                        return Strings.EventCommandList.waitforroute.ToString(
                            Strings.EventCommandList.moverouteplayer);
                    }
                    else if (MyMap.LocalEvents.ContainsKey(command.Guids[0]))
                    {
                        return Strings.EventCommandList.waitforroute.ToString(
                            Strings.EventCommandList.moverouteevent.ToString( (command.Ints[0]),
                                MyMap.LocalEvents[command.Guids[0]].Name));
                    }
                    else
                    {
                        return Strings.EventCommandList.waitforroute.ToString(
                            Strings.EventCommandList.deletedevent);
                    }
                case EventCommandType.HoldPlayer:
                    return Strings.EventCommandList.holdplayer;
                case EventCommandType.ReleasePlayer:
                    return Strings.EventCommandList.releaseplayer;
                case EventCommandType.SpawnNpc:
                    switch (command.Ints[1])
                    {
                        case 0: //On Map/Tile Selection
                            for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
                            {
                                if (MapList.GetOrderedMaps()[i].MapNum == command.Ints[2])
                                {
                                    return Strings.EventCommandList.spawnnpc.ToString( NpcBase.GetName(command.Ints[0]),
                                        Strings.EventCommandList.spawnonmap.ToString( MapList.GetOrderedMaps()[i].Name,
                                            command.Ints[3], command.Ints[4],
                                            Strings.Directions.dir[command.Ints[5]]));
                                }
                            }
                            return Strings.EventCommandList.spawnnpc.ToString( NpcBase.GetName(command.Ints[0]),
                                Strings.EventCommandList.spawnonmap.ToString(
                                    Strings.EventCommandList.mapnotfound, command.Ints[3], command.Ints[4],
                                    Strings.Directions.dir[command.Ints[5]]));
                        case 1: //On/Around Entity
                            var retain = Strings.EventCommandList.False;
                            if (Convert.ToBoolean(command.Ints[5])) retain = Strings.EventCommandList.True;
                            if (command.Ints[2] == -1)
                            {
                                return Strings.EventCommandList.spawnnpc.ToString( NpcBase.GetName(command.Ints[0]),
                                    Strings.EventCommandList.spawnonplayer.ToString( command.Ints[3], command.Ints[4],
                                        retain));
                            }
                            else
                            {
                                if (MyMap.LocalEvents.ContainsKey(command.Guids[0]))
                                {
                                    return Strings.EventCommandList.spawnnpc.ToString( NpcBase.GetName(command.Ints[0]),
                                        Strings.EventCommandList.spawnonevent.ToString( command.Ints[2],
                                            MyMap.LocalEvents[command.Guids[0]].Name, command.Ints[3], command.Ints[4],
                                            retain));
                                }
                                else
                                {
                                    return Strings.EventCommandList.spawnnpc.ToString( NpcBase.GetName(command.Ints[0]),
                                        Strings.EventCommandList.spawnonevent.ToString( command.Ints[2],
                                            Strings.EventCommandList.deletedevent, command.Ints[3],
                                            command.Ints[4], retain));
                                }
                            }
                    }
                    return output;
                case EventCommandType.DespawnNpc:
                    return Strings.EventCommandList.despawnnpcs;
                case EventCommandType.PlayAnimation:
                    switch (command.Ints[1])
                    {
                        case 0: //On Map/Tile Selection
                            for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
                            {
                                if (MapList.GetOrderedMaps()[i].MapNum == command.Ints[2])
                                {
                                    return Strings.EventCommandList.playanimation.ToString(
                                        AnimationBase.GetName(command.Ints[0]),
                                        Strings.EventCommandList.animationonmap.ToString(
                                            MapList.GetOrderedMaps()[i].Name, command.Ints[3], command.Ints[4],
                                            Strings.Directions.dir[command.Ints[5]]));
                                }
                            }
                            return Strings.EventCommandList.playanimation.ToString(
                                AnimationBase.GetName(command.Ints[0]),
                                Strings.EventCommandList.animationonmap.ToString(
                                    Strings.EventCommandList.mapnotfound, command.Ints[3], command.Ints[4],
                                    Strings.Directions.dir[command.Ints[5]]));
                        case 1: //On/Around Entity
                            var spawnOpt = "";
                            switch (command.Ints[5])
                            {
                                //0 does not adhere to direction, 1 is Spawning Relative to Direction, 2 is Rotating Relative to Direction, and 3 is both.
                                case 1:
                                    spawnOpt = Strings.EventCommandList.animationrelativedir;
                                    break;
                                case 2:
                                    spawnOpt = Strings.EventCommandList.animationrotatedir;
                                    break;
                                case 3:
                                    spawnOpt = Strings.EventCommandList.animationrelativerotate;
                                    break;
                            }
                            if (command.Ints[2] == -1)
                            {
                                return Strings.EventCommandList.playanimation.ToString(
                                    AnimationBase.GetName(command.Ints[0]),
                                    Strings.EventCommandList.animationonplayer.ToString( command.Ints[3],
                                        command.Ints[4], spawnOpt));
                            }
                            else
                            {
                                if (MyMap.LocalEvents.ContainsKey(command.Guids[0]))
                                {
                                    return Strings.EventCommandList.playanimation.ToString(
                                        AnimationBase.GetName(command.Ints[0]),
                                        Strings.EventCommandList.animationonevent.ToString( (command.Ints[2]),
                                            MyMap.LocalEvents[command.Guids[0]].Name, command.Ints[3],
                                            command.Ints[4], spawnOpt));
                                }
                                else
                                {
                                    return Strings.EventCommandList.playanimation.ToString(
                                        AnimationBase.GetName(command.Ints[0]),
                                        Strings.EventCommandList.animationonevent.ToString( (command.Ints[2]),
                                            Strings.EventCommandList.deletedevent, command.Ints[3],
                                            command.Ints[4], spawnOpt));
                                }
                            }
                    }
                    return output;
                case EventCommandType.PlayBgm:
                    return Strings.EventCommandList.playbgm.ToString( command.Strs[0]);
                case EventCommandType.FadeoutBgm:
                    return Strings.EventCommandList.fadeoutbgm;
                case EventCommandType.PlaySound:
                    return Strings.EventCommandList.playsound.ToString( command.Strs[0]);
                case EventCommandType.StopSounds:
                    return Strings.EventCommandList.stopsounds;
                case EventCommandType.ShowPicture:
                    return Strings.EventCommandList.showpicture;
                case EventCommandType.HidePicture:
                    return Strings.EventCommandList.hidepicture;
                case EventCommandType.Wait:
                    return Strings.EventCommandList.wait.ToString( command.Ints[0]);
                case EventCommandType.OpenBank:
                    return Strings.EventCommandList.openbank;
                case EventCommandType.OpenShop:
                    return Strings.EventCommandList.openshop.ToString( ShopBase.GetName(command.Ints[0]));
                case EventCommandType.OpenCraftingTable:
                    return Strings.EventCommandList.opencrafting.ToString( CraftingTableBase.GetName(command.Ints[0]));
                case EventCommandType.SetClass:
                    return Strings.EventCommandList.setclass.ToString( ClassBase.GetName(command.Ints[0]));
                case EventCommandType.StartQuest:
                    if (command.Ints[1] == 0)
                    {
                        return Strings.EventCommandList.startquest.ToString( QuestBase.GetName(command.Ints[0]),
                            Strings.EventCommandList.forcedstart);
                    }
                    else
                    {
                        return Strings.EventCommandList.startquest.ToString( QuestBase.GetName(command.Ints[0]),
                            Strings.EventCommandList.showoffer);
                    }
                case EventCommandType.CompleteQuestTask:
                    var quest = QuestBase.Lookup.Get<QuestBase>(command.Ints[0]);
                    if (quest != null)
                    {
                        //Try to find task
                        foreach (var task in quest.Tasks)
                        {
                            if (task.Id == command.Ints[1])
                            {
                                return Strings.EventCommandList.completetask.ToString(
                                    QuestBase.GetName(command.Ints[0]), task.GetTaskString(Strings.TaskEditor.descriptions));
                            }
                        }
                    }
                    return Strings.EventCommandList.completetask.ToString( QuestBase.GetName(command.Ints[0]),
                        Strings.EventCommandList.taskundefined);
                case EventCommandType.EndQuest:
                    if (command.Ints[1] == 0)
                    {
                        return Strings.EventCommandList.endquest.ToString( QuestBase.GetName(command.Ints[0]),
                            Strings.EventCommandList.runcompletionevent);
                    }
                    return Strings.EventCommandList.endquest.ToString( QuestBase.GetName(command.Ints[0]),
                        Strings.EventCommandList.skipcompletionevent);
                default:
                    return Strings.EventCommandList.unknown;
            }
        }

        private void lstCommands_ItemActivated(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     Given a new or existing command this creates the window to select to edit it's values.
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
                    cmdWindow = new EventCommandText(command, this);
                    break;
                case EventCommandType.ShowOptions:
                    cmdWindow = new EventCommandOptions(command, CurrentPage, this);
                    break;
                case EventCommandType.AddChatboxText:
                    cmdWindow = new EventCommandChatboxText(command, this);
                    break;
                case EventCommandType.SetSwitch:
                    cmdWindow = new EventCommandSwitch(command, this);
                    break;
                case EventCommandType.SetVariable:
                    cmdWindow = new EventCommandVariable(command, this);
                    break;
                case EventCommandType.SetSelfSwitch:
                    cmdWindow = new EventCommandSelfSwitch(command, this);
                    break;
                case EventCommandType.ConditionalBranch:
                    cmdWindow = new EventCommandConditionalBranch(command, CurrentPage, this);
                    break;
                case EventCommandType.ExitEventProcess:
                    //No editor
                    break;
                case EventCommandType.Label:
                    cmdWindow = new EventCommandLabel(command, this);
                    break;
                case EventCommandType.GoToLabel:
                    cmdWindow = new EventCommandGotoLabel(command, this);
                    break;
                case EventCommandType.StartCommonEvent:
                    cmdWindow = new EventCommandStartCommonEvent(command, this);
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
                    cmdWindow = new EventCommandGiveExperience(command, this);
                    break;
                case EventCommandType.ChangeLevel:
                    cmdWindow = new EventCommandChangeLevel(command, this);
                    break;
                case EventCommandType.ChangeSpells:
                    cmdWindow = new EventCommandChangeSpells(command, CurrentPage, this);
                    break;
                case EventCommandType.ChangeItems:
                    cmdWindow = new EventCommandChangeItems(command, CurrentPage, this);
                    break;
                case EventCommandType.ChangeSprite:
                    cmdWindow = new EventCommandChangeSprite(command, this);
                    break;
                case EventCommandType.ChangeFace:
                    cmdWindow = new EventCommandChangeFace(command, this);
                    break;
                case EventCommandType.ChangeGender:
                    cmdWindow = new EventCommandChangeGender(command, this);
                    break;
                case EventCommandType.SetAccess:
                    cmdWindow = new EventCommandSetAccess(command, this);
                    break;
                case EventCommandType.WarpPlayer:
                    cmdWindow = new EventCommandWarp(command, this);
                    break;
                case EventCommandType.SetMoveRoute:
                    if (command.Route == null)
                    {
                        command.Route = new EventMoveRoute();
                    }
                    cmdWindow = new EventMoveRouteDesigner(this, mCurrentMap, MyEvent, command.Route, command);
                    break;
                case EventCommandType.WaitForRouteCompletion:
                    cmdWindow = new EventCommandWaitForRouteCompletion(command, this, mCurrentMap, MyEvent);
                    break;
                case EventCommandType.HoldPlayer:
                    break;
                case EventCommandType.ReleasePlayer:
                    break;
                case EventCommandType.SpawnNpc:
                    cmdWindow = new EventCommandSpawnNpc(this, mCurrentMap, MyEvent, command);
                    break;
                case EventCommandType.DespawnNpc:
                    break;
                case EventCommandType.PlayAnimation:
                    cmdWindow = new EventCommandPlayAnimation(this, mCurrentMap, MyEvent, command);
                    break;
                case EventCommandType.PlayBgm:
                    cmdWindow = new EventCommandPlayBgm(command, this);
                    break;
                case EventCommandType.FadeoutBgm:
                    break;
                case EventCommandType.PlaySound:
                    cmdWindow = new EventCommandPlayBgs(command, this);
                    break;
                case EventCommandType.StopSounds:
                    break;
                case EventCommandType.ShowPicture:
                    cmdWindow = new EventCommand_ShowPicture(command, this);
                    break;
                case EventCommandType.HidePicture:
                    break;
                case EventCommandType.Wait:
                    cmdWindow = new EventCommandWait(command, this);
                    break;
                case EventCommandType.OpenBank:
                    break;
                case EventCommandType.OpenShop:
                    cmdWindow = new EventCommandOpenShop(command, this);
                    break;
                case EventCommandType.OpenCraftingTable:
                    cmdWindow = new EventCommandOpenCraftingTable(command, this);
                    break;
                case EventCommandType.SetClass:
                    cmdWindow = new EventCommandSetClass(command, this);
                    break;
                case EventCommandType.StartQuest:
                    cmdWindow = new EventCommandStartQuest(command, CurrentPage, this);
                    break;
                case EventCommandType.CompleteQuestTask:
                    cmdWindow = new EventCommandCompleteQuestTask(command, this);
                    break;
                case EventCommandType.EndQuest:
                    cmdWindow = new EventCommandEndQuest(command, this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (cmdWindow != null)
            {
                if (cmdWindow.GetType() == typeof(EventMoveRouteDesigner))
                {
                    Controls.Add(cmdWindow);
                    cmdWindow.Width = ClientSize.Width;
                    cmdWindow.Height = ClientSize.Height;
                    cmdWindow.BringToFront();
                    mEditingCommand = command;
                }
                else
                {
                    grpCreateCommands.Show();
                    grpCreateCommands.Controls.Add(cmdWindow);
                    cmdWindow.Left = (grpCreateCommands.Width / 2) - cmdWindow.Width / 2;
                    cmdWindow.Top = (grpCreateCommands.Height / 2) - cmdWindow.Height / 2;
                    mEditingCommand = command;
                    DisableButtons();
                }
            }
            else //Added a command with no editor
            {
                ListPageCommands();
                EnableButtons();
            }
        }

        /// <summary>
        ///     Resets the form when a user saves a command they are creating or editting.
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
            EnableButtons();
        }

        /// <summary>
        ///     Resets the form when a user cancels editting or creating a new command for the event.
        /// </summary>
        public void CancelCommandEdit(bool moveRoute = false, bool condition = false)
        {
            if (mCurrentCommand > -1 && mCommandProperties.Count > mCurrentCommand && !condition)
            {
                if (!mIsEdit)
                {
                    if (mIsInsert)
                    {
                        mCommandProperties[mCurrentCommand].MyList.Commands.RemoveAt(
                            mCommandProperties[mCurrentCommand].MyList.Commands.IndexOf(
                                mCommandProperties[mCurrentCommand].Cmd) - 1);
                    }
                    else
                    {
                        mCommandProperties[mCurrentCommand].MyList.Commands.RemoveAt(
                            mCommandProperties[mCurrentCommand].MyList.Commands.Count - 1);
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
        ///     Opens the 'Add Command' window in order to insert a command at the select location in the command list.
        /// </summary>
        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (mCurrentCommand < 0 || mCurrentCommand >= mCommandProperties.Count) return;
            if (!mCommandProperties[mCurrentCommand].Editable) return;
            if (mCommandProperties[mCurrentCommand].Type == EventCommandType.Null)
            {
                grpNewCommands.Show();
                mIsInsert = false;
                mIsEdit = false;
            }
            else
            {
                grpNewCommands.Show();
                mIsInsert = true;
                mIsEdit = false;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (mCurrentCommand < 0 || mCurrentCommand >= mCommandProperties.Count) return;
            if (!mCommandProperties[mCurrentCommand].Editable) return;
            if (mCommandProperties[mCurrentCommand].MyIndex < 0 || mCommandProperties[mCurrentCommand].MyIndex >= mCommandProperties[mCurrentCommand].MyList.Commands.Count) return;
            OpenEditCommand(mCommandProperties[mCurrentCommand].MyList.Commands[mCommandProperties[mCurrentCommand].MyIndex]);
            mIsEdit = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (mCurrentCommand < 0 || mCurrentCommand >= mCommandProperties.Count) return;
            if (!mCommandProperties[mCurrentCommand].Editable) return;
            if (mCommandProperties[mCurrentCommand].MyIndex < 0 || mCommandProperties[mCurrentCommand].MyIndex >= mCommandProperties[mCurrentCommand].MyList.Commands.Count) return;
            mCommandProperties[mCurrentCommand].MyList.Commands.Remove(mCommandProperties[mCurrentCommand].Cmd);
            ListPageCommands();
        }

        #endregion

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
            EventMoveRouteDesigner moveRouteDesigner = new EventMoveRouteDesigner(this, mCurrentMap, MyEvent,
                CurrentPage.MoveRoute);
            Controls.Add(moveRouteDesigner);
            moveRouteDesigner.BringToFront();
            moveRouteDesigner.Size = ClientSize;
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
            cmbTriggerVal.Hide();
            lblTriggerVal.Hide();
            txtCommand.Hide();
            lblCommand.Hide();

            if (!MyEvent.CommonEvent)
            {
                if (cmbTrigger.SelectedIndex == (int) EventPage.EventTriggers.ProjectileHit)
                {
                    cmbTriggerVal.Show();
                    lblTriggerVal.Show();
                    lblTriggerVal.Text = Strings.EventEditor.projectile;
                    cmbTriggerVal.SelectedIndex = 0;
                }
            }
            else
            {
                if (cmbTrigger.SelectedIndex == (int) EventPage.CommonEventTriggers.Command)
                {
                    txtCommand.Show();
                    txtCommand.Text = CurrentPage.TriggerCommand;
                    lblCommand.Show();
                    lblCommand.Text = Strings.EventEditor.command;
                }
            }
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

        private void chkInteractionFreeze_CheckedChanged(object sender, EventArgs e)
        {
            CurrentPage.InteractionFreeze = Convert.ToInt32(chkInteractionFreeze.Checked);
        }

        #endregion

        #region "Inspector Options"

        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            CurrentPage.Desc = txtDesc.Text;
        }

        private void chkDisablePreview_CheckedChanged(object sender, EventArgs e)
        {
            CurrentPage.DisablePreview = Convert.ToInt32(chkDisableInspector.Checked);
            if (chkDisableInspector.Checked)
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
            CurrentPage.FaceGraphic = TextUtils.SanitizeNone(cmbPreviewFace?.Text);
            UpdateFacePreview();
        }

        #endregion
    }

    public class CommandListProperties
    {
        public EventCommand Cmd;
        public bool Editable;
        public int MyIndex;
        public CommandList MyList;
        public EventCommandType Type = EventCommandType.Null;
    }
}