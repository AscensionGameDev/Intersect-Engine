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
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Extra", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Show Text");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Show Options");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Set Switch");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Set Variable");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Conditional Branch");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("Warp Player");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmEvent));
            this.label1 = new System.Windows.Forms.Label();
            this.txtEventname = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.txtDesc = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkDisablePreview = new System.Windows.Forms.CheckBox();
            this.chkHideName = new System.Windows.Forms.CheckBox();
            this.chkWalkThrough = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cmbTrigger = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cmbLayering = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbEventFreq = new System.Windows.Forms.ComboBox();
            this.cmbEventSpeed = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSetRoute = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbMoveType = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbPreviewFace = new System.Windows.Forms.ComboBox();
            this.label25 = new System.Windows.Forms.Label();
            this.cmbEventSprite = new System.Windows.Forms.ComboBox();
            this.cmbEventDir = new System.Windows.Forms.ComboBox();
            this.grpEventConditions = new System.Windows.Forms.GroupBox();
            this.cmbCond2Val = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.cmbCond2 = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.cmbCond1Val = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.cmbCond1 = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.grpNewCommands = new System.Windows.Forms.GroupBox();
            this.lstCommands = new System.Windows.Forms.ListView();
            this.grpEventCommands = new System.Windows.Forms.GroupBox();
            this.lstEventCommands = new System.Windows.Forms.ListBox();
            this.grpCreateCommands = new System.Windows.Forms.GroupBox();
            this.grpCreateWarp = new System.Windows.Forms.GroupBox();
            this.txtNewWarpDir = new System.Windows.Forms.TextBox();
            this.txtNewWarpY = new System.Windows.Forms.TextBox();
            this.txtNewWarpX = new System.Windows.Forms.TextBox();
            this.txtNewWarpMap = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.grpNewCondition = new System.Windows.Forms.GroupBox();
            this.cmbNewCond2Val = new System.Windows.Forms.ComboBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.cmbNewCond1 = new System.Windows.Forms.ComboBox();
            this.cmbNewCond2 = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.cmbNewCond1Val = new System.Windows.Forms.ComboBox();
            this.grpSetSwitch = new System.Windows.Forms.GroupBox();
            this.cmbSetSwitchVal = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.cmbSetSwitch = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.grpShowOptions = new System.Windows.Forms.GroupBox();
            this.txtShowOptionsOpt4 = new System.Windows.Forms.TextBox();
            this.txtShowOptionsOpt3 = new System.Windows.Forms.TextBox();
            this.txtShowOptionsOpt2 = new System.Windows.Forms.TextBox();
            this.txtShowOptionsOpt1 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtShowOptions = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnCreateCancel = new System.Windows.Forms.Button();
            this.btnCreateOkay = new System.Windows.Forms.Button();
            this.grpShowText = new System.Windows.Forms.GroupBox();
            this.txtShowText = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.commandMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnInsert = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.btnDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpEventConditions.SuspendLayout();
            this.grpNewCommands.SuspendLayout();
            this.grpEventCommands.SuspendLayout();
            this.grpCreateCommands.SuspendLayout();
            this.grpCreateWarp.SuspendLayout();
            this.grpNewCondition.SuspendLayout();
            this.grpSetSwitch.SuspendLayout();
            this.grpShowOptions.SuspendLayout();
            this.grpShowText.SuspendLayout();
            this.commandMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name:";
            // 
            // txtEventname
            // 
            this.txtEventname.Location = new System.Drawing.Point(12, 23);
            this.txtEventname.Name = "txtEventname";
            this.txtEventname.Size = new System.Drawing.Size(124, 20);
            this.txtEventname.TabIndex = 2;
            this.txtEventname.TextChanged += new System.EventHandler(this.txtEventname_TextChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(12, 49);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(760, 575);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox7);
            this.tabPage1.Controls.Add(this.groupBox6);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.grpEventConditions);
            this.tabPage1.Controls.Add(this.grpNewCommands);
            this.tabPage1.Controls.Add(this.grpEventCommands);
            this.tabPage1.Controls.Add(this.grpCreateCommands);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(752, 549);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.txtDesc);
            this.groupBox7.Location = new System.Drawing.Point(105, 149);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(178, 96);
            this.groupBox7.TabIndex = 11;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Description (For entity inspector)";
            // 
            // txtDesc
            // 
            this.txtDesc.Location = new System.Drawing.Point(7, 19);
            this.txtDesc.Multiline = true;
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.Size = new System.Drawing.Size(165, 71);
            this.txtDesc.TabIndex = 0;
            this.txtDesc.TextChanged += new System.EventHandler(this.txtDesc_TextChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Location = new System.Drawing.Point(6, 149);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(93, 96);
            this.groupBox6.TabIndex = 10;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Preview";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkDisablePreview);
            this.groupBox3.Controls.Add(this.chkHideName);
            this.groupBox3.Controls.Add(this.chkWalkThrough);
            this.groupBox3.Controls.Add(this.groupBox5);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Location = new System.Drawing.Point(6, 394);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(277, 143);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Extra";
            // 
            // chkDisablePreview
            // 
            this.chkDisablePreview.Location = new System.Drawing.Point(6, 53);
            this.chkDisablePreview.Name = "chkDisablePreview";
            this.chkDisablePreview.Size = new System.Drawing.Size(97, 43);
            this.chkDisablePreview.TabIndex = 4;
            this.chkDisablePreview.Text = "Disable Targetting/Inspector";
            this.chkDisablePreview.UseVisualStyleBackColor = true;
            this.chkDisablePreview.CheckedChanged += new System.EventHandler(this.chkDisablePreview_CheckedChanged);
            // 
            // chkHideName
            // 
            this.chkHideName.AutoSize = true;
            this.chkHideName.Location = new System.Drawing.Point(6, 37);
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
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cmbTrigger);
            this.groupBox5.Location = new System.Drawing.Point(109, 79);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(162, 54);
            this.groupBox5.TabIndex = 1;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Trigger";
            // 
            // cmbTrigger
            // 
            this.cmbTrigger.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTrigger.FormattingEnabled = true;
            this.cmbTrigger.Items.AddRange(new object[] {
            "Action Button",
            "Auto"});
            this.cmbTrigger.Location = new System.Drawing.Point(8, 19);
            this.cmbTrigger.Name = "cmbTrigger";
            this.cmbTrigger.Size = new System.Drawing.Size(148, 21);
            this.cmbTrigger.TabIndex = 2;
            this.cmbTrigger.SelectedIndexChanged += new System.EventHandler(this.cmbTrigger_SelectedIndexChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cmbLayering);
            this.groupBox4.Location = new System.Drawing.Point(109, 16);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(162, 54);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Layering";
            // 
            // cmbLayering
            // 
            this.cmbLayering.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLayering.FormattingEnabled = true;
            this.cmbLayering.Items.AddRange(new object[] {
            "Below Player",
            "Same as Player",
            "Above Player"});
            this.cmbLayering.Location = new System.Drawing.Point(6, 19);
            this.cmbLayering.Name = "cmbLayering";
            this.cmbLayering.Size = new System.Drawing.Size(148, 21);
            this.cmbLayering.TabIndex = 1;
            this.cmbLayering.SelectedIndexChanged += new System.EventHandler(this.cmbLayering_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbEventFreq);
            this.groupBox2.Controls.Add(this.cmbEventSpeed);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.btnSetRoute);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.cmbMoveType);
            this.groupBox2.Location = new System.Drawing.Point(106, 251);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(177, 137);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Movement";
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
            this.cmbEventFreq.Location = new System.Drawing.Point(62, 100);
            this.cmbEventFreq.Name = "cmbEventFreq";
            this.cmbEventFreq.Size = new System.Drawing.Size(109, 21);
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
            this.cmbEventSpeed.Location = new System.Drawing.Point(62, 71);
            this.cmbEventSpeed.Name = "cmbEventSpeed";
            this.cmbEventSpeed.Size = new System.Drawing.Size(109, 21);
            this.cmbEventSpeed.TabIndex = 5;
            this.cmbEventSpeed.SelectedIndexChanged += new System.EventHandler(this.cmbEventSpeed_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Freq:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Speed:";
            // 
            // btnSetRoute
            // 
            this.btnSetRoute.Location = new System.Drawing.Point(96, 43);
            this.btnSetRoute.Name = "btnSetRoute";
            this.btnSetRoute.Size = new System.Drawing.Size(75, 23);
            this.btnSetRoute.TabIndex = 2;
            this.btnSetRoute.Text = "Set Route....";
            this.btnSetRoute.UseVisualStyleBackColor = true;
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
            "Random"});
            this.cmbMoveType.Location = new System.Drawing.Point(62, 19);
            this.cmbMoveType.Name = "cmbMoveType";
            this.cmbMoveType.Size = new System.Drawing.Size(109, 21);
            this.cmbMoveType.TabIndex = 0;
            this.cmbMoveType.SelectedIndexChanged += new System.EventHandler(this.cmbMoveType_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbPreviewFace);
            this.groupBox1.Controls.Add(this.label25);
            this.groupBox1.Controls.Add(this.cmbEventSprite);
            this.groupBox1.Controls.Add(this.cmbEventDir);
            this.groupBox1.Location = new System.Drawing.Point(7, 251);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(93, 137);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Graphic";
            // 
            // cmbPreviewFace
            // 
            this.cmbPreviewFace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPreviewFace.FormattingEnabled = true;
            this.cmbPreviewFace.Location = new System.Drawing.Point(6, 94);
            this.cmbPreviewFace.Name = "cmbPreviewFace";
            this.cmbPreviewFace.Size = new System.Drawing.Size(81, 21);
            this.cmbPreviewFace.TabIndex = 10;
            this.cmbPreviewFace.SelectedIndexChanged += new System.EventHandler(this.cmbPreviewFace_SelectedIndexChanged);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(5, 78);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(75, 13);
            this.label25.TabIndex = 9;
            this.label25.Text = "Preview Face:";
            // 
            // cmbEventSprite
            // 
            this.cmbEventSprite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEventSprite.FormattingEnabled = true;
            this.cmbEventSprite.Location = new System.Drawing.Point(6, 18);
            this.cmbEventSprite.Name = "cmbEventSprite";
            this.cmbEventSprite.Size = new System.Drawing.Size(81, 21);
            this.cmbEventSprite.TabIndex = 8;
            this.cmbEventSprite.SelectedIndexChanged += new System.EventHandler(this.cmbEventSprite_SelectedIndexChanged);
            // 
            // cmbEventDir
            // 
            this.cmbEventDir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEventDir.FormattingEnabled = true;
            this.cmbEventDir.Items.AddRange(new object[] {
            "Up",
            "Down",
            "Left",
            "Right"});
            this.cmbEventDir.Location = new System.Drawing.Point(6, 45);
            this.cmbEventDir.Name = "cmbEventDir";
            this.cmbEventDir.Size = new System.Drawing.Size(81, 21);
            this.cmbEventDir.TabIndex = 7;
            this.cmbEventDir.SelectedIndexChanged += new System.EventHandler(this.cmbEventDir_SelectedIndexChanged);
            // 
            // grpEventConditions
            // 
            this.grpEventConditions.Controls.Add(this.cmbCond2Val);
            this.grpEventConditions.Controls.Add(this.label13);
            this.grpEventConditions.Controls.Add(this.cmbCond2);
            this.grpEventConditions.Controls.Add(this.label14);
            this.grpEventConditions.Controls.Add(this.cmbCond1Val);
            this.grpEventConditions.Controls.Add(this.label12);
            this.grpEventConditions.Controls.Add(this.cmbCond1);
            this.grpEventConditions.Controls.Add(this.label11);
            this.grpEventConditions.Location = new System.Drawing.Point(6, 7);
            this.grpEventConditions.Name = "grpEventConditions";
            this.grpEventConditions.Size = new System.Drawing.Size(277, 139);
            this.grpEventConditions.TabIndex = 5;
            this.grpEventConditions.TabStop = false;
            this.grpEventConditions.Text = "Conditions";
            // 
            // cmbCond2Val
            // 
            this.cmbCond2Val.FormattingEnabled = true;
            this.cmbCond2Val.Items.AddRange(new object[] {
            "False",
            "True"});
            this.cmbCond2Val.Location = new System.Drawing.Point(200, 47);
            this.cmbCond2Val.Name = "cmbCond2Val";
            this.cmbCond2Val.Size = new System.Drawing.Size(71, 21);
            this.cmbCond2Val.TabIndex = 7;
            this.cmbCond2Val.SelectedIndexChanged += new System.EventHandler(this.cmbCond2Val_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(180, 50);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(14, 13);
            this.label13.TabIndex = 6;
            this.label13.Text = "is";
            // 
            // cmbCond2
            // 
            this.cmbCond2.FormattingEnabled = true;
            this.cmbCond2.Location = new System.Drawing.Point(59, 47);
            this.cmbCond2.Name = "cmbCond2";
            this.cmbCond2.Size = new System.Drawing.Size(115, 21);
            this.cmbCond2.TabIndex = 5;
            this.cmbCond2.SelectedIndexChanged += new System.EventHandler(this.cmbCond2_SelectedIndexChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 49);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(51, 13);
            this.label14.TabIndex = 4;
            this.label14.Text = "Switch 2:";
            // 
            // cmbCond1Val
            // 
            this.cmbCond1Val.FormattingEnabled = true;
            this.cmbCond1Val.Items.AddRange(new object[] {
            "False",
            "True"});
            this.cmbCond1Val.Location = new System.Drawing.Point(200, 20);
            this.cmbCond1Val.Name = "cmbCond1Val";
            this.cmbCond1Val.Size = new System.Drawing.Size(71, 21);
            this.cmbCond1Val.TabIndex = 3;
            this.cmbCond1Val.SelectedIndexChanged += new System.EventHandler(this.cmbCond1Val_SelectedIndexChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(180, 23);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(14, 13);
            this.label12.TabIndex = 2;
            this.label12.Text = "is";
            // 
            // cmbCond1
            // 
            this.cmbCond1.FormattingEnabled = true;
            this.cmbCond1.Location = new System.Drawing.Point(59, 20);
            this.cmbCond1.Name = "cmbCond1";
            this.cmbCond1.Size = new System.Drawing.Size(115, 21);
            this.cmbCond1.TabIndex = 1;
            this.cmbCond1.SelectedIndexChanged += new System.EventHandler(this.cmbCond1_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(51, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "Switch 1:";
            // 
            // grpNewCommands
            // 
            this.grpNewCommands.Controls.Add(this.lstCommands);
            this.grpNewCommands.Location = new System.Drawing.Point(289, 7);
            this.grpNewCommands.Name = "grpNewCommands";
            this.grpNewCommands.Size = new System.Drawing.Size(457, 536);
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
            listViewGroup3.Header = "Extra";
            listViewGroup3.Name = "Extra";
            this.lstCommands.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3});
            listViewItem1.Group = listViewGroup1;
            listViewItem1.Tag = "0";
            listViewItem2.Group = listViewGroup1;
            listViewItem2.Tag = "1";
            listViewItem3.Group = listViewGroup2;
            listViewItem3.Tag = "2";
            listViewItem4.Group = listViewGroup2;
            listViewItem4.Tag = "3";
            listViewItem5.Group = listViewGroup2;
            listViewItem5.Tag = "4";
            listViewItem6.Group = listViewGroup3;
            listViewItem6.Tag = "5";
            this.lstCommands.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6});
            this.lstCommands.Location = new System.Drawing.Point(7, 19);
            this.lstCommands.MultiSelect = false;
            this.lstCommands.Name = "lstCommands";
            this.lstCommands.Size = new System.Drawing.Size(444, 404);
            this.lstCommands.TabIndex = 0;
            this.lstCommands.TileSize = new System.Drawing.Size(80, 30);
            this.lstCommands.UseCompatibleStateImageBehavior = false;
            this.lstCommands.View = System.Windows.Forms.View.Tile;
            this.lstCommands.ItemActivate += new System.EventHandler(this.lstCommands_ItemActivated);
            this.lstCommands.SelectedIndexChanged += new System.EventHandler(this.lstCommands_SelectedIndexChanged);
            // 
            // grpEventCommands
            // 
            this.grpEventCommands.Controls.Add(this.lstEventCommands);
            this.grpEventCommands.Location = new System.Drawing.Point(289, 7);
            this.grpEventCommands.Name = "grpEventCommands";
            this.grpEventCommands.Size = new System.Drawing.Size(457, 536);
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
            this.lstEventCommands.Size = new System.Drawing.Size(445, 511);
            this.lstEventCommands.TabIndex = 0;
            this.lstEventCommands.SelectedIndexChanged += new System.EventHandler(this.lstEventCommands_SelectedIndexChanged);
            this.lstEventCommands.DoubleClick += new System.EventHandler(this.lstEventCommands_DoubleClick);
            this.lstEventCommands.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstEventCommands_KeyDown);
            this.lstEventCommands.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstEventCommands_Click);
            // 
            // grpCreateCommands
            // 
            this.grpCreateCommands.Controls.Add(this.grpCreateWarp);
            this.grpCreateCommands.Controls.Add(this.grpNewCondition);
            this.grpCreateCommands.Controls.Add(this.grpSetSwitch);
            this.grpCreateCommands.Controls.Add(this.grpShowOptions);
            this.grpCreateCommands.Controls.Add(this.btnCreateCancel);
            this.grpCreateCommands.Controls.Add(this.btnCreateOkay);
            this.grpCreateCommands.Controls.Add(this.grpShowText);
            this.grpCreateCommands.Location = new System.Drawing.Point(289, 7);
            this.grpCreateCommands.Name = "grpCreateCommands";
            this.grpCreateCommands.Size = new System.Drawing.Size(457, 536);
            this.grpCreateCommands.TabIndex = 8;
            this.grpCreateCommands.TabStop = false;
            this.grpCreateCommands.Visible = false;
            // 
            // grpCreateWarp
            // 
            this.grpCreateWarp.Controls.Add(this.txtNewWarpDir);
            this.grpCreateWarp.Controls.Add(this.txtNewWarpY);
            this.grpCreateWarp.Controls.Add(this.txtNewWarpX);
            this.grpCreateWarp.Controls.Add(this.txtNewWarpMap);
            this.grpCreateWarp.Controls.Add(this.label23);
            this.grpCreateWarp.Controls.Add(this.label22);
            this.grpCreateWarp.Controls.Add(this.label21);
            this.grpCreateWarp.Controls.Add(this.label24);
            this.grpCreateWarp.Location = new System.Drawing.Point(84, 84);
            this.grpCreateWarp.Name = "grpCreateWarp";
            this.grpCreateWarp.Size = new System.Drawing.Size(291, 195);
            this.grpCreateWarp.TabIndex = 16;
            this.grpCreateWarp.TabStop = false;
            this.grpCreateWarp.Text = "Warp";
            // 
            // txtNewWarpDir
            // 
            this.txtNewWarpDir.Location = new System.Drawing.Point(46, 97);
            this.txtNewWarpDir.Name = "txtNewWarpDir";
            this.txtNewWarpDir.Size = new System.Drawing.Size(100, 20);
            this.txtNewWarpDir.TabIndex = 18;
            // 
            // txtNewWarpY
            // 
            this.txtNewWarpY.Location = new System.Drawing.Point(46, 70);
            this.txtNewWarpY.Name = "txtNewWarpY";
            this.txtNewWarpY.Size = new System.Drawing.Size(100, 20);
            this.txtNewWarpY.TabIndex = 17;
            // 
            // txtNewWarpX
            // 
            this.txtNewWarpX.Location = new System.Drawing.Point(46, 43);
            this.txtNewWarpX.Name = "txtNewWarpX";
            this.txtNewWarpX.Size = new System.Drawing.Size(100, 20);
            this.txtNewWarpX.TabIndex = 16;
            // 
            // txtNewWarpMap
            // 
            this.txtNewWarpMap.Location = new System.Drawing.Point(46, 16);
            this.txtNewWarpMap.Name = "txtNewWarpMap";
            this.txtNewWarpMap.Size = new System.Drawing.Size(100, 20);
            this.txtNewWarpMap.TabIndex = 15;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(9, 100);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(23, 13);
            this.label23.TabIndex = 14;
            this.label23.Text = "Dir:";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(9, 73);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(17, 13);
            this.label22.TabIndex = 13;
            this.label22.Text = "Y:";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(9, 19);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(31, 13);
            this.label21.TabIndex = 8;
            this.label21.Text = "Map:";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(9, 46);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(17, 13);
            this.label24.TabIndex = 12;
            this.label24.Text = "X:";
            // 
            // grpNewCondition
            // 
            this.grpNewCondition.Controls.Add(this.cmbNewCond2Val);
            this.grpNewCondition.Controls.Add(this.label20);
            this.grpNewCondition.Controls.Add(this.label17);
            this.grpNewCondition.Controls.Add(this.cmbNewCond1);
            this.grpNewCondition.Controls.Add(this.cmbNewCond2);
            this.grpNewCondition.Controls.Add(this.label19);
            this.grpNewCondition.Controls.Add(this.label18);
            this.grpNewCondition.Controls.Add(this.cmbNewCond1Val);
            this.grpNewCondition.Location = new System.Drawing.Point(84, 84);
            this.grpNewCondition.Name = "grpNewCondition";
            this.grpNewCondition.Size = new System.Drawing.Size(291, 195);
            this.grpNewCondition.TabIndex = 8;
            this.grpNewCondition.TabStop = false;
            this.grpNewCondition.Text = "Conditional Branch";
            // 
            // cmbNewCond2Val
            // 
            this.cmbNewCond2Val.FormattingEnabled = true;
            this.cmbNewCond2Val.Items.AddRange(new object[] {
            "False",
            "True"});
            this.cmbNewCond2Val.Location = new System.Drawing.Point(206, 44);
            this.cmbNewCond2Val.Name = "cmbNewCond2Val";
            this.cmbNewCond2Val.Size = new System.Drawing.Size(71, 21);
            this.cmbNewCond2Val.TabIndex = 15;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(9, 19);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(51, 13);
            this.label20.TabIndex = 8;
            this.label20.Text = "Switch 1:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(186, 47);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(14, 13);
            this.label17.TabIndex = 14;
            this.label17.Text = "is";
            // 
            // cmbNewCond1
            // 
            this.cmbNewCond1.FormattingEnabled = true;
            this.cmbNewCond1.Location = new System.Drawing.Point(65, 17);
            this.cmbNewCond1.Name = "cmbNewCond1";
            this.cmbNewCond1.Size = new System.Drawing.Size(115, 21);
            this.cmbNewCond1.TabIndex = 9;
            // 
            // cmbNewCond2
            // 
            this.cmbNewCond2.FormattingEnabled = true;
            this.cmbNewCond2.Location = new System.Drawing.Point(65, 44);
            this.cmbNewCond2.Name = "cmbNewCond2";
            this.cmbNewCond2.Size = new System.Drawing.Size(115, 21);
            this.cmbNewCond2.TabIndex = 13;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(186, 20);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(14, 13);
            this.label19.TabIndex = 10;
            this.label19.Text = "is";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(9, 46);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(51, 13);
            this.label18.TabIndex = 12;
            this.label18.Text = "Switch 2:";
            // 
            // cmbNewCond1Val
            // 
            this.cmbNewCond1Val.FormattingEnabled = true;
            this.cmbNewCond1Val.Items.AddRange(new object[] {
            "False",
            "True"});
            this.cmbNewCond1Val.Location = new System.Drawing.Point(206, 17);
            this.cmbNewCond1Val.Name = "cmbNewCond1Val";
            this.cmbNewCond1Val.Size = new System.Drawing.Size(71, 21);
            this.cmbNewCond1Val.TabIndex = 11;
            // 
            // grpSetSwitch
            // 
            this.grpSetSwitch.Controls.Add(this.cmbSetSwitchVal);
            this.grpSetSwitch.Controls.Add(this.label15);
            this.grpSetSwitch.Controls.Add(this.cmbSetSwitch);
            this.grpSetSwitch.Controls.Add(this.label16);
            this.grpSetSwitch.Location = new System.Drawing.Point(83, 88);
            this.grpSetSwitch.Name = "grpSetSwitch";
            this.grpSetSwitch.Size = new System.Drawing.Size(291, 51);
            this.grpSetSwitch.TabIndex = 4;
            this.grpSetSwitch.TabStop = false;
            this.grpSetSwitch.Text = "Set Switch";
            // 
            // cmbSetSwitchVal
            // 
            this.cmbSetSwitchVal.FormattingEnabled = true;
            this.cmbSetSwitchVal.Items.AddRange(new object[] {
            "False",
            "True"});
            this.cmbSetSwitchVal.Location = new System.Drawing.Point(208, 19);
            this.cmbSetSwitchVal.Name = "cmbSetSwitchVal";
            this.cmbSetSwitchVal.Size = new System.Drawing.Size(71, 21);
            this.cmbSetSwitchVal.TabIndex = 7;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(188, 22);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(16, 13);
            this.label15.TabIndex = 6;
            this.label15.Text = "to";
            // 
            // cmbSetSwitch
            // 
            this.cmbSetSwitch.FormattingEnabled = true;
            this.cmbSetSwitch.Location = new System.Drawing.Point(67, 19);
            this.cmbSetSwitch.Name = "cmbSetSwitch";
            this.cmbSetSwitch.Size = new System.Drawing.Size(115, 21);
            this.cmbSetSwitch.TabIndex = 5;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(8, 21);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(61, 13);
            this.label16.TabIndex = 4;
            this.label16.Text = "Set Switch:";
            // 
            // grpShowOptions
            // 
            this.grpShowOptions.Controls.Add(this.txtShowOptionsOpt4);
            this.grpShowOptions.Controls.Add(this.txtShowOptionsOpt3);
            this.grpShowOptions.Controls.Add(this.txtShowOptionsOpt2);
            this.grpShowOptions.Controls.Add(this.txtShowOptionsOpt1);
            this.grpShowOptions.Controls.Add(this.label9);
            this.grpShowOptions.Controls.Add(this.label10);
            this.grpShowOptions.Controls.Add(this.label8);
            this.grpShowOptions.Controls.Add(this.label7);
            this.grpShowOptions.Controls.Add(this.txtShowOptions);
            this.grpShowOptions.Controls.Add(this.label6);
            this.grpShowOptions.Location = new System.Drawing.Point(84, 87);
            this.grpShowOptions.Name = "grpShowOptions";
            this.grpShowOptions.Size = new System.Drawing.Size(291, 192);
            this.grpShowOptions.TabIndex = 2;
            this.grpShowOptions.TabStop = false;
            this.grpShowOptions.Text = "Show Options";
            // 
            // txtShowOptionsOpt4
            // 
            this.txtShowOptionsOpt4.Location = new System.Drawing.Point(173, 157);
            this.txtShowOptionsOpt4.Name = "txtShowOptionsOpt4";
            this.txtShowOptionsOpt4.Size = new System.Drawing.Size(100, 20);
            this.txtShowOptionsOpt4.TabIndex = 9;
            // 
            // txtShowOptionsOpt3
            // 
            this.txtShowOptionsOpt3.Location = new System.Drawing.Point(11, 157);
            this.txtShowOptionsOpt3.Name = "txtShowOptionsOpt3";
            this.txtShowOptionsOpt3.Size = new System.Drawing.Size(100, 20);
            this.txtShowOptionsOpt3.TabIndex = 8;
            // 
            // txtShowOptionsOpt2
            // 
            this.txtShowOptionsOpt2.Location = new System.Drawing.Point(174, 118);
            this.txtShowOptionsOpt2.Name = "txtShowOptionsOpt2";
            this.txtShowOptionsOpt2.Size = new System.Drawing.Size(100, 20);
            this.txtShowOptionsOpt2.TabIndex = 7;
            // 
            // txtShowOptionsOpt1
            // 
            this.txtShowOptionsOpt1.Location = new System.Drawing.Point(12, 118);
            this.txtShowOptionsOpt1.Name = "txtShowOptionsOpt1";
            this.txtShowOptionsOpt1.Size = new System.Drawing.Size(100, 20);
            this.txtShowOptionsOpt1.TabIndex = 6;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(171, 141);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(50, 13);
            this.label9.TabIndex = 5;
            this.label9.Text = "Option 4:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(9, 141);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(50, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "Option 3:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(171, 104);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Option 2:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 104);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Option 1:";
            // 
            // txtShowOptions
            // 
            this.txtShowOptions.Location = new System.Drawing.Point(45, 17);
            this.txtShowOptions.Multiline = true;
            this.txtShowOptions.Name = "txtShowOptions";
            this.txtShowOptions.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtShowOptions.Size = new System.Drawing.Size(234, 82);
            this.txtShowOptions.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Text:";
            // 
            // btnCreateCancel
            // 
            this.btnCreateCancel.Location = new System.Drawing.Point(7, 10);
            this.btnCreateCancel.Name = "btnCreateCancel";
            this.btnCreateCancel.Size = new System.Drawing.Size(109, 40);
            this.btnCreateCancel.TabIndex = 3;
            this.btnCreateCancel.Text = "Back";
            this.btnCreateCancel.UseVisualStyleBackColor = true;
            this.btnCreateCancel.Click += new System.EventHandler(this.btnCreateCancel_Click);
            // 
            // btnCreateOkay
            // 
            this.btnCreateOkay.Location = new System.Drawing.Point(348, 10);
            this.btnCreateOkay.Name = "btnCreateOkay";
            this.btnCreateOkay.Size = new System.Drawing.Size(109, 40);
            this.btnCreateOkay.TabIndex = 2;
            this.btnCreateOkay.Text = "Save";
            this.btnCreateOkay.UseVisualStyleBackColor = true;
            this.btnCreateOkay.Click += new System.EventHandler(this.btnCreateOkay_Click);
            // 
            // grpShowText
            // 
            this.grpShowText.Controls.Add(this.txtShowText);
            this.grpShowText.Controls.Add(this.label5);
            this.grpShowText.Location = new System.Drawing.Point(85, 87);
            this.grpShowText.Name = "grpShowText";
            this.grpShowText.Size = new System.Drawing.Size(291, 132);
            this.grpShowText.TabIndex = 0;
            this.grpShowText.TabStop = false;
            this.grpShowText.Text = "Show Text";
            // 
            // txtShowText
            // 
            this.txtShowText.Location = new System.Drawing.Point(45, 17);
            this.txtShowText.Multiline = true;
            this.txtShowText.Name = "txtShowText";
            this.txtShowText.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtShowText.Size = new System.Drawing.Size(234, 100);
            this.txtShowText.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Text:";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(576, 630);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(93, 30);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(675, 630);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(93, 30);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(192, 12);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(93, 30);
            this.button3.TabIndex = 8;
            this.button3.Text = "New Page";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Enabled = false;
            this.button4.Location = new System.Drawing.Point(291, 12);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(93, 30);
            this.button4.TabIndex = 9;
            this.button4.Text = "Copy Page";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Enabled = false;
            this.button5.Location = new System.Drawing.Point(390, 12);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(93, 30);
            this.button5.TabIndex = 10;
            this.button5.Text = "Paste Page";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Enabled = false;
            this.button6.Location = new System.Drawing.Point(489, 12);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(93, 30);
            this.button6.TabIndex = 11;
            this.button6.Text = "Delete Page";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Enabled = false;
            this.button7.Location = new System.Drawing.Point(588, 12);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(93, 30);
            this.button7.TabIndex = 12;
            this.button7.Text = "Clear Page";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // commandMenu
            // 
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
            // FrmEvent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 662);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.txtEventname);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FrmEvent";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Event Editor";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmEvent_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmEvent_FormClosed);
            this.Load += new System.EventHandler(this.frmEvent_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpEventConditions.ResumeLayout(false);
            this.grpEventConditions.PerformLayout();
            this.grpNewCommands.ResumeLayout(false);
            this.grpEventCommands.ResumeLayout(false);
            this.grpCreateCommands.ResumeLayout(false);
            this.grpCreateWarp.ResumeLayout(false);
            this.grpCreateWarp.PerformLayout();
            this.grpNewCondition.ResumeLayout(false);
            this.grpNewCondition.PerformLayout();
            this.grpSetSwitch.ResumeLayout(false);
            this.grpSetSwitch.PerformLayout();
            this.grpShowOptions.ResumeLayout(false);
            this.grpShowOptions.PerformLayout();
            this.grpShowText.ResumeLayout(false);
            this.grpShowText.PerformLayout();
            this.commandMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private TextBox txtEventname;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private GroupBox grpEventCommands;
        private ListBox lstEventCommands;
        private GroupBox grpEventConditions;
        private GroupBox groupBox3;
        private GroupBox groupBox2;
        private GroupBox groupBox1;
        private Button btnSave;
        private Button btnCancel;
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
        private Button button7;
        private ComboBox cmbEventFreq;
        private ComboBox cmbEventSpeed;
        private Label label4;
        private Label label3;
        private Button btnSetRoute;
        private Label label2;
        private ComboBox cmbMoveType;
        private GroupBox groupBox5;
        private ComboBox cmbTrigger;
        private GroupBox groupBox4;
        private ComboBox cmbLayering;
        private CheckBox chkWalkThrough;
        private GroupBox grpNewCommands;
        private ListView lstCommands;
        private GroupBox grpCreateCommands;
        private GroupBox grpShowText;
        private TextBox txtShowText;
        private Label label5;
        private Button btnCreateOkay;
        private Button btnCreateCancel;
        private GroupBox grpShowOptions;
        private TextBox txtShowOptionsOpt4;
        private TextBox txtShowOptionsOpt3;
        private TextBox txtShowOptionsOpt2;
        private TextBox txtShowOptionsOpt1;
        private Label label9;
        private Label label10;
        private Label label8;
        private Label label7;
        private TextBox txtShowOptions;
        private Label label6;
        private ContextMenuStrip commandMenu;
        private ToolStripMenuItem btnInsert;
        private ToolStripMenuItem btnEdit;
        private ToolStripMenuItem btnDelete;
        private ComboBox cmbCond2Val;
        private Label label13;
        private ComboBox cmbCond2;
        private Label label14;
        private ComboBox cmbCond1Val;
        private Label label12;
        private ComboBox cmbCond1;
        private Label label11;
        private GroupBox grpSetSwitch;
        private ComboBox cmbSetSwitchVal;
        private Label label15;
        private ComboBox cmbSetSwitch;
        private Label label16;
        private GroupBox grpNewCondition;
        private ComboBox cmbNewCond2Val;
        private Label label20;
        private Label label17;
        private ComboBox cmbNewCond1;
        private ComboBox cmbNewCond2;
        private Label label19;
        private Label label18;
        private ComboBox cmbNewCond1Val;
        private CheckBox chkHideName;
        private GroupBox grpCreateWarp;
        private TextBox txtNewWarpDir;
        private TextBox txtNewWarpY;
        private TextBox txtNewWarpX;
        private TextBox txtNewWarpMap;
        private Label label23;
        private Label label22;
        private Label label21;
        private Label label24;
        private ComboBox cmbEventDir;
        private CheckBox chkDisablePreview;
        private ComboBox cmbPreviewFace;
        private Label label25;
        private ComboBox cmbEventSprite;
        private GroupBox groupBox7;
        private TextBox txtDesc;
        private GroupBox groupBox6;
    }
}