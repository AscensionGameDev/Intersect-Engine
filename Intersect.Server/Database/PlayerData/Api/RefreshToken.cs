using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Intersect.Logging;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

namespace Intersect.Server.Database.PlayerData.Api
{

    public partial class RefreshToken
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

        // To help avoid concurrency exceptions
        private static readonly ConcurrentDictionary<Guid, EntityState> _pendingChanges = new ConcurrentDictionary<Guid, EntityState>();

        public static async ValueTask<bool> TryAddAsync(
            RefreshToken token,
            bool checkForDuplicates = true,
            CancellationToken cancellationToken = default
        )
        {
            var tokensToRemove = new List<RefreshToken>();

            using (var context = DbInterface.CreatePlayerContext(readOnly: false))
            {
                try
                {
                    if (context.RefreshTokens == null)
                    {
                        return false;
                    }

                    if (checkForDuplicates)
                    {
                        if (TryFind(token.Id, out var duplicate))
                        {
                            tokensToRemove.Add(duplicate);
                        }

                        var forClient = FindExpiredForClient(token.ClientId)?.ToList();
                        if (forClient != null)
                        {
                            tokensToRemove.AddRange(forClient);
                        }

                        var forUser = FindExpiredForUser(token.UserId)?.ToList();
                        if (forUser != null)
                        {
                            tokensToRemove.AddRange(forUser);
                        }
                    }

                    _ = context.RefreshTokens.Add(token);
                    context.DetachExcept(token);
                    context.ChangeTracker.DetectChanges();

                    if (tokensToRemove.Count > 0)
                    {
                        _ = RemoveAllAsync(tokensToRemove, cancellationToken);
                    }

                    _ = _pendingChanges.TryAdd(token.Id, context.Entry(token).State);
                    _ = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    _ = _pendingChanges.TryRemove(token.Id, out _);

                    return true;
                }
                catch (DbUpdateConcurrencyException concurrencyException)
                {
                    concurrencyException.LogError();
                    throw;
                }
            }
        }

        public static bool TryFind(Guid id, out RefreshToken refreshToken)
        {
            try
            {
                using (var context = DbInterface.CreatePlayerContext())
                {
                    refreshToken = context?.RefreshTokens?.Find(id);
                    return refreshToken != default;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                refreshToken = default;
                return false;
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

                    _ = Remove(refreshToken);

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

        public static IEnumerable<RefreshToken> FindExpiredForClient(Guid clientId)
        {
            if (clientId == Guid.Empty)
            {
                return null;
            }

            try
            {
                using (var context = DbInterface.CreatePlayerContext())
                {
                    var tokenQuery = context?.RefreshTokens.Where(queryToken => queryToken.ClientId == clientId && queryToken.Expires < DateTime.UtcNow);

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

        public static IEnumerable<RefreshToken> FindExpiredForUser(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return null;
            }

            try
            {
                using (var context = DbInterface.CreatePlayerContext())
                {
                    var tokenQuery = context?.RefreshTokens.Where(queryToken => queryToken.UserId == userId && queryToken.Expires < DateTime.UtcNow);

                    return tokenQuery.AsEnumerable()?.ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        public static IEnumerable<RefreshToken> FindExpiredForUser(User user)
        {
            return FindExpiredForUser(user.Id);
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

        public static bool HasTokens(Guid userId)
        {
            try
            {
                using (var context = DbInterface.CreatePlayerContext())
                {
                    return context.RefreshTokens.Any(queryToken => queryToken.UserId == userId);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                throw;
            }
        }

        public static bool HasTokens(User user) => HasTokens(user.Id);

        public static bool Remove(Guid id) => TryFind(id, out var token) && Remove(token);

        public static bool Remove(RefreshToken token) => RemoveAll(new []{ token });

        public static bool RemoveAll(IEnumerable<RefreshToken> tokens) => RemoveAllAsync(tokens.ToList(), default).Result;

        public static async Task<bool> RemoveAllAsync(IList<RefreshToken> tokens, CancellationToken cancellationToken)
        {
            try
            {
                var unblockedTokens = tokens.Where(token => _pendingChanges.TryAdd(token.Id, EntityState.Deleted)).ToArray();

                if (unblockedTokens.Length < 1)
                {
                    return false;
                }

                Log.Diagnostic($"Attempted to remove {tokens.Count} tokens but only {unblockedTokens.Length} were available to remove.");

                using (var context = DbInterface.CreatePlayerContext(readOnly: false))
                {
                    context.RefreshTokens.RemoveRange(unblockedTokens);
                    context.DetachExcept(unblockedTokens);
                    _ = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }

                foreach (var token in unblockedTokens)
                {
                    _ = _pendingChanges.TryRemove(token.Id, out _);
                }

                return true;
            }
            catch (DbUpdateConcurrencyException concurrencyException)
            {
                concurrencyException.LogError();
                return false;
            }
        }

        public static async Task<bool> RemoveForUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            using (var context = DbInterface.CreatePlayerContext(readOnly: false))
            {
                try
                {
                    context.RefreshTokens.RemoveRange(context.RefreshTokens.Where(token => token.UserId == userId));
                    context.DetachExcept<RefreshToken>(entity => entity.UserId == userId);
                    _ = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    return true;
                }
                catch (DbUpdateConcurrencyException concurrencyException)
                {
                    concurrencyException.LogError();
                    return false;
                }
            }
        }

        public static async Task<int> RemoveExpiredAsync(int pruneCount, CancellationToken cancellationToken = default)
        {
            using (var context = DbInterface.CreatePlayerContext(readOnly: false))
            {
                var remaining = 0;
                try
                {
                    var tokensToRemove = context.RefreshTokens.Where(queryToken => queryToken.Expires < DateTime.UtcNow).Take(pruneCount).ToArray();
                    var unblockedTokens = tokensToRemove.Where(token => _pendingChanges.TryAdd(token.Id, EntityState.Deleted)).ToArray();
                    context.RefreshTokens.RemoveRange(unblockedTokens);
                    context.DetachExcept(unblockedTokens);
                    _ = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    foreach (var token in unblockedTokens)
                    {
                        _ = _pendingChanges.TryRemove(token.Id, out _);
                    }

                    remaining = tokensToRemove.Length - unblockedTokens.Length;
                }
                catch (DbUpdateConcurrencyException concurrencyException)
                {
                    concurrencyException.LogError();
                }

                if (remaining == 0)
                {
                    remaining = context.RefreshTokens.Where(queryToken => queryToken.Expires < DateTime.UtcNow).Any() ? 1 : 0;
                }

                return remaining;
            }
        }
    }
}
