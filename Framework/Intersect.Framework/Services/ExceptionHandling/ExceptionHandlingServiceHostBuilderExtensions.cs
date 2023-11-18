using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Intersect.Framework.Services.ExceptionHandling;

public static class ExceptionHandlingServiceHostBuilderExtensions
{
    public static IHostBuilder UseExceptionHandling(this IHostBuilder hostBuilder) =>
        hostBuilder.ConfigureServices((_, services) => services.AddHostedService<ExceptionHandlingService>());
}