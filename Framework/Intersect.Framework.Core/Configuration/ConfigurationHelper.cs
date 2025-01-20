using System.Text;
using Intersect.Core;
using Intersect.Framework.Reflection;
using Intersect.IO.Files;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Intersect.Configuration;

public static partial class ConfigurationHelper
{
    public static string CacheName { get; set; } = default!;

    public static T Load<T>(T configuration, string filePath, bool failQuietly = false)
        where T : IConfiguration<T>
    {
        if (!File.Exists(filePath))
        {
            if (failQuietly)
            {
                return configuration;
            }

            throw new FileNotFoundException("Missing configuration file.", filePath);
        }

        try
        {
            var json = File.ReadAllText(filePath, Encoding.UTF8);

            JsonConvert.PopulateObject(json, configuration);

            return configuration;
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogError(
                exception,
                "Error when reading {ConfigurationType} from {FilePath}",
                typeof(T).GetName(qualified: true),
                filePath
            );

            throw;
        }
    }

    public static T Save<T>(T configuration, string filePath, bool failQuietly = false)
        where T : IConfiguration<T>
    {
        var directoryPath = Path.GetDirectoryName(filePath);
        if (directoryPath == null)
        {
            throw new ArgumentException($"'{filePath}' has no directory name", nameof(filePath));
        }

        if (!FileSystemHelper.EnsureDirectoryExists(directoryPath))
        {
            throw new DirectoryNotFoundException($"Directory does not exist: {directoryPath}");
        }

        try
        {
            var json = JsonConvert.SerializeObject(configuration, Formatting.Indented);

            File.WriteAllText(filePath, json, Encoding.UTF8);

            return configuration;
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogError(
                exception,
                "Error when writing {ConfigurationType} to {FilePath}",
                typeof(T).GetName(qualified: true),
                filePath
            );

            throw;
        }
    }

    public static T LoadSafely<T>(T configuration, string filePath = null)
        where T : IConfiguration<T>
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return configuration;
        }

        try
        {
            configuration.Load(filePath);
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Error when reading {ConfigurationType} to {FilePath}",
                typeof(T).GetName(qualified: true),
                filePath
            );
        }
        finally
        {
            try
            {
                configuration.Save(filePath);
            }
            catch (Exception exception)
            {
                ApplicationContext.Context.Value?.Logger.LogError(
                    exception,
                    "Error when writing {ConfigurationType} to {FilePath}",
                    typeof(T).GetName(qualified: true),
                    filePath
                );
            }
        }

        return configuration;
    }
}
