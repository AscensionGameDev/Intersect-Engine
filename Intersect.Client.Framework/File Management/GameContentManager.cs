using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Graphics;
using Intersect.Logging;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Intersect.Plugins;
using Intersect.Compression;

namespace Intersect.Client.Framework.File_Management
{

    public abstract class GameContentManager : IContentManager
    {

        public enum TextureType
        {

            Tileset = 0,

            Item,

            Entity,

            Spell,

            Animation,

            Face,

            Image,

            Fog,

            Resource,

            Paperdoll,

            Gui,

            Misc,

        }

        public enum UI
        {

            Menu,

            InGame

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

        protected Dictionary<string, GameAudioSource> mMusicDict = new Dictionary<string, GameAudioSource>();

        protected Dictionary<string, IAsset> mPaperdollDict = new Dictionary<string, IAsset>();

        protected Dictionary<string, IAsset> mResourceDict = new Dictionary<string, IAsset>();

        protected Dictionary<string, GameShader> mShaderDict = new Dictionary<string, GameShader>();

        protected Dictionary<string, GameAudioSource> mSoundDict = new Dictionary<string, GameAudioSource>();

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

            return mMusicDict.TryGetValue(name.ToLower(), out var music) ? music : null;
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

            return mSoundDict.TryGetValue(name.ToLower(), out var sound) ? sound : null;
        }

        public virtual string GetUIJson(UI stage, string name, string resolution, out bool loadedCachedJson)
        {
            var key = new KeyValuePair<UI, string>(stage, $"{name}.{resolution}.json");
            if (mUiDict.TryGetValue(key, out string uiJson))
            {
                loadedCachedJson = true;
                return uiJson;
            }

            loadedCachedJson = false;

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

            var path = "";
            if (resolution != null)
            {
                path = Path.Combine(dir, name + "." + resolution + ".json");
                if (File.Exists(path))
                {
                    return File.ReadAllText(path);
                }
            }

            path = Path.Combine(dir, name + ".json");
            if (File.Exists(path))
            {
                var i = 0;
                while (true)
                {
                    try
                    {
                        var json = File.ReadAllText(path);
                        mUiDict.Add(key, json);
                        return json;
                    }
                    catch (Exception ex)
                    {
                        i++;
                        if (i > 10)
                        {
                            throw;
                        }

                        System.Threading.Thread.Sleep(50);
                    }
                }
            }

            return "";
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

            var path = "";
            if (resolution != null)
            {
                path = Path.Combine(dir, name + "." + resolution + ".json");
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
                case ContentTypes.Sound:
                    throw new NotImplementedException();

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
        public TAsset Load<TAsset>(ContentTypes contentType, string assetPath) where TAsset : class, IAsset
        {
            if (!File.Exists(assetPath))
            {
                throw new FileNotFoundException($@"Asset does not exist at '{assetPath}'.");
            }

            return Load<TAsset>(contentType, assetPath, () => File.OpenRead(assetPath));
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

    }

}
