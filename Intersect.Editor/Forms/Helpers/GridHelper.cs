using System;
using System.Drawing;
using System.Linq;
using Intersect.Editor.Core;

namespace Intersect.Editor.Forms.Helpers
{
    using Color = System.Drawing.Color;
    using Graphics = System.Drawing.Graphics;

    public partial struct GridCell
    {
        public int X { get; private set; }

        public int Y { get; private set; }

        public Color? Color { get; private set; }

        public string Label { get; private set; }

        public Color? LabelColor { get; private set; }

        public GridCell(int x, int y, Color? color = default, string label = null, Color? labelColor = default)
        {
            X = x;
            Y = y;
            Color = color;
            Label = label;
            LabelColor = labelColor;
        }

        public GridCell At(int x, int y)
        {
            if (X == x && Y == y)
            {
                return this;
            }

            return new GridCell(x, y, Color, Label);
        }

        public GridCell WithColor(Color color)
        {
            if (Color == color)
            {
                return this;
            }

            return new GridCell(X, Y, color, Label);
        }

        public GridCell WithLabel(string label)
        {
            if (String.Equals(Label, label, StringComparison.Ordinal))
            {
                return this;
            }

            return new GridCell(X, Y, Color, label);
        }

        public GridCell WithLabelColor(Color labelColor)
        {
            if (LabelColor == labelColor)
            {
                return this;
            }

            return new GridCell(X, Y, Color, Label, labelColor);
        }

        public static int CalculatePrecedenceKey(GridCell gridCell) =>
            (String.IsNullOrEmpty(gridCell.Label) ? 0 : 2) + (gridCell.Color == null ? 0 : 1);
    }

    public struct Grid
    {
        public int DisplayWidth;
        public int DisplayHeight;
        public int Columns;
        public int Rows;
        public GridCell[] Cells;

        public Grid WithAdditionalCells(params GridCell[] additionalCells) =>
            new Grid
            {
                DisplayWidth = DisplayWidth,
                DisplayHeight = DisplayHeight,
                Columns = Columns,
                Rows = Rows,
                Cells = Cells.Concat(additionalCells).ToArray()
            };
    }

    /// <summary>
    /// Helper class for drawing grids on <see cref="Bitmap"/>s.
    /// </summary>
    public static partial class GridHelper
    {
        /// <summary>
        /// Mutable background color for all grids, default <see cref="Color.White"/>.
        /// </summary>
        public static Color ColorBackground { get; set; } = Color.White;

        /// <summary>
        /// Mutable foreground color for all grids (only affects the grid lines, not text color), default <see cref="Color.Black"/>.
        /// </summary>
        public static Color ColorForeground { get; set; } = Color.Black;

