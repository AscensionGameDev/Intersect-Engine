namespace Intersect_Editor.Forms
{
    partial class frmMapProperties
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
            this.gridMapProperties = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // gridMapProperties
            // 
            this.gridMapProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridMapProperties.Location = new System.Drawing.Point(0, -1);
            this.gridMapProperties.Name = "gridMapProperties";
            this.gridMapProperties.Size = new System.Drawing.Size(153, 141);
            this.gridMapProperties.TabIndex = 0;
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
            this.Name = "frmMapProperties";
            this.Text = "Map Properties";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid gridMapProperties;
    }
}