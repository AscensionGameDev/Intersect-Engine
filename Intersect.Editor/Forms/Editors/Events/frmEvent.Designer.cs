using System.ComponentModel;
using System.Windows.Forms;
using DarkUI.Controls;
using Intersect.GameObjects.Events;

namespace Intersect.Editor.Forms.Editors.Events
{
    partial class FrmEvent
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new Container();
            var showTextTreeNode = new TreeNode("Show Text");
            var showOptionsTreeNode = new TreeNode("Show Options");
            var inputVariableTreeNode = new TreeNode("Input Variable");
            var addChatboxTextTreeNode = new TreeNode("Add Chatbox Text");
            var dialogueTreeNode = new TreeNode("Dialogue", new TreeNode[] { showTextTreeNode, showOptionsTreeNode, inputVariableTreeNode, addChatboxTextTreeNode });
            var setVariableTreeNode = new TreeNode("Set Variable");
            var setSelfSwitchTreeNode = new TreeNode("Set Self Switch");
            var conditionalBranchTreeNode = new TreeNode("Conditional Branch");
            var exitEventProcessTreeNode = new TreeNode("Exit Event Process");
            var labelTreeNode = new TreeNode("Label");
            var gotoLabelTreeNode = new TreeNode("Go To Label");
            var startCommonEventTreeNode = new TreeNode("Start Common Event");
            var logicFlowTreeNode = new TreeNode("Logic Flow", new TreeNode[] { setVariableTreeNode, setSelfSwitchTreeNode, conditionalBranchTreeNode, exitEventProcessTreeNode, labelTreeNode, gotoLabelTreeNode, startCommonEventTreeNode });
            var restoreHPTreeNode = new TreeNode("Restore HP");
            var restoreMPTreeNode = new TreeNode("Restore MP");
            var levelUpTreeNode = new TreeNode("Level Up");
            var giveExperienceTreeNode = new TreeNode("Give Experience");
            var changeLevelTreeNode = new TreeNode("Change Level");
            var changeSpellsTreeNode = new TreeNode("Change Spells");
            var changeItemsTreeNode = new TreeNode("Change Items");
            var changeSpriteTreeNode = new TreeNode("Change Sprite");
            var changePlayerColorTreeNode = new TreeNode("Change Player Color");
            var changeFaceTreeNode = new TreeNode("Change Face");
            var changeGenderTreeNode = new TreeNode("Change Gender");
            var setAccessTreeNode = new TreeNode("Set Access");
            var changeClassTreeNode = new TreeNode("Change Class");
            var equipUneqiupTreeNode = new TreeNode("Equip/Unequip Item");
            var changeNameColorTreeNode = new TreeNode("Change Name Color");
            var changePlayerLabelTreeNode = new TreeNode("Change Player Label");
            var changePlayerNameTreeNode = new TreeNode("Change Player Name");
            var resetStatPointTreeNode = new TreeNode("Reset Stat Point Allocations");
            var castSpellOnTreeNode = new TreeNode("Cast Spell On");
            var playerControlTreeNode = new TreeNode("Player Control", new TreeNode[] { restoreHPTreeNode, restoreMPTreeNode, levelUpTreeNode, giveExperienceTreeNode, changeLevelTreeNode, changeSpellsTreeNode, changeItemsTreeNode, changeSpriteTreeNode, changePlayerColorTreeNode, changeFaceTreeNode, changeGenderTreeNode, setAccessTreeNode, changeClassTreeNode, equipUneqiupTreeNode, changeNameColorTreeNode, changePlayerLabelTreeNode, changePlayerNameTreeNode, resetStatPointTreeNode, castSpellOnTreeNode });
            var warpPlayerTreeNode = new TreeNode("Warp Player");
            var setMoveRouteTreeNode = new TreeNode("Set Move Route");
            var waitForRouteCompleteTreeNode = new TreeNode("Wait for Route Completion");
            var holdPlayerTreeNode = new TreeNode("Hold Player");
            var releasePlayerTreeNode = new TreeNode("Release Player");
            var spawnNPCTreeNode = new TreeNode("Spawn NPC");
            var despawnNPCTreeNode = new TreeNode("Despawn NPC");
            var hidePlayerTreeNode = new TreeNode("Hide Player");
            var showPlayerTreeNode = new TreeNode("Show Player");
            var movementTreeNode = new TreeNode("Movement", new TreeNode[] { warpPlayerTreeNode, setMoveRouteTreeNode, waitForRouteCompleteTreeNode, holdPlayerTreeNode, releasePlayerTreeNode, spawnNPCTreeNode, despawnNPCTreeNode, hidePlayerTreeNode, showPlayerTreeNode });
            var playAnimationTreeNode = new TreeNode("Play Animation");
            var playBGMTreeNode = new TreeNode("Play BGM");
            var fadeoutBGMTreeNode = new TreeNode("Fadeout BGM");
            var playSoundTreeNode = new TreeNode("Play Sound");
            var stopSoundsTreeNode = new TreeNode("Stop Sounds");
            var showPictureTreeNode = new TreeNode("Show Picture");
            var hidePIctureTreeNode = new TreeNode("Hide Picture");
            var screenFadeTreeNode = new TreeNode("Screen Fade");
            var specialEffectsTreeNode = new TreeNode("Special Effects", new TreeNode[] { playAnimationTreeNode, playBGMTreeNode, fadeoutBGMTreeNode, playSoundTreeNode, stopSoundsTreeNode, showPictureTreeNode, hidePIctureTreeNode, screenFadeTreeNode });
            var startQuestTreeNode = new TreeNode("Start Quest");
            var completeQuestTreeNode = new TreeNode("Complete Quest Task");
            var endQuestTreeNode = new TreeNode("End Quest");
            var questControlTreeNode = new TreeNode("Quest Control", new TreeNode[] { startQuestTreeNode, completeQuestTreeNode, endQuestTreeNode });
            var waitTreeNode = new TreeNode("Wait...");
            var etcTreeNode = new TreeNode("Etc", new TreeNode[] { waitTreeNode });
            var openBankTreeNode = new TreeNode("Open Bank");
            var openShopTreeNode = new TreeNode("Open Shop");
            var openCraftingStationTreeNode = new TreeNode("Open Crafting Station");
            var shopAndBankTreeNode = new TreeNode("Shop and Bank", new TreeNode[] { openBankTreeNode, openShopTreeNode, openCraftingStationTreeNode });
            var createGuildTreeNode = new TreeNode("Create Guild");
            var disbandGuildTreeNode = new TreeNode("Disband Guild");
            var openGuildBankTreeNode = new TreeNode("Open Guild Bank");
            var setGuildBankSlotsTreeNode = new TreeNode("Set Guild Bank Slots Count");
            var guildsTreeNode = new TreeNode("Guilds", new TreeNode[] { createGuildTreeNode, disbandGuildTreeNode, openGuildBankTreeNode, setGuildBankSlotsTreeNode });
            lblName = new Label();
            txtEventname = new DarkTextBox();
            grpEntityOptions = new DarkGroupBox();
            grpExtra = new DarkGroupBox();
            chkIgnoreNpcAvoids = new DarkCheckBox();
            chkInteractionFreeze = new DarkCheckBox();
            chkWalkingAnimation = new DarkCheckBox();
            chkDirectionFix = new DarkCheckBox();
            chkHideName = new DarkCheckBox();
            chkWalkThrough = new DarkCheckBox();
            grpInspector = new DarkGroupBox();
            pnlFacePreview = new Panel();
            lblInspectorDesc = new Label();
            txtDesc = new DarkTextBox();
            chkDisableInspector = new DarkCheckBox();
            cmbPreviewFace = new DarkComboBox();
            lblFace = new Label();
            grpPreview = new DarkGroupBox();
            lblAnimation = new Label();
            cmbAnimation = new DarkComboBox();
            pnlPreview = new Panel();
            grpMovement = new DarkGroupBox();
            lblLayer = new Label();
            cmbLayering = new DarkComboBox();
            cmbEventFreq = new DarkComboBox();
            cmbEventSpeed = new DarkComboBox();
            lblFreq = new Label();
            lblSpeed = new Label();
            btnSetRoute = new DarkButton();
            lblType = new Label();
            cmbMoveType = new DarkComboBox();
            grpTriggers = new DarkGroupBox();
            lblVariableTrigger = new Label();
            lblCommand = new Label();
            lblTriggerVal = new Label();
            cmbTrigger = new DarkComboBox();
            cmbTriggerVal = new DarkComboBox();
            cmbVariable = new DarkComboBox();
            txtCommand = new DarkTextBox();
            grpEventConditions = new DarkGroupBox();
            btnEditConditions = new DarkButton();
            grpNewCommands = new DarkGroupBox();
            lblCloseCommands = new Label();
            lstCommands = new TreeView();
            grpEventCommands = new DarkGroupBox();
            lstEventCommands = new ListBox();
            grpCreateCommands = new DarkGroupBox();
            btnSave = new DarkButton();
            btnCancel = new DarkButton();
            commandMenu = new ContextMenuStrip(components);
            btnInsert = new ToolStripMenuItem();
            btnEdit = new ToolStripMenuItem();
            btnCut = new ToolStripMenuItem();
            btnCopy = new ToolStripMenuItem();
            btnPaste = new ToolStripMenuItem();
            btnDelete = new ToolStripMenuItem();
            grpPageOptions = new DarkGroupBox();
            btnClearPage = new DarkButton();
            btnDeletePage = new DarkButton();
            btnPastePage = new DarkButton();
            btnCopyPage = new DarkButton();
            btnNewPage = new DarkButton();
            grpGeneral = new DarkGroupBox();
            chkIsGlobal = new DarkCheckBox();
            pnlTabsContainer = new Panel();
            pnlTabs = new Panel();
            btnTabsLeft = new DarkButton();
            btnTabsRight = new DarkButton();
            pnlEditorComponents = new Panel();
            grpEntityOptions.SuspendLayout();
            grpExtra.SuspendLayout();
            grpInspector.SuspendLayout();
            grpPreview.SuspendLayout();
            grpMovement.SuspendLayout();
            grpTriggers.SuspendLayout();
            grpEventConditions.SuspendLayout();
            grpNewCommands.SuspendLayout();
            grpEventCommands.SuspendLayout();
            commandMenu.SuspendLayout();
            grpPageOptions.SuspendLayout();
            grpGeneral.SuspendLayout();
            pnlTabsContainer.SuspendLayout();
            pnlEditorComponents.SuspendLayout();
            SuspendLayout();
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Location = new System.Drawing.Point(6, 21);
            lblName.Margin = new Padding(4, 0, 4, 0);
            lblName.Name = "lblName";
            lblName.Size = new Size(42, 15);
            lblName.TabIndex = 1;
            lblName.Text = "Name:";
            // 
            // txtEventname
            // 
            txtEventname.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            txtEventname.BorderStyle = BorderStyle.FixedSingle;
            txtEventname.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            txtEventname.Location = new System.Drawing.Point(49, 19);
            txtEventname.Margin = new Padding(4, 3, 4, 3);
            txtEventname.Name = "txtEventname";
            txtEventname.Size = new Size(144, 23);
            txtEventname.TabIndex = 2;
            txtEventname.TextChanged += txtEventname_TextChanged;
            // 
            // grpEntityOptions
            // 
            grpEntityOptions.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpEntityOptions.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpEntityOptions.Controls.Add(grpExtra);
            grpEntityOptions.Controls.Add(grpInspector);
            grpEntityOptions.Controls.Add(grpPreview);
            grpEntityOptions.Controls.Add(grpMovement);
            grpEntityOptions.ForeColor = System.Drawing.Color.Gainsboro;
            grpEntityOptions.Location = new System.Drawing.Point(4, 69);
            grpEntityOptions.Margin = new Padding(4, 3, 4, 3);
            grpEntityOptions.Name = "grpEntityOptions";
            grpEntityOptions.Padding = new Padding(4, 3, 4, 3);
            grpEntityOptions.Size = new Size(380, 424);
            grpEntityOptions.TabIndex = 12;
            grpEntityOptions.TabStop = false;
            grpEntityOptions.Text = "Entity Options";
            // 
            // grpExtra
            // 
            grpExtra.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpExtra.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpExtra.Controls.Add(chkIgnoreNpcAvoids);
            grpExtra.Controls.Add(chkInteractionFreeze);
            grpExtra.Controls.Add(chkWalkingAnimation);
            grpExtra.Controls.Add(chkDirectionFix);
            grpExtra.Controls.Add(chkHideName);
            grpExtra.Controls.Add(chkWalkThrough);
            grpExtra.ForeColor = System.Drawing.Color.Gainsboro;
            grpExtra.Location = new System.Drawing.Point(7, 343);
            grpExtra.Margin = new Padding(4, 3, 4, 3);
            grpExtra.Name = "grpExtra";
            grpExtra.Padding = new Padding(4, 3, 4, 3);
            grpExtra.Size = new Size(368, 74);
            grpExtra.TabIndex = 9;
            grpExtra.TabStop = false;
            grpExtra.Text = "Extra";
            // 
            // chkIgnoreNpcAvoids
            // 
            chkIgnoreNpcAvoids.AutoSize = true;
            chkIgnoreNpcAvoids.Location = new System.Drawing.Point(144, 47);
            chkIgnoreNpcAvoids.Margin = new Padding(4, 3, 4, 3);
            chkIgnoreNpcAvoids.Name = "chkIgnoreNpcAvoids";
            chkIgnoreNpcAvoids.Size = new Size(124, 19);
            chkIgnoreNpcAvoids.TabIndex = 7;
            chkIgnoreNpcAvoids.Text = "Ignroe Npc Avoids";
            chkIgnoreNpcAvoids.CheckedChanged += chkIgnoreNpcAvoids_CheckedChanged;
            // 
            // chkInteractionFreeze
            // 
            chkInteractionFreeze.AutoSize = true;
            chkInteractionFreeze.Location = new System.Drawing.Point(7, 47);
            chkInteractionFreeze.Margin = new Padding(4, 3, 4, 3);
            chkInteractionFreeze.Name = "chkInteractionFreeze";
            chkInteractionFreeze.Size = new Size(119, 19);
            chkInteractionFreeze.TabIndex = 6;
            chkInteractionFreeze.Text = "Interaction Freeze";
            chkInteractionFreeze.CheckedChanged += chkInteractionFreeze_CheckedChanged;
            // 
            // chkWalkingAnimation
            // 
            chkWalkingAnimation.AutoSize = true;
            chkWalkingAnimation.Location = new System.Drawing.Point(250, 22);
            chkWalkingAnimation.Margin = new Padding(4, 3, 4, 3);
            chkWalkingAnimation.Name = "chkWalkingAnimation";
            chkWalkingAnimation.Size = new Size(101, 19);
            chkWalkingAnimation.TabIndex = 5;
            chkWalkingAnimation.Text = "Walking Anim";
            chkWalkingAnimation.CheckedChanged += chkWalkingAnimation_CheckedChanged;
            // 
            // chkDirectionFix
            // 
            chkDirectionFix.AutoSize = true;
            chkDirectionFix.Location = new System.Drawing.Point(182, 22);
            chkDirectionFix.Margin = new Padding(4, 3, 4, 3);
            chkDirectionFix.Name = "chkDirectionFix";
            chkDirectionFix.Size = new Size(59, 19);
            chkDirectionFix.TabIndex = 4;
            chkDirectionFix.Text = "Dir Fix";
            chkDirectionFix.CheckedChanged += chkDirectionFix_CheckedChanged;
            // 
            // chkHideName
            // 
            chkHideName.AutoSize = true;
            chkHideName.Location = new System.Drawing.Point(88, 22);
            chkHideName.Margin = new Padding(4, 3, 4, 3);
            chkHideName.Name = "chkHideName";
            chkHideName.Size = new Size(86, 19);
            chkHideName.TabIndex = 3;
            chkHideName.Text = "Hide Name";
            chkHideName.CheckedChanged += chkHideName_CheckedChanged;
            // 
            // chkWalkThrough
            // 
            chkWalkThrough.AutoSize = true;
            chkWalkThrough.Location = new System.Drawing.Point(7, 22);
            chkWalkThrough.Margin = new Padding(4, 3, 4, 3);
            chkWalkThrough.Name = "chkWalkThrough";
            chkWalkThrough.Size = new Size(71, 19);
            chkWalkThrough.TabIndex = 2;
            chkWalkThrough.Text = "Passable";
            chkWalkThrough.CheckedChanged += chkWalkThrough_CheckedChanged;
            // 
            // grpInspector
            // 
            grpInspector.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpInspector.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpInspector.Controls.Add(pnlFacePreview);
            grpInspector.Controls.Add(lblInspectorDesc);
            grpInspector.Controls.Add(txtDesc);
            grpInspector.Controls.Add(chkDisableInspector);
            grpInspector.Controls.Add(cmbPreviewFace);
            grpInspector.Controls.Add(lblFace);
            grpInspector.ForeColor = System.Drawing.Color.Gainsboro;
            grpInspector.Location = new System.Drawing.Point(7, 207);
            grpInspector.Margin = new Padding(4, 3, 4, 3);
            grpInspector.Name = "grpInspector";
            grpInspector.Padding = new Padding(4, 3, 4, 3);
            grpInspector.Size = new Size(369, 135);
            grpInspector.TabIndex = 7;
            grpInspector.TabStop = false;
            grpInspector.Text = "Entity Inspector Options";
            // 
            // pnlFacePreview
            // 
            pnlFacePreview.BackgroundImageLayout = ImageLayout.Stretch;
            pnlFacePreview.Location = new System.Drawing.Point(10, 53);
            pnlFacePreview.Margin = new Padding(4, 3, 4, 3);
            pnlFacePreview.Name = "pnlFacePreview";
            pnlFacePreview.Size = new Size(75, 74);
            pnlFacePreview.TabIndex = 12;
            // 
            // lblInspectorDesc
            // 
            lblInspectorDesc.Location = new System.Drawing.Point(92, 48);
            lblInspectorDesc.Margin = new Padding(4, 0, 4, 0);
            lblInspectorDesc.Name = "lblInspectorDesc";
            lblInspectorDesc.Size = new Size(131, 22);
            lblInspectorDesc.TabIndex = 11;
            lblInspectorDesc.Text = "Inspector Description:";
            // 
            // txtDesc
            // 
            txtDesc.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            txtDesc.BorderStyle = BorderStyle.FixedSingle;
            txtDesc.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            txtDesc.Location = new System.Drawing.Point(92, 70);
            txtDesc.Margin = new Padding(4, 3, 4, 3);
            txtDesc.Multiline = true;
            txtDesc.Name = "txtDesc";
            txtDesc.Size = new Size(269, 57);
            txtDesc.TabIndex = 0;
            txtDesc.TextChanged += txtDesc_TextChanged;
            // 
            // chkDisableInspector
            // 
            chkDisableInspector.Location = new System.Drawing.Point(238, 17);
            chkDisableInspector.Margin = new Padding(4, 3, 4, 3);
            chkDisableInspector.Name = "chkDisableInspector";
            chkDisableInspector.Size = new Size(125, 24);
            chkDisableInspector.TabIndex = 4;
            chkDisableInspector.Text = "Disable Inspector";
            chkDisableInspector.CheckedChanged += chkDisablePreview_CheckedChanged;
            // 
            // cmbPreviewFace
            // 
            cmbPreviewFace.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbPreviewFace.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbPreviewFace.BorderStyle = ButtonBorderStyle.Solid;
            cmbPreviewFace.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbPreviewFace.DrawDropdownHoverOutline = false;
            cmbPreviewFace.DrawFocusRectangle = false;
            cmbPreviewFace.DrawMode = DrawMode.OwnerDrawFixed;
            cmbPreviewFace.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPreviewFace.FlatStyle = FlatStyle.Flat;
            cmbPreviewFace.ForeColor = System.Drawing.Color.Gainsboro;
            cmbPreviewFace.FormattingEnabled = true;
            cmbPreviewFace.Location = new System.Drawing.Point(54, 17);
            cmbPreviewFace.Margin = new Padding(4, 3, 4, 3);
            cmbPreviewFace.Name = "cmbPreviewFace";
            cmbPreviewFace.Size = new Size(132, 24);
            cmbPreviewFace.TabIndex = 10;
            cmbPreviewFace.Text = null;
            cmbPreviewFace.TextPadding = new Padding(2);
            cmbPreviewFace.SelectedIndexChanged += cmbPreviewFace_SelectedIndexChanged;
            // 
            // lblFace
            // 
            lblFace.AutoSize = true;
            lblFace.Location = new System.Drawing.Point(7, 21);
            lblFace.Margin = new Padding(4, 0, 4, 0);
            lblFace.Name = "lblFace";
            lblFace.Size = new Size(34, 15);
            lblFace.TabIndex = 9;
            lblFace.Text = "Face:";
            // 
            // grpPreview
            // 
            grpPreview.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpPreview.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpPreview.Controls.Add(lblAnimation);
            grpPreview.Controls.Add(cmbAnimation);
            grpPreview.Controls.Add(pnlPreview);
            grpPreview.ForeColor = System.Drawing.Color.Gainsboro;
            grpPreview.Location = new System.Drawing.Point(7, 15);
            grpPreview.Margin = new Padding(4, 3, 4, 3);
            grpPreview.Name = "grpPreview";
            grpPreview.Padding = new Padding(4, 3, 4, 3);
            grpPreview.Size = new Size(187, 188);
            grpPreview.TabIndex = 10;
            grpPreview.TabStop = false;
            grpPreview.Text = "Preview";
            // 
            // lblAnimation
            // 
            lblAnimation.AutoSize = true;
            lblAnimation.Location = new System.Drawing.Point(5, 134);
            lblAnimation.Margin = new Padding(4, 0, 4, 0);
            lblAnimation.Name = "lblAnimation";
            lblAnimation.Size = new Size(66, 15);
            lblAnimation.TabIndex = 2;
            lblAnimation.Text = "Animation:";
            // 
            // cmbAnimation
            // 
            cmbAnimation.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbAnimation.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbAnimation.BorderStyle = ButtonBorderStyle.Solid;
            cmbAnimation.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbAnimation.DrawDropdownHoverOutline = false;
            cmbAnimation.DrawFocusRectangle = false;
            cmbAnimation.DrawMode = DrawMode.OwnerDrawFixed;
            cmbAnimation.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAnimation.FlatStyle = FlatStyle.Flat;
            cmbAnimation.ForeColor = System.Drawing.Color.Gainsboro;
            cmbAnimation.FormattingEnabled = true;
            cmbAnimation.Location = new System.Drawing.Point(23, 152);
            cmbAnimation.Margin = new Padding(4, 3, 4, 3);
            cmbAnimation.Name = "cmbAnimation";
            cmbAnimation.Size = new Size(145, 24);
            cmbAnimation.TabIndex = 1;
            cmbAnimation.Text = null;
            cmbAnimation.TextPadding = new Padding(2);
            cmbAnimation.SelectedIndexChanged += cmbAnimation_SelectedIndexChanged;
            // 
            // pnlPreview
            // 
            pnlPreview.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            pnlPreview.BorderStyle = BorderStyle.FixedSingle;
            pnlPreview.Location = new System.Drawing.Point(38, 16);
            pnlPreview.Margin = new Padding(4, 3, 4, 3);
            pnlPreview.Name = "pnlPreview";
            pnlPreview.Size = new Size(112, 110);
            pnlPreview.TabIndex = 0;
            pnlPreview.DoubleClick += pnlPreview_DoubleClick;
            // 
            // grpMovement
            // 
            grpMovement.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpMovement.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpMovement.Controls.Add(lblLayer);
            grpMovement.Controls.Add(cmbLayering);
            grpMovement.Controls.Add(cmbEventFreq);
            grpMovement.Controls.Add(cmbEventSpeed);
            grpMovement.Controls.Add(lblFreq);
            grpMovement.Controls.Add(lblSpeed);
            grpMovement.Controls.Add(btnSetRoute);
            grpMovement.Controls.Add(lblType);
            grpMovement.Controls.Add(cmbMoveType);
            grpMovement.ForeColor = System.Drawing.Color.Gainsboro;
            grpMovement.Location = new System.Drawing.Point(197, 15);
            grpMovement.Margin = new Padding(4, 3, 4, 3);
            grpMovement.Name = "grpMovement";
            grpMovement.Padding = new Padding(4, 3, 4, 3);
            grpMovement.Size = new Size(180, 188);
            grpMovement.TabIndex = 8;
            grpMovement.TabStop = false;
            grpMovement.Text = "Movement";
            // 
            // lblLayer
            // 
            lblLayer.AutoSize = true;
            lblLayer.Location = new System.Drawing.Point(7, 155);
            lblLayer.Margin = new Padding(4, 0, 4, 0);
            lblLayer.Name = "lblLayer";
            lblLayer.Size = new Size(38, 15);
            lblLayer.TabIndex = 7;
            lblLayer.Text = "Layer:";
            // 
            // cmbLayering
            // 
            cmbLayering.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbLayering.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbLayering.BorderStyle = ButtonBorderStyle.Solid;
            cmbLayering.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbLayering.DrawDropdownHoverOutline = false;
            cmbLayering.DrawFocusRectangle = false;
            cmbLayering.DrawMode = DrawMode.OwnerDrawFixed;
            cmbLayering.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLayering.FlatStyle = FlatStyle.Flat;
            cmbLayering.ForeColor = System.Drawing.Color.Gainsboro;
            cmbLayering.FormattingEnabled = true;
            cmbLayering.Items.AddRange(new object[] { "Below Player", "Same as Player", "Above Player" });
            cmbLayering.Location = new System.Drawing.Point(56, 151);
            cmbLayering.Margin = new Padding(4, 3, 4, 3);
            cmbLayering.Name = "cmbLayering";
            cmbLayering.Size = new Size(117, 24);
            cmbLayering.TabIndex = 1;
            cmbLayering.Text = "Below Player";
            cmbLayering.TextPadding = new Padding(2);
            cmbLayering.SelectedIndexChanged += cmbLayering_SelectedIndexChanged;
            // 
            // cmbEventFreq
            // 
            cmbEventFreq.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbEventFreq.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbEventFreq.BorderStyle = ButtonBorderStyle.Solid;
            cmbEventFreq.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbEventFreq.DrawDropdownHoverOutline = false;
            cmbEventFreq.DrawFocusRectangle = false;
            cmbEventFreq.DrawMode = DrawMode.OwnerDrawFixed;
            cmbEventFreq.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEventFreq.FlatStyle = FlatStyle.Flat;
            cmbEventFreq.ForeColor = System.Drawing.Color.Gainsboro;
            cmbEventFreq.FormattingEnabled = true;
            cmbEventFreq.Items.AddRange(new object[] { "Not Very Often", "Not Often", "Normal", "Often", "Very Often" });
            cmbEventFreq.Location = new System.Drawing.Point(56, 120);
            cmbEventFreq.Margin = new Padding(4, 3, 4, 3);
            cmbEventFreq.Name = "cmbEventFreq";
            cmbEventFreq.Size = new Size(116, 24);
            cmbEventFreq.TabIndex = 6;
            cmbEventFreq.Text = "Not Very Often";
            cmbEventFreq.TextPadding = new Padding(2);
            cmbEventFreq.SelectedIndexChanged += cmbEventFreq_SelectedIndexChanged;
            // 
            // cmbEventSpeed
            // 
            cmbEventSpeed.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbEventSpeed.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbEventSpeed.BorderStyle = ButtonBorderStyle.Solid;
            cmbEventSpeed.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbEventSpeed.DrawDropdownHoverOutline = false;
            cmbEventSpeed.DrawFocusRectangle = false;
            cmbEventSpeed.DrawMode = DrawMode.OwnerDrawFixed;
            cmbEventSpeed.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEventSpeed.FlatStyle = FlatStyle.Flat;
            cmbEventSpeed.ForeColor = System.Drawing.Color.Gainsboro;
            cmbEventSpeed.FormattingEnabled = true;
            cmbEventSpeed.Items.AddRange(new object[] { "Slowest", "Slower", "Normal", "Faster", "Fastest" });
            cmbEventSpeed.Location = new System.Drawing.Point(56, 89);
            cmbEventSpeed.Margin = new Padding(4, 3, 4, 3);
            cmbEventSpeed.Name = "cmbEventSpeed";
            cmbEventSpeed.Size = new Size(116, 24);
            cmbEventSpeed.TabIndex = 5;
            cmbEventSpeed.Text = "Slowest";
            cmbEventSpeed.TextPadding = new Padding(2);
            cmbEventSpeed.SelectedIndexChanged += cmbEventSpeed_SelectedIndexChanged;
            // 
            // lblFreq
            // 
            lblFreq.AutoSize = true;
            lblFreq.Location = new System.Drawing.Point(7, 123);
            lblFreq.Margin = new Padding(4, 0, 4, 0);
            lblFreq.Name = "lblFreq";
            lblFreq.Size = new Size(33, 15);
            lblFreq.TabIndex = 4;
            lblFreq.Text = "Freq:";
            // 
            // lblSpeed
            // 
            lblSpeed.AutoSize = true;
            lblSpeed.Location = new System.Drawing.Point(7, 92);
            lblSpeed.Margin = new Padding(4, 0, 4, 0);
            lblSpeed.Name = "lblSpeed";
            lblSpeed.Size = new Size(42, 15);
            lblSpeed.TabIndex = 3;
            lblSpeed.Text = "Speed:";
            // 
            // btnSetRoute
            // 
            btnSetRoute.Enabled = false;
            btnSetRoute.Location = new System.Drawing.Point(85, 50);
            btnSetRoute.Margin = new Padding(4, 3, 4, 3);
            btnSetRoute.Name = "btnSetRoute";
            btnSetRoute.Padding = new Padding(6);
            btnSetRoute.Size = new Size(88, 27);
            btnSetRoute.TabIndex = 2;
            btnSetRoute.Text = "Set Route....";
            btnSetRoute.Click += btnSetRoute_Click;
            // 
            // lblType
            // 
            lblType.AutoSize = true;
            lblType.Location = new System.Drawing.Point(7, 25);
            lblType.Margin = new Padding(4, 0, 4, 0);
            lblType.Name = "lblType";
            lblType.Size = new Size(34, 15);
            lblType.TabIndex = 1;
            lblType.Text = "Type:";
            // 
            // cmbMoveType
            // 
            cmbMoveType.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbMoveType.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbMoveType.BorderStyle = ButtonBorderStyle.Solid;
            cmbMoveType.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbMoveType.DrawDropdownHoverOutline = false;
            cmbMoveType.DrawFocusRectangle = false;
            cmbMoveType.DrawMode = DrawMode.OwnerDrawFixed;
            cmbMoveType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMoveType.FlatStyle = FlatStyle.Flat;
            cmbMoveType.ForeColor = System.Drawing.Color.Gainsboro;
            cmbMoveType.FormattingEnabled = true;
            cmbMoveType.Items.AddRange(new object[] { "None", "Random", "Move Route" });
            cmbMoveType.Location = new System.Drawing.Point(56, 22);
            cmbMoveType.Margin = new Padding(4, 3, 4, 3);
            cmbMoveType.Name = "cmbMoveType";
            cmbMoveType.Size = new Size(116, 24);
            cmbMoveType.TabIndex = 0;
            cmbMoveType.Text = "None";
            cmbMoveType.TextPadding = new Padding(2);
            cmbMoveType.SelectedIndexChanged += cmbMoveType_SelectedIndexChanged;
            // 
            // grpTriggers
            // 
            grpTriggers.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpTriggers.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpTriggers.Controls.Add(lblVariableTrigger);
            grpTriggers.Controls.Add(lblCommand);
            grpTriggers.Controls.Add(lblTriggerVal);
            grpTriggers.Controls.Add(cmbTrigger);
            grpTriggers.Controls.Add(cmbTriggerVal);
            grpTriggers.Controls.Add(cmbVariable);
            grpTriggers.Controls.Add(txtCommand);
            grpTriggers.ForeColor = System.Drawing.Color.Gainsboro;
            grpTriggers.Location = new System.Drawing.Point(4, 499);
            grpTriggers.Margin = new Padding(4, 3, 4, 3);
            grpTriggers.Name = "grpTriggers";
            grpTriggers.Padding = new Padding(4, 3, 4, 3);
            grpTriggers.Size = new Size(380, 87);
            grpTriggers.TabIndex = 21;
            grpTriggers.TabStop = false;
            grpTriggers.Text = "Trigger";
            // 
            // lblVariableTrigger
            // 
            lblVariableTrigger.AutoSize = true;
            lblVariableTrigger.Location = new System.Drawing.Point(8, 55);
            lblVariableTrigger.Margin = new Padding(4, 0, 4, 0);
            lblVariableTrigger.Name = "lblVariableTrigger";
            lblVariableTrigger.Size = new Size(51, 15);
            lblVariableTrigger.TabIndex = 13;
            lblVariableTrigger.Text = "Variable:";
            lblVariableTrigger.Visible = false;
            // 
            // lblCommand
            // 
            lblCommand.AutoSize = true;
            lblCommand.Location = new System.Drawing.Point(8, 55);
            lblCommand.Margin = new Padding(4, 0, 4, 0);
            lblCommand.Name = "lblCommand";
            lblCommand.Size = new Size(80, 15);
            lblCommand.TabIndex = 11;
            lblCommand.Text = "/Command: /";
            lblCommand.Visible = false;
            // 
            // lblTriggerVal
            // 
            lblTriggerVal.AutoSize = true;
            lblTriggerVal.Location = new System.Drawing.Point(8, 55);
            lblTriggerVal.Margin = new Padding(4, 0, 4, 0);
            lblTriggerVal.Name = "lblTriggerVal";
            lblTriggerVal.Size = new Size(59, 15);
            lblTriggerVal.TabIndex = 10;
            lblTriggerVal.Text = "Projectile:";
            lblTriggerVal.Visible = false;
            // 
            // cmbTrigger
            // 
            cmbTrigger.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbTrigger.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbTrigger.BorderStyle = ButtonBorderStyle.Solid;
            cmbTrigger.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbTrigger.DrawDropdownHoverOutline = false;
            cmbTrigger.DrawFocusRectangle = false;
            cmbTrigger.DrawMode = DrawMode.OwnerDrawFixed;
            cmbTrigger.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTrigger.FlatStyle = FlatStyle.Flat;
            cmbTrigger.ForeColor = System.Drawing.Color.Gainsboro;
            cmbTrigger.FormattingEnabled = true;
            cmbTrigger.Items.AddRange(new object[] { "Action Button", "Player Touch", "Autorun", "Projectile Hit" });
            cmbTrigger.Location = new System.Drawing.Point(7, 20);
            cmbTrigger.Margin = new Padding(4, 3, 4, 3);
            cmbTrigger.Name = "cmbTrigger";
            cmbTrigger.Size = new Size(350, 24);
            cmbTrigger.TabIndex = 2;
            cmbTrigger.Text = "Action Button";
            cmbTrigger.TextPadding = new Padding(2);
            cmbTrigger.SelectedIndexChanged += cmbTrigger_SelectedIndexChanged;
            // 
            // cmbTriggerVal
            // 
            cmbTriggerVal.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbTriggerVal.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbTriggerVal.BorderStyle = ButtonBorderStyle.Solid;
            cmbTriggerVal.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbTriggerVal.DrawDropdownHoverOutline = false;
            cmbTriggerVal.DrawFocusRectangle = false;
            cmbTriggerVal.DrawMode = DrawMode.OwnerDrawFixed;
            cmbTriggerVal.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTriggerVal.FlatStyle = FlatStyle.Flat;
            cmbTriggerVal.ForeColor = System.Drawing.Color.Gainsboro;
            cmbTriggerVal.FormattingEnabled = true;
            cmbTriggerVal.Items.AddRange(new object[] { "None" });
            cmbTriggerVal.Location = new System.Drawing.Point(98, 51);
            cmbTriggerVal.Margin = new Padding(4, 3, 4, 3);
            cmbTriggerVal.Name = "cmbTriggerVal";
            cmbTriggerVal.Size = new Size(259, 24);
            cmbTriggerVal.TabIndex = 9;
            cmbTriggerVal.Text = "None";
            cmbTriggerVal.TextPadding = new Padding(2);
            cmbTriggerVal.Visible = false;
            // 
            // cmbVariable
            // 
            cmbVariable.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbVariable.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbVariable.BorderStyle = ButtonBorderStyle.Solid;
            cmbVariable.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbVariable.DrawDropdownHoverOutline = false;
            cmbVariable.DrawFocusRectangle = false;
            cmbVariable.DrawMode = DrawMode.OwnerDrawFixed;
            cmbVariable.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbVariable.FlatStyle = FlatStyle.Flat;
            cmbVariable.ForeColor = System.Drawing.Color.Gainsboro;
            cmbVariable.FormattingEnabled = true;
            cmbVariable.Items.AddRange(new object[] { "None" });
            cmbVariable.Location = new System.Drawing.Point(98, 51);
            cmbVariable.Margin = new Padding(4, 3, 4, 3);
            cmbVariable.Name = "cmbVariable";
            cmbVariable.Size = new Size(259, 24);
            cmbVariable.TabIndex = 14;
            cmbVariable.Text = "None";
            cmbVariable.TextPadding = new Padding(2);
            cmbVariable.SelectedIndexChanged += cmbVariable_SelectedIndexChanged;
            // 
            // txtCommand
            // 
            txtCommand.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            txtCommand.BorderStyle = BorderStyle.FixedSingle;
            txtCommand.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            txtCommand.Location = new System.Drawing.Point(98, 51);
            txtCommand.Margin = new Padding(4, 3, 4, 3);
            txtCommand.Name = "txtCommand";
            txtCommand.Size = new Size(260, 23);
            txtCommand.TabIndex = 12;
            txtCommand.Visible = false;
            txtCommand.TextChanged += txtCommand_TextChanged;
            // 
            // grpEventConditions
            // 
            grpEventConditions.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpEventConditions.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpEventConditions.Controls.Add(btnEditConditions);
            grpEventConditions.ForeColor = System.Drawing.Color.Gainsboro;
            grpEventConditions.Location = new System.Drawing.Point(4, 2);
            grpEventConditions.Margin = new Padding(4, 3, 4, 3);
            grpEventConditions.Name = "grpEventConditions";
            grpEventConditions.Padding = new Padding(4, 3, 4, 3);
            grpEventConditions.Size = new Size(380, 63);
            grpEventConditions.TabIndex = 5;
            grpEventConditions.TabStop = false;
            grpEventConditions.Text = "Conditions";
            // 
            // btnEditConditions
            // 
            btnEditConditions.Location = new System.Drawing.Point(8, 23);
            btnEditConditions.Margin = new Padding(4, 3, 4, 3);
            btnEditConditions.Name = "btnEditConditions";
            btnEditConditions.Padding = new Padding(6);
            btnEditConditions.Size = new Size(355, 27);
            btnEditConditions.TabIndex = 0;
            btnEditConditions.Text = "Spawn/Execution Conditions";
            btnEditConditions.Click += btnEditConditions_Click;
            // 
            // grpNewCommands
            // 
            grpNewCommands.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpNewCommands.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpNewCommands.Controls.Add(lblCloseCommands);
            grpNewCommands.Controls.Add(lstCommands);
            grpNewCommands.ForeColor = System.Drawing.Color.Gainsboro;
            grpNewCommands.Location = new System.Drawing.Point(389, 0);
            grpNewCommands.Margin = new Padding(4, 3, 4, 3);
            grpNewCommands.Name = "grpNewCommands";
            grpNewCommands.Padding = new Padding(4, 3, 4, 3);
            grpNewCommands.Size = new Size(515, 580);
            grpNewCommands.TabIndex = 7;
            grpNewCommands.TabStop = false;
            grpNewCommands.Text = "Add Commands";
            grpNewCommands.Visible = false;
            // 
            // lblCloseCommands
            // 
            lblCloseCommands.AutoSize = true;
            lblCloseCommands.Location = new System.Drawing.Point(490, 16);
            lblCloseCommands.Margin = new Padding(4, 0, 4, 0);
            lblCloseCommands.Name = "lblCloseCommands";
            lblCloseCommands.Size = new Size(14, 15);
            lblCloseCommands.TabIndex = 1;
            lblCloseCommands.Text = "X";
            lblCloseCommands.Click += lblCloseCommands_Click;
            // 
            // lstCommands
            // 
            lstCommands.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            lstCommands.BorderStyle = BorderStyle.FixedSingle;
            lstCommands.ForeColor = System.Drawing.Color.Gainsboro;
            lstCommands.LineColor = System.Drawing.Color.FromArgb(150, 150, 150);
            lstCommands.Location = new System.Drawing.Point(7, 37);
            lstCommands.Margin = new Padding(4, 3, 4, 3);
            lstCommands.Name = "lstCommands";
            showTextTreeNode.Name = "showtext";
            showTextTreeNode.Tag = (int)EventCommandType.ShowText;
            showTextTreeNode.Text = "Show Text";
            showOptionsTreeNode.Name = "showoptions";
            showOptionsTreeNode.Tag = (int)EventCommandType.ShowOptions;
            showOptionsTreeNode.Text = "Show Options";
            inputVariableTreeNode.Name = "inputvariable";
            inputVariableTreeNode.Tag = (int)EventCommandType.InputVariable;
            inputVariableTreeNode.Text = "Input Variable";
            addChatboxTextTreeNode.Name = "addchatboxtext";
            addChatboxTextTreeNode.Tag = (int)EventCommandType.AddChatboxText;
            addChatboxTextTreeNode.Text = "Add Chatbox Text";
            dialogueTreeNode.Name = "dialogue";
            dialogueTreeNode.Text = "Dialogue";
            setVariableTreeNode.Name = "setvariable";
            setVariableTreeNode.Tag = (int)EventCommandType.SetVariable;
            setVariableTreeNode.Text = "Set Variable";
            setSelfSwitchTreeNode.Name = "setselfswitch";
            setSelfSwitchTreeNode.Tag = (int)EventCommandType.SetSelfSwitch;
            setSelfSwitchTreeNode.Text = "Set Self Switch";
            conditionalBranchTreeNode.Name = "conditionalbranch";
            conditionalBranchTreeNode.Tag = (int)EventCommandType.ConditionalBranch;
            conditionalBranchTreeNode.Text = "Conditional Branch";
            exitEventProcessTreeNode.Name = "exiteventprocess";
            exitEventProcessTreeNode.Tag = (int)EventCommandType.ExitEventProcess;
            exitEventProcessTreeNode.Text = "Exit Event Process";
            labelTreeNode.Name = "label";
            labelTreeNode.Tag = (int)EventCommandType.Label;
            labelTreeNode.Text = "Label";
            gotoLabelTreeNode.Name = "gotolabel";
            gotoLabelTreeNode.Tag = (int)EventCommandType.GoToLabel;
            gotoLabelTreeNode.Text = "Go To Label";
            startCommonEventTreeNode.Name = "startcommonevent";
            startCommonEventTreeNode.Tag = (int)EventCommandType.StartCommonEvent;
            startCommonEventTreeNode.Text = "Start Common Event";
            logicFlowTreeNode.Name = "logicflow";
            logicFlowTreeNode.Text = "Logic Flow";
            restoreHPTreeNode.Name = "restorehp";
            restoreHPTreeNode.Tag = (int)EventCommandType.RestoreHp;
            restoreHPTreeNode.Text = "Restore HP";
            restoreMPTreeNode.Name = "restoremp";
            restoreMPTreeNode.Tag = (int)EventCommandType.RestoreMp;
            restoreMPTreeNode.Text = "Restore MP";
            levelUpTreeNode.Name = "levelup";
            levelUpTreeNode.Tag = (int)EventCommandType.LevelUp;
            levelUpTreeNode.Text = "Level Up";
            giveExperienceTreeNode.Name = "giveexperience";
            giveExperienceTreeNode.Tag = (int)EventCommandType.GiveExperience;
            giveExperienceTreeNode.Text = "Give Experience";
            changeLevelTreeNode.Name = "changelevel";
            changeLevelTreeNode.Tag = (int)EventCommandType.ChangeLevel;
            changeLevelTreeNode.Text = "Change Level";
            changeSpellsTreeNode.Name = "changespells";
            changeSpellsTreeNode.Tag = (int)EventCommandType.ChangeSpells;
            changeSpellsTreeNode.Text = "Change Spells";
            changeItemsTreeNode.Name = "changeitems";
            changeItemsTreeNode.Tag = (int)EventCommandType.ChangeItems;
            changeItemsTreeNode.Text = "Change Items";
            changeSpriteTreeNode.Name = "changesprite";
            changeSpriteTreeNode.Tag = (int)EventCommandType.ChangeSprite;
            changeSpriteTreeNode.Text = "Change Sprite";
            changePlayerColorTreeNode.Name = "changeplayercolor";
            changePlayerColorTreeNode.Tag = (int)EventCommandType.ChangePlayerColor;
            changePlayerColorTreeNode.Text = "Change Player Color";
            changeFaceTreeNode.Name = "changeface";
            changeFaceTreeNode.Tag = (int)EventCommandType.ChangeFace;
            changeFaceTreeNode.Text = "Change Face";
            changeGenderTreeNode.Name = "changegender";
            changeGenderTreeNode.Tag = (int)EventCommandType.ChangeGender;
            changeGenderTreeNode.Text = "Change Gender";
            setAccessTreeNode.Name = "setaccess";
            setAccessTreeNode.Tag = (int)EventCommandType.SetAccess;
            setAccessTreeNode.Text = "Set Access";
            changeClassTreeNode.Name = "changeclass";
            changeClassTreeNode.Tag = (int)EventCommandType.SetClass;
            changeClassTreeNode.Text = "Change Class";
            equipUneqiupTreeNode.Name = "equipitem";
            equipUneqiupTreeNode.Tag = (int)EventCommandType.EquipItem;
            equipUneqiupTreeNode.Text = "Equip/Unequip Item";
            changeNameColorTreeNode.Name = "changenamecolor";
            changeNameColorTreeNode.Tag = (int)EventCommandType.ChangeNameColor;
            changeNameColorTreeNode.Text = "Change Name Color";
            changePlayerLabelTreeNode.Name = "changeplayerlabel";
            changePlayerLabelTreeNode.Tag = (int)EventCommandType.PlayerLabel;
            changePlayerLabelTreeNode.Text = "Change Player Label";
            changePlayerNameTreeNode.Name = "changename";
            changePlayerNameTreeNode.Tag = (int)EventCommandType.ChangeName;
            changePlayerNameTreeNode.Text = "Change Player Name";
            resetStatPointTreeNode.Name = "resetstatallocations";
            resetStatPointTreeNode.Tag = (int)EventCommandType.ResetStatPointAllocations;
            resetStatPointTreeNode.Text = "Reset Stat Point Allocations";
            castSpellOnTreeNode.Name = "castspellon";
            castSpellOnTreeNode.Tag = (int)EventCommandType.CastSpellOn;
            castSpellOnTreeNode.Text = "Cast Spell On";
            playerControlTreeNode.Name = "playercontrol";
            playerControlTreeNode.Text = "Player Control";
            warpPlayerTreeNode.Name = "warpplayer";
            warpPlayerTreeNode.Tag = (int)EventCommandType.WarpPlayer;
            warpPlayerTreeNode.Text = "Warp Player";
            setMoveRouteTreeNode.Name = "setmoveroute";
            setMoveRouteTreeNode.Tag = (int)EventCommandType.SetMoveRoute;
            setMoveRouteTreeNode.Text = "Set Move Route";
            waitForRouteCompleteTreeNode.Name = "waitmoveroute";
            waitForRouteCompleteTreeNode.Tag = (int)EventCommandType.WaitForRouteCompletion;
            waitForRouteCompleteTreeNode.Text = "Wait for Route Completion";
            holdPlayerTreeNode.Name = "holdplayer";
            holdPlayerTreeNode.Tag = (int)EventCommandType.HoldPlayer;
            holdPlayerTreeNode.Text = "Hold Player";
            releasePlayerTreeNode.Name = "releaseplayer";
            releasePlayerTreeNode.Tag = (int)EventCommandType.ReleasePlayer;
            releasePlayerTreeNode.Text = "Release Player";
            spawnNPCTreeNode.Name = "spawnnpc";
            spawnNPCTreeNode.Tag = (int)EventCommandType.SpawnNpc;
            spawnNPCTreeNode.Text = "Spawn NPC";
            despawnNPCTreeNode.Name = "despawnnpcs";
            despawnNPCTreeNode.Tag = (int)EventCommandType.DespawnNpc;
            despawnNPCTreeNode.Text = "Despawn NPC";
            hidePlayerTreeNode.Name = "hideplayer";
            hidePlayerTreeNode.Tag = (int)EventCommandType.HidePlayer;
            hidePlayerTreeNode.Text = "Hide Player";
            showPlayerTreeNode.Name = "showplayer";
            showPlayerTreeNode.Tag = (int)EventCommandType.ShowPlayer;
            showPlayerTreeNode.Text = "Show Player";
            movementTreeNode.Name = "movement";
            movementTreeNode.Text = "Movement";
            playAnimationTreeNode.Name = "playanimation";
            playAnimationTreeNode.Tag = (int)EventCommandType.PlayAnimation;
            playAnimationTreeNode.Text = "Play Animation";
            playBGMTreeNode.Name = "playbgm";
            playBGMTreeNode.Tag = (int)EventCommandType.PlayBgm;
            playBGMTreeNode.Text = "Play BGM";
            fadeoutBGMTreeNode.Name = "fadeoutbgm";
            fadeoutBGMTreeNode.Tag = (int)EventCommandType.FadeoutBgm;
            fadeoutBGMTreeNode.Text = "Fadeout BGM";
            playSoundTreeNode.Name = "playsound";
            playSoundTreeNode.Tag = (int)EventCommandType.PlaySound;
            playSoundTreeNode.Text = "Play Sound";
            stopSoundsTreeNode.Name = "stopsounds";
            stopSoundsTreeNode.Tag = (int)EventCommandType.StopSounds;
            stopSoundsTreeNode.Text = "Stop Sounds";
            showPictureTreeNode.Name = "showpicture";
            showPictureTreeNode.Tag = (int)EventCommandType.ShowPicture;
            showPictureTreeNode.Text = "Show Picture";
            hidePIctureTreeNode.Name = "hidepicture";
            hidePIctureTreeNode.Tag = (int)EventCommandType.HidePicture;
            hidePIctureTreeNode.Text = "Hide Picture";
            screenFadeTreeNode.Name = "fade";
            screenFadeTreeNode.Tag = (int)EventCommandType.Fade;
            screenFadeTreeNode.Text = "Screen Fade";
            specialEffectsTreeNode.Name = "specialeffects";
            specialEffectsTreeNode.Text = "Special Effects";
            startQuestTreeNode.Name = "startquest";
            startQuestTreeNode.Tag = (int)EventCommandType.StartQuest;
            startQuestTreeNode.Text = "Start Quest";
            completeQuestTreeNode.Name = "completequesttask";
            completeQuestTreeNode.Tag = (int)EventCommandType.CompleteQuestTask;
            completeQuestTreeNode.Text = "Complete Quest Task";
            endQuestTreeNode.Name = "endquest";
            endQuestTreeNode.Tag = (int)EventCommandType.EndQuest;
            endQuestTreeNode.Text = "End Quest";
            questControlTreeNode.Name = "questcontrol";
            questControlTreeNode.Text = "Quest Control";
            waitTreeNode.Name = "wait";
            waitTreeNode.Tag = (int)EventCommandType.Wait;
            waitTreeNode.Text = "Wait...";
            etcTreeNode.Name = "etc";
            etcTreeNode.Text = "Etc";
            openBankTreeNode.Name = "openbank";
            openBankTreeNode.Tag = (int)EventCommandType.OpenBank;
            openBankTreeNode.Text = "Open Bank";
            openShopTreeNode.Name = "openshop";
            openShopTreeNode.Tag = (int)EventCommandType.OpenShop;
            openShopTreeNode.Text = "Open Shop";
            openCraftingStationTreeNode.Name = "opencraftingstation";
            openCraftingStationTreeNode.Tag = (int)EventCommandType.OpenCraftingTable;
            openCraftingStationTreeNode.Text = "Open Crafting Station";
            shopAndBankTreeNode.Name = "shopandbank";
            shopAndBankTreeNode.Text = "Shop and Bank";
            createGuildTreeNode.Name = "createguild";
            createGuildTreeNode.Tag = (int)EventCommandType.CreateGuild;
            createGuildTreeNode.Text = "Create Guild";
            disbandGuildTreeNode.Name = "disbandguild";
            disbandGuildTreeNode.Tag = (int)EventCommandType.DisbandGuild;
            disbandGuildTreeNode.Text = "Disband Guild";
            openGuildBankTreeNode.Name = "openguildbank";
            openGuildBankTreeNode.Tag = (int)EventCommandType.OpenGuildBank;
            openGuildBankTreeNode.Text = "Open Guild Bank";
            setGuildBankSlotsTreeNode.Name = "setguildbankslots";
            setGuildBankSlotsTreeNode.Tag = (int)EventCommandType.OpenShop;
            setGuildBankSlotsTreeNode.Text = "Set Guild Bank Slots Count";
            guildsTreeNode.Name = "guilds";
            guildsTreeNode.Text = "Guilds";
            lstCommands.Nodes.AddRange(new TreeNode[] { dialogueTreeNode, logicFlowTreeNode, playerControlTreeNode, movementTreeNode, specialEffectsTreeNode, questControlTreeNode, etcTreeNode, shopAndBankTreeNode, guildsTreeNode });
            lstCommands.Size = new Size(500, 536);
            lstCommands.TabIndex = 2;
            lstCommands.NodeMouseDoubleClick += lstCommands_NodeMouseDoubleClick;
            // 
            // grpEventCommands
            // 
            grpEventCommands.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpEventCommands.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpEventCommands.Controls.Add(lstEventCommands);
            grpEventCommands.ForeColor = System.Drawing.Color.Gainsboro;
            grpEventCommands.Location = new System.Drawing.Point(389, 0);
            grpEventCommands.Margin = new Padding(4, 3, 4, 3);
            grpEventCommands.Name = "grpEventCommands";
            grpEventCommands.Padding = new Padding(4, 3, 4, 3);
            grpEventCommands.Size = new Size(515, 580);
            grpEventCommands.TabIndex = 6;
            grpEventCommands.TabStop = false;
            grpEventCommands.Text = "Commands";
            // 
            // lstEventCommands
            // 
            lstEventCommands.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            lstEventCommands.BorderStyle = BorderStyle.FixedSingle;
            lstEventCommands.DrawMode = DrawMode.OwnerDrawFixed;
            lstEventCommands.ForeColor = System.Drawing.Color.Gainsboro;
            lstEventCommands.FormattingEnabled = true;
            lstEventCommands.HorizontalScrollbar = true;
            lstEventCommands.Items.AddRange(new object[] { "@>" });
            lstEventCommands.Location = new System.Drawing.Point(7, 22);
            lstEventCommands.Margin = new Padding(4, 3, 4, 3);
            lstEventCommands.Name = "lstEventCommands";
            lstEventCommands.Size = new Size(500, 535);
            lstEventCommands.TabIndex = 0;
            lstEventCommands.DrawItem += lstEventCommands_DrawItem;
            lstEventCommands.SelectedIndexChanged += lstEventCommands_SelectedIndexChanged;
            lstEventCommands.DoubleClick += lstEventCommands_DoubleClick;
            lstEventCommands.KeyDown += lstEventCommands_KeyDown;
            lstEventCommands.MouseDown += lstEventCommands_Click;
            // 
            // grpCreateCommands
            // 
            grpCreateCommands.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpCreateCommands.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpCreateCommands.ForeColor = System.Drawing.Color.Gainsboro;
            grpCreateCommands.Location = new System.Drawing.Point(389, 0);
            grpCreateCommands.Margin = new Padding(4, 3, 4, 3);
            grpCreateCommands.Name = "grpCreateCommands";
            grpCreateCommands.Padding = new Padding(4, 3, 4, 3);
            grpCreateCommands.Size = new Size(515, 580);
            grpCreateCommands.TabIndex = 8;
            grpCreateCommands.TabStop = false;
            grpCreateCommands.Visible = false;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(6, 688);
            btnSave.Margin = new Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Padding = new Padding(6);
            btnSave.Size = new Size(108, 24);
            btnSave.TabIndex = 6;
            btnSave.Text = "Save";
            btnSave.Click += btnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(122, 688);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Padding = new Padding(6);
            btnCancel.Size = new Size(108, 24);
            btnCancel.TabIndex = 7;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // commandMenu
            // 
            commandMenu.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            commandMenu.ImageScalingSize = new Size(24, 24);
            commandMenu.Items.AddRange(new ToolStripItem[] { btnInsert, btnEdit, btnCut, btnCopy, btnPaste, btnDelete });
            commandMenu.Name = "commandMenu";
            commandMenu.RenderMode = ToolStripRenderMode.System;
            commandMenu.Size = new Size(108, 136);
            // 
            // btnInsert
            // 
            btnInsert.ForeColor = System.Drawing.Color.Gainsboro;
            btnInsert.Name = "btnInsert";
            btnInsert.Size = new Size(107, 22);
            btnInsert.Text = "Insert";
            btnInsert.Click += btnInsert_Click;
            // 
            // btnEdit
            // 
            btnEdit.ForeColor = System.Drawing.Color.Gainsboro;
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new Size(107, 22);
            btnEdit.Text = "Edit";
            btnEdit.Click += btnEdit_Click;
            // 
            // btnCut
            // 
            btnCut.ForeColor = System.Drawing.Color.Gainsboro;
            btnCut.Name = "btnCut";
            btnCut.Size = new Size(107, 22);
            btnCut.Text = "Cut";
            btnCut.Click += btnCut_Click;
            // 
            // btnCopy
            // 
            btnCopy.ForeColor = System.Drawing.Color.Gainsboro;
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new Size(107, 22);
            btnCopy.Text = "Copy";
            btnCopy.Click += btnCopy_Click;
            // 
            // btnPaste
            // 
            btnPaste.ForeColor = System.Drawing.Color.Gainsboro;
            btnPaste.Name = "btnPaste";
            btnPaste.Size = new Size(107, 22);
            btnPaste.Text = "Paste";
            btnPaste.Click += btnPaste_Click;
            // 
            // btnDelete
            // 
            btnDelete.ForeColor = System.Drawing.Color.Gainsboro;
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(107, 22);
            btnDelete.Text = "Delete";
            btnDelete.Click += btnDelete_Click;
            // 
            // grpPageOptions
            // 
            grpPageOptions.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpPageOptions.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpPageOptions.Controls.Add(btnClearPage);
            grpPageOptions.Controls.Add(btnDeletePage);
            grpPageOptions.Controls.Add(btnPastePage);
            grpPageOptions.Controls.Add(btnCopyPage);
            grpPageOptions.Controls.Add(btnNewPage);
            grpPageOptions.ForeColor = System.Drawing.Color.Gainsboro;
            grpPageOptions.Location = new System.Drawing.Point(358, 0);
            grpPageOptions.Margin = new Padding(4, 3, 4, 3);
            grpPageOptions.Name = "grpPageOptions";
            grpPageOptions.Padding = new Padding(4, 3, 4, 3);
            grpPageOptions.Size = new Size(587, 51);
            grpPageOptions.TabIndex = 13;
            grpPageOptions.TabStop = false;
            grpPageOptions.Text = "Page Options";
            // 
            // btnClearPage
            // 
            btnClearPage.Location = new System.Drawing.Point(471, 17);
            btnClearPage.Margin = new Padding(4, 3, 4, 3);
            btnClearPage.Name = "btnClearPage";
            btnClearPage.Padding = new Padding(6);
            btnClearPage.Size = new Size(108, 24);
            btnClearPage.TabIndex = 17;
            btnClearPage.Text = "Clear Page";
            btnClearPage.Click += btnClearPage_Click;
            // 
            // btnDeletePage
            // 
            btnDeletePage.Enabled = false;
            btnDeletePage.Location = new System.Drawing.Point(356, 17);
            btnDeletePage.Margin = new Padding(4, 3, 4, 3);
            btnDeletePage.Name = "btnDeletePage";
            btnDeletePage.Padding = new Padding(6);
            btnDeletePage.Size = new Size(108, 24);
            btnDeletePage.TabIndex = 16;
            btnDeletePage.Text = "Delete Page";
            btnDeletePage.Click += btnDeletePage_Click;
            // 
            // btnPastePage
            // 
            btnPastePage.Location = new System.Drawing.Point(240, 17);
            btnPastePage.Margin = new Padding(4, 3, 4, 3);
            btnPastePage.Name = "btnPastePage";
            btnPastePage.Padding = new Padding(6);
            btnPastePage.Size = new Size(108, 24);
            btnPastePage.TabIndex = 15;
            btnPastePage.Text = "Paste Page";
            btnPastePage.Click += btnPastePage_Click;
            // 
            // btnCopyPage
            // 
            btnCopyPage.Location = new System.Drawing.Point(124, 17);
            btnCopyPage.Margin = new Padding(4, 3, 4, 3);
            btnCopyPage.Name = "btnCopyPage";
            btnCopyPage.Padding = new Padding(6);
            btnCopyPage.Size = new Size(108, 24);
            btnCopyPage.TabIndex = 14;
            btnCopyPage.Text = "Copy Page";
            btnCopyPage.Click += btnCopyPage_Click;
            // 
            // btnNewPage
            // 
            btnNewPage.Location = new System.Drawing.Point(8, 17);
            btnNewPage.Margin = new Padding(4, 3, 4, 3);
            btnNewPage.Name = "btnNewPage";
            btnNewPage.Padding = new Padding(6);
            btnNewPage.Size = new Size(108, 24);
            btnNewPage.TabIndex = 13;
            btnNewPage.Text = "New Page";
            btnNewPage.Click += btnNewPage_Click;
            // 
            // grpGeneral
            // 
            grpGeneral.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpGeneral.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpGeneral.Controls.Add(chkIsGlobal);
            grpGeneral.Controls.Add(lblName);
            grpGeneral.Controls.Add(txtEventname);
            grpGeneral.ForeColor = System.Drawing.Color.Gainsboro;
            grpGeneral.Location = new System.Drawing.Point(6, 0);
            grpGeneral.Margin = new Padding(4, 3, 4, 3);
            grpGeneral.Name = "grpGeneral";
            grpGeneral.Padding = new Padding(4, 3, 4, 3);
            grpGeneral.Size = new Size(344, 51);
            grpGeneral.TabIndex = 18;
            grpGeneral.TabStop = false;
            grpGeneral.Text = "General";
            // 
            // chkIsGlobal
            // 
            chkIsGlobal.AutoSize = true;
            chkIsGlobal.Location = new System.Drawing.Point(201, 19);
            chkIsGlobal.Margin = new Padding(4, 3, 4, 3);
            chkIsGlobal.Name = "chkIsGlobal";
            chkIsGlobal.Size = new Size(92, 19);
            chkIsGlobal.TabIndex = 3;
            chkIsGlobal.Text = "Global Event";
            chkIsGlobal.CheckedChanged += chkIsGlobal_CheckedChanged;
            // 
            // pnlTabsContainer
            // 
            pnlTabsContainer.Controls.Add(pnlTabs);
            pnlTabsContainer.Location = new System.Drawing.Point(72, 57);
            pnlTabsContainer.Margin = new Padding(4, 3, 4, 3);
            pnlTabsContainer.Name = "pnlTabsContainer";
            pnlTabsContainer.Size = new Size(807, 22);
            pnlTabsContainer.TabIndex = 22;
            // 
            // pnlTabs
            // 
            pnlTabs.AutoSize = true;
            pnlTabs.Location = new System.Drawing.Point(0, 0);
            pnlTabs.Margin = new Padding(4, 3, 4, 3);
            pnlTabs.Name = "pnlTabs";
            pnlTabs.Size = new Size(807, 22);
            pnlTabs.TabIndex = 23;
            // 
            // btnTabsLeft
            // 
            btnTabsLeft.Location = new System.Drawing.Point(6, 57);
            btnTabsLeft.Margin = new Padding(4, 3, 4, 3);
            btnTabsLeft.Name = "btnTabsLeft";
            btnTabsLeft.Padding = new Padding(6);
            btnTabsLeft.Size = new Size(58, 22);
            btnTabsLeft.TabIndex = 0;
            btnTabsLeft.Text = "<";
            btnTabsLeft.Click += btnTabsLeft_Click;
            // 
            // btnTabsRight
            // 
            btnTabsRight.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnTabsRight.Location = new System.Drawing.Point(887, 57);
            btnTabsRight.Margin = new Padding(4, 3, 4, 3);
            btnTabsRight.Name = "btnTabsRight";
            btnTabsRight.Padding = new Padding(6);
            btnTabsRight.Size = new Size(58, 22);
            btnTabsRight.TabIndex = 1;
            btnTabsRight.Text = ">";
            btnTabsRight.Click += btnTabsRight_Click;
            // 
            // pnlEditorComponents
            // 
            pnlEditorComponents.AutoScroll = true;
            pnlEditorComponents.BorderStyle = BorderStyle.FixedSingle;
            pnlEditorComponents.Controls.Add(grpNewCommands);
            pnlEditorComponents.Controls.Add(grpEventCommands);
            pnlEditorComponents.Controls.Add(grpCreateCommands);
            pnlEditorComponents.Controls.Add(grpEntityOptions);
            pnlEditorComponents.Controls.Add(grpTriggers);
            pnlEditorComponents.Controls.Add(grpEventConditions);
            pnlEditorComponents.Location = new System.Drawing.Point(6, 85);
            pnlEditorComponents.Margin = new Padding(4, 3, 4, 3);
            pnlEditorComponents.MaximumSize = new Size(939, 597);
            pnlEditorComponents.Name = "pnlEditorComponents";
            pnlEditorComponents.Size = new Size(939, 597);
            pnlEditorComponents.TabIndex = 23;
            // 
            // FrmEvent
            // 
            AcceptButton = btnSave;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            CancelButton = btnCancel;
            ClientSize = new Size(951, 718);
            Controls.Add(btnTabsRight);
            Controls.Add(btnTabsLeft);
            Controls.Add(pnlEditorComponents);
            Controls.Add(grpPageOptions);
            Controls.Add(grpGeneral);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(pnlTabsContainer);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            KeyPreview = true;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimumSize = new Size(967, 757);
            Name = "FrmEvent";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Event Editor";
            FormClosing += frmEvent_FormClosing;
            FormClosed += FrmEvent_FormClosed;
            Load += frmEvent_Load;
            VisibleChanged += FrmEvent_VisibleChanged;
            KeyDown += FrmEvent_KeyDown;
            Move += FrmEvent_Move;
            grpEntityOptions.ResumeLayout(false);
            grpExtra.ResumeLayout(false);
            grpExtra.PerformLayout();
            grpInspector.ResumeLayout(false);
            grpInspector.PerformLayout();
            grpPreview.ResumeLayout(false);
            grpPreview.PerformLayout();
            grpMovement.ResumeLayout(false);
            grpMovement.PerformLayout();
            grpTriggers.ResumeLayout(false);
            grpTriggers.PerformLayout();
            grpEventConditions.ResumeLayout(false);
            grpNewCommands.ResumeLayout(false);
            grpNewCommands.PerformLayout();
            grpEventCommands.ResumeLayout(false);
            commandMenu.ResumeLayout(false);
            grpPageOptions.ResumeLayout(false);
            grpGeneral.ResumeLayout(false);
            grpGeneral.PerformLayout();
            pnlTabsContainer.ResumeLayout(false);
            pnlTabsContainer.PerformLayout();
            pnlEditorComponents.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Label lblName;
        private DarkTextBox txtEventname;
        private DarkGroupBox grpEventCommands;
        private ListBox lstEventCommands;
        private DarkGroupBox grpEventConditions;
        private DarkGroupBox grpExtra;
        private DarkGroupBox grpMovement;
        private DarkGroupBox grpInspector;
        private DarkButton btnSave;
        private DarkButton btnCancel;
        private DarkComboBox cmbEventFreq;
        private DarkComboBox cmbEventSpeed;
        private Label lblFreq;
        private Label lblSpeed;
        private DarkButton btnSetRoute;
        private Label lblType;
        private DarkComboBox cmbMoveType;
        private DarkComboBox cmbTrigger;
        private DarkComboBox cmbLayering;
        private DarkCheckBox chkWalkThrough;
        private DarkGroupBox grpNewCommands;
        private DarkGroupBox grpCreateCommands;
        private ContextMenuStrip commandMenu;
        private ToolStripMenuItem btnInsert;
        private ToolStripMenuItem btnEdit;
        private ToolStripMenuItem btnDelete;
        private DarkCheckBox chkHideName;
        private DarkCheckBox chkDisableInspector;
        private DarkComboBox cmbPreviewFace;
        private Label lblFace;
        private DarkTextBox txtDesc;
        private DarkGroupBox grpPreview;
        private DarkGroupBox grpPageOptions;
        private DarkButton btnNewPage;
        private DarkButton btnCopyPage;
        private DarkButton btnPastePage;
        private DarkButton btnDeletePage;
        private DarkButton btnClearPage;
        private DarkGroupBox grpEntityOptions;
        private Label lblInspectorDesc;
        private Panel pnlPreview;
        private Panel pnlFacePreview;
        private DarkCheckBox chkWalkingAnimation;
        private DarkCheckBox chkDirectionFix;
        private DarkGroupBox grpGeneral;
        private Label lblAnimation;
        private DarkComboBox cmbAnimation;
        private DarkCheckBox chkIsGlobal;
        private Label lblLayer;
        private Label lblCloseCommands;
        private DarkCheckBox chkInteractionFreeze;
        private Label lblTriggerVal;
        private DarkComboBox cmbTriggerVal;
        private Panel pnlTabsContainer;
        private DarkGroupBox grpTriggers;
        private Panel pnlEditorComponents;
        private DarkButton btnTabsLeft;
        private DarkButton btnTabsRight;
        private Panel pnlTabs;
        private TreeView lstCommands;
        private DarkButton btnEditConditions;
        private DarkTextBox txtCommand;
        private Label lblCommand;
        private ToolStripMenuItem btnCut;
        private ToolStripMenuItem btnCopy;
        private ToolStripMenuItem btnPaste;
        private DarkComboBox cmbVariable;
        private Label lblVariableTrigger;
        private DarkCheckBox chkIgnoreNpcAvoids;
    }
}