namespace Intersect.Editor.Forms.DockingElements
{
    partial class FrmMapProperties
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMapProperties));
            this.gridMapProperties = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // gridMapProperties
            // 
            this.gridMapProperties.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.gridMapProperties.CategoryForeColor = System.Drawing.Color.Gainsboro;
            this.gridMapProperties.CategorySplitterColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.gridMapProperties.CommandsBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.gridMapProperties.CommandsForeColor = System.Drawing.Color.Gainsboro;
            this.gridMapProperties.CommandsVisibleIfAvailable = false;
            this.gridMapProperties.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(133)))), ((int)(((byte)(133)))), ((int)(((byte)(133)))));
            this.gridMapProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridMapProperties.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridMapProperties.HelpBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.gridMapProperties.HelpBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.gridMapProperties.HelpForeColor = System.Drawing.Color.Gainsboro;
            this.gridMapProperties.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(51)))), ((int)(((byte)(53)))));
            this.gridMapProperties.Location = new System.Drawing.Point(0, 0);
            this.gridMapProperties.Name = "gridMapProperties";
            this.gridMapProperties.Size = new System.Drawing.Size(154, 140);
            this.gridMapProperties.TabIndex = 0;
            this.gridMapProperties.ToolbarVisible = false;
            this.gridMapProperties.ViewBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.gridMapProperties.ViewBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.gridMapProperties.ViewForeColor = System.Drawing.Color.Gainsboro;
            // 
            // frmMapProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(154, 140);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this.gridMapProperties);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FrmMapProperties";
            this.Text = "Map Properties";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid gridMapProperties;
    }
}