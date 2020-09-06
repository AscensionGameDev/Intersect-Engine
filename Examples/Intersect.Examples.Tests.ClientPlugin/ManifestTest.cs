// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using System.Collections;
using System.Collections.Generic;
using Intersect.Plugins.Loaders;
using NUnit.Framework;

namespace Intersect.Examples.ClientPlugin
{
    [TestFixture]
    public class ManifestTest
    {
        [Test]
        public void TestManifestDetection()
        {
            var manifestType = typeof(Manifest);
            var pluginAssembly = manifestType.Assembly;
            var coreAssembly = typeof(ManifestLoader).Assembly;
            var manifest = ManifestLoader.FindManifest(pluginAssembly);
            Assert.NotNull(manifestType);
            Assert.IsInstanceOf(manifestType, manifest);
        }
    }
}
