using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Intersect.Editor.Classes.Content;
using Intersect.Editor.Forms;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Editor.Classes.Core
{
    public static class GameContentManager
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

        //MonoGame Content Manager
        private static ContentManager contentManger;

        //Initial Resource Downloading
        private static string resourceRelayer = "http://ascensiongamedev.com/resources/Intersect/findResources.php";

        private static frmLoadingContent loadingForm;
        private static bool downloadCompleted;
        private static string errorString = "";

        //Game Content
        public static List<GameTexture> AllTextures = new List<GameTexture>();

        static IDictionary<string, GameTexture> mTilesetDict = new Dictionary<string, GameTexture>();
        static IDictionary<string, GameTexture> itemDict = new Dictionary<string, GameTexture>();
        static IDictionary<string, GameTexture> entityDict = new Dictionary<string, GameTexture>();
        static IDictionary<string, GameTexture> spellDict = new Dictionary<string, GameTexture>();
        static IDictionary<string, GameTexture> animationDict = new Dictionary<string, GameTexture>();
        static IDictionary<string, GameTexture> faceDict = new Dictionary<string, GameTexture>();
        static IDictionary<string, GameTexture> imageDict = new Dictionary<string, GameTexture>();
        static IDictionary<string, GameTexture> mFogDict = new Dictionary<string, GameTexture>();
        static IDictionary<string, GameTexture> resourceDict = new Dictionary<string, GameTexture>();
        static IDictionary<string, GameTexture> paperdollDict = new Dictionary<string, GameTexture>();
        static IDictionary<string, GameTexture> guiDict = new Dictionary<string, GameTexture>();
        static IDictionary<string, GameTexture> mMiscDict = new Dictionary<string, GameTexture>();
        static IDictionary<string, Effect> mShaderDict = new Dictionary<string, Effect>();
        static IDictionary<string, object> mMusicDict = new Dictionary<string, object>();
        static IDictionary<string, object> mSoundDict = new Dictionary<string, object>();

        //Resource Downloader
        public static void CheckForResources()
        {
            ServicePointManager.Expect100Continue = false;
            if (!Directory.Exists("resources"))
            {
                loadingForm = new frmLoadingContent();
                loadingForm.Show();
                loadingForm.BringToFront();
                using (WebClient client = new WebClient())
                {
                    byte[] response =
                        client.UploadValues(resourceRelayer, new NameValueCollection()
                        {
                            {"version", Assembly.GetExecutingAssembly().GetName().Version.ToString()},
                        });
                    string result = Encoding.UTF8.GetString(response);
                    if (Uri.TryCreate(result, UriKind.Absolute, out Uri urlResult))
                    {
                        client.DownloadProgressChanged += Client_DownloadProgressChanged;
                        client.DownloadFileCompleted += Client_DownloadFileCompleted;
                        bool retry = true;
                        while (retry == true)
                        {
                            try
                            {
                                downloadCompleted = false;
                                errorString = "";
                                client.DownloadFileAsync(urlResult, "resources.zip");
                                while (!downloadCompleted)
                                {
                                    Application.DoEvents();
                                }
                            }
                            catch (Exception ex)
                            {
                                errorString = ex.Message;
                            }
                            if (errorString != "")
                            {
                                if (
                                    MessageBox.Show(
                                        "Failed to download client resources.\n\nException Info: " + errorString +
                                        "\n\n" +
                                        "Would you like to try again?", "Failed to load Resources!",
                                        MessageBoxButtons.YesNo) != DialogResult.Yes)
                                {
                                    retry = false;
                                }
                            }
                            else
                            {
                                retry = false;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(
                            "Failed to load resources from client directory and Ascension Game Dev server. Cannot launch game!",
                            "Failed to load Resources!");
                    }
                }
                loadingForm.Close();
            }
            if (!Directory.Exists("resources"))
            {
                Environment.Exit(1);
            }
        }

        private static void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            downloadCompleted = true;
            if (!e.Cancelled && e.Error == null)
            {
                try
                {
                    System.IO.Compression.ZipFile.ExtractToDirectory("resources.zip",
                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                    File.Delete("resources.zip");
                }
                catch (Exception ex)
                {
                    errorString = ex.Message;
                }
            }
            else
            {
                if (e.Cancelled)
                {
                    errorString = "Download was cancelled!";
                }
                else
                {
                    errorString = e.Error.Message;
                }
            }
        }

        private static void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            loadingForm.SetProgress(e.ProgressPercentage);
        }

        public static void LoadEditorContent()
        {
            //Start by creating a MonoGame Content Manager
            //We create a dummy game service so we can load up a content manager.
            var container = new GameServiceContainer();
            container.AddService(typeof(IGraphicsDeviceService),
                new DummyGraphicsDeviceManager(EditorGraphics.GetGraphicsDevice()));
            contentManger = new ContentManager(container, "");
            LoadItems();
            LoadEntities();
            LoadSpells();
            LoadAnimations();
            LoadFaces();
            LoadImages();
            LoadFogs();
            LoadResources();
            LoadPaperdolls();
            //NO GUI
            LoadMisc();
            LoadShaders();
            LoadSounds();
            LoadMusic();
        }

        public static void Update()
        {
            for (int i = 0; i < AllTextures.Count; i++) AllTextures[i].Update();
        }

        //Loading Game Resources
        public static void LoadTextureGroup(string directory, IDictionary<string, GameTexture> dict)
        {
            dict.Clear();
            if (!Directory.Exists("resources/" + directory))
            {
                Directory.CreateDirectory("resources/" + directory);
            }
            var items = Directory.GetFiles("resources/" + directory, "*.png");
            for (int i = 0; i < items.Length; i++)
            {
                string filename = items[i].Replace("resources/" + directory + "\\", "").ToLower();
                dict.Add(filename, new GameTexture("resources/" + directory + "/" + filename));
            }
        }

        public static void LoadTilesets()
        {
            if (!Directory.Exists("resources/tilesets"))
            {
                Directory.CreateDirectory("resources/tilesets");
            }
            var tilesets = Directory.GetFiles("resources/tilesets", "*.png");
            var tilesetWarning = false;
            var suppressTilesetWarning = Preferences.LoadPreference("SuppressTextureWarning");
            if (suppressTilesetWarning != "" && Convert.ToBoolean(suppressTilesetWarning))
            {
                tilesetWarning = true;
            }
            List<string> newTilesets = new List<string>();
            Array.Sort(tilesets, new AlphanumComparatorFast());
            if (tilesets.Length > 0)
            {
                var tilesetBaseList = Database.GetGameObjectList(GameObjectType.Tileset);
                for (var i = 0; i < tilesets.Length; i++)
                {
                    tilesets[i] = tilesets[i].Replace("resources/tilesets\\", "");
                    if (tilesetBaseList.Length > 0)
                    {
                        for (var x = 0; x < tilesetBaseList.Length; x++)
                        {
                            if (tilesetBaseList[x].ToLower() == tilesets[i].ToLower())
                            {
                                break;
                            }
                            if (x != tilesetBaseList.Length - 1) continue;
                            newTilesets.Add(tilesets[i]);
                        }
                    }
                    else
                    {
                        newTilesets.Add(tilesets[i]);
                    }
                }
            }

            mTilesetDict.Clear();
            var badTilesets = new List<string>();
            for (var i = 0; i < TilesetBase.Lookup.Count; i++)
            {
                var tileset =
                    TilesetBase.Lookup.Get<TilesetBase>(Database.GameObjectIdFromList(GameObjectType.Tileset, i));
                if (File.Exists("resources/tilesets/" + tileset.Name))
                {
                    try
                    {
                        mTilesetDict[tileset.Name.ToLower()] = new GameTexture("resources/tilesets/" + tileset.Name);
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"Fake methods! ({tileset.Name})");
                        if (exception.InnerException != null) Log.Error(exception.InnerException);
                        Log.Error(exception);
                        throw;
                    }
                    if (!tilesetWarning)
                    {
                        using (var img = Image.FromFile("resources/tilesets/" + tileset.Name))
                        {
                            if (img.Width > 2048 || img.Height > 2048)
                            {
                                badTilesets.Add(tileset.Name);
                            }
                        }
                    }
                }
            }

            if (badTilesets.Count > 0)
            {
                MessageBox.Show(
                    "One or more tilesets is too large and likely won't load for your players on older machines! We recommmend that no graphic is larger than 2048 pixels in width or height.\n\nFaulting tileset(s): " +
                    string.Join(",", badTilesets.ToArray()), "Large Tileset Warning!", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }

            if (newTilesets.Count > 0)
            {
                PacketSender.SendNewTilesets(newTilesets.ToArray());
            }
        }

        private static void LoadItems()
        {
            LoadTextureGroup("items", itemDict);
        }

        private static void LoadEntities()
        {
            LoadTextureGroup("entities", entityDict);
        }

        private static void LoadSpells()
        {
            LoadTextureGroup("spells", spellDict);
        }

        private static void LoadAnimations()
        {
            LoadTextureGroup("animations", animationDict);
        }

        private static void LoadFaces()
        {
            LoadTextureGroup("faces", faceDict);
        }

        private static void LoadImages()
        {
            LoadTextureGroup("images", imageDict);
        }

        private static void LoadFogs()
        {
            LoadTextureGroup("fogs", mFogDict);
        }

        private static void LoadResources()
        {
            LoadTextureGroup("resources", resourceDict);
        }

        private static void LoadPaperdolls()
        {
            LoadTextureGroup("paperdolls", paperdollDict);
        }

        private static void LoadMisc()
        {
            LoadTextureGroup("misc", mMiscDict);
        }

        public static void LoadShaders()
        {
            mShaderDict.Clear();
            if (!Directory.Exists("resources/" + "shaders"))
            {
                Directory.CreateDirectory("resources/" + "shaders");
            }
            var items = Directory.GetFiles("resources/" + "shaders", "*_editor.xnb");
            for (int i = 0; i < items.Length; i++)
            {
                string filename = items[i].Replace("resources/" + "shaders" + "\\", "").ToLower();
                mShaderDict.Add(filename,
                    contentManger.Load<Effect>(RemoveExtension("resources/" + "shaders" + "/" + filename)));
            }
        }

        public static void LoadSounds()
        {
            mSoundDict.Clear();
            if (!Directory.Exists("resources/" + "sounds"))
            {
                Directory.CreateDirectory("resources/" + "sounds");
            }
            var items = Directory.GetFiles("resources/" + "sounds", "*.wav");
            for (int i = 0; i < items.Length; i++)
            {
                string filename = items[i].Replace("resources/" + "sounds" + "\\", "").ToLower();
                mSoundDict.Add(filename, null); //TODO Sound Playback
            }
        }

        public static void LoadMusic()
        {
            mMusicDict.Clear();
            if (!Directory.Exists("resources/" + "music"))
            {
                Directory.CreateDirectory("resources/" + "music");
            }
            var items = Directory.GetFiles("resources/" + "music", "*.ogg");
            for (int i = 0; i < items.Length; i++)
            {
                string filename = items[i].Replace("resources/" + "music" + "\\", "").ToLower();
                mMusicDict.Add(filename, null); //TODO Music Playback
            }
        }

        public static string RemoveExtension(string fileName)
        {
            int fileExtPos = fileName.LastIndexOf(".");
            if (fileExtPos >= 0)
                fileName = fileName.Substring(0, fileExtPos);
            return fileName;
        }

        //Retreiving Game Resources
        //Content Getters
        public static Texture2D GetTexture(TextureType type, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
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
                    textureDict = itemDict;
                    break;
                case TextureType.Entity:
                    textureDict = entityDict;
                    break;
                case TextureType.Spell:
                    textureDict = spellDict;
                    break;
                case TextureType.Animation:
                    textureDict = animationDict;
                    break;
                case TextureType.Face:
                    textureDict = faceDict;
                    break;
                case TextureType.Image:
                    textureDict = imageDict;
                    break;
                case TextureType.Fog:
                    textureDict = mFogDict;
                    break;
                case TextureType.Resource:
                    textureDict = resourceDict;
                    break;
                case TextureType.Paperdoll:
                    textureDict = paperdollDict;
                    break;
                case TextureType.Gui:
                    textureDict = guiDict;
                    break;
                case TextureType.Misc:
                    textureDict = mMiscDict;
                    break;
                default:
                    return null;
            }

            if (textureDict == null) return null;
            if (textureDict == mTilesetDict
            ) //When assigning name in tilebase base we force it to be lowercase.. so lets save some processing time here..
            {
                return textureDict.TryGetValue(name, out GameTexture texture1) ? texture1.GetTexture() : null;
            }
            return textureDict.TryGetValue(name.ToLower(), out GameTexture texture) ? texture.GetTexture() : null;
        }

        public static Effect GetShader(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Log.Error("Tried to load shader with null name.");
                return null;
            }

            if (mShaderDict == null) return null;
            return mShaderDict.TryGetValue(name.ToLower(), out Effect effect) ? effect : null;
        }

        public static object GetMusic(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Log.Error("Tried to load music with null name.");
                return null;
            }

            if (mMusicDict == null) return null;
            return mMusicDict.TryGetValue(name.ToLower(), out object music) ? music : null;
        }

        public static object GetSound(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Log.Error("Tried to load sound with null name.");
                return null;
            }

            if (mSoundDict == null) return null;
            return mSoundDict.TryGetValue(name.ToLower(), out object sound) ? sound : null;
        }

        //Getting Filenames
        public static string[] GetTextureNames(TextureType type)
        {
            IDictionary<string, GameTexture> textureDict = null;
            switch (type)
            {
                case TextureType.Tileset:
                    textureDict = mTilesetDict;
                    break;
                case TextureType.Item:
                    textureDict = itemDict;
                    break;
                case TextureType.Entity:
                    textureDict = entityDict;
                    break;
                case TextureType.Spell:
                    textureDict = spellDict;
                    break;
                case TextureType.Animation:
                    textureDict = animationDict;
                    break;
                case TextureType.Face:
                    textureDict = faceDict;
                    break;
                case TextureType.Image:
                    textureDict = imageDict;
                    break;
                case TextureType.Fog:
                    textureDict = mFogDict;
                    break;
                case TextureType.Resource:
                    textureDict = resourceDict;
                    break;
                case TextureType.Paperdoll:
                    textureDict = paperdollDict;
                    break;
                case TextureType.Gui:
                    textureDict = guiDict;
                    break;
                case TextureType.Misc:
                    textureDict = mMiscDict;
                    break;
                default:
                    return null;
            }

            var keys = textureDict?.Keys.ToArray();
            if (keys != null)
            {
                Array.Sort(keys,new AlphanumComparatorFast());
            }
            return textureDict?.Keys.ToArray();
        }

        public static string[] GetMusicNames() => mMusicDict?.Keys.ToArray();

        public static string[] GetSoundNames() => mSoundDict?.Keys.ToArray();
    }

    internal class DummyGraphicsDeviceManager : IGraphicsDeviceService
    {
        public DummyGraphicsDeviceManager(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
        }

        public GraphicsDevice GraphicsDevice { get; private set; }

        // Not used but required that these be here:
        public event EventHandler<EventArgs> DeviceCreated;

        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;
    }
}