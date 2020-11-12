using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Logging;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

using Intersect.Client.Framework.Audio;

namespace Intersect.Client.Framework.Content
{
    public abstract class GameContentManager : IContentManager
    {
        protected IGameContext GameContext { get; }

        protected AssetLookup AssetLookup { get; }

        private Dictionary<Assembly, EmbeddedAssetProvider> CachedEmbeddedAssetProviders { get; }

        protected IAssetProvider DefaultAssetProvider { get; }

        protected GameContentManager(IGameContext gameContext, IAssetProvider defaultAssetProvider)
        {
            GameContext = gameContext ?? throw new ArgumentNullException(nameof(gameContext));
            AssetLookup = new AssetLookup();
            CachedEmbeddedAssetProviders = new Dictionary<Assembly, EmbeddedAssetProvider>();
            DefaultAssetProvider =
                defaultAssetProvider ?? throw new ArgumentNullException(nameof(defaultAssetProvider));
        }

        public virtual void Clear() => AssetLookup.ClearAll();

        public virtual void Clear(ContentType contentType) => AssetLookup.Clear(contentType);

        public TAsset Find<TAsset>(ContentType contentType, string assetName) where TAsset : class, IAsset =>
            AssetLookup.Find<TAsset>(contentType, assetName);

        public TAsset Find<TAsset>(AssetReference assetReference) where TAsset : class, IAsset =>
            AssetLookup.Find<TAsset>(assetReference.ContentType, assetReference.Name);

        public IEnumerable<TAsset> FindAll<TAsset>(ContentType contentType) where TAsset : class, IAsset =>
            AssetLookup.GetAssets<TAsset>(contentType);

        public IFont FindFont(string fontName, int fontSize) => Find<IFont>(
            ContentType.Font, GameFont.FormatAssetName(fontName, fontSize)
        );

        public IAudioSource FindSound(string assetName) => Find<IAudioSource>(ContentType.Sound, assetName);

        public ITexture FindTexture(TextureType textureType, string assetName) =>
            Find<ITexture>(textureType.ToContentType(), assetName);

        public void IndexAvailableAssets() => IndexAssets(DefaultAssetProvider);

        public void IndexEmbeddedAssets(Assembly assembly) => IndexAssets(GetEmbeddedAssetProviderFor(assembly));

        public IEnumerable<AssetReference> ListAvailableAssets(ContentType contentType) =>
            AssetLookup.ListAssets(contentType);

        public IEnumerable<AssetReference> ListAvailableTextures(TextureType textureType) =>
            ListAvailableAssets(textureType.ToContentType());

        public Stream OpenRead(AssetReference assetReference)
        {
            IAssetProvider assetProvider = DefaultAssetProvider;
            var assembly = assetReference.Assembly;
            if (assembly != null)
            {
                if (CachedEmbeddedAssetProviders.TryGetValue(assembly, out var embeddedAssetProvider))
                {
                    assetProvider = embeddedAssetProvider;
                }
            }

            return assetProvider.OpenRead(assetReference);
        }

        public AssetReference Resolve(ContentType contentType, string assetName) =>
            DefaultAssetProvider.Resolve(contentType, assetName);

        public AssetReference ResolveEmbedded(ContentType contentType, Assembly assembly, string assetName)
        {
            var embeddedAssetProvider = GetEmbeddedAssetProviderFor(assembly);
            return embeddedAssetProvider.Resolve(contentType, assetName);
        }

        protected EmbeddedAssetProvider GetEmbeddedAssetProviderFor(Assembly assembly)
        {
            if (!CachedEmbeddedAssetProviders.TryGetValue(assembly, out var embeddedAssetProvider))
            {
                embeddedAssetProvider = new EmbeddedAssetProvider(assembly);
                CachedEmbeddedAssetProviders.Add(assembly, embeddedAssetProvider);
            }

            return embeddedAssetProvider;
        }

