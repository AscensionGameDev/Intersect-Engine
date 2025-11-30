using System.Diagnostics.CodeAnalysis;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Input;

/// <summary>
/// Helper class for loading default control mappings from resources/default_settings.json
/// </summary>
internal static class DefaultSettingsLoader
{
    /// <summary>
    /// Loads default control mappings from resources/default_settings.json if the file exists.
    /// </summary>
    /// <param name="controlsToLoad">The controls to attempt to load from the file</param>
    /// <returns>Dictionary of controls with their mappings from the file (empty if file doesn't exist)</returns>
    public static Dictionary<Control, ControlMapping> LoadDefaultsFromFile(IEnumerable<Control> controlsToLoad)
    {
        var defaults = new Dictionary<Control, ControlMapping>();

        var defaultSettingsPath = Path.Combine("resources", "default_settings.json");
        if (!File.Exists(defaultSettingsPath))
        {
            // File doesn't exist - this is expected for most games
            return defaults;
        }

        try
        {
            var json = File.ReadAllText(defaultSettingsPath);
            var settings = JObject.Parse(json);

            foreach (var control in controlsToLoad)
            {
                if (TryLoadControlMapping(settings, control, out var mapping))
                {
                    defaults[control] = mapping;
                }
            }
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogError(
                exception,
                "Failed to load default_settings.json"
            );
        }

        return defaults;
    }

    /// <summary>
    /// Attempts to load a single control's mapping from the JSON settings.
    /// </summary>
    /// <param name="settings">The parsed JSON object containing the settings</param>
    /// <param name="control">The control to load</param>
    /// <param name="mapping">The loaded control mapping, if successful</param>
    /// <returns>True if at least one valid binding was loaded, false otherwise</returns>
    private static bool TryLoadControlMapping(
        JObject settings,
        Control control,
        [NotNullWhen(true)] out ControlMapping? mapping)
    {
        var controlId = control.GetControlId();
        var bindings = new List<ControlBinding>();

        // Try to load binding0 and binding1
        for (int i = 0; i < 2; i++)
        {
            var key = $"{controlId}_binding{i}";
            if (settings.TryGetValue(key, out var token) && token.Type == JTokenType.String)
            {
                try
                {
                    var binding = JsonConvert.DeserializeObject<ControlBinding>(token.Value<string>());
                    if (binding != null)
                    {
                        bindings.Add(binding);
                    }
                }
                catch (Exception ex)
                {
                    ApplicationContext.Context.Value?.Logger.LogWarning(
                        ex,
                        "Failed to deserialize binding {Key} from default_settings.json",
                        key
                    );
                }
            }
        }

        // Need at least one valid binding
        if (bindings.Count > 0)
        {
            // Pad to 2 bindings (ControlMapping requires at least 2)
            while (bindings.Count < 2)
            {
                bindings.Add(ControlBinding.Default);
            }

            mapping = new ControlMapping(bindings[0], bindings.Skip(1).ToArray());
            return true;
        }

        mapping = null;
        return false;
    }
}
