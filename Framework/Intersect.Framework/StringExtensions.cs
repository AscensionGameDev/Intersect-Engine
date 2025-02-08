using System.Text;

namespace Intersect.Framework;

public static class StringExtensions
{
    public static string Reverse(this string @string)
    {
        var codepoints = new int[@string.Length];
        var numCodepoints = 0;
        for (var index = 0; index < @string.Length; ++index)
        {
            ++numCodepoints;

            codepoints[index] = char.ConvertToUtf32(@string, index);
            if (char.IsSurrogatePair(@string, index))
            {
                ++index;
            }
        }

        codepoints = codepoints[..numCodepoints];
        Array.Reverse(codepoints);

        return codepoints.Aggregate(string.Empty, (current, codepoint) => current + char.ConvertFromUtf32(codepoint));
    }
}