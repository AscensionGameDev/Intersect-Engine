using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DarkUI.Controls;
using DarkUI.Forms;
using Intersect.Editor.Classes;
using Intersect.Editor.Classes.Core;
using Intersect.Editor.Forms.Editors;
using Intersect.Editor.Forms.Editors.Event_Commands;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Localization;

namespace Intersect.Editor.Forms
{
    public partial class FrmEvent : Form
    {
        private readonly List<CommandListProperties> _commandProperties = new List<CommandListProperties>();
        private readonly MapBase _currentMap;
        private int _currentCommand = -1;
        private EventCommand _editingCommand;
        private ByteBuffer _eventBackup = new ByteBuffer();
        private bool _isEdit;
        private bool _isInsert;
        private ByteBuffer _pageCopy;
        private List<DarkButton> _pageTabs = new List<DarkButton>();
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
            Text = Strings.Get("eventeditor", "title", MyEvent.Index, txtEventname.Text);
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
            commandMenu.Show((ListBox) sender, e.Location);
            btnEdit.Enabled = true;
            if (!_commandProperties[_currentCommand].Editable) btnEdit.Enabled = false;
            if (_commandProperties[_currentCommand].MyList.Commands.Count == 0 ||
                _commandProperties[_currentCommand].MyIndex >=
                _commandProperties[_currentCommand].MyList.Commands.Count ||
                _commandProperties[_currentCommand].MyIndex < 0) btnEdit.Enabled = false;
            btnDelete.Enabled = true;
        }

        private void lstEventCommands_DoubleClick(object sender, EventArgs e)
        {
            if (_currentCommand <= -1) return;
            if (!_commandProperties[_currentCommand].Editable) return;
            if (_commandProperties[_currentCommand].Type == EventCommandType.Null)
            {
                grpNewCommands.Show();
                DisableButtons();
                _isInsert = false;
            }
            else
            {
                grpNewCommands.Show();
                DisableButtons();
                _isInsert = true;
            }
        }

