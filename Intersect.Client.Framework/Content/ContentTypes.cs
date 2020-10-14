using System;

using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.Graphics;

namespace Intersect.Client.Framework.Content
{

    public enum ContentTypes
    {
        [AssetType(typeof(GameTexture))]
        Animation,

        [AssetType(typeof(GameTexture))]
        Entity,

        [AssetType(typeof(GameTexture))]
        Face,

        [AssetType(typeof(GameTexture))]
        Fog,

        [AssetType(typeof(GameFont))]
        Font,

        [AssetType(typeof(GameTexture))]
        Image,

        [AssetType(typeof(GameTexture))]
        Interface,

        [AssetType(typeof(GameTexture))]
        Item,

        [AssetType(typeof(GameTexture))]
        Miscellaneous,

        [AssetType(typeof(GameAudioSource))]
        Music,

        [AssetType(typeof(GameTexture))]
        Paperdoll,

        [AssetType(typeof(GameTexture))]
        Resource,

        [AssetType(typeof(GameShader))]
        Shader,

        [AssetType(typeof(GameAudioSource))]
        Sound,

        [AssetType(typeof(GameTexture))]
        Spell,

        [AssetType(typeof(GameTexture))]
        TexturePack,

        [AssetType(typeof(GameTexture))]
        TileSet,

        [Obsolete] Gui = Interface

    }

}