        protected void IndexAssets(IAssetProvider assetProvider)
        {
            if (assetProvider == null)
            {
                throw new ArgumentNullException(nameof(assetProvider));
            }

            IndexTexturePacks(assetProvider);

            var contentTypes = Enum.GetValues(typeof(ContentType)).Cast<ContentType>().ToList();
            foreach (var contentType in contentTypes)
            {
                if (contentType == ContentType.TexturePack)
                {
                    // Texture packs need to be indexed first.
                    continue;
                }

                var assetReferences = assetProvider.IndexAssetsOfType(contentType);
                switch (contentType)
                {
                    // XNB assets
                    case ContentType.Font:
                        AssetLookup.Add(contentType, assetReferences.Select(CreateFont));
                        break;

                    case ContentType.Shader:
                        AssetLookup.Add(
                            contentType,
                            assetReferences.Where(reference => !reference.Name.Contains("_editor")).Select(CreateShader)
                        );

                        break;

                    // Audio assets
                    case ContentType.Music:
                        AssetLookup.Add(contentType, assetReferences.Select(CreateMusic));
                        break;

                    case ContentType.Sound:
                        AssetLookup.Add(contentType, assetReferences.Select(CreateSound));
                        break;

                    // Texture assets
                    default:
                        var packedTextures = IndexPackedTextures(contentType);
                        AssetLookup.Add(contentType, packedTextures);

                        var textures = assetReferences.Select(CreateTexture);
                        AssetLookup.Add(contentType, textures);
                        break;
                }
            }
        }

        private void IndexTexturePacks(IAssetProvider assetProvider)
        {
            var assetReferences = assetProvider.IndexAssetsOfType(ContentType.TexturePack);
            var texturePackFrames =
                assetReferences.SelectMany(assetReference => ProcessTexturePack(assetProvider, assetReference));

            AssetLookup.Add(ContentType.TexturePack, texturePackFrames);
        }

        protected List<ITexturePackFrame> FilterTexturePackFramesFor(ContentType contentType) =>
            AssetLookup.FindAll<ITexturePackFrame>(
                    ContentType.TexturePack,
                    texturePackFrame => texturePackFrame.Reference == Resolve(contentType, texturePackFrame.Name)
                )
                .ToList();

        private List<ITexture> IndexPackedTextures(ContentType contentType)
        {
            var texturePacksForType = FilterTexturePackFramesFor(contentType);
            return texturePacksForType.Select(texturePackFrame => IndexPackedTexture(contentType, texturePackFrame))
                .ToList();
        }

        private ITexture IndexPackedTexture(ContentType contentType, ITexturePackFrame texturePackFrame) =>
            CreateTexture(
                new AssetReference(
                    texturePackFrame.Reference.Assembly, contentType, texturePackFrame.Name,
                    texturePackFrame.Reference.ResolvedPath
                ), texturePackFrame
            );

        protected IFont CreateFont(AssetReference assetReference) => GameContext.Renderer.LoadFont(assetReference);

        protected abstract IAudioSource CreateMusic(AssetReference assetReference);

        protected IShader CreateShader(AssetReference assetReference) =>
            GameContext.Renderer.LoadShader(assetReference);

        protected abstract IAudioSource CreateSound(AssetReference assetReference);

        protected ITexture CreateTexture(AssetReference assetReference) =>
            GameContext.Renderer.LoadTexture(assetReference);

        protected ITexture CreateTexture(AssetReference assetReference, ITexturePackFrame texturePackFrame) =>
            GameContext.Renderer.LoadTexture(assetReference, texturePackFrame);

