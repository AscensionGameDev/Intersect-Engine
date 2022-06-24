using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Intersect.Async;
using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Graphics;
using Intersect.Compression;
using Intersect.Logging;
using Intersect.Plugins;

namespace Intersect.Client.Framework.File_Management
{

    public abstract partial class GameContentManager : IContentManager
    {



        public enum UI
        {
            Menu,

            InGame,

            Shared,

            Debug,
        }

        public static GameContentManager Current;

        protected Dictionary<string, IAsset> mAnimationDict = new Dictionary<string, IAsset>();

        protected Dictionary<string, IAsset> mEntityDict = new Dictionary<string, IAsset>();

        protected Dictionary<string, IAsset> mFaceDict = new Dictionary<string, IAsset>();

        protected Dictionary<string, IAsset> mFogDict = new Dictionary<string, IAsset>();

        protected List<GameFont> mFontDict = new List<GameFont>();

        protected Dictionary<string, IAsset> mGuiDict = new Dictionary<string, IAsset>();

        protected Dictionary<string, IAsset> mImageDict = new Dictionary<string, IAsset>();

        protected Dictionary<string, IAsset> mItemDict = new Dictionary<string, IAsset>();

        protected Dictionary<string, IAsset> mMiscDict = new Dictionary<string, IAsset>();

        protected Dictionary<string, IAsset> mMusicDict = new Dictionary<string, IAsset>();

        protected Dictionary<string, IAsset> mPaperdollDict = new Dictionary<string, IAsset>();

        protected Dictionary<string, IAsset> mResourceDict = new Dictionary<string, IAsset>();

        protected Dictionary<string, GameShader> mShaderDict = new Dictionary<string, GameShader>();

        protected Dictionary<string, IAsset> mSoundDict = new Dictionary<string, IAsset>();

        protected Dictionary<KeyValuePair<UI, string>, string> mUiDict = new Dictionary<KeyValuePair<UI, string>, string>();

        /// <summary>
        /// Contains all indexed files and their caches from sound pack files.
        /// </summary>
        public AssetPacker SoundPacks { get; set; }

        /// <summary>
        /// Contains all indexed files and their caches from music pack files.
        /// </summary>
        public AssetPacker MusicPacks { get; set; }

        protected Dictionary<string, IAsset> mSpellDict = new Dictionary<string, IAsset>();

        protected Dictionary<string, IAsset> mTexturePackDict = new Dictionary<string, IAsset>();

        //Game Content
        protected Dictionary<string, IAsset> mTilesetDict = new Dictionary<string, IAsset>();

        public bool TilesetsLoaded = false;

        public ContentWatcher ContentWatcher { get; protected set; }

        public void Init(GameContentManager manager)
        {
            Current = manager;
        }

        //Content Loading
        public void LoadAll()
        {
            LoadTexturePacks();
            LoadEntities();
            LoadItems();
            LoadAnimations();
            LoadSpells();
            LoadFaces();
            LoadImages();
            LoadFogs();
            LoadResources();
            LoadPaperdolls();
            LoadMisc();
            LoadGui();
            LoadFonts();
            LoadShaders();
        }

        public abstract void LoadTexturePacks();

        public abstract void LoadTilesets(string[] tilesetnames);

        public abstract void LoadItems();

        public abstract void LoadEntities();

        public abstract void LoadSpells();

        public abstract void LoadAnimations();

        public abstract void LoadFaces();

        public abstract void LoadImages();

        public abstract void LoadFogs();

        public abstract void LoadResources();

        public abstract void LoadPaperdolls();

        public abstract void LoadGui();

        public abstract void LoadMisc();

        public abstract void LoadFonts();

        public abstract void LoadShaders();

        //Audio Loading
        public void LoadAudio()
        {
            LoadSounds();
            LoadMusic();
        }

        public abstract void LoadSounds();

        public abstract void LoadMusic();

        public static string RemoveExtension(string fileName)
        {
            var fileExtPos = fileName.LastIndexOf(".");
            if (fileExtPos >= 0)
            {
                fileName = fileName.Substring(0, fileExtPos);
            }

            return fileName;
        }

        public string[] GetTextureNames(TextureType type)
        {
            switch (type)
            {
                case TextureType.Tileset:
                    return mTilesetDict.Keys.ToArray();

                case TextureType.Item:
                    return mItemDict.Keys.ToArray();

                case TextureType.Entity:
                    return mEntityDict.Keys.ToArray();

                case TextureType.Spell:
                    return mSpellDict.Keys.ToArray();

                case TextureType.Animation:
                    return mAnimationDict.Keys.ToArray();

                case TextureType.Face:
                    return mFaceDict.Keys.ToArray();

                case TextureType.Image:
                    return mImageDict.Keys.ToArray();

                case TextureType.Fog:
                    return mFogDict.Keys.ToArray();

                case TextureType.Resource:
                    return mResourceDict.Keys.ToArray();

                case TextureType.Paperdoll:
                    return mPaperdollDict.Keys.ToArray();

                case TextureType.Gui:
                    return mGuiDict.Keys.ToArray();

                case TextureType.Misc:
                    return mMiscDict.Keys.ToArray();
            }

            return null;
        }

