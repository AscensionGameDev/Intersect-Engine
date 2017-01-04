/*
    Intersect Game Engine (Editor)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Intersect_Editor.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Windows.Forms;
using System.Collections.Specialized;
using Intersect_Editor.Classes.Content;
using Intersect_Library;
using Intersect_Library.GameObjects;

namespace Intersect_Editor.Classes.Core
{
    public static class GameContentManager
    {
        //MonoGame Content Manager
        private static ContentManager contentManger;

        //Initial Resource Downloading
        private static string resourceRelayer = "http://ascensiongamedev.com/resources/Intersect/findResources.php";
        private static frmLoadingContent loadingForm;
        private static bool downloadCompleted = false;
        private static string errorString = "";

        //Game Content
        public static List<GameTexture> AllTextures = new List<GameTexture>();
        static Dictionary<string, GameTexture> tilesetDict = new Dictionary<string, GameTexture>();
        static Dictionary<string, GameTexture> itemDict = new Dictionary<string, GameTexture>();
        static Dictionary<string, GameTexture> entityDict = new Dictionary<string, GameTexture>();
        static Dictionary<string, GameTexture> spellDict = new Dictionary<string, GameTexture>();
        static Dictionary<string, GameTexture> animationDict = new Dictionary<string, GameTexture>();
        static Dictionary<string, GameTexture> faceDict = new Dictionary<string, GameTexture>();
        static Dictionary<string, GameTexture> imageDict = new Dictionary<string, GameTexture>();
        static Dictionary<string, GameTexture> fogDict = new Dictionary<string, GameTexture>();
        static Dictionary<string, GameTexture> resourceDict = new Dictionary<string, GameTexture>();
        static Dictionary<string, GameTexture> paperdollDict = new Dictionary<string, GameTexture>();
        static Dictionary<string, GameTexture> guiDict = new Dictionary<string, GameTexture>();
        static Dictionary<string, GameTexture> miscDict = new Dictionary<string, GameTexture>();
        static Dictionary<string, Effect> shaderDict = new Dictionary<string, Effect>();
        static Dictionary<string, object> musicDict = new Dictionary<string, object>();
        static Dictionary<string, object> soundDict = new Dictionary<string, object>();

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

        //Resource Downloader
        public static void CheckForResources()
        {
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
                       { "version", Assembly.GetExecutingAssembly().GetName().Version.ToString() },
                    });
                    string result = Encoding.UTF8.GetString(response);
                    Uri urlResult;
                    if (Uri.TryCreate(result, UriKind.Absolute, out urlResult))
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
        private static void Client_DownloadFileCompleted(global::System.Object sender, global::System.ComponentModel.AsyncCompletedEventArgs e)
        {
            downloadCompleted = true;
            if (!e.Cancelled && e.Error == null)
            {
                try
                {
                    global::System.IO.Compression.ZipFile.ExtractToDirectory("resources.zip",
                        global::System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
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
        private static void Client_DownloadProgressChanged(global::System.Object sender, DownloadProgressChangedEventArgs e)
        {
            loadingForm.SetProgress(e.ProgressPercentage);
        }

        public static void LoadEditorContent()
        {
            //Start by creating a MonoGame Content Manager
            //We create a dummy game service so we can load up a content manager.
            var container = new GameServiceContainer();
            container.AddService(typeof(IGraphicsDeviceService), new DummyGraphicsDeviceManager(EditorGraphics.GetGraphicsDevice()));
            contentManger = new ContentManager(container, "");

            LoadTilesets();
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
        public static void LoadTextureGroup(string directory, Dictionary<string, GameTexture> dict)
        {
            dict.Clear();
            if (!Directory.Exists("resources/" + directory)) { Directory.CreateDirectory("resources/" + directory); }
            var items = Directory.GetFiles("resources/" + directory, "*.png");
            for (int i = 0; i < items.Length; i++)
            {
                string filename = items[i].Replace("resources/" + directory + "\\", "").ToLower();
                dict.Add(filename, new GameTexture("resources/" + directory + "/" + filename));
            }
        }
        public static void LoadTilesets()
        {
            if (!Directory.Exists("resources/tilesets")) { Directory.CreateDirectory("resources/tilesets"); }
            var tilesets = Directory.GetFiles("resources/tilesets", "*.png");
            List<string> newTilesets = new List<string>();
            Array.Sort(tilesets, new AlphanumComparatorFast());
            if (tilesets.Length > 0)
            {
                var tilesetBaseList = Database.GetGameObjectList(GameObject.Tileset);
                for (var i = 0; i < tilesets.Length; i++)
                {
                    tilesets[i] = tilesets[i].Replace("resources/tilesets\\", "");
                    if (tilesetBaseList.Length > 0)
                    {
                        for (var x = 0; x < tilesetBaseList.Length; x++)
                        {
                            if (tilesetBaseList[x] == tilesets[i])
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

            tilesetDict.Clear();
            for (var i = 0; i < TilesetBase.ObjectCount(); i++)
            {
                if (File.Exists("resources/tilesets/" + TilesetBase.GetTileset(Database.GameObjectIdFromList(GameObject.Tileset, i)).Value))
                {
                    tilesetDict.Add(TilesetBase.GetTileset(Database.GameObjectIdFromList(GameObject.Tileset, i)).Value.ToLower(), new GameTexture("resources/tilesets/" + TilesetBase.GetTileset(Database.GameObjectIdFromList(GameObject.Tileset, i)).Value));
                }
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
            LoadTextureGroup("fogs", fogDict);
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
            LoadTextureGroup("misc", miscDict);
        }
        public static void LoadShaders()
        {
            shaderDict.Clear();
            if (!Directory.Exists("resources/" + "shaders")) { Directory.CreateDirectory("resources/" + "shaders"); }
            var items = Directory.GetFiles("resources/" + "shaders", "*.xnb");
            for (int i = 0; i < items.Length; i++)
            {
                string filename = items[i].Replace("resources/" + "shaders" + "\\", "").ToLower();
                shaderDict.Add(filename, contentManger.Load<Effect>(RemoveExtension("resources/" + "shaders" + "/" + filename)));
            }
        }
        public static void LoadSounds()
        {
            soundDict.Clear();
            if (!Directory.Exists("resources/" + "sounds")) { Directory.CreateDirectory("resources/" + "sounds"); }
            var items = Directory.GetFiles("resources/" + "sounds", "*.wav");
            for (int i = 0; i < items.Length; i++)
            {
                string filename = items[i].Replace("resources/" + "sounds" + "\\", "").ToLower();
                soundDict.Add(filename, null); //TODO Sound Playback
            }
        }
        public static void LoadMusic()
        {
            musicDict.Clear();
            if (!Directory.Exists("resources/" + "music")) { Directory.CreateDirectory("resources/" + "music"); }
            var items = Directory.GetFiles("resources/" + "music", "*.ogg");
            for (int i = 0; i < items.Length; i++)
            {
                string filename = items[i].Replace("resources/" + "music" + "\\", "").ToLower();
                musicDict.Add(filename,null); //TODO Music Playback
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
            switch (type)
            {
                case TextureType.Tileset:
                    if (tilesetDict.ContainsKey(name.ToLower())) return tilesetDict[name.ToLower()].GetTexture();
                    break;
                case TextureType.Item:
                    if (itemDict.ContainsKey(name.ToLower())) return itemDict[name.ToLower()].GetTexture();
                    break;
                case TextureType.Entity:
                    if (entityDict.ContainsKey(name.ToLower())) return entityDict[name.ToLower()].GetTexture();
                    break;
                case TextureType.Spell:
                    if (spellDict.ContainsKey(name.ToLower())) return spellDict[name.ToLower()].GetTexture();
                    break;
                case TextureType.Animation:
                    if (animationDict.ContainsKey(name.ToLower())) return animationDict[name.ToLower()].GetTexture();
                    break;
                case TextureType.Face:
                    if (faceDict.ContainsKey(name.ToLower())) return faceDict[name.ToLower()].GetTexture();
                    break;
                case TextureType.Image:
                    if (imageDict.ContainsKey(name.ToLower())) return imageDict[name.ToLower()].GetTexture();
                    break;
                case TextureType.Fog:
                    if (fogDict.ContainsKey(name.ToLower())) return fogDict[name.ToLower()].GetTexture();
                    break;
                case TextureType.Resource:
                    if (resourceDict.ContainsKey(name.ToLower())) return resourceDict[name.ToLower()].GetTexture();
                    break;
                case TextureType.Paperdoll:
                    if (paperdollDict.ContainsKey(name.ToLower())) return paperdollDict[name.ToLower()].GetTexture();
                    break;
                case TextureType.Gui:
                    if (guiDict.ContainsKey(name.ToLower())) return guiDict[name.ToLower()].GetTexture();
                    break;
                case TextureType.Misc:
                    if (miscDict.ContainsKey(name.ToLower())) return miscDict[name.ToLower()].GetTexture();
                    break;
            }
            return null;
        }
        public static Effect GetShader(string name)
        {
            if (shaderDict.ContainsKey(name.ToLower())) return shaderDict[name.ToLower()];
            return null;
        }
        public static object GetMusic(string name)
        {
            if (musicDict.ContainsKey(name.ToLower())) return musicDict[name.ToLower()];
            return null;
        }
        public static object GetSound(string name)
        {
            if (soundDict.ContainsKey(name.ToLower())) return soundDict[name.ToLower()];
            return null;
        }
        //Getting Filenames
        public static string[] GetTextureNames(TextureType type)
        {
            switch (type)
            {
                case TextureType.Tileset:
                    return tilesetDict.Keys.ToArray();
                case TextureType.Item:
                    return itemDict.Keys.ToArray();
                case TextureType.Entity:
                    return entityDict.Keys.ToArray();
                case TextureType.Spell:
                    return spellDict.Keys.ToArray();
                case TextureType.Animation:
                    return animationDict.Keys.ToArray();
                case TextureType.Face:
                    return faceDict.Keys.ToArray();
                case TextureType.Image:
                    return imageDict.Keys.ToArray();
                case TextureType.Fog:
                    return fogDict.Keys.ToArray();
                case TextureType.Resource:
                    return resourceDict.Keys.ToArray();
                case TextureType.Paperdoll:
                    return paperdollDict.Keys.ToArray();
                case TextureType.Gui:
                    return guiDict.Keys.ToArray();
                case TextureType.Misc:
                    return miscDict.Keys.ToArray();
            }
            return null;
        }
        public static string[] GetMusicNames()
        {
            return musicDict.Keys.ToArray();
        }
        public static string[] GetSoundNames()
        {
            return soundDict.Keys.ToArray();
        }
    }

    internal class DummyGraphicsDeviceManager : IGraphicsDeviceService
    {
        public GraphicsDevice GraphicsDevice { get; private set; }

        // Not used:
        public event EventHandler<EventArgs> DeviceCreated;
        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;

        public DummyGraphicsDeviceManager(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
        }
    }
}
