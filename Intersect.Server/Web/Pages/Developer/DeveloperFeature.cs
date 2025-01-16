using Serilog;
using ILogger = Serilog.ILogger;

namespace Intersect.Server.Web.Pages.Developer;

public sealed partial record DeveloperFeature(
    string Name,
    string Link,
    Func<string> LocalizedNameProvider,
    Func<HttpContext, bool>? EnablementProvider = default
)
{
    private ILogger? _logger;

    private ILogger Logger => _logger ??= Log.ForContext<DeveloperFeature>();

    public string Label
    {
        get
        {
            try
            {
                var localizedName = LocalizedNameProvider();
                return string.IsNullOrWhiteSpace(localizedName) ? Name : localizedName;
            }
            catch (Exception exception)
            {
                Logger.Warning(exception, "Error occurred when localizing name of {Feature}", Name);
                return Name;
            }
        }
    }
}