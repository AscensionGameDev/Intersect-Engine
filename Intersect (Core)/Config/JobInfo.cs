using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Config
{
	public static class JobInfo
	{
		public enum JobIdentityEnum
		{
			None = 0,
			Lumberjack = 1,
			Minor = 2,
			Hunter = 3,
			Alchemist = 4
		}

		public static string[] JobName = { "Aucun", "Bucheron", "Mineur", "Chasseur", "Alchimiste"};

		public static int JobMaxLevel = 10;

		public static int GetNextLevelExp(int level)
		{
			return level * 100 + ((level - 1) * (49 + level));
		}
	}
}
