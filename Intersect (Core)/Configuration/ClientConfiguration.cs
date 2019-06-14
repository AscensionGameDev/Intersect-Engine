using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace Intersect.Configuration
{
    /// <inheritdoc />
    /// <summary>
    /// Client configuration options
    /// </summary>
    public sealed class ClientConfiguration : IConfiguration<ClientConfiguration>
    {

        public const string DefaultPath = @"resources/config.json";

        #region Constants

        public const string DEFAULT_HOST = "localhost";

        public const int DEFAULT_PORT = 5400;

        public const string DEFAULT_FONT = "arial";

        public const int DEFAULT_CHAT_LINES = 100;

        public const string DEFAULT_MENU_BACKGROUND = "background.png";

        #endregion

        #region Static Properties and Methods

        [NotNull] public static ClientConfiguration Instance { get; } = new ClientConfiguration();

        public void Validate()
        {
            Host = string.IsNullOrWhiteSpace(Host) ? DEFAULT_HOST : Host.Trim();
            Port = Math.Min(Math.Max(Port, (ushort) 1), ushort.MaxValue);
            Font = string.IsNullOrWhiteSpace(Font) ? DEFAULT_FONT : Font.Trim();
            ChatLines = Math.Min(Math.Max(ChatLines, 10), 500);
            IntroImages = new List<string>(IntroImages?.Distinct() ?? new List<string>());
        }

        #endregion

        #region Options

        /// <summary>
        /// Hostname of the server to connect to
        /// </summary>
        public string Host { get; protected set; } = DEFAULT_HOST;

        /// <summary>
        /// Port of the server to connect to
        /// </summary>
        public ushort Port { get; protected set; } = DEFAULT_PORT;

        /// <summary>
        /// The font family to use on the client
        /// </summary>
        public string Font { get; protected set; } = DEFAULT_FONT;

        /// <summary>
        /// Number of lines to save for chat scrollback
        /// </summary>
        public int ChatLines { get; protected set; } = DEFAULT_CHAT_LINES;

        /// <summary>
        /// Menu music file name
        /// </summary>
        public string MenuMusic { get; protected set; } = "";

        /// <summary>
        /// Menu background art
        /// </summary>
        public string MenuBackground { get; protected set; } = DEFAULT_MENU_BACKGROUND;

        // TODO: What is this for?
        public List<string> IntroImages { get; protected set; } = new List<string>();

        #endregion

        #region Serialization Hooks

        [OnDeserializing]
        internal void OnDeserializing(StreamingContext context)
        {
            IntroImages?.Clear();
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            Validate();
        }

        #endregion

        /// <inheritdoc />
        public ClientConfiguration Load(string filePath = DefaultPath, bool failQuietly = false) =>
            ConfigurationHelper.Load(this, filePath, failQuietly);

        /// <inheritdoc />
        public ClientConfiguration Save(string filePath = DefaultPath, bool failQuietly = false) =>
            ConfigurationHelper.Save(this, filePath, failQuietly);

        [NotNull]
        public static ClientConfiguration LoadAndSave([CanBeNull] string filePath = null) =>
            ConfigurationHelper.LoadSafely(Instance, filePath);

    }
}
