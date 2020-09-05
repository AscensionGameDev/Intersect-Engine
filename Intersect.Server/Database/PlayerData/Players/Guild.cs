using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Intersect.Enums;
using Intersect.Server.Entities;
using JetBrains.Annotations;

namespace Intersect.Server.Database.PlayerData.Players
{
    /// <summary>
    /// A class containing the definition of each guild, alongside the methods to use them.
    /// </summary>
    public class Guild
    {

        public Guild()
        {
        }

        public Guild(Player creator, string name)
        {
            Name = name;
            FoundingDate = DateTime.Now;
        }
        
        /// <summary>
        /// The database Id of the guild.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [NotNull]
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the guild.
        /// </summary>
        public string Name { get; set; }

        public DateTime FoundingDate { get; set; }

        /// <summary>
        /// Contains a record of all guild members
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<Player> Members { get; set; } = new List<Player>();

        /// <summary>
        /// Contains a record of all guild members ranks.
        /// </summary>
        [NotMapped]
        public Dictionary<Guid, GuildRanks> MemberRanks { get; set; } = new Dictionary<Guid, GuildRanks>();

        /// <summary>
        /// A Jsonified version of the guild member ranks record.
        /// </summary>
        [Column("MemberRanks")]
        public string MemberRanksJson
        {
            get => JsonConvert.SerializeObject(MemberRanks);
            set => MemberRanks = JsonConvert.DeserializeObject<Dictionary<Guid, GuildRanks>>(value);
        }

        /// <summary>
        /// Find all online members of this guild.
        /// </summary>
        /// <returns>A list of online players.</returns>
        public List<Player> FindOnlineMembers()
        {
            var online = new List<Player>();
            foreach (var member in Members)
            {
                if (Player.FindOnline(member.Id) != null)
                {
                    online.Add(member);
                }
            }

            return online;
        }

        /// <summary>
        /// Joins a player into the guild.
        /// </summary>
        /// <param name="player">The player to join into the guild.</param>
        /// <param name="rank">The rank to give this new member.</param>
        public void AddMember(Player player, GuildRanks rank)
        {
            player.Guild = this;
            Members.Add(player);
            SetPlayerRank(player, rank);
        }

        /// <summary>
        /// Sets a player's guild rank.
        /// </summary>
        /// <param name="player">The player to set the rank for.</param>
        /// <param name="rank">The rank to assign.</param>
        public void SetPlayerRank(Player player, GuildRanks rank) => SetPlayerRank(player.Id, rank);

        /// <summary>
        /// Sets a player's guild rank.
        /// </summary>
        /// <param name="id">The player to set the rank for.</param>
        /// <param name="rank">The rank to assign.</param>
        public void SetPlayerRank(Guid id, GuildRanks rank)
        {
            MemberRanks[id] = rank;
        }

        /// <summary>
        /// Returns the rank of a player within this guild.
        /// </summary>
        /// <param name="player">The player to search for.</param>
        /// <returns>Returns the rank of a player within this guild.</returns>
        public GuildRanks GetPlayerRank(Player player) => GetPlayerRank(player.Id);

        /// <summary>
        /// Returns the rank of a player within this guild.
        /// </summary>
        /// <param name="id">The Id of the player to search for.</param>
        /// <returns>Returns the rank of a player within this guild.</returns>
        public GuildRanks GetPlayerRank(Guid id)
        {
            return MemberRanks.Where(m => m.Key == id).SingleOrDefault().Value;
        }

        /// <summary>
        /// Check whether a specified player is a member of this guild.
        /// </summary>
        /// <param name="player">The player to check against.</param>
        /// <returns>Whether or not this player is a member of this guild</returns>
        public bool IsMember(Player player) => IsMember(player.Id);

        /// <summary>
        /// Check whether a specified player is a member of this guild.
        /// </summary>
        /// <param name="id">The Id to check against.</param>
        /// <returns>Whether or not this player is a member of this guild</returns>
        public bool IsMember(Guid id)
        {
            if (Members.Any(m => m.Id == id))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Search for a guild by Id.
        /// </summary>
        /// <param name="context">The playercontext to search through.</param>
        /// <param name="id">The guild Id to search for.</param>
        /// <returns>Returns a <see cref="Guild"/> that matches the Id, if any.</returns>
        public static Guild GetGuild(PlayerContext context, Guid id)
        {
            var guild = context.Guilds.Where(p => p.Id == id).SingleOrDefault();

            return guild;
        }

        /// <summary>
        /// Search for a guild by Name.
        /// </summary>
        /// <param name="context">The playercontext to search through.</param>
        /// <param name="name">The guild Name to search for.</param>
        /// <returns>Returns a <see cref="Guild"/> that matches the Name, if any.</returns>
        public static Guild GetGuild(PlayerContext context, string name)
        {
            var guild = context.Guilds.Where(p => p.Name == name).SingleOrDefault();

            return guild;
        }
    }
}
