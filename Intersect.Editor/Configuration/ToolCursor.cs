using Newtonsoft.Json;
using Intersect.Editor.General;

namespace Intersect.Editor.Configuration;

public sealed class ToolCursor
{
    public const string CursorsFolder = "resources/cursors";
    
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
        if (!Directory.Exists(CursorsFolder))
        {
            return;
        }

        foreach (EditingTool tool in Enum.GetValues(typeof(EditingTool)))
        {
            var fileName = $"editor_{tool.ToString().ToLowerInvariant()}.json";
            var filePath = Path.Combine(CursorsFolder, fileName);
            ToolCursor toolCursor;

            if (File.Exists(filePath))
            {
                if (!_toolCursorDict.TryGetValue(tool, out toolCursor))
                {
                    continue;
                }

                var json = File.ReadAllText(filePath);
                toolCursor = JsonConvert.DeserializeObject<ToolCursor>(json);
            }
            else
            {
                toolCursor = new ToolCursor();
                var json = JsonConvert.SerializeObject(toolCursor);
                File.WriteAllText(filePath, json);
            }

            _toolCursorDict[tool] = toolCursor;
        }
    }
}