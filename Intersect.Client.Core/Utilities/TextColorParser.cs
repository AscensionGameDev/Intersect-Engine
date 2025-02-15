using System.Reflection;
using System.Text.RegularExpressions;

namespace Intersect.Client.Utilities
{
    public static class TextColorParser
    {
        private static readonly Dictionary<string, Func<Color>> KnownColors = InitializeColorDictionary();
        private static readonly Regex ColorTagRegex = new(@"\\c{([^}]*)}", RegexOptions.Compiled);
        private static readonly string SplitPattern = @"(\\c{[^}]*})";

        private static Dictionary<string, Func<Color>> InitializeColorDictionary()
        {
            return typeof(Color)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(Color))
                .ToDictionary(
                    p => p.Name.ToLowerInvariant(),
                    p => (Func<Color>)(() => p.GetMethod?.Invoke(null, null) as Color ?? Color.White)
                );
        }

        public static List<ColorizedText> Parse(string text, Color defaultColor)
        {
            var output = new List<ColorizedText>();
            var segments = Regex.Split(text, SplitPattern)
                .Where(s => !string.IsNullOrWhiteSpace(s));

            var currentColor = defaultColor;

            foreach (var segment in segments)
            {
                var match = ColorTagRegex.Match(segment);

                if (match.Success)
                {
                    string colorCode = match.Groups[1].Value.ToLowerInvariant();

                    if (KnownColors.TryGetValue(colorCode, out Func<Color>? colorFunc))
                    {
                        currentColor = colorFunc();
                    }
                    else
                    {
                        currentColor = Color.FromHex(colorCode, defaultColor) ?? defaultColor;
                    }
                }
                else
                {
                    output.Add(new ColorizedText(segment, currentColor));
                }
            }

            return output;
        }
    }

    public class ColorizedText
    {
        public string Text { get; }
        public Color Color { get; }

        public ColorizedText(string text, Color color)
        {
            Text = text;
            Color = color;
        }
    }
}
