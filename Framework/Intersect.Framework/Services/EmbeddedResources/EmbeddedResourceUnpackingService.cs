using System.Resources;
using Intersect.Framework.Reflection;
using Intersect.Framework.Services.Bootstrapping;
using Microsoft.Extensions.Logging;

namespace Intersect.Framework.Services.EmbeddedResources;

public sealed class EmbeddedResourceUnpackingService : IBootstrapTask
{
    private readonly ILogger<EmbeddedResourceUnpackingService> _logger;
    private readonly EmbeddedResourceUnpackingServiceOptions _options;

    public EmbeddedResourceUnpackingService(
        ILoggerFactory loggerFactory,
        EmbeddedResourceUnpackingServiceOptions options
    )
    {
        _logger = loggerFactory.CreateLogger<EmbeddedResourceUnpackingService>();
        _options = options;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        foreach (var resourceUnpackingRequest in _options.Requests)
        {
            var sanitizedResourceName = resourceUnpackingRequest.ResourceName;
            sanitizedResourceName = sanitizedResourceName.Replace('-', '_').Replace('/', '.').Replace('\\', '.');

            try
            {
                await resourceUnpackingRequest.Assembly.UnpackEmbeddedFileAsync(
                    cancellationToken,
                    sanitizedResourceName,
                    overwrite: resourceUnpackingRequest.Overwrite,
                    unpackedName: resourceUnpackingRequest.UnpackedName
                );
            }
            catch (MissingManifestResourceException missingManifestResourceException)
            {
                _logger.LogError(
                    missingManifestResourceException,
                    "'{ResourceName}' was not found in '{AssemblyName}' ({SanitizedResourceName})",
                    resourceUnpackingRequest.ResourceName,
                    resourceUnpackingRequest.Assembly.FullName ?? resourceUnpackingRequest.Assembly.GetName().FullName,
                    sanitizedResourceName
                );
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error unpacking {ResourceName}", resourceUnpackingRequest.ResourceName);
                throw;
            }
        }
    }
}
