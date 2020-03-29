using System;
using System.Collections;
using System.Collections.Generic;

namespace Intersect
{

    public class AlphanumComparatorFast : IComparer, IComparer<string>
    {

        public int Compare(object x, object y)
        {
            return Compare(x as string, y as string);
        }

        public int Compare(string x, string y)
        {
            if (string.IsNullOrEmpty(x) || string.IsNullOrEmpty(y))
            {
                return 0;
            }

            var len1 = x.Length;
            var len2 = y.Length;
            var marker1 = 0;
            var marker2 = 0;

            // Walk through two the strings with two markers.
            while (marker1 < len1 && marker2 < len2)
            {
                var ch1 = x[marker1];
                var ch2 = y[marker2];

                // Some buffers we can build up characters in for each chunk.
                var space1 = new char[len1];
                var loc1 = 0;
                var space2 = new char[len2];
                var loc2 = 0;

                // Walk through all following characters that are digits or
                // characters in BOTH strings starting at the appropriate marker.
                // Collect char arrays.
                do
                {
                    space1[loc1++] = ch1;
                    marker1++;

                    if (marker1 < len1)
                    {
                        ch1 = x[marker1];
                    }
                    else
                    {
                        break;
                    }
                } while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

                do
                {
                    space2[loc2++] = ch2;
                    marker2++;

                    if (marker2 < len2)
                    {
                        ch2 = y[marker2];
                    }
                    else
                    {
                        break;
                    }
                } while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

                // If we have collected numbers, compare them numerically.
                // Otherwise, if we have strings, compare them alphabetically.
                var str1 = new string(space1);
                var str2 = new string(space2);

                int result;

                if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
                {
                    var thisNumericChunk = 0;
                    var thatNumericChunk = 0;
                    int.TryParse(str1, out thisNumericChunk);
                    int.TryParse(str2, out thatNumericChunk);
                    result = thisNumericChunk.CompareTo(thatNumericChunk);
                }
                else
                {
                    result = string.Compare(str1, str2, StringComparison.Ordinal);
                }

                if (result != 0)
                {
                    return result;
                }
            }

            return len1 - len2;
        }

    }

}
