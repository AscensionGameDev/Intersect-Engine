using CommandLine;
using Intersect.Client.Framework.Graphics;
using Intersect.Core;

namespace Intersect.Client.Core;

public partial record ClientCommandLineOptions : ICommandLineOptions
{
    [Option("borderless", Default = false, Required = false)]
    public bool BorderlessWindow { get; init; }

    [Option("screen-width", Default = 0, Required = false)]
    public int ScreenWidth { get; init; }

    [Option("screen-height", Default = 0, Required = false)]
    public int ScreenHeight { get; init; }

    [Option('S', "server", Default = null, Required = false)]
    public string? Server { get; init; }

    [Option("working-directory", Default = null, Required = false)]
    public string? WorkingDirectory { get; init; }

    [Option('p', "plugin-directory", Default = null, Required = false)]
    public IEnumerable<string>? PluginDirectories { get; init; }

    public Resolution ScreenResolution => new(ScreenWidth, ScreenHeight);
}
