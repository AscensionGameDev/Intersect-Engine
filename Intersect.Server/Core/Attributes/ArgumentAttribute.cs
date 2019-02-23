using System;
using JetBrains.Annotations;

namespace Intersect.Server.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ArgumentAttribute : Attribute
    {
        [NotNull]
        public string Name { get; }

        public ArgumentAttribute([InvokerParameterName] string name)
        {
            Name = name;
        }
    }
}
