using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandSetDungeonObjective
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
            this.grpObjective = new DarkUI.Controls.DarkGroupBox();
            this.txtObjective = new DarkUI.Controls.DarkTextBox();
            this.lblObjective = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpObjective.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpObjective
            // 
            this.grpObjective.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpObjective.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpObjective.Controls.Add(this.txtObjective);
            this.grpObjective.Controls.Add(this.lblObjective);
            this.grpObjective.Controls.Add(this.btnCancel);
            this.grpObjective.Controls.Add(this.btnSave);
            this.grpObjective.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpObjective.Location = new System.Drawing.Point(3, 3);
            this.grpObjective.Name = "grpObjective";
            this.grpObjective.Size = new System.Drawing.Size(176, 126);
            this.grpObjective.TabIndex = 17;
            this.grpObjective.TabStop = false;
            this.grpObjective.Text = "Set Dungeon Objective";
            // 
            // txtObjective
            // 
            this.txtObjective.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtObjective.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtObjective.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtObjective.Location = new System.Drawing.Point(7, 38);
            this.txtObjective.Multiline = true;
            this.txtObjective.Name = "txtObjective";
            this.txtObjective.Size = new System.Drawing.Size(157, 52);
            this.txtObjective.TabIndex = 22;
            // 
            // lblObjective
            // 
            this.lblObjective.AutoSize = true;
            this.lblObjective.Location = new System.Drawing.Point(4, 22);
            this.lblObjective.Name = "lblObjective";
            this.lblObjective.Size = new System.Drawing.Size(35, 13);
            this.lblObjective.TabIndex = 21;
            this.lblObjective.Text = "Objective:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(89, 97);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(7, 97);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // EventCommand_SetDungeonObjective
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpObjective);
            this.Name = "EventCommandSetDungeonObjective";
            this.Size = new System.Drawing.Size(182, 132);
            this.grpObjective.ResumeLayout(false);
            this.grpObjective.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpObjective;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private System.Windows.Forms.Label lblObjective;
        private DarkTextBox txtObjective;
    }
}
