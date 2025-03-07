using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Intersect.Core;
using Intersect.Framework.Net;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Intersect.Web;

public partial class IntersectHttpClient : HttpClient
{
    private static HttpClientHandler GetHttpClientHandler()
    {
        return new()
        {
            ServerCertificateCustomValidationCallback = (request, certificate, chain, policyErrors) =>
            {
                var host = request.RequestUri?.Host;
                if (string.IsNullOrWhiteSpace(host))
                {
                    return false;
                }

                if (IPAddress.TryParse(host, out var address))
                {
                    return address.IsLoopback() || address.IsPrivate();
                }

                return string.Equals("localhost", host, StringComparison.Ordinal) || (certificate?.Verify() ?? false);
            },
        };
    }

    private readonly Uri _baseUri;
    private readonly TokenResponse? _tokenResponse;

    public IntersectHttpClient(string serverUri, TokenResponse? tokenResponse = default) : base(GetHttpClientHandler())
    {
#if DEBUG
        Timeout = TimeSpan.FromSeconds(15);
#endif

        _baseUri = new Uri(new Uri(serverUri), "/");
        _tokenResponse = tokenResponse;
    }

    public TokenResultType TryRequestToken(string username, string password, out TokenResponse? tokenResponse, bool hashed = false)
    {
        tokenResponse = default;

        if (string.IsNullOrWhiteSpace(username))
        {
            return TokenResultType.InvalidUsername;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return TokenResultType.InvalidPassword;
        }

        var hashword = hashed ? password : Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
        if (string.IsNullOrWhiteSpace(hashword))
        {
            return TokenResultType.ClientSideFailure;
        }

        try
        {
            using HttpRequestMessage requestMessage = new(HttpMethod.Post, new Uri(_baseUri, "/api/oauth/token"));
            Dictionary<string, string> objectTokenRequestBody = new()
            {
                {"grant_type", "password" },
                {"username", username},
                {"password", hashword},
            };
            var tokenRequestBody = JsonConvert.SerializeObject(objectTokenRequestBody);
            requestMessage.Content = new StringContent(tokenRequestBody, MediaTypeHeaderValue.Parse("application/json"));

            var responseMessage = Send(requestMessage);
            if (responseMessage.StatusCode != HttpStatusCode.OK)
            {
                ApplicationContext.Context.Value?.Logger.LogError(
                    "Authentication failed, received {StatusCode} from server",
                    responseMessage.StatusCode
                );
                return TokenResultType.InvalidCredentials;
            }

            try
            {
                if (responseMessage.Content.Headers.ContentType?.MediaType != "application/json")
                {
                    return TokenResultType.InvalidResponse;
                }


                var contentLength = responseMessage.Content.Headers.ContentLength;
                if (contentLength.HasValue && contentLength.Value < 3)
                {
                    return TokenResultType.InvalidResponse;
                }

                using var responseStream = responseMessage.Content.ReadAsStream();
                using StreamReader responseStreamReader = new(responseStream);

                var rawResponse = responseStreamReader.ReadToEnd();
                if (string.IsNullOrWhiteSpace(rawResponse))
                {
                    return TokenResultType.InvalidResponse;
                }

                tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(rawResponse);
                return TokenResultType.TokenReceived;
            }
            catch (Exception exception)
            {
                ApplicationContext.Context.Value?.Logger.LogError(exception, "Failed to parse token response");
                return TokenResultType.InvalidResponse;
            }
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogError(exception, "Failed to authenticate");
            return TokenResultType.Failed;
        }
    }

    public TokenResultType TryRequestToken(string refreshToken, out TokenResponse? tokenResponse)
    {
        tokenResponse = default;

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return TokenResultType.InvalidRefreshToken;
        }

