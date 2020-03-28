using System;
using System.Collections.Generic;
using System.Reflection;

using JetBrains.Annotations;

namespace Intersect.Core.ExperimentalFeatures
{

    public abstract partial class CommonExperiments<TExperiments> where TExperiments : CommonExperiments<TExperiments>
    {

        public const string CONFIG_PATH = "resources/config/experiments.config.json";

        [NotNull] private static readonly IDictionary<Type, Guid> mNamespaceIdsByDeclaringType;

        private static TExperiments mInstance;

        static CommonExperiments()
        {
            mNamespaceIdsByDeclaringType = new Dictionary<Type, Guid>();
        }

        [NotNull]
        protected static TExperiments Instance
        {
            get => mInstance ??
                   throw new InvalidOperationException(
                       $@"Did you forget to set this in the static constructor for '{typeof(TExperiments).AssemblyQualifiedName}'."
                   );
            set => mInstance = value;
        }

        protected static Guid GetNamespaceIdFor([NotNull] Type declaringType)
        {
            if (mNamespaceIdsByDeclaringType.TryGetValue(declaringType, out var namespaceId))
            {
                return namespaceId;
            }

            var namespaceIdProperty = declaringType.GetField(
                nameof(NamespaceId), BindingFlags.Static | BindingFlags.NonPublic
            );

            if (namespaceIdProperty == null)
            {
                throw new InvalidOperationException();
            }

            // ReSharper disable once InvertIf
            if (namespaceIdProperty.GetValue(null) is Guid generatedNamespaceId)
            {
                mNamespaceIdsByDeclaringType[declaringType] = generatedNamespaceId;

                return generatedNamespaceId;
            }

            throw new InvalidOperationException();
        }

    }

}
