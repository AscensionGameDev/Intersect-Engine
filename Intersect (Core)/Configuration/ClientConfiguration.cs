using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Intersect.Configuration
{

    /// <inheritdoc />
    /// <summary>
    /// Client configuration options
    /// </summary>
    public sealed class ClientConfiguration : IConfiguration<ClientConfiguration>
    {

        public const string DefaultPath = @"resources/config.json";

        /// <inheritdoc />
        public ClientConfiguration Load(string filePath = DefaultPath, bool failQuietly = false)
        {
            return ConfigurationHelper.Load(this, filePath, failQuietly);
        }

        /// <inheritdoc />
        public ClientConfiguration Save(string filePath = DefaultPath, bool failQuietly = false)
        {
            return ConfigurationHelper.Save(this, filePath, failQuietly);
        }

        public static ClientConfiguration LoadAndSave(string filePath = null)
        {
            return ConfigurationHelper.LoadSafely(Instance, filePath);
        }

        #region Constants

        public const string DEFAULT_HOST = "localhost";

        public const int DEFAULT_PORT = 5400;

        public const string DEFAULT_FONT = "sourcesansproblack";

        public const string DEFAULT_UI_FONT = "sourcesanspro,8";

        public const int DEFAULT_CHAT_LINES = 100;

        public const string DEFAULT_MENU_BACKGROUND = "background.png";

        public const string DEFAULT_MENU_MUSIC = "RPG-Theme_v001_Looping.ogg";

        #endregion

        #region Static Properties and Methods

        public static ClientConfiguration Instance { get; } = new ClientConfiguration();

        public void Validate()
        {
            Host = string.IsNullOrWhiteSpace(Host) ? DEFAULT_HOST : Host.Trim();
            Port = Math.Min(Math.Max(Port, (ushort) 1), ushort.MaxValue);
            GameFont = string.IsNullOrWhiteSpace(GameFont) ? DEFAULT_FONT : GameFont.Trim();
            UIFont = string.IsNullOrWhiteSpace(UIFont) ? DEFAULT_UI_FONT : UIFont.Trim();
            ChatLines = Math.Min(Math.Max(ChatLines, 10), 500);
            IntroImages = new List<string>(IntroImages?.Distinct() ?? new List<string>());
        }

        #endregion

        #region Options

        /// <summary>
        /// Hostname of the server to connect to
        /// </summary>
        public string Host { get; set; } = DEFAULT_HOST;

        /// <summary>
        /// Port of the server to connect to
        /// </summary>
        public ushort Port { get; set; } = DEFAULT_PORT;

        /// <summary>
        /// The font family to use on misc non-ui rendering
        /// </summary>
        public string GameFont { get; set; } = DEFAULT_FONT;

        /// <summary>
        /// The font family to use on entity names
        /// </summary>
        public string EntityNameFont { get; set; } = DEFAULT_FONT;

        /// <summary>
        /// The font family to use on chat bubbles
        /// </summary>
        public string ChatBubbleFont { get; set; } = DEFAULT_FONT;

        /// <summary>
        /// The font family to use on action messages
        /// </summary>
        public string ActionMsgFont { get; set; } = DEFAULT_FONT;

        /// <summary>
        /// The font family to use on unstyled windows such as the debug menu/admin window
        /// </summary>
        public string UIFont { get; set; } = DEFAULT_UI_FONT;

        /// <summary>
        /// Number of lines to save for chat scrollback
        /// </summary>
        public int ChatLines { get; set; } = DEFAULT_CHAT_LINES;

        /// <summary>
        /// Menu music file name
        /// </summary>
        public string MenuMusic { get; set; } = DEFAULT_MENU_MUSIC;

        /// <summary>
        /// Menu background art
        /// </summary>
        public string MenuBackground { get; set; } = DEFAULT_MENU_BACKGROUND;

        // TODO: What is this for?
        public List<string> IntroImages { get; set; } = new List<string>();

        public string UpdateUrl { get; set; } = "";

        /// <summary>
        /// Sets a custom mouse cursor.
        /// </summary>
        public string MouseCursor { get; set; } = "";

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

    }

}