        //Content Getters
        public virtual GameTexture GetTexture(TextureType type, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            IDictionary<string, IAsset> textureDict;
            switch (type)
            {
                case TextureType.Tileset:
                    textureDict = mTilesetDict;

                    break;

                case TextureType.Item:
                    textureDict = mItemDict;

                    break;

                case TextureType.Entity:
                    textureDict = mEntityDict;

                    break;

                case TextureType.Spell:
                    textureDict = mSpellDict;

                    break;

                case TextureType.Animation:
                    textureDict = mAnimationDict;

                    break;

                case TextureType.Face:
                    textureDict = mFaceDict;

                    break;

                case TextureType.Image:
                    textureDict = mImageDict;

                    break;

                case TextureType.Fog:
                    textureDict = mFogDict;

                    break;

                case TextureType.Resource:
                    textureDict = mResourceDict;

                    break;

                case TextureType.Paperdoll:
                    textureDict = mPaperdollDict;

                    break;

                case TextureType.Gui:
                    textureDict = mGuiDict;

                    break;

                case TextureType.Misc:
                    textureDict = mMiscDict;

                    break;

                default:
                    return null;
            }

            if (textureDict == null)
            {
                return null;
            }

            return textureDict.TryGetValue(name.ToLower(), out var asset) ? asset as GameTexture : default;
        }

        public virtual GameShader GetShader(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            if (mShaderDict == null)
            {
                return null;
            }

            return mShaderDict.TryGetValue(name.ToLower(), out var effect) ? effect : null;
        }

        public virtual GameFont GetFont(string name, int size)
        {
            if (name == null)
            {
                return null;
            }

            return mFontDict.Where(t => t != null)
                .Where(t => t.GetName().ToLower().Trim() == name.ToLower().Trim())
                .FirstOrDefault(t => t.GetSize() == size);
        }

        public virtual GameAudioSource GetMusic(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            if (mMusicDict == null)
            {
                return null;
            }

            return mMusicDict.TryGetValue(name.ToLower(), out var music) ? music as GameAudioSource : default;
        }

        public virtual GameAudioSource GetSound(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            if (mSoundDict == null)
            {
                return null;
            }

            return mSoundDict.TryGetValue(name.ToLower(), out var sound) ? sound as GameAudioSource : default;
        }

        public virtual bool GetLayout(UI stage, string name, string resolution, bool skipCache, Action<string, bool> layoutHandler)
        {
            var isReload = false;

            var stageName = stage.ToString().ToLowerInvariant();
            if (stage == UI.InGame)
            {
                stageName = "game";
            }

            var resourcePartialDirectory = Path.Combine("gui", "layouts", stageName);

            void onLayoutReady()
            {
                var result = GetLayout(stage, name, resolution, skipCache || isReload, out var cacheHit);
                layoutHandler(result, cacheHit);
                isReload = true;
            }

            void addChangeListener(string targetPath)
            {
                ContentWatcher?.AddEventListener(ContentWatcher.Event.Change, targetPath, () => onLayoutReady());
                onLayoutReady();
            }

            void addCreateOrChangeListener(string targetPath, bool createIfMissing)
            {
                var resourcePath = Path.Combine("resources", targetPath);
                if (File.Exists(resourcePath))
                {
                    addChangeListener(targetPath);
                }
                else
                {
                    if (createIfMissing)
                    {
                        layoutHandler?.Invoke(string.Empty, false);
                    }

                    ContentWatcher?.AddEventListener(ContentWatcher.Event.Create, targetPath, () => addChangeListener(targetPath));
                }
            }

            if (!string.IsNullOrWhiteSpace(resolution))
            {
                var resolutionPath = Path.Combine(resourcePartialDirectory, $"{name}.{resolution}.json");
                addCreateOrChangeListener(resolutionPath, false);
            }

            var genericPath = Path.Combine(resourcePartialDirectory, $"{name}.json");
            addCreateOrChangeListener(genericPath, true);

            return true;
        }

        protected virtual string GetLayout(UI stage, string name, string resolution, bool skipCache, out bool cacheHit)
        {
            cacheHit = false;
            var key = new KeyValuePair<UI, string>(stage, $"{name}.{resolution}.json");
            if (!skipCache && mUiDict.TryGetValue(key, out var rawLayout))
            {
                cacheHit = true;
                return rawLayout;
            }

            var stageName = stage.ToString().ToLowerInvariant();
            if (stage == UI.InGame)
            {
                stageName = "game";
            }

            var resourceDirectory = Path.Combine("resources", "gui", "layouts", stageName);

            if (!Directory.Exists(resourceDirectory))
            {
                Directory.CreateDirectory(resourceDirectory);
            }

            var resolutionPath = Path.Combine(resourceDirectory, $"{name}.{resolution}.json");
            if (!string.IsNullOrWhiteSpace(resolution) && File.Exists(resolutionPath)) {
                rawLayout = File.ReadAllText(resolutionPath);
                mUiDict[key] = rawLayout;
                return rawLayout;
            }

            var genericPath = Path.Combine(resourceDirectory, $"{name}.json");
            if (File.Exists(genericPath))
            {
                rawLayout = ExceptionRepeater.Run(() => File.ReadAllText(genericPath), 5);
                mUiDict[key] = rawLayout;
                return rawLayout;
            }

            return string.Empty;
        }

