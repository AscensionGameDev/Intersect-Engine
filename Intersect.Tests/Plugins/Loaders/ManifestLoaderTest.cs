using Intersect.Logging;
using Intersect.Plugins.Manifests;

using Moq;

using NUnit.Framework;

using Semver;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Intersect.Plugins.Interfaces;
using Intersect.Plugins.Manifests.Types;

namespace Intersect.Plugins.Loaders
{
    internal class MockAssembly : Assembly
    {
        public override string FullName => nameof(MockAssembly);

        public Exception ExceptionGetTypes { get; set; } = null;

        public Exception ExceptionGetManifestResourceInfo { get; set; } = null;

        public Exception ExceptionGetManifestResourceStream { get; set; } = null;

        public Type[] MockTypes { get; set; } = Array.Empty<Type>();

        public Dictionary<string, ManifestResourceInfo> MockManifestResourceInfo { get; set; } =
            new Dictionary<string, ManifestResourceInfo>();

        public Dictionary<string, Stream> MockManifestResourceStream { get; set; } = new Dictionary<string, Stream>();

        public override Type[] GetTypes() => ExceptionGetTypes != null ? throw ExceptionGetTypes : MockTypes;

        public override ManifestResourceInfo GetManifestResourceInfo(string resourceName) =>
            ExceptionGetManifestResourceInfo != null
                ? throw ExceptionGetManifestResourceInfo
                : (MockManifestResourceInfo.ContainsKey(resourceName)
                    ? MockManifestResourceInfo[resourceName]
                    : default);

        public override Stream GetManifestResourceStream(string name) => ExceptionGetManifestResourceStream != null
            ? throw ExceptionGetManifestResourceStream
            : (MockManifestResourceStream.ContainsKey(name) ? MockManifestResourceStream[name] : default);
    }

    internal interface IllegalVirtualManifestInterface
    {
    }

    internal abstract class IllegalVirtualManifestAbstractClass
    {
    }

    internal class IllegalVirtualManifestGenericClass<T>
    {
    }

    internal class IllegalVirtualManifestDefinedClass
    {
    }

    internal class IllegalVirtualManifestNoSupportedConstructorsClass : IManifestHelper
    {
        public IllegalVirtualManifestNoSupportedConstructorsClass(string name)
        {
            Name = name;
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string Key { get; }

        /// <inheritdoc />
        public SemVersion Version { get; }

        /// <inheritdoc />
        public Authors Authors { get; }

        /// <inheritdoc />
        public string Homepage { get; }
    }

    internal struct VirtualManifestValueType : IManifestHelper
    {
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string Key { get; }

        /// <inheritdoc />
        public SemVersion Version { get; }

        /// <inheritdoc />
        public Authors Authors { get; }

        /// <inheritdoc />
        public string Homepage { get; }
    }

    [TestFixture]
    public class ManifestLoaderTest
    {
        [SetUp]
        public void SetUp()
        {
            ManifestLoader.ManifestLoaderDelegates.Clear();
        }

        [Test]
        public void IsVirtualManifestType()
        {
            Assert.IsFalse(ManifestLoader.IsVirtualManifestType(typeof(IllegalVirtualManifestAbstractClass)));
            Assert.IsFalse(ManifestLoader.IsVirtualManifestType(typeof(IllegalVirtualManifestDefinedClass)));
            Assert.IsFalse(ManifestLoader.IsVirtualManifestType(typeof(IllegalVirtualManifestGenericClass<string>)));
            Assert.IsFalse(ManifestLoader.IsVirtualManifestType(typeof(IllegalVirtualManifestInterface)));
            Assert.IsFalse(
                ManifestLoader.IsVirtualManifestType(typeof(IllegalVirtualManifestNoSupportedConstructorsClass))
            );

            Assert.IsTrue(ManifestLoader.IsVirtualManifestType(typeof(VirtualManifestValueType)));
            Assert.IsTrue(ManifestLoader.IsVirtualManifestType(typeof(VirtualTestManifest)));
        }

