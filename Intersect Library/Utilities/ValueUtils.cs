using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect.Utilities
{
    public static class ValueUtils
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            var temp = a;
            a = b;
            b = temp;
        }
    }
}
