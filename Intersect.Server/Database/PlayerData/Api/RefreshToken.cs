using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace Intersect.Server.Database.PlayerData.Api
{

    public class RefreshToken
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 0)]
        [Key]
        public Guid Id { get; set; }

        [Required, ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        public Guid ClientId { get; set; }

        [Required]
        public string Subject { get; set; }

        public DateTime Issued { get; set; }

        public DateTime Expires { get; set; }

        [Required]
        public Guid TicketId { get; set; }

        [Required]
        public string Ticket { get; set; }

        public static async ValueTask<bool> Add(
            [NotNull] RefreshToken token,
            bool commit = false,
            bool checkForDuplicates = true
        )
        {
            lock (DbInterface.GetPlayerContextLock())
            {
                if (DbInterface.GetPlayerContext()?.RefreshTokens == null)
                {
                    return false;
                }
            }

            if (checkForDuplicates)
            {
                var duplicate = Find(token.Id);
                if (duplicate != null && !Remove(token.Id, commit))
                {
                    return false;
                }

                var forClient = FindForClient(token.ClientId)?.ToList();
                if (forClient != null && !RemoveAll(forClient, commit))
                {
                    return false;
                }

                var forUser = FindForUser(token.UserId)?.ToList();
                if (forUser != null && !RemoveAll(forUser, commit))
                {
                    return false;
                }
            }

            lock (DbInterface.GetPlayerContextLock())
            {
                var context = DbInterface.GetPlayerContext();
                context.RefreshTokens.Add(token);
                context.SaveChanges();
            }

            return true;
        }

        [CanBeNull]
        public static RefreshToken Find(Guid id)
        {
            lock (DbInterface.GetPlayerContextLock())
            {
                return DbInterface.GetPlayerContext()?.RefreshTokens?.Find(id);
            }
        }

        public static RefreshToken FindForTicket([CanBeNull] Guid ticketId)
        {
            if (ticketId == Guid.Empty)
            {
                return null;
            }

            lock (DbInterface.GetPlayerContextLock())
            {
                var playerContext = DbInterface.GetPlayerContext();
                var refreshToken =
                    playerContext?.RefreshTokens.FirstOrDefault(queryToken => queryToken.TicketId == ticketId);

                if (refreshToken == null || DateTime.UtcNow < refreshToken.Expires)
                {
                    return refreshToken;
                }

                Remove(refreshToken, true);
            }

            return null;
        }

        public static IEnumerable<RefreshToken> FindForClient(Guid clientId)
        {
            if (clientId == Guid.Empty)
            {
                return null;
            }

            lock (DbInterface.GetPlayerContextLock())
            {
                var tokenQuery = DbInterface.GetPlayerContext()
                    ?.RefreshTokens.Where(queryToken => queryToken.ClientId == clientId);

                return tokenQuery.AsEnumerable()?.ToList();
            }
        }

        public static IEnumerable<RefreshToken> FindForUser(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return null;
            }

            lock (DbInterface.GetPlayerContextLock())
            {
                var tokenQuery = DbInterface.GetPlayerContext()
                    ?.RefreshTokens.Where(queryToken => queryToken.UserId == userId);

                return tokenQuery.AsEnumerable()?.ToList();
            }
        }

        public static IEnumerable<RefreshToken> FindForUser([NotNull] User user)
        {
            return FindForUser(user.Id);
        }

        public static RefreshToken FindOneForUser(Guid userId)
        {
            lock (DbInterface.GetPlayerContextLock())
            {
                var token = DbInterface.GetPlayerContext()
                    ?.RefreshTokens.First(queryToken => queryToken.UserId == userId);

                return token;
            }
        }

        public static RefreshToken FindOneForUser([NotNull] User user)
        {
            return FindOneForUser(user.Id);
        }

        public static bool Remove(Guid id, bool commit = false)
        {
            var token = Find(id);

            return token != null && Remove(token, commit);
        }

        public static bool Remove([NotNull] RefreshToken token, bool commit = false)
        {
            lock (DbInterface.GetPlayerContextLock())
            {
                DbInterface.GetPlayerContext()?.RefreshTokens.Remove(token);

                return true;
            }
        }

        public static bool RemoveAll([NotNull] IEnumerable<RefreshToken> tokens, bool commit = false)
        {
            lock (DbInterface.GetPlayerContextLock())
            {
                DbInterface.GetPlayerContext()?.RefreshTokens.RemoveRange(tokens);

                return true;
            }
        }

    }

}