        [Test]
        public void FindManifest_LoadsJsonManifest_WellFormed()
        {
            ManifestLoader.ManifestLoaderDelegates.Add(ManifestLoader.LoadJsonManifestFrom);
            var mockAssembly = new MockAssembly
            {
                MockManifestResourceInfo = new Dictionary<string, ManifestResourceInfo>
                {
                    {
                        @"manifest.json",
                        typeof(ManifestLoaderTest).Assembly.GetManifestResourceInfo(
                            $@"{VirtualTestManifest.Namespace}.manifest.well-formed.json"
                        )
                    }
                },
                MockManifestResourceStream = new Dictionary<string, Stream>
                {
                    {
                        @"manifest.json",
                        typeof(ManifestLoaderTest).Assembly.GetManifestResourceStream(
                            $@"{VirtualTestManifest.Namespace}.manifest.well-formed.json"
                        )
                    }
                }
            };

            var manifest = ManifestLoader.FindManifest(mockAssembly);
            Assert.IsTrue(manifest is JsonManifest);
            Assert.NotNull(manifest);
            Assert.AreEqual("Test Manifest", manifest.Name);
            Assert.AreEqual("AscensionGameDev.Intersect.Tests", manifest.Key);
            Assert.AreEqual(new SemVersion(1), manifest.Version);
            Assert.AreEqual("https://github.com/AscensionGameDev/Intersect-Engine", manifest.Homepage);
        }

        [Test]
        public void FindManifest_ReturnsNullWhenNoManifestsFound()
        {
            ManifestLoader.ManifestLoaderDelegates.Add(ManifestLoader.LoadJsonManifestFrom);
            ManifestLoader.ManifestLoaderDelegates.Add(ManifestLoader.LoadVirtualManifestFrom);
            var mockAssembly = new MockAssembly
            {
            };

            var manifest = ManifestLoader.FindManifest(mockAssembly);
            Assert.Null(manifest);
        }

        [Test]
        public void FindManifest_LogsErrorsIfAnExceptionIsThrownByDelegate()
        {
            var mockLogger = new Mock<Logger>();
            Log.Default = mockLogger.Object;
            var mockException = new Exception("Delegate exception");
            ManifestLoader.ManifestLoaderDelegates.Add((Assembly assembly) => throw mockException);
            var mockAssembly = new MockAssembly
            {
            };

            var manifest = ManifestLoader.FindManifest(mockAssembly);
            Assert.Null(manifest);
            mockLogger.Verify(
                l => l.Error(
                    It.Is<Exception>(e => e.Message == mockException.Message),
                    "Exception thrown by manifest loader delegate."
                )
            );
        }

        [Test]
        public void FindManifest_LoadsJsonManifest_Lowercase()
        {
            ManifestLoader.ManifestLoaderDelegates.Add(ManifestLoader.LoadJsonManifestFrom);
            var mockAssembly = new MockAssembly
            {
                MockManifestResourceInfo = new Dictionary<string, ManifestResourceInfo>
                {
                    {
                        @"manifest.json",
                        typeof(ManifestLoaderTest).Assembly.GetManifestResourceInfo(
                            $@"{VirtualTestManifest.Namespace}.manifest.lowercase.json"
                        )
                    }
                },
                MockManifestResourceStream = new Dictionary<string, Stream>
                {
                    {
                        @"manifest.json",
                        typeof(ManifestLoaderTest).Assembly.GetManifestResourceStream(
                            $@"{VirtualTestManifest.Namespace}.manifest.lowercase.json"
                        )
                    }
                }
            };

            var manifest = ManifestLoader.FindManifest(mockAssembly);
            Assert.IsTrue(manifest is JsonManifest);
            Assert.NotNull(manifest);
            Assert.AreEqual("Test Manifest", manifest.Name);
            Assert.AreEqual("AscensionGameDev.Intersect.Tests", manifest.Key);
            Assert.AreEqual(new SemVersion(1), manifest.Version);
            Assert.AreEqual("https://github.com/AscensionGameDev/Intersect-Engine", manifest.Homepage);
        }

        [Test]
        public void FindManifest_LoadsVirtualManifest()
        {
            ManifestLoader.ManifestLoaderDelegates.Add(ManifestLoader.LoadVirtualManifestFrom);
            var mockAssembly = new MockAssembly
            {
                MockTypes = new[] {typeof(VirtualTestManifest)}
            };

            var manifest = ManifestLoader.FindManifest(mockAssembly);
            Assert.IsTrue(manifest is VirtualTestManifest);
            Assert.NotNull(manifest);
            Assert.AreEqual("Test Manifest", manifest.Name);
            Assert.AreEqual("AscensionGameDev.Intersect.Tests", manifest.Key);
            Assert.AreEqual(new SemVersion(1), manifest.Version);
            Assert.AreEqual("https://github.com/AscensionGameDev/Intersect-Engine", manifest.Homepage);
        }

