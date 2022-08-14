using System.Globalization;

using Intersect.Localization;
using Intersect.Localization.Common;
using Intersect.Logging;

using Newtonsoft.Json;

namespace Intersect.Editor.Localization;

public sealed class LocalizationLoader
{
    public static readonly string StringsDirectoryPath = Path.Combine("resources", "strings");

    public LocalizationLoader(string applicationName)
    {
        ApplicationName = !string.IsNullOrWhiteSpace(applicationName)
            ? applicationName
            : throw new ArgumentNullException(nameof(applicationName));
    }

    public string ApplicationName { get; }

    private JsonSerializerSettings SerializerSettings { get; } = new JsonSerializerSettings
    {
        Converters = new List<JsonConverter>
        {
            new LocalizedString.Converter()
        },
        ObjectCreationHandling = ObjectCreationHandling.Reuse,
    };

    public string? FindStrings(CultureInfo cultureInfo)
    {
        if (!Directory.Exists(StringsDirectoryPath))
        {
            return default;
        }

        var possibleFiles = Directory
            .EnumerateFiles(
                StringsDirectoryPath,
                $"{ApplicationName}*.json"
            )
            .ToList();
        var match = LocalizationHelper.MatchCulture(cultureInfo, possibleFiles);
        return match;
    }

    public string GetFileNameForCulture(CultureInfo cultureInfo) =>
        Path.Combine(
            StringsDirectoryPath,
            $"{ApplicationName}.{cultureInfo.IetfLanguageTag}.json"
        );

    private void LoadFrom<TRootNamespace>(TRootNamespace rootNamespace, string json)
        where TRootNamespace : RootNamespace, new()
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentNullException(nameof(json));
        }

        JsonConvert.PopulateObject(json, rootNamespace, SerializerSettings);
    }

    public void Reset<TRootNamespace>(TRootNamespace rootNamespace)
        where TRootNamespace : RootNamespace, new()
    {
        var json = Serialize(rootNamespace, Formatting.None);
        LoadFrom(rootNamespace, json);
    }

    private string Serialize(RootNamespace rootNamespace, Formatting formatting) =>
        JsonConvert.SerializeObject(rootNamespace, formatting, SerializerSettings);

    public bool SaveDefaultIfMissing<TRootNamespace>()
        where TRootNamespace : RootNamespace, new()
    {
        var defaultFileName = GetFileNameForCulture(LocalizationHelper.CultureEnUS);
        if (File.Exists(defaultFileName))
        {
            return false;
        }

        TRootNamespace rootNamespace = new();
        return TrySave(rootNamespace, LocalizationHelper.CultureEnUS);
    }

    public bool TryLoad<TRootNamespace>(TRootNamespace rootNamespace, CultureInfo cultureInfo)
        where TRootNamespace : RootNamespace, new()
    {
        var fileName = FindStrings(cultureInfo);

        if (fileName == default)
        {
            return TrySave(rootNamespace, LocalizationHelper.CultureEnUS);
        }

        try
        {
            using var fileStream = File.OpenRead(fileName);
            using var fileReader = new StreamReader(fileStream);
            var json = fileReader.ReadToEnd();

            LoadFrom(rootNamespace, json);
            return true;
        }
        catch (Exception exception)
        {
            // TODO: don't do this automatically all the time
            //
            // The plan is to refactor and move this to core for reuse, but
            //  the editor specifically has no need for this -- there should be
            //  an option in the File menu to dump the default locale to disk
            //
            // idea: possibly with junk like <<<text>>> (the string is "text")
            //  added to all strings so as to mark them as "needs localization"
            if (SaveDefaultIfMissing<TRootNamespace>())
            {
                return true;
            }

            Log.Error(
                exception,
                $"Exception reading localizations from '{fileName}'."
            );
            return false;
        }
    }

    public bool TrySave<TRootNamespace>(TRootNamespace rootNamespace, CultureInfo? cultureInfo = default)
        where TRootNamespace : RootNamespace, new()
    {
        cultureInfo ??= CultureInfo.CurrentCulture;
        var fileName = GetFileNameForCulture(cultureInfo);

        try
        {
            if (!Directory.Exists(StringsDirectoryPath))
            {
                _ = Directory.CreateDirectory(StringsDirectoryPath);
            }

            var json = Serialize(rootNamespace, Formatting.Indented);
            using var fileStream = File.OpenWrite(fileName);
            using var fileWriter = new StreamWriter(fileStream);
            fileWriter.Write(json);
            return true;
        }
        catch (Exception exception)
        {
            Log.Error(
                exception,
                $"Exception writing localizations to '{fileName}'."
            );
            return false;
        }
    }
}

internal static partial class Strings
{
    private static readonly LocalizationLoader _localizationLoader = new("editor");

    public static CultureInfo CurrentCulture { get; set; } = CultureInfo.CurrentCulture;

    public static bool Load()
    {
        if (!_localizationLoader.TryLoad(Root, CurrentCulture))
        {
            // TODO: We should not close when localization fails to load
            // Instead, show a message box in the active/default locale
            throw new Exception("Failed to load localized strings.");
        }

        Program.OpenGLLink = Errors.opengllink.ToString();
        Program.OpenALLink = Errors.openallink.ToString();

        return true;
    }

    public static bool Save() =>
        _localizationLoader.TrySave(Root, CurrentCulture);

    public struct Errors
    {

        public static LocalizedString displaynotsupported = @"Invalid Display Configuration!";

        public static LocalizedString displaynotsupportederror =
            @"Fullscreen {00} resolution is not supported on this device!";

        public static LocalizedString errorencountered =
            @"The Intersect Client has encountered an error and must close. Error information can be found in logs/errors.log";

        public static LocalizedString notconnected = @"Not connected to the game server. Is it online?";

        public static LocalizedString notsupported = @"Not Supported!";

        public static LocalizedString openallink = @"https://goo.gl/Nbx6hx";

        public static LocalizedString opengllink = @"https://goo.gl/RSP3ts";

        public static LocalizedString passwordinvalid =
            @"Password is invalid. Please use alphanumeric characters with a length between 4 and 20.";

        public static LocalizedString resourcesnotfound =
            @"The resources directory could not be found! Intersect will now close.";

        public static LocalizedString title = @"Error!";

        public static LocalizedString usernameinvalid =
            @"Username is invalid. Please use alphanumeric characters with a length between 2 and 20.";

        public static LocalizedString LoadFile =
            @"Failed to load a {00}. Please send the game administrator a copy of your errors log file in the logs directory.";

        public static LocalizedString lostconnection =
            @"Lost connection to the game server. Please make sure you're connected to the internet and try again!";

    }

    public struct Main
    {

        public static LocalizedString gamename = @"Intersect Client";

    }


    public struct Numbers
    {

        public static LocalizedString thousands = "k";

        public static LocalizedString millions = "m";

        public static LocalizedString billions = "b";

        public static LocalizedString dec = ".";

        public static LocalizedString comma = ",";

    }

    public struct Update
    {

        public static LocalizedString Checking = @"Checking for updates, please wait!";

        public static LocalizedString Updating = @"Downloading updates, please wait!";

        public static LocalizedString Restart = @"Update complete! Relaunch {00} to play!";

        public static LocalizedString Done = @"Update complete! Launching game!";

        public static LocalizedString Error = @"Update Error! Check logs for more info!";

        public static LocalizedString Files = @"{00} Files Remaining";

        public static LocalizedString Size = @"{00} Left";

        public static LocalizedString Percent = @"{00}%";

    }
}
