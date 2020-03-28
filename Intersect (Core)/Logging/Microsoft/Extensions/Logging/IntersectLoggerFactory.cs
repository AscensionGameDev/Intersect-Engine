using Intersect.Collections;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

namespace Intersect.Logging.Microsoft.Extensions.Logging
{

    public class IntersectLoggerFactory : ILoggerFactory
    {

        private IntersectLoggerFactory()
        {
        }

        [NotNull]
        public static NamedInstanceStore<IntersectLoggerFactory> Instances { get; } =
            new NamedInstanceStore<IntersectLoggerFactory>(DoCreateFactory);

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public global::Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
        {
            return new IntersectLogger(categoryName);
        }

        /// <inheritdoc />
        public void AddProvider(ILoggerProvider provider)
        {
        }

        public static IntersectLoggerFactory Create([NotNull] string name)
        {
            return Instances.GetInstance(name);
        }

        private static IntersectLoggerFactory DoCreateFactory()
        {
            return new IntersectLoggerFactory();
        }

    }

}
