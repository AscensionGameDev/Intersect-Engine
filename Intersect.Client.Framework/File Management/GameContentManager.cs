using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.Graphics;
using Intersect.Logging;

namespace Intersect.Client.Framework.File_Management
{

    public abstract class GameContentManager
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

        protected Dictionary<string, GameTexture> mAnimationDict = new Dictionary<string, GameTexture>();

        protected Dictionary<string, GameTexture> mEntityDict = new Dictionary<string, GameTexture>();

        protected Dictionary<string, GameTexture> mFaceDict = new Dictionary<string, GameTexture>();

        protected Dictionary<string, GameTexture> mFogDict = new Dictionary<string, GameTexture>();

        protected List<GameFont> mFontDict = new List<GameFont>();

        protected Dictionary<string, GameTexture> mGuiDict = new Dictionary<string, GameTexture>();

        protected Dictionary<string, GameTexture> mImageDict = new Dictionary<string, GameTexture>();

        protected Dictionary<string, GameTexture> mItemDict = new Dictionary<string, GameTexture>();

        protected Dictionary<string, GameTexture> mMiscDict = new Dictionary<string, GameTexture>();

        protected Dictionary<string, GameAudioSource> mMusicDict = new Dictionary<string, GameAudioSource>();

        protected Dictionary<string, GameTexture> mPaperdollDict = new Dictionary<string, GameTexture>();

        protected Dictionary<string, GameTexture> mResourceDict = new Dictionary<string, GameTexture>();

        protected Dictionary<string, GameShader> mShaderDict = new Dictionary<string, GameShader>();

        protected Dictionary<string, GameAudioSource> mSoundDict = new Dictionary<string, GameAudioSource>();

        protected Dictionary<string, GameTexture> mSpellDict = new Dictionary<string, GameTexture>();

        protected Dictionary<string, GameTexture> mTexturePackDict = new Dictionary<string, GameTexture>();

        //Game Content
        protected Dictionary<string, GameTexture> mTilesetDict = new Dictionary<string, GameTexture>();

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

            IDictionary<string, GameTexture> textureDict = null;
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

            return textureDict.TryGetValue(name.ToLower(), out var texture) ? texture : null;
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

    }

}
