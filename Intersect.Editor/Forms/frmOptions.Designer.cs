using DarkUI.Controls;

namespace Intersect.Editor.Forms
{
    partial class FrmOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmOptions));
            this.txtGamePath = new DarkUI.Controls.DarkTextBox();
            this.btnBrowseClient = new DarkUI.Controls.DarkButton();
            this.chkSuppressTilesetWarning = new DarkUI.Controls.DarkCheckBox();
            this.btnTileHeader = new DarkUI.Controls.DarkButton();
            this.pnlGeneral = new System.Windows.Forms.Panel();
            this.grpClientPath = new DarkUI.Controls.DarkGroupBox();
            this.pnlGeneral.SuspendLayout();
            this.grpClientPath.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtGamePath
            // 
            this.txtGamePath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtGamePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGamePath.Enabled = false;
            this.txtGamePath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtGamePath.Location = new System.Drawing.Point(96, 18);
            this.txtGamePath.Name = "txtGamePath";
            this.txtGamePath.Size = new System.Drawing.Size(230, 20);
            this.txtGamePath.TabIndex = 2;
            // 
            // btnBrowseClient
            // 
            this.btnBrowseClient.Location = new System.Drawing.Point(6, 16);
            this.btnBrowseClient.Name = "btnBrowseClient";
            this.btnBrowseClient.Padding = new System.Windows.Forms.Padding(5);
            this.btnBrowseClient.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseClient.TabIndex = 1;
            this.btnBrowseClient.Text = "Browse";
            this.btnBrowseClient.Click += new System.EventHandler(this.btnBrowseClient_Click);
            // 
            // chkSuppressTilesetWarning
            // 
            this.chkSuppressTilesetWarning.Location = new System.Drawing.Point(8, 5);
            this.chkSuppressTilesetWarning.Name = "chkSuppressTilesetWarning";
            this.chkSuppressTilesetWarning.Size = new System.Drawing.Size(332, 17);
            this.chkSuppressTilesetWarning.TabIndex = 0;
            this.chkSuppressTilesetWarning.Text = "Suppress large tileset size warning.";
            this.chkSuppressTilesetWarning.CheckedChanged += new System.EventHandler(this.chkSuppressTilesetWarning_CheckedChanged);
            // 
            // btnTileHeader
            // 
            this.btnTileHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btnTileHeader.Location = new System.Drawing.Point(4, 3);
            this.btnTileHeader.Name = "btnTileHeader";
            this.btnTileHeader.Padding = new System.Windows.Forms.Padding(5);
            this.btnTileHeader.Size = new System.Drawing.Size(57, 23);
            this.btnTileHeader.TabIndex = 19;
            this.btnTileHeader.Text = "General";
            // 
            // pnlGeneral
            // 
            this.pnlGeneral.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlGeneral.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlGeneral.Controls.Add(this.chkSuppressTilesetWarning);
            this.pnlGeneral.Controls.Add(this.grpClientPath);
            this.pnlGeneral.Location = new System.Drawing.Point(4, 26);
            this.pnlGeneral.Name = "pnlGeneral";
            this.pnlGeneral.Size = new System.Drawing.Size(357, 149);
            this.pnlGeneral.TabIndex = 20;
            // 
            // grpClientPath
            // 
            this.grpClientPath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpClientPath.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpClientPath.Controls.Add(this.btnBrowseClient);
            this.grpClientPath.Controls.Add(this.txtGamePath);
            this.grpClientPath.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpClientPath.Location = new System.Drawing.Point(8, 29);
            this.grpClientPath.Name = "grpClientPath";
            this.grpClientPath.Size = new System.Drawing.Size(332, 45);
            this.grpClientPath.TabIndex = 3;
            this.grpClientPath.TabStop = false;
            this.grpClientPath.Text = "Client Path";
            // 
            // FrmOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(373, 180);
            this.Controls.Add(this.pnlGeneral);
            this.Controls.Add(this.btnTileHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FrmOptions";
            this.Text = "Options";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmOptions_FormClosing);
            this.pnlGeneral.ResumeLayout(false);
            this.grpClientPath.ResumeLayout(false);
            this.grpClientPath.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private DarkTextBox txtGamePath;
        private DarkButton btnBrowseClient;
        private DarkCheckBox chkSuppressTilesetWarning;
        private DarkUI.Controls.DarkButton btnTileHeader;
        private System.Windows.Forms.Panel pnlGeneral;
        private DarkGroupBox grpClientPath;
    }
}