using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Localization;
using Intersect.Client.MonoGame.Audio;
using Intersect.Client.MonoGame.Graphics;
using Intersect.Logging;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Intersect.Client.General;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Client.MonoGame.Content
{
    public class MonoContentManager : GameContentManager
    {
        protected ContentManager ContentManager { get; }

        public MonoContentManager(ContentManager contentManager)
        {
            ContentManager = contentManager;

            Init(this);
            if (!Directory.Exists("resources"))
            {
                Log.Error(Strings.Errors.resourcesnotfound);

                Environment.Exit(1);
            }
        }

        //Graphic Loading
        public override void LoadTilesets(IEnumerable<string> tilesetnames)
        {
            AssetLookup.Clear(ContentType.Tileset);
            var tilesetFiles = Directory.GetFiles(Path.Combine("resources", "tilesets"))
                .Select(f => Path.GetFileName(f));

            foreach (var t in tilesetnames)
            {
                var realFilename = tilesetFiles.FirstOrDefault(
                    file => t.Equals(file, StringComparison.InvariantCultureIgnoreCase)
                );

                if (t != "" &&
                    (!string.IsNullOrWhiteSpace(realFilename) ||
                     GameTexturePacks.GetFrame(Path.Combine("resources", "tilesets", t.ToLower())) != null) &&
                    !AssetLookup.Contains(ContentType.Tileset, t.ToLower()))
                {
                    AssetLookup.Add(
                        ContentType.Tileset,
                        Core.Graphics.GameRenderer.LoadTexture(
                            Path.Combine(
                                "resources", "tilesets", !string.IsNullOrWhiteSpace(realFilename) ? realFilename : t
                            )
                        )
                    );
                }
            }

            TilesetsLoaded = true;
        }

        public override void LoadTexturePacks()
        {
            AssetLookup.Clear(ContentType.TexturePack);
            var dir = Path.Combine("resources", "packs");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var items = Directory.GetFiles(dir, "*.json");
            for (var i = 0; i < items.Length; i++)
            {
                var json = File.ReadAllText(items[i]);
                var obj = JObject.Parse(json);
                var frames = (JArray) obj["frames"];
                var img = obj["meta"]["image"].ToString();
                if (File.Exists(Path.Combine("resources", "packs", img)))
                {
                    var platformText = Core.Graphics.GameRenderer.LoadTexture(Path.Combine("resources", "packs", img));
                    if (platformText != null)
                    {
                        foreach (var frame in frames)
                        {
                            var filename = frame["filename"].ToString();
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

                            GameTexturePacks.AddFrame(
                                new GameTexturePackFrame(filename, sourceRect, rotated, sourceSize, platformText)
                            );
                        }
                    }
                }
            }
        }

        public void LoadTextureGroup(string directory, ContentType contentType)
        {
            AssetLookup.Clear(contentType);
            var dir = Path.Combine("resources", directory);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var items = Directory.GetFiles(dir, "*.png");
            for (var i = 0; i < items.Length; i++)
            {
                var filename = items[i].Replace(dir, "").TrimStart(Path.DirectorySeparatorChar);
                AssetLookup.Add(contentType, Core.Graphics.GameRenderer.LoadTexture(Path.Combine(dir, filename)));
            }

            var texturePackFrames = GameTexturePacks.GetFolderFrames(directory);
            if (texturePackFrames != null)
            {
                foreach (var texturePackFrame in texturePackFrames)
                {
                    var filename = Path.GetFileName(texturePackFrame.Name.Replace("\\", "/"));
                    if (!AssetLookup.Contains(contentType, filename))
                    {
                        AssetLookup.Add(
                            contentType, Core.Graphics.GameRenderer.LoadTexture(Path.Combine(dir, filename), texturePackFrame)
                        );
                    }
                }
            }
        }

        public override void LoadItems()
        {
            LoadTextureGroup("items", ContentType.Item);
        }

        public override void LoadEntities()
        {
            LoadTextureGroup("entities", ContentType.Entity);
        }

        public override void LoadSpells()
        {
            LoadTextureGroup("spells", ContentType.Spell);
        }

        public override void LoadAnimations()
        {
            LoadTextureGroup("animations", ContentType.Animation);
        }

        public override void LoadFaces()
        {
            LoadTextureGroup("faces", ContentType.Face);
        }

        public override void LoadImages()
        {
            LoadTextureGroup("images", ContentType.Image);
        }

        public override void LoadFogs()
        {
            LoadTextureGroup("fogs", ContentType.Fog);
        }

        public override void LoadResources()
        {
            LoadTextureGroup("resources", ContentType.Resource);
        }

        public override void LoadPaperdolls()
        {
            LoadTextureGroup("paperdolls", ContentType.Paperdoll);
        }

        public override void LoadGui()
        {
            LoadTextureGroup("gui", ContentType.Interface);
        }

        public override void LoadMisc()
        {
            LoadTextureGroup("misc", ContentType.Miscellaneous);
        }

        public override void LoadFonts()
        {
            AssetLookup.Clear(ContentType.Font);
            var dir = Path.Combine("resources", "fonts");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var items = Directory.GetFiles(dir, "*.xnb");
            for (var i = 0; i < items.Length; i++)
            {
                var filename = items[i].Replace(dir, "").TrimStart(Path.DirectorySeparatorChar).ToLower();
                var font = Core.Graphics.GameRenderer.LoadFont(Path.Combine(dir, filename));
                if (!AssetLookup.Contains(ContentType.Font, font.Name))
                {
                    AssetLookup.Add(ContentType.Font, font);
                }
            }
        }

        public override void LoadShaders()
        {
            AssetLookup.Clear(ContentType.Shader);
            var dir = Path.Combine("resources", "shaders");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var items = Directory.GetFiles(dir, "*.xnb");
            for (var i = 0; i < items.Length; i++)
            {
                var filename = items[i].Replace(dir, "").TrimStart(Path.DirectorySeparatorChar).ToLower();
                if (!filename.Contains("_editor"))
                {
                    var name = RemoveExtension(filename);
                    var effect = ContentManager.Load<Effect>(Path.Combine(dir, name));
                    AssetLookup.Add(ContentType.Shader, new MonoShader(name, effect));
                }
            }
        }

        public override void LoadSounds()
        {
            AssetLookup.Clear(ContentType.Sound);
            var directory = Path.Combine("resources", "sounds");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var filePaths = Directory.GetFiles(directory, "*.wav");
            AssetLookup.Add(
                ContentType.Sound,
                filePaths.Select(filePath => new MonoSoundSource(RemoveExtension(Path.GetFileName(filePath)), filePath))
            );
        }

        public override void LoadMusic()
        {
            AssetLookup.Clear(ContentType.Music);
            var directory = Path.Combine("resources", "music");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var filePaths = Directory.GetFiles(directory, "*.ogg");
            AssetLookup.Add(
                ContentType.Music,
                filePaths.Select(filePath => new MonoMusicSource(RemoveExtension(Path.GetFileName(filePath)), filePath))
            );
        }

        /// <inheritdoc />
        protected override TAsset InternalLoad<TAsset>(
            ContentType contentType,
            string assetName,
            Func<string, Stream> createStream
        )
        {
            var directoryName = contentType.GetDirectory();
            var assetPath = Path.Combine("resources", directoryName, assetName);

            if (!File.Exists(assetPath))
            {
                return default;
            }

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
                case ContentType.TexturePack:
                case ContentType.Tileset:
                    return Core.Graphics.GameRenderer.LoadTexture(
                        assetPath, createStream == null ? null : (Func<Stream>) (() => createStream(assetPath))
                    ) as TAsset;

                case ContentType.Font:
                    return Core.Graphics.GameRenderer.LoadFont(assetPath) as TAsset;

                case ContentType.Shader:
                    return Core.Graphics.GameRenderer.LoadShader(assetPath) as TAsset;

                case ContentType.Music:
                    return new MonoMusicSource(assetName, $"{assetPath}.ogg") as TAsset;

                case ContentType.Sound:
                    return new MonoSoundSource(assetName, $"{assetPath}.wav") as TAsset;

                default:
                    throw new ArgumentOutOfRangeException(nameof(contentType), contentType, null);
            }
        }
    }
}
