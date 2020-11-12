using System;
using System.Linq;

namespace Intersect.Client.Framework.Content
{
    public static class ContentTypesExtensions
    {
        public static Type GetAssetType(this ContentType contentType)
        {
            var memberName = contentType.ToString();
            var memberInfo = typeof(ContentType).GetMember(memberName).FirstOrDefault();
            if (memberInfo == null)
            {
                throw new InvalidOperationException($@"{nameof(ContentType)} missing expected member: {memberName}");
            }

            var attribute = memberInfo.GetCustomAttributes(typeof(AssetTypeAttribute), true).FirstOrDefault();
            if (attribute is AssetTypeAttribute assetTypeAttribute)
            {
                return assetTypeAttribute.Type;
            }

            throw new InvalidOperationException(
                $@"{nameof(ContentType)} missing {nameof(AssetTypeAttribute)} on member: {memberName}"
            );
        }

        public static ContentType ToContentType(this TextureType textureType)
        {
            switch (textureType)
            {
                case TextureType.Tileset:
                    return ContentType.Tileset;

                case TextureType.Item:
                    return ContentType.Item;

                case TextureType.Entity:
                    return ContentType.Entity;

                case TextureType.Spell:
                    return ContentType.Spell;

                case TextureType.Animation:
                    return ContentType.Animation;

                case TextureType.Face:
                    return ContentType.Face;

                case TextureType.Image:
                    return ContentType.Image;

                case TextureType.Fog:
                    return ContentType.Fog;

                case TextureType.Resource:
                    return ContentType.Resource;

                case TextureType.Paperdoll:
                    return ContentType.Paperdoll;

                case TextureType.Interface:
                    return ContentType.Interface;

                case TextureType.Miscellaneous:
                    return ContentType.Miscellaneous;

                default:
                    throw new ArgumentOutOfRangeException(nameof(textureType), textureType, null);
            }
        }

        public static string GetDirectory(this ContentType contentType)
        {
            switch (contentType)
            {
                case ContentType.Animation:
                    return "animations";

                case ContentType.Entity:
                    return "entities";

                case ContentType.Face:
                    return "faces";

                case ContentType.Fog:
                    return "fogs";

                case ContentType.Font:
                    return "fonts";

                case ContentType.Image:
                    return "images";

                case ContentType.Interface:
                    return "gui";

                case ContentType.Item:
                    return "items";

                case ContentType.Miscellaneous:
                    return "misc";

                case ContentType.Music:
                    return "music";

                case ContentType.Paperdoll:
                    return "paperdolls";

                case ContentType.Resource:
                    return "resources";

                case ContentType.Shader:
                    return "shaders";

                case ContentType.Sound:
                    return "sounds";

                case ContentType.Spell:
                    return "spells";

                case ContentType.TexturePack:
                    return "packs";

                case ContentType.Tileset:
                    return "tilesets";

                default:
                    throw new ArgumentOutOfRangeException(nameof(contentType), contentType, null);
            }
        }

        public static string GetExtension(this ContentType contentType)
        {
            switch (contentType)
            {
                case ContentType.Animation:
                case ContentType.Entity:
                case ContentType.Face:
                case ContentType.Fog:
                case ContentType.Image:
                case ContentType.Interface:
                case ContentType.Item:
                case ContentType.Miscellaneous:
                case ContentType.Paperdoll:
                case ContentType.Resource:
                case ContentType.Spell:
                case ContentType.Tileset:
                    return "png";

                case ContentType.Font:
                case ContentType.Shader:
                    return "xnb";

                case ContentType.Music:
                    return "ogg";

                case ContentType.Sound:
                    return "wav";

                case ContentType.TexturePack:
                    return "json";

                default:
                    throw new ArgumentOutOfRangeException(nameof(contentType), contentType, null);
            }
        }
    }
}
