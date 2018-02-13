using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.Graphics;
using Intersect_Client.Classes.Core;
using Intersect_Client_MonoGame.Classes.SFML.Graphics;
using Intersect_MonoGameDx.Classes.SFML.Audio;

namespace Intersect_Client.Classes.Bridges_and_Interfaces.SFML.File_Management
{
    public class MonoContentManager : GameContentManager
    {
        private bool mDownloadCompleted;
        private string mErrorString = "";

        private FrmLoadingContent mLoadingForm;

        //Initial Resource Downloading
        private string mResourceRelayer = "http://ascensiongamedev.com/resources/Intersect/findResources.php";

        public MonoContentManager()
        {
            ServicePointManager.Expect100Continue = false;
            Init(this);
            if (!Directory.Exists("resources"))
            {
                mLoadingForm = new FrmLoadingContent();
                mLoadingForm.Show();
                mLoadingForm.BringToFront();
                using (WebClient client = new WebClient())
                {
                    byte[] response =
                        client.UploadValues(mResourceRelayer, new NameValueCollection()
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
                                mDownloadCompleted = false;
                                mErrorString = "";
                                client.DownloadFileAsync(urlResult, "resources.zip");
                                while (!mDownloadCompleted)
                                {
                                    Application.DoEvents();
                                }
                            }
                            catch (Exception ex)
                            {
                                mErrorString = ex.Message;
                            }
                            if (mErrorString != "")
                            {
                                if (
                                    MessageBox.Show(Strings.Get("resources", "resourceexception", mErrorString),
                                        Strings.Get("resources", "failedtoload"),
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
                        MessageBox.Show(Strings.Get("resources", "resourcesfatal"),
                            Strings.Get("resources", "failedtoload"));
                    }
                }
                mLoadingForm.Close();
            }
            if (!Directory.Exists("resources"))
            {
                Environment.Exit(1);
            }
        }

        private void Client_DownloadFileCompleted(object sender,
            global::System.ComponentModel.AsyncCompletedEventArgs e)
        {
            mDownloadCompleted = true;
            if (!e.Cancelled && e.Error == null)
            {
                try
                {
                    global::System.IO.Compression.ZipFile.ExtractToDirectory("resources.zip",
                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                    File.Delete("resources.zip");
                }
                catch (Exception ex)
                {
                    mErrorString = ex.Message;
                }
            }
            else
            {
                if (e.Cancelled)
                {
                    mErrorString = Strings.Get("resources", "cancelled");
                }
                else
                {
                    mErrorString = e.Error.Message;
                }
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            mLoadingForm.SetProgress(e.ProgressPercentage);
        }

        //Graphic Loading
        public override void LoadTilesets(string[] tilesetnames)
        {
            mTilesetDict.Clear();
            foreach (var t in tilesetnames)
            {
                if (t != "" && File.Exists(Path.Combine("resources", "tilesets", t)) &&
                    !mTilesetDict.ContainsKey(t.ToLower()))
                {
                    mTilesetDict.Add(t.ToLower(),
                        GameGraphics.Renderer.LoadTexture(Path.Combine("resources", "tilesets", t)));
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
            for (int i = 0; i < items.Length; i++)
            {
                string filename = items[i].Replace(dir, "").TrimStart(Path.DirectorySeparatorChar).ToLower();
                dict.Add(filename, GameGraphics.Renderer.LoadTexture(Path.Combine(dir, filename)));
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
            for (int i = 0; i < items.Length; i++)
            {
                string filename = items[i].Replace(dir, "").TrimStart(Path.DirectorySeparatorChar).ToLower();
                GameFont font = GameGraphics.Renderer.LoadFont(Path.Combine(dir, filename));
                if (mFontDict.IndexOf(font) == -1)
                    mFontDict.Add(font);
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
            for (int i = 0; i < items.Length; i++)
            {
                string filename = items[i].Replace(dir, "").TrimStart(Path.DirectorySeparatorChar).ToLower();
                if (!filename.Contains("_editor"))
                {
                    mShaderDict.Add(filename.Replace(".xnb", ""),
                        GameGraphics.Renderer.LoadShader(Path.Combine(dir, filename)));
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
            for (int i = 0; i < items.Length; i++)
            {
                string filename = items[i].Replace(dir, "").TrimStart(Path.DirectorySeparatorChar).ToLower();
                mSoundDict.Add(RemoveExtension(filename),
                    new MonoSoundSource(Path.Combine(dir, filename),
                        ((MonoRenderer) GameGraphics.Renderer).GetContentManager()));
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
            for (int i = 0; i < items.Length; i++)
            {
                string filename = items[i].Replace(dir, "").TrimStart(Path.DirectorySeparatorChar).ToLower();
                mMusicDict.Add(RemoveExtension(filename), new MonoMusicSource(Path.Combine(dir, filename)));
            }
        }
    }
}