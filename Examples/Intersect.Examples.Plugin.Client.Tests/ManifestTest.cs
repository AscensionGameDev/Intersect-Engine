using Intersect.Plugins.Loaders;
using NUnit.Framework;

namespace Intersect.Examples.Plugin.Client.Tests;

[TestFixture]
public class ManifestTest
{
    [Test]
    public void TestManifestDetection()
    {
        var manifestType = typeof(Manifest);
        var pluginAssembly = manifestType.Assembly;
        var manifest = ManifestLoader.FindManifest(pluginAssembly);
        Assert.NotNull(manifestType);
        Assert.IsInstanceOf(manifestType, manifest);
    }
}