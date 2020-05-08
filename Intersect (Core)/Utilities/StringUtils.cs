using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Utilities
{
	public static class StringUtils
	{
		public static bool EqualIgnoreCase(this string str, string other)
		{
			return str.ToLower().Equals(other.ToLower());
		}

		public static bool ContainsIgnoreCase(this string str, string other)
		{
			return str.ToLower().Contains(other.ToLower());
		}
	}
}
