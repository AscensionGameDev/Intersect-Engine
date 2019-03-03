using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

using Intersect.Server.Database.PlayerData;

using JetBrains.Annotations;

namespace Intersect.Server.Classes.Database.PlayerData.Api
{

    public class RefreshToken
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 0)]
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public Guid ClientId { get; set; }

        [Required]
        public string Subject { get; set; }

        public DateTime Issued { get; set; }

        public DateTime Expires { get; set; }

        [Required]
        public string Ticket { get; set; }

        public static async ValueTask<bool> Add(
            [NotNull] RefreshToken token,
            bool commit = false,
            bool checkForDuplicates = true
        )
        {
            if (PlayerContext.Current?.RefreshTokens == null)
            {
                return false;
            }

            if (checkForDuplicates)
            {
                var duplicate = await Find(token.Id);
                if (duplicate != null && !await Remove(token.Id, commit))
                {
                    return false;
                }

                var forClient = await FindForClient(token.ClientId);
                if (forClient != null && !await RemoveAll(forClient, commit))
                {
                    return false;
                }

                var forUser = await FindForUser(token.UserId);
                if (forUser != null && !await RemoveAll(forUser, commit))
                {
                    return false;
                }
            }

            PlayerContext.Current.RefreshTokens.Add(token);
            await PlayerContext.Current.Commit(commit);

            return true;
        }

        public static async ValueTask<RefreshToken> Find(Guid id)
        {
            if (PlayerContext.Current?.RefreshTokens == null)
            {
                return null;
            }

            var task = PlayerContext.Current.RefreshTokens.FindAsync(id);

            return task == null ? null : await task;
        }

        public static ValueTask<IQueryable<RefreshToken>> FindForClient(Guid clientId)
        {
            var tokenQuery = PlayerContext.Current?.RefreshTokens?.Where(queryToken => queryToken.ClientId == clientId);

            return new ValueTask<IQueryable<RefreshToken>>(tokenQuery);
        }

        public static ValueTask<IQueryable<RefreshToken>> FindForUser(Guid userId)
        {
            var tokenQuery = PlayerContext.Current?.RefreshTokens?.Where(queryToken => queryToken.UserId == userId);

            return new ValueTask<IQueryable<RefreshToken>>(tokenQuery);
        }

        public static async ValueTask<IQueryable<RefreshToken>> FindForUser([NotNull] User user)
        {
            if (PlayerContext.Current?.RefreshTokens == null)
            {
                return null;
            }

            return await FindForUser(user.Id);
        }

        public static ValueTask<RefreshToken> FindOneForUser(Guid userId)
        {
            var token = PlayerContext.Current?.RefreshTokens?.First(queryToken => queryToken.UserId == userId);

            return new ValueTask<RefreshToken>(token);
        }

        public static async ValueTask<RefreshToken> FindOneForUser([NotNull] User user)
        {
            if (PlayerContext.Current?.RefreshTokens == null)
            {
                return null;
            }

            return await FindOneForUser(user.Id);
        }

        public static async ValueTask<bool> Remove(Guid id, bool commit = false)
        {
            if (PlayerContext.Current?.RefreshTokens == null)
            {
                return false;
            }

            var token = await Find(id);

            if (token == null)
            {
                return false;
            }

            return await Remove(token, commit);
        }

        public static async ValueTask<bool> Remove([NotNull] RefreshToken token, bool commit = false)
        {
            if (PlayerContext.Current?.RefreshTokens == null)
            {
                return false;
            }

            PlayerContext.Current.RefreshTokens.Remove(token);
            await PlayerContext.Current.Commit(commit);

            return true;
        }

        public static async ValueTask<bool> RemoveAll([NotNull] IEnumerable<RefreshToken> tokens, bool commit = false)
        {
            if (PlayerContext.Current?.RefreshTokens == null)
            {
                return false;
            }

            PlayerContext.Current.RefreshTokens.RemoveRange(tokens);
            await PlayerContext.Current.Commit(commit);

            return true;
        }

    }

}
