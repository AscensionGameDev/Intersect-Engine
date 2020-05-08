using Intersect.Config;
using Intersect.Server.Entities;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Server.Database.PlayerData.Players
{
	public class PlayerJob
	{
		public PlayerJob()
		{ }

		public int JobIdentity { get; set; } = (int)JobInfo.JobIdentityEnum.None;
		public int JobLevel { get; set; } = 1;
		public int JobExp { get; set; } = 0;

		public string JobData { get; set; } = "";

		public void CheckLevel(Player player)
		{
			if (JobLevel < JobInfo.JobMaxLevel)
			{
				int nextLevel = JobInfo.GetNextLevelExp(JobLevel);
				if (JobExp >= nextLevel)
				{
					JobExp -= nextLevel;
					JobLevel += 1;
					PacketSender.SendChatMsg(player, $"Vous êtes niveau {JobLevel} {JobInfo.JobName[JobIdentity]}.", Color.Green);
				}
				else
				{
					PacketSender.SendChatMsg(player, $"Votre progréssion pour votre métier de {JobInfo.JobName[JobIdentity]} est de {JobExp}/{nextLevel}xp.", Color.Green);
				}
			}
			else
			{
				JobExp = 0;
				PacketSender.SendChatMsg(player, "Pas d'expérience supplémentaire pour votre métier, vous êtes déjà au niveau maximum.", Color.Green);
			}
		}

		
	}
}
