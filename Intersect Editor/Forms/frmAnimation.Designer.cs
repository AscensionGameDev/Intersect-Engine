namespace Intersect_Editor.Forms
{
    partial class frmAnimation
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lstAnimations = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSound = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.scrlLowerLoopCount = new System.Windows.Forms.HScrollBar();
            this.lblLowerLoopCount = new System.Windows.Forms.Label();
            this.scrlLowerFrameDuration = new System.Windows.Forms.HScrollBar();
            this.lblLowerFrameDuration = new System.Windows.Forms.Label();
            this.scrlLowerFrameCount = new System.Windows.Forms.HScrollBar();
            this.lblLowerFrameCount = new System.Windows.Forms.Label();
            this.scrlLowerVerticalFrames = new System.Windows.Forms.HScrollBar();
            this.scrlLowerHorizontalFrames = new System.Windows.Forms.HScrollBar();
            this.lblLowerVerticalFrames = new System.Windows.Forms.Label();
            this.lblLowerHorizontalFrames = new System.Windows.Forms.Label();
            this.cmbLowerGraphic = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.picLowerAnimation = new System.Windows.Forms.PictureBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.scrlUpperLoopCount = new System.Windows.Forms.HScrollBar();
            this.lblUpperLoopCount = new System.Windows.Forms.Label();
            this.scrlUpperFrameDuration = new System.Windows.Forms.HScrollBar();
            this.lblUpperFrameDuration = new System.Windows.Forms.Label();
            this.scrlUpperFrameCount = new System.Windows.Forms.HScrollBar();
            this.lblUpperFrameCount = new System.Windows.Forms.Label();
            this.scrlUpperVerticalFrames = new System.Windows.Forms.HScrollBar();
            this.scrlUpperHorizontalFrames = new System.Windows.Forms.HScrollBar();
            this.lblUpperVerticalFrames = new System.Windows.Forms.Label();
            this.lblUpperHorizontalFrames = new System.Windows.Forms.Label();
            this.cmbUpperGraphic = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.picUpperAnimation = new System.Windows.Forms.PictureBox();
            this.tmrUpperAnimation = new System.Windows.Forms.Timer(this.components);
            this.tmrLowerAnimation = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLowerAnimation)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUpperAnimation)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnDelete);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.lstAnimations);
            this.groupBox1.Location = new System.Drawing.Point(3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 504);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Animations";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(6, 468);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(190, 27);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDelete.Location = new System.Drawing.Point(7, 436);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(190, 27);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(6, 403);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(190, 27);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lstAnimations
            // 
            this.lstAnimations.FormattingEnabled = true;
            this.lstAnimations.Location = new System.Drawing.Point(6, 19);
            this.lstAnimations.Name = "lstAnimations";
            this.lstAnimations.Size = new System.Drawing.Size(191, 381);
            this.lstAnimations.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.cmbSound);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtName);
            this.groupBox2.Location = new System.Drawing.Point(216, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(434, 76);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "General";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Sound:";
            // 
            // cmbSound
            // 
            this.cmbSound.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSound.FormattingEnabled = true;
            this.cmbSound.Items.AddRange(new object[] {
            "None"});
            this.cmbSound.Location = new System.Drawing.Point(60, 45);
            this.cmbSound.Name = "cmbSound";
            this.cmbSound.Size = new System.Drawing.Size(368, 21);
            this.cmbSound.TabIndex = 2;
            this.cmbSound.SelectedIndexChanged += new System.EventHandler(this.cmbSound_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(60, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(368, 20);
            this.txtName.TabIndex = 0;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.scrlLowerLoopCount);
            this.groupBox3.Controls.Add(this.lblLowerLoopCount);
            this.groupBox3.Controls.Add(this.scrlLowerFrameDuration);
            this.groupBox3.Controls.Add(this.lblLowerFrameDuration);
            this.groupBox3.Controls.Add(this.scrlLowerFrameCount);
            this.groupBox3.Controls.Add(this.lblLowerFrameCount);
            this.groupBox3.Controls.Add(this.scrlLowerVerticalFrames);
            this.groupBox3.Controls.Add(this.scrlLowerHorizontalFrames);
            this.groupBox3.Controls.Add(this.lblLowerVerticalFrames);
            this.groupBox3.Controls.Add(this.lblLowerHorizontalFrames);
            this.groupBox3.Controls.Add(this.cmbLowerGraphic);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.picLowerAnimation);
            this.groupBox3.Location = new System.Drawing.Point(216, 85);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(214, 421);
            this.groupBox3.TabIndex = 19;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Lower Layer (Below Target)";
            // 
            // scrlLowerLoopCount
            // 
            this.scrlLowerLoopCount.LargeChange = 1;
            this.scrlLowerLoopCount.Location = new System.Drawing.Point(7, 395);
            this.scrlLowerLoopCount.Maximum = 10;
            this.scrlLowerLoopCount.Minimum = 1;
            this.scrlLowerLoopCount.Name = "scrlLowerLoopCount";
            this.scrlLowerLoopCount.Size = new System.Drawing.Size(197, 17);
            this.scrlLowerLoopCount.TabIndex = 13;
            this.scrlLowerLoopCount.Value = 1;
            this.scrlLowerLoopCount.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlLowerLoopCount_Scroll);
            // 
            // lblLowerLoopCount
            // 
            this.lblLowerLoopCount.AutoSize = true;
            this.lblLowerLoopCount.Location = new System.Drawing.Point(7, 382);
            this.lblLowerLoopCount.Name = "lblLowerLoopCount";
            this.lblLowerLoopCount.Size = new System.Drawing.Size(74, 13);
            this.lblLowerLoopCount.TabIndex = 12;
            this.lblLowerLoopCount.Text = "Loop Count: 1";
            // 
            // scrlLowerFrameDuration
            // 
            this.scrlLowerFrameDuration.LargeChange = 1;
            this.scrlLowerFrameDuration.Location = new System.Drawing.Point(7, 363);
            this.scrlLowerFrameDuration.Maximum = 2000;
            this.scrlLowerFrameDuration.Minimum = 10;
            this.scrlLowerFrameDuration.Name = "scrlLowerFrameDuration";
            this.scrlLowerFrameDuration.Size = new System.Drawing.Size(197, 17);
            this.scrlLowerFrameDuration.TabIndex = 11;
            this.scrlLowerFrameDuration.Value = 10;
            this.scrlLowerFrameDuration.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlLowerFrameDuration_Scroll);
            // 
            // lblLowerFrameDuration
            // 
            this.lblLowerFrameDuration.AutoSize = true;
            this.lblLowerFrameDuration.Location = new System.Drawing.Point(7, 350);
            this.lblLowerFrameDuration.Name = "lblLowerFrameDuration";
            this.lblLowerFrameDuration.Size = new System.Drawing.Size(125, 13);
            this.lblLowerFrameDuration.TabIndex = 10;
            this.lblLowerFrameDuration.Text = "Frame Duration (ms): 100";
            // 
            // scrlLowerFrameCount
            // 
            this.scrlLowerFrameCount.LargeChange = 1;
            this.scrlLowerFrameCount.Location = new System.Drawing.Point(7, 330);
            this.scrlLowerFrameCount.Minimum = 1;
            this.scrlLowerFrameCount.Name = "scrlLowerFrameCount";
            this.scrlLowerFrameCount.Size = new System.Drawing.Size(197, 17);
            this.scrlLowerFrameCount.TabIndex = 9;
            this.scrlLowerFrameCount.Value = 1;
            this.scrlLowerFrameCount.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlLowerFrameCount_Scroll);
            // 
            // lblLowerFrameCount
            // 
            this.lblLowerFrameCount.AutoSize = true;
            this.lblLowerFrameCount.Location = new System.Drawing.Point(7, 317);
            this.lblLowerFrameCount.Name = "lblLowerFrameCount";
            this.lblLowerFrameCount.Size = new System.Drawing.Size(119, 13);
            this.lblLowerFrameCount.TabIndex = 8;
            this.lblLowerFrameCount.Text = "Graphic Frame Count: 1";
            // 
            // scrlLowerVerticalFrames
            // 
            this.scrlLowerVerticalFrames.LargeChange = 1;
            this.scrlLowerVerticalFrames.Location = new System.Drawing.Point(7, 296);
            this.scrlLowerVerticalFrames.Maximum = 10;
            this.scrlLowerVerticalFrames.Minimum = 1;
            this.scrlLowerVerticalFrames.Name = "scrlLowerVerticalFrames";
            this.scrlLowerVerticalFrames.Size = new System.Drawing.Size(197, 17);
            this.scrlLowerVerticalFrames.TabIndex = 7;
            this.scrlLowerVerticalFrames.Value = 1;
            this.scrlLowerVerticalFrames.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlLowerVerticalFrames_Scroll);
            // 
            // scrlLowerHorizontalFrames
            // 
            this.scrlLowerHorizontalFrames.LargeChange = 1;
            this.scrlLowerHorizontalFrames.Location = new System.Drawing.Point(7, 260);
            this.scrlLowerHorizontalFrames.Maximum = 10;
            this.scrlLowerHorizontalFrames.Minimum = 1;
            this.scrlLowerHorizontalFrames.Name = "scrlLowerHorizontalFrames";
            this.scrlLowerHorizontalFrames.Size = new System.Drawing.Size(197, 17);
            this.scrlLowerHorizontalFrames.TabIndex = 6;
            this.scrlLowerHorizontalFrames.Value = 1;
            this.scrlLowerHorizontalFrames.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlLowerHorizontalFrames_Scroll);
            // 
            // lblLowerVerticalFrames
            // 
            this.lblLowerVerticalFrames.AutoSize = true;
            this.lblLowerVerticalFrames.Location = new System.Drawing.Point(7, 283);
            this.lblLowerVerticalFrames.Name = "lblLowerVerticalFrames";
            this.lblLowerVerticalFrames.Size = new System.Drawing.Size(131, 13);
            this.lblLowerVerticalFrames.TabIndex = 5;
            this.lblLowerVerticalFrames.Text = "Graphic Vertical Frames: 1";
            // 
            // lblLowerHorizontalFrames
            // 
            this.lblLowerHorizontalFrames.AutoSize = true;
            this.lblLowerHorizontalFrames.Location = new System.Drawing.Point(7, 247);
            this.lblLowerHorizontalFrames.Name = "lblLowerHorizontalFrames";
            this.lblLowerHorizontalFrames.Size = new System.Drawing.Size(143, 13);
            this.lblLowerHorizontalFrames.TabIndex = 4;
            this.lblLowerHorizontalFrames.Text = "Graphic Horizontal Frames: 1";
            // 
            // cmbLowerGraphic
            // 
            this.cmbLowerGraphic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLowerGraphic.FormattingEnabled = true;
            this.cmbLowerGraphic.Items.AddRange(new object[] {
            "None"});
            this.cmbLowerGraphic.Location = new System.Drawing.Point(58, 223);
            this.cmbLowerGraphic.Name = "cmbLowerGraphic";
            this.cmbLowerGraphic.Size = new System.Drawing.Size(149, 21);
            this.cmbLowerGraphic.TabIndex = 3;
            this.cmbLowerGraphic.SelectedIndexChanged += new System.EventHandler(this.cmbLowerGraphic_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 226);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Graphic: ";
            // 
            // picLowerAnimation
            // 
            this.picLowerAnimation.Location = new System.Drawing.Point(7, 19);
            this.picLowerAnimation.Name = "picLowerAnimation";
            this.picLowerAnimation.Size = new System.Drawing.Size(200, 200);
            this.picLowerAnimation.TabIndex = 0;
            this.picLowerAnimation.TabStop = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.scrlUpperLoopCount);
            this.groupBox4.Controls.Add(this.lblUpperLoopCount);
            this.groupBox4.Controls.Add(this.scrlUpperFrameDuration);
            this.groupBox4.Controls.Add(this.lblUpperFrameDuration);
            this.groupBox4.Controls.Add(this.scrlUpperFrameCount);
            this.groupBox4.Controls.Add(this.lblUpperFrameCount);
            this.groupBox4.Controls.Add(this.scrlUpperVerticalFrames);
            this.groupBox4.Controls.Add(this.scrlUpperHorizontalFrames);
            this.groupBox4.Controls.Add(this.lblUpperVerticalFrames);
            this.groupBox4.Controls.Add(this.lblUpperHorizontalFrames);
            this.groupBox4.Controls.Add(this.cmbUpperGraphic);
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Controls.Add(this.picUpperAnimation);
            this.groupBox4.Location = new System.Drawing.Point(436, 85);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(214, 421);
            this.groupBox4.TabIndex = 20;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Upper Layer (Above Target)";
            // 
            // scrlUpperLoopCount
            // 
            this.scrlUpperLoopCount.LargeChange = 1;
            this.scrlUpperLoopCount.Location = new System.Drawing.Point(6, 395);
            this.scrlUpperLoopCount.Maximum = 10;
            this.scrlUpperLoopCount.Minimum = 1;
            this.scrlUpperLoopCount.Name = "scrlUpperLoopCount";
            this.scrlUpperLoopCount.Size = new System.Drawing.Size(197, 17);
            this.scrlUpperLoopCount.TabIndex = 25;
            this.scrlUpperLoopCount.Value = 1;
            this.scrlUpperLoopCount.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlUpperLoopCount_Scroll);
            // 
            // lblUpperLoopCount
            // 
            this.lblUpperLoopCount.AutoSize = true;
            this.lblUpperLoopCount.Location = new System.Drawing.Point(6, 382);
            this.lblUpperLoopCount.Name = "lblUpperLoopCount";
            this.lblUpperLoopCount.Size = new System.Drawing.Size(74, 13);
            this.lblUpperLoopCount.TabIndex = 24;
            this.lblUpperLoopCount.Text = "Loop Count: 1";
            // 
            // scrlUpperFrameDuration
            // 
            this.scrlUpperFrameDuration.LargeChange = 1;
            this.scrlUpperFrameDuration.Location = new System.Drawing.Point(6, 363);
            this.scrlUpperFrameDuration.Maximum = 2000;
            this.scrlUpperFrameDuration.Minimum = 10;
            this.scrlUpperFrameDuration.Name = "scrlUpperFrameDuration";
            this.scrlUpperFrameDuration.Size = new System.Drawing.Size(197, 17);
            this.scrlUpperFrameDuration.TabIndex = 23;
            this.scrlUpperFrameDuration.Value = 10;
            this.scrlUpperFrameDuration.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlUpperFrameDuration_Scroll);
            // 
            // lblUpperFrameDuration
            // 
            this.lblUpperFrameDuration.AutoSize = true;
            this.lblUpperFrameDuration.Location = new System.Drawing.Point(6, 350);
            this.lblUpperFrameDuration.Name = "lblUpperFrameDuration";
            this.lblUpperFrameDuration.Size = new System.Drawing.Size(125, 13);
            this.lblUpperFrameDuration.TabIndex = 22;
            this.lblUpperFrameDuration.Text = "Frame Duration (ms): 100";
            // 
            // scrlUpperFrameCount
            // 
            this.scrlUpperFrameCount.LargeChange = 1;
            this.scrlUpperFrameCount.Location = new System.Drawing.Point(6, 330);
            this.scrlUpperFrameCount.Minimum = 1;
            this.scrlUpperFrameCount.Name = "scrlUpperFrameCount";
            this.scrlUpperFrameCount.Size = new System.Drawing.Size(197, 17);
            this.scrlUpperFrameCount.TabIndex = 21;
            this.scrlUpperFrameCount.Value = 1;
            this.scrlUpperFrameCount.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlUpperFrameCount_Scroll);
            // 
            // lblUpperFrameCount
            // 
            this.lblUpperFrameCount.AutoSize = true;
            this.lblUpperFrameCount.Location = new System.Drawing.Point(6, 317);
            this.lblUpperFrameCount.Name = "lblUpperFrameCount";
            this.lblUpperFrameCount.Size = new System.Drawing.Size(119, 13);
            this.lblUpperFrameCount.TabIndex = 20;
            this.lblUpperFrameCount.Text = "Graphic Frame Count: 1";
            // 
            // scrlUpperVerticalFrames
            // 
            this.scrlUpperVerticalFrames.LargeChange = 1;
            this.scrlUpperVerticalFrames.Location = new System.Drawing.Point(6, 296);
            this.scrlUpperVerticalFrames.Maximum = 10;
            this.scrlUpperVerticalFrames.Minimum = 1;
            this.scrlUpperVerticalFrames.Name = "scrlUpperVerticalFrames";
            this.scrlUpperVerticalFrames.Size = new System.Drawing.Size(197, 17);
            this.scrlUpperVerticalFrames.TabIndex = 19;
            this.scrlUpperVerticalFrames.Value = 1;
            this.scrlUpperVerticalFrames.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlUpperVerticalFrames_Scroll);
            // 
            // scrlUpperHorizontalFrames
            // 
            this.scrlUpperHorizontalFrames.LargeChange = 1;
            this.scrlUpperHorizontalFrames.Location = new System.Drawing.Point(6, 260);
            this.scrlUpperHorizontalFrames.Maximum = 10;
            this.scrlUpperHorizontalFrames.Minimum = 1;
            this.scrlUpperHorizontalFrames.Name = "scrlUpperHorizontalFrames";
            this.scrlUpperHorizontalFrames.Size = new System.Drawing.Size(197, 17);
            this.scrlUpperHorizontalFrames.TabIndex = 18;
            this.scrlUpperHorizontalFrames.Value = 1;
            this.scrlUpperHorizontalFrames.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlUpperHorizontalFrames_Scroll);
            // 
            // lblUpperVerticalFrames
            // 
            this.lblUpperVerticalFrames.AutoSize = true;
            this.lblUpperVerticalFrames.Location = new System.Drawing.Point(6, 283);
            this.lblUpperVerticalFrames.Name = "lblUpperVerticalFrames";
            this.lblUpperVerticalFrames.Size = new System.Drawing.Size(131, 13);
            this.lblUpperVerticalFrames.TabIndex = 17;
            this.lblUpperVerticalFrames.Text = "Graphic Vertical Frames: 1";
            // 
            // lblUpperHorizontalFrames
            // 
            this.lblUpperHorizontalFrames.AutoSize = true;
            this.lblUpperHorizontalFrames.Location = new System.Drawing.Point(6, 247);
            this.lblUpperHorizontalFrames.Name = "lblUpperHorizontalFrames";
            this.lblUpperHorizontalFrames.Size = new System.Drawing.Size(143, 13);
            this.lblUpperHorizontalFrames.TabIndex = 16;
            this.lblUpperHorizontalFrames.Text = "Graphic Horizontal Frames: 1";
            // 
            // cmbUpperGraphic
            // 
            this.cmbUpperGraphic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUpperGraphic.FormattingEnabled = true;
            this.cmbUpperGraphic.Items.AddRange(new object[] {
            "None"});
            this.cmbUpperGraphic.Location = new System.Drawing.Point(57, 223);
            this.cmbUpperGraphic.Name = "cmbUpperGraphic";
            this.cmbUpperGraphic.Size = new System.Drawing.Size(149, 21);
            this.cmbUpperGraphic.TabIndex = 15;
            this.cmbUpperGraphic.SelectedIndexChanged += new System.EventHandler(this.cmbUpperGraphic_SelectedIndexChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 226);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(50, 13);
            this.label14.TabIndex = 14;
            this.label14.Text = "Graphic: ";
            // 
            // picUpperAnimation
            // 
            this.picUpperAnimation.Location = new System.Drawing.Point(6, 19);
            this.picUpperAnimation.Name = "picUpperAnimation";
            this.picUpperAnimation.Size = new System.Drawing.Size(200, 200);
            this.picUpperAnimation.TabIndex = 1;
            this.picUpperAnimation.TabStop = false;
            // 
            // tmrUpperAnimation
            // 
            this.tmrUpperAnimation.Enabled = true;
            this.tmrUpperAnimation.Tick += new System.EventHandler(this.tmrUpperAnimation_Tick);
            // 
            // tmrLowerAnimation
            // 
            this.tmrLowerAnimation.Enabled = true;
            this.tmrLowerAnimation.Tick += new System.EventHandler(this.tmrLowerAnimation_Tick);
            // 
            // frmAnimation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 518);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmAnimation";
            this.Text = "Animation Editor";
            this.Load += new System.EventHandler(this.frmAnimation_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLowerAnimation)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUpperAnimation)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ListBox lstAnimations;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSound;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.HScrollBar scrlLowerLoopCount;
        private System.Windows.Forms.Label lblLowerLoopCount;
        private System.Windows.Forms.HScrollBar scrlLowerFrameDuration;
        private System.Windows.Forms.Label lblLowerFrameDuration;
        private System.Windows.Forms.HScrollBar scrlLowerFrameCount;
        private System.Windows.Forms.Label lblLowerFrameCount;
        private System.Windows.Forms.HScrollBar scrlLowerVerticalFrames;
        private System.Windows.Forms.HScrollBar scrlLowerHorizontalFrames;
        private System.Windows.Forms.Label lblLowerVerticalFrames;
        private System.Windows.Forms.Label lblLowerHorizontalFrames;
        private System.Windows.Forms.ComboBox cmbLowerGraphic;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox picLowerAnimation;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.HScrollBar scrlUpperLoopCount;
        private System.Windows.Forms.Label lblUpperLoopCount;
        private System.Windows.Forms.HScrollBar scrlUpperFrameDuration;
        private System.Windows.Forms.Label lblUpperFrameDuration;
        private System.Windows.Forms.HScrollBar scrlUpperFrameCount;
        private System.Windows.Forms.Label lblUpperFrameCount;
        private System.Windows.Forms.HScrollBar scrlUpperVerticalFrames;
        private System.Windows.Forms.HScrollBar scrlUpperHorizontalFrames;
        private System.Windows.Forms.Label lblUpperVerticalFrames;
        private System.Windows.Forms.Label lblUpperHorizontalFrames;
        private System.Windows.Forms.ComboBox cmbUpperGraphic;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.PictureBox picUpperAnimation;
        private System.Windows.Forms.Timer tmrUpperAnimation;
        private System.Windows.Forms.Timer tmrLowerAnimation;
    }
}