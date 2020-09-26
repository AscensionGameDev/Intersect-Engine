using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.Graphics;
using Intersect.Logging;

using JetBrains.Annotations;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Intersect.Plugins;

namespace Intersect.Client.Framework.Content
{
    public abstract class GameContentManager : IContentManager
    {
        public enum UI
        {
            Menu,

            InGame
        }

        public static GameContentManager Current;

        protected AssetLookup AssetLookup { get; }

        public bool TilesetsLoaded = false;

        protected GameContentManager()
        {
            AssetLookup = new AssetLookup();
        }

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

        public void LoadTilesets(params string[] tilesetNames) => LoadTilesets(tilesetNames as IEnumerable<string>);

        public abstract void LoadTilesets(IEnumerable<string> tilesetNames);

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

        public string GetPathForAssetType(ContentType contentType) => Path.Combine(
            "resources", contentType.GetDirectory()
        ).Replace('\\', '/');

        public string GetPathForAsset(ContentType contentType, string assetName) => Path.Combine(
            GetPathForAssetType(contentType), assetName
        ).Replace('\\', '/');

        public abstract ITexturePackFrame FindTexturePackFrameFor(TextureType textureType, string textureName);

        public string[] GetTextureNames(TextureType textureType) =>
            AssetLookup.GetAvailableAssetNamesFor(textureType.ToContentType()).ToArray();

        /// <inheritdoc />
        public IAudioSource LoadSound(string soundName) => Load<IAudioSource>(ContentType.Sound, soundName);

        public virtual ITexture LoadTexture(TextureType textureType, string textureFileName)
        {
            if (string.IsNullOrWhiteSpace(textureFileName))
            {
                return null;
            }

            return Load<ITexture>(textureType.ToContentType(), textureFileName);
        }

        public virtual IShader GetShader(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            return AssetLookup.Find(ContentType.Shader, name) as IShader;
        }

        public IFont LoadFont(string name, int size)
        {
            if (name == null)
            {
                return null;
            }

            return AssetLookup.Find<IFont>(ContentType.Font, $"{name},{size}");

            // return AssetLookup.Find<IFont>(
            //     ContentType.Font,
            //     font => string.Equals(font.Name, name, StringComparison.CurrentCultureIgnoreCase) && font.Size == size
            // );
        }

        /// <inheritdoc />
        public IAudioSource LoadMusic(string musicName) => Load<IAudioSource>(ContentType.Music, musicName);

        public virtual IAudioSource GetSound(string name) => AssetLookup.Find<IAudioSource>(ContentType.Sound, name);

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
                        return File.ReadAllText(path);
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

        [NotNull]
        protected abstract TAsset InternalLoad<TAsset>(
            ContentType contentType,
            [NotNull] string assetName,
            [NotNull] Func<string, Stream> createStream
        ) where TAsset : class, IAsset;

        /// <inheritdoc />
        public TAsset Load<TAsset>(ContentType contentType, string assetName) where TAsset : class, IAsset =>
            Load<TAsset>(contentType, assetName, null);

        /// <inheritdoc />
        public TAsset Load<TAsset>(ContentType contentType, string assetName, Func<string, Stream> createStream)
            where TAsset : class, IAsset
        {
            var asset = AssetLookup.Find<TAsset>(contentType, assetName);
            if (asset != default(TAsset))
            {
                return asset;
            }

            return InternalLoad<TAsset>(contentType, assetName, createStream);
        }

        /// <inheritdoc />
        public TAsset LoadEmbedded<TAsset>(IPluginContext context, ContentType contentType, string assetName)
            where TAsset : class, IAsset
        {
            var manifestResourceName = context.EmbeddedResources.Resolve(assetName);
            return Load<TAsset>(contentType, assetName, _ => context.EmbeddedResources.Read(manifestResourceName));
        }
    }
}
