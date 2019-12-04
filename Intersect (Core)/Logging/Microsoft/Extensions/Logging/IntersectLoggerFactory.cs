using Intersect.Collections;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

namespace Intersect.Logging.Microsoft.Extensions.Logging
{
    public class IntersectLoggerFactory : ILoggerFactory
    {

        public static IntersectLoggerFactory Create([NotNull] string name) => Instances.GetInstance(name);

        private static IntersectLoggerFactory DoCreateFactory() => new IntersectLoggerFactory();

        [NotNull]
        public static NamedInstanceStore<IntersectLoggerFactory> Instances { get; } =
            new NamedInstanceStore<IntersectLoggerFactory>(DoCreateFactory);

        private IntersectLoggerFactory()
        {
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public global::Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName) =>
            new IntersectLogger(categoryName);

        /// <inheritdoc />
        public void AddProvider(ILoggerProvider provider)
        {
        }

    }
}
