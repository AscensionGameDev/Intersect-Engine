using DarkUI.Controls;
using Intersect.Editor.Forms.Controls;

namespace Intersect.Editor.Forms
{
    partial class FrmWarpSelection
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
            components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmWarpSelection));
            grpEverything = new DarkGroupBox();
            btnRefreshPreview = new Button();
            grpMapPreview = new DarkGroupBox();
            pnlMapContainer = new Panel();
            pnlMap = new Panel();
            grpMapList = new DarkGroupBox();
            chkAlphabetical = new DarkCheckBox();
            mapTreeList1 = new MapTreeList();
            btnCancel = new DarkButton();
            btnOk = new DarkButton();
            tmrMapCheck = new System.Windows.Forms.Timer(components);
            grpEverything.SuspendLayout();
            grpMapPreview.SuspendLayout();
            pnlMapContainer.SuspendLayout();
            grpMapList.SuspendLayout();
            SuspendLayout();
            // 
            // grpEverything
            // 
            grpEverything.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpEverything.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpEverything.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpEverything.Controls.Add(btnRefreshPreview);
            grpEverything.Controls.Add(grpMapPreview);
            grpEverything.Controls.Add(grpMapList);
            grpEverything.Controls.Add(btnCancel);
            grpEverything.Controls.Add(btnOk);
            grpEverything.ForeColor = System.Drawing.Color.Gainsboro;
            grpEverything.Location = new System.Drawing.Point(2, 2);
            grpEverything.Margin = new Padding(4, 3, 4, 3);
            grpEverything.Name = "grpEverything";
            grpEverything.Padding = new Padding(4, 3, 4, 3);
            grpEverything.Size = new Size(913, 697);
            grpEverything.TabIndex = 1;
            grpEverything.TabStop = false;
            // 
            // btnRefreshPreview
            // 
            btnRefreshPreview.BackgroundImage = (Image)resources.GetObject("btnRefreshPreview.BackgroundImage");
            btnRefreshPreview.BackgroundImageLayout = ImageLayout.Center;
            btnRefreshPreview.Enabled = false;
            btnRefreshPreview.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(70, 70, 70);
            btnRefreshPreview.FlatStyle = FlatStyle.Flat;
            btnRefreshPreview.ForeColor = System.Drawing.Color.Transparent;
            btnRefreshPreview.Location = new System.Drawing.Point(878, 2);
            btnRefreshPreview.Margin = new Padding(4, 3, 4, 3);
            btnRefreshPreview.Name = "btnRefreshPreview";
            btnRefreshPreview.Size = new Size(28, 28);
            btnRefreshPreview.TabIndex = 8;
            btnRefreshPreview.UseVisualStyleBackColor = true;
            btnRefreshPreview.Click += btnRefreshPreview_Click;
            // 
            // grpMapPreview
            // 
            grpMapPreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpMapPreview.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpMapPreview.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpMapPreview.Controls.Add(pnlMapContainer);
            grpMapPreview.ForeColor = System.Drawing.Color.Gainsboro;
            grpMapPreview.Location = new System.Drawing.Point(300, 25);
            grpMapPreview.Margin = new Padding(4, 3, 4, 3);
            grpMapPreview.Name = "grpMapPreview";
            grpMapPreview.Padding = new Padding(4, 3, 4, 3);
            grpMapPreview.Size = new Size(607, 613);
            grpMapPreview.TabIndex = 7;
            grpMapPreview.TabStop = false;
            grpMapPreview.Text = "Map Preview";
            // 
            // pnlMapContainer
            // 
            pnlMapContainer.AutoScroll = true;
            pnlMapContainer.Controls.Add(pnlMap);
            pnlMapContainer.Dock = DockStyle.Fill;
            pnlMapContainer.Location = new System.Drawing.Point(4, 19);
            pnlMapContainer.Margin = new Padding(4, 3, 4, 3);
            pnlMapContainer.Name = "pnlMapContainer";
            pnlMapContainer.Size = new Size(599, 591);
            pnlMapContainer.TabIndex = 0;
            // 
            // pnlMap
            // 
            pnlMap.Location = new System.Drawing.Point(0, 0);
            pnlMap.Margin = new Padding(4, 3, 4, 3);
            pnlMap.Name = "pnlMap";
            pnlMap.Size = new Size(233, 115);
            pnlMap.TabIndex = 0;
            pnlMap.DoubleClick += pnlMap_DoubleClick;
            pnlMap.MouseDown += pnlMap_MouseDown;
            // 
            // grpMapList
            // 
            grpMapList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            grpMapList.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpMapList.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpMapList.Controls.Add(chkAlphabetical);
            grpMapList.Controls.Add(mapTreeList1);
            grpMapList.ForeColor = System.Drawing.Color.Gainsboro;
            grpMapList.Location = new System.Drawing.Point(7, 25);
            grpMapList.Margin = new Padding(4, 3, 4, 3);
            grpMapList.Name = "grpMapList";
            grpMapList.Padding = new Padding(4, 3, 4, 3);
            grpMapList.Size = new Size(268, 613);
            grpMapList.TabIndex = 6;
            grpMapList.TabStop = false;
            grpMapList.Text = "Map List";
            // 
            // chkAlphabetical
            // 
            chkAlphabetical.AutoSize = true;
            chkAlphabetical.Location = new System.Drawing.Point(159, 15);
            chkAlphabetical.Margin = new Padding(4, 3, 4, 3);
            chkAlphabetical.Name = "chkAlphabetical";
            chkAlphabetical.Size = new Size(101, 19);
            chkAlphabetical.TabIndex = 1;
            chkAlphabetical.Text = "Chronological";
            chkAlphabetical.CheckedChanged += chkChronological_CheckedChanged;
            // 
            // mapTreeList1
            // 
            mapTreeList1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            mapTreeList1.Location = new System.Drawing.Point(8, 42);
            mapTreeList1.Margin = new Padding(5, 3, 5, 3);
            mapTreeList1.Name = "mapTreeList1";
            mapTreeList1.Size = new Size(253, 564);
            mapTreeList1.TabIndex = 0;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.Location = new System.Drawing.Point(811, 663);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Padding = new Padding(6, 6, 6, 6);
            btnCancel.Size = new Size(96, 27);
            btnCancel.TabIndex = 5;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnOk
            // 
            btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOk.Location = new System.Drawing.Point(710, 663);
            btnOk.Margin = new Padding(4, 3, 4, 3);
            btnOk.Name = "btnOk";
            btnOk.Padding = new Padding(6, 6, 6, 6);
            btnOk.Size = new Size(96, 27);
            btnOk.TabIndex = 4;
            btnOk.Text = "Ok";
            btnOk.Click += btnOk_Click;
            // 
            // tmrMapCheck
            // 
            tmrMapCheck.Tick += tmrMapCheck_Tick;
            // 
            // FrmWarpSelection
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            ClientSize = new Size(918, 703);
            Controls.Add(grpEverything);
            DoubleBuffered = true;
            Margin = new Padding(4, 3, 4, 3);
            Name = "FrmWarpSelection";
            Text = "Warp Tile Selection";
            Load += frmWarpSelection_Load;
            grpEverything.ResumeLayout(false);
            grpMapPreview.ResumeLayout(false);
            pnlMapContainer.ResumeLayout(false);
            grpMapList.ResumeLayout(false);
            grpMapList.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DarkGroupBox grpEverything;
        private DarkGroupBox grpMapPreview;
        private Panel pnlMapContainer;
        private Panel pnlMap;
        private DarkGroupBox grpMapList;
        private DarkCheckBox chkAlphabetical;
        private Controls.MapTreeList mapTreeList1;
        private DarkButton btnCancel;
        private DarkButton btnOk;
        private System.Windows.Forms.Timer tmrMapCheck;
        private Button btnRefreshPreview;
    }
}