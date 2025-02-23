using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.Graphics;

namespace Intersect.Client.Framework.Content;


public enum ContentType
{
    [AssetType(typeof(IGameTexture))]
    Animation,

    [AssetType(typeof(IGameTexture))]
    Entity,

    [AssetType(typeof(IGameTexture))]
    Face,

    [AssetType(typeof(IGameTexture))]
    Fog,

    [AssetType(typeof(IFont))]
    Font,

    [AssetType(typeof(IGameTexture))]
    Image,

    [AssetType(typeof(IGameTexture))]
    Interface,

    [AssetType(typeof(IGameTexture))]
    Item,

    [AssetType(typeof(IGameTexture))]
    Miscellaneous,

    [AssetType(typeof(GameAudioSource))]
    Music,

    [AssetType(typeof(IGameTexture))]
    Paperdoll,

    [AssetType(typeof(IGameTexture))]
    Resource,

    [AssetType(typeof(GameShader))]
    Shader,

    [AssetType(typeof(GameAudioSource))]
    Sound,

    [AssetType(typeof(IGameTexture))]
    Spell,

    [AssetType(typeof(IGameTexture))]
    TextureAtlas,

    [AssetType(typeof(IGameTexture))]
    Tileset,

    [Obsolete] Gui = Interface

}
