using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Intersect.Server.Models;
using Intersect.Server.WebApi.Authentication;
using JetBrains.Annotations;
using Jose;

namespace Intersect.Server.Database
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

            return mSessions.TryGetValue(token, out User user) ? new ClaimsPrincipal(new UserIdentity(user)) : null;
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

            if (!Classes.Core.Database.CheckPassword(username, password))
                throw new ArgumentException("Invalid credentials.");

            var power = Classes.Core.Database.CheckPower(username);
            var passwordHashBytes = BitConverter.GetBytes(password.GetHashCode());
            var user = new User
            {
                Name = username,
                Password = password,
                Power = power,
                Guid = new Guid(username.GetHashCode(), BitConverter.ToInt16(passwordHashBytes, 0), BitConverter.ToInt16(passwordHashBytes, 2), BitConverter.GetBytes(power))
            };

            var token = new JwtToken
            {
                Data = $"{user.Guid}_{user.Name}",
                Expiration = DateTime.UtcNow.AddMinutes(1).ToBinary()
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
