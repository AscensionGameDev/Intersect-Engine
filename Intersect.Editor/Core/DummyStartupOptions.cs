using Intersect.Core;

namespace Intersect.Editor.Core;

public sealed class DummyStartupOptions : ICommandLineOptions
{
    public string WorkingDirectory => Environment.CurrentDirectory;
    public IEnumerable<string> PluginDirectories => [];
}