using JetBrains.Annotations;

using System;

namespace Intersect.Client.Framework.Content
{

    [AttributeUsage(AttributeTargets.Field)]
    public class AssetTypeAttribute : Attribute
    {

        public AssetTypeAttribute([NotNull] Type type)
        {
            if (!typeof(IAsset).IsAssignableFrom(type))
            {
                throw new ArgumentException($@"Invalid asset type {type.FullName}. Must inherit from {nameof(IAsset)}.", nameof(type));
            }

            if (!type.IsClass)
            {
                throw new ArgumentException($@"Invalid asset type {type.FullName}. Must be a class (abstract is acceptable).", nameof(type));
            }

            Type = type;
        }

        [NotNull] public Type Type { get; }

    }

}
