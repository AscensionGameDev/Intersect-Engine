using Newtonsoft.Json;

using System;
using System.IO;

namespace Intersect.Client.Framework.Storage
{
    public class JsonPreferencesSerializer : PreferencesSerializer
    {
        public static string DefaultDirectory =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Intersect", "Client");

        public const string DefaultFileName = "preferences.json";

        private string FilePath { get; }

        public JsonPreferencesSerializer(IGameContext gameContext, string destinationPath = null) : base(gameContext)
        {
            if (string.IsNullOrWhiteSpace(destinationPath))
            {
                FilePath = Path.Combine(DefaultDirectory, DefaultFileName);
                if (!Directory.Exists(DefaultDirectory))
                {
                    Directory.CreateDirectory(DefaultDirectory);
                }
            } else if (!File.Exists(destinationPath) && !Path.HasExtension(destinationPath))
            {
                FilePath = Path.Combine(destinationPath, DefaultFileName);
                if (!Directory.Exists(destinationPath))
                {
                    Directory.CreateDirectory(destinationPath);
                }
            }
            else
            {
                FilePath = destinationPath;
                var directory = Path.GetDirectoryName(destinationPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
        }

        /// <inheritdoc />
        public override bool Deserialize(IPreferences destinationPreferences)
        {
            try
            {
                using (var fileStream = GameContext.Storage.OpenFileRead(FilePath))
                {
                    using (var streamReader = new StreamReader(fileStream))
                    {
                        var contents = streamReader.ReadToEnd();
                        JsonConvert.PopulateObject(contents, destinationPreferences);
                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                GameContext.Logger.Error(exception, "Error reading from preferences file.");
                return false;
            }
        }

        /// <inheritdoc />
        public override bool Serialize(IPreferences preferences)
        {
            try
            {
                using (var fileStream = GameContext.Storage.OpenFileWrite(FilePath, true))
                {
                    using (var streamWriter = new StreamWriter(fileStream))
                    {
                        var contents = JsonConvert.SerializeObject(preferences);
                        streamWriter.WriteLine(contents);
                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                GameContext.Logger.Error(exception, "Error writing to preferences file.");
                return false;
            }
        }
    }
}
