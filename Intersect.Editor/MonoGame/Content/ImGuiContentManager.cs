using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.UserInterface;
using Intersect.Editor.MonoGame.Graphics;
using Intersect.Plugins;

using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Editor.MonoGame.Content;

internal partial class ImGuiContentManager : IContentManager
{
    private readonly IContentManager _parent;
    private readonly ImGuiRenderer _imGuiRenderer;

    public ImGuiContentManager(IContentManager parent, ImGuiRenderer imGuiRenderer)
    {
        _parent = parent;
        _imGuiRenderer = imGuiRenderer;
    }

    public TAsset? Find<TAsset>(ContentTypes contentType, string assetName) where TAsset : class, IAsset
    {
        if (typeof(TAsset) == typeof(ImGuiTexture))
        {
            var gameTexture = _parent.Find<GameTexture>(contentType, assetName);
            if (gameTexture?.PlatformTextureObject is not Texture2D texture2D)
            {
                return default;
            }

            var copy = new Texture2D(texture2D.GraphicsDevice, texture2D.Width, texture2D.Height);
            var data = new Microsoft.Xna.Framework.Color[copy.Width * copy.Height];
            texture2D.GetData(data);
            copy.SetData(data);

            var texture = new MonoGameTexture(copy);
            return new ImGuiTexture(assetName, texture, _imGuiRenderer) as TAsset;
        }

        return _parent.Find<TAsset>(contentType, assetName);
    }

    public TAsset? Load<TAsset>(ContentTypes contentType, string assetPath, string assetAlias) where TAsset : class, IAsset
    {
        if (typeof(TAsset) == typeof(ImGuiTexture))
        {
            var gameTexture = _parent.Load<GameTexture>(contentType, assetPath, assetAlias);
            if (gameTexture?.PlatformTextureObject is not Texture2D texture2D)
            {
                return default;
            }

            var texture = new MonoGameTexture(texture2D);
            return new ImGuiTexture(assetAlias, texture, _imGuiRenderer) as TAsset;
        }

        return _parent.Load<TAsset>(contentType, assetPath, assetAlias);
    }

    public TAsset? LoadEmbedded<TAsset>(IPluginContext context, ContentTypes contentType, string assetName) where TAsset : class, IAsset
    {
        if (typeof(TAsset) == typeof(ImGuiTexture))
        {
            var gameTexture = _parent.LoadEmbedded<GameTexture>(context, contentType, assetName);
            if (gameTexture?.PlatformTextureObject is not Texture2D texture2D)
            {
                return default;
            }

            var texture = new MonoGameTexture(texture2D);
            return new ImGuiTexture(assetName, texture, _imGuiRenderer) as TAsset;
        }

        return _parent.LoadEmbedded<TAsset>(context, contentType, assetName);
    }

    public TAsset? LoadEmbedded<TAsset>(IPluginContext context, ContentTypes contentType, string assetName, string assetAlias) where TAsset : class, IAsset
    {
        if (typeof(TAsset) == typeof(ImGuiTexture))
        {
            var gameTexture = _parent.LoadEmbedded<GameTexture>(context, contentType, assetName, assetAlias);
            if (gameTexture?.PlatformTextureObject is not Texture2D texture2D)
            {
                return default;
            }

            var texture = new MonoGameTexture(texture2D);
            return new ImGuiTexture(assetAlias, texture, _imGuiRenderer) as TAsset;
        }

        return _parent.LoadEmbedded<TAsset>(context, contentType, assetName, assetAlias);
    }
}