        public virtual string GetUIJson(UI stage, string name, string resolution, out bool cacheHit)
        {
            return GetLayout(stage, name, resolution, false, out cacheHit);
        }

        public virtual void SaveUIJson(UI stage, string name, string json, string resolution)
        {
            var stageName = stage.ToString().ToLowerInvariant();
            if (stage == UI.InGame)
            {
                stageName = "game";
            }

            var stagePartialPath = Path.Combine("gui", "layouts", stageName);

            string resourceName;
            if (resolution != null)
            {
                resourceName = Path.Combine(stagePartialPath, $"{name}.{resolution}.json");
                var resourcePath = Path.Combine("resources", resourceName);
                if (File.Exists(resourcePath))
                {
                    ContentWatcher?.Modify(resourceName, () =>
                    {
                        try
                        {
                            File.WriteAllText(resourcePath, json);
                        }
                        catch (Exception exception)
                        {
                            Log.Debug(exception);
                        }
                    });
                    return;
                }
            }

            resourceName = Path.Combine(stagePartialPath, $"{name}.json");
            ContentWatcher?.Modify(resourceName, () =>
            {
                var resourcePath = Path.Combine("resources", resourceName);
                try
                {
                    File.WriteAllText(resourcePath, json);
                }
                catch (Exception exception)
                {
                    Log.Debug(exception);
                }
            });
        }

        protected Dictionary<string, IAsset> GetAssetLookup(ContentTypes contentType)
        {
            switch (contentType)
            {
                case ContentTypes.Animation:
                    return mAnimationDict;

                case ContentTypes.Entity:
                    return mEntityDict;

                case ContentTypes.Face:
                    return mFaceDict;

                case ContentTypes.Fog:
                    return mFogDict;

                case ContentTypes.Image:
                    return mImageDict;

                case ContentTypes.Interface:
                    return mGuiDict;

                case ContentTypes.Item:
                    return mItemDict;

                case ContentTypes.Miscellaneous:
                    return mMiscDict;

                case ContentTypes.Paperdoll:
                    return mPaperdollDict;

                case ContentTypes.Resource:
                    return mResourceDict;

                case ContentTypes.Spell:
                    return mSpellDict;

                case ContentTypes.TexturePack:
                    return mTexturePackDict;

                case ContentTypes.TileSet:
                    return mTilesetDict;

                case ContentTypes.Font:
                    throw new NotImplementedException();

                case ContentTypes.Shader:
                    throw new NotImplementedException();

                case ContentTypes.Music:
                    return mMusicDict;

                case ContentTypes.Sound:
                    return mSoundDict;

                default:
                    throw new ArgumentOutOfRangeException(nameof(contentType), contentType, null);
            }
        }

        protected abstract TAsset Load<TAsset>(
            Dictionary<string, IAsset> lookup,
            ContentTypes contentType,
            string assetName,
            Func<Stream> createStream
        ) where TAsset : class, IAsset;

        /// <inheritdoc />
        public TAsset Find<TAsset>(ContentTypes contentType, string assetName) where TAsset : class, IAsset
        {
            var assetLookup = GetAssetLookup(contentType);

            if (assetLookup.TryGetValue(assetName, out var asset))
            {
                return asset as TAsset;
            }

            return null;
        }

        /// <inheritdoc />
        public TAsset Load<TAsset>(ContentTypes contentType, string assetPath, string assetAlias) where TAsset : class, IAsset
        {
            if (!File.Exists(assetPath))
            {
                throw new FileNotFoundException($@"Asset does not exist at '{assetPath}'.");
            }

            return Load<TAsset>(contentType, assetAlias.ToLower(), () => File.OpenRead(assetPath));
        }

        /// <inheritdoc />
        public TAsset Load<TAsset>(ContentTypes contentType, string assetName, Func<Stream> createStream)
            where TAsset : class, IAsset
        {
            var assetLookup = GetAssetLookup(contentType);

            if (assetLookup.TryGetValue(assetName, out var asset))
            {
                return asset as TAsset;
            }

            return Load<TAsset>(assetLookup, contentType, assetName, createStream);
        }

        /// <inheritdoc />
        public TAsset LoadEmbedded<TAsset>(IPluginContext context, ContentTypes contentType, string assetName)
            where TAsset : class, IAsset
        {
            var manifestResourceName = context.EmbeddedResources.Resolve(assetName);
            return Load<TAsset>(contentType, assetName, () => context.EmbeddedResources.Read(manifestResourceName));
        }

        /// <inheritdoc />
        public TAsset LoadEmbedded<TAsset>(IPluginContext context, ContentTypes contentType, string assetName, string assetAlias)
            where TAsset : class, IAsset
        {
            var manifestResourceName = context.EmbeddedResources.Resolve(assetName);
            return Load<TAsset>(contentType, assetName, assetAlias);
        }

    }

}
