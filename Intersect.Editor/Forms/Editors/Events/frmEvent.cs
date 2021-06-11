using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using DarkUI.Controls;
using DarkUI.Forms;

using Intersect.Editor.Content;
using Intersect.Editor.Forms.Editors.Events.Event_Commands;
using Intersect.Editor.Localization;
using Intersect.Editor.Maps;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;
using Intersect.GameObjects.Maps;
using Intersect.Utilities;

using Newtonsoft.Json;

namespace Intersect.Editor.Forms.Editors.Events
{

    public partial class FrmEvent : Form
    {

        private static string mCopyData = null;

        private static string mCopyLists = null;

        private readonly List<CommandListProperties> mCommandProperties = new List<CommandListProperties>();

        private readonly MapBase mCurrentMap;

        public EventPage CurrentPage;

        public int CurrentPageIndex;

        private int mCurrentCommand = -1;

        private EventCommand mEditingCommand;

        private string mEventBackup = null;

        private bool mIsEdit;

        private bool mIsInsert;

        private string mPageCopy;

        private List<DarkButton> mPageTabs = new List<DarkButton>();

        public EventBase MyEvent;

        public MapInstance MyMap;

        public bool NewEvent;

        public int mOldSelectedCommand;

        private void txtEventname_TextChanged(object sender, EventArgs e)
        {
            MyEvent.Name = txtEventname.Text;
            Text = Strings.EventEditor.title.ToString(txtEventname.Text);
        }

        private void lstEventCommands_SelectedIndexChanged(object sender, EventArgs e)
        {
            mCurrentCommand = lstEventCommands.SelectedIndex;
            mOldSelectedCommand = lstEventCommands.SelectedIndex;
        }

        private void lstEventCommands_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Delete)
            {
                return;
            }

            if (mCurrentCommand < 0 || mCurrentCommand >= mCommandProperties.Count)
            {
                return;
            }

            if (!mCommandProperties[mCurrentCommand].Editable)
            {
                return;
            }

            if (mCommandProperties[mCurrentCommand].MyIndex < 0 ||
                mCommandProperties[mCurrentCommand].MyIndex >= mCommandProperties[mCurrentCommand].MyList.Count)
            {
                return;
            }

            HandleRemoveCommand(mCommandProperties[mCurrentCommand].Cmd);
            mCommandProperties[mCurrentCommand].MyList.Remove(mCommandProperties[mCurrentCommand].Cmd);
            mCurrentCommand = -1;

