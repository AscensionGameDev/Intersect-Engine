using CommandLine;

using System.Collections.Generic;
using System.IO;
using System.Linq;

using Intersect.Client.Framework.Graphics;
using Intersect.Core;

namespace Intersect.Client.Core
{
    internal struct ClientCommandLineOptions : ICommandLineOptions
    {
        public ClientCommandLineOptions(
            bool borderlessWindow,
            int screenWidth,
            int screenHeight,
            string server,
            string workingDirectory,
            IEnumerable<string> pluginDirectories
        )
        {
            BorderlessWindow = borderlessWindow;
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
            Server = server;
            WorkingDirectory = workingDirectory;
            PluginDirectories = pluginDirectories?.Select(Path.GetFullPath).ToArray();
        }

        [Option("borderless", Default = false, Required = false)]
        public bool BorderlessWindow { get; }

        [Option("screen-width", Default = 0, Required = false)]
        public int ScreenWidth { get; }

        [Option("screen-height", Default = 0, Required = false)]
        public int ScreenHeight { get; }

        [Option('S', "server", Default = null, Required = false)]
        public string Server { get; }

        [Option("working-directory", Default = null, Required = false)]
        public string WorkingDirectory { get; }

        [Option('p',  "plugin-directory", Default = null, Required = false)]
        public IEnumerable<string> PluginDirectories { get; }

        public Resolution ScreenResolution => new Resolution(ScreenWidth, ScreenHeight);

    }
}
