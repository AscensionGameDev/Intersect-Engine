using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Intersect.Enums;
using Intersect.Server.Entities;
using JetBrains.Annotations;
using Intersect.Server.Networking;

namespace Intersect.Server.Database.PlayerData.Players
{
    /// <summary>
    /// A class containing the definition of each guild, alongside the methods to use them.
    /// </summary>
    public class Guild
    {

        // Entity Framework Garbage.
        public Guild()
        {
        }

        /// <summary>
        /// Create a new Guild instance.
        /// </summary>
        /// <param name="creator">The <see cref="Player"/> that created the guild.</param>
        /// <param name="name">The Name of the guild.</param>
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
        public Guid Id { get; private set; }

        /// <summary>
        /// The name of the guild.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The date on which this guild was founded.
        /// </summary>
        public DateTime FoundingDate { get; private set; }

        /// <summary>
        /// Contains a record of all guild members
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<Player> Members { get; private set; } = new List<Player>();

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

            // Send our new guild list to everyone that's online.
            UpdateMemberList();

            // Send our entity data to nearby players.
            PacketSender.SendEntityDataToProximity(player);
        }

        /// <summary>
        /// Removes a player from the guild.
        /// </summary>
        /// <param name="player">The player to remove from the guild.</param>
        public void RemoveMember(Player player)
        {
            player.Guild = null;
            player.GuildRank = GuildRanks.Recruit;
            player.GuildInvite = null;

            Members.Remove(player);

            // Send our new guild list to everyone that's online.
            UpdateMemberList();

            // Send our entity data to nearby players.
            PacketSender.SendEntityDataToProximity(player);
        }

        /// <summary>
        /// Send an updated version of our guild's memberlist to each online member.
        /// </summary>
        public void UpdateMemberList()
        {
            foreach (var member in FindOnlineMembers())
            {
                PacketSender.SendGuild(member);
            }
        }

        /// <summary>
        /// Sets a player's guild rank.
        /// </summary>
        /// <param name="id">The player to set the rank for.</param>
        /// <param name="rank">The rank to assign.</param>
        public void SetPlayerRank(Guid id, GuildRanks rank) => SetPlayerRank(Player.Find(id), rank);

        /// <summary>
        /// Sets a player's guild rank.
        /// </summary>
        /// <param name="player">The player to set the rank for.</param>
        /// <param name="rank">The rank to assign.</param>
        public void SetPlayerRank(Player player, GuildRanks rank)
        {
            player.GuildRank = rank;

            // Send our new guild list to everyone that's online.
            UpdateMemberList();
        }

        /// <summary>
        /// Returns the rank of a player within this guild.
        /// </summary>
        /// <param name="id">The Id of the player to search for.</param>
        /// <returns>Returns the rank of a player within this guild.</returns>
        public GuildRanks GetPlayerRank(Guid id) => GetPlayerRank(Player.Find(id));

        /// <summary>
        /// Returns the rank of a player within this guild.
        /// </summary>
        /// <param name="player">The player to search for.</param>
        /// <returns>Returns the rank of a player within this guild.</returns>
        public GuildRanks GetPlayerRank(Player player) => player.GuildRank;

        /// <summary>
        /// Check whether a specified player is a member of this guild.
        /// </summary>
        /// <param name="id">The Id to check against.</param>
        /// <returns>Whether or not this player is a member of this guild</returns>
        public bool IsMember(Guid id) => IsMember(Player.Find(id));

        /// <summary>
        /// Check whether a specified player is a member of this guild.
        /// </summary>
        /// <param name="player">The player to check against.</param>
        /// <returns>Whether or not this player is a member of this guild</returns>
        public bool IsMember(Player player)
        {
            return Members.Any(m => m == player);
        }

        /// <summary>
        /// Search for a guild by Id.
        /// </summary>
        /// <param name="context">The playercontext to search through.</param>
        /// <param name="id">The guild Id to search for.</param>
        /// <returns>Returns a <see cref="Guild"/> that matches the Id, if any.</returns>
        public static Guild GetGuild(Guid id, PlayerContext context = null)
        {
            if (context == null)
            {
                lock (DbInterface.GetPlayerContextLock())
                {
                    context = DbInterface.GetPlayerContext();

                    return context.Guilds.Where(p => p.Id == id).SingleOrDefault();
                }
            }
            else
            {
                return context.Guilds.Where(p => p.Id == id).SingleOrDefault();
            }
        }

        /// <summary>
        /// Search for a guild by Name.
        /// </summary>
        /// <param name="context">The playercontext to search through.</param>
        /// <param name="name">The guild Name to search for.</param>
        /// <returns>Returns a <see cref="Guild"/> that matches the Name, if any.</returns>
        public static Guild GetGuild(string name, PlayerContext context = null)
        {
            if (context == null)
            {
                lock (DbInterface.GetPlayerContextLock())
                {
                    context = DbInterface.GetPlayerContext();

                    return context.Guilds.Where(p => p.Name == name).SingleOrDefault();
                }
            }
            else
            {
                return context.Guilds.Where(p => p.Name == name).SingleOrDefault();
            }
        }

        /// <summary>
        /// Completely removes a guild from the game.
        /// </summary>
        /// <param name="context">The playercontext to delete the guild from.</param>
        /// <param name="guild">The <see cref="Guild"/> to delete.</param>
        public static void DeleteGuild(Guild guild, PlayerContext context = null)
        {
            // Remove our members cleanly before deleting this from our database.
            foreach (var member in guild.Members.ToArray())
            {
                guild.RemoveMember(member);
            }

            if (context == null)
            {
                lock (DbInterface.GetPlayerContextLock())
                {
                    context = DbInterface.GetPlayerContext();

                    context.Guilds.Remove(guild);
                }
            }
            else
            {
                context.Guilds.Remove(guild);
            }
        }
    }
}