        private List<ITexturePackFrame> ProcessTexturePack(
            IAssetProvider assetProvider,
            AssetReference texturePackReference
        )
        {
            try
            {
                JObject descriptorRoot;
                using (var descriptorStream = assetProvider.OpenRead(texturePackReference))
                {
                    using (var descriptorStreamReader = new StreamReader(descriptorStream))
                    {
                        var descriptorSource = descriptorStreamReader.ReadToEnd();
                        descriptorRoot = JObject.Parse(descriptorSource);
                    }
                }

                if (!descriptorRoot.TryGetValue("frames", out var framesToken) || !(framesToken is JArray frames))
                {
                    throw new Exception("Texture pack descriptor does not have a valid 'frames' property.");
                }

                if (!descriptorRoot.TryGetValue("meta", out var metaToken) || !(metaToken is JObject meta))
                {
                    throw new Exception("Texture pack descriptor does not have a valid 'meta' property.");
                }

                if (!meta.TryGetValue("image", out var imageToken) || imageToken.Type != JTokenType.String)
                {
                    throw new Exception("Texture pack descriptor does not have a valid 'meta.image' property.");
                }

                var imageName = imageToken.Value<string>();
                var backingTextureReference = assetProvider.Resolve(ContentType.TexturePack, imageName, false);

                if (!assetProvider.Exists(backingTextureReference))
                {
                    throw new Exception($"Texture pack image missing: {imageName}");
                }

                var backingTexture = CreateTexture(backingTextureReference);

                var texturePackFrames = frames.Select(
                        frame =>
                        {
                            var resolvedFileName = frame["filename"].ToString();
                            var sourceRect = new Rectangle(
                                int.Parse(frame["frame"]["x"].ToString()), int.Parse(frame["frame"]["y"].ToString()),
                                int.Parse(frame["frame"]["w"].ToString()), int.Parse(frame["frame"]["h"].ToString())
                            );

                            var rotated = bool.Parse(frame["rotated"].ToString());
                            var sourceSize = new Rectangle(
                                int.Parse(frame["spriteSourceSize"]["x"].ToString()),
                                int.Parse(frame["spriteSourceSize"]["y"].ToString()),
                                int.Parse(frame["spriteSourceSize"]["w"].ToString()),
                                int.Parse(frame["spriteSourceSize"]["h"].ToString())
                            );

                            return new GameTexturePackFrame(
                                new AssetReference(
                                    texturePackReference.Assembly, ContentType.TexturePack,
                                    Path.GetFileNameWithoutExtension(resolvedFileName), resolvedFileName
                                ), sourceRect, rotated, sourceSize, backingTexture
                            );
                        }
                    )
                    .ToList<ITexturePackFrame>();

                return texturePackFrames;
            }
            catch (Exception exception)
            {
                GameContext.Logger.Error(exception, $"Failed to process texture pack: {texturePackReference.Name}");

                return new List<ITexturePackFrame>();
            }
        }

        #region Static cruft

        // TODO: Refactor this stuff out

        public enum UI
        {
            Menu,

            InGame
        }

        public static GameContentManager Current { get; protected set; }

        public virtual string GetUIJson(UI stage, string name, string resolution)
        {
            var layouts = Path.Combine("resources", "gui", "layouts");
            if (!Directory.Exists(layouts))
            {
                Directory.CreateDirectory(layouts);
            }

            var dir = Path.Combine(layouts, stage == UI.Menu ? "menu" : "game");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string path;
            if (resolution != null)
            {
                path = Path.Combine(dir, $"{name}.{resolution}.json");
                if (File.Exists(path))
                {
                    return File.ReadAllText(path);
                }
            }

            path = Path.Combine(dir, $"{name}.json");
            if (!File.Exists(path))
            {
                return string.Empty;
            }

            var i = 0;
            while (true)
            {
                try
                {
                    return File.ReadAllText(path);
                }
                catch
                {
                    i++;
                    if (i > 10)
                    {
                        throw;
                    }

                    Thread.Sleep(50);
                }
            }
        }

        public virtual void SaveUIJson(UI stage, string name, string json, string resolution)
        {
            var layouts = Path.Combine("resources", "gui", "layouts");
            if (!Directory.Exists(layouts))
            {
                Directory.CreateDirectory(layouts);
            }

            var dir = Path.Combine(layouts, stage == UI.Menu ? "menu" : "game");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string path;
            if (resolution == null)
            {
                path = Path.Combine(dir, name + ".json");
                try
                {
                    File.WriteAllText(path, json);
                }
                catch (Exception exception)
                {
                    Log.Debug(exception);
                }
            }
            else
            {
                path = Path.Combine(dir, $"{name}.{resolution}.json");
                if (File.Exists(path))
                {
                    try
                    {
                        File.WriteAllText(path, json);
                    }
                    catch (Exception exception)
                    {
                        Log.Debug(exception);
                    }

                    return;
                }
            }
        }

        #endregion Static cruft
    }
}
