using System.Runtime.Serialization;
using Intersect.Configuration;
using Intersect.Editor.General;

namespace Intersect.Editor.Configuration
{
    /// <inheritdoc />
    /// <summary>
    /// Editor's cursor configuration.
    /// </summary>
    public sealed partial class ToolCursor : IConfiguration<ToolCursor>
    {
        /// <summary>
        /// X and Y click-points for the editor cursor.
        /// </summary>
        public Point CursorClickPoint { get; set; }

        public static ToolCursor Brush { get; } = new ToolCursor();

        public static ToolCursor Dropper { get; } = new ToolCursor();

        public static ToolCursor Erase { get; } = new ToolCursor();

        public static ToolCursor Fill { get; } = new ToolCursor();

        public static ToolCursor Rectangle { get; } = new ToolCursor();

        public static ToolCursor Selection { get; } = new ToolCursor();

        public static void LoadAndSave()
        {
            ConfigurationHelper.LoadSafely(Brush,
                $"resources/cursors/editor_{EditingTool.Brush.ToString().ToLowerInvariant()}.json");
            ConfigurationHelper.LoadSafely(Dropper,
                $"resources/cursors/editor_{EditingTool.Dropper.ToString().ToLowerInvariant()}.json");
            ConfigurationHelper.LoadSafely(Erase,
                $"resources/cursors/editor_{EditingTool.Erase.ToString().ToLowerInvariant()}.json");
            ConfigurationHelper.LoadSafely(Fill,
                $"resources/cursors/editor_{EditingTool.Fill.ToString().ToLowerInvariant()}.json");
            ConfigurationHelper.LoadSafely(Rectangle,
                $"resources/cursors/editor_{EditingTool.Rectangle.ToString().ToLowerInvariant()}.json");
            ConfigurationHelper.LoadSafely(Selection,
                $"resources/cursors/editor_{EditingTool.Selection.ToString().ToLowerInvariant()}.json");
        }

        private void Validate()
        {
            CursorClickPoint = new Point(
                CursorClickPoint.X != default
                    ? CursorClickPoint.X
                    : 0,
                CursorClickPoint.Y != default
                    ? CursorClickPoint.Y
                    : 0);
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            Validate();
        }

        public ToolCursor Load(string filePath, bool failQuietly = false)
        {
            return ConfigurationHelper.Load(this, filePath, failQuietly);
        }

        public ToolCursor Save(string filePath, bool failQuietly = false)
        {
            return ConfigurationHelper.Save(this, filePath, failQuietly);
        }
    }
}
