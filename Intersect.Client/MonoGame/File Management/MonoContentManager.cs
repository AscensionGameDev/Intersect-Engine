using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Localization;
using Intersect.Client.MonoGame.Audio;
using Intersect.Client.MonoGame.Graphics;
using Intersect.Logging;

using Newtonsoft.Json.Linq;

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

            var dir = Path.Combine("resources", "tilesets");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var tilesetFiles = Directory.GetFiles(dir).Select(f => Path.GetFileName(f));
            foreach (var t in tilesetnames)
            {
                var realFilename = tilesetFiles.FirstOrDefault(file => t.Equals(file, StringComparison.InvariantCultureIgnoreCase)) ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(t) &&
                    (!string.IsNullOrWhiteSpace(realFilename) ||
                     GameTexturePacks.GetFrame(Path.Combine("resources", "tilesets", t.ToLower())) != null) &&
                    !mTilesetDict.ContainsKey(t.ToLower()))
                {
                    mTilesetDict.Add(
                        t.ToLower(), Core.Graphics.Renderer.LoadTexture(Path.Combine("resources", "tilesets", t), Path.Combine("resources", "tilesets", realFilename))
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
                    var platformText = Core.Graphics.Renderer.LoadTexture(Path.Combine("resources", "packs", img), Path.Combine("resources", "packs", img));
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

        public void LoadTextureGroup(string directory, Dictionary<string, GameTexture> dict)
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
                dict.Add(filename, Core.Graphics.Renderer.LoadTexture(Path.Combine(dir, filename), Path.Combine(dir, items[i].Replace(dir, "").TrimStart(Path.DirectorySeparatorChar))));
            }

            var packItems = GameTexturePacks.GetFolderFrames(directory);
            if (packItems != null)
            {
                foreach (var itm in packItems)
                {
                    var filename = Path.GetFileName(itm.Filename.ToLower().Replace("\\","/"));
                    if (!dict.ContainsKey(filename))
                    {
                        dict.Add(filename, Core.Graphics.Renderer.LoadTexture(Path.Combine(dir, filename), Path.Combine(dir, Path.Combine(dir, filename))));
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

            const string shaderPrefix = "Intersect.Client.Resources.Shaders.";
            var availableShaders = typeof(MonoContentManager).Assembly
                .GetManifestResourceNames()
                .Where(resourceName =>
                    resourceName.StartsWith(shaderPrefix)
                    && resourceName.EndsWith(".xnb")
                ).ToArray();

            for (var i = 0; i < availableShaders.Length; i++)
            {
                var resourceFullName = availableShaders[i];
                var shaderNameWithoutExtension = resourceFullName.Substring(0, resourceFullName.Length - 4);
                var shaderName = shaderNameWithoutExtension.Substring(shaderPrefix.Length);
                mShaderDict.Add(shaderName, Core.Graphics.Renderer.LoadShader(resourceFullName));
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
                        Path.Combine(dir, filename), Path.Combine(dir, items[i].Replace(dir, "").TrimStart(Path.DirectorySeparatorChar))
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
                mMusicDict.Add(RemoveExtension(filename), new MonoMusicSource(Path.Combine(dir, filename), Path.Combine(dir, items[i].Replace(dir, "").TrimStart(Path.DirectorySeparatorChar))));
            }
        }

    }

}