        /// <summary>
        ///     Opens the graphic selector window to pick the default graphic for this event page.
        /// </summary>
        private void pnlPreview_DoubleClick(object sender, EventArgs e)
        {
            Event_GraphicSelector graphicSelector = new Event_GraphicSelector(CurrentPage.Graphic, this);
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

        public void CloseMoveRouteDesigner(Event_MoveRouteDesigner moveRouteDesigner)
        {
            Controls.Remove(moveRouteDesigner);
        }

        public void CloseGraphicSelector(Event_GraphicSelector graphicSelector)
        {
            Controls.Remove(graphicSelector);
            UpdateEventPreview();
        }

        private void btnNewPage_Click(object sender, EventArgs e)
        {
            MyEvent.MyPages.Add(new EventPage());
            UpdateTabControl();
            LoadPage(MyEvent.MyPages.Count - 1);
        }

        private void UpdateTabControl()
        {
            foreach (var page in _pageTabs)
            {
                pnlTabs.Controls.Remove(page);
            }
            _pageTabs.Clear();
            for (int i = 0; i < MyEvent.MyPages.Count; i++)
            {
                var btn = new DarkButton()
                {
                    Text = (i + 1).ToString()
                };
                btn.Click += TabBtn_Click;
                _pageTabs.Add(btn);
            }
            pnlTabs.Controls.AddRange(_pageTabs.ToArray());
            for (int i = 0; i < MyEvent.MyPages.Count; i++)
            {
                var btn = _pageTabs[i];
                btn.Size = new Size(0, 0);
                btn.MaximumSize = new Size(100, 22);
                btn.AutoSize = true;
                if (i > 0)
                {
                    btn.Left = _pageTabs[i - 1].Right - 1;
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
            LoadPage(_pageTabs.IndexOf((DarkButton) sender));
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
                DarkMessageBox.ShowWarning(Strings.Get("eventcommandlist", "notcommon"),
                    Strings.Get("eventcommandlist", "notcommoncaption"), DarkDialogButton.Ok,
                    Properties.Resources.Icon);
                EnableButtons();
                return;
            }
            if (tmpCommand.Type == EventCommandType.WaitForRouteCompletion)
            {
                tmpCommand.Ints[0] = -1;
            }
            if (_isInsert)
            {
                _commandProperties[_currentCommand].MyList.Commands.Insert(
                    _commandProperties[_currentCommand].MyList.Commands
                        .IndexOf(_commandProperties[_currentCommand].Cmd),
                    tmpCommand);
            }
            else
            {
                _commandProperties[_currentCommand].MyList.Commands.Add(tmpCommand);
            }
            OpenEditCommand(tmpCommand);
            _isEdit = false;
        }

        private void btnEditConditions_Click(object sender, EventArgs e)
        {
            var editForm = new frmDynamicRequirements(CurrentPage.ConditionLists, RequirementType.Event);
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
            _currentMap = currentMap;
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
                DarkMessageBox.ShowWarning(Strings.Get("eventeditor", "savedialogue"),
                    Strings.Get("eventeditor", "savecaption"), DarkDialogButton.YesNo, Properties.Resources.Icon) ==
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
            grpGeneral.Text = Strings.Get("eventeditor", "general");
            lblName.Text = Strings.Get("eventeditor", "name");
            chkIsGlobal.Text = Strings.Get("eventeditor", "global");

            grpPageOptions.Text = Strings.Get("eventeditor", "pageoptions");
            btnNewPage.Text = Strings.Get("eventeditor", "newpage");
            btnCopyPage.Text = Strings.Get("eventeditor", "copypage");
            btnPastePage.Text = Strings.Get("eventeditor", "pastepage");
            btnDeletePage.Text = Strings.Get("eventeditor", "deletepage");
            btnClearPage.Text = Strings.Get("eventeditor", "clearpage");

            grpEventConditions.Text = Strings.Get("eventeditor", "conditions");
            btnEditConditions.Text = Strings.Get("eventeditor", "editconditions");

            grpEntityOptions.Text = Strings.Get("eventeditor", "entityoptions");
            grpPreview.Text = Strings.Get("eventeditor", "eventpreview");
            lblAnimation.Text = Strings.Get("eventeditor", "animation");

            grpMovement.Text = Strings.Get("eventeditor", "movement");
            lblType.Text = Strings.Get("eventeditor", "movementtype");
            cmbMoveType.Items.Clear();
            for (int i = 0; i < 3; i++)
            {
                cmbMoveType.Items.Add(Strings.Get("eventeditor", "movetype" + i));
            }
            btnSetRoute.Text = Strings.Get("eventeditor", "setroute");
            lblSpeed.Text = Strings.Get("eventeditor", "speed");
            cmbEventSpeed.Items.Clear();
            for (int i = 0; i < 5; i++)
            {
                cmbEventSpeed.Items.Add(Strings.Get("eventeditor", "speed" + i));
            }
            lblFreq.Text = Strings.Get("eventeditor", "frequency");
            cmbEventFreq.Items.Clear();
            for (int i = 0; i < 5; i++)
            {
                cmbEventFreq.Items.Add(Strings.Get("eventeditor", "frequency" + i));
            }
            lblLayer.Text = Strings.Get("eventeditor", "layer");
            cmbLayering.Items.Clear();
            for (int i = 0; i < 3; i++)
            {
                cmbLayering.Items.Add(Strings.Get("eventeditor", "layer" + i));
            }
            grpInspector.Text = Strings.Get("eventeditor", "inspector");
            chkDisableInspector.Text = Strings.Get("eventeditor", "disableinspector");
            lblInspectorDesc.Text = Strings.Get("eventeditor", "inspectordesc");
            lblFace.Text = Strings.Get("eventeditor", "face");
            grpExtra.Text = Strings.Get("eventeditor", "extras");
            chkWalkThrough.Text = Strings.Get("eventeditor", "passable");
            chkHideName.Text = Strings.Get("eventeditor", "hidename");
            chkDirectionFix.Text = Strings.Get("eventeditor", "directionfix");
            chkWalkingAnimation.Text = Strings.Get("eventeditor", "walkinganim");
            chkInteractionFreeze.Text = Strings.Get("eventeditor", "interactionfreeze");
            grpTriggers.Text = Strings.Get("eventeditor", "trigger");
            grpNewCommands.Text = Strings.Get("eventeditor", "addcommand");
            grpEventCommands.Text = Strings.Get("eventeditor", "commandlist");
            btnInsert.Text = Strings.Get("eventeditor", "insertcommand");
            btnEdit.Text = Strings.Get("eventeditor", "editcommand");
            btnDelete.Text = Strings.Get("eventeditor", "deletecommand");
            btnSave.Text = Strings.Get("eventeditor", "save");
            btnCancel.Text = Strings.Get("eventeditor", "cancel");

            for (int i = 0; i < lstCommands.Nodes.Count; i++)
            {
                lstCommands.Nodes[i].Text = Strings.Get("eventcommands", lstCommands.Nodes[i].Name);
                for (int x = 0; x < lstCommands.Nodes[i].Nodes.Count; x++)
                {
                    lstCommands.Nodes[i].Nodes[x].Text =
                        Strings.Get("eventcommands", lstCommands.Nodes[i].Nodes[x].Name);
                }
            }
        }

        #endregion

        #region "Loading/Initialization/Saving"

        /// <summary>
        ///     This function creates a backup of the event we are editting just in case we canel our revisions.
        ///     It also populates general lists in our editor (ie. switches/variables) for event spawning conditions.
        ///     If the event is a common event (not a map entity) we hide the entity options on the form.
        /// </summary>
        public void InitEditor()
        {
            _eventBackup = new ByteBuffer();
            _eventBackup.WriteBytes(MyEvent.EventData());
            txtEventname.Text = MyEvent.Name;
            if (MyEvent.Index < 0)
            {
                txtEventname.Enabled = false;
                grpTriggers.Hide();
            }
            cmbPreviewFace.Items.Clear();
            cmbPreviewFace.Items.Add(Strings.Get("general", "none"));
            cmbPreviewFace.Items.AddRange(GameContentManager.GetTextureNames(GameContentManager.TextureType.Face));
            cmbAnimation.Items.Clear();
            cmbAnimation.Items.Add(Strings.Get("general", "none"));
            cmbAnimation.Items.AddRange(Database.GetGameObjectList(GameObjectType.Animation));
            if (MyEvent.CommonEvent)
            {
                grpEntityOptions.Hide();
                cmbTrigger.Items.Clear();
                for (int i = 0; i < 6; i++)
                {
                    cmbTrigger.Items.Add(Strings.Get("eventeditor", "commontrigger" + i));
                }
            }
            else
            {
                cmbTrigger.Items.Clear();
                for (int i = 0; i < 3; i++) //Change the 3 to 4 when on projectile hit is done
                {
                    cmbTrigger.Items.Add(Strings.Get("eventeditor", "trigger" + i));
                }
                cmbTriggerVal.Items.Clear();
                cmbTriggerVal.Items.Add(Strings.Get("general", "none"));
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
            Text = Strings.Get("eventeditor", "title", MyEvent.Index, txtEventname.Text);
            CurrentPageIndex = pageNum;
            CurrentPage = MyEvent.MyPages[pageNum];
            for (int i = 0; i < _pageTabs.Count; i++)
            {
                if (i == CurrentPageIndex)
                {
                    _pageTabs[i].BackColor = System.Drawing.Color.FromArgb(90, 90, 90);
                }
                else
                {
                    _pageTabs[i].BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
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
                    lblTriggerVal.Text = Strings.Get("eventeditor", "projectile");
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
                    lblCommand.Text = Strings.Get("eventeditor", "command");
                }
            }
            cmbPreviewFace.SelectedIndex = cmbPreviewFace.Items.IndexOf(CurrentPage.FaceGraphic);
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
                if (MyMap.EventIndex == MyEvent.Index + 1) MyMap.EventIndex--;
                MyMap.Events.Remove(MyEvent.Index);
            }
            else
            {
                MyEvent.Load(_eventBackup.ToArray());
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
            if (MyEvent.CommonEvent && MyEvent.Index >= 0)
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
            _commandProperties.Clear();
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
                            lstEventCommands.Items.Add(indent + Strings.Get("eventcommandlist", "linestart") +
                                                       GetCommandText(commandList.Commands[i]));
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
                                lstEventCommands.Items.Add(indent + "      : " +
                                                           Strings.Get("eventcommandlist", "whenoption",
                                                               Truncate(commandList.Commands[i].Strs[x], 20)));
                                clp = new CommandListProperties
                                {
                                    Editable = false,
                                    MyIndex = i,
                                    MyList = commandList,
                                    Type = commandList.Commands[i].Type,
                                    Cmd = commandList.Commands[i]
                                };
                                _commandProperties.Add(clp);
                                PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[x - 1]],
                                    indent + "          ");
                            }
                            lstEventCommands.Items.Add(indent + "      : " +
                                                       Strings.Get("eventcommandlist", "endoptions"));
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
                            lstEventCommands.Items.Add(indent + Strings.Get("eventcommandlist", "linestart") +
                                                       GetCommandText(commandList.Commands[i]));
                            clp = new CommandListProperties
                            {
                                Editable = true,
                                MyIndex = i,
                                MyList = commandList,
                                Cmd = commandList.Commands[i],
                                Type = commandList.Commands[i].Type
                            };
                            _commandProperties.Add(clp);

                            PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[4]],
                                indent + "          ");

                            lstEventCommands.Items.Add(indent + "      : " +
                                                       Strings.Get("eventcommandlist", "conditionalelse"));
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };
                            _commandProperties.Add(clp);

                            PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[5]],
                                indent + "          ");

                            lstEventCommands.Items.Add(indent + "      : " +
                                                       Strings.Get("eventcommandlist", "conditionalend"));
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
                            lstEventCommands.Items.Add(indent + Strings.Get("eventcommandlist", "linestart") +
                                                       GetCommandText(commandList.Commands[i]));
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
                            lstEventCommands.Items.Add(indent + "      : " +
                                                       Strings.Get("eventcommandlist", "spellsucceeded"));
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };
                            _commandProperties.Add(clp);
                            PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[4]],
                                indent + "          ");

                            //When the spell failed to be taught:
                            lstEventCommands.Items.Add(indent + "      : " +
                                                       Strings.Get("eventcommandlist", "spellfailed"));
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };
                            _commandProperties.Add(clp);
                            PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[5]],
                                indent + "          ");

                            lstEventCommands.Items.Add(
                                indent + "      : " + Strings.Get("eventcommandlist", "endspell"));
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
                            lstEventCommands.Items.Add(indent + Strings.Get("eventcommandlist", "linestart") +
                                                       GetCommandText(commandList.Commands[i]));
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
                            lstEventCommands.Items.Add(indent + "      : " +
                                                       Strings.Get("eventcommandlist", "itemschanged"));
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };
                            _commandProperties.Add(clp);
                            PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[4]],
                                indent + "          ");

                            //When the items failed to be given/taken:
                            lstEventCommands.Items.Add(indent + "      : " +
                                                       Strings.Get("eventcommandlist", "itemnotchanged"));
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };
                            _commandProperties.Add(clp);
                            PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[5]],
                                indent + "          ");

                            lstEventCommands.Items.Add(indent + "      : " +
                                                       Strings.Get("eventcommandlist", "enditemchange"));
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

                        case EventCommandType.StartQuest:
                            lstEventCommands.Items.Add(indent + Strings.Get("eventcommandlist", "linestart") +
                                                       GetCommandText(commandList.Commands[i]));
                            clp = new CommandListProperties
                            {
                                Editable = true,
                                MyIndex = i,
                                MyList = commandList,
                                Cmd = commandList.Commands[i],
                                Type = commandList.Commands[i].Type
                            };
                            _commandProperties.Add(clp);

                            //When the quest is accepted/started successfully:
                            lstEventCommands.Items.Add(indent + "      : " +
                                                       Strings.Get("eventcommandlist", "queststarted"));
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };
                            _commandProperties.Add(clp);
                            PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[4]],
                                indent + "          ");

                            //When the quest was declined or requirements not met:
                            lstEventCommands.Items.Add(indent + "      : " +
                                                       Strings.Get("eventcommandlist", "questnotstarted"));
                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList.Commands[i].Type,
                                Cmd = commandList.Commands[i]
                            };
                            _commandProperties.Add(clp);
                            PrintCommandList(CurrentPage.CommandLists[commandList.Commands[i].Ints[5]],
                                indent + "          ");

                            lstEventCommands.Items.Add(indent + "      : " +
                                                       Strings.Get("eventcommandlist", "endstartquest"));
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
                            lstEventCommands.Items.Add(indent + Strings.Get("eventcommandlist", "linestart") +
                                                       GetCommandText(commandList.Commands[i]));
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
            lstEventCommands.Items.Add(indent + Strings.Get("eventcommandlist", "linestart"));
            clp = new CommandListProperties {Editable = true, MyIndex = -1, MyList = commandList};
            _commandProperties.Add(clp);
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
                    return Strings.Get("eventcommandlist", "showtext", Truncate(command.Strs[0], 30));
                case EventCommandType.ShowOptions:
                    return Strings.Get("eventcommandlist", "showoptions", Truncate(command.Strs[0], 30));
                case EventCommandType.AddChatboxText:
                    var channel = "";
                    switch (command.Ints[0])
                    {
                        case 0:
                            channel += Strings.Get("eventcommandlist", "chatplayer");
                            break;
                        case 1:
                            channel += Strings.Get("eventcommandlist", "chatlocal");
                            break;
                        case 2:
                            channel += Strings.Get("eventcommandlist", "chatglobal");
                            break;
                    }
                    return Strings.Get("eventcommandlist", "chatboxtext", channel, command.Strs[1],
                        Truncate(command.Strs[0], 20));
                case EventCommandType.SetSwitch:
                    var value = "";
                    value = Strings.Get("eventcommandlist", "false");
                    if (Convert.ToBoolean(command.Ints[2]))
                    {
                        value = Strings.Get("eventcommandlist", "true");
                    }
                    if (command.Ints[0] == (int) SwitchVariableTypes.PlayerSwitch)
                    {
                        return Strings.Get("eventcommandlist", "playerswitch",
                            PlayerSwitchBase.GetName(command.Ints[1]),
                            value);
                    }
                    else if (command.Ints[0] == (int) SwitchVariableTypes.ServerSwitch)
                    {
                        return Strings.Get("eventcommandlist", "globalswitch",
                            ServerSwitchBase.GetName(command.Ints[1]),
                            value);
                    }
                    else
                    {
                        return Strings.Get("eventcommandlist", "invalid");
                    }
                case EventCommandType.SetVariable:
                    var varvalue = "";
                    switch (command.Ints[2])
                    {
                        case 0:
                            varvalue = Strings.Get("eventcommandlist", "setvariable", command.Ints[3]);
                            break;
                        case 1:
                            varvalue = Strings.Get("eventcommandlist", "addvariable", command.Ints[3]);
                            break;

                        case 2:
                            varvalue = Strings.Get("eventcommandlist", "subtractvariable", command.Ints[3]);
                            break;

                        case 3:
                            varvalue = Strings.Get("eventcommandlist", "randvariable", command.Ints[3],
                                command.Ints[4]);
                            break;
                    }
                    if (command.Ints[0] == (int) SwitchVariableTypes.PlayerVariable)
                    {
                        return Strings.Get("eventcommandlist", "playervariable",
                            PlayerVariableBase.GetName(command.Ints[1]), varvalue);
                    }
                    else if (command.Ints[0] == (int) SwitchVariableTypes.ServerVariable)
                    {
                        return Strings.Get("eventcommandlist", "globalvariable",
                            ServerVariableBase.GetName(command.Ints[1]), varvalue);
                    }
                    else
                    {
                        return Strings.Get("eventcommandlist", "invalid");
                    }
                case EventCommandType.SetSelfSwitch:
                    var selfvalue = "";
                    selfvalue = Strings.Get("eventcommandlist", "false");
                    if (Convert.ToBoolean(command.Ints[1]))
                    {
                        selfvalue = Strings.Get("eventcommandlist", "true");
                    }
                    return Strings.Get("eventcommandlist", "selfswitch",
                        Strings.Get("eventcommandlist", "selfswitch" + command.Ints[0]), selfvalue);
                case EventCommandType.ConditionalBranch:
                    return Strings.Get("eventcommandlist", "conditionalbranch", command.GetConditionalDesc());
                case EventCommandType.ExitEventProcess:
                    return Strings.Get("eventcommandlist", "exitevent");
                case EventCommandType.Label:
                    return Strings.Get("eventcommandlist", "label", command.Strs[0]);
                case EventCommandType.GoToLabel:
                    return Strings.Get("eventcommandlist", "gotolabel", command.Strs[0]);
                case EventCommandType.StartCommonEvent:
                    return Strings.Get("eventcommandlist", "commonevent", EventBase.GetName(command.Ints[0]));
                case EventCommandType.RestoreHp:
                    return Strings.Get("eventcommandlist", "restorehp");
                case EventCommandType.RestoreMp:
                    return Strings.Get("eventcommandlist", "restoremp");
                case EventCommandType.LevelUp:
                    return Strings.Get("eventcommandlist", "levelup");
                case EventCommandType.GiveExperience:
                    return Strings.Get("eventcommandlist", "giveexp", command.Ints[0]);
                case EventCommandType.ChangeLevel:
                    return Strings.Get("eventcommandlist", "setlevel", command.Ints[0]);
                case EventCommandType.ChangeSpells:
                    if (command.Ints[0] == 0)
                    {
                        return Strings.Get("eventcommandlist", "changespells",
                            Strings.Get("eventcommandlist", "teach", SpellBase.GetName(command.Ints[1])));
                    }
                    else
                    {
                        return Strings.Get("eventcommandlist", "changespells",
                            Strings.Get("eventcommandlist", "forget", SpellBase.GetName(command.Ints[1])));
                    }
                case EventCommandType.ChangeItems:
                    if (command.Ints[0] == 0)
                    {
                        return Strings.Get("eventcommandlist", "changeitems",
                            Strings.Get("eventcommandlist", "give", ItemBase.GetName(command.Ints[1])));
                    }
                    else
                    {
                        return Strings.Get("eventcommandlist", "changeitems",
                            Strings.Get("eventcommandlist", "take", ItemBase.GetName(command.Ints[1])));
                    }
                case EventCommandType.ChangeSprite:
                    return Strings.Get("eventcommandlist", "setsprite", command.Strs[0]);
                case EventCommandType.ChangeFace:
                    return Strings.Get("eventcommandlist", "setface", command.Strs[0]);
                case EventCommandType.ChangeGender:
                    if (command.Ints[0] == 0)
                    {
                        return Strings.Get("eventcommandlist", "setgender", Strings.Get("eventcommandlist", "male"));
                    }
                    else
                    {
                        return Strings.Get("eventcommandlist", "setgender", Strings.Get("eventcommandlist", "female"));
                    }
                case EventCommandType.SetAccess:
                    switch (command.Ints[0])
                    {
                        case 0:
                            return Strings.Get("eventcommandlist", "setaccess",
                                Strings.Get("eventcommandlist", "regularuser"));
                        case 1:
                            return Strings.Get("eventcommandlist", "setaccess",
                                Strings.Get("eventcommandlist", "moderator"));
                        case 2:
                            return Strings.Get("eventcommandlist", "setaccess",
                                Strings.Get("eventcommandlist", "admin"));
                    }
                    return Strings.Get("eventcommandlist", "setaccess",
                        Strings.Get("eventcommandlist", "unknownrole"));
                case EventCommandType.WarpPlayer:
                    var mapName = Strings.Get("eventcommandlist", "mapnotfound");
                    for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
                    {
                        if (MapList.GetOrderedMaps()[i].MapNum == command.Ints[0])
                        {
                            mapName = MapList.GetOrderedMaps()[i].Name;
                        }
                    }
                    return Strings.Get("eventcommandlist", "warp", mapName, command.Ints[1], command.Ints[2],
                        Strings.Get("directions", (command.Ints[3] - 1).ToString()));
                case EventCommandType.SetMoveRoute:
                    if (command.Route.Target == -1)
                    {
                        return Strings.Get("eventcommandlist", "moveroute",
                            Strings.Get("eventcommandlist", "moverouteplayer"));
                    }
                    else
                    {
                        if (MyMap.Events.ContainsKey(command.Route.Target))
                        {
                            return Strings.Get("eventcommandlist", "moveroute",
                                Strings.Get("eventcommandlist", "moverouteevent", (command.Route.Target),
                                    MyMap.Events[command.Route.Target].Name));
                        }
                        else
                        {
                            return Strings.Get("eventcommandlist", "moveroute",
                                Strings.Get("eventcommandlist", "deletedevent"));
                        }
                    }
                case EventCommandType.WaitForRouteCompletion:
                    if (command.Ints[0] == -1)
                    {
                        return Strings.Get("eventcommandlist", "waitforroute",
                            Strings.Get("eventcommandlist", "moverouteplayer"));
                    }
                    else if (MyMap.Events.ContainsKey(command.Ints[0]))
                    {
                        return Strings.Get("eventcommandlist", "waitforroute",
                            Strings.Get("eventcommandlist", "moverouteevent", (command.Ints[0]),
                                MyMap.Events[command.Ints[0]].Name));
                    }
                    else
                    {
                        return Strings.Get("eventcommandlist", "waitforroute",
                            Strings.Get("eventcommandlist", "deletedevent"));
                    }
                case EventCommandType.HoldPlayer:
                    return Strings.Get("eventcommandlist", "holdplayer");
                case EventCommandType.ReleasePlayer:
                    return Strings.Get("eventcommandlist", "releaseplayer");
                case EventCommandType.SpawnNpc:
                    switch (command.Ints[1])
                    {
                        case 0: //On Map/Tile Selection
                            for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
                            {
                                if (MapList.GetOrderedMaps()[i].MapNum == command.Ints[2])
                                {
                                    return Strings.Get("eventcommandlist", "spawnnpc", NpcBase.GetName(command.Ints[0]),
                                        Strings.Get("eventcommandlist", "spawnonmap", MapList.GetOrderedMaps()[i].Name,
                                            command.Ints[3], command.Ints[4],
                                            Strings.Get("directions", command.Ints[5].ToString())));
                                }
                            }
                            return Strings.Get("eventcommandlist", "spawnnpc", NpcBase.GetName(command.Ints[0]),
                                Strings.Get("eventcommandlist", "spawnonmap",
                                    Strings.Get("eventcommandlist", "mapnotfound"), command.Ints[3], command.Ints[4],
                                    Strings.Get("directions", command.Ints[5].ToString())));
                        case 1: //On/Around Entity
                            var retain = Strings.Get("eventcommandlist", "false");
                            if (Convert.ToBoolean(command.Ints[5])) retain = Strings.Get("eventcommandlist", "true");
                            if (command.Ints[2] == -1)
                            {
                                return Strings.Get("eventcommandlist", "spawnnpc", NpcBase.GetName(command.Ints[0]),
                                    Strings.Get("eventcommandlist", "spawnonplayer", command.Ints[3], command.Ints[4],
                                        retain));
                            }
                            else
                            {
                                if (MyMap.Events.ContainsKey(command.Ints[2]))
                                {
                                    return Strings.Get("eventcommandlist", "spawnnpc", NpcBase.GetName(command.Ints[0]),
                                        Strings.Get("eventcommandlist", "spawnonevent", command.Ints[2] + 1,
                                            MyMap.Events[command.Ints[2]].Name, command.Ints[3], command.Ints[4],
                                            retain));
                                }
                                else
                                {
                                    return Strings.Get("eventcommandlist", "spawnnpc", NpcBase.GetName(command.Ints[0]),
                                        Strings.Get("eventcommandlist", "spawnonevent", command.Ints[2] + 1,
                                            Strings.Get("eventcommandlist", "deletedevent"), command.Ints[3],
                                            command.Ints[4], retain));
                                }
                            }
                    }
                    return output;
                case EventCommandType.DespawnNpc:
                    return Strings.Get("eventcommandlist", "despawnnpcs");
                case EventCommandType.PlayAnimation:
                    output += "Play Animation " + AnimationBase.GetName(command.Ints[0]) + " ";
                    switch (command.Ints[1])
                    {
                        case 0: //On Map/Tile Selection
                            for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
                            {
                                if (MapList.GetOrderedMaps()[i].MapNum == command.Ints[2])
                                {
                                    return Strings.Get("eventcommandlist", "playanimation",
                                        AnimationBase.GetName(command.Ints[0]),
                                        Strings.Get("eventcommandlist", "animationonmap",
                                            MapList.GetOrderedMaps()[i].Name, command.Ints[3], command.Ints[4],
                                            Strings.Get("directions", command.Ints[5].ToString())));
                                }
                            }
                            return Strings.Get("eventcommandlist", "playanimation",
                                AnimationBase.GetName(command.Ints[0]),
                                Strings.Get("eventcommandlist", "animationonmap",
                                    Strings.Get("eventcommandlist", "mapnotfound"), command.Ints[3], command.Ints[4],
                                    Strings.Get("directions", command.Ints[5].ToString())));
                        case 1: //On/Around Entity
                            var spawnOpt = "";
                            switch (command.Ints[5])
                            {
                                //0 does not adhere to direction, 1 is Spawning Relative to Direction, 2 is Rotating Relative to Direction, and 3 is both.
                                case 1:
                                    spawnOpt = Strings.Get("eventcommandlist", "animationrelativedir");
                                    break;
                                case 2:
                                    spawnOpt = Strings.Get("eventcommandlist", "animationrotatedir");
                                    break;
                                case 3:
                                    spawnOpt = Strings.Get("eventcommandlist", "animationrelativerotate");
                                    break;
                            }
                            if (command.Ints[2] == -1)
                            {
                                return Strings.Get("eventcommandlist", "playanimation",
                                    AnimationBase.GetName(command.Ints[0]),
                                    Strings.Get("eventcommandlist", "animationonplayer", command.Ints[3],
                                        command.Ints[4], spawnOpt));
                            }
                            else
                            {
                                if (MyMap.Events.ContainsKey(command.Ints[2]))
                                {
                                    return Strings.Get("eventcommandlist", "playanimation",
                                        AnimationBase.GetName(command.Ints[0]),
                                        Strings.Get("eventcommandlist", "animationonevent", (command.Ints[2] + 1),
                                            MyMap.Events[command.Ints[2]].Name, command.Ints[3],
                                            command.Ints[4], spawnOpt));
                                }
                                else
                                {
                                    return Strings.Get("eventcommandlist", "playanimation",
                                        AnimationBase.GetName(command.Ints[0]),
                                        Strings.Get("eventcommandlist", "animationonevent", (command.Ints[2] + 1),
                                            Strings.Get("eventcommandlist", "deletedevent"), command.Ints[3],
                                            command.Ints[4], spawnOpt));
                                }
                            }
                    }
                    return output;
                case EventCommandType.PlayBgm:
                    return Strings.Get("eventcommandlist", "playbgm", command.Strs[0]);
                case EventCommandType.FadeoutBgm:
                    return Strings.Get("eventcommandlist", "fadeoutbgm");
                case EventCommandType.PlaySound:
                    return Strings.Get("eventcommandlist", "playsound", command.Strs[0]);
                case EventCommandType.StopSounds:
                    return Strings.Get("eventcommandlist", "stopsounds");
                case EventCommandType.Wait:
                    return Strings.Get("eventcommandlist", "wait", command.Ints[0]);
                case EventCommandType.OpenBank:
                    return Strings.Get("eventcommandlist", "openbank");
                case EventCommandType.OpenShop:
                    return Strings.Get("eventcommandlist", "openshop", ShopBase.GetName(command.Ints[0]));
                case EventCommandType.OpenCraftingBench:
                    return Strings.Get("eventcommandlist", "opencrafting", BenchBase.GetName(command.Ints[0]));
                case EventCommandType.SetClass:
                    return Strings.Get("eventcommandlist", "setclass", ClassBase.GetName(command.Ints[0]));
                case EventCommandType.StartQuest:
                    if (command.Ints[1] == 0)
                    {
                        return Strings.Get("eventcommandlist", "startquest", QuestBase.GetName(command.Ints[0]),
                            Strings.Get("eventcommandlist", "forcedstart"));
                    }
                    else
                    {
                        return Strings.Get("eventcommandlist", "startquest", QuestBase.GetName(command.Ints[0]),
                            Strings.Get("eventcommandlist", "showoffer"));
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
                                return Strings.Get("eventcommandlist", "completetask",
                                    QuestBase.GetName(command.Ints[0]), task.GetTaskString());
                            }
                        }
                    }
                    return Strings.Get("eventcommandlist", "completetask", QuestBase.GetName(command.Ints[0]),
                        Strings.Get("eventcommandlist", "taskundefined"));
                case EventCommandType.EndQuest:
                    if (command.Ints[1] == 0)
                    {
                        return Strings.Get("eventcommandlist", "endquest", QuestBase.GetName(command.Ints[0]),
                            Strings.Get("eventcommandlist", "runcompletionevent"));
                    }
                    return Strings.Get("eventcommandlist", "endquest", QuestBase.GetName(command.Ints[0]),
                        Strings.Get("eventcommandlist", "skipcompletionevent"));
                default:
                    return Strings.Get("eventcommandlist", "unknown");
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
                case EventCommandType.DespawnNpc:
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
                case EventCommandType.OpenCraftingBench:
                    cmdWindow = new EventCommand_OpenBench(command, this);
                    break;
                case EventCommandType.SetClass:
                    cmdWindow = new EventCommand_SetClass(command, this);
                    break;
                case EventCommandType.StartQuest:
                    cmdWindow = new EventCommand_StartQuest(command, CurrentPage, this);
                    break;
                case EventCommandType.CompleteQuestTask:
                    cmdWindow = new EventCommand_CompleteQuestTask(command, this);
                    break;
                case EventCommandType.EndQuest:
                    cmdWindow = new EventCommand_EndQuest(command, this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (cmdWindow != null)
            {
                if (cmdWindow.GetType() == typeof(Event_MoveRouteDesigner))
                {
                    Controls.Add(cmdWindow);
                    cmdWindow.Width = ClientSize.Width;
                    cmdWindow.Height = ClientSize.Height;
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
            if (_currentCommand > -1 && _commandProperties.Count > _currentCommand && !condition)
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
        ///     Opens the 'Add Command' window in order to insert a command at the select location in the command list.
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
            if (_commandProperties[_currentCommand].MyList.Commands.Count == 0) return;
            OpenEditCommand(
                _commandProperties[_currentCommand].MyList.Commands[_commandProperties[_currentCommand].MyIndex]);
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
            Event_MoveRouteDesigner moveRouteDesigner = new Event_MoveRouteDesigner(this, _currentMap, MyEvent,
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
                    lblTriggerVal.Text = Strings.Get("eventeditor", "projectile");
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
                    lblCommand.Text = Strings.Get("eventeditor", "command");
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
            CurrentPage.FaceGraphic = cmbPreviewFace.Text;
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