using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Localization;
using Intersect.Client.MonoGame.Audio;
using Intersect.Client.MonoGame.Graphics;
using Intersect.Logging;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;

namespace Intersect.Client.MonoGame.File_Management
{

    public class MonoContentManager : GameContentManager
    {

        public MonoContentManager()
        {
            Init(this);
            if (!Directory.Exists("resources"))
            {
                Log.Error(Strings.Errors.resourcesnotfound);

                Environment.Exit(1);
            }
        }

        //Graphic Loading
        public override void LoadTilesets(string[] tilesetnames)
        {
            mTilesetDict.Clear();
            foreach (var t in tilesetnames)
            {
                if (t != "" &&
                    (File.Exists(Path.Combine("resources", "tilesets", t)) ||
                     GameTexturePacks.GetFrame(Path.Combine("resources", "tilesets", t.ToLower())) != null) &&
                    !mTilesetDict.ContainsKey(t.ToLower()))
                {
                    mTilesetDict.Add(
                        t.ToLower(), Core.Graphics.Renderer.LoadTexture(Path.Combine("resources", "tilesets", t))
                    );
                }
            }

            TilesetsLoaded = true;
        }

        public override void LoadTexturePacks()
        {
            mTexturePackDict.Clear();
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
                    var platformText = Core.Graphics.Renderer.LoadTexture(Path.Combine("resources", "packs", img));
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

        public void LoadTextureGroup(string directory, Dictionary<string, IAsset> dict)
        {
            dict.Clear();
            var dir = Path.Combine("resources", directory);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var items = Directory.GetFiles(dir, "*.png");
            for (var i = 0; i < items.Length; i++)
            {
                var filename = items[i].Replace(dir, "").TrimStart(Path.DirectorySeparatorChar).ToLower();
                dict.Add(filename, Core.Graphics.Renderer.LoadTexture(Path.Combine(dir, filename)));
            }

            var packItems = GameTexturePacks.GetFolderFrames(directory);
            if (packItems != null)
            {
                foreach (var itm in packItems)
                {
                    var filename = Path.GetFileName(itm.Filename.ToLower().Replace("\\", "/"));
                    if (!dict.ContainsKey(filename))
                    {
                        dict.Add(filename, Core.Graphics.Renderer.LoadTexture(Path.Combine(dir, filename)));
                    }
                }
            }
        }

        public override void LoadItems()
        {
            LoadTextureGroup("items", mItemDict);
        }

        public override void LoadEntities()
        {
            LoadTextureGroup("entities", mEntityDict);
        }

        public override void LoadSpells()
        {
            LoadTextureGroup("spells", mSpellDict);
        }

        public override void LoadAnimations()
        {
            LoadTextureGroup("animations", mAnimationDict);
        }

        public override void LoadFaces()
        {
            LoadTextureGroup("faces", mFaceDict);
        }

        public override void LoadImages()
        {
            LoadTextureGroup("images", mImageDict);
        }

        public override void LoadFogs()
        {
            LoadTextureGroup("fogs", mFogDict);
        }

        public override void LoadResources()
        {
            LoadTextureGroup("resources", mResourceDict);
        }

        public override void LoadPaperdolls()
        {
            LoadTextureGroup("paperdolls", mPaperdollDict);
        }

        public override void LoadGui()
        {
            LoadTextureGroup("gui", mGuiDict);
        }

        public override void LoadMisc()
        {
            LoadTextureGroup("misc", mMiscDict);
        }

        public override void LoadFonts()
        {
            mFontDict.Clear();
            var dir = Path.Combine("resources", "fonts");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var items = Directory.GetFiles(dir, "*.xnb");
            for (var i = 0; i < items.Length; i++)
            {
                var filename = items[i].Replace(dir, "").TrimStart(Path.DirectorySeparatorChar).ToLower();
                var font = Core.Graphics.Renderer.LoadFont(Path.Combine(dir, filename));
                if (mFontDict.IndexOf(font) == -1)
                {
                    mFontDict.Add(font);
                }
            }
        }

        public override void LoadShaders()
        {
            mShaderDict.Clear();
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
                    mShaderDict.Add(
                        filename.Replace(".xnb", ""), Core.Graphics.Renderer.LoadShader(Path.Combine(dir, filename))
                    );
                }
            }
        }

        public override void LoadSounds()
        {
            mSoundDict.Clear();
            var dir = Path.Combine("resources", "sounds");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var items = Directory.GetFiles(dir, "*.wav");
            for (var i = 0; i < items.Length; i++)
            {
                var filename = items[i].Replace(dir, "").TrimStart(Path.DirectorySeparatorChar).ToLower();
                mSoundDict.Add(
                    RemoveExtension(filename),
                    new MonoSoundSource(
                        Path.Combine(dir, filename), ((MonoRenderer) Core.Graphics.Renderer).GetContentManager()
                    )
                );
            }
        }

        public override void LoadMusic()
        {
            mMusicDict.Clear();
            var dir = Path.Combine("resources", "music");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var items = Directory.GetFiles(dir, "*.ogg");
            for (var i = 0; i < items.Length; i++)
            {
                var filename = items[i].Replace(dir, "").TrimStart(Path.DirectorySeparatorChar).ToLower();
                mMusicDict.Add(RemoveExtension(filename), new MonoMusicSource(Path.Combine(dir, filename)));
            }
        }

        /// <inheritdoc />
        protected override TAsset Load<TAsset>(
            Dictionary<string, IAsset> lookup,
            ContentTypes contentType,
            string name,
            Func<Stream> createStream
        )
        {
            switch (contentType)
            {
                case ContentTypes.Animation:
                case ContentTypes.Entity:
                case ContentTypes.Face:
                case ContentTypes.Fog:
                case ContentTypes.Image:
                case ContentTypes.Interface:
                case ContentTypes.Item:
                case ContentTypes.Miscellaneous:
                case ContentTypes.Paperdoll:
                case ContentTypes.Resource:
                case ContentTypes.Spell:
                case ContentTypes.TexturePack:
                case ContentTypes.TileSet:
                    return Core.Graphics.Renderer.LoadTexture(name, createStream) as TAsset;

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

    }

}
