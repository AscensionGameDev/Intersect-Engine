using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect_Library
{
    public static class TextUtils
    {
        public static bool IsEmpty(string str)
        {
            return (str == null || str.Length < 1);
        }
    }
}
