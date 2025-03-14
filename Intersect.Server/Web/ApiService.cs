using System.Globalization;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Threading.RateLimiting;
using Htmx.TagHelpers;
using Intersect.Core;
using Intersect.Enums;
using Intersect.Framework.Reflection;
using Intersect.Security.Claims;
using Intersect.Server.Core;
using Intersect.Server.Database.PlayerData.Api;
using Intersect.Server.Web.Authentication;
using Intersect.Server.Web.Configuration;
using Intersect.Server.Web.Constraints;
using Intersect.Server.Web.Middleware;
using Intersect.Server.Web.Serialization;
using Intersect.Server.Web.Swagger.Filters;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Intersect.Server.Collections.Indexing;
using Intersect.Server.Web.Controllers;
using Intersect.Server.Web.Controllers.Api;
using Intersect.Server.Web.Controllers.AssetManagement;
using Intersect.Server.Web.Extensions;
using Intersect.Server.Web.Types.Chat;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using MyCSharp.HttpUserAgentParser.AspNetCore.DependencyInjection;
using MyCSharp.HttpUserAgentParser.MemoryCache.DependencyInjection;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Intersect.Server.Web;

internal partial class ApiService : ApplicationService<ServerContext, IApiService, ApiService>, IApiService
{
    private const string BearerCookieFallbackAuthenticationScheme = "BearerCookieFallback";
    private WebApplication? _app;
    private static readonly Assembly Assembly = typeof(ApiService).Assembly;

    private static string GetOptionsName<TOptions>() => typeof(TOptions).Name.Replace("Options", string.Empty);

    // ReSharper disable once MemberCanBeMadeStatic.Local
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

        ApplicationContext.CurrentContext.Logger.LogInformation(
            "Launching Intersect REST API in '{EnvironmentName}' mode...",
            builder.Environment.EnvironmentName
        );

        builder.Services.AddSingleton(ApplicationContext.GetCurrentContext<IApplicationContext>());

        var updateServerSection = builder.Configuration.GetSection(GetOptionsName<UpdateServerOptions>());
        builder.Services.Configure<UpdateServerOptions>(updateServerSection);

        // I can't get System.Text.Json to deserialize an array as non-null, and it totally ignores
        // the JsonConverter attribute I tried putting on it, so I am just giving up and doing this
        // to make sure the array is not null in the event that it is empty.
        configuration.StaticFilePaths ??= [];

        var tokenGenerationOptionsSection =
            apiConfigurationSection.GetRequiredSection(nameof(TokenGenerationOptions));
        var tokenGenerationOptions = tokenGenerationOptionsSection.Get<TokenGenerationOptions>();

        builder.Services.Configure<TokenGenerationOptions>(tokenGenerationOptionsSection);

        var responseCompressionSection = builder.Configuration.GetSection(GetOptionsName<ResponseCompressionOptions>());
        builder.Services.Configure<ResponseCompressionOptions>(responseCompressionSection);

        builder.Services.AddResponseCompression();

        builder.Services.AddSingleton(ApplicationContext.CurrentContext);

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
                routeOptions.ConstraintMap.Add(nameof(AdminAction), typeof(EnumConstraint<AdminAction>));
                routeOptions.ConstraintMap.Add(nameof(GameObjectType), typeof(EnumConstraint<GameObjectType>));
                routeOptions.ConstraintMap.Add(nameof(ChatMessage), typeof(ChatMessage.RouteConstraint));
                routeOptions.ConstraintMap.Add(nameof(LookupKey), typeof(NonNullConstraint));
            }
        );

        builder.Services.AddHttpUserAgentMemoryCachedParser().AddHttpUserAgentParserAccessor();

        builder.Services.AddRateLimiter(
            rateLimiterOptions =>
            {
                rateLimiterOptions.AddPolicy(
                    "client_per_second",
                    context => RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: context.User.Identity?.Name ?? "__no_api_key__",
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 5,
                            QueueLimit = 0,
                            Window = TimeSpan.FromMinutes(1),
                        }
                    )
                );
                rateLimiterOptions.AddPolicy(
                    "client_per_minute",
                    context => RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: context.User.Identity?.Name ?? "__no_api_key__",
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 1,
                            QueueLimit = 0,
                            Window = TimeSpan.FromSeconds(1),
                        }
                    )
                );
            }
        );

        builder.Services.Configure<SecurityStampValidatorOptions>(
            options =>
            {
                options.ValidationInterval = TimeSpan.FromSeconds(10);
            }
        );

