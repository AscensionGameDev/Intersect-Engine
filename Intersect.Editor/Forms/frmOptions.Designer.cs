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
            this.txtGamePath = new DarkUI.Controls.DarkTextBox();
            this.btnBrowseClient = new DarkUI.Controls.DarkButton();
            this.chkSuppressTilesetWarning = new DarkUI.Controls.DarkCheckBox();
            this.btnGeneralOptions = new DarkUI.Controls.DarkButton();
            this.pnlGeneral = new System.Windows.Forms.Panel();
            this.grpClientPath = new DarkUI.Controls.DarkGroupBox();
            this.btnUpdateOptions = new DarkUI.Controls.DarkButton();
            this.pnlUpdate = new System.Windows.Forms.Panel();
            this.grpAssetPackingOptions = new DarkUI.Controls.DarkGroupBox();
            this.cmbTextureSize = new DarkUI.Controls.DarkComboBox();
            this.lblTextureSize = new System.Windows.Forms.Label();
            this.nudMusicBatch = new DarkUI.Controls.DarkNumericUpDown();
            this.nudSoundBatch = new DarkUI.Controls.DarkNumericUpDown();
            this.lblMusicBatch = new System.Windows.Forms.Label();
            this.lblSoundBatch = new System.Windows.Forms.Label();
            this.chkPackageAssets = new DarkUI.Controls.DarkCheckBox();
            this.pnlGeneral.SuspendLayout();
            this.grpClientPath.SuspendLayout();
            this.pnlUpdate.SuspendLayout();
            this.grpAssetPackingOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMusicBatch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSoundBatch)).BeginInit();
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
            // 
            // btnGeneralOptions
            // 
            this.btnGeneralOptions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btnGeneralOptions.Location = new System.Drawing.Point(4, 3);
            this.btnGeneralOptions.Name = "btnGeneralOptions";
            this.btnGeneralOptions.Padding = new System.Windows.Forms.Padding(5);
            this.btnGeneralOptions.Size = new System.Drawing.Size(57, 23);
            this.btnGeneralOptions.TabIndex = 19;
            this.btnGeneralOptions.Text = "General";
            this.btnGeneralOptions.Click += new System.EventHandler(this.btnGeneralOptions_Click);
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
            // btnUpdateOptions
            // 
            this.btnUpdateOptions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btnUpdateOptions.Location = new System.Drawing.Point(67, 3);
            this.btnUpdateOptions.Name = "btnUpdateOptions";
            this.btnUpdateOptions.Padding = new System.Windows.Forms.Padding(5);
            this.btnUpdateOptions.Size = new System.Drawing.Size(57, 23);
            this.btnUpdateOptions.TabIndex = 21;
            this.btnUpdateOptions.Text = "Update";
            this.btnUpdateOptions.Click += new System.EventHandler(this.btnUpdateOptions_Click);
            // 
            // pnlUpdate
            // 
            this.pnlUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlUpdate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlUpdate.Controls.Add(this.grpAssetPackingOptions);
            this.pnlUpdate.Controls.Add(this.chkPackageAssets);
            this.pnlUpdate.Location = new System.Drawing.Point(4, 26);
            this.pnlUpdate.Name = "pnlUpdate";
            this.pnlUpdate.Size = new System.Drawing.Size(357, 149);
            this.pnlUpdate.TabIndex = 22;
            // 
            // grpAssetPackingOptions
            // 
            this.grpAssetPackingOptions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpAssetPackingOptions.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpAssetPackingOptions.Controls.Add(this.cmbTextureSize);
            this.grpAssetPackingOptions.Controls.Add(this.lblTextureSize);
            this.grpAssetPackingOptions.Controls.Add(this.nudMusicBatch);
            this.grpAssetPackingOptions.Controls.Add(this.nudSoundBatch);
            this.grpAssetPackingOptions.Controls.Add(this.lblMusicBatch);
            this.grpAssetPackingOptions.Controls.Add(this.lblSoundBatch);
            this.grpAssetPackingOptions.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpAssetPackingOptions.Location = new System.Drawing.Point(14, 29);
            this.grpAssetPackingOptions.Name = "grpAssetPackingOptions";
            this.grpAssetPackingOptions.Size = new System.Drawing.Size(332, 112);
            this.grpAssetPackingOptions.TabIndex = 4;
            this.grpAssetPackingOptions.TabStop = false;
            this.grpAssetPackingOptions.Text = "Asset Packing Options";
            // 
            // cmbTextureSize
            // 
            this.cmbTextureSize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbTextureSize.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbTextureSize.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbTextureSize.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbTextureSize.DrawDropdownHoverOutline = false;
            this.cmbTextureSize.DrawFocusRectangle = false;
            this.cmbTextureSize.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbTextureSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTextureSize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbTextureSize.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbTextureSize.FormattingEnabled = true;
            this.cmbTextureSize.Items.AddRange(new object[] {
            "1",
            "256",
            "512",
            "1024",
            "2048",
            "4096",
            "8192"});
            this.cmbTextureSize.Location = new System.Drawing.Point(104, 42);
            this.cmbTextureSize.Name = "cmbTextureSize";
            this.cmbTextureSize.Size = new System.Drawing.Size(59, 21);
            this.cmbTextureSize.TabIndex = 52;
            this.cmbTextureSize.Text = "1";
            this.cmbTextureSize.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblTextureSize
            // 
            this.lblTextureSize.AutoSize = true;
            this.lblTextureSize.ForeColor = System.Drawing.Color.White;
            this.lblTextureSize.Location = new System.Drawing.Point(6, 48);
            this.lblTextureSize.Name = "lblTextureSize";
            this.lblTextureSize.Size = new System.Drawing.Size(94, 13);
            this.lblTextureSize.TabIndex = 51;
            this.lblTextureSize.Text = "Texture Pack Size";
            // 
            // nudMusicBatch
            // 
            this.nudMusicBatch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudMusicBatch.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudMusicBatch.Location = new System.Drawing.Point(267, 16);
            this.nudMusicBatch.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.nudMusicBatch.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudMusicBatch.Name = "nudMusicBatch";
            this.nudMusicBatch.Size = new System.Drawing.Size(59, 20);
            this.nudMusicBatch.TabIndex = 50;
            this.nudMusicBatch.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // nudSoundBatch
            // 
            this.nudSoundBatch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudSoundBatch.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudSoundBatch.Location = new System.Drawing.Point(104, 16);
            this.nudSoundBatch.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.nudSoundBatch.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSoundBatch.Name = "nudSoundBatch";
            this.nudSoundBatch.Size = new System.Drawing.Size(59, 20);
            this.nudSoundBatch.TabIndex = 49;
            this.nudSoundBatch.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // lblMusicBatch
            // 
            this.lblMusicBatch.AutoSize = true;
            this.lblMusicBatch.ForeColor = System.Drawing.Color.White;
            this.lblMusicBatch.Location = new System.Drawing.Point(169, 20);
            this.lblMusicBatch.Name = "lblMusicBatch";
            this.lblMusicBatch.Size = new System.Drawing.Size(89, 13);
            this.lblMusicBatch.TabIndex = 8;
            this.lblMusicBatch.Text = "Music Batch Size";
            // 
            // lblSoundBatch
            // 
            this.lblSoundBatch.AutoSize = true;
            this.lblSoundBatch.ForeColor = System.Drawing.Color.White;
            this.lblSoundBatch.Location = new System.Drawing.Point(6, 20);
            this.lblSoundBatch.Name = "lblSoundBatch";
            this.lblSoundBatch.Size = new System.Drawing.Size(92, 13);
            this.lblSoundBatch.TabIndex = 7;
            this.lblSoundBatch.Text = "Sound Batch Size";
            // 
            // chkPackageAssets
            // 
            this.chkPackageAssets.Location = new System.Drawing.Point(14, 5);
            this.chkPackageAssets.Name = "chkPackageAssets";
            this.chkPackageAssets.Size = new System.Drawing.Size(332, 17);
            this.chkPackageAssets.TabIndex = 1;
            this.chkPackageAssets.Text = "Package assets when generating updates";
            // 
            // FrmOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(373, 180);
            this.Controls.Add(this.pnlUpdate);
            this.Controls.Add(this.btnUpdateOptions);
            this.Controls.Add(this.pnlGeneral);
            this.Controls.Add(this.btnGeneralOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmOptions";
            this.Text = "Options";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmOptions_FormClosing);
            this.pnlGeneral.ResumeLayout(false);
            this.grpClientPath.ResumeLayout(false);
            this.grpClientPath.PerformLayout();
            this.pnlUpdate.ResumeLayout(false);
            this.grpAssetPackingOptions.ResumeLayout(false);
            this.grpAssetPackingOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMusicBatch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSoundBatch)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DarkTextBox txtGamePath;
        private DarkButton btnBrowseClient;
        private DarkCheckBox chkSuppressTilesetWarning;
        private DarkUI.Controls.DarkButton btnGeneralOptions;
        private System.Windows.Forms.Panel pnlGeneral;
        private DarkGroupBox grpClientPath;
        private DarkButton btnUpdateOptions;
        private System.Windows.Forms.Panel pnlUpdate;
        private DarkCheckBox chkPackageAssets;
        private DarkGroupBox grpAssetPackingOptions;
        private System.Windows.Forms.Label lblMusicBatch;
        private System.Windows.Forms.Label lblSoundBatch;
        private DarkNumericUpDown nudMusicBatch;
        private DarkNumericUpDown nudSoundBatch;
        private System.Windows.Forms.Label lblTextureSize;
        private DarkComboBox cmbTextureSize;
    }
}