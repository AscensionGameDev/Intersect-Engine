using System.ComponentModel;
using DarkUI.Controls;

namespace Intersect.Editor.Forms
{
    partial class FrmUploadToServer
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
            this.lblServerUrl = new DarkUI.Controls.DarkLabel();
            this.txtServerUrl = new DarkUI.Controls.DarkTextBox();
            this.lblUploadType = new DarkUI.Controls.DarkLabel();
            this.rbClientAssets = new DarkUI.Controls.DarkRadioButton();
            this.rbEditorAssets = new DarkUI.Controls.DarkRadioButton();
            this.lblDirectory = new DarkUI.Controls.DarkLabel();
            this.txtDirectory = new DarkUI.Controls.DarkTextBox();
            this.btnBrowse = new DarkUI.Controls.DarkButton();
            this.btnUpload = new DarkUI.Controls.DarkButton();
            this.btnClose = new DarkUI.Controls.DarkButton();
            this.progressBar = new DarkUI.Controls.DarkProgressBar();
            this.lblStatus = new DarkUI.Controls.DarkLabel();
            this.grpUploadType = new DarkUI.Controls.DarkGroupBox();
            this.grpUploadType.SuspendLayout();
            this.SuspendLayout();
            //
            // lblServerUrl
            //
            this.lblServerUrl.AutoSize = true;
            this.lblServerUrl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lblServerUrl.Location = new System.Drawing.Point(12, 15);
            this.lblServerUrl.Name = "lblServerUrl";
            this.lblServerUrl.Size = new System.Drawing.Size(63, 13);
            this.lblServerUrl.TabIndex = 0;
            this.lblServerUrl.Text = "Server URL:";
            //
            // txtServerUrl
            //
            this.txtServerUrl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtServerUrl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtServerUrl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtServerUrl.Location = new System.Drawing.Point(15, 31);
            this.txtServerUrl.Name = "txtServerUrl";
            this.txtServerUrl.Size = new System.Drawing.Size(457, 20);
            this.txtServerUrl.TabIndex = 1;
            //
            // lblUploadType
            //
            this.lblUploadType.AutoSize = true;
            this.lblUploadType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lblUploadType.Location = new System.Drawing.Point(12, 64);
            this.lblUploadType.Name = "lblUploadType";
            this.lblUploadType.Size = new System.Drawing.Size(71, 13);
            this.lblUploadType.TabIndex = 2;
            this.lblUploadType.Text = "Upload Type:";
            //
            // grpUploadType
            //
            this.grpUploadType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.grpUploadType.Controls.Add(this.rbClientAssets);
            this.grpUploadType.Controls.Add(this.rbEditorAssets);
            this.grpUploadType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.grpUploadType.Location = new System.Drawing.Point(15, 80);
            this.grpUploadType.Name = "grpUploadType";
            this.grpUploadType.Size = new System.Drawing.Size(457, 60);
            this.grpUploadType.TabIndex = 3;
            this.grpUploadType.TabStop = false;
            //
            // rbClientAssets
            //
            this.rbClientAssets.AutoSize = true;
            this.rbClientAssets.Checked = true;
            this.rbClientAssets.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.rbClientAssets.Location = new System.Drawing.Point(15, 20);
            this.rbClientAssets.Name = "rbClientAssets";
            this.rbClientAssets.Size = new System.Drawing.Size(86, 17);
            this.rbClientAssets.TabIndex = 0;
            this.rbClientAssets.TabStop = true;
            this.rbClientAssets.Text = "Client Assets";
            this.rbClientAssets.UseVisualStyleBackColor = true;
            //
            // rbEditorAssets
            //
            this.rbEditorAssets.AutoSize = true;
            this.rbEditorAssets.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.rbEditorAssets.Location = new System.Drawing.Point(150, 20);
            this.rbEditorAssets.Name = "rbEditorAssets";
            this.rbEditorAssets.Size = new System.Drawing.Size(86, 17);
            this.rbEditorAssets.TabIndex = 1;
            this.rbEditorAssets.Text = "Editor Assets";
            this.rbEditorAssets.UseVisualStyleBackColor = true;
            //
            // lblDirectory
            //
            this.lblDirectory.AutoSize = true;
            this.lblDirectory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lblDirectory.Location = new System.Drawing.Point(12, 153);
            this.lblDirectory.Name = "lblDirectory";
            this.lblDirectory.Size = new System.Drawing.Size(92, 13);
            this.lblDirectory.TabIndex = 4;
            this.lblDirectory.Text = "Source Directory:";
            //
            // txtDirectory
            //
            this.txtDirectory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtDirectory.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDirectory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtDirectory.Location = new System.Drawing.Point(15, 169);
            this.txtDirectory.Name = "txtDirectory";
            this.txtDirectory.ReadOnly = true;
            this.txtDirectory.Size = new System.Drawing.Size(357, 20);
            this.txtDirectory.TabIndex = 5;
            //
            // btnBrowse
            //
            this.btnBrowse.Location = new System.Drawing.Point(378, 167);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Padding = new System.Windows.Forms.Padding(5);
            this.btnBrowse.Size = new System.Drawing.Size(94, 23);
            this.btnBrowse.TabIndex = 6;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            //
            // progressBar
            //
            this.progressBar.Location = new System.Drawing.Point(15, 205);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(457, 23);
            this.progressBar.TabIndex = 7;
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lblStatus.Location = new System.Drawing.Point(12, 237);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 8;
            //
            // btnUpload
            //
            this.btnUpload.Enabled = false;
            this.btnUpload.Location = new System.Drawing.Point(297, 260);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Padding = new System.Windows.Forms.Padding(5);
            this.btnUpload.Size = new System.Drawing.Size(85, 28);
            this.btnUpload.TabIndex = 9;
            this.btnUpload.Text = "Upload";
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            //
            // btnClose
            //
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(388, 260);
            this.btnClose.Name = "btnClose";
            this.btnClose.Padding = new System.Windows.Forms.Padding(5);
            this.btnClose.Size = new System.Drawing.Size(84, 28);
            this.btnClose.TabIndex = 10;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // FrmUploadToServer
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 300);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtDirectory);
            this.Controls.Add(this.lblDirectory);
            this.Controls.Add(this.grpUploadType);
            this.Controls.Add(this.lblUploadType);
            this.Controls.Add(this.txtServerUrl);
            this.Controls.Add(this.lblServerUrl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmUploadToServer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Upload to Server";
            this.grpUploadType.ResumeLayout(false);
            this.grpUploadType.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private DarkLabel lblServerUrl;
        private DarkTextBox txtServerUrl;
        private DarkLabel lblUploadType;
        private DarkRadioButton rbClientAssets;
        private DarkRadioButton rbEditorAssets;
        private DarkLabel lblDirectory;
        private DarkTextBox txtDirectory;
        private DarkButton btnBrowse;
        private DarkButton btnUpload;
        private DarkButton btnClose;
        private DarkProgressBar progressBar;
        private DarkLabel lblStatus;
        private DarkGroupBox grpUploadType;
    }
}