            mOldSelectedCommand = lstEventCommands.SelectedIndex;
            ListPageCommands();
        }

        private void lstEventCommands_Click(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            var i = lstEventCommands.IndexFromPoint(e.Location);
            if (i <= -1 || i >= lstEventCommands.Items.Count)
            {
                return;
            }

            if (!mCommandProperties[i].Editable)
            {
                return;
            }

            lstEventCommands.SelectedIndex = i;

            if (mCurrentCommand < 0 || mCurrentCommand >= mCommandProperties.Count)
            {
                return;
            }

            commandMenu.Show((ListBox) sender, e.Location);
            btnEdit.Enabled = mCommandProperties[mCurrentCommand].Editable;
            if (mCommandProperties[mCurrentCommand].MyIndex < 0 ||
                mCommandProperties[mCurrentCommand].MyIndex >= mCommandProperties[mCurrentCommand].MyList.Count)
            {
                btnEdit.Enabled = false;
            }

            btnCopy.Enabled = btnEdit.Enabled;
            btnCut.Enabled = btnEdit.Enabled;
            btnDelete.Enabled = true;

            mOldSelectedCommand = lstEventCommands.SelectedIndex;
        }

        private void lstEventCommands_DoubleClick(object sender, EventArgs e)
        {
            if (mCurrentCommand <= -1)
            {
                return;
            }

            if (!mCommandProperties[mCurrentCommand].Editable)
            {
                return;
            }

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

            mOldSelectedCommand = lstEventCommands.SelectedIndex;
        }

        /// <summary>
        ///     Opens the graphic selector window to pick the default graphic for this event page.
        /// </summary>
        private void pnlPreview_DoubleClick(object sender, EventArgs e)
        {
            var graphicSelector = new EventGraphicSelector(CurrentPage.Graphic, this);
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

            if (CurrentPage.Graphic.Type == EventGraphicType.Sprite)
            {
                if (File.Exists("resources/entities/" + CurrentPage.Graphic.Filename))
                {
                    sourceBitmap = new Bitmap("resources/entities/" + CurrentPage.Graphic.Filename);
                }
            }
            else if (CurrentPage.Graphic.Type == EventGraphicType.Tileset)
            {
                if (File.Exists("resources/tilesets/" + CurrentPage.Graphic.Filename))
                {
                    sourceBitmap = new Bitmap("resources/tilesets/" + CurrentPage.Graphic.Filename);
                }
            }

            if (sourceBitmap != null)
            {
                if (CurrentPage.Graphic.Type == EventGraphicType.Sprite)
                {
                    graphics.DrawImage(
                        sourceBitmap,
                        new Rectangle(
                            pnlPreview.Width / 2 - sourceBitmap.Width / Options.Instance.Sprites.NormalFrames / 2,
                            pnlPreview.Height / 2 - sourceBitmap.Height / Options.Instance.Sprites .Directions / 2, sourceBitmap.Width / Options.Instance.Sprites.NormalFrames,
                            sourceBitmap.Height / Options.Instance.Sprites.Directions
                        ),
                        new Rectangle(
                            CurrentPage.Graphic.X * sourceBitmap.Width / Options.Instance.Sprites.NormalFrames,
                            CurrentPage.Graphic.Y * sourceBitmap.Height / Options.Instance.Sprites.Directions, sourceBitmap.Width / Options.Instance.Sprites.NormalFrames,
                            sourceBitmap.Height / Options.Instance.Sprites.Directions
                        ), GraphicsUnit.Pixel
                    );
                }
                else if (CurrentPage.Graphic.Type == EventGraphicType.Tileset)
                {
                    graphics.DrawImage(
                        sourceBitmap,
                        new Rectangle(
                            pnlPreview.Width / 2 -
                            (Options.TileWidth + CurrentPage.Graphic.Width * Options.TileWidth) / 2,
                            pnlPreview.Height / 2 -
                            (Options.TileHeight + CurrentPage.Graphic.Height * Options.TileHeight) / 2,
                            Options.TileWidth + CurrentPage.Graphic.Width * Options.TileWidth,
                            Options.TileHeight + CurrentPage.Graphic.Height * Options.TileHeight
                        ),
                        new Rectangle(
                            CurrentPage.Graphic.X * Options.TileWidth, CurrentPage.Graphic.Y * Options.TileHeight,
                            Options.TileWidth + CurrentPage.Graphic.Width * Options.TileWidth,
                            Options.TileHeight + CurrentPage.Graphic.Height * Options.TileHeight
                        ), GraphicsUnit.Pixel
                    );
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
            for (var i = 0; i < MyEvent.Pages.Count; i++)
            {
                var btn = new DarkButton()
                {
                    Text = (i + 1).ToString()
                };

                btn.Click += TabBtn_Click;
                mPageTabs.Add(btn);
            }

            pnlTabs.Controls.AddRange(mPageTabs.ToArray());
            for (var i = 0; i < MyEvent.Pages.Count; i++)
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
            else if (lstEventCommands.Focused && e.KeyData == (Keys.Control | Keys.X))
            {
                if (lstEventCommands.SelectedIndex > -1)
                {
                    btnCut_Click(null, null);
                }

                return;
            }
            else if (lstEventCommands.Focused && e.KeyData == (Keys.Control | Keys.C))
            {
                if (lstEventCommands.SelectedIndex > -1)
                {
                    btnCopy_Click(null, null);
                }

                return;
            }
            else if (lstEventCommands.Focused && e.KeyData == (Keys.Control | Keys.V))
            {
                if (lstEventCommands.SelectedIndex > -1)
                {
                    btnPaste_Click(null, null);
                }

                return;
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
            mPageCopy = JsonConvert.SerializeObject(
                CurrentPage,
                new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
                }
            );

            EnableButtons();
        }

        private void btnPastePage_Click(object sender, EventArgs e)
        {
            if (mPageCopy != null)
            {
                MyEvent.Pages[CurrentPageIndex] = new EventPage();
                JsonConvert.PopulateObject(
                    mPageCopy, MyEvent.Pages[CurrentPageIndex],
                    new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                        DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                        ObjectCreationHandling = ObjectCreationHandling.Replace
                    }
                );

                LoadPage(CurrentPageIndex);
            }
        }

        private void cmbAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage.AnimationId = AnimationBase.IdFromList(cmbAnimation.SelectedIndex - 1);
        }

        private void chkIsGlobal_CheckedChanged(object sender, EventArgs e)
        {
            MyEvent.Global = chkIsGlobal.Checked;
        }

        private void lblCloseCommands_Click(object sender, EventArgs e)
        {
            if (grpNewCommands.Visible)
            {
                grpNewCommands.Hide();
                EnableButtons();
            }
        }

        private void btnTabsRight_Click(object sender, EventArgs e)
        {
            if (pnlTabs.Right > pnlTabsContainer.Width)
            {
                pnlTabs.SetBounds(pnlTabs.Bounds.Left - 200, 0, pnlTabs.Width, pnlTabs.Height);
                if (pnlTabs.Right < pnlTabsContainer.Width)
                {
                    pnlTabs.SetBounds(
                        pnlTabs.Bounds.Left + pnlTabsContainer.Width - pnlTabs.Right, 0, pnlTabs.Width, pnlTabs.Height
                    );
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
                    pnlTabs.SetBounds(0, 0, pnlTabs.Width, pnlTabs.Height);
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

            var type = (EventCommandType) int.Parse(e.Node.Tag.ToString());

            if ((type == EventCommandType.SetMoveRoute || type == EventCommandType.WaitForRouteCompletion) &&
                MyEvent.CommonEvent)
            {
                DarkMessageBox.ShowWarning(
                    Strings.EventCommandList.notcommon, Strings.EventCommandList.notcommoncaption, DarkDialogButton.Ok,
                    Properties.Resources.Icon
                );

                EnableButtons();

                return;
            }

            EventCommand tmpCommand = null;
            grpNewCommands.Hide();

            switch (type)
            {
                case EventCommandType.Null:
                    break;
                case EventCommandType.ShowText:
                    tmpCommand = new ShowTextCommand();

                    break;
                case EventCommandType.ShowOptions:
                    tmpCommand = new ShowOptionsCommand(CurrentPage.CommandLists);

                    break;
                case EventCommandType.InputVariable:
                    tmpCommand = new InputVariableCommand(CurrentPage.CommandLists);

                    break;
                case EventCommandType.AddChatboxText:
                    tmpCommand = new AddChatboxTextCommand();

                    break;
                case EventCommandType.SetVariable:
                    tmpCommand = new SetVariableCommand();

                    break;
                case EventCommandType.SetSelfSwitch:
                    tmpCommand = new SetSelfSwitchCommand() {Value = true};

                    break;
                case EventCommandType.ConditionalBranch:
                    tmpCommand = new ConditionalBranchCommand(CurrentPage.CommandLists);

                    break;
                case EventCommandType.ExitEventProcess:
                    tmpCommand = new ExitEventProcessingCommand();

                    break;
                case EventCommandType.Label:
                    tmpCommand = new LabelCommand();

                    break;
                case EventCommandType.GoToLabel:
                    tmpCommand = new GoToLabelCommand();

                    break;
                case EventCommandType.StartCommonEvent:
                    tmpCommand = new StartCommmonEventCommand();

                    break;
                case EventCommandType.RestoreHp:
                    tmpCommand = new RestoreHpCommand();

                    break;
                case EventCommandType.RestoreMp:
                    tmpCommand = new RestoreMpCommand();

                    break;
                case EventCommandType.LevelUp:
                    tmpCommand = new LevelUpCommand();

                    break;
                case EventCommandType.GiveExperience:
                    tmpCommand = new GiveExperienceCommand();

                    break;
                case EventCommandType.ChangeLevel:
                    tmpCommand = new ChangeLevelCommand();

                    break;
                case EventCommandType.ChangeSpells:
                    tmpCommand = new ChangeSpellsCommand(CurrentPage.CommandLists);

                    break;
                case EventCommandType.ChangeItems:
                    tmpCommand = new ChangeItemsCommand(CurrentPage.CommandLists);

                    break;
                case EventCommandType.EquipItem:
                    tmpCommand = new EquipItemCommand();

                    break;
                case EventCommandType.ChangeSprite:
                    tmpCommand = new ChangeSpriteCommand();

                    break;
                case EventCommandType.ChangeFace:
                    tmpCommand = new ChangeFaceCommand();

                    break;
                case EventCommandType.ChangeGender:
                    tmpCommand = new ChangeGenderCommand();

                    break;
                case EventCommandType.ChangeNameColor:
                    tmpCommand = new ChangeNameColorCommand();

                    break;
                case EventCommandType.PlayerLabel:
                    tmpCommand = new ChangePlayerLabelCommand();

                    break;
                case EventCommandType.SetAccess:
                    tmpCommand = new SetAccessCommand();

                    break;
                case EventCommandType.WarpPlayer:
                    tmpCommand = new WarpCommand();

                    break;
                case EventCommandType.SetMoveRoute:
                    tmpCommand = new SetMoveRouteCommand();

                    break;
                case EventCommandType.WaitForRouteCompletion:
                    tmpCommand = new WaitForRouteCommand();

                    break;
                case EventCommandType.HoldPlayer:
                    tmpCommand = new HoldPlayerCommand();

                    break;
                case EventCommandType.ReleasePlayer:
                    tmpCommand = new ReleasePlayerCommand();

                    break;
                case EventCommandType.SpawnNpc:
                    tmpCommand = new SpawnNpcCommand();

                    break;
                case EventCommandType.PlayAnimation:
                    tmpCommand = new PlayAnimationCommand();

                    break;
                case EventCommandType.PlayBgm:
                    tmpCommand = new PlayBgmCommand();

                    break;
                case EventCommandType.FadeoutBgm:
                    tmpCommand = new FadeoutBgmCommand();

                    break;
                case EventCommandType.PlaySound:
                    tmpCommand = new PlaySoundCommand();

                    break;
                case EventCommandType.StopSounds:
                    tmpCommand = new StopSoundsCommand();

                    break;
                case EventCommandType.Wait:
                    tmpCommand = new WaitCommand();

                    break;
                case EventCommandType.OpenBank:
                    tmpCommand = new OpenBankCommand();

                    break;
                case EventCommandType.OpenShop:
                    tmpCommand = new OpenShopCommand();

                    break;
                case EventCommandType.OpenCraftingTable:
                    tmpCommand = new OpenCraftingTableCommand();

                    break;
                case EventCommandType.SetClass:
                    tmpCommand = new SetClassCommand();

                    break;
                case EventCommandType.DespawnNpc:
                    tmpCommand = new DespawnNpcCommand();

                    break;
                case EventCommandType.StartQuest:
                    tmpCommand = new StartQuestCommand(CurrentPage.CommandLists);

                    break;
                case EventCommandType.CompleteQuestTask:
                    tmpCommand = new CompleteQuestTaskCommand();

                    break;
                case EventCommandType.EndQuest:
                    tmpCommand = new EndQuestCommand();

                    break;
                case EventCommandType.ShowPicture:
                    tmpCommand = new ShowPictureCommand();

                    break;
                case EventCommandType.HidePicture:
                    tmpCommand = new HidePictureCommmand();

                    break;
                case EventCommandType.HidePlayer:
                    tmpCommand = new HidePlayerCommand();

                    break;
                case EventCommandType.ShowPlayer:
                    tmpCommand = new ShowPlayerCommand();

                    break;

                case EventCommandType.ChangePlayerColor:
                    tmpCommand = new ChangePlayerColorCommand();

                    break;

                case EventCommandType.ChangeName:
                    tmpCommand = new ChangeNameCommand(CurrentPage.CommandLists);

                    break;

                case EventCommandType.CreateGuild:
                    tmpCommand = new CreateGuildCommand(CurrentPage.CommandLists);

                    break;

                case EventCommandType.DisbandGuild:
                    tmpCommand = new DisbandGuildCommand(CurrentPage.CommandLists);

                    break;
                case EventCommandType.OpenGuildBank:
                    tmpCommand = new OpenGuildBankCommand();

                    break;
                case EventCommandType.SetGuildBankSlots:
                    tmpCommand = new SetGuildBankSlotsCommand();

                    break;
                case EventCommandType.ResetStatPointAllocations:
                    tmpCommand = new ResetStatPointAllocationsCommand();

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (mIsInsert)
            {
                mCommandProperties[mCurrentCommand]
                    .MyList.Insert(
                        mCommandProperties[mCurrentCommand].MyList.IndexOf(mCommandProperties[mCurrentCommand].Cmd),
                        tmpCommand
                    );
            }
            else
            {
                mCommandProperties[mCurrentCommand].MyList.Add(tmpCommand);
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
			
			this.Icon = Properties.Resources.Icon;
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
            if (e.CloseReason != CloseReason.UserClosing)
            {
                return;
            }

            if (btnSave.Enabled == false)
            {
                e.Cancel = true;

                return;
            }

            if (DarkMessageBox.ShowWarning(
                    Strings.EventEditor.savedialogue, Strings.EventEditor.savecaption, DarkDialogButton.YesNo,
                    Properties.Resources.Icon
                ) ==
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
            for (var i = 0; i < Strings.EventEditor.movementtypes.Count; i++)
            {
                cmbMoveType.Items.Add(Strings.EventEditor.movementtypes[i]);
            }

            btnSetRoute.Text = Strings.EventEditor.setroute;
            lblSpeed.Text = Strings.EventEditor.speed;
            cmbEventSpeed.Items.Clear();
            for (var i = 0; i < Strings.EventEditor.speeds.Count; i++)
            {
                cmbEventSpeed.Items.Add(Strings.EventEditor.speeds[i]);
            }

            lblFreq.Text = Strings.EventEditor.frequency;
            cmbEventFreq.Items.Clear();
            for (var i = 0; i < Strings.EventEditor.frequencies.Count; i++)
            {
                cmbEventFreq.Items.Add(Strings.EventEditor.frequencies[i]);
            }

            lblLayer.Text = Strings.EventEditor.layer;
            cmbLayering.Items.Clear();
            for (var i = 0; i < Strings.EventEditor.layers.Count; i++)
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
            btnCopy.Text = Strings.EventEditor.copycommand;
            btnCut.Text = Strings.EventEditor.cutcommand;
            btnPaste.Text = Strings.EventEditor.pastecommand;
            btnSave.Text = Strings.EventEditor.save;
            btnCancel.Text = Strings.EventEditor.cancel;

            for (var i = 0; i < lstCommands.Nodes.Count; i++)
            {
                lstCommands.Nodes[i].Text = Strings.EventCommands.commands[lstCommands.Nodes[i].Name];
                for (var x = 0; x < lstCommands.Nodes[i].Nodes.Count; x++)
                {
                    lstCommands.Nodes[i].Nodes[x].Text =
                        Strings.EventCommands.commands[lstCommands.Nodes[i].Nodes[x].Name];
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
        public void InitEditor(bool disableNaming, bool disableTriggers, bool questEvent)
        {
            mEventBackup = MyEvent.JsonData;
            txtEventname.Text = MyEvent.Name;
            if (disableNaming)
            {
                txtEventname.Enabled = false;
            }

            if (disableTriggers)
            {
                grpTriggers.Hide();
            }

            cmbPreviewFace.Items.Clear();
            cmbPreviewFace.Items.Add(Strings.General.none);
            cmbPreviewFace.Items.AddRange(
                GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Face)
            );

            cmbAnimation.Items.Clear();
            cmbAnimation.Items.Add(Strings.General.none);
            cmbAnimation.Items.AddRange(AnimationBase.Names);
            if (MyEvent.CommonEvent || questEvent)
            {
                grpEntityOptions.Hide();
                cmbTrigger.Items.Clear();
                for (var i = 0; i < Strings.EventEditor.commontriggers.Count; i++)
                {
                    cmbTrigger.Items.Add(Strings.EventEditor.commontriggers[i]);
                }
            }
            else
            {
                cmbTrigger.Items.Clear();
                for (var i = 0; i < Strings.EventEditor.triggers.Count; i++)
                {
                    cmbTrigger.Items.Add(Strings.EventEditor.triggers[i]);
                }

                cmbTriggerVal.Items.Clear();
                cmbTriggerVal.Items.Add(Strings.General.none);
                cmbTriggerVal.Items.AddRange(ProjectileBase.Names);
            }

            chkIsGlobal.Checked = Convert.ToBoolean(MyEvent.Global);
            if (MyEvent.CommonEvent)
            {
                chkIsGlobal.Hide();
            }

            UpdateTabControl();
            LoadPage(0);
        }

        /// <summary>
        ///     Initializes all of the form controls with values from the passed event page.
        /// </summary>
        /// <param name="pageNum">The index of the page to load.</param>
        public void LoadPage(int pageNum)
        {
            Text = Strings.EventEditor.title.ToString(txtEventname.Text);
            CurrentPageIndex = pageNum;
            if (MyEvent.Pages.Count == 0)
            {
                MyEvent.Pages.Add(new EventPage());
            }

            CurrentPage = MyEvent.Pages[pageNum];
            for (var i = 0; i < mPageTabs.Count; i++)
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

            cmbMoveType.SelectedIndex = (int) CurrentPage.Movement.Type;
            if (CurrentPage.Movement.Type == EventMovementType.MoveRoute)
            {
                btnSetRoute.Enabled = true;
            }
            else
            {
                btnSetRoute.Enabled = false;
            }

            cmbEventSpeed.SelectedIndex = (int) CurrentPage.Movement.Speed;
            cmbEventFreq.SelectedIndex = (int) CurrentPage.Movement.Frequency;
            chkWalkThrough.Checked = Convert.ToBoolean(CurrentPage.Passable);
            cmbLayering.SelectedIndex = (int) CurrentPage.Layer;
            if (MyEvent.CommonEvent)
            {
                cmbTrigger.SelectedIndex = (int) CurrentPage.CommonTrigger;
            }
            else
            {
                cmbTrigger.SelectedIndex = (int) CurrentPage.Trigger;
            }

            SetupTrigger();

            cmbPreviewFace.SelectedIndex = cmbPreviewFace.Items.IndexOf(TextUtils.NullToNone(CurrentPage.FaceGraphic));
            if (cmbPreviewFace.SelectedIndex == -1)
            {
                cmbPreviewFace.SelectedIndex = 0;
                UpdateFacePreview();
            }

            cmbAnimation.SelectedIndex = AnimationBase.ListIndex(CurrentPage.AnimationId) + 1;
            chkHideName.Checked = Convert.ToBoolean(CurrentPage.HideName);
            chkDisableInspector.Checked = Convert.ToBoolean(CurrentPage.DisablePreview);
            chkDirectionFix.Checked = Convert.ToBoolean(CurrentPage.DirectionFix);
            chkWalkingAnimation.Checked = Convert.ToBoolean(CurrentPage.WalkingAnimation);
            chkInteractionFreeze.Checked = Convert.ToBoolean(CurrentPage.InteractionFreeze);
            txtDesc.Text = CurrentPage.Description;
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
            CommandPrinter.PrintCommandList(
                CurrentPage, CurrentPage.CommandLists.Values.First(), " ", lstEventCommands, mCommandProperties, MyMap
            );

            // Reset our view to roughly where the user left off.
            if (mOldSelectedCommand > (lstEventCommands.Items.Count -1))
            {
                lstEventCommands.SelectedIndex = lstEventCommands.Items.Count - 1;
            }
            else
            {
                lstEventCommands.SelectedIndex = mOldSelectedCommand;
            }
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
                    cmdWindow = new EventCommandText((ShowTextCommand) command, this);

                    break;
                case EventCommandType.ShowOptions:
                    cmdWindow = new EventCommandOptions((ShowOptionsCommand) command, CurrentPage, this);

                    break;
                case EventCommandType.InputVariable:
                    cmdWindow = new EventCommandInput((InputVariableCommand) command, this);

                    break;
                case EventCommandType.AddChatboxText:
                    cmdWindow = new EventCommandChatboxText((AddChatboxTextCommand) command, this);

                    break;
                case EventCommandType.SetVariable:
                    cmdWindow = new EventCommandVariable((SetVariableCommand) command, this);

                    break;
                case EventCommandType.SetSelfSwitch:
                    cmdWindow = new EventCommandSelfSwitch((SetSelfSwitchCommand) command, this);

                    break;
                case EventCommandType.ConditionalBranch:
                    cmdWindow = new EventCommandConditionalBranch(
                        ((ConditionalBranchCommand) command).Condition, CurrentPage, this,
                        (ConditionalBranchCommand) command
                    );

                    break;
                case EventCommandType.ExitEventProcess:
                    //No editor
                    break;
                case EventCommandType.Label:
                    cmdWindow = new EventCommandLabel((LabelCommand) command, this);

                    break;
                case EventCommandType.GoToLabel:
                    cmdWindow = new EventCommandGotoLabel((GoToLabelCommand) command, this);

                    break;
                case EventCommandType.StartCommonEvent:
                    cmdWindow = new EventCommandStartCommonEvent((StartCommmonEventCommand) command, this);

                    break;
                case EventCommandType.RestoreHp:
                    cmdWindow = new EventCommandChangeVital((RestoreHpCommand) command, this);

                    break;
                case EventCommandType.RestoreMp:
                    cmdWindow = new EventCommandChangeVital((RestoreMpCommand) command, this);

                    break;
                case EventCommandType.LevelUp:
                    //No editor
                    break;
                case EventCommandType.GiveExperience:
                    cmdWindow = new EventCommandGiveExperience((GiveExperienceCommand) command, this);

                    break;
                case EventCommandType.ChangeLevel:
                    cmdWindow = new EventCommandChangeLevel((ChangeLevelCommand) command, this);

                    break;
                case EventCommandType.ChangeSpells:
                    cmdWindow = new EventCommandChangeSpells((ChangeSpellsCommand) command, CurrentPage, this);

                    break;
                case EventCommandType.ChangeItems:
                    cmdWindow = new EventCommandChangeItems((ChangeItemsCommand) command, CurrentPage, this);

                    break;
                case EventCommandType.EquipItem:
                    cmdWindow = new EventCommandEquipItems((EquipItemCommand) command, this);

                    break;
                case EventCommandType.ChangeSprite:
                    cmdWindow = new EventCommandChangeSprite((ChangeSpriteCommand) command, this);

                    break;
                case EventCommandType.ChangeFace:
                    cmdWindow = new EventCommandChangeFace((ChangeFaceCommand) command, this);

                    break;
                case EventCommandType.ChangeGender:
                    cmdWindow = new EventCommandChangeGender((ChangeGenderCommand) command, this);

                    break;
                case EventCommandType.ChangeNameColor:
                    cmdWindow = new EventCommandChangeNameColor((ChangeNameColorCommand) command, this);

                    break;
                case EventCommandType.PlayerLabel:
                    cmdWindow = new EventCommandChangePlayerLabel((ChangePlayerLabelCommand) command, this);

                    break;
                case EventCommandType.SetAccess:
                    cmdWindow = new EventCommandSetAccess((SetAccessCommand) command, this);

                    break;
                case EventCommandType.WarpPlayer:
                    cmdWindow = new EventCommandWarp((WarpCommand) command, this);

                    break;
                case EventCommandType.SetMoveRoute:
                    var cmd = (SetMoveRouteCommand) command;
                    if (cmd.Route == null)
                    {
                        cmd.Route = new EventMoveRoute();
                    }

                    cmdWindow = new EventMoveRouteDesigner(this, mCurrentMap, MyEvent, cmd.Route, cmd);

                    break;
                case EventCommandType.WaitForRouteCompletion:
                    cmdWindow = new EventCommandWaitForRouteCompletion(
                        (WaitForRouteCommand) command, this, mCurrentMap, MyEvent
                    );

                    break;
                case EventCommandType.HoldPlayer:
                    break;
                case EventCommandType.ReleasePlayer:
                    break;
                case EventCommandType.HidePlayer:
                    break;
                case EventCommandType.ShowPlayer:
                    break;
                case EventCommandType.SpawnNpc:
                    cmdWindow = new EventCommandSpawnNpc(this, mCurrentMap, MyEvent, (SpawnNpcCommand) command);

                    break;
                case EventCommandType.DespawnNpc:
                    break;
                case EventCommandType.PlayAnimation:
                    cmdWindow = new EventCommandPlayAnimation(
                        this, mCurrentMap, MyEvent, (PlayAnimationCommand) command
                    );

                    break;
                case EventCommandType.PlayBgm:
                    cmdWindow = new EventCommandPlayBgm((PlayBgmCommand) command, this);

                    break;
                case EventCommandType.FadeoutBgm:
                    break;
                case EventCommandType.PlaySound:
                    cmdWindow = new EventCommandPlayBgs((PlaySoundCommand) command, this);

                    break;
                case EventCommandType.StopSounds:
                    break;
                case EventCommandType.ShowPicture:
                    cmdWindow = new EventCommand_ShowPicture((ShowPictureCommand) command, this);

                    break;
                case EventCommandType.HidePicture:
                    break;
                case EventCommandType.Wait:
                    cmdWindow = new EventCommandWait((WaitCommand) command, this);

                    break;
                case EventCommandType.OpenBank:
                    break;
                case EventCommandType.OpenShop:
                    cmdWindow = new EventCommandOpenShop((OpenShopCommand) command, this);

                    break;
                case EventCommandType.OpenCraftingTable:
                    cmdWindow = new EventCommandOpenCraftingTable((OpenCraftingTableCommand) command, this);

                    break;
                case EventCommandType.SetClass:
                    cmdWindow = new EventCommandSetClass((SetClassCommand) command, this);

                    break;
                case EventCommandType.StartQuest:
                    cmdWindow = new EventCommandStartQuest((StartQuestCommand) command, CurrentPage, this);

                    break;
                case EventCommandType.CompleteQuestTask:
                    cmdWindow = new EventCommandCompleteQuestTask((CompleteQuestTaskCommand) command, this);

                    break;
                case EventCommandType.EndQuest:
                    cmdWindow = new EventCommandEndQuest((EndQuestCommand) command, this);

                    break;
                case EventCommandType.ChangePlayerColor:
                    cmdWindow = new EventCommandChangePlayerColor((ChangePlayerColorCommand)command, this);

                    break;
                case EventCommandType.ChangeName:
                    cmdWindow = new EventCommandChangeName((ChangeNameCommand)command, CurrentPage, this);

                    break;

                case EventCommandType.CreateGuild:
                    cmdWindow = new EventCommandCreateGuild((CreateGuildCommand)command, CurrentPage, this);

                    break;

                case EventCommandType.DisbandGuild:

                    break;
                case EventCommandType.OpenGuildBank:

                    break;
                case EventCommandType.SetGuildBankSlots:
                    cmdWindow = new EventCommandSetGuildBankSlots((SetGuildBankSlotsCommand)command, CurrentPage, this);

                    break;
                case EventCommandType.ResetStatPointAllocations:

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
                    cmdWindow.Left = grpCreateCommands.Width / 2 - cmdWindow.Width / 2;
                    cmdWindow.Top = grpCreateCommands.Height / 2 - cmdWindow.Height / 2;
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
                        HandleRemoveCommand(
                            mCommandProperties[mCurrentCommand]
                                .MyList[
                                    mCommandProperties[mCurrentCommand]
                                        .MyList.IndexOf(mCommandProperties[mCurrentCommand].Cmd) -
                                    1]
                        );

                        mCommandProperties[mCurrentCommand]
                            .MyList.RemoveAt(
                                mCommandProperties[mCurrentCommand]
                                    .MyList.IndexOf(mCommandProperties[mCurrentCommand].Cmd) -
                                1
                            );
                    }
                    else
                    {
                        HandleRemoveCommand(
                            mCommandProperties[mCurrentCommand]
                                .MyList[mCommandProperties[mCurrentCommand].MyList.Count - 1]
                        );

                        mCommandProperties[mCurrentCommand]
                            .MyList.RemoveAt(mCommandProperties[mCurrentCommand].MyList.Count - 1);
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

        private void HandleRemoveCommand(EventCommand cmd)
        {
            //If we cancelled an insert for a command that created additional command lists we need to remove those orphaned list(s)
            var branchesToRemove = new List<Guid>();
            switch (cmd.Type)
            {
                case EventCommandType.ShowOptions:
                    branchesToRemove.AddRange(((ShowOptionsCommand) cmd).BranchIds);

                    break;
                case EventCommandType.InputVariable:
                    branchesToRemove.AddRange(((InputVariableCommand) cmd).BranchIds);

                    break;
                case EventCommandType.ConditionalBranch:
                    branchesToRemove.AddRange(((ConditionalBranchCommand) cmd).BranchIds);

                    break;
                case EventCommandType.ChangeItems:
                    branchesToRemove.AddRange(((ChangeItemsCommand) cmd).BranchIds);

                    break;
                case EventCommandType.ChangeSpells:
                    branchesToRemove.AddRange(((ChangeSpellsCommand) cmd).BranchIds);

                    break;
                case EventCommandType.StartQuest:
                    branchesToRemove.AddRange(((StartQuestCommand) cmd).BranchIds);

                    break;
            }

            foreach (var branch in branchesToRemove)
            {
                CurrentPage.CommandLists.Remove(branch);
            }
        }

        /// <summary>
        ///     Opens the 'Add Command' window in order to insert a command at the select location in the command list.
        /// </summary>
        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (mCurrentCommand < 0 || mCurrentCommand >= mCommandProperties.Count)
            {
                return;
            }

            if (!mCommandProperties[mCurrentCommand].Editable)
            {
                return;
            }

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
            if (mCurrentCommand < 0 || mCurrentCommand >= mCommandProperties.Count)
            {
                return;
            }

            if (!mCommandProperties[mCurrentCommand].Editable)
            {
                return;
            }

            if (mCommandProperties[mCurrentCommand].MyIndex < 0 ||
                mCommandProperties[mCurrentCommand].MyIndex >= mCommandProperties[mCurrentCommand].MyList.Count)
            {
                return;
            }

            OpenEditCommand(mCommandProperties[mCurrentCommand].MyList[mCommandProperties[mCurrentCommand].MyIndex]);
            mIsEdit = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (mCurrentCommand < 0 || mCurrentCommand >= mCommandProperties.Count)
            {
                return;
            }

            if (!mCommandProperties[mCurrentCommand].Editable)
            {
                return;
            }

            if (mCommandProperties[mCurrentCommand].MyIndex < 0 ||
                mCommandProperties[mCurrentCommand].MyIndex >= mCommandProperties[mCurrentCommand].MyList.Count)
            {
                return;
            }

            HandleRemoveCommand(mCommandProperties[mCurrentCommand].Cmd);
            mCommandProperties[mCurrentCommand].MyList.Remove(mCommandProperties[mCurrentCommand].Cmd);
            mCurrentCommand = -1;
            ListPageCommands();
        }

        private void btnCut_Click(object sender, EventArgs e)
        {
            if (mCurrentCommand < 0 || mCurrentCommand >= mCommandProperties.Count)
            {
                return;
            }

            if (!mCommandProperties[mCurrentCommand].Editable)
            {
                return;
            }

            if (mCommandProperties[mCurrentCommand].MyIndex < 0 ||
                mCommandProperties[mCurrentCommand].MyIndex >= mCommandProperties[mCurrentCommand].MyList.Count)
            {
                return;
            }

            //Get a json representation of what we're cutting
            btnCopy_Click(sender, e);

            //Delete the command
            btnDelete_Click(sender, e);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (mCurrentCommand < 0 || mCurrentCommand >= mCommandProperties.Count)
            {
                return;
            }

            if (!mCommandProperties[mCurrentCommand].Editable)
            {
                return;
            }

            if (mCommandProperties[mCurrentCommand].MyIndex < 0 ||
                mCommandProperties[mCurrentCommand].MyIndex >= mCommandProperties[mCurrentCommand].MyList.Count)
            {
                return;
            }

            var copyLists = new Dictionary<Guid, List<EventCommand>>();
            mCopyData = mCommandProperties[mCurrentCommand]
                .MyList[mCommandProperties[mCurrentCommand].MyIndex]
                .GetCopyData(CurrentPage.CommandLists, copyLists);

            mCopyLists = JsonConvert.SerializeObject(
                copyLists,
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.None, TypeNameHandling = TypeNameHandling.Auto,
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                }
            );
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            if (mCurrentCommand < 0 || mCurrentCommand >= mCommandProperties.Count)
            {
                return;
            }

            if (!mCommandProperties[mCurrentCommand].Editable)
            {
                return;
            }

            if (mCopyData != null)
            {
                var newCmd = JsonConvert.DeserializeObject<EventCommand>(
                    mCopyData,
                    new JsonSerializerSettings()
                    {
                        Formatting = Formatting.None, TypeNameHandling = TypeNameHandling.Auto,
                        DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                        ObjectCreationHandling = ObjectCreationHandling.Replace
                    }
                );

                var lists = JsonConvert.DeserializeObject<Dictionary<Guid, List<EventCommand>>>(
                    mCopyLists,
                    new JsonSerializerSettings()
                    {
                        Formatting = Formatting.None, TypeNameHandling = TypeNameHandling.Auto,
                        DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                        ObjectCreationHandling = ObjectCreationHandling.Replace
                    }
                );

                var newListIds = new Dictionary<Guid, Guid>();
                foreach (var list in lists)
                {
                    newListIds.Add(list.Key, Guid.NewGuid());
                }

                newCmd.FixBranchIds(newListIds);
                foreach (var list in lists)
                {
                    foreach (var cmd in list.Value)
                    {
                        cmd.FixBranchIds(newListIds);
                    }
                }

                foreach (var list in lists)
                {
                    CurrentPage.CommandLists.Add(newListIds[list.Key], list.Value);
                }

                if (mCommandProperties[mCurrentCommand].Type == EventCommandType.Null)
                {
                    mCommandProperties[mCurrentCommand].MyList.Add(newCmd);
                }
                else
                {
                    mCommandProperties[mCurrentCommand]
                        .MyList.Insert(
                            mCommandProperties[mCurrentCommand].MyList.IndexOf(mCommandProperties[mCurrentCommand].Cmd),
                            newCmd
                        );
                }

                ListPageCommands();
            }
        }

        #endregion

        #region "Movement Options"

        private void cmbMoveType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage.Movement.Type = (EventMovementType) cmbMoveType.SelectedIndex;
            if (CurrentPage.Movement.Type == EventMovementType.MoveRoute)
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
            CurrentPage.Movement.Speed = (EventMovementSpeed) cmbEventSpeed.SelectedIndex;
        }

        private void cmbEventFreq_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage.Movement.Frequency = (EventMovementFrequency) cmbEventFreq.SelectedIndex;
        }

        private void btnSetRoute_Click(object sender, EventArgs e)
        {
            var moveRouteDesigner = new EventMoveRouteDesigner(this, mCurrentMap, MyEvent, CurrentPage.Movement.Route);
            Controls.Add(moveRouteDesigner);
            moveRouteDesigner.BringToFront();
            moveRouteDesigner.Size = ClientSize;
        }

        #endregion

        #region "Extra Options"

        private void chkWalkThrough_CheckedChanged(object sender, EventArgs e)
        {
            CurrentPage.Passable = chkWalkThrough.Checked;
        }

        private void cmbLayering_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage.Layer = (EventRenderLayer) cmbLayering.SelectedIndex;
        }

        private void cmbTrigger_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MyEvent.CommonEvent)
            {
                CurrentPage.CommonTrigger = (CommonEventTrigger) cmbTrigger.SelectedIndex;
            }
            else
            {
                CurrentPage.Trigger = (EventTrigger) cmbTrigger.SelectedIndex;
            }

            SetupTrigger();
        }

        private void SetupTrigger()
        {
            cmbTriggerVal.Hide();
            lblTriggerVal.Hide();
            txtCommand.Hide();
            lblCommand.Hide();
            lblVariableTrigger.Hide();
            cmbVariable.Hide();

            if (MyEvent.CommonEvent)
            {
                cmbVariable.Items.Clear();


                if (cmbTrigger.SelectedIndex == (int)CommonEventTrigger.SlashCommand)
                {
                    txtCommand.Show();
                    txtCommand.Text = CurrentPage.TriggerCommand;
                    lblCommand.Show();
                    lblCommand.Text = Strings.EventEditor.command;
                }
                else if (cmbTrigger.SelectedIndex == (int)CommonEventTrigger.PlayerVariableChange)
                {
                    cmbVariable.Show();
                    cmbVariable.Items.Add(Strings.General.none);
                    cmbVariable.Items.AddRange(PlayerVariableBase.Names);
                    cmbVariable.SelectedIndex = PlayerVariableBase.ListIndex(CurrentPage.TriggerId) + 1;
                }
                else if (cmbTrigger.SelectedIndex == (int)CommonEventTrigger.ServerVariableChange)
                {
                    cmbVariable.Show();
                    cmbVariable.Items.Add(Strings.General.none);
                    cmbVariable.Items.AddRange(ServerVariableBase.Names);
                    cmbVariable.SelectedIndex = ServerVariableBase.ListIndex(CurrentPage.TriggerId) + 1;
                }
                else if (cmbTrigger.SelectedIndex == (int)CommonEventTrigger.GuildVariableChange)
                {
                    cmbVariable.Show();
                    cmbVariable.Items.Add(Strings.General.none);
                    cmbVariable.Items.AddRange(GuildVariableBase.Names);
                    cmbVariable.SelectedIndex = GuildVariableBase.ListIndex(CurrentPage.TriggerId) + 1;
                }
            }
        }

        private void cmbVariable_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MyEvent.CommonEvent)
            {
                if (cmbTrigger.SelectedIndex == (int)CommonEventTrigger.PlayerVariableChange)
                {
                    CurrentPage.TriggerId = PlayerVariableBase.IdFromList(cmbVariable.SelectedIndex - 1);
                }
                else if (cmbTrigger.SelectedIndex == (int)CommonEventTrigger.ServerVariableChange)
                {
                    CurrentPage.TriggerId = ServerVariableBase.IdFromList(cmbVariable.SelectedIndex - 1);
                }
                else if (cmbTrigger.SelectedIndex == (int)CommonEventTrigger.GuildVariableChange)
                {
                    CurrentPage.TriggerId = GuildVariableBase.IdFromList(cmbVariable.SelectedIndex - 1);
                }
            }
        }

        private void chkHideName_CheckedChanged(object sender, EventArgs e)
        {
            CurrentPage.HideName = chkHideName.Checked;
        }

        private void chkDirectionFix_CheckedChanged(object sender, EventArgs e)
        {
            CurrentPage.DirectionFix = chkDirectionFix.Checked;
        }

        private void chkWalkingAnimation_CheckedChanged(object sender, EventArgs e)
        {
            CurrentPage.WalkingAnimation = chkWalkingAnimation.Checked;
        }

        private void chkInteractionFreeze_CheckedChanged(object sender, EventArgs e)
        {
            CurrentPage.InteractionFreeze = chkInteractionFreeze.Checked;
        }

        #endregion

        #region "Inspector Options"

        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            CurrentPage.Description = txtDesc.Text;
        }

        private void chkDisablePreview_CheckedChanged(object sender, EventArgs e)
        {
            CurrentPage.DisablePreview = chkDisableInspector.Checked;
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
            if (CurrentPage.DisablePreview || cmbPreviewFace.SelectedIndex < 1)
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

        public List<EventCommand> MyList;

        public EventCommandType Type = EventCommandType.Null;

    }

}
