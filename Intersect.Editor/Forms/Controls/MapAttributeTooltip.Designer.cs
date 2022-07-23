using System.Drawing;
using System.Windows.Forms;

using Intersect.Editor.Localization;

namespace Intersect.Editor.Forms.Controls
{
    partial class MapAttributeTooltip
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
            components = new System.ComponentModel.Container();

            var defaultFont = SystemFonts.DefaultFont;
            var titleFont = new Font(defaultFont.FontFamily, defaultFont.Size + 2, FontStyle.Bold, defaultFont.Unit, defaultFont.GdiCharSet, defaultFont.GdiVerticalFont);

            lblAttributeTypeLabel = new Label
            {
                Anchor = AnchorStyles.Right,
                AutoSize = true,
                Font = titleFont,
                ForeColor = System.Drawing.Color.White,
                Text = Strings.Attributes.AttributeType,
            };

            lblAttributeType = new Label
            {
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Font = titleFont,
                ForeColor = System.Drawing.Color.White,
                Text = Strings.General.None,
            };

            pnlContents = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                ColumnCount = 2,
            };

            Controls.Add(pnlContents);
        }

        #endregion

        private Label lblAttributeTypeLabel;
        private Label lblAttributeType;
        private TableLayoutPanel pnlContents;
    }
}