#if DEBUG
        IdentityModelEventSource.ShowPII = true;
#endif

        builder.Services.AddSingleton<IntersectAuthenticationManager>();

        builder.Services.AddAuthentication(
                options =>
                {
                    options.DefaultScheme = BearerCookieFallbackAuthenticationScheme;
                }
            )
            .AddCookie(
                CookieAuthenticationDefaults.AuthenticationScheme,
                options =>
                {
                    options.Events.OnSignedIn += async (context) => { };
                    options.Events.OnSigningIn += async (context) => { };
                    options.Events.OnSigningOut += async (context) => { };
                    options.Events.OnCheckSlidingExpiration += async (context) => { };
                    options.Events.OnValidatePrincipal += async context =>
                    {
                        var authenticationManager = context.HttpContext.RequestServices
                            .GetRequiredService<IntersectAuthenticationManager>();
                        var authenticationResult = await authenticationManager.UpdatePrincipal(
                            context.Principal,
                            context.ShouldRenew
                        );
                        var (result, _, updatedPrincipal) = authenticationResult;
                        if (result != AuthenticationResultType.Success || updatedPrincipal == default)
                        {
                            context.RejectPrincipal();
                            return;
                        }

                        if (updatedPrincipal == context.Principal)
                        {
                            return;
                        }

                        var logger = context.GetAPILogger();
                        logger.LogInformation(
                            "Renewing cookie for {UserId}",
                            updatedPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)
                        );
                        context.ShouldRenew = true;
                        context.ReplacePrincipal(updatedPrincipal);
                    };
                    options.Events.OnRedirectToLogin += async (context) =>
                    {
                        if (context.HttpContext.IsPage())
                        {
                            context.GetAPILogger().LogTrace(
                                "{OnRedirectToLogin} called for page (non-API) endpoint: {Route}",
                                nameof(CookieAuthenticationEvents.OnRedirectToLogin),
                                context.HttpContext.Request.Path
                            );
                            return;
                        }

                        if (context.HttpContext.IsController())
                        {
                            context.GetAPILogger().LogTrace(
                                "{OnRedirectToLogin} called for controller (API) endpoint: {Route}",
                                nameof(CookieAuthenticationEvents.OnRedirectToLogin),
                                context.HttpContext.Request.Path
                            );

                            context.RedirectUri = string.Empty;
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            context.Response.Headers.Location = StringValues.Empty;
                        }

                        context.GetAPILogger().LogWarning(
                            "{OnRedirectToLogin} called for unclassified endpoint: {Route}",
                            nameof(CookieAuthenticationEvents.OnRedirectToLogin),
                            context.HttpContext.Request.Path
                        );
                    };
                    options.Events.OnRedirectToAccessDenied += async (context) =>
                    {
                    };
                    options.Events.OnRedirectToLogout += async (context) => { };
                    options.Events.OnRedirectToReturnUrl += async (context) => { };
                    options.ExpireTimeSpan = TimeSpan.FromSeconds(10);
                    options.SlidingExpiration = true;
                    builder.Configuration.Bind(nameof(CookieAuthenticationOptions), options);
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
                    options.ForwardDefaultSelector += context =>
                    {
                        if (context.IsController())
                        {
                            return null;
                        }

                        if (context.IsPage())
                        {
                            return CookieAuthenticationDefaults.AuthenticationScheme;
                        }

                        if (context.Request.Headers.Authorization.Count > 0)
                        {
                            context.GetAPILogger().LogWarning(
                                "JwtBearer ForwardDefaultSelector invoked for unclassified endpoint, not forwarding because there is an Authorization header: {Route}",
                                context.Request.Path
                            );

                            return null;
                        }

                        context.GetAPILogger().LogWarning(
                            "JwtBearer ForwardDefaultSelector invoked for unclassified endpoint, falling back to cookie authentication because there is no Authorization header: {Route}",
                            context.Request.Path
                        );

                        return CookieAuthenticationDefaults.AuthenticationScheme;
                    };
                    builder.Configuration.Bind($"Api.{nameof(JwtBearerOptions)}", options);
                    options.TokenValidationParameters.ValidAudience ??= tokenGenerationOptions.Audience;
                    options.TokenValidationParameters.ValidIssuer ??= tokenGenerationOptions.Issuer;
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = async context =>
                        {
                        },
                        OnChallenge = async context =>
                        {
                            if (context.AuthenticateFailure is not null || context.HttpContext.IsController())
                            {
                                // This was needed to make sure authentication failures didn't return 200
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.HandleResponse();
                            }
                        },
                        OnMessageReceived = async context => { },
                        OnTokenValidated = async context =>
                        {
                            var rawTicketId = context.Principal?.FindFirstValue(IntersectClaimTypes.TicketId);
                            if (rawTicketId == default)
                            {
                                context.Fail("expired_token");
                                return;
                            }

                            if (!Guid.TryParse(rawTicketId, out var ticketId))
                            {
                                context.Fail("expired_token");
                                return;
                            }

                            if (!RefreshToken.TryFindForTicket(ticketId, out var refreshToken, includeUser: true))
                            {
                                context.Fail("expired_token");
                                return;
                            }

                            var authenticationManager = context.HttpContext.RequestServices
                                .GetRequiredService<IntersectAuthenticationManager>();
                            var authenticationResult = await authenticationManager.UpdatePrincipal(
                                context.Principal,
                                force: false,
                                user: refreshToken.User,
                                ignoreMidpoint: true
                            );
                            var (result, _, updatedPrincipal) = authenticationResult;
                            if (result != AuthenticationResultType.Success || updatedPrincipal == default)
                            {
                                context.Fail("expired_token");
                                return;
                            }

                            if (updatedPrincipal == context.Principal)
                            {
                                // context.NoResult();
                                context.Success();
                                return;
                            }

                            context.Fail("expired_token");

                            // var logger = context.GetAPILogger();
                            // logger.LogInformation(
                            //     "Changing token for {UserId}",
                            //     updatedPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)
                            // );
                            // context.Principal = updatedPrincipal;
                            // context.Success();
                        },
                    };
                    SymmetricSecurityKey issuerKey = new(tokenGenerationOptions.SecretData);
                    options.TokenValidationParameters.IssuerSigningKey = issuerKey;
                }
            )
            .AddPolicyScheme(
                BearerCookieFallbackAuthenticationScheme,
                "Bearer-to-Cookie Fallback",
                pso =>
                {
                    pso.ForwardDefaultSelector = context =>
                    {
                        if (context.Request.Headers.Authorization.Count > 0)
                        {
                            return JwtBearerDefaults.AuthenticationScheme;
                        }

                        if (context.IsController())
                        {
                            return JwtBearerDefaults.AuthenticationScheme;
                        }

                        if (context.IsPage())
                        {
                            return CookieAuthenticationDefaults.AuthenticationScheme;
                        }

                        context.GetAPILogger().LogWarning(
                            "Trying to authenticate with no authorization header on unclassified endpoint, falling back to cookie authentication: {Route}",
                            context.Request.Path
                        );

                        return CookieAuthenticationDefaults.AuthenticationScheme;
                    };
                }
            );

        builder.Services.AddOutputCache(o => o.AddPolicy(nameof(AvatarController), AvatarController.OutputCachePolicy));

        builder.Services.AddAuthorization(authOptions => authOptions.AddIntersectPolicies());

        builder.Services.AddResponseCaching();

        builder.Services.AddMvc(o => o.CacheProfiles.Add(nameof(AvatarController), AvatarController.ResponseCacheProfile))
            .WithRazorPagesRoot("/Web/Pages")
            .AddRazorPagesOptions(
                rpo =>
                {
                    rpo.Conventions.AuthorizeFolder("/Developer", policy: "Developer");
                }
            )
            .AddNewtonsoftJson(
                newtonsoftOptions =>
                {
                    newtonsoftOptions.SerializerSettings.ContractResolver =
                        new ApiVisibilityContractResolver(new HttpContextAccessor());
                    newtonsoftOptions.SerializerSettings.Converters.Add(new StringEnumConverter());
                }
            )
            .AddFormatterMappings(
                formatterMappings =>
                {
                    formatterMappings.ClearMediaTypeMappingForFormat("application/xml");
                    formatterMappings.ClearMediaTypeMappingForFormat("text/xml");
                    formatterMappings.ClearMediaTypeMappingForFormat("text/json");
                }
            );

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddHealthChecks();

        builder.Services.AddSwaggerGen(
            sgo =>
            {
                var version = typeof(ApiService).Assembly.GetVersionName();
                sgo.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = $"{Options.Instance.GameName} v{version} API",
                    Version = version,
                });

                sgo.TagActionsBy(
                    api =>
                    {
                        List<string> tags = [];

                        var groupName = api.GroupName;
                        if (!string.IsNullOrWhiteSpace(groupName))
                        {
                            tags.Add(groupName);
                        }

                        if (api.ActionDescriptor is ControllerActionDescriptor controllerDescriptor)
                        {
                            tags.Add(controllerDescriptor.ControllerName);
                        }

                        foreach (var parameterDescriptor in api.ActionDescriptor.Parameters)
                        {
                            if (parameterDescriptor.ParameterType.IsEnum)
                            {
                                tags.Add("Admin Actions");
                            }
                        }

                        return tags;
                    }
                );

                sgo.OrderActionsBy(api => api.RelativePath ?? api.GroupName ?? api.HttpMethod);

                var documentFilterTokenRequest =
                    new PolymorphicDocumentFilter<OAuthController.TokenRequest, OAuthController.GrantType>("grant_type")
                        .WithSubtype<OAuthController.TokenRequestPasswordGrant>(OAuthController.GrantType.Password)
                        .WithSubtype<OAuthController.TokenRequestRefreshTokenGrant>(
                            OAuthController.GrantType.RefreshToken
                        );
                sgo.AddDocumentFilterInstance(documentFilterTokenRequest);
                sgo.AddSchemaFilterInstance(documentFilterTokenRequest.CreateSchemaFilter());
                sgo.SchemaFilter<LookupKeySchemaFilter>();
                sgo.SchemaFilter<DictionarySchemaFilter>();
                sgo.SchemaFilter<GameObjectTypeSchemaFilter>();
                sgo.OperationFilter<MetadataOperationFilter>();
                sgo.SchemaFilter<MetadataSchemaFilter>();
                sgo.OperationFilter<AuthorizationOperationFilter>();
                sgo.DocumentFilter<GeneratorDocumentFilter>();
                sgo.EnableAnnotations(enableAnnotationsForInheritance: true, enableAnnotationsForPolymorphism: true);
                sgo.UseOneOfForPolymorphism();
                sgo.UseAllOfForInheritance();
                sgo.AddSecurityDefinition(
                    SecuritySchemes.Bearer,
                    new OpenApiSecurityScheme
                    {
                        Name = nameof(HttpRequestHeader.Authorization),
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = SecuritySchemes.Bearer,
                        Description = "JWT token obtained from the /api/oauth/token endpoint",
                    }
                );
            });
        builder.Services.AddSwaggerGenNewtonsoftSupport();

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
                        ApplicationContext.Context.Value?.Logger.LogDebug($"Invalid address '{rawProxyAddress}'");
                    }
                    else if (IPAddress.TryParse(rawProxyAddress.Trim(), out IPAddress? proxyAddress))
                    {
                        ApplicationContext.Context.Value?.Logger.LogDebug($"Added {proxyAddress} as a known good proxy for forwarded headers");
                        options.KnownProxies.Add(proxyAddress);
                    }
                    else
                    {
                        try
                        {
                            var addresses = Dns.GetHostAddresses(rawProxyAddress.Trim());
                            foreach (var address in addresses)
                            {
                                ApplicationContext.Context.Value?.Logger.LogDebug($"Added {address} as a known good proxy for forwarded headers");
                                options.KnownProxies.Add(address);
                            }

                            if (addresses.Length < 1)
                            {
                                ApplicationContext.Context.Value?.Logger.LogWarning($"Failed to resolve {rawProxyAddress.Trim()}, unable to add as known proxy");
                            }
                        }
                        catch (Exception exception)
                        {
                            ApplicationContext.Context.Value?.Logger.LogDebug(exception, $"Failed to resolve {rawProxyAddress.Trim()}");
                        }
                    }
                }
            });
        }

        var app = builder.Build();

        app.UseNetworkFilterMiddleware(configuration.AllowedNetworkTypes);

        if (knownProxies?.Any() ?? false)
        {
            app.UseForwardedHeaders();
        }

        if (app.Environment.IsProduction())
        {
            app.UseHttpsRedirection();
            app.UseHsts();
        }

        app.UseResponseCompression();

        if (app.Environment.IsDevelopment())
        {
            app.UseODataRouteDebug();
        }

        if (configuration.RequestLogging)
        {
            app.UseIntersectRequestLogging(configuration.RequestLogLevel);
        }

        app.UseRouting();

        app.UseAuthentication();

        app.UseRequestLocalization(
            new RequestLocalizationOptions
            {
                ApplyCurrentCultureToResponseHeaders = true,
                FallBackToParentCultures = true,
                FallBackToParentUICultures = true,
                SupportedCultures = new List<CultureInfo>
                {
                    CultureInfo.GetCultureInfo("en-US"),
                    CultureInfo.GetCultureInfo("es"),
                    CultureInfo.GetCultureInfo("it"),
                    CultureInfo.GetCultureInfo("pt-BR"),
                },
                SupportedUICultures = new List<CultureInfo>
                {
                    CultureInfo.GetCultureInfo("en-US"),
                    CultureInfo.GetCultureInfo("es"),
                    CultureInfo.GetCultureInfo("it"),
                    CultureInfo.GetCultureInfo("pt-BR"),
                },
            }
        );