        [Test]
        public void FindManifest_LoadsVirtualManifestWhenJsonManifestNotFound()
        {
            ManifestLoader.ManifestLoaderDelegates.Add(ManifestLoader.LoadJsonManifestFrom);
            ManifestLoader.ManifestLoaderDelegates.Add(ManifestLoader.LoadVirtualManifestFrom);
            var mockAssembly = new MockAssembly
            {
                MockTypes = new[] {typeof(VirtualTestManifest)}
            };

            var manifest = ManifestLoader.FindManifest(mockAssembly);
            Assert.IsTrue(manifest is VirtualTestManifest);
            Assert.NotNull(manifest);
            Assert.AreEqual("Test Manifest", manifest.Name);
            Assert.AreEqual("AscensionGameDev.Intersect.Tests", manifest.Key);
            Assert.AreEqual(new SemVersion(1), manifest.Version);
            Assert.AreEqual("https://github.com/AscensionGameDev/Intersect-Engine", manifest.Homepage);
        }

        [Test]
        public void FindManifest_LoadsJsonManifestWhenFoundInsteadOfVirtualManifest()
        {
            ManifestLoader.ManifestLoaderDelegates.Add(ManifestLoader.LoadJsonManifestFrom);
            ManifestLoader.ManifestLoaderDelegates.Add(ManifestLoader.LoadVirtualManifestFrom);
            var mockAssembly = new MockAssembly
            {
                MockTypes = new[] {typeof(VirtualTestManifest)},
                MockManifestResourceInfo = new Dictionary<string, ManifestResourceInfo>
                {
                    {
                        @"manifest.json",
                        typeof(ManifestLoaderTest).Assembly.GetManifestResourceInfo(
                            $@"{VirtualTestManifest.Namespace}.manifest.lowercase.json"
                        )
                    }
                },
                MockManifestResourceStream = new Dictionary<string, Stream>
                {
                    {
                        @"manifest.json",
                        typeof(ManifestLoaderTest).Assembly.GetManifestResourceStream(
                            $@"{VirtualTestManifest.Namespace}.manifest.lowercase.json"
                        )
                    }
                }
            };

            var manifest = ManifestLoader.FindManifest(mockAssembly);
            Assert.IsTrue(manifest is JsonManifest);
            Assert.NotNull(manifest);
            Assert.AreEqual("Test Manifest", manifest.Name);
            Assert.AreEqual("AscensionGameDev.Intersect.Tests", manifest.Key);
            Assert.AreEqual(new SemVersion(1), manifest.Version);
            Assert.AreEqual("https://github.com/AscensionGameDev/Intersect-Engine", manifest.Homepage);
        }

        [Test]
        public void FindManifest_LoadsVirtualManifestWhenFoundInsteadOfJsonManifest()
        {
            ManifestLoader.ManifestLoaderDelegates.Add(ManifestLoader.LoadVirtualManifestFrom);
            ManifestLoader.ManifestLoaderDelegates.Add(ManifestLoader.LoadJsonManifestFrom);
            var mockAssembly = new MockAssembly
            {
                MockTypes = new[] {typeof(VirtualTestManifest)},
                MockManifestResourceInfo = new Dictionary<string, ManifestResourceInfo>
                {
                    {
                        @"manifest.json",
                        typeof(ManifestLoaderTest).Assembly.GetManifestResourceInfo(
                            $@"{VirtualTestManifest.Namespace}.manifest.lowercase.json"
                        )
                    }
                },
                MockManifestResourceStream = new Dictionary<string, Stream>
                {
                    {
                        @"manifest.json",
                        typeof(ManifestLoaderTest).Assembly.GetManifestResourceStream(
                            $@"{VirtualTestManifest.Namespace}.manifest.lowercase.json"
                        )
                    }
                }
            };

            var manifest = ManifestLoader.FindManifest(mockAssembly);
            Assert.IsTrue(manifest is VirtualTestManifest);
            Assert.NotNull(manifest);
            Assert.AreEqual("Test Manifest", manifest.Name);
            Assert.AreEqual("AscensionGameDev.Intersect.Tests", manifest.Key);
            Assert.AreEqual(new SemVersion(1), manifest.Version);
            Assert.AreEqual("https://github.com/AscensionGameDev/Intersect-Engine", manifest.Homepage);
        }

        [Test]
        public void FindManifest_ThrowsIfNoDelegates()
        {
            Assert.Throws<InvalidOperationException>(
                () => ManifestLoader.FindManifest(new MockAssembly()),
                $"{nameof(ManifestLoader.ManifestLoaderDelegates)} was initialized with no pre-registered delegates, or the pre-defined delegates were removed and no alternatives were added."
            );
        }

        [Test]
        public void LoadJsonManifestFrom_ReturnsNullIfNoManifest()
        {
            Assert.IsNull(ManifestLoader.LoadJsonManifestFrom(new MockAssembly()));
        }

