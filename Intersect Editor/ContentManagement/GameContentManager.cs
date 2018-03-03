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
using Intersect.Editor.Core;
using Intersect.Editor.Forms;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Logging;
using Intersect.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Editor.ContentManagement
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
        private static ContentManager sContentManger;

        //Initial Resource Downloading
        private static string sResourceRelayer = "http://ascensiongamedev.com/resources/Intersect/findResources.php";

        private static FrmLoadingContent sLoadingForm;
        private static bool sDownloadCompleted;
        private static string sErrorString = "";

        //Game Content
        public static List<GameTexture> AllTextures = new List<GameTexture>();

        static IDictionary<string, GameTexture> sTilesetDict = new Dictionary<string, GameTexture>();
        static IDictionary<string, GameTexture> sItemDict = new Dictionary<string, GameTexture>();
        static IDictionary<string, GameTexture> sEntityDict = new Dictionary<string, GameTexture>();
        static IDictionary<string, GameTexture> sPellDict = new Dictionary<string, GameTexture>();
        static IDictionary<string, GameTexture> sAnimationDict = new Dictionary<string, GameTexture>();
        static IDictionary<string, GameTexture> sFaceDict = new Dictionary<string, GameTexture>();
        static IDictionary<string, GameTexture> sImageDict = new Dictionary<string, GameTexture>();
        static IDictionary<string, GameTexture> sFogDict = new Dictionary<string, GameTexture>();
        static IDictionary<string, GameTexture> sResourceDict = new Dictionary<string, GameTexture>();
        static IDictionary<string, GameTexture> sPaperdollDict = new Dictionary<string, GameTexture>();
        static IDictionary<string, GameTexture> sGuiDict = new Dictionary<string, GameTexture>();
        static IDictionary<string, GameTexture> sMiscDict = new Dictionary<string, GameTexture>();
        static IDictionary<string, Effect> sHaderDict = new Dictionary<string, Effect>();
        static IDictionary<string, object> sMusicDict = new Dictionary<string, object>();
        static IDictionary<string, object> sOundDict = new Dictionary<string, object>();

        //Resource Downloader
        public static void CheckForResources()
        {
            ServicePointManager.Expect100Continue = false;
            if (!Directory.Exists("resources"))
            {
                sLoadingForm = new FrmLoadingContent();
                sLoadingForm.Show();
                sLoadingForm.BringToFront();
                using (WebClient client = new WebClient())
                {
                    byte[] response =
                        client.UploadValues(sResourceRelayer, new NameValueCollection()
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
                                sDownloadCompleted = false;
                                sErrorString = "";
                                client.DownloadFileAsync(urlResult, "resources.zip");
                                while (!sDownloadCompleted)
                                {
                                    Application.DoEvents();
                                }
                            }
                            catch (Exception ex)
                            {
                                sErrorString = ex.Message;
                            }
                            if (sErrorString != "")
                            {
                                if (
                                    MessageBox.Show(
                                        "Failed to download client resources.\n\nException Info: " + sErrorString +
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
                sLoadingForm.Close();
            }
            if (!Directory.Exists("resources"))
            {
                Environment.Exit(1);
            }
        }

        private static void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            sDownloadCompleted = true;
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
                    sErrorString = ex.Message;
                }
            }
            else
            {
                if (e.Cancelled)
                {
                    sErrorString = "Download was cancelled!";
                }
                else
                {
                    sErrorString = e.Error.Message;
                }
            }
        }

        private static void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            sLoadingForm.SetProgress(e.ProgressPercentage);
        }

        public static void LoadEditorContent()
        {
            //Start by creating a MonoGame Content Manager
            //We create a dummy game service so we can load up a content manager.
            var container = new GameServiceContainer();
            container.AddService(typeof(IGraphicsDeviceService),
                new DummyGraphicsDeviceManager(EditorGraphics.GetGraphicsDevice()));
            sContentManger = new ContentManager(container, "");
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

            sTilesetDict.Clear();
            var badTilesets = new List<string>();
            for (var i = 0; i < TilesetBase.Lookup.Count; i++)
            {
                var tileset =
                    TilesetBase.Lookup.Get<TilesetBase>(Database.GameObjectIdFromList(GameObjectType.Tileset, i));
                if (File.Exists("resources/tilesets/" + tileset.Name))
                {
                    try
                    {
                        sTilesetDict[tileset.Name.ToLower()] = new GameTexture("resources/tilesets/" + tileset.Name);
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
            LoadTextureGroup("items", sItemDict);
        }

        private static void LoadEntities()
        {
            LoadTextureGroup("entities", sEntityDict);
        }

        private static void LoadSpells()
        {
            LoadTextureGroup("spells", sPellDict);
        }

        private static void LoadAnimations()
        {
            LoadTextureGroup("animations", sAnimationDict);
        }

        private static void LoadFaces()
        {
            LoadTextureGroup("faces", sFaceDict);
        }

        private static void LoadImages()
        {
            LoadTextureGroup("images", sImageDict);
        }

        private static void LoadFogs()
        {
            LoadTextureGroup("fogs", sFogDict);
        }

        private static void LoadResources()
        {
            LoadTextureGroup("resources", sResourceDict);
        }

        private static void LoadPaperdolls()
        {
            LoadTextureGroup("paperdolls", sPaperdollDict);
        }

        private static void LoadMisc()
        {
            LoadTextureGroup("misc", sMiscDict);
        }

        public static void LoadShaders()
        {
            sHaderDict.Clear();
            if (!Directory.Exists("resources/" + "shaders"))
            {
                Directory.CreateDirectory("resources/" + "shaders");
            }
            var items = Directory.GetFiles("resources/" + "shaders", "*_editor.xnb");
            for (int i = 0; i < items.Length; i++)
            {
                string filename = items[i].Replace("resources/" + "shaders" + "\\", "").ToLower();
                sHaderDict.Add(filename,
                    sContentManger.Load<Effect>(RemoveExtension("resources/" + "shaders" + "/" + filename)));
            }
        }

        public static void LoadSounds()
        {
            sOundDict.Clear();
            if (!Directory.Exists("resources/" + "sounds"))
            {
                Directory.CreateDirectory("resources/" + "sounds");
            }
            var items = Directory.GetFiles("resources/" + "sounds", "*.wav");
            for (int i = 0; i < items.Length; i++)
            {
                string filename = items[i].Replace("resources/" + "sounds" + "\\", "").ToLower();
                sOundDict.Add(filename, null); //TODO Sound Playback
            }
        }

        public static void LoadMusic()
        {
            sMusicDict.Clear();
            if (!Directory.Exists("resources/" + "music"))
            {
                Directory.CreateDirectory("resources/" + "music");
            }
            var items = Directory.GetFiles("resources/" + "music", "*.ogg");
            for (int i = 0; i < items.Length; i++)
            {
                string filename = items[i].Replace("resources/" + "music" + "\\", "").ToLower();
                sMusicDict.Add(filename, null); //TODO Music Playback
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
                    textureDict = sTilesetDict;
                    break;
                case TextureType.Item:
                    textureDict = sItemDict;
                    break;
                case TextureType.Entity:
                    textureDict = sEntityDict;
                    break;
                case TextureType.Spell:
                    textureDict = sPellDict;
                    break;
                case TextureType.Animation:
                    textureDict = sAnimationDict;
                    break;
                case TextureType.Face:
                    textureDict = sFaceDict;
                    break;
                case TextureType.Image:
                    textureDict = sImageDict;
                    break;
                case TextureType.Fog:
                    textureDict = sFogDict;
                    break;
                case TextureType.Resource:
                    textureDict = sResourceDict;
                    break;
                case TextureType.Paperdoll:
                    textureDict = sPaperdollDict;
                    break;
                case TextureType.Gui:
                    textureDict = sGuiDict;
                    break;
                case TextureType.Misc:
                    textureDict = sMiscDict;
                    break;
                default:
                    return null;
            }

            if (textureDict == null) return null;
            if (textureDict == sTilesetDict
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

            if (sHaderDict == null) return null;
            return sHaderDict.TryGetValue(name.ToLower(), out Effect effect) ? effect : null;
        }

        public static object GetMusic(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Log.Error("Tried to load music with null name.");
                return null;
            }

            if (sMusicDict == null) return null;
            return sMusicDict.TryGetValue(name.ToLower(), out object music) ? music : null;
        }

        public static object GetSound(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Log.Error("Tried to load sound with null name.");
                return null;
            }

            if (sOundDict == null) return null;
            return sOundDict.TryGetValue(name.ToLower(), out object sound) ? sound : null;
        }

        public static string[] GetSmartSortedTextureNames(TextureType type) => SmartSort(GetTextureNames(type));

        //Getting Filenames
        public static string[] GetTextureNames(TextureType type)
        {
            IDictionary<string, GameTexture> textureDict = null;
            switch (type)
            {
                case TextureType.Tileset:
                    textureDict = sTilesetDict;
                    break;
                case TextureType.Item:
                    textureDict = sItemDict;
                    break;
                case TextureType.Entity:
                    textureDict = sEntityDict;
                    break;
                case TextureType.Spell:
                    textureDict = sPellDict;
                    break;
                case TextureType.Animation:
                    textureDict = sAnimationDict;
                    break;
                case TextureType.Face:
                    textureDict = sFaceDict;
                    break;
                case TextureType.Image:
                    textureDict = sImageDict;
                    break;
                case TextureType.Fog:
                    textureDict = sFogDict;
                    break;
                case TextureType.Resource:
                    textureDict = sResourceDict;
                    break;
                case TextureType.Paperdoll:
                    textureDict = sPaperdollDict;
                    break;
                case TextureType.Gui:
                    textureDict = sGuiDict;
                    break;
                case TextureType.Misc:
                    textureDict = sMiscDict;
                    break;
                default:
                    return null;
            }

            var keys = textureDict?.Keys.ToArray();
            if (keys != null)
            {
                Array.Sort(keys, new AlphanumComparatorFast());
            }
            return textureDict?.Keys.ToArray();
        }

        public static string[] MusicNames => sMusicDict?.Keys.ToArray();

        public static string[] SmartSortedMusicNames => SmartSort(MusicNames);

        public static string[] SoundNames => sOundDict?.Keys.ToArray();

        public static string[] SmartSortedSoundNames => SmartSort(SoundNames);

        private static string[] SmartSort(string[] strings)
        {
            var sortedStrings = strings ?? new string[] { };
            Array.Sort(sortedStrings, new AlphanumComparator());
            return sortedStrings;
        }
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