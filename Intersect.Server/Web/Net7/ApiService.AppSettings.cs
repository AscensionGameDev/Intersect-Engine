using Intersect.Logging;
using Microsoft.Extensions.Hosting;

namespace Intersect.Server.Web;

internal partial class ApiService
{
    private static void UnpackAppSettings()
    {
        var hostBuilder = Host.CreateApplicationBuilder();

        var names = new[] { "appsettings.json", $"appsettings.{hostBuilder.Environment.EnvironmentName}.json" };
        var manifestResourceNamePairs = Assembly.GetManifestResourceNames()
            .Where(mrn => names.Any(mrn.EndsWith))
            .Select(mrn => (mrn, names.First(mrn.EndsWith)))
            .ToArray();

        foreach (var (mrn, name) in manifestResourceNamePairs)
        {
            if (string.IsNullOrWhiteSpace(mrn) || string.IsNullOrWhiteSpace(name))
            {
                Log.Warn($"Manifest resource name or file name is null/empty: ({mrn}, {name})");
                continue;
            }

            if (File.Exists(name))
            {
                Log.Debug($"'{name}' already exists, not unpacking '{mrn}'");
                continue;
            }

            using var mrs = Assembly.GetManifestResourceStream(mrn);
            if (mrs == default)
            {
                Log.Warn($"Unable to open stream for embedded content: '{mrn}'");
                continue;
            }

            using var fs = File.OpenWrite(name);
            mrs.CopyTo(fs);
        }
    }
}