#if DEBUG
        if (configuration.EnableAuthenticationDebugging && app.Environment.IsDevelopment())
        {
            app.UseMiddleware<AuthenticationDebugMiddleware>();
        }
#endif

        app.UseMiddleware<ApplicationContextMiddleware>();

        app.UseResponseCaching();
        app.UseOutputCache();

        StaticFileOptions staticFileOptions = new()
        {
            HttpsCompression = HttpsCompressionMode.Compress,
            ServeUnknownFileTypes = true,
        };

        app.UseStaticFiles(staticFileOptions);

        // Unreadable if it LINQs it...
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var (sourcePath, requestPath) in configuration.StaticFilePaths)
        {
            var pathToRoot = Path.Combine(builder.Environment.ContentRootPath, sourcePath);
            if (!Directory.Exists(pathToRoot))
            {
                Directory.CreateDirectory(pathToRoot);
            }

            app.UseStaticFiles(
                new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
                        pathToRoot,
                        ExclusionFilters.Sensitive
                    ),
                    HttpsCompression = HttpsCompressionMode.Compress,
                    RequestPath = requestPath ?? string.Empty,
                    ServeUnknownFileTypes = true,
                }
            );
        }

        app.UseAuthorization();

        app.MapHtmxAntiforgeryScript();

        app.UseCookiePolicy(
            new CookiePolicyOptions
            {
                /* Because OnAppendCookie is called after the HttpOnly is overwritten we need to set this to None
                 * and then manually force it if it isn't excluded. */
                HttpOnly = HttpOnlyPolicy.None,
                MinimumSameSitePolicy = SameSiteMode.Strict,
                OnAppendCookie = appendCookieContext =>
                {
                    if (appendCookieContext.CookieOptions.HttpOnly)
                    {
                        return;
                    }

                    appendCookieContext.CookieOptions.HttpOnly =
                        !(configuration.AllowedNonHttpOnlyCookies?.Contains(appendCookieContext.CookieName) ?? false);
                },
#if DEBUG
                Secure = CookieSecurePolicy.SameAsRequest,
#else
                Secure = CookieSecurePolicy.Always,
#endif
            }
        );

        // Swagger is always enabled in development, but outside of development it needs to be manually enabled
        if (configuration.EnableSwaggerUI || app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        var healthChecksBuilder = app.MapHealthChecks("/health");

        if (app.Environment.IsProduction())
        {
            healthChecksBuilder.RequireAuthorization();
        }

        app.MapRazorPages();
        app.MapControllers();

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
            ApplicationContext.Context.Value?.Logger.LogError(exception, "Error starting API service");
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