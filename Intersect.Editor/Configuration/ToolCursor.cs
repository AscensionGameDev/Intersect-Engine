using Newtonsoft.Json;
using Intersect.Editor.General;

namespace Intersect.Editor.Configuration;

public sealed class ToolCursor
{
    public Point CursorClickPoint { get; set; }

    private static readonly Dictionary<EditingTool, ToolCursor> _toolCursorDict;

    public static IReadOnlyDictionary<EditingTool, ToolCursor> ToolCursorDict => _toolCursorDict;

    static ToolCursor()
    {
        _toolCursorDict = new Dictionary<EditingTool, ToolCursor>
        {
            { EditingTool.Brush, new ToolCursor() },
            { EditingTool.Dropper, new ToolCursor() },
            { EditingTool.Erase, new ToolCursor() },
            { EditingTool.Fill, new ToolCursor() },
            { EditingTool.Rectangle, new ToolCursor() },
            { EditingTool.Selection, new ToolCursor() }
        };
    }

    public static void Load()
    {
        const string cursorFolder = "resources/cursors/";

        if (!Directory.Exists(cursorFolder))
        {
            return;
        }

        foreach (EditingTool tool in Enum.GetValues(typeof(EditingTool)))
        {
            var fileName = $"{cursorFolder}editor_{tool.ToString().ToLowerInvariant()}.json";
            ToolCursor toolCursor;

            if (File.Exists(fileName))
            {
                if (!_toolCursorDict.TryGetValue(tool, out toolCursor))
                {
                    continue;
                }

                var json = File.ReadAllText(fileName);
                toolCursor = JsonConvert.DeserializeObject<ToolCursor>(json);
            }
            else
            {
                toolCursor = new ToolCursor();
                var json = JsonConvert.SerializeObject(toolCursor);
                File.WriteAllText(fileName, json);
            }

            _toolCursorDict[tool] = toolCursor;
        }
    }
}