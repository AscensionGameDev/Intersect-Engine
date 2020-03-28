using System;

using JetBrains.Annotations;

namespace Intersect.Core.ExperimentalFeatures
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ExperimentalFlagAliasAttribute : Attribute
    {

        public ExperimentalFlagAliasAttribute([NotNull] string of)
        {
            if (string.IsNullOrWhiteSpace(of))
            {
                throw new ArgumentNullException(nameof(of));
            }

            Of = of;
        }

        [NotNull]
        public string Of { get; }

    }

}
