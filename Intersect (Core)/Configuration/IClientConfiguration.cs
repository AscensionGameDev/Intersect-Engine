using System.Collections.Generic;

namespace Intersect.Configuration
{
    /// <summary>
    /// Declares the API for client configuration.
    /// </summary>
    public interface IClientConfiguration : IConfiguration<ClientConfiguration>
    {
        /// <summary>
        /// Hostname of the server to connect to
        /// </summary>
        string Host { get; set; }

        /// <summary>
        /// Port of the server to connect to
        /// </summary>
        ushort Port { get; set; }

        /// <summary>
        /// The font family to use on misc non-ui rendering
        /// </summary>
        string GameFont { get; set; }

        /// <summary>
        /// The font family to use on entity names
        /// </summary>
        string EntityNameFont { get; set; }

        /// <summary>
        /// The font family to use on chat bubbles
        /// </summary>
        string ChatBubbleFont { get; set; }

        /// <summary>
        /// The font family to use on action messages
        /// </summary>
        string ActionMsgFont { get; set; }

        /// <summary>
        /// The font family to use on unstyled windows such as the debug menu/admin window
        /// </summary>
        string UIFont { get; set; }

        /// <summary>
        /// Number of lines to save for chat scrollback
        /// </summary>
        int ChatLines { get; set; }

        /// <summary>
        /// Menu music file name
        /// </summary>
        string MenuMusic { get; set; }

        /// <summary>
        /// Menu background art
        /// </summary>
        string MenuBackground { get; set; }

        /// <summary>
        /// Images to display on startup
        /// </summary>
        List<string> IntroImages { get; set; }

        /// <summary>
        /// Url of the update manifest
        /// </summary>
        string UpdateUrl { get; set; }
    }
}
