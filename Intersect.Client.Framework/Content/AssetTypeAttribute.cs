using System;

namespace Intersect.Client.Framework.Content
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AssetTypeAttribute : Attribute
    {
        public AssetTypeAttribute(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!typeof(IAsset).IsAssignableFrom(type))
            {
                throw new ArgumentException(
                    $@"Invalid asset type {type.FullName}. Must inherit from {nameof(IAsset)}.", nameof(type)
                );
            }

            Type = type;
        }

        public Type Type { get; }
    }
}
