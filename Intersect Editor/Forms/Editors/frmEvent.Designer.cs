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
            this.label1.Location = new System.Drawing.Point(9, 34);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name:";
            // 
            // txtEventname
            // 
            this.txtEventname.Location = new System.Drawing.Point(72, 29);
            this.txtEventname.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtEventname.Name = "txtEventname";
            this.txtEventname.Size = new System.Drawing.Size(184, 26);
            this.txtEventname.TabIndex = 2;
            this.txtEventname.TextChanged += new System.EventHandler(this.txtEventname_TextChanged);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Location = new System.Drawing.Point(18, 94);
            this.tabControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1216, 802);
            this.tabControl.TabIndex = 5;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage1.Size = new System.Drawing.Size(1208, 769);
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
            this.grpEntityOptions.Location = new System.Drawing.Point(30, 306);
            this.grpEntityOptions.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpEntityOptions.Name = "grpEntityOptions";
            this.grpEntityOptions.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpEntityOptions.Size = new System.Drawing.Size(489, 574);
            this.grpEntityOptions.TabIndex = 12;
            this.grpEntityOptions.TabStop = false;
            this.grpEntityOptions.Text = "Entity Options";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Controls.Add(this.cmbAnimation);
            this.groupBox6.Controls.Add(this.pnlPreview);
            this.groupBox6.Location = new System.Drawing.Point(9, 20);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox6.Size = new System.Drawing.Size(240, 294);
            this.groupBox6.TabIndex = 10;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Preview";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 225);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 20);
            this.label6.TabIndex = 2;
            this.label6.Text = "Animation:";
            // 
            // cmbAnimation
            // 
            this.cmbAnimation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAnimation.FormattingEnabled = true;
            this.cmbAnimation.Location = new System.Drawing.Point(30, 249);
            this.cmbAnimation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbAnimation.Name = "cmbAnimation";
            this.cmbAnimation.Size = new System.Drawing.Size(186, 28);
            this.cmbAnimation.TabIndex = 1;
            this.cmbAnimation.SelectedIndexChanged += new System.EventHandler(this.cmbAnimation_SelectedIndexChanged);
            // 
            // pnlPreview
            // 
            this.pnlPreview.Location = new System.Drawing.Point(26, 22);
            this.pnlPreview.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlPreview.Name = "pnlPreview";
            this.pnlPreview.Size = new System.Drawing.Size(192, 197);
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
            this.groupBox2.Location = new System.Drawing.Point(255, 22);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Size = new System.Drawing.Size(231, 292);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Movement";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(2, 235);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(62, 20);
            this.label7.TabIndex = 8;
            this.label7.Text = "Trigger:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 197);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 20);
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
            this.cmbTrigger.Location = new System.Drawing.Point(72, 232);
            this.cmbTrigger.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbTrigger.Name = "cmbTrigger";
            this.cmbTrigger.Size = new System.Drawing.Size(150, 28);
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
            this.cmbLayering.Location = new System.Drawing.Point(72, 194);
            this.cmbLayering.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbLayering.Name = "cmbLayering";
            this.cmbLayering.Size = new System.Drawing.Size(150, 28);
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
            this.cmbEventFreq.Location = new System.Drawing.Point(72, 156);
            this.cmbEventFreq.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbEventFreq.Name = "cmbEventFreq";
            this.cmbEventFreq.Size = new System.Drawing.Size(148, 28);
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
            this.cmbEventSpeed.Location = new System.Drawing.Point(72, 118);
            this.cmbEventSpeed.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbEventSpeed.Name = "cmbEventSpeed";
            this.cmbEventSpeed.Size = new System.Drawing.Size(148, 28);
            this.cmbEventSpeed.TabIndex = 5;
            this.cmbEventSpeed.SelectedIndexChanged += new System.EventHandler(this.cmbEventSpeed_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 159);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 20);
            this.label4.TabIndex = 4;
            this.label4.Text = "Freq:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 123);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 20);
            this.label3.TabIndex = 3;
            this.label3.Text = "Speed:";
            // 
            // btnSetRoute
            // 
            this.btnSetRoute.Enabled = false;
            this.btnSetRoute.Location = new System.Drawing.Point(110, 66);
            this.btnSetRoute.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSetRoute.Name = "btnSetRoute";
            this.btnSetRoute.Size = new System.Drawing.Size(112, 35);
            this.btnSetRoute.TabIndex = 2;
            this.btnSetRoute.Text = "Set Route....";
            this.btnSetRoute.UseVisualStyleBackColor = true;
            this.btnSetRoute.Click += new System.EventHandler(this.btnSetRoute_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 34);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 20);
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
            this.cmbMoveType.Location = new System.Drawing.Point(72, 29);
            this.cmbMoveType.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbMoveType.Name = "cmbMoveType";
            this.cmbMoveType.Size = new System.Drawing.Size(148, 28);
            this.cmbMoveType.TabIndex = 0;
            this.cmbMoveType.SelectedIndexChanged += new System.EventHandler(this.cmbMoveType_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkWalkingAnimation);
            this.groupBox3.Controls.Add(this.chkDirectionFix);
            this.groupBox3.Controls.Add(this.chkHideName);
            this.groupBox3.Controls.Add(this.chkWalkThrough);
            this.groupBox3.Location = new System.Drawing.Point(9, 502);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Size = new System.Drawing.Size(472, 63);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Extra";
            // 
            // chkWalkingAnimation
            // 
            this.chkWalkingAnimation.AutoSize = true;
            this.chkWalkingAnimation.Location = new System.Drawing.Point(321, 29);
            this.chkWalkingAnimation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkWalkingAnimation.Name = "chkWalkingAnimation";
            this.chkWalkingAnimation.Size = new System.Drawing.Size(131, 24);
            this.chkWalkingAnimation.TabIndex = 5;
            this.chkWalkingAnimation.Text = "Walking Anim";
            this.chkWalkingAnimation.UseVisualStyleBackColor = true;
            this.chkWalkingAnimation.CheckedChanged += new System.EventHandler(this.chkWalkingAnimation_CheckedChanged);
            // 
            // chkDirectionFix
            // 
            this.chkDirectionFix.AutoSize = true;
            this.chkDirectionFix.Location = new System.Drawing.Point(234, 29);
            this.chkDirectionFix.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkDirectionFix.Name = "chkDirectionFix";
            this.chkDirectionFix.Size = new System.Drawing.Size(79, 24);
            this.chkDirectionFix.TabIndex = 4;
            this.chkDirectionFix.Text = "Dir Fix";
            this.chkDirectionFix.UseVisualStyleBackColor = true;
            this.chkDirectionFix.CheckedChanged += new System.EventHandler(this.chkDirectionFix_CheckedChanged);
            // 
            // chkHideName
            // 
            this.chkHideName.AutoSize = true;
            this.chkHideName.Location = new System.Drawing.Point(112, 29);
            this.chkHideName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkHideName.Name = "chkHideName";
            this.chkHideName.Size = new System.Drawing.Size(114, 24);
            this.chkHideName.TabIndex = 3;
            this.chkHideName.Text = "Hide Name";
            this.chkHideName.UseVisualStyleBackColor = true;
            this.chkHideName.CheckedChanged += new System.EventHandler(this.chkHideName_CheckedChanged);
            // 
            // chkWalkThrough
            // 
            this.chkWalkThrough.AutoSize = true;
            this.chkWalkThrough.Location = new System.Drawing.Point(9, 29);
            this.chkWalkThrough.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkWalkThrough.Name = "chkWalkThrough";
            this.chkWalkThrough.Size = new System.Drawing.Size(100, 24);
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
            this.groupBox1.Location = new System.Drawing.Point(3, 323);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(480, 180);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Entity Inspector Options";
            // 
            // pnlFacePreview
            // 
            this.pnlFacePreview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlFacePreview.Location = new System.Drawing.Point(14, 71);
            this.pnlFacePreview.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlFacePreview.Name = "pnlFacePreview";
            this.pnlFacePreview.Size = new System.Drawing.Size(96, 98);
            this.pnlFacePreview.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(118, 65);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(168, 29);
            this.label5.TabIndex = 11;
            this.label5.Text = "Inspector Description:";
            // 
            // txtDesc
            // 
            this.txtDesc.Location = new System.Drawing.Point(118, 94);
            this.txtDesc.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtDesc.Multiline = true;
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.Size = new System.Drawing.Size(350, 75);
            this.txtDesc.TabIndex = 0;
            this.txtDesc.TextChanged += new System.EventHandler(this.txtDesc_TextChanged);
            // 
            // chkDisablePreview
            // 
            this.chkDisablePreview.Location = new System.Drawing.Point(310, 23);
            this.chkDisablePreview.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkDisablePreview.Name = "chkDisablePreview";
            this.chkDisablePreview.Size = new System.Drawing.Size(170, 32);
            this.chkDisablePreview.TabIndex = 4;
            this.chkDisablePreview.Text = "Disable Inspector";
            this.chkDisablePreview.UseVisualStyleBackColor = true;
            this.chkDisablePreview.CheckedChanged += new System.EventHandler(this.chkDisablePreview_CheckedChanged);
            // 
            // cmbPreviewFace
            // 
            this.cmbPreviewFace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPreviewFace.FormattingEnabled = true;
            this.cmbPreviewFace.Location = new System.Drawing.Point(118, 23);
            this.cmbPreviewFace.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbPreviewFace.Name = "cmbPreviewFace";
            this.cmbPreviewFace.Size = new System.Drawing.Size(120, 28);
            this.cmbPreviewFace.TabIndex = 10;
            this.cmbPreviewFace.SelectedIndexChanged += new System.EventHandler(this.cmbPreviewFace_SelectedIndexChanged);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(9, 28);
            this.label25.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(107, 20);
            this.label25.TabIndex = 9;
            this.label25.Text = "Preview Face:";
            // 
            // grpEventConditions
            // 
            this.grpEventConditions.Controls.Add(this.btnRemoveCondition);
            this.grpEventConditions.Controls.Add(this.btnAddCondition);
            this.grpEventConditions.Controls.Add(this.lstConditions);
            this.grpEventConditions.Location = new System.Drawing.Point(30, 135);
            this.grpEventConditions.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpEventConditions.Name = "grpEventConditions";
            this.grpEventConditions.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpEventConditions.Size = new System.Drawing.Size(489, 165);
            this.grpEventConditions.TabIndex = 5;
            this.grpEventConditions.TabStop = false;
            this.grpEventConditions.Text = "Conditions";
            // 
            // btnRemoveCondition
            // 
            this.btnRemoveCondition.Location = new System.Drawing.Point(438, 74);
            this.btnRemoveCondition.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnRemoveCondition.Name = "btnRemoveCondition";
            this.btnRemoveCondition.Size = new System.Drawing.Size(43, 35);
            this.btnRemoveCondition.TabIndex = 2;
            this.btnRemoveCondition.Text = "-";
            this.btnRemoveCondition.UseVisualStyleBackColor = true;
            this.btnRemoveCondition.Click += new System.EventHandler(this.btnRemoveCondition_Click);
            // 
            // btnAddCondition
            // 
            this.btnAddCondition.Location = new System.Drawing.Point(438, 29);
            this.btnAddCondition.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnAddCondition.Name = "btnAddCondition";
            this.btnAddCondition.Size = new System.Drawing.Size(43, 35);
            this.btnAddCondition.TabIndex = 1;
            this.btnAddCondition.Text = "+";
            this.btnAddCondition.UseVisualStyleBackColor = true;
            this.btnAddCondition.Click += new System.EventHandler(this.btnAddCondition_Click);
            // 
            // lstConditions
            // 
            this.lstConditions.FormattingEnabled = true;
            this.lstConditions.ItemHeight = 20;
            this.lstConditions.Location = new System.Drawing.Point(9, 29);
            this.lstConditions.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lstConditions.Name = "lstConditions";
            this.lstConditions.Size = new System.Drawing.Size(421, 124);
            this.lstConditions.TabIndex = 0;
            // 
            // grpNewCommands
            // 
            this.grpNewCommands.Controls.Add(this.lstCommands);
            this.grpNewCommands.Location = new System.Drawing.Point(528, 135);
            this.grpNewCommands.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpNewCommands.Name = "grpNewCommands";
            this.grpNewCommands.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpNewCommands.Size = new System.Drawing.Size(686, 745);
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
            this.lstCommands.Location = new System.Drawing.Point(10, 29);
            this.lstCommands.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lstCommands.MultiSelect = false;
            this.lstCommands.Name = "lstCommands";
            this.lstCommands.Size = new System.Drawing.Size(664, 707);
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
            this.grpEventCommands.Location = new System.Drawing.Point(528, 135);
            this.grpEventCommands.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpEventCommands.Name = "grpEventCommands";
            this.grpEventCommands.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpEventCommands.Size = new System.Drawing.Size(686, 745);
            this.grpEventCommands.TabIndex = 6;
            this.grpEventCommands.TabStop = false;
            this.grpEventCommands.Text = "Commands";
            // 
            // lstEventCommands
            // 
            this.lstEventCommands.FormattingEnabled = true;
            this.lstEventCommands.HorizontalScrollbar = true;
            this.lstEventCommands.ItemHeight = 20;
            this.lstEventCommands.Items.AddRange(new object[] {
            "@>"});
            this.lstEventCommands.Location = new System.Drawing.Point(9, 29);
            this.lstEventCommands.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lstEventCommands.Name = "lstEventCommands";
            this.lstEventCommands.ScrollAlwaysVisible = true;
            this.lstEventCommands.Size = new System.Drawing.Size(666, 704);
            this.lstEventCommands.TabIndex = 0;
            this.lstEventCommands.SelectedIndexChanged += new System.EventHandler(this.lstEventCommands_SelectedIndexChanged);
            this.lstEventCommands.DoubleClick += new System.EventHandler(this.lstEventCommands_DoubleClick);
            this.lstEventCommands.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstEventCommands_KeyDown);
            this.lstEventCommands.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstEventCommands_Click);
            // 
            // grpCreateCommands
            // 
            this.grpCreateCommands.Location = new System.Drawing.Point(528, 135);
            this.grpCreateCommands.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpCreateCommands.Name = "grpCreateCommands";
            this.grpCreateCommands.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpCreateCommands.Size = new System.Drawing.Size(686, 745);
            this.grpCreateCommands.TabIndex = 8;
            this.grpCreateCommands.TabStop = false;
            this.grpCreateCommands.Visible = false;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(942, 902);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(140, 46);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(1090, 902);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(140, 46);
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
            this.commandMenu.Size = new System.Drawing.Size(148, 94);
            // 
            // btnInsert
            // 
            this.btnInsert.Name = "btnInsert";
            this.btnInsert.Size = new System.Drawing.Size(147, 30);
            this.btnInsert.Text = "Insert";
            this.btnInsert.Click += new System.EventHandler(this.btnInsert_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(147, 30);
            this.btnEdit.Text = "Edit";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(147, 30);
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
            this.grpPageOptions.Location = new System.Drawing.Point(470, 8);
            this.grpPageOptions.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpPageOptions.Name = "grpPageOptions";
            this.grpPageOptions.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpPageOptions.Size = new System.Drawing.Size(765, 77);
            this.grpPageOptions.TabIndex = 13;
            this.grpPageOptions.TabStop = false;
            this.grpPageOptions.Text = "Page Options";
            // 
            // btnClearPage
            // 
            this.btnClearPage.Location = new System.Drawing.Point(603, 25);
            this.btnClearPage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnClearPage.Name = "btnClearPage";
            this.btnClearPage.Size = new System.Drawing.Size(140, 46);
            this.btnClearPage.TabIndex = 17;
            this.btnClearPage.Text = "Clear Page";
            this.btnClearPage.UseVisualStyleBackColor = true;
            this.btnClearPage.Click += new System.EventHandler(this.btnClearPage_Click);
            // 
            // btnDeletePage
            // 
            this.btnDeletePage.Enabled = false;
            this.btnDeletePage.Location = new System.Drawing.Point(454, 25);
            this.btnDeletePage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnDeletePage.Name = "btnDeletePage";
            this.btnDeletePage.Size = new System.Drawing.Size(140, 46);
            this.btnDeletePage.TabIndex = 16;
            this.btnDeletePage.Text = "Delete Page";
            this.btnDeletePage.UseVisualStyleBackColor = true;
            this.btnDeletePage.Click += new System.EventHandler(this.btnDeletePage_Click);
            // 
            // btnPastePage
            // 
            this.btnPastePage.Location = new System.Drawing.Point(306, 25);
            this.btnPastePage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnPastePage.Name = "btnPastePage";
            this.btnPastePage.Size = new System.Drawing.Size(140, 46);
            this.btnPastePage.TabIndex = 15;
            this.btnPastePage.Text = "Paste Page";
            this.btnPastePage.UseVisualStyleBackColor = true;
            this.btnPastePage.Click += new System.EventHandler(this.btnPastePage_Click);
            // 
            // btnCopyPage
            // 
            this.btnCopyPage.Location = new System.Drawing.Point(158, 25);
            this.btnCopyPage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCopyPage.Name = "btnCopyPage";
            this.btnCopyPage.Size = new System.Drawing.Size(140, 46);
            this.btnCopyPage.TabIndex = 14;
            this.btnCopyPage.Text = "Copy Page";
            this.btnCopyPage.UseVisualStyleBackColor = true;
            this.btnCopyPage.Click += new System.EventHandler(this.btnCopyPage_Click);
            // 
            // btnNewPage
            // 
            this.btnNewPage.Location = new System.Drawing.Point(9, 25);
            this.btnNewPage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnNewPage.Name = "btnNewPage";
            this.btnNewPage.Size = new System.Drawing.Size(140, 46);
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
            this.groupBox7.Location = new System.Drawing.Point(18, 8);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox7.Size = new System.Drawing.Size(442, 75);
            this.groupBox7.TabIndex = 18;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "General";
            // 
            // chkIsGlobal
            // 
            this.chkIsGlobal.AutoSize = true;
            this.chkIsGlobal.Location = new System.Drawing.Point(303, 34);
            this.chkIsGlobal.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkIsGlobal.Name = "chkIsGlobal";
            this.chkIsGlobal.Size = new System.Drawing.Size(126, 24);
            this.chkIsGlobal.TabIndex = 3;
            this.chkIsGlobal.Text = "Global Event";
            this.chkIsGlobal.UseVisualStyleBackColor = true;
            this.chkIsGlobal.CheckedChanged += new System.EventHandler(this.chkIsGlobal_CheckedChanged);
            // 
            // FrmEvent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1252, 957);
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
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
    }
}