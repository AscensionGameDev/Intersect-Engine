using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Intersect.Server.Web.Middleware;

public static class IntersectRateLimiting
{
    public static void UseIntersectRequestLogging(this WebApplicationBuilder builder, Options? options = default)
    {
        options ??= builder.Configuration.GetSection("Intersect").GetValue("RateLimiting", Options.Defaults);

        builder.Services.AddRateLimiter(
            rateLimiterOptions =>
            {
                // void AddPolicy(string prefix, )
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
    }

    public class RateLimitPeriod
    {
        public int PerMinute { get; set; }

        public int PerSecond { get; set; }
    }

    public class Options
    {
        public const string DefaultClientKey = "__NO_API_KEY__";

        public static Options Defaults => new()
        {
            ClientLimits = new Dictionary<string, RateLimitPeriod>
            {
                {
                    DefaultClientKey, new RateLimitPeriod
                    {
                        PerMinute = 5,
                        PerSecond = 1,
                    }
                },
            },
            RemoteAddressLimitsFallback = new RateLimitPeriod
            {
                PerMinute = 60,
                PerSecond = 1,
            },
            GlobalLimits = new RateLimitPeriod
            {
                PerMinute = 6000,
                PerSecond = 100,
            },
        };

        public Dictionary<string, RateLimitPeriod>? ClientLimits { get; set; }

        public RateLimitPeriod? ClientLimitsFallback { get; set; }

        public Dictionary<string, RateLimitPeriod>? EndpointLimits { get; set; }

        public RateLimitPeriod? EndpointLimitsFallback { get; set; }

        public Dictionary<string, RateLimitPeriod>? RemoteAddressLimits { get; set; }

        public RateLimitPeriod? RemoteAddressLimitsFallback { get; set; }

        public RateLimitPeriod? GlobalLimits { get; set; }
    }
}