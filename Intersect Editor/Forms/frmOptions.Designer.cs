namespace Intersect_Editor.Forms
{
    partial class frmOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOptions));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.chkSuppressTilesetWarning = new System.Windows.Forms.CheckBox();
            this.btnBrowseClient = new System.Windows.Forms.Button();
            this.txtGamePath = new System.Windows.Forms.TextBox();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Location = new System.Drawing.Point(5, 4);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(365, 164);
            this.tabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txtGamePath);
            this.tabPage1.Controls.Add(this.btnBrowseClient);
            this.tabPage1.Controls.Add(this.chkSuppressTilesetWarning);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(357, 138);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // chkSuppressTilesetWarning
            // 
            this.chkSuppressTilesetWarning.AutoSize = true;
            this.chkSuppressTilesetWarning.Location = new System.Drawing.Point(7, 7);
            this.chkSuppressTilesetWarning.Name = "chkSuppressTilesetWarning";
            this.chkSuppressTilesetWarning.Size = new System.Drawing.Size(190, 17);
            this.chkSuppressTilesetWarning.TabIndex = 0;
            this.chkSuppressTilesetWarning.Text = "Suppress large tileset size warning.";
            this.chkSuppressTilesetWarning.UseVisualStyleBackColor = true;
            this.chkSuppressTilesetWarning.CheckedChanged += new System.EventHandler(this.chkSuppressTilesetWarning_CheckedChanged);
            // 
            // btnBrowseClient
            // 
            this.btnBrowseClient.Location = new System.Drawing.Point(7, 31);
            this.btnBrowseClient.Name = "btnBrowseClient";
            this.btnBrowseClient.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseClient.TabIndex = 1;
            this.btnBrowseClient.Text = "Browse";
            this.btnBrowseClient.UseVisualStyleBackColor = true;
            this.btnBrowseClient.Click += new System.EventHandler(this.btnBrowseClient_Click);
            // 
            // txtGamePath
            // 
            this.txtGamePath.Enabled = false;
            this.txtGamePath.Location = new System.Drawing.Point(97, 33);
            this.txtGamePath.Name = "txtGamePath";
            this.txtGamePath.Size = new System.Drawing.Size(242, 20);
            this.txtGamePath.TabIndex = 2;
            // 
            // frmOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(373, 180);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmOptions";
            this.Text = "Options";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmOptions_FormClosing);
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox txtGamePath;
        private System.Windows.Forms.Button btnBrowseClient;
        private System.Windows.Forms.CheckBox chkSuppressTilesetWarning;
    }
}