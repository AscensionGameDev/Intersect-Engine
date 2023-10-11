using System.Net;
using System.Reflection;
using System.Threading.RateLimiting;
using Intersect.Core;
using Intersect.Enums;
using Intersect.Logging;
using Intersect.Server.Core;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Web.Authentication;
using Intersect.Server.Web.Configuration;
using Intersect.Server.Web.Constraints;
using Intersect.Server.Web.Middleware;
using Intersect.Server.Web.RestApi.Payloads;
using Intersect.Server.Web.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Intersect.Server.Web;

internal partial class ApiService : ApplicationService<ServerContext, IApiService, ApiService>, IApiService
{
    private WebApplication? _app;
    private static readonly Assembly Assembly = typeof(ApiService).Assembly;

    private WebApplication? Configure()
    {
        UnpackAppSettings();

        ValidateConfiguration();

        var builder = WebApplication.CreateBuilder();

        var apiConfigurationSection = builder.Configuration.GetRequiredSection("Api");
        var configuration = apiConfigurationSection.Get<ApiConfiguration>();
        builder.Services.Configure<ApiConfiguration>(apiConfigurationSection);

        if (!configuration.Enabled)
        {
            return default;
        }

        Log.Info($"Launching Intersect REST API in '{builder.Environment.EnvironmentName}' mode...");

        var corsPolicies = builder.Configuration.GetValue<Dictionary<string, CorsPolicy>>("Cors");
        if (corsPolicies != default)
        {
            builder.Services.AddCors(
                options =>
                {
                    foreach (var (name, policy) in corsPolicies)
                    {
                        options.AddPolicy(name, policy);
                    }
                }
            );
        }

        builder.Services.AddRouting(
            routeOptions =>
            {
                routeOptions.ConstraintMap.Add(nameof(AdminAction), typeof(AdminActionsConstraint));
                routeOptions.ConstraintMap.Add(nameof(ChatMessage), typeof(ChatMessage.RouteConstraint));
                routeOptions.ConstraintMap.Add(nameof(GameObjectType), typeof(GameObjectTypeConstraint));
                routeOptions.ConstraintMap.Add(nameof(LookupKey), typeof(LookupKey.RouteConstraint));
            }
        );

        builder.Services.AddRateLimiter(
            rateLimiterOptions =>
            {
                rateLimiterOptions.AddPolicy(
                    "client_per_second",
                    context => RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: context.User.Identity?.Name ?? "__no_api_key__",
                        factory: partition => new SlidingWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 5,
                            QueueLimit = 0,
                            Window = TimeSpan.FromMinutes(1)
                        }
                    )
                );
                rateLimiterOptions.AddPolicy(
                    "client_per_minute",
                    context => RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: context.User.Identity?.Name ?? "__no_api_key__",
                        factory: partition => new SlidingWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 1,
                            QueueLimit = 0,
                            Window = TimeSpan.FromSeconds(1)
                        }
                    )
                );
            }
        );

        builder.Services.AddControllers(
                mvcOptions =>
                {
                    mvcOptions.FormatterMappings.ClearMediaTypeMappingForFormat("application/xml");
                    mvcOptions.FormatterMappings.ClearMediaTypeMappingForFormat("text/xml");
                    mvcOptions.FormatterMappings.ClearMediaTypeMappingForFormat("text/json");
                    mvcOptions.FormatterMappings.ClearMediaTypeMappingForFormat("application/xml");
                }
            )
            .AddNewtonsoftJson(
                newtonsoftOptions =>
                {
                    newtonsoftOptions.SerializerSettings.ContractResolver =
                        new ApiVisibilityContractResolver(new HttpContextAccessor());
                }
            )
            .AddOData(
                options =>
                {
                    options.Count().Select().OrderBy();
                    options.RouteOptions.EnableKeyInParenthesis = false;
                    options.RouteOptions.EnableNonParenthesisForEmptyParameterFunction = true;
                    options.RouteOptions.EnableQualifiedOperationCall = false;
                    options.RouteOptions.EnableUnqualifiedOperationCall = true;
                }
            );
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHealthChecks();
        builder.Services.AddSwaggerGen();
        var tokenGenerationOptionsSection =
            apiConfigurationSection.GetRequiredSection(nameof(TokenGenerationOptions));
        var tokenGenerationOptions = tokenGenerationOptionsSection.Get<TokenGenerationOptions>();

        builder.Services.Configure<TokenGenerationOptions>(tokenGenerationOptionsSection);

        IdentityModelEventSource.ShowPII = true;
        builder.Services.AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                }
            )
            .AddJwtBearer(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ClockSkew = TimeSpan.FromSeconds(5),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = true,
                    };
                    builder.Configuration.Bind($"Api.{nameof(JwtBearerOptions)}", options);
                    options.TokenValidationParameters.ValidAudience ??= tokenGenerationOptions.Audience;
                    options.TokenValidationParameters.ValidIssuer ??= tokenGenerationOptions.Issuer;
                    options.Events = new JwtBearerEvents()
                    {
                        OnAuthenticationFailed = async (context) => {},
                        OnChallenge = async (context) => {},
                        OnMessageReceived = async (context) => {},
                        OnTokenValidated = async (context) => {},
                    };
                    SymmetricSecurityKey issuerKey = new(tokenGenerationOptions.SecretData);
                    options.TokenValidationParameters.IssuerSigningKey = issuerKey;
                }
            )
            .AddCookie(
                CookieAuthenticationDefaults.AuthenticationScheme,
                options =>
                {
                    builder.Configuration.Bind(nameof(CookieAuthenticationOptions), options);
                }
            );

        builder.Services.AddAuthorization(
            authOptions =>
            {
                authOptions.DefaultPolicy = new AuthorizationPolicyBuilder(authOptions.DefaultPolicy)
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .Build();
            }
        );

        builder.Services
            .AddIdentity<User, UserRole>(
                identityOptions =>
                {
                    identityOptions.Stores.ProtectPersonalData = true;
                }
            )
            .AddUserStore<IntersectUserStore>()
            .AddRoleStore<IntersectRoleStore>();

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddProblemDetails();
        }

        var knownProxies = configuration.KnownProxies;
        if (knownProxies?.Any() ?? false)
        {
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;

                foreach (var rawProxyAddress in knownProxies)
                {
                    if (string.IsNullOrWhiteSpace(rawProxyAddress))
                    {
                        Log.Debug($"Invalid address '{rawProxyAddress}'");
                    }
                    else if (IPAddress.TryParse(rawProxyAddress.Trim(), out IPAddress? proxyAddress))
                    {
                        Log.Debug($"Added {proxyAddress} as a known good proxy for forwarded headers");
                        options.KnownProxies.Add(proxyAddress);
                    }
                    else
                    {
                        try
                        {
                            var addresses = Dns.GetHostAddresses(rawProxyAddress.Trim());
                            foreach (var address in addresses)
                            {
                                Log.Debug($"Added {address} as a known good proxy for forwarded headers");
                                options.KnownProxies.Add(address);
                            }

                            if (addresses.Length < 1)
                            {
                                Log.Warn($"Failed to resolve {rawProxyAddress.Trim()}, unable to add as known proxy");
                            }
                        }
                        catch (Exception exception)
                        {
                            Log.Debug(exception, $"Failed to resolve {rawProxyAddress.Trim()}");
                        }
                    }
                }
            });
        }

        var app = builder.Build();

        if (knownProxies?.Any() ?? false)
        {
            app.UseForwardedHeaders();
        }
        app.UseNetworkFilterMiddleware(configuration.AllowedNetworkTypes);

        if (app.Environment.IsDevelopment())
        {
            app.UseODataRouteDebug();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        if (app.Environment.IsProduction())
        {
            app.UseHttpsRedirection();
            app.UseHsts();
        }

        if (configuration.RequestLogging)
        {
            app.UseIntersectRequestLogging(configuration.RequestLogLevel);
        }

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        // app.MapControllers();

        var healthChecksBuilder = app.MapHealthChecks("/health");
        if (app.Environment.IsProduction())
        {
            healthChecksBuilder.RequireAuthorization();
        }

        return app;
    }

    private async Task StartAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var app = Configure();
            if (app == default)
            {
                return;
            }

            _app = app;
            await app.StartAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            Log.Error(exception);
            throw;
        }
    }

    private async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (_app != default)
        {
            await _app.StopAsync(cancellationToken);
        }
    }

    public override bool IsEnabled => Configuration.Enabled;

    protected override void TaskStart(ServerContext applicationContext)
    {
        StartAsync().ConfigureAwait(false).GetAwaiter().GetResult();
    }

    protected override void TaskStop(ServerContext applicationContext)
    {
        StopAsync().ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public ApiConfiguration Configuration =>
        _app?.Configuration.GetValue<ApiConfiguration>("Api") ?? new ApiConfiguration { Enabled = true };
}