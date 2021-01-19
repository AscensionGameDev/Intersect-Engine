
using Newtonsoft.Json;

using NUnit.Framework;

using Semver;

using System.IO;

namespace Intersect.Plugins.Manifests
{

    [TestFixture]
    public class JsonManifestTests
    {

        private static TResourceType LoadResource<TResourceType>(
            string resourceName,
            TResourceType resource = default
        )
        {
            var filename = $@"{VirtualTestManifest.Namespace}.{resourceName}";
            using (var stream = typeof(JsonManifestTests).Assembly.GetManifestResourceStream(filename))
            {
                if (stream == null)
                {
                    Assert.Fail("Could not load resources for testing, is it marked as an embedded resource?");
                }

                using (var reader = new StreamReader(stream))
                {
                    var contents = reader.ReadToEnd();
                    if (resource != null)
                    {
                        JsonConvert.PopulateObject(contents, resource);
                    }
                    else
                    {
                        resource = JsonConvert.DeserializeObject<TResourceType>(contents);
                    }

                    return resource;
                }
            }
        }

        [Test]
        public void CanBeDeserialized_WellFormed()
        {
            var manifest = LoadResource<JsonManifest>("manifest.well-formed.json");
            Assert.NotNull(manifest);
            Assert.AreEqual("Test Manifest", manifest.Name);
            Assert.AreEqual("AscensionGameDev.Intersect.Tests", manifest.Key);
            Assert.AreEqual(new SemVersion(1), manifest.Version);
            Assert.AreEqual("https://github.com/AscensionGameDev/Intersect-Engine", manifest.Homepage);
        }

        [Test]
        public void CanBeDeserialized_CaseInsensitive()
        {
            var manifest = LoadResource<JsonManifest>("manifest.lowercase.json");
            Assert.NotNull(manifest);
            Assert.AreEqual("Test Manifest", manifest.Name);
            Assert.AreEqual("AscensionGameDev.Intersect.Tests", manifest.Key);
            Assert.AreEqual(new SemVersion(1), manifest.Version);
            Assert.AreEqual("https://github.com/AscensionGameDev/Intersect-Engine", manifest.Homepage);
        }

        [Test]
        public void CanBePopulated_WellFormed()
        {
            var manifest = new JsonManifest();
            LoadResource("manifest.well-formed.json", manifest);
            Assert.AreEqual("Test Manifest", manifest.Name);
            Assert.AreEqual("AscensionGameDev.Intersect.Tests", manifest.Key);
            Assert.AreEqual(new SemVersion(1), manifest.Version);
            Assert.AreEqual("https://github.com/AscensionGameDev/Intersect-Engine", manifest.Homepage);
        }

        [Test]
        public void CanBePopulated_CaseInsensitive()
        {
            var manifest = new JsonManifest();
            LoadResource("manifest.lowercase.json", manifest);
            Assert.AreEqual("Test Manifest", manifest.Name);
            Assert.AreEqual("AscensionGameDev.Intersect.Tests", manifest.Key);
            Assert.AreEqual(new SemVersion(1), manifest.Version);
            Assert.AreEqual("https://github.com/AscensionGameDev/Intersect-Engine", manifest.Homepage);
        }

    }

}