        /// <summary>
        /// Interpolates between two colors by a given ratio.
        /// </summary>
        public static Color ColorInterpolate(Color color1, Color color2, float ratio)
        {
            byte a = (byte)((color2.A - color1.A) * ratio + color1.A);
            byte r = (byte)((color2.R - color1.R) * ratio + color1.R);
            byte g = (byte)((color2.G - color1.G) * ratio + color1.G);
            byte b = (byte)((color2.B - color1.B) * ratio + color1.B);
            return Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        /// Mutable selection tile color for all grids, default <see cref="Color.Red"/>.
        /// </summary>
        public static Color ColorSelection { get; set; } = Color.Red;

        /// <summary>
        /// Mutable fallback font for text rendering on grids, default Arial 14pt.
        /// </summary>
        public static Font DefaultFont { get; set; } = new Font(new FontFamily("Arial"), 14);

        public static (int x, int y)? CellFromPoint(Grid grid, int x, int y)
        {
            if (x < 0 || y < 0 || grid.DisplayWidth <= x || grid.DisplayHeight <= y)
            {
                return null;
            }

            var cellWidth = grid.DisplayWidth / grid.Columns;
            var cellHeight = grid.DisplayHeight / grid.Rows;
            return (
                (int)Math.Floor((double)x / cellWidth) - 2,
                (int)Math.Floor((double)y / cellHeight) - 2
            );

        }

        /// <summary>
        /// Creates a bitmap of the given grid.
        /// </summary>
        /// <param name="grid">the grid to draw</param>
        /// <returns>a bitmap with the configured grid drawn on it</returns>
        public static Bitmap DrawGrid(Grid grid) =>
            DrawGrid(grid.DisplayWidth, grid.DisplayHeight, grid.Columns, grid.Rows, grid.Cells);

        /// <summary>
        /// Creates a bitmap of the given dimensions and renders a grid with the specified dimensions and optional cell contents.
        /// </summary>
        /// <param name="bitmapWidth">width of the bitmap in pixels</param>
        /// <param name="bitmapHeight">height of the bitmap in pixels</param>
        /// <param name="gridColumns">number of columns the grid should have</param>
        /// <param name="gridRows">number of rows the grid should have</param>
        /// <param name="gridCells">the optional cell contents</param>
        /// <returns>a bitmap with the configured grid drawn on it</returns>
        public static Bitmap DrawGrid(
            int bitmapWidth,
            int bitmapHeight,
            int gridColumns,
            int gridRows,
            params GridCell[] gridCells
        ) =>
            DrawGrid(bitmapWidth, bitmapHeight, gridColumns, gridRows, DefaultFont, gridCells);

        /// <summary>
        /// Creates a bitmap of the given dimensions and renders a grid with the specified dimensions and optional cell contents.
        /// </summary>
        /// <param name="bitmapWidth">width of the bitmap in pixels</param>
        /// <param name="bitmapHeight">height of the bitmap in pixels</param>
        /// <param name="gridColumns">number of columns the grid should have</param>
        /// <param name="gridRows">number of rows the grid should have</param>
        /// <param name="font">the font to use for text rendering</param>
        /// <param name="gridCells">the optional cell contents</param>
        /// <returns>a bitmap with the configured grid drawn on it</returns>
        public static Bitmap DrawGrid(
            int bitmapWidth,
            int bitmapHeight,
            int gridColumns,
            int gridRows,
            Font font,
            params GridCell[] gridCells
        ) =>
            DrawGrid(new Bitmap(bitmapWidth, bitmapHeight), gridColumns, gridRows, font, gridCells);

        /// <summary>
        /// Renders a grid on the provided bitmap with the specified dimensions and optional cell contents.
        /// </summary>
        /// <param name="bitmap">the bitmap to render the grid on</param>
        /// <param name="gridColumns">number of columns the grid should have</param>
        /// <param name="gridRows">number of rows the grid should have</param>
        /// <param name="gridCells">the optional cell contents</param>
        /// <returns>a bitmap with the configured grid drawn on it</returns>
        public static Bitmap DrawGrid(Bitmap bitmap, int gridColumns, int gridRows, params GridCell[] gridCells) =>
            DrawGrid(bitmap, gridColumns, gridRows, DefaultFont, gridCells);

        /// <summary>
        /// Renders a grid on the provided bitmap with the specified dimensions and optional cell contents.
        /// </summary>
        /// <param name="bitmap">the bitmap to render the grid on</param>
        /// <param name="gridColumns">number of columns the grid should have</param>
        /// <param name="gridRows">number of rows the grid should have</param>
        /// <param name="font">the font to use for text rendering</param>
        /// <param name="gridCells">the optional cell contents</param>
        /// <returns>a bitmap with the configured grid drawn on it</returns>
        public static Bitmap DrawGrid(
            Bitmap bitmap,
            int gridColumns,
            int gridRows,
            Font font,
            params GridCell[] gridCells
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

                // Draw the cell contents in order of precedence (color-only first, then text, so that text doesn't get rendered under background colors)
                foreach (var gridCell in gridCells.OrderBy(GridCell.CalculatePrecedenceKey))
                {
                    var x = gridCell.X * cellWidth;
                    var y = gridCell.Y * cellHeight;

                    // Draw the background color (if any for this cell)
                    if (gridCell.Color != null && gridCell.Color != Color.Transparent && gridCell.Color.Value.A != 0)
                    {
                        graphics.FillRectangle(
                            new SolidBrush(ColorSelection), new Rectangle(x, y, cellWidth, cellHeight)
                        );
                    }

                    // Draw the text (if any for this cell)
                    if (!string.IsNullOrWhiteSpace(gridCell.Label))
                    {
                        var measurement = graphics.MeasureString(gridCell.Label, font);
                        graphics.DrawString(
                            gridCell.Label, font, new SolidBrush(gridCell.LabelColor ?? ColorForeground),
                            x + (cellWidth - measurement.Width) / 2, y + (cellHeight - measurement.Height) / 2
                        );
                    }
                }

                // Draw separator lines (1 less than the number of columns/rows
                // Columns and rows are combined into a single for-loop to reduce the number of iterations
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
