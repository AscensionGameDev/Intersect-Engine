using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Intersect.Config
{
    /// <summary>
    /// Client configuration options
    /// </summary>
    public class ClientOptions
    {

        #region Constants

        public const string DEFAULT_HOST = "localhost";

        public const int DEFAULT_PORT = 5400;

        public const string DEFAULT_FONT = "arial";

        public const int DEFAULT_CHAT_LINES = 100;

        public const string DEFAULT_MENU_BACKGROUND = "background.png";

        #endregion

        #region Static Properties and Methods

        [NotNull] public static ClientOptions Instance { get; private set; } = new ClientOptions();

        public static void LoadFrom(string json)
        {
            Instance = (string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<ClientOptions>(json)) ?? Instance;
        }

        public static string ToJson()
        {
            return JsonConvert.SerializeObject(Instance, Formatting.Indented);
        }

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

    }
}