        [Test]
        public void LoadJsonManifestFrom_ReturnsNullIfExceptionThrownFromResourceInfo()
        {
            var mockLogger = new Mock<Logger>();
            Log.Default = mockLogger.Object;
            var mockException = new Exception(nameof(MockAssembly.ExceptionGetManifestResourceInfo));
            Assert.IsNull(
                ManifestLoader.LoadJsonManifestFrom(
                    new MockAssembly
                    {
                        ExceptionGetManifestResourceInfo = mockException
                    }
                )
            );

            mockLogger.Verify(l => l.Warn(mockException, "Failed to load manifest.json from MockAssembly."));
        }

        [Test]
        public void LoadJsonManifestFrom_ReturnsNullIfStreamIsNull()
        {
            var mockLogger = new Mock<Logger>();
            Log.Default = mockLogger.Object;
            var mockException = new InvalidDataException("Manifest resource stream null when info exists.");
            Assert.IsNull(
                ManifestLoader.LoadJsonManifestFrom(
                    new MockAssembly
                    {
                        MockManifestResourceInfo = new Dictionary<string, ManifestResourceInfo>
                        {
                            {
                                @"manifest.json",
                                typeof(ManifestLoaderTest).Assembly.GetManifestResourceInfo(
                                    $@"{VirtualTestManifest.Namespace}.manifest.lowercase.json"
                                )
                            }
                        }
                    }
                )
            );

            mockLogger.Verify(
                l => l.Warn(
                    It.Is<InvalidDataException>(e => e.Message == mockException.Message),
                    "Failed to load manifest.json from MockAssembly."
                )
            );
        }

        [Test]
        public void LoadJsonManifestFrom_ReturnsNullIfStreamIsEmpty()
        {
            var mockLogger = new Mock<Logger>();
            Log.Default = mockLogger.Object;
            var mockException = new InvalidDataException("Manifest is empty or failed to load and is null.");
            Assert.IsNull(
                ManifestLoader.LoadJsonManifestFrom(
                    new MockAssembly
                    {
                        MockManifestResourceInfo = new Dictionary<string, ManifestResourceInfo>
                        {
                            {
                                @"manifest.json",
                                typeof(ManifestLoaderTest).Assembly.GetManifestResourceInfo(
                                    $@"{VirtualTestManifest.Namespace}.manifest.lowercase.json"
                                )
                            }
                        },
                        MockManifestResourceStream = new Dictionary<string, Stream>
                        {
                            {@"manifest.json", new MemoryStream()}
                        }
                    }
                )
            );

            mockLogger.Verify(
                l => l.Warn(
                    It.Is<InvalidDataException>(e => e.Message == mockException.Message),
                    "Failed to load manifest.json from MockAssembly."
                )
            );
        }

        [Test]
        public void LoadJsonManifestFrom_ReturnsNullIfExceptionThrownFromResourceStream()
        {
            var mockLogger = new Mock<Logger>();
            Log.Default = mockLogger.Object;
            var mockException = new Exception(nameof(MockAssembly.ExceptionGetManifestResourceStream));
            Assert.IsNull(
                ManifestLoader.LoadJsonManifestFrom(
                    new MockAssembly
                    {
                        ExceptionGetManifestResourceStream = mockException,
                        MockManifestResourceInfo = new Dictionary<string, ManifestResourceInfo>
                        {
                            {
                                @"manifest.json",
                                typeof(ManifestLoaderTest).Assembly.GetManifestResourceInfo(
                                    $@"{VirtualTestManifest.Namespace}.manifest.lowercase.json"
                                )
                            }
                        }
                    }
                )
            );

            mockLogger.Verify(l => l.Warn(mockException, "Failed to load manifest.json from MockAssembly."));
        }

        [Test]
        public void LoadVirtualManifestFrom_ReturnsNullIfNoTypes()
        {
            Assert.IsNull(ManifestLoader.LoadVirtualManifestFrom(new MockAssembly()));
        }

        [Test]
        public void LoadVirtualManifestFrom_ReturnsNullIfExceptionThrown()
        {
            var mockLogger = new Mock<Logger>();
            Log.Default = mockLogger.Object;
            var mockException = new Exception(nameof(MockAssembly.ExceptionGetTypes));
            Assert.IsNull(
                ManifestLoader.LoadVirtualManifestFrom(
                    new MockAssembly
                    {
                        ExceptionGetTypes = mockException
                    }
                )
            );

            mockLogger.Verify(l => l.Warn(mockException, "Failed to load virtual manifest from MockAssembly."));
        }
    }
}
