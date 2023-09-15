using System.Reflection;
using System.Threading.RateLimiting;
using Intersect.Core;
using Intersect.Enums;
using Intersect.Server.Core;
using Intersect.Server.Web.Constraints;
using Intersect.Server.Web.Middleware;
using Intersect.Server.Web.RestApi.Configuration;
using Intersect.Server.Web.RestApi.Payloads;
using Intersect.Server.Web.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Intersect.Server.Web;

internal partial class ApiService : ApplicationService<ServerContext, IApiService, ApiService>, IApiService
{
    static ApiService()
    {
        UnpackAppSettings();
    }

    private WebApplication? _app;
    private static readonly Assembly Assembly = typeof(ApiService).Assembly;

    public ApiConfiguration Configuration { get; } = ApiConfiguration.Create();

    private WebApplication Configure()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddRouting(
            routeOptions =>
            {
                routeOptions.ConstraintMap.Add(nameof(AdminAction), typeof(AdminActionsConstraint));
                routeOptions.ConstraintMap.Add(nameof(ChatMessage), typeof(ChatMessage.RouteConstraint));
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
        ).AddJsonOptions(
            jsonOptions =>
            {
            }
        ).AddNewtonsoftJson(
            newtonsoftOptions =>
            {
                newtonsoftOptions.SerializerSettings.ContractResolver = new ApiVisibilityContractResolver(new HttpContextAccessor());
            }
        );
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHealthChecks();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        if (app.Environment.IsProduction())
        {
            app.UseHttpsRedirection();
            app.UseHsts();
        }

        if (Configuration.RequestLogging)
        {
            app.UseIntersectRequestLogging(Configuration.RequestLogLevel);
        }

        app.UseAuthorization();

        app.MapControllers();

        var healthChecksBuilder = app.MapHealthChecks("/health");
        if (app.Environment.IsProduction())
        {
            healthChecksBuilder.RequireAuthorization();
        }

        return app;
    }

    private async Task StartAsync(CancellationToken cancellationToken = default)
    {
        var app = Configure();
        _app = app;
        await app.StartAsync(cancellationToken);
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
}