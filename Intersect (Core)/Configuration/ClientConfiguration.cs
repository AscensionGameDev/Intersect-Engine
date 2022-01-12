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

        public const bool DEFAULT_MENUBG_STRETCH = true;

        public const bool DEFAULT_ANIM_MENUBG_TOGGLE = false;

        public const bool DEFAULT_ANIM_MENUBG_STRETCH = true;

        public const string DEFAULT_ANIM_MENUBG_TEXTURE = "main_menu";

        public const int DEFAULT_ANIM_MENUBG_FRAMECOUNT = 11;

        public const long DEFAULT_ANIM_MENUBG_FRAMESPEED = 50;

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
        /// Hostname of the server that the client will connect to.
        /// </summary>
        public string Host { get; set; } = DEFAULT_HOST;

        /// <summary>
        /// Port of the server that the client will connect to.
        /// </summary>
        public ushort Port { get; set; } = DEFAULT_PORT;

        /// <summary>
        /// The font family to use on misc non-ui rendering.
        /// </summary>
        public string GameFont { get; set; } = DEFAULT_FONT;

        /// <summary>
        /// The font family to use on entity names.
        /// </summary>
        public string EntityNameFont { get; set; } = DEFAULT_FONT;

        /// <summary>
        /// The font family to use on chat bubbles.
        /// </summary>
        public string ChatBubbleFont { get; set; } = DEFAULT_FONT;

        /// <summary>
        /// The font family to use on action messages.
        /// </summary>
        public string ActionMsgFont { get; set; } = DEFAULT_FONT;

        /// <summary>
        /// The font family to use on un-styled windows such as the debug menu/admin window.
        /// </summary>
        public string UIFont { get; set; } = DEFAULT_UI_FONT;

        /// <summary>
        /// Number of lines to save for chat scrollback.
        /// </summary>
        public int ChatLines { get; set; } = DEFAULT_CHAT_LINES;

        /// <summary>
        /// Sets a music file to be played in the main menu.
        /// </summary>
        public string MenuMusic { get; set; } = DEFAULT_MENU_MUSIC;

        /// <summary>
        /// Sets a list from which introductory images will be drawn when the game client starts.
        /// </summary>
        public List<string> IntroImages { get; set; } = new List<string>();

        /// <summary>
        /// Sets a texture to be used as static background in the main menu.
        /// </summary>
        public string MenuBackground { get; set; } = DEFAULT_MENU_BACKGROUND;

        /// <summary>
        /// Toggles the ability to stretch the static background in the main menu.
        /// </summary>
        public bool MenuBackgroundStretched { get; set; } = DEFAULT_MENUBG_STRETCH;

        /// <summary>
        /// Toggles the ability to draw an animated background in the main menu.
        /// When enabled, the static background in the main menu won't be drawn.
        /// </summary>
        public bool AnimatedMenuBgEnabled { get; set; } = DEFAULT_ANIM_MENUBG_TOGGLE;

        /// <summary>
        /// Toggles the ability to stretch the animated background in the main menu.
        /// </summary>
        public bool AnimatedMenuBgStretched { get; set; } = DEFAULT_ANIM_MENUBG_STRETCH;

        /// <summary>
        /// Sets the base name for the files to be used as frames in the animated background of the main menu.
        /// ie: if you set "main_menu", you will need to place your animation frames inside the animation folder
        /// and name them like this:  "main_menu_0.png, main_menu_1.png, main_menu_2.png, etc".
        /// </summary>
        public string AnimatedMenuBgTexture { get; set; } = DEFAULT_ANIM_MENUBG_TEXTURE;

        /// <summary>
        /// Sets the frame count of the animated background in the main menu.
        /// Make sure to set this value depending on the amount of files to be used from the animation resources.
        /// </summary>
        public int AnimatedMenuBgFrameCount { get; set; } = DEFAULT_ANIM_MENUBG_FRAMECOUNT;

        /// <summary>
        /// Sets the frame speed (milliseconds) of the animated background in the main menu.
        /// </summary>
        public long AnimatedMenuBgFrameSpeed { get; set; } = DEFAULT_ANIM_MENUBG_FRAMESPEED;

        /// <summary>
        /// Optional - built in updater system.
        /// Sets an URL to Package Updates (ie: https://freemmorpgmaker.com/updater/update.json).
        /// </summary>
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
