using System.ComponentModel;
using System.Windows.Forms;
using DarkUI.Controls;

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
            var treeNode1 = new TreeNode("Show Text");
            var treeNode2 = new TreeNode("Show Options");
            var treeNode3 = new TreeNode("Input Variable");
            var treeNode4 = new TreeNode("Add Chatbox Text");
            var treeNode5 = new TreeNode("Dialogue", new TreeNode[] { treeNode1, treeNode2, treeNode3, treeNode4 });
            var treeNode6 = new TreeNode("Set Variable");
            var treeNode7 = new TreeNode("Set Self Switch");
            var treeNode8 = new TreeNode("Conditional Branch");
            var treeNode9 = new TreeNode("Exit Event Process");
            var treeNode10 = new TreeNode("Label");
            var treeNode11 = new TreeNode("Go To Label");
            var treeNode12 = new TreeNode("Start Common Event");
            var treeNode13 = new TreeNode("Logic Flow", new TreeNode[] { treeNode6, treeNode7, treeNode8, treeNode9, treeNode10, treeNode11, treeNode12 });
            var treeNode14 = new TreeNode("Restore HP");
            var treeNode15 = new TreeNode("Restore MP");
            var treeNode16 = new TreeNode("Level Up");
            var treeNode17 = new TreeNode("Give Experience");
            var treeNode18 = new TreeNode("Change Level");
            var treeNode19 = new TreeNode("Change Spells");
            var treeNode20 = new TreeNode("Change Items");
            var treeNode21 = new TreeNode("Change Sprite");
            var treeNode22 = new TreeNode("Change Player Color");
            var treeNode23 = new TreeNode("Change Face");
            var treeNode24 = new TreeNode("Change Gender");
            var treeNode25 = new TreeNode("Set Access");
            var treeNode26 = new TreeNode("Change Class");
            var treeNode27 = new TreeNode("Equip/Unequip Item");
            var treeNode28 = new TreeNode("Change Name Color");
            var treeNode29 = new TreeNode("Change Player Label");
            var treeNode30 = new TreeNode("Change Player Name");
            var treeNode31 = new TreeNode("Reset Stat Point Allocations");
            var treeNode32 = new TreeNode("Cast Spell On");
            var treeNode33 = new TreeNode("Player Control", new TreeNode[] { treeNode14, treeNode15, treeNode16, treeNode17, treeNode18, treeNode19, treeNode20, treeNode21, treeNode22, treeNode23, treeNode24, treeNode25, treeNode26, treeNode27, treeNode28, treeNode29, treeNode30, treeNode31, treeNode32 });
            var treeNode34 = new TreeNode("Warp Player");
            var treeNode35 = new TreeNode("Set Move Route");
            var treeNode36 = new TreeNode("Wait for Route Completion");
            var treeNode37 = new TreeNode("Hold Player");
            var treeNode38 = new TreeNode("Release Player");
            var treeNode39 = new TreeNode("Spawn NPC");
            var treeNode40 = new TreeNode("Despawn NPC");
            var treeNode41 = new TreeNode("Hide Player");
            var treeNode42 = new TreeNode("Show Player");
            var treeNode43 = new TreeNode("Movement", new TreeNode[] { treeNode34, treeNode35, treeNode36, treeNode37, treeNode38, treeNode39, treeNode40, treeNode41, treeNode42 });
            var treeNode44 = new TreeNode("Play Animation");
            var treeNode45 = new TreeNode("Play BGM");
            var treeNode46 = new TreeNode("Fadeout BGM");
            var treeNode47 = new TreeNode("Play Sound");
            var treeNode48 = new TreeNode("Stop Sounds");
            var treeNode49 = new TreeNode("Show Picture");
            var treeNode50 = new TreeNode("Hide Picture");
            var treeNode51 = new TreeNode("Special Effects", new TreeNode[] { treeNode44, treeNode45, treeNode46, treeNode47, treeNode48, treeNode49, treeNode50 });
            var treeNode52 = new TreeNode("Start Quest");
            var treeNode53 = new TreeNode("Complete Quest Task");
            var treeNode54 = new TreeNode("End Quest");
            var treeNode55 = new TreeNode("Quest Control", new TreeNode[] { treeNode52, treeNode53, treeNode54 });
            var treeNode56 = new TreeNode("Wait...");
            var treeNode57 = new TreeNode("Etc", new TreeNode[] { treeNode56 });
            var treeNode58 = new TreeNode("Open Bank");
            var treeNode59 = new TreeNode("Open Shop");
            var treeNode60 = new TreeNode("Open Crafting Station");
            var treeNode61 = new TreeNode("Shop and Bank", new TreeNode[] { treeNode58, treeNode59, treeNode60 });
            var treeNode62 = new TreeNode("Create Guild");
            var treeNode63 = new TreeNode("Disband Guild");
            var treeNode64 = new TreeNode("Open Guild Bank");
            var treeNode65 = new TreeNode("Set Guild Bank Slots Count");
            var treeNode66 = new TreeNode("Guilds", new TreeNode[] { treeNode62, treeNode63, treeNode64, treeNode65 });
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
            treeNode1.Name = "showtext";
            treeNode1.Tag = "1";
            treeNode1.Text = "Show Text";
            treeNode2.Name = "showoptions";
            treeNode2.Tag = "2";
            treeNode2.Text = "Show Options";
            treeNode3.Name = "inputvariable";
            treeNode3.Tag = "49";
            treeNode3.Text = "Input Variable";
            treeNode4.Name = "addchatboxtext";
            treeNode4.Tag = "3";
            treeNode4.Text = "Add Chatbox Text";
            treeNode5.Name = "dialogue";
            treeNode5.Text = "Dialogue";
            treeNode6.Name = "setvariable";
            treeNode6.Tag = "5";
            treeNode6.Text = "Set Variable";
            treeNode7.Name = "setselfswitch";
            treeNode7.Tag = "6";
            treeNode7.Text = "Set Self Switch";
            treeNode8.Name = "conditionalbranch";
            treeNode8.Tag = "7";
            treeNode8.Text = "Conditional Branch";
            treeNode9.Name = "exiteventprocess";
            treeNode9.Tag = "8";
            treeNode9.Text = "Exit Event Process";
            treeNode10.Name = "label";
            treeNode10.Tag = "9";
            treeNode10.Text = "Label";
            treeNode11.Name = "gotolabel";
            treeNode11.Tag = "10";
            treeNode11.Text = "Go To Label";
            treeNode12.Name = "startcommonevent";
            treeNode12.Tag = "11";
            treeNode12.Text = "Start Common Event";
            treeNode13.Name = "logicflow";
            treeNode13.Text = "Logic Flow";
            treeNode14.Name = "restorehp";
            treeNode14.Tag = "12";
            treeNode14.Text = "Restore HP";
            treeNode15.Name = "restoremp";
            treeNode15.Tag = "13";
            treeNode15.Text = "Restore MP";
            treeNode16.Name = "levelup";
            treeNode16.Tag = "14";
            treeNode16.Text = "Level Up";
            treeNode17.Name = "giveexperience";
            treeNode17.Tag = "15";
            treeNode17.Text = "Give Experience";
            treeNode18.Name = "changelevel";
            treeNode18.Tag = "16";
            treeNode18.Text = "Change Level";
            treeNode19.Name = "changespells";
            treeNode19.Tag = "17";
            treeNode19.Text = "Change Spells";
            treeNode20.Name = "changeitems";
            treeNode20.Tag = "18";
            treeNode20.Text = "Change Items";
            treeNode21.Name = "changesprite";
            treeNode21.Tag = "19";
            treeNode21.Text = "Change Sprite";
            treeNode22.Name = "changeplayercolor";
            treeNode22.Tag = "51";
            treeNode22.Text = "Change Player Color";
            treeNode23.Name = "changeface";
            treeNode23.Tag = "20";
            treeNode23.Text = "Change Face";
            treeNode24.Name = "changegender";
            treeNode24.Tag = "21";
            treeNode24.Text = "Change Gender";
            treeNode25.Name = "setaccess";
            treeNode25.Tag = "22";
            treeNode25.Text = "Set Access";
            treeNode26.Name = "changeclass";
            treeNode26.Tag = "38";
            treeNode26.Text = "Change Class";
            treeNode27.Name = "equipitem";
            treeNode27.Tag = "47";
            treeNode27.Text = "Equip/Unequip Item";
            treeNode28.Name = "changenamecolor";
            treeNode28.Tag = "48";
            treeNode28.Text = "Change Name Color";
            treeNode29.Name = "changeplayerlabel";
            treeNode29.Tag = "50";
            treeNode29.Text = "Change Player Label";
            treeNode30.Name = "changename";
            treeNode30.Tag = "52";
            treeNode30.Text = "Change Player Name";
            treeNode31.Name = "resetstatallocations";
            treeNode31.Tag = "57";
            treeNode31.Text = "Reset Stat Point Allocations";
            treeNode32.Name = "castspellon";
            treeNode32.Tag = "58";
            treeNode32.Text = "Cast Spell On";
            treeNode33.Name = "playercontrol";
            treeNode33.Text = "Player Control";
            treeNode34.Name = "warpplayer";
            treeNode34.Tag = "23";
            treeNode34.Text = "Warp Player";
            treeNode35.Name = "setmoveroute";
            treeNode35.Tag = "24";
            treeNode35.Text = "Set Move Route";
            treeNode36.Name = "waitmoveroute";
            treeNode36.Tag = "25";
            treeNode36.Text = "Wait for Route Completion";
            treeNode37.Name = "holdplayer";
            treeNode37.Tag = "26";
            treeNode37.Text = "Hold Player";
            treeNode38.Name = "releaseplayer";
            treeNode38.Tag = "27";
            treeNode38.Text = "Release Player";
            treeNode39.Name = "spawnnpc";
            treeNode39.Tag = "28";
            treeNode39.Text = "Spawn NPC";
            treeNode40.Name = "despawnnpcs";
            treeNode40.Tag = "39";
            treeNode40.Text = "Despawn NPC";
            treeNode41.Name = "hideplayer";
            treeNode41.Tag = "45";
            treeNode41.Text = "Hide Player";
            treeNode42.Name = "showplayer";
            treeNode42.Tag = "46";
            treeNode42.Text = "Show Player";
            treeNode43.Name = "movement";
            treeNode43.Text = "Movement";
            treeNode44.Name = "playanimation";
            treeNode44.Tag = "29";
            treeNode44.Text = "Play Animation";
            treeNode45.Name = "playbgm";
            treeNode45.Tag = "30";
            treeNode45.Text = "Play BGM";
            treeNode46.Name = "fadeoutbgm";
            treeNode46.Tag = "31";
            treeNode46.Text = "Fadeout BGM";
            treeNode47.Name = "playsound";
            treeNode47.Tag = "32";
            treeNode47.Text = "Play Sound";
            treeNode48.Name = "stopsounds";
            treeNode48.Tag = "33";
            treeNode48.Text = "Stop Sounds";
            treeNode49.Name = "showpicture";
            treeNode49.Tag = "43";
            treeNode49.Text = "Show Picture";
            treeNode50.Name = "hidepicture";
            treeNode50.Tag = "44";
            treeNode50.Text = "Hide Picture";
            treeNode51.Name = "specialeffects";
            treeNode51.Text = "Special Effects";
            treeNode52.Name = "startquest";
            treeNode52.Tag = "40";
            treeNode52.Text = "Start Quest";
            treeNode53.Name = "completequesttask";
            treeNode53.Tag = "41";
            treeNode53.Text = "Complete Quest Task";
            treeNode54.Name = "endquest";
            treeNode54.Tag = "42";
            treeNode54.Text = "End Quest";
            treeNode55.Name = "questcontrol";
            treeNode55.Text = "Quest Control";
            treeNode56.Name = "wait";
            treeNode56.Tag = "34";
            treeNode56.Text = "Wait...";
            treeNode57.Name = "etc";
            treeNode57.Text = "Etc";
            treeNode58.Name = "openbank";
            treeNode58.Tag = "35";
            treeNode58.Text = "Open Bank";
            treeNode59.Name = "openshop";
            treeNode59.Tag = "36";
            treeNode59.Text = "Open Shop";
            treeNode60.Name = "opencraftingstation";
            treeNode60.Tag = "37";
            treeNode60.Text = "Open Crafting Station";
            treeNode61.Name = "shopandbank";
            treeNode61.Text = "Shop and Bank";
            treeNode62.Name = "createguild";
            treeNode62.Tag = "53";
            treeNode62.Text = "Create Guild";
            treeNode63.Name = "disbandguild";
            treeNode63.Tag = "54";
            treeNode63.Text = "Disband Guild";
            treeNode64.Name = "openguildbank";
            treeNode64.Tag = "55";
            treeNode64.Text = "Open Guild Bank";
            treeNode65.Name = "setguildbankslots";
            treeNode65.Tag = "56";
            treeNode65.Text = "Set Guild Bank Slots Count";
            treeNode66.Name = "guilds";
            treeNode66.Text = "Guilds";
            lstCommands.Nodes.AddRange(new TreeNode[] { treeNode5, treeNode13, treeNode33, treeNode43, treeNode51, treeNode55, treeNode57, treeNode61, treeNode66 });
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