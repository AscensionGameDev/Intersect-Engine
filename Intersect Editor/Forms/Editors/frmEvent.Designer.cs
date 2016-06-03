using System.ComponentModel;
using System.Windows.Forms;

namespace Intersect_Editor.Forms
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Dialog", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Logic Flow", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Player Control", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Movement", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup5 = new System.Windows.Forms.ListViewGroup("Questing", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup6 = new System.Windows.Forms.ListViewGroup("Special Effects", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup7 = new System.Windows.Forms.ListViewGroup("Etc", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup8 = new System.Windows.Forms.ListViewGroup("Shop and Bank", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Show Text");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Show Options");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Add Chatbox Text");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Set Switch");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Set Variable");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("Set Self Switch");
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("Conditional Branch");
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem("Exit Event Process");
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem("Label");
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem("Go To Label");
            System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem("Start Common Event");
            System.Windows.Forms.ListViewItem listViewItem12 = new System.Windows.Forms.ListViewItem("Restore HP");
            System.Windows.Forms.ListViewItem listViewItem13 = new System.Windows.Forms.ListViewItem("Restore MP");
            System.Windows.Forms.ListViewItem listViewItem14 = new System.Windows.Forms.ListViewItem("Level Up");
            System.Windows.Forms.ListViewItem listViewItem15 = new System.Windows.Forms.ListViewItem("Give Experience");
            System.Windows.Forms.ListViewItem listViewItem16 = new System.Windows.Forms.ListViewItem("Change Level");
            System.Windows.Forms.ListViewItem listViewItem17 = new System.Windows.Forms.ListViewItem("Change Spells");
            System.Windows.Forms.ListViewItem listViewItem18 = new System.Windows.Forms.ListViewItem("Change Items");
            System.Windows.Forms.ListViewItem listViewItem19 = new System.Windows.Forms.ListViewItem("Change Sprite");
            System.Windows.Forms.ListViewItem listViewItem20 = new System.Windows.Forms.ListViewItem("Change Face");
            System.Windows.Forms.ListViewItem listViewItem21 = new System.Windows.Forms.ListViewItem("Change Gender");
            System.Windows.Forms.ListViewItem listViewItem22 = new System.Windows.Forms.ListViewItem("Set Access");
            System.Windows.Forms.ListViewItem listViewItem23 = new System.Windows.Forms.ListViewItem("Warp Player");
            System.Windows.Forms.ListViewItem listViewItem24 = new System.Windows.Forms.ListViewItem("Set Move Route");
            System.Windows.Forms.ListViewItem listViewItem25 = new System.Windows.Forms.ListViewItem("Wait for Route Completion");
            System.Windows.Forms.ListViewItem listViewItem26 = new System.Windows.Forms.ListViewItem("Hold Player");
            System.Windows.Forms.ListViewItem listViewItem27 = new System.Windows.Forms.ListViewItem("Release Player");
            System.Windows.Forms.ListViewItem listViewItem28 = new System.Windows.Forms.ListViewItem("Spawn NPC");
            System.Windows.Forms.ListViewItem listViewItem29 = new System.Windows.Forms.ListViewItem("Play Animation");
            System.Windows.Forms.ListViewItem listViewItem30 = new System.Windows.Forms.ListViewItem("Play BGM");
            System.Windows.Forms.ListViewItem listViewItem31 = new System.Windows.Forms.ListViewItem("Fadeout BGM");
            System.Windows.Forms.ListViewItem listViewItem32 = new System.Windows.Forms.ListViewItem("Play Sound");
            System.Windows.Forms.ListViewItem listViewItem33 = new System.Windows.Forms.ListViewItem("Stop Sounds");
            System.Windows.Forms.ListViewItem listViewItem34 = new System.Windows.Forms.ListViewItem("Wait...");
            System.Windows.Forms.ListViewItem listViewItem35 = new System.Windows.Forms.ListViewItem("Open Bank");
            System.Windows.Forms.ListViewItem listViewItem36 = new System.Windows.Forms.ListViewItem("Open Shop");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmEvent));
            this.label1 = new System.Windows.Forms.Label();
            this.txtEventname = new System.Windows.Forms.TextBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.grpEntityOptions = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbAnimation = new System.Windows.Forms.ComboBox();
            this.pnlPreview = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbTrigger = new System.Windows.Forms.ComboBox();
            this.cmbLayering = new System.Windows.Forms.ComboBox();
            this.cmbEventFreq = new System.Windows.Forms.ComboBox();
            this.cmbEventSpeed = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSetRoute = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbMoveType = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkWalkingAnimation = new System.Windows.Forms.CheckBox();
            this.chkDirectionFix = new System.Windows.Forms.CheckBox();
            this.chkHideName = new System.Windows.Forms.CheckBox();
            this.chkWalkThrough = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pnlFacePreview = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.txtDesc = new System.Windows.Forms.TextBox();
            this.chkDisablePreview = new System.Windows.Forms.CheckBox();
            this.cmbPreviewFace = new System.Windows.Forms.ComboBox();
            this.label25 = new System.Windows.Forms.Label();
            this.grpEventConditions = new System.Windows.Forms.GroupBox();
            this.btnRemoveCondition = new System.Windows.Forms.Button();
            this.btnAddCondition = new System.Windows.Forms.Button();
            this.lstConditions = new System.Windows.Forms.ListBox();
            this.grpNewCommands = new System.Windows.Forms.GroupBox();
            this.lstCommands = new System.Windows.Forms.ListView();
            this.grpEventCommands = new System.Windows.Forms.GroupBox();
            this.lstEventCommands = new System.Windows.Forms.ListBox();
            this.grpCreateCommands = new System.Windows.Forms.GroupBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.commandMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnInsert = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.btnDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.grpPageOptions = new System.Windows.Forms.GroupBox();
            this.btnClearPage = new System.Windows.Forms.Button();
            this.btnDeletePage = new System.Windows.Forms.Button();
            this.btnPastePage = new System.Windows.Forms.Button();
            this.btnCopyPage = new System.Windows.Forms.Button();
            this.btnNewPage = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.chkIsGlobal = new System.Windows.Forms.CheckBox();
            this.lblCloseCommands = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.grpEntityOptions.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpEventConditions.SuspendLayout();
            this.grpNewCommands.SuspendLayout();
            this.grpEventCommands.SuspendLayout();
            this.commandMenu.SuspendLayout();
            this.grpPageOptions.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name:";
            // 
            // txtEventname
            // 
            this.txtEventname.Location = new System.Drawing.Point(48, 19);
            this.txtEventname.Name = "txtEventname";
            this.txtEventname.Size = new System.Drawing.Size(124, 20);
            this.txtEventname.TabIndex = 2;
            this.txtEventname.TextChanged += new System.EventHandler(this.txtEventname_TextChanged);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Location = new System.Drawing.Point(12, 61);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(811, 521);
            this.tabControl.TabIndex = 5;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage1.Size = new System.Drawing.Size(803, 495);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // grpEntityOptions
            // 
            this.grpEntityOptions.Controls.Add(this.groupBox6);
            this.grpEntityOptions.Controls.Add(this.groupBox2);
            this.grpEntityOptions.Controls.Add(this.groupBox3);
            this.grpEntityOptions.Controls.Add(this.groupBox1);
            this.grpEntityOptions.Location = new System.Drawing.Point(20, 199);
            this.grpEntityOptions.Name = "grpEntityOptions";
            this.grpEntityOptions.Size = new System.Drawing.Size(326, 373);
            this.grpEntityOptions.TabIndex = 12;
            this.grpEntityOptions.TabStop = false;
            this.grpEntityOptions.Text = "Entity Options";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Controls.Add(this.cmbAnimation);
            this.groupBox6.Controls.Add(this.pnlPreview);
            this.groupBox6.Location = new System.Drawing.Point(6, 13);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(160, 191);
            this.groupBox6.TabIndex = 10;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Preview";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 146);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Animation:";
            // 
            // cmbAnimation
            // 
            this.cmbAnimation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAnimation.FormattingEnabled = true;
            this.cmbAnimation.Location = new System.Drawing.Point(20, 162);
            this.cmbAnimation.Name = "cmbAnimation";
            this.cmbAnimation.Size = new System.Drawing.Size(125, 21);
            this.cmbAnimation.TabIndex = 1;
            this.cmbAnimation.SelectedIndexChanged += new System.EventHandler(this.cmbAnimation_SelectedIndexChanged);
            // 
            // pnlPreview
            // 
            this.pnlPreview.Location = new System.Drawing.Point(17, 14);
            this.pnlPreview.Name = "pnlPreview";
            this.pnlPreview.Size = new System.Drawing.Size(128, 128);
            this.pnlPreview.TabIndex = 0;
            this.pnlPreview.DoubleClick += new System.EventHandler(this.pnlPreview_DoubleClick);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.cmbTrigger);
            this.groupBox2.Controls.Add(this.cmbLayering);
            this.groupBox2.Controls.Add(this.cmbEventFreq);
            this.groupBox2.Controls.Add(this.cmbEventSpeed);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.btnSetRoute);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.cmbMoveType);
            this.groupBox2.Location = new System.Drawing.Point(170, 14);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(154, 190);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Movement";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(1, 153);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Trigger:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 128);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(36, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Layer:";
            // 
            // cmbTrigger
            // 
            this.cmbTrigger.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTrigger.FormattingEnabled = true;
            this.cmbTrigger.Items.AddRange(new object[] {
            "Action Button",
            "Player Touch",
            "Autorun"});
            this.cmbTrigger.Location = new System.Drawing.Point(48, 151);
            this.cmbTrigger.Name = "cmbTrigger";
            this.cmbTrigger.Size = new System.Drawing.Size(101, 21);
            this.cmbTrigger.TabIndex = 2;
            this.cmbTrigger.SelectedIndexChanged += new System.EventHandler(this.cmbTrigger_SelectedIndexChanged);
            // 
            // cmbLayering
            // 
            this.cmbLayering.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLayering.FormattingEnabled = true;
            this.cmbLayering.Items.AddRange(new object[] {
            "Below Player",
            "Same as Player",
            "Above Player"});
            this.cmbLayering.Location = new System.Drawing.Point(48, 126);
            this.cmbLayering.Name = "cmbLayering";
            this.cmbLayering.Size = new System.Drawing.Size(101, 21);
            this.cmbLayering.TabIndex = 1;
            this.cmbLayering.SelectedIndexChanged += new System.EventHandler(this.cmbLayering_SelectedIndexChanged);
            // 
            // cmbEventFreq
            // 
            this.cmbEventFreq.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEventFreq.FormattingEnabled = true;
            this.cmbEventFreq.Items.AddRange(new object[] {
            "Not Very Often",
            "Not Often",
            "Normal",
            "Often",
            "Very Often"});
            this.cmbEventFreq.Location = new System.Drawing.Point(48, 101);
            this.cmbEventFreq.Name = "cmbEventFreq";
            this.cmbEventFreq.Size = new System.Drawing.Size(100, 21);
            this.cmbEventFreq.TabIndex = 6;
            this.cmbEventFreq.SelectedIndexChanged += new System.EventHandler(this.cmbEventFreq_SelectedIndexChanged);
            // 
            // cmbEventSpeed
            // 
            this.cmbEventSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEventSpeed.FormattingEnabled = true;
            this.cmbEventSpeed.Items.AddRange(new object[] {
            "Slowest",
            "Slower",
            "Normal",
            "Faster",
            "Fastest"});
            this.cmbEventSpeed.Location = new System.Drawing.Point(48, 77);
            this.cmbEventSpeed.Name = "cmbEventSpeed";
            this.cmbEventSpeed.Size = new System.Drawing.Size(100, 21);
            this.cmbEventSpeed.TabIndex = 5;
            this.cmbEventSpeed.SelectedIndexChanged += new System.EventHandler(this.cmbEventSpeed_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Freq:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Speed:";
            // 
            // btnSetRoute
            // 
            this.btnSetRoute.Enabled = false;
            this.btnSetRoute.Location = new System.Drawing.Point(73, 43);
            this.btnSetRoute.Name = "btnSetRoute";
            this.btnSetRoute.Size = new System.Drawing.Size(75, 23);
            this.btnSetRoute.TabIndex = 2;
            this.btnSetRoute.Text = "Set Route....";
            this.btnSetRoute.UseVisualStyleBackColor = true;
            this.btnSetRoute.Click += new System.EventHandler(this.btnSetRoute_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Type:";
            // 
            // cmbMoveType
            // 
            this.cmbMoveType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMoveType.FormattingEnabled = true;
            this.cmbMoveType.Items.AddRange(new object[] {
            "None",
            "Random",
            "Move Route"});
            this.cmbMoveType.Location = new System.Drawing.Point(48, 19);
            this.cmbMoveType.Name = "cmbMoveType";
            this.cmbMoveType.Size = new System.Drawing.Size(100, 21);
            this.cmbMoveType.TabIndex = 0;
            this.cmbMoveType.SelectedIndexChanged += new System.EventHandler(this.cmbMoveType_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkWalkingAnimation);
            this.groupBox3.Controls.Add(this.chkDirectionFix);
            this.groupBox3.Controls.Add(this.chkHideName);
            this.groupBox3.Controls.Add(this.chkWalkThrough);
            this.groupBox3.Location = new System.Drawing.Point(6, 326);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(315, 41);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Extra";
            // 
            // chkWalkingAnimation
            // 
            this.chkWalkingAnimation.AutoSize = true;
            this.chkWalkingAnimation.Location = new System.Drawing.Point(214, 19);
            this.chkWalkingAnimation.Name = "chkWalkingAnimation";
            this.chkWalkingAnimation.Size = new System.Drawing.Size(91, 17);
            this.chkWalkingAnimation.TabIndex = 5;
            this.chkWalkingAnimation.Text = "Walking Anim";
            this.chkWalkingAnimation.UseVisualStyleBackColor = true;
            this.chkWalkingAnimation.CheckedChanged += new System.EventHandler(this.chkWalkingAnimation_CheckedChanged);
            // 
            // chkDirectionFix
            // 
            this.chkDirectionFix.AutoSize = true;
            this.chkDirectionFix.Location = new System.Drawing.Point(156, 19);
            this.chkDirectionFix.Name = "chkDirectionFix";
            this.chkDirectionFix.Size = new System.Drawing.Size(55, 17);
            this.chkDirectionFix.TabIndex = 4;
            this.chkDirectionFix.Text = "Dir Fix";
            this.chkDirectionFix.UseVisualStyleBackColor = true;
            this.chkDirectionFix.CheckedChanged += new System.EventHandler(this.chkDirectionFix_CheckedChanged);
            // 
            // chkHideName
            // 
            this.chkHideName.AutoSize = true;
            this.chkHideName.Location = new System.Drawing.Point(75, 19);
            this.chkHideName.Name = "chkHideName";
            this.chkHideName.Size = new System.Drawing.Size(79, 17);
            this.chkHideName.TabIndex = 3;
            this.chkHideName.Text = "Hide Name";
            this.chkHideName.UseVisualStyleBackColor = true;
            this.chkHideName.CheckedChanged += new System.EventHandler(this.chkHideName_CheckedChanged);
            // 
            // chkWalkThrough
            // 
            this.chkWalkThrough.AutoSize = true;
            this.chkWalkThrough.Location = new System.Drawing.Point(6, 19);
            this.chkWalkThrough.Name = "chkWalkThrough";
            this.chkWalkThrough.Size = new System.Drawing.Size(69, 17);
            this.chkWalkThrough.TabIndex = 2;
            this.chkWalkThrough.Text = "Passable";
            this.chkWalkThrough.UseVisualStyleBackColor = true;
            this.chkWalkThrough.CheckedChanged += new System.EventHandler(this.chkWalkThrough_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pnlFacePreview);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtDesc);
            this.groupBox1.Controls.Add(this.chkDisablePreview);
            this.groupBox1.Controls.Add(this.cmbPreviewFace);
            this.groupBox1.Controls.Add(this.label25);
            this.groupBox1.Location = new System.Drawing.Point(2, 210);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(320, 117);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Entity Inspector Options";
            // 
            // pnlFacePreview
            // 
            this.pnlFacePreview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlFacePreview.Location = new System.Drawing.Point(9, 46);
            this.pnlFacePreview.Name = "pnlFacePreview";
            this.pnlFacePreview.Size = new System.Drawing.Size(64, 64);
            this.pnlFacePreview.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(79, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 19);
            this.label5.TabIndex = 11;
            this.label5.Text = "Inspector Description:";
            // 
            // txtDesc
            // 
            this.txtDesc.Location = new System.Drawing.Point(79, 61);
            this.txtDesc.Multiline = true;
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.Size = new System.Drawing.Size(235, 50);
            this.txtDesc.TabIndex = 0;
            this.txtDesc.TextChanged += new System.EventHandler(this.txtDesc_TextChanged);
            // 
            // chkDisablePreview
            // 
            this.chkDisablePreview.Location = new System.Drawing.Point(207, 15);
            this.chkDisablePreview.Name = "chkDisablePreview";
            this.chkDisablePreview.Size = new System.Drawing.Size(113, 21);
            this.chkDisablePreview.TabIndex = 4;
            this.chkDisablePreview.Text = "Disable Inspector";
            this.chkDisablePreview.UseVisualStyleBackColor = true;
            this.chkDisablePreview.CheckedChanged += new System.EventHandler(this.chkDisablePreview_CheckedChanged);
            // 
            // cmbPreviewFace
            // 
            this.cmbPreviewFace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPreviewFace.FormattingEnabled = true;
            this.cmbPreviewFace.Location = new System.Drawing.Point(79, 15);
            this.cmbPreviewFace.Name = "cmbPreviewFace";
            this.cmbPreviewFace.Size = new System.Drawing.Size(81, 21);
            this.cmbPreviewFace.TabIndex = 10;
            this.cmbPreviewFace.SelectedIndexChanged += new System.EventHandler(this.cmbPreviewFace_SelectedIndexChanged);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(6, 18);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(75, 13);
            this.label25.TabIndex = 9;
            this.label25.Text = "Preview Face:";
            // 
            // grpEventConditions
            // 
            this.grpEventConditions.Controls.Add(this.btnRemoveCondition);
            this.grpEventConditions.Controls.Add(this.btnAddCondition);
            this.grpEventConditions.Controls.Add(this.lstConditions);
            this.grpEventConditions.Location = new System.Drawing.Point(20, 88);
            this.grpEventConditions.Name = "grpEventConditions";
            this.grpEventConditions.Size = new System.Drawing.Size(326, 107);
            this.grpEventConditions.TabIndex = 5;
            this.grpEventConditions.TabStop = false;
            this.grpEventConditions.Text = "Conditions";
            // 
            // btnRemoveCondition
            // 
            this.btnRemoveCondition.Location = new System.Drawing.Point(292, 48);
            this.btnRemoveCondition.Name = "btnRemoveCondition";
            this.btnRemoveCondition.Size = new System.Drawing.Size(29, 23);
            this.btnRemoveCondition.TabIndex = 2;
            this.btnRemoveCondition.Text = "-";
            this.btnRemoveCondition.UseVisualStyleBackColor = true;
            this.btnRemoveCondition.Click += new System.EventHandler(this.btnRemoveCondition_Click);
            // 
            // btnAddCondition
            // 
            this.btnAddCondition.Location = new System.Drawing.Point(292, 19);
            this.btnAddCondition.Name = "btnAddCondition";
            this.btnAddCondition.Size = new System.Drawing.Size(29, 23);
            this.btnAddCondition.TabIndex = 1;
            this.btnAddCondition.Text = "+";
            this.btnAddCondition.UseVisualStyleBackColor = true;
            this.btnAddCondition.Click += new System.EventHandler(this.btnAddCondition_Click);
            // 
            // lstConditions
            // 
            this.lstConditions.FormattingEnabled = true;
            this.lstConditions.Location = new System.Drawing.Point(6, 19);
            this.lstConditions.Name = "lstConditions";
            this.lstConditions.Size = new System.Drawing.Size(282, 82);
            this.lstConditions.TabIndex = 0;
            // 
            // grpNewCommands
            // 
            this.grpNewCommands.Controls.Add(this.lblCloseCommands);
            this.grpNewCommands.Controls.Add(this.lstCommands);
            this.grpNewCommands.Location = new System.Drawing.Point(352, 88);
            this.grpNewCommands.Name = "grpNewCommands";
            this.grpNewCommands.Size = new System.Drawing.Size(457, 484);
            this.grpNewCommands.TabIndex = 7;
            this.grpNewCommands.TabStop = false;
            this.grpNewCommands.Text = "Add Commands";
            this.grpNewCommands.Visible = false;
            // 
            // lstCommands
            // 
            listViewGroup1.Header = "Dialog";
            listViewGroup1.Name = "Dialog";
            listViewGroup2.Header = "Logic Flow";
            listViewGroup2.Name = "Logic Flow";
            listViewGroup3.Header = "Player Control";
            listViewGroup3.Name = "Player Control";
            listViewGroup4.Header = "Movement";
            listViewGroup4.Name = "Movement";
            listViewGroup5.Header = "Questing";
            listViewGroup5.Name = "Questing";
            listViewGroup6.Header = "Special Effects";
            listViewGroup6.Name = "Special Effects";
            listViewGroup7.Header = "Etc";
            listViewGroup7.Name = "Etc";
            listViewGroup8.Header = "Shop and Bank";
            listViewGroup8.Name = "Shop and Bank";
            this.lstCommands.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3,
            listViewGroup4,
            listViewGroup5,
            listViewGroup6,
            listViewGroup7,
            listViewGroup8});
            this.lstCommands.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstCommands.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            listViewItem1.Group = listViewGroup1;
            listViewItem1.Tag = "0";
            listViewItem2.Group = listViewGroup1;
            listViewItem2.Tag = "1";
            listViewItem3.Group = listViewGroup1;
            listViewItem4.Group = listViewGroup2;
            listViewItem4.Tag = "2";
            listViewItem5.Group = listViewGroup2;
            listViewItem5.Tag = "3";
            listViewItem6.Group = listViewGroup2;
            listViewItem7.Group = listViewGroup2;
            listViewItem7.Tag = "4";
            listViewItem8.Group = listViewGroup2;
            listViewItem9.Group = listViewGroup2;
            listViewItem10.Group = listViewGroup2;
            listViewItem11.Group = listViewGroup2;
            listViewItem12.Group = listViewGroup3;
            listViewItem13.Group = listViewGroup3;
            listViewItem14.Group = listViewGroup3;
            listViewItem15.Group = listViewGroup3;
            listViewItem16.Group = listViewGroup3;
            listViewItem17.Group = listViewGroup3;
            listViewItem18.Group = listViewGroup3;
            listViewItem19.Group = listViewGroup3;
            listViewItem20.Group = listViewGroup3;
            listViewItem21.Group = listViewGroup3;
            listViewItem22.Group = listViewGroup3;
            listViewItem23.Group = listViewGroup4;
            listViewItem23.Tag = "5";
            listViewItem24.Group = listViewGroup4;
            listViewItem25.Group = listViewGroup4;
            listViewItem26.Group = listViewGroup4;
            listViewItem27.Group = listViewGroup4;
            listViewItem28.Group = listViewGroup4;
            listViewItem29.Group = listViewGroup6;
            listViewItem30.Group = listViewGroup6;
            listViewItem31.Group = listViewGroup6;
            listViewItem32.Group = listViewGroup6;
            listViewItem33.Group = listViewGroup6;
            listViewItem34.Group = listViewGroup7;
            listViewItem35.Group = listViewGroup8;
            listViewItem36.Group = listViewGroup8;
            this.lstCommands.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8,
            listViewItem9,
            listViewItem10,
            listViewItem11,
            listViewItem12,
            listViewItem13,
            listViewItem14,
            listViewItem15,
            listViewItem16,
            listViewItem17,
            listViewItem18,
            listViewItem19,
            listViewItem20,
            listViewItem21,
            listViewItem22,
            listViewItem23,
            listViewItem24,
            listViewItem25,
            listViewItem26,
            listViewItem27,
            listViewItem28,
            listViewItem29,
            listViewItem30,
            listViewItem31,
            listViewItem32,
            listViewItem33,
            listViewItem34,
            listViewItem35,
            listViewItem36});
            this.lstCommands.Location = new System.Drawing.Point(7, 30);
            this.lstCommands.MultiSelect = false;
            this.lstCommands.Name = "lstCommands";
            this.lstCommands.Size = new System.Drawing.Size(444, 450);
            this.lstCommands.TabIndex = 0;
            this.lstCommands.TileSize = new System.Drawing.Size(160, 30);
            this.lstCommands.UseCompatibleStateImageBehavior = false;
            this.lstCommands.View = System.Windows.Forms.View.Tile;
            this.lstCommands.ItemActivate += new System.EventHandler(this.lstCommands_ItemActivated);
            this.lstCommands.SelectedIndexChanged += new System.EventHandler(this.lstCommands_SelectedIndexChanged);
            // 
            // grpEventCommands
            // 
            this.grpEventCommands.Controls.Add(this.lstEventCommands);
            this.grpEventCommands.Location = new System.Drawing.Point(352, 88);
            this.grpEventCommands.Name = "grpEventCommands";
            this.grpEventCommands.Size = new System.Drawing.Size(457, 484);
            this.grpEventCommands.TabIndex = 6;
            this.grpEventCommands.TabStop = false;
            this.grpEventCommands.Text = "Commands";
            // 
            // lstEventCommands
            // 
            this.lstEventCommands.FormattingEnabled = true;
            this.lstEventCommands.HorizontalScrollbar = true;
            this.lstEventCommands.Items.AddRange(new object[] {
            "@>"});
            this.lstEventCommands.Location = new System.Drawing.Point(6, 19);
            this.lstEventCommands.Name = "lstEventCommands";
            this.lstEventCommands.ScrollAlwaysVisible = true;
            this.lstEventCommands.Size = new System.Drawing.Size(445, 459);
            this.lstEventCommands.TabIndex = 0;
            this.lstEventCommands.SelectedIndexChanged += new System.EventHandler(this.lstEventCommands_SelectedIndexChanged);
            this.lstEventCommands.DoubleClick += new System.EventHandler(this.lstEventCommands_DoubleClick);
            this.lstEventCommands.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstEventCommands_KeyDown);
            this.lstEventCommands.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstEventCommands_Click);
            // 
            // grpCreateCommands
            // 
            this.grpCreateCommands.Location = new System.Drawing.Point(352, 88);
            this.grpCreateCommands.Name = "grpCreateCommands";
            this.grpCreateCommands.Size = new System.Drawing.Size(457, 484);
            this.grpCreateCommands.TabIndex = 8;
            this.grpCreateCommands.TabStop = false;
            this.grpCreateCommands.Visible = false;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(628, 586);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(93, 30);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(727, 586);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(93, 30);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // commandMenu
            // 
            this.commandMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.commandMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnInsert,
            this.btnEdit,
            this.btnDelete});
            this.commandMenu.Name = "commandMenu";
            this.commandMenu.Size = new System.Drawing.Size(108, 70);
            // 
            // btnInsert
            // 
            this.btnInsert.Name = "btnInsert";
            this.btnInsert.Size = new System.Drawing.Size(107, 22);
            this.btnInsert.Text = "Insert";
            this.btnInsert.Click += new System.EventHandler(this.btnInsert_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(107, 22);
            this.btnEdit.Text = "Edit";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(107, 22);
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // grpPageOptions
            // 
            this.grpPageOptions.Controls.Add(this.btnClearPage);
            this.grpPageOptions.Controls.Add(this.btnDeletePage);
            this.grpPageOptions.Controls.Add(this.btnPastePage);
            this.grpPageOptions.Controls.Add(this.btnCopyPage);
            this.grpPageOptions.Controls.Add(this.btnNewPage);
            this.grpPageOptions.Location = new System.Drawing.Point(313, 5);
            this.grpPageOptions.Name = "grpPageOptions";
            this.grpPageOptions.Size = new System.Drawing.Size(510, 50);
            this.grpPageOptions.TabIndex = 13;
            this.grpPageOptions.TabStop = false;
            this.grpPageOptions.Text = "Page Options";
            // 
            // btnClearPage
            // 
            this.btnClearPage.Location = new System.Drawing.Point(402, 16);
            this.btnClearPage.Name = "btnClearPage";
            this.btnClearPage.Size = new System.Drawing.Size(93, 30);
            this.btnClearPage.TabIndex = 17;
            this.btnClearPage.Text = "Clear Page";
            this.btnClearPage.UseVisualStyleBackColor = true;
            this.btnClearPage.Click += new System.EventHandler(this.btnClearPage_Click);
            // 
            // btnDeletePage
            // 
            this.btnDeletePage.Enabled = false;
            this.btnDeletePage.Location = new System.Drawing.Point(303, 16);
            this.btnDeletePage.Name = "btnDeletePage";
            this.btnDeletePage.Size = new System.Drawing.Size(93, 30);
            this.btnDeletePage.TabIndex = 16;
            this.btnDeletePage.Text = "Delete Page";
            this.btnDeletePage.UseVisualStyleBackColor = true;
            this.btnDeletePage.Click += new System.EventHandler(this.btnDeletePage_Click);
            // 
            // btnPastePage
            // 
            this.btnPastePage.Location = new System.Drawing.Point(204, 16);
            this.btnPastePage.Name = "btnPastePage";
            this.btnPastePage.Size = new System.Drawing.Size(93, 30);
            this.btnPastePage.TabIndex = 15;
            this.btnPastePage.Text = "Paste Page";
            this.btnPastePage.UseVisualStyleBackColor = true;
            this.btnPastePage.Click += new System.EventHandler(this.btnPastePage_Click);
            // 
            // btnCopyPage
            // 
            this.btnCopyPage.Location = new System.Drawing.Point(105, 16);
            this.btnCopyPage.Name = "btnCopyPage";
            this.btnCopyPage.Size = new System.Drawing.Size(93, 30);
            this.btnCopyPage.TabIndex = 14;
            this.btnCopyPage.Text = "Copy Page";
            this.btnCopyPage.UseVisualStyleBackColor = true;
            this.btnCopyPage.Click += new System.EventHandler(this.btnCopyPage_Click);
            // 
            // btnNewPage
            // 
            this.btnNewPage.Location = new System.Drawing.Point(6, 16);
            this.btnNewPage.Name = "btnNewPage";
            this.btnNewPage.Size = new System.Drawing.Size(93, 30);
            this.btnNewPage.TabIndex = 13;
            this.btnNewPage.Text = "New Page";
            this.btnNewPage.UseVisualStyleBackColor = true;
            this.btnNewPage.Click += new System.EventHandler(this.btnNewPage_Click);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.chkIsGlobal);
            this.groupBox7.Controls.Add(this.label1);
            this.groupBox7.Controls.Add(this.txtEventname);
            this.groupBox7.Location = new System.Drawing.Point(12, 5);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(295, 49);
            this.groupBox7.TabIndex = 18;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "General";
            // 
            // chkIsGlobal
            // 
            this.chkIsGlobal.AutoSize = true;
            this.chkIsGlobal.Location = new System.Drawing.Point(202, 22);
            this.chkIsGlobal.Name = "chkIsGlobal";
            this.chkIsGlobal.Size = new System.Drawing.Size(87, 17);
            this.chkIsGlobal.TabIndex = 3;
            this.chkIsGlobal.Text = "Global Event";
            this.chkIsGlobal.UseVisualStyleBackColor = true;
            this.chkIsGlobal.CheckedChanged += new System.EventHandler(this.chkIsGlobal_CheckedChanged);
            // 
            // lblCloseCommands
            // 
            this.lblCloseCommands.AutoSize = true;
            this.lblCloseCommands.Location = new System.Drawing.Point(437, 14);
            this.lblCloseCommands.Name = "lblCloseCommands";
            this.lblCloseCommands.Size = new System.Drawing.Size(14, 13);
            this.lblCloseCommands.TabIndex = 1;
            this.lblCloseCommands.Text = "X";
            this.lblCloseCommands.Click += new System.EventHandler(this.lblCloseCommands_Click);
            // 
            // FrmEvent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(835, 622);
            this.Controls.Add(this.grpEntityOptions);
            this.Controls.Add(this.grpEventConditions);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.grpPageOptions);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpNewCommands);
            this.Controls.Add(this.grpCreateCommands);
            this.Controls.Add(this.grpEventCommands);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "FrmEvent";
            this.Text = "Event Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmEvent_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmEvent_FormClosed);
            this.Load += new System.EventHandler(this.frmEvent_Load);
            this.VisibleChanged += new System.EventHandler(this.FrmEvent_VisibleChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmEvent_KeyDown);
            this.tabControl.ResumeLayout(false);
            this.grpEntityOptions.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpEventConditions.ResumeLayout(false);
            this.grpNewCommands.ResumeLayout(false);
            this.grpNewCommands.PerformLayout();
            this.grpEventCommands.ResumeLayout(false);
            this.commandMenu.ResumeLayout(false);
            this.grpPageOptions.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Label label1;
        private TextBox txtEventname;
        private TabControl tabControl;
        private TabPage tabPage1;
        private GroupBox grpEventCommands;
        private ListBox lstEventCommands;
        private GroupBox grpEventConditions;
        private GroupBox groupBox3;
        private GroupBox groupBox2;
        private GroupBox groupBox1;
        private Button btnSave;
        private Button btnCancel;
        private ComboBox cmbEventFreq;
        private ComboBox cmbEventSpeed;
        private Label label4;
        private Label label3;
        private Button btnSetRoute;
        private Label label2;
        private ComboBox cmbMoveType;
        private ComboBox cmbTrigger;
        private ComboBox cmbLayering;
        private CheckBox chkWalkThrough;
        private GroupBox grpNewCommands;
        private ListView lstCommands;
        private GroupBox grpCreateCommands;
        private ContextMenuStrip commandMenu;
        private ToolStripMenuItem btnInsert;
        private ToolStripMenuItem btnEdit;
        private ToolStripMenuItem btnDelete;
        private CheckBox chkHideName;
        private CheckBox chkDisablePreview;
        private ComboBox cmbPreviewFace;
        private Label label25;
        private TextBox txtDesc;
        private GroupBox groupBox6;
        private GroupBox grpPageOptions;
        private Button btnNewPage;
        private Button btnCopyPage;
        private Button btnPastePage;
        private Button btnDeletePage;
        private Button btnClearPage;
        private GroupBox grpEntityOptions;
        private Label label5;
        private Panel pnlPreview;
        private Panel pnlFacePreview;
        private CheckBox chkWalkingAnimation;
        private CheckBox chkDirectionFix;
        private Button btnRemoveCondition;
        private Button btnAddCondition;
        private ListBox lstConditions;
        private GroupBox groupBox7;
        private Label label6;
        private ComboBox cmbAnimation;
        private CheckBox chkIsGlobal;
        private Label label7;
        private Label label8;
        private Label lblCloseCommands;
    }
}