        try
        {
            using HttpRequestMessage requestMessage = new(HttpMethod.Post, new Uri(_baseUri, "/api/oauth/token"));
            Dictionary<string, string> objectTokenRequestBody = new()
            {
                {"grant_type", "refresh_token" },
                {"refresh_token", refreshToken},
            };
            var tokenRequestBody = JsonConvert.SerializeObject(objectTokenRequestBody);
            requestMessage.Content = new StringContent(tokenRequestBody, MediaTypeHeaderValue.Parse("application/json"));

            var responseMessage = Send(requestMessage);
            if (responseMessage.StatusCode != HttpStatusCode.OK)
            {
                ApplicationContext.Context.Value?.Logger.LogError(
                    "Authentication failed, received {StatusCode} from server",
                    responseMessage.StatusCode
                );
                return TokenResultType.InvalidCredentials;
            }

            try
            {
                if (responseMessage.Content.Headers.ContentType?.MediaType != "application/json")
                {
                    return TokenResultType.InvalidResponse;
                }


                var contentLength = responseMessage.Content.Headers.ContentLength;
                if (contentLength.HasValue && contentLength.Value < 3)
                {
                    return TokenResultType.InvalidResponse;
                }

                using var responseStream = responseMessage.Content.ReadAsStream();
                using StreamReader responseStreamReader = new(responseStream);

                var rawResponse = responseStreamReader.ReadToEnd();
                if (string.IsNullOrWhiteSpace(rawResponse))
                {
                    return TokenResultType.InvalidResponse;
                }

                tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(rawResponse);
                return TokenResultType.TokenReceived;
            }
            catch (Exception exception)
            {
                ApplicationContext.Context.Value?.Logger.LogError(exception, "Failed to parse token response");
                return TokenResultType.InvalidResponse;
            }
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogError(exception, "Failed to authenticate");
            return TokenResultType.Failed;
        }
    }

    public new HttpResponseMessage Send(HttpRequestMessage request) => Send(request, cancellationToken: default);

    public override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_tokenResponse is { } tokenResponse)
        {
            if (request.Headers.Authorization == default)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
            }
        }
        else
        {
            tokenResponse = default;
        }

        var responseMessage = base.Send(request, cancellationToken);

        if (responseMessage.IsSuccessStatusCode || tokenResponse == default)
        {
            return responseMessage;
        }

        if (TryRequestToken(tokenResponse.RefreshToken, out tokenResponse) != TokenResultType.TokenReceived)
        {
            return responseMessage;
        }

        using HttpRequestMessage newRequest = new(request.Method, request.RequestUri)
        {
            Version = request.Version,
        };

        if (request.Content != default)
        {
            MemoryStream memoryStream = new();
            request.Content.CopyTo(memoryStream, default, cancellationToken);
            memoryStream.Position = 0;
            newRequest.Content = new StreamContent(memoryStream);
            foreach (var header in request.Content.Headers)
            {
                newRequest.Content.Headers.Add(header.Key, header.Value);
            }
        }

        foreach (var option in request.Options)
        {
            newRequest.Options.Set(new(option.Key), option.Value);
        }

        foreach (var header in request.Headers)
        {
            newRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return base.Send(newRequest, cancellationToken);
    }

    public new HttpResponseMessage Send(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
    {
        if (_tokenResponse is { } tokenResponse)
        {
            if (request.Headers.Authorization == default)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
            }
        }
        else
        {
            tokenResponse = default;
        }

        var responseMessage = base.Send(request, completionOption, cancellationToken);

        if (responseMessage.IsSuccessStatusCode || tokenResponse == default)
        {
            return responseMessage;
        }

        if (TryRequestToken(tokenResponse.RefreshToken, out tokenResponse) != TokenResultType.TokenReceived)
        {
            return responseMessage;
        }

        using HttpRequestMessage newRequest = new(request.Method, request.RequestUri)
        {
            Version = request.Version,
        };

        if (request.Content != default)
        {
            MemoryStream memoryStream = new();
            request.Content.CopyTo(memoryStream, default, cancellationToken);
            memoryStream.Position = 0;
            newRequest.Content = new StreamContent(memoryStream);
            foreach (var header in request.Content.Headers)
            {
                newRequest.Content.Headers.Add(header.Key, header.Value);
            }
        }

        foreach (var option in request.Options)
        {
            newRequest.Options.Set(new(option.Key), option.Value);
        }

        foreach (var header in request.Headers)
        {
            newRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return base.Send(newRequest, completionOption, cancellationToken);
    }
}
