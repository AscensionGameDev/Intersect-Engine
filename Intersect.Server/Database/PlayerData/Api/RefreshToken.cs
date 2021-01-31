using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Intersect.Logging;

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
            RefreshToken token,
            bool commit = false,
            bool checkForDuplicates = true
        )
        {
            using (var context = DbInterface.CreatePlayerContext(readOnly: false))
            {
                if (context.RefreshTokens == null)
                {
                    return false;
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

                context.RefreshTokens.Add(token);
                context.ChangeTracker.DetectChanges();
                context.SaveChanges();

                return true;
            }
        }

        public static RefreshToken Find(Guid id)
        {
            try
            {
                using (var context = DbInterface.CreatePlayerContext())
                {
                    return context?.RefreshTokens?.Find(id);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        public static RefreshToken FindForTicket(Guid ticketId)
        {
            if (ticketId == Guid.Empty)
            {
                return null;
            }

            try
            {
                using (var context = DbInterface.CreatePlayerContext())
                {
                    var refreshToken = context?.RefreshTokens.FirstOrDefault(queryToken => queryToken.TicketId == ticketId);

                    if (refreshToken == null || DateTime.UtcNow < refreshToken.Expires)
                    {
                        return refreshToken;
                    }

                    Remove(refreshToken, true);

                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        public static IEnumerable<RefreshToken> FindForClient(Guid clientId)
        {
            if (clientId == Guid.Empty)
            {
                return null;
            }

            try
            {
                using (var context = DbInterface.CreatePlayerContext())
                {
                    var tokenQuery = context?.RefreshTokens.Where(queryToken => queryToken.ClientId == clientId);

                    return tokenQuery.AsEnumerable()?.ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        public static IEnumerable<RefreshToken> FindForUser(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return null;
            }

            try
            {
                using (var context = DbInterface.CreatePlayerContext())
                {
                    var tokenQuery = context?.RefreshTokens.Where(queryToken => queryToken.UserId == userId);

                    return tokenQuery.AsEnumerable()?.ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        public static IEnumerable<RefreshToken> FindForUser(User user)
        {
            return FindForUser(user.Id);
        }

        public static RefreshToken FindOneForUser(Guid userId)
        {
            try
            {
                using (var context = DbInterface.CreatePlayerContext())
                {
                    var token = context?.RefreshTokens.First(queryToken => queryToken.UserId == userId);

                    return token;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        public static RefreshToken FindOneForUser(User user)
        {
            return FindOneForUser(user.Id);
        }

        public static bool Remove(Guid id, bool commit = false)
        {
            var token = Find(id);

            return token != null && Remove(token, commit);
        }

        public static bool Remove(RefreshToken token, bool commit = false)
        {
            try
            {
                using (var context = DbInterface.CreatePlayerContext(readOnly: false))
                {
                    context?.RefreshTokens.Remove(token);
                    context?.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        public static bool RemoveAll(IEnumerable<RefreshToken> tokens, bool commit = false)
        {
            try
            {
                using (var context = DbInterface.CreatePlayerContext(readOnly: false))
                {
                    context?.RefreshTokens.RemoveRange(tokens);
                    context?.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

    }

}
