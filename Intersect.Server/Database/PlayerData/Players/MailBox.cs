using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Intersect.Server.Entities;
using Intersect.Utilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Intersect.Server.Database.PlayerData.Players
{
	public class MailBox
	{
		public MailBox()
		{
		}

		public MailBox(Player sender, Player to, string title, string msg, Guid itemid, int quantity, int[] statBuffs)
		{
			Sender = sender;
			Player = to;
			Title = title;
			Message = msg;
			ItemId = itemid;
			Quantity = quantity;
			StatBuffs = statBuffs;
		}

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; private set; }

		[JsonProperty(nameof(Player))]
		private Guid JsonPlayerId => Player?.Id ?? Guid.Empty;

		[JsonProperty(nameof(Sender))]
		private Guid JsonTargetId => Sender?.Id ?? Guid.Empty;

		[JsonIgnore]
		public virtual Player Player { get; private set; }
		
		[JsonIgnore]
		public virtual Player Sender { get; private set; }

		public string Title { get; private set; }

		public string Message { get; private set; }

		public Guid ItemId { get; set; } = Guid.Empty;

		public int Quantity { get; set; }

		[Column("StatBuffs")]
		[JsonIgnore]
		public string StatBuffsJson
		{
			get => DatabaseUtils.SaveIntArray(StatBuffs, (int)Enums.Stats.StatCount);
			set => StatBuffs = DatabaseUtils.LoadIntArray(value, (int)Enums.Stats.StatCount);
		}
		[NotMapped]
		public int[] StatBuffs { get; set; } = new int[(int)Enums.Stats.StatCount];

		public static void GetMails(PlayerContext context, Player player)
		{
			var mail = context.Player_MailBox.Where(p => player.Id == p.Player.Id).Include(p => p.Sender).ToList();
			if (mail != null)
			{
				player.MailBoxs = mail;
			}
		}

	}
}
