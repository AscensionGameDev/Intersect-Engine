using System.Reflection;
using System.Text.RegularExpressions;

namespace Intersect.Client.Utilities
{
    public static class TextColorParser
    {
        private static readonly Regex ColorTagRegex = new(@"(\\c{[^}]*})", RegexOptions.Compiled);

        public static List<ColorizedText> Parse(string text, Color? defaultColor)
        {
            var segments = ColorTagRegex.Split(text)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();

            var output = new List<ColorizedText>(segments.Length);
            var currentColor = defaultColor;

            foreach (var segment in segments)
            {
                if (segment.StartsWith("\\c{") && segment.EndsWith("}"))
                {
                    string colorCode = segment[3..^1].ToLowerInvariant();
                    currentColor = Color.FromString(colorCode, defaultColor);
                }
                else
                {
                    output.Add(new ColorizedText(segment, currentColor));
                }
            }

            return output;
        }
    }
}
