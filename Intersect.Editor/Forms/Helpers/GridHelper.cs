using System;
using System.Drawing;
using System.Linq;

namespace Intersect.Editor.Forms.Helpers
{
    using Color = System.Drawing.Color;

    public struct GridTile
    {
        public int X { get; private set; }

        public int Y { get; private set; }

        public Color? Color { get; private set; }

        public string Label { get; private set; }

        public Color? LabelColor { get; private set; }

        public GridTile(int x, int y, Color? color = default, string label = null, Color? labelColor = default)
        {
            X = x;
            Y = y;
            Color = color;
            Label = label;
            LabelColor = labelColor;
        }

        public GridTile At(int x, int y)
        {
            if (X == x && Y == y)
            {
                return this;
            }

            return new GridTile(x, y, Color, Label);
        }

        public GridTile WithColor(Color color)
        {
            if (Color == color)
            {
                return this;
            }

            return new GridTile(X, Y, color, Label);
        }

        public GridTile WithLabel(string label)
        {
            if (String.Equals(Label, label, StringComparison.Ordinal))
            {
                return this;
            }

            return new GridTile(X, Y, Color, label);
        }

        public GridTile WithLabelColor(Color labelColor)
        {
            if (LabelColor == labelColor)
            {
                return this;
            }

            return new GridTile(X, Y, Color, Label, labelColor);
        }

        public static int CalculatePrecedenceKey(GridTile gridTile) =>
            (String.IsNullOrEmpty(gridTile.Label) ? 0 : 2) + (gridTile.Color == null ? 0 : 1);
    }

    public static class GridHelper
    {
        public static Color ColorBackground { get; set; } = Color.White;

        public static Color ColorForeground { get; set; } = Color.Black;

        public static Color ColorSelection { get; set; } = Color.Red;

        public static Font DefaultFont { get; set; } = new Font(new FontFamily("Arial"), 14);

        public static Bitmap DrawGrid(
            int bitmapWidth,
            int bitmapHeight,
            int gridColumns,
            int gridRows,
            params GridTile[] gridTiles
        ) =>
            DrawGrid(bitmapWidth, bitmapHeight, gridColumns, gridRows, DefaultFont, gridTiles);

        public static Bitmap DrawGrid(
            int bitmapWidth,
            int bitmapHeight,
            int gridColumns,
            int gridRows,
            Font font,
            params GridTile[] gridTiles
        ) =>
            DrawGrid(new Bitmap(bitmapWidth, bitmapHeight), gridColumns, gridRows, font, gridTiles);

        public static Bitmap DrawGrid(Bitmap bitmap, int gridColumns, int gridRows, params GridTile[] gridTiles) =>
            DrawGrid(bitmap, gridColumns, gridRows, DefaultFont, gridTiles);

        public static Bitmap DrawGrid(
            Bitmap bitmap,
            int gridColumns,
            int gridRows,
            Font font,
            params GridTile[] gridTiles
        )
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }

            var gridWidth = bitmap.Width;
            var cellWidth = gridWidth / gridColumns;

            var gridHeight = bitmap.Height;
            var cellHeight = gridHeight / gridRows;

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(ColorBackground);

                foreach (var gridTile in gridTiles.OrderBy(GridTile.CalculatePrecedenceKey))
                {
                    var x = gridTile.X * cellWidth;
                    var y = gridTile.Y * cellHeight;

                    if (gridTile.Color != null && gridTile.Color != Color.Transparent && gridTile.Color.Value.A != 0)
                    {
                        graphics.FillRectangle(
                            new SolidBrush(ColorSelection), new Rectangle(x, y, cellWidth, cellHeight)
                        );
                    }

                    if (!string.IsNullOrWhiteSpace(gridTile.Label))
                    {
                        var measurement = graphics.MeasureString(gridTile.Label, font);
                        graphics.DrawString(
                            gridTile.Label, font, new SolidBrush(gridTile.LabelColor ?? ColorForeground),
                            x + (cellWidth - measurement.Width) / 2, y + (cellHeight - measurement.Height) / 2
                        );
                    }
                }

                for (var lineIndex = 1; lineIndex < Math.Max(gridColumns, gridRows); ++lineIndex)
                {
                    if (lineIndex < gridColumns)
                    {
                        graphics.DrawLine(Pens.Black, cellWidth * lineIndex, 0, cellWidth * lineIndex, gridHeight);
                    }

                    if (lineIndex < gridRows)
                    {
                        graphics.DrawLine(Pens.Black, 0, cellHeight * lineIndex, gridWidth, cellHeight * lineIndex);
                    }
                }
            }

            return bitmap;
        }
    }
}
