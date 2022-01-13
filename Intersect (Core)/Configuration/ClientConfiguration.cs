using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Intersect.Enums;

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

        public const string DEFAULT_MENU_BACKGROUND = "background";

        public const DisplayModes DEFAULT_MENU_BACKGROUND_DISPLAY_MODE = DisplayModes.Default;

        public const bool DEFAULT_MENU_BACKGROUND_ANIMATED = false;

        public const long DEFAULT_MENU_BACKGROUND_FRAME_INTERVAL = 50;

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

        /// <summary>
        /// Sets the display mode of the background in the main menu.
        /// </summary>
        public DisplayModes MenuBackgroundDisplayMode { get; set; } = DEFAULT_MENU_BACKGROUND_DISPLAY_MODE;

        /// <summary>
        /// Toggles the ability to draw an animated background in the main menu.
        /// Example: when enabled, if you set "background" as MenuBackground, you will need to
        /// place your animation frames inside the animation resources folder and name them
        /// sequentially like this:  "background_0.png", "background_1.png", etc..
        /// </summary>
        public bool MenuBackgroundAnimated { get; set; } = DEFAULT_MENU_BACKGROUND_ANIMATED;

        /// <summary>
        /// Sets the frames interval (milliseconds) of the animated background in the main menu.
        /// </summary>
        public long MenuBackgroundFrameInterval { get; set; } = DEFAULT_MENU_BACKGROUND_FRAME_INTERVAL;

        // TODO: What is this for?
        public List<string> IntroImages { get; set; } = new List<string>();

        public string UpdateUrl { get; set; } = "";

        /// <summary>
        /// Sets a custom mouse cursor.
        /// </summary>
        public string MouseCursor { get; set; } = "";

        /// <summary>
        /// Determines the time it takes to fade-in or fade-out a song when no other instructions are given.
        /// </summary>
        public int MusicFadeTimer { get; set; } = 1500;

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
