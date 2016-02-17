namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    partial class EventCommand_Warp
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnVisual = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.scrlY = new System.Windows.Forms.HScrollBar();
            this.scrlX = new System.Windows.Forms.HScrollBar();
            this.cmbMap = new System.Windows.Forms.ComboBox();
            this.cmbDirection = new System.Windows.Forms.ComboBox();
            this.label23 = new System.Windows.Forms.Label();
            this.lblY = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.lblX = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnVisual);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.scrlY);
            this.groupBox1.Controls.Add(this.scrlX);
            this.groupBox1.Controls.Add(this.cmbMap);
            this.groupBox1.Controls.Add(this.cmbDirection);
            this.groupBox1.Controls.Add(this.label23);
            this.groupBox1.Controls.Add(this.lblY);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.lblX);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(182, 195);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Warp";
            // 
            // btnVisual
            // 
            this.btnVisual.Location = new System.Drawing.Point(12, 130);
            this.btnVisual.Name = "btnVisual";
            this.btnVisual.Size = new System.Drawing.Size(155, 23);
            this.btnVisual.TabIndex = 21;
            this.btnVisual.Text = "Open Visual Interface";
            this.btnVisual.UseVisualStyleBackColor = true;
            this.btnVisual.Click += new System.EventHandler(this.btnVisual_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(87, 166);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(6, 166);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // scrlY
            // 
            this.scrlY.Location = new System.Drawing.Point(46, 73);
            this.scrlY.Name = "scrlY";
            this.scrlY.Size = new System.Drawing.Size(121, 17);
            this.scrlY.TabIndex = 18;
            this.scrlY.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlY_Scroll);
            // 
            // scrlX
            // 
            this.scrlX.Location = new System.Drawing.Point(46, 46);
            this.scrlX.Name = "scrlX";
            this.scrlX.Size = new System.Drawing.Size(121, 17);
            this.scrlX.TabIndex = 17;
            this.scrlX.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlX_Scroll);
            // 
            // cmbMap
            // 
            this.cmbMap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMap.FormattingEnabled = true;
            this.cmbMap.Items.AddRange(new object[] {
            "Retain Direction",
            "Up",
            "Down",
            "Left",
            "Right"});
            this.cmbMap.Location = new System.Drawing.Point(46, 16);
            this.cmbMap.Name = "cmbMap";
            this.cmbMap.Size = new System.Drawing.Size(121, 21);
            this.cmbMap.TabIndex = 16;
            this.cmbMap.SelectedIndexChanged += new System.EventHandler(this.cmbMap_SelectedIndexChanged);
            // 
            // cmbDirection
            // 
            this.cmbDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDirection.FormattingEnabled = true;
            this.cmbDirection.Items.AddRange(new object[] {
            "Retain Direction",
            "Up",
            "Down",
            "Left",
            "Right"});
            this.cmbDirection.Location = new System.Drawing.Point(46, 97);
            this.cmbDirection.Name = "cmbDirection";
            this.cmbDirection.Size = new System.Drawing.Size(121, 21);
            this.cmbDirection.TabIndex = 15;
            this.cmbDirection.SelectedIndexChanged += new System.EventHandler(this.cmbDirection_SelectedIndexChanged);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(9, 100);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(23, 13);
            this.label23.TabIndex = 14;
            this.label23.Text = "Dir:";
            this.label23.Click += new System.EventHandler(this.label23_Click);
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.Location = new System.Drawing.Point(9, 73);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(26, 13);
            this.lblY.TabIndex = 13;
            this.lblY.Text = "Y: 0";
            this.lblY.Click += new System.EventHandler(this.lblY_Click);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(9, 19);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(31, 13);
            this.label21.TabIndex = 8;
            this.label21.Text = "Map:";
            this.label21.Click += new System.EventHandler(this.label21_Click);
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Location = new System.Drawing.Point(9, 46);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(26, 13);
            this.lblX.TabIndex = 12;
            this.lblX.Text = "X: 0";
            this.lblX.Click += new System.EventHandler(this.lblX_Click);
            // 
            // EventCommand_Warp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.groupBox1);
            this.Name = "EventCommand_Warp";
            this.Size = new System.Drawing.Size(188, 201);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cmbMap;
        private System.Windows.Forms.ComboBox cmbDirection;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label lblX;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.HScrollBar scrlY;
        private System.Windows.Forms.HScrollBar scrlX;
        private System.Windows.Forms.Button btnVisual;
    }
}
