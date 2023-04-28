using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Intersect.Editor.General;

namespace Intersect.Editor.Configuration
{
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
            ToolCursor toolCursor = new ToolCursor();
            foreach (EditingTool tool in Enum.GetValues(typeof(EditingTool)))
            {
                string fileName = $"resources/cursors/editor_{tool.ToString().ToLowerInvariant()}.json";

                if (File.Exists(fileName))
                {
                    if (!_toolCursorDict.TryGetValue(tool, out ToolCursor cursor))
                    {
                        continue;
                    }

                    string json = File.ReadAllText(fileName);
                    cursor = JsonConvert.DeserializeObject<ToolCursor>(json);
                    _toolCursorDict[tool] = cursor;
                }
                else
                {
                    string json = JsonConvert.SerializeObject(toolCursor);
                    File.WriteAllText(fileName, json);
                    _toolCursorDict[tool] = toolCursor;
                }
            }
        }
    }
}