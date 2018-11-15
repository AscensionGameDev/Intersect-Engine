using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using Intersect.Server.Database.PlayerData;
using JetBrains.Annotations;
using Jose;

namespace Intersect.Server.WebApi.Authentication
{
    public sealed class AuthorizationProvider : IAuthorizationProvider
    {
        [NotNull]
        private RSACryptoServiceProvider RsaPrivate { get; }

        [NotNull]
        private RSACryptoServiceProvider RsaPublic { get; }

        [NotNull]
        private readonly IDictionary<JwtToken, User> mSessions;

        public AuthorizationProvider()
        {
            mSessions = new Dictionary<JwtToken, User>();

            RsaPrivate = new RSACryptoServiceProvider(4096);
            RsaPublic = new RSACryptoServiceProvider();
            RsaPublic.ImportParameters(RsaPrivate.ExportParameters(false));
        }

        public ClaimsPrincipal FindUserFrom(JwtToken token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            var claimsPrincipal = mSessions.TryGetValue(token, out User user) ? new ClaimsPrincipal(new UserIdentity(user)) : null;
            return claimsPrincipal;
        }

        public bool Expire(JwtToken token)
        {
            return token != null && mSessions.Remove(token);
        }

        public JwtToken Authorize(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));

            if (!LegacyDatabase.CheckPassword(username, password))
                throw new ArgumentException("Invalid credentials.");

            var user = LegacyDatabase.GetUser(username);

            if (!user.Power.Api)
                throw new ArgumentException("API access not permitted for user!");

            var token = new JwtToken
            {
                Data = $"{user.Id}_{user.Name}",
                Expiration = DateTime.UtcNow.AddMinutes(5).ToBinary()
            };

            mSessions[token] = user;
            return token;
        }

        public JwtToken Decode(string authorizationHeader)
        {
            return string.IsNullOrWhiteSpace(authorizationHeader) ? null : JWT.Decode<JwtToken>(authorizationHeader, RsaPrivate);
        }

        public string Encode(JwtToken token)
        {
            return token == null ? null : JWT.Encode(token, RsaPublic, JweAlgorithm.RSA_OAEP_256, JweEncryption.A256GCM);
        }
    }
}
