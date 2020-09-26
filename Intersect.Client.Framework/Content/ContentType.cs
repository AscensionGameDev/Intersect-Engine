using System;

using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.Graphics;

namespace Intersect.Client.Framework.Content
{

    public enum ContentType
    {
        [AssetType(typeof(ITexture))]
        Animation,

        [AssetType(typeof(ITexture))]
        Entity,

        [AssetType(typeof(ITexture))]
        Face,

        [AssetType(typeof(ITexture))]
        Fog,

        [AssetType(typeof(IFont))]
        Font,

        [AssetType(typeof(ITexture))]
        Image,

        [AssetType(typeof(ITexture))]
        Interface,

        [AssetType(typeof(ITexture))]
        Item,

        [AssetType(typeof(ITexture))]
        Miscellaneous,

        [AssetType(typeof(IAudioSource))]
        Music,

        [AssetType(typeof(ITexture))]
        Paperdoll,

        [AssetType(typeof(ITexture))]
        Resource,

        [AssetType(typeof(IShader))]
        Shader,

        [AssetType(typeof(IAudioSource))]
        Sound,

        [AssetType(typeof(ITexture))]
        Spell,

        [AssetType(typeof(ITexture))]
        TexturePack,

        [AssetType(typeof(ITexture))]
        Tileset,

        [Obsolete] Gui = Interface

    }

}
