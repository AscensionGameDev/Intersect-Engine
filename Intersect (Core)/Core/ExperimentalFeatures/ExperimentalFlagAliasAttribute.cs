using System;

namespace Intersect.Core.ExperimentalFeatures
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public partial class ExperimentalFlagAliasAttribute : Attribute
    {

        public ExperimentalFlagAliasAttribute(string of)
        {
            if (string.IsNullOrWhiteSpace(of))
            {
                throw new ArgumentNullException(nameof(of));
            }

            Of = of;
        }

        public string Of { get; }

    }

}
