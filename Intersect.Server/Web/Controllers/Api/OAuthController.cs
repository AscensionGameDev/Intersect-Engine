using System.Diagnostics;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Intersect.Server.Collections.Indexing;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Api;
using Intersect.Server.Web.Authentication;
using Intersect.Server.Web.Http;
using Intersect.Server.Web.Types;
using Intersect.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace Intersect.Server.Web.Controllers.Api
{
    [Route("api/oauth")]
    // [ConfigurableAuthorize]
    public sealed partial class OAuthController : IntersectController
    {
        private readonly IntersectAuthenticationManager _authenticationManager;

        public OAuthController(IntersectAuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
        }

        private class UsernameAndTokenResponse
        {
            [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public Guid? TokenId { get; set; } = default;

            [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public string? Username { get; set; } = default;
        }

        private class TokenResponse
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }

            [JsonProperty("token_type")]
            public string TokenType { get; set; } = TokenTypes.Bearer;

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
        [EndpointSummary($"{nameof(OAuthController)}_{nameof(RequestToken)}_Summary")]
        [EndpointDescription($"{nameof(OAuthController)}_{nameof(RequestToken)}_Description")]
        [Consumes(typeof(TokenRequest), ContentTypes.Json)]
        [ProducesResponseType<TokenResponse>((int)HttpStatusCode.OK, ContentTypes.Json)]
        [ProducesResponseType<StatusMessageResponseBody>((int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType<StatusMessageResponseBody>((int)HttpStatusCode.InternalServerError, ContentTypes.Json)]
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
            if (!Database.PlayerData.User.TryFindByName(passwordGrant.Username, out var user) ||
                !user.IsPasswordValid(passwordGrant.Password))
            {
                // TODO(i18n): Localized by locale specified in header
                return BadRequest("Invalid credentials");
            }

            var tokenResponse = await IssueTokenFor(user);
            return tokenResponse;
        }

        private async Task<IActionResult> RequestTokenFrom(TokenRequestRefreshTokenGrant refreshTokenGrant)
        {
            if (!Guid.TryParse(refreshTokenGrant.RefreshToken, out var refreshTokenId) ||
                !RefreshToken.TryFind(refreshTokenId, out var refreshToken) || refreshToken?.User == default)
            {
                // TODO(i18n): Localized by locale specified in header
                return BadRequest("Invalid token");
            }

            var tokenResponse = await IssueTokenFor(refreshToken.User);
            return tokenResponse;
        }

        private async Task<IActionResult> IssueTokenFor(User user)
        {
            var authenticationResult = await _authenticationManager.TryAuthenticate(user);
            switch (authenticationResult.Type)
            {
                case AuthenticationResultType.Unknown:
                    return InternalServerError();
                case AuthenticationResultType.ErrorOccurred:
                    return InternalServerError();
                case AuthenticationResultType.Expired:
                    return Forbidden();
                case AuthenticationResultType.Unauthorized:
                    return Unauthorized();
                case AuthenticationResultType.Success:
                    break;
                default:
                    throw new UnreachableException();
            }

            var claimsIdentity = authenticationResult.Identity;
            var refreshToken = authenticationResult.RefreshToken;
            if (claimsIdentity == default || refreshToken == default)
            {
                return InternalServerError();
            }

            TokenResponse tokenResponse = new()
            {
                AccessToken = refreshToken.Ticket,
                RefreshToken = refreshToken.Id.ToString(),
                Expires = authenticationResult.ExpiresAt,
                Issued = authenticationResult.IssuedAt,
                TokenType = "bearer",
            };
            return Ok(tokenResponse);
        }

        [Authorize]
        [HttpDelete("tokens/{tokenId:guid}")]
        [EndpointSummary($"{nameof(OAuthController)}_{nameof(DeleteTokenById)}_Summary")]
        [EndpointDescription($"{nameof(OAuthController)}_{nameof(DeleteTokenById)}_Description")]
        [ProducesResponseType<UsernameAndTokenResponse>((int)HttpStatusCode.OK, ContentTypes.Json)]
        [ProducesResponseType<StatusMessageResponseBody>((int)HttpStatusCode.Forbidden, ContentTypes.Json)]
        [ProducesResponseType<StatusMessageResponseBody>((int)HttpStatusCode.InternalServerError, ContentTypes.Json)]
        [ProducesResponseType<StatusMessageResponseBody>((int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType<StatusMessageResponseBody>((int)HttpStatusCode.Unauthorized, ContentTypes.Json)]
        public async Task<IActionResult> DeleteTokenById(Guid tokenId)
        {
            var actor = IntersectUser;
            if (actor == default)
            {
                // TODO(i18n): Localized by locale specified in header
                return Unauthorized("Request is not authorized");
            }

            if (!RefreshToken.TryFind(tokenId, out var refreshToken))
            {
                // TODO(i18n): Localized by locale specified in header
                return NotFound("Token not found");
            }

            if (refreshToken.Id != tokenId)
            {
                // TODO(i18n): Localized by locale specified in header
                return InternalServerError("Failed to delete token");
            }

            if (refreshToken.UserId != actor.Id && !(actor.Power.ApiRoles?.UserManage ?? false))
            {
                // TODO(i18n): Localized by locale specified in header
                return Forbidden("No authorization to perform this action");
            }

            if (RefreshToken.Remove(refreshToken))
            {
                return Ok(
                    new UsernameAndTokenResponse
                    {
                        TokenId = tokenId,
                    }
                );
            }

            // TODO(i18n): Localized by locale specified in header
            return InternalServerError("Failed to delete token");
        }

        [Authorize]
        [HttpDelete("tokens/{lookupKey:LookupKey}")]
        [EndpointSummary($"{nameof(OAuthController)}_{nameof(DeleteTokensForUser)}_Summary")]
        [EndpointDescription($"{nameof(OAuthController)}_{nameof(DeleteTokensForUser)}_Description")]
        [ProducesResponseType<UsernameAndTokenResponse>((int)HttpStatusCode.OK, ContentTypes.Json)]
        [ProducesResponseType<StatusMessageResponseBody>((int)HttpStatusCode.Forbidden, ContentTypes.Json)]
        [ProducesResponseType<StatusMessageResponseBody>((int)HttpStatusCode.Gone, ContentTypes.Json)]
        [ProducesResponseType<StatusMessageResponseBody>((int)HttpStatusCode.InternalServerError, ContentTypes.Json)]
        [ProducesResponseType<StatusMessageResponseBody>((int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType<StatusMessageResponseBody>((int)HttpStatusCode.Unauthorized, ContentTypes.Json)]
        public async Task<IActionResult> DeleteTokensForUser(LookupKey lookupKey, CancellationToken cancellationToken)
        {
            var actor = IntersectUser;
            if (actor == default)
            {
                // TODO(i18n): Localized by locale specified in header
                return Unauthorized("Request is not authorized");
            }

            if (!Database.PlayerData.User.TryFetch(lookupKey, out var user))
            {
                // ReSharper disable once InvertIf
                if (!lookupKey.Matches(actor.Id, actor.Name))
                {
                    if (!(actor.Power.ApiRoles?.UserManage ?? false))
                    {
                        // TODO(i18n): Localized by locale specified in header
                        return Forbidden("No authorization to perform this action");
                    }
                }

                // TODO(i18n): Localized by locale specified in header
                return NotFound("The user was not found");
            }

            if (actor.Id != user.Id && !(actor.Power.ApiRoles?.UserManage ?? false))
            {
                // TODO(i18n): Localized by locale specified in header
                return Forbidden("No authorization to perform this action");
            }

            if (!RefreshToken.HasTokens(user))
            {
                // TODO(i18n): Localized by locale specified in header
                return actor.Id != user.Id
                    ? NotFound("There are no tokens for the specified user")
                    : Gone("There are no tokens for the specified user");
            }

            if (await RefreshToken.RemoveForUserAsync(user.Id, cancellationToken).ConfigureAwait(false))
            {
                return Ok(
                    new UsernameAndTokenResponse
                    {
                        Username = user.Name,
                    }
                );
            }

            // TODO(i18n): Localized by locale specified in header
            return InternalServerError("Failed to delete token");
        }

        [Authorize]
        [HttpDelete("tokens/{lookupKey:LookupKey}/{tokenId:guid}")]
        [EndpointSummary($"{nameof(OAuthController)}_{nameof(DeleteTokenByIdForUser)}_Summary")]
        [EndpointDescription($"{nameof(OAuthController)}_{nameof(DeleteTokenByIdForUser)}_Description")]
        [ProducesResponseType<UsernameAndTokenResponse>((int)HttpStatusCode.OK, ContentTypes.Json)]
        [ProducesResponseType<StatusMessageResponseBody>((int)HttpStatusCode.Forbidden, ContentTypes.Json)]
        [ProducesResponseType<StatusMessageResponseBody>((int)HttpStatusCode.InternalServerError, ContentTypes.Json)]
        [ProducesResponseType<StatusMessageResponseBody>((int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType<StatusMessageResponseBody>((int)HttpStatusCode.Unauthorized, ContentTypes.Json)]
        public async Task<IActionResult> DeleteTokenByIdForUser(LookupKey lookupKey, Guid tokenId)
        {
            var actor = IntersectUser;
            if (actor == default)
            {
                // TODO(i18n): Localized by locale specified in header
                return Unauthorized("Request is not authorized");
            }

            if (!Database.PlayerData.User.TryFetch(lookupKey, out var user))
            {
                // ReSharper disable once InvertIf
                if (!lookupKey.Matches(actor.Id, actor.Name))
                {
                    if (!(actor.Power.ApiRoles?.UserManage ?? false))
                    {
                        // TODO(i18n): Localized by locale specified in header
                        return Forbidden("No authorization to perform this action");
                    }
                }

                // TODO(i18n): Localized by locale specified in header
                return NotFound("The user was not found");
            }

            if (actor.Id != user.Id && !(actor.Power.ApiRoles?.UserManage ?? false))
            {
                // TODO(i18n): Localized by locale specified in header
                return Forbidden("No authorization to perform this action");
            }

            if (!RefreshToken.TryFind(tokenId, out _))
            {
                // TODO(i18n): Localized by locale specified in header
                return NotFound("Token not found");
            }

            if (RefreshToken.Remove(tokenId))
            {
                return Ok(
                    new UsernameAndTokenResponse
                    {
                        TokenId = tokenId,
                        Username = user.Name,
                    }
                );
            }

            // TODO(i18n): Localized by locale specified in header
            return InternalServerError("Failed to delete token");
        }
    }
}
