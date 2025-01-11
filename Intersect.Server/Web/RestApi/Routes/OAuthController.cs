using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Intersect.Server.Database.PlayerData.Api;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Intersect.Security.Claims;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Web.Configuration;
using Intersect.Server.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace Intersect.Server.Web.RestApi.Routes
{
    [Route("api/oauth")]
    // [ConfigurableAuthorize]
    public sealed partial class OAuthController : IntersectController
    {
        private readonly IOptions<TokenGenerationOptions> _tokenGenerationOptions;

        public OAuthController(IOptions<TokenGenerationOptions> tokenGenerationOptions)
        {
            _tokenGenerationOptions = tokenGenerationOptions;
        }

        private class UsernameAndTokenResponse
        {
            [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public Guid TokenId { get; set; } = default;

            [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public string Username { get; set; } = default;
        }

        private class TokenResponse
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }

            [JsonProperty("token_type")]
            public string TokenType { get; set; } = "bearer";

            [JsonProperty("expires_in")]
            public int ExpiresIn => (int)(Expires - DateTime.UtcNow).TotalMinutes;

            [JsonProperty(".issued")]
            public DateTime Issued { get; set; } = DateTime.UtcNow;

            [JsonProperty(".expires")]
            public DateTime Expires { get; set; }
        }

        public partial class GrantTypeConverter : Newtonsoft.Json.JsonConverter<GrantType>
        {
            public override void WriteJson(JsonWriter writer, GrantType value, JsonSerializer serializer)
            {
                var enumString = value.ToString();
                enumString = TitleCaseCharacterPattern().Replace(
                    enumString,
                    match => (match.Index == 0 ? string.Empty : "_") + match.Value.ToLowerInvariant()
                );
                writer.WriteValue(enumString);
            }

            public override GrantType ReadJson(
                JsonReader reader,
                Type objectType,
                GrantType existingValue,
                bool hasExistingValue,
                JsonSerializer serializer
            )
            {
                if (reader.TokenType != JsonToken.String)
                {
                    return default;
                }

                if (reader.Value is not string enumString)
                {
                    return default;
                }

                enumString = SnakeCaseCharacterPattern().Replace(enumString, match => match.Value.Last().ToString().ToUpperInvariant());
                return Enum.TryParse<GrantType>(enumString, out var grantType) ? grantType : default;
            }

            [GeneratedRegex("[A-Z]")]
            private static partial Regex TitleCaseCharacterPattern();
            [GeneratedRegex("(?:^|_)[a-z]")]
            private static partial Regex SnakeCaseCharacterPattern();
        }

        [Newtonsoft.Json.JsonConverter(typeof(GrantTypeConverter))]
        [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter<GrantType>))]
        public enum GrantType
        {
            Password,
            RefreshToken,
        }

        public sealed class TokenRequestConverter : Newtonsoft.Json.JsonConverter<TokenRequest>
        {
            public override bool CanWrite => false;

            public override void WriteJson(JsonWriter writer, TokenRequest value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override TokenRequest ReadJson(
                JsonReader reader,
                Type objectType,
                TokenRequest existingValue,
                bool hasExistingValue,
                JsonSerializer serializer
            )
            {
                if (reader.TokenType == JsonToken.Null)
                {
                    return default;
                }

                var @object = JObject.Load(reader);
                if (!@object.TryGetValue("grant_type", out var grantTypeToken))
                {
                    return default;
                }

                var grantType = grantTypeToken.ToObject<GrantType>();
                TokenRequest tokenRequest = grantType switch
                {
                    GrantType.Password => JsonConvert.DeserializeObject<TokenRequestPasswordGrant>(@object.ToString(), SerializerSettings),
                    GrantType.RefreshToken => JsonConvert.DeserializeObject<TokenRequestRefreshTokenGrant>(@object.ToString(), SerializerSettings),
                    _ => default,
                };
                return tokenRequest;
            }

            private static readonly JsonSerializerSettings SerializerSettings = new()
            {
                ContractResolver = new BaseSpecifiedConcreteClassConverter(),
            };
        }

        public class BaseSpecifiedConcreteClassConverter : DefaultContractResolver
        {
            protected override JsonConverter ResolveContractConverter(Type objectType)
            {
                if (typeof(TokenRequest).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                {
                    return default;
                }
                return base.ResolveContractConverter(objectType);
            }
        }

        [SwaggerDiscriminator("grant_type")]
        [SwaggerSubType(typeof(TokenRequestPasswordGrant), DiscriminatorValue = "password")]
        [SwaggerSubType(typeof(TokenRequestRefreshTokenGrant), DiscriminatorValue = "refresh_token")]
        [Newtonsoft.Json.JsonConverter(typeof(TokenRequestConverter))]
        public abstract class TokenRequest
        {
            [JsonProperty("grant_type")]
            public abstract GrantType GrantType { get; }
        }

        public class TokenRequestPasswordGrant : TokenRequest
        {
            public override GrantType GrantType => GrantType.Password;

            [JsonProperty("username")]
            public string Username { get; set; }

            [JsonProperty("password")]
            public string Password { get; set; }
        }

        public class TokenRequestRefreshTokenGrant : TokenRequest
        {
            public override GrantType GrantType => GrantType.RefreshToken;

            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }
        }

        [HttpPost("token")]
        [Consumes(typeof(TokenRequest), ContentTypes.Json)]
        public async Task<IActionResult> RequestToken([FromBody] TokenRequest tokenRequest)
        {
            return tokenRequest switch
            {
                TokenRequestPasswordGrant passwordGrant => await RequestTokenFrom(passwordGrant),
                TokenRequestRefreshTokenGrant refreshTokenGrant => await RequestTokenFrom(refreshTokenGrant),
                _ => BadRequest(),
            };
        }

        private async Task<IActionResult> RequestTokenFrom(TokenRequestPasswordGrant passwordGrant)
        {
            var user = Database.PlayerData.User.Find(passwordGrant.Username);
            if (!user.IsPasswordValid(passwordGrant.Password))
            {
                return BadRequest();
            }

            var tokenResponse = await IssueTokenFor(user);
            return Ok(tokenResponse);
        }

        private async Task<IActionResult> RequestTokenFrom(TokenRequestRefreshTokenGrant refreshTokenGrant)
        {
            var refreshTokenId = Guid.TryParse(refreshTokenGrant.RefreshToken, out var parsedId) ? parsedId : default;
            if (!RefreshToken.TryFind(refreshTokenId, out var refreshToken) || refreshToken?.User == default)
            {
                return BadRequest();
            }

            var tokenResponse = await IssueTokenFor(refreshToken.User);
            return Ok(tokenResponse);
        }

        private async Task<TokenResponse> IssueTokenFor(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var ticketId = Guid.NewGuid();
            var clientId = Guid.Empty;
            var claims = user.Claims.ToList();
            claims.Add(new Claim(IntersectClaimTypes.ClientId, clientId.ToString()));
            claims.Add(new Claim(IntersectClaimTypes.TicketId, ticketId.ToString()));
            foreach (var role in user.Power.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _tokenGenerationOptions.Value.Audience,
                Issuer = _tokenGenerationOptions.Value.Issuer,
                Subject = new ClaimsIdentity(claims.ToArray()),
                Expires = DateTime.UtcNow.AddMinutes(_tokenGenerationOptions.Value.AccessTokenLifetime),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(_tokenGenerationOptions.Value.SecretData),
                    SecurityAlgorithms.HmacSha512Signature
                ),
            };
            var accessToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializedAccessToken = tokenHandler.WriteToken(accessToken);
            var issued = DateTime.UtcNow;
            var expires = issued.AddMinutes(_tokenGenerationOptions.Value.RefreshTokenLifetime);
            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                ClientId = clientId,
                Subject = user.Name,
                Issued = issued,
                Expires = expires,
                TicketId = ticketId,
                Ticket = serializedAccessToken,
            };
            await RefreshToken.TryAddAsync(refreshToken);
            return new TokenResponse
            {
                AccessToken = serializedAccessToken,
                RefreshToken = refreshToken.Id.ToString(),
                Expires = expires,
                Issued = issued,
                TokenType = "bearer",
            };
        }

        [HttpDelete("tokens/{tokenId:guid}")]
        public async Task<IActionResult> DeleteTokenById(Guid tokenId)
        {
            var actor = IntersectUser;
            if (actor == default)
            {
                return Unauthorized();
            }

            if (!RefreshToken.TryFind(tokenId, out var refreshToken))
            {
                return Unauthorized();
            }

            if (refreshToken.Id != tokenId)
            {
                return InternalServerError();
            }

            if (refreshToken.UserId != actor.Id && !actor.Power.ApiRoles.UserManage)
            {
                return Unauthorized();
            }

            return RefreshToken.Remove(refreshToken) ? Ok(new UsernameAndTokenResponse { TokenId = tokenId }) : InternalServerError();
        }

        [Authorize]
        [HttpDelete("tokens/{username}")]
        public async Task<IActionResult> DeleteTokensForUsername(string username, CancellationToken cancellationToken)
        {
            var actor = IntersectUser;
            if (actor == default)
            {
                return Unauthorized();
            }

            var user = Database.PlayerData.User.Find(username);

            if (user == null)
            {
                return Unauthorized();
            }

            if (!actor.Power.ApiRoles.UserManage && actor.Id != user.Id)
            {
                return Unauthorized();
            }

            if (!RefreshToken.HasTokens(user))
            {
                return Gone();
            }

            var success = await RefreshToken.RemoveForUserAsync(user.Id, cancellationToken).ConfigureAwait(false);
            return success ? Ok(new { Username = username }) : Unauthorized();
        }

        [HttpDelete("tokens/{username}/{tokenId:guid}")]
        public async Task<IActionResult> DeleteTokenForUsernameById(string username, Guid tokenId)
        {
            var intersectUser = IntersectUser;
            if (intersectUser == default)
            {
                return Unauthorized();
            }

            var user = Database.PlayerData.User.Find(username);

            if (user == null)
            {
                return Unauthorized();
            }

            if (intersectUser.Id != user.Id && !intersectUser.Power.ApiRoles.UserManage)
            {
                return Unauthorized();
            }

            if (!RefreshToken.TryFind(tokenId, out _))
            {
                return Gone();
            }

            return !RefreshToken.Remove(tokenId) ? InternalServerError() : Ok(new UsernameAndTokenResponse { TokenId = tokenId, Username = username });
        }
    }
}
