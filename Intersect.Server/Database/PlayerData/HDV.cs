using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using Intersect.Server.Localization;
using Intersect.Server.Networking;
using Intersect.Utilities;
using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;


namespace Intersect.Server.Database.PlayerData
{
	public class HDV
	{
		public HDV()
		{

		}

		public HDV(Guid hdv, Guid seller, Guid itemid, int quantity, int[] statBuffs, Dictionary<string, int> tags, int price, Dictionary<string, string> stringtags)
		{
			HDVId = hdv;
			SellerId = seller;
			ItemId = itemid;
			Quantity = quantity;
			StatBuffs = statBuffs;
			Tags = tags;
			Price = price;
			StringTags = stringtags;
		}

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; private set; }

		public Guid HDVId { get; set; } = Guid.Empty;

		public Guid SellerId { get; set; } = Guid.Empty;

		public Guid ItemId { get; set; } = Guid.Empty;

		public int Quantity { get; set; }

		[NotMapped, JsonIgnore]
		public Dictionary<string, int> Tags = new Dictionary<string, int>();

		[Column("Tags")]
		public string JsonTags
		{
			get => JsonConvert.SerializeObject(Tags);
			set
			{
				if (value == null)
					Tags = new Dictionary<string, int>();
				else
					Tags = JsonConvert.DeserializeObject<Dictionary<string, int>>(value);
			}
		}

		[Column("StatBuffs")]
		[JsonIgnore]
		public string StatBuffsJson
		{
			get => DatabaseUtils.SaveIntArray(StatBuffs, (int)Enums.Stats.StatCount);
			set => StatBuffs = DatabaseUtils.LoadIntArray(value, (int)Enums.Stats.StatCount);
		}
		[NotMapped]
		public int[] StatBuffs { get; set; } = new int[(int)Enums.Stats.StatCount];

		[NotMapped]
		public Dictionary<string, string> StringTags = new Dictionary<string, string>();

		[Column("StringTags")]
		[JsonIgnore]
		public string JsonStringTags
		{
			get => JsonConvert.SerializeObject(StringTags);
			set
			{
				if (value == null)
					StringTags = new Dictionary<string, string>();
				else
					StringTags = JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
			}
		}

		public int Price { get; set; }


		#region DatabaseRequest

		public static void DeletePlayer(Guid sellerDeleted)
		{
			lock (DbInterface.GetPlayerContextLock())
			{
				var context = DbInterface.GetPlayerContext();
				context.HDV.RemoveRange(context.HDV.Where(h => h.SellerId == sellerDeleted));
			}
		}

		public static void Remove(HDV item)
		{
			lock (DbInterface.GetPlayerContextLock())
			{
				var context = DbInterface.GetPlayerContext();
				context.HDV.Remove(item);
			}
		}

		public static void Add(HDV hdv)
		{

			lock (DbInterface.GetPlayerContextLock())
			{
				var context = DbInterface.GetPlayerContext();
				context.HDV.Add(hdv);
			}
			DbInterface.SavePlayerDatabaseAsync();
		}

		[NotNull]
		public static IEnumerable<HDV> List(Guid hdvID)
		{
			if (hdvID == null || hdvID == Guid.Empty)
			{
				return null;
			}
			lock (DbInterface.GetPlayerContextLock())
			{
				var context = DbInterface.GetPlayerContext();
				try
				{
					return QueryHDV(context, hdvID) ?? throw new InvalidOperationException();
				}
				catch (Exception exception)
				{
					exception.ToString();
					throw;
				}
			}
		}

		public static HDV Get(Guid id)
		{
			lock (DbInterface.GetPlayerContextLock())
			{
				var context = DbInterface.GetPlayerContext();
				try
				{
					return QueryHDVByID(context, id) ?? null;
				}
				catch (Exception exception)
				{
					exception.ToString();
					throw;
				}
			}
		}


		[NotNull]
		private static readonly Func<PlayerContext, Guid, IEnumerable<HDV>> QueryHDV =
			EF.CompileQuery(
				(PlayerContext context, Guid hdvID) => context.HDV.Where(h => h.HDVId == hdvID)
			) ??
			throw new InvalidOperationException();

		private static readonly Func<PlayerContext, Guid, HDV> QueryHDVByID =
			EF.CompileQuery(
				(PlayerContext context, Guid hdvID) => context.HDV.Where(h => h.Id == hdvID).FirstOrDefault()
			) ??
			throw new InvalidOperationException();

		#endregion
	}
}
