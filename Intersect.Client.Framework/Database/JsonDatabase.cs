using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Intersect.Configuration;
using Intersect.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Database;

public class JsonDatabase : GameDatabase
{
    private readonly string _instancePath;
    private JObject? _instance;

    public JsonDatabase()
    {
        _instancePath = GetInstancePath(ClientConfiguration.Instance);
        _ = TryOpenOrCreate(_instancePath, out _instance);
    }

    private static string GetInstancePath(ClientConfiguration instance)
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ".intersect",
            Assembly.GetEntryAssembly().GetName().Name, $"{instance.Host}.{instance.Port}.json");
    }

    private bool TryOpenOrCreate(string instancePath, [NotNullWhen(true)] out JObject? instance)
    {
        try
        {
            var fileInfo = new FileInfo(instancePath);

            if (!fileInfo.Exists)
            {
                if (!fileInfo.Directory!.Exists)
                {
                    fileInfo.Directory.Create();
                }

                using var streamWriter = fileInfo.CreateText();
                streamWriter.WriteLine("{}");
                instance = new JObject();
                return true;
            }

            using var streamReader = fileInfo.OpenText();
            var json = streamReader.ReadToEnd();
            instance = JObject.Parse(json);
            return true;
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogError(
                exception,
                $"Failed to open or create part or all of the path: {instancePath}"
            );
            instance = default;
            return false;
        }
    }

    private bool TryWrite(string instancePath, JObject instance)
    {
        try
        {
            var fileInfo = new FileInfo(instancePath);

            if (!fileInfo.Exists)
            {
                if (!fileInfo.Directory!.Exists)
                {
                    fileInfo.Directory.Create();
                }
            }

            using var streamWriter = fileInfo.CreateText();
            var json = instance.ToString(
#if DEBUG
                Formatting.Indented
#else
                Formatting.None
#endif
            );
            streamWriter.WriteLine(json);
            return true;
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogError(
                exception,
                $"Failed to open or create part or all of the path, or failed while writing to: {instancePath}"
            );
            instance = default;
            return false;
        }
    }

    private void Persist() => _ = TryWrite(_instancePath, _instance);

    public override void DeletePreference(string key)
    {
        _instance?.Remove(key);
        Persist();
    }

    public override bool HasPreference(string key) => _instance?.ContainsKey(key) ?? false;

    public override void SavePreference<TValue>(string key, TValue value)
    {
        if (_instance == default)
        {
            return;
        }

        var stringifiedValue = Convert.ToString(value);
        _instance[key] = JValue.CreateString(stringifiedValue);
        Persist();
    }

    public override string LoadPreference(string key)
    {
        var token = _instance?[key];

        if (token == default)
        {
            return string.Empty;
        }

        if (token.Type == JTokenType.String)
        {
            return token.Value<string>();
        }

        ApplicationContext.Context.Value?.Logger.LogWarning(
            $"Found invalid type {token.Type} stored in {key} instead of {JTokenType.String}."
        );
        return string.Empty;
    }

    public override bool LoadConfig() => ClientConfiguration.LoadAndSave() != default;
}