using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Intersect.Framework.Services.Bootstrapping;

public static class BootstrapperServiceHostBuilderExtensions
{
    public static IHostBuilder UseBootstrapper(
        this IHostBuilder hostBuilder,
        Action<HostBuilderContext, IServiceCollection>? bootstrapConfigurationAction = default
    ) => hostBuilder.ConfigureServices(
        (context, services) =>
        {
            services.AddSingleton(new BootstrapServiceOptions());
            services.AddHostedService<BootstrapperService>();

            bootstrapConfigurationAction?.Invoke(context, services);
        }
    );
}