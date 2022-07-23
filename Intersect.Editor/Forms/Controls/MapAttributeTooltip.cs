using System;
using System.Drawing;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.GameObjects.Maps;

namespace Intersect.Editor.Forms.Controls
{
    public partial class MapAttributeTooltip : FlowLayoutPanel
    {
        private readonly object _mapAttributeLock = new object();
        private MapAttribute _mapAttribute;

        public MapAttributeTooltip()
        {
            InitializeComponent();

            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BorderStyle = BorderStyle.FixedSingle;
            FlowDirection = FlowDirection.TopDown;
        }

        public MapAttribute MapAttribute
        {
            get => _mapAttribute;
            set
            {
                lock (_mapAttributeLock)
                {
                    if (_mapAttribute == value)
                    {
                        return;
                    }

                    var oldType = _mapAttribute?.GetType();
                    _mapAttribute = value;
                    OnAttributeChanged(oldType);
                }
            }
        }

        private static Label CreateLabel(AnchorStyles anchor = AnchorStyles.Left, FontStyle fontStyle = FontStyle.Regular)
        {
            return new Label
            {
                Anchor = anchor,
                AutoSize = true,
                Font = new Font(SystemFonts.DefaultFont, fontStyle),
                ForeColor = System.Drawing.Color.White,
            };
        }

        protected virtual void OnAttributeChanged(Type oldType)
        {
            Hide();

            if (_mapAttribute == null)
            {
                return;
            }

            var localizedProperties = Strings.Localizer.Localize(typeof(Strings), _mapAttribute);
            if (localizedProperties.Count < 1)
            {
                return;
            }

            if (localizedProperties.Count < pnlContents.RowCount)
            {
                for (var rowIndex = pnlContents.RowCount - 1; rowIndex >= localizedProperties.Count; rowIndex--)
                {
                    for (var columnIndex = 1; columnIndex >= 0; columnIndex--)
                    {
                        var control = pnlContents.GetControlFromPosition(columnIndex, rowIndex);
                        pnlContents.Controls.Remove(control);
                    }
                }
            }

            for (var rowIndex = 0; rowIndex < localizedProperties.Count; rowIndex++)
            {
                if (rowIndex >= pnlContents.RowCount)
                {
                    pnlContents.Controls.Add(CreateLabel(AnchorStyles.Right, FontStyle.Bold), 0, rowIndex);
                    pnlContents.Controls.Add(CreateLabel(AnchorStyles.Left, rowIndex == 0 ? FontStyle.Bold : FontStyle.Regular), 1, rowIndex);
                }

                var labelControl = pnlContents.GetControlFromPosition(0, rowIndex) as Label ?? throw new InvalidOperationException();
                var displayValueControl = pnlContents.GetControlFromPosition(1, rowIndex) as Label ?? throw new InvalidOperationException();

                var localizedProperty = localizedProperties[rowIndex];
                labelControl.Text = localizedProperty.Key;
                displayValueControl.Text = localizedProperty.Value;
            }

            pnlContents.RowCount = localizedProperties.Count;

            Invalidate();
        }
    }
}
