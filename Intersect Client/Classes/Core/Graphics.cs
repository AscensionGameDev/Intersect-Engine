/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using SFML.Graphics;
using SFML.Window;
using Color = SFML.Graphics.Color;
using Font = SFML.Graphics.Font;
using Image = SFML.Graphics.Image;
using KeyEventArgs = SFML.Window.KeyEventArgs;
using View = SFML.Graphics.View;

namespace Intersect_Client.Classes
{
    public static class Graphics
    {
        public static RenderWindow RenderWindow;

        //Screen Values
        public static int ScreenWidth;
        public static int ScreenHeight;
        public static int DisplayMode;
        public static bool FullScreen = false;
        public static bool MustReInit;
        public static int FadeStage = 1;
        public static float FadeAmt = 255f;
        public static FrmGame MyForm;
        public static List<Keyboard.Key> KeyStates = new List<Keyboard.Key>();
        public static List<Mouse.Button> MouseState = new List<Mouse.Button>();
        public static Font GameFont;
        public static int Fps;
        private static int _fpsCount;
        private static long _fpsTimer;
        private static RenderStates _renderState = new RenderStates(BlendMode.Alpha);

        //Game Textures
        public static List<Texture> Tilesets = new List<Texture>();
        public static List<string> ItemFileNames;
        public static Texture[] ItemTextures;
        public static List<string> EntityFileNames;
        public static Texture[] EntityTextures;
        public static List<string> SpellFileNames;
        public static Texture[] SpellTextures;
        public static List<string> AnimationFileNames;
        public static Texture[] AnimationTextures;
        public static List<string> FaceFileNames;
        public static Texture[] FaceTextures;
        public static List<string> ImageFileNames;
        public static Texture[] ImageTextures;
        public static List<string> FogFileNames;
        public static Texture[] FogTextures;


        //DayNight Stuff
        public static bool LightsChanged = true;
        public static int NightOffsetX = 0;
        public static int NightOffsetY = 0;
        private static Thread LightThread;
        public static RenderTexture NightCacheTexture;
        private static RenderTexture NightCacheTextureBackup;
        private static bool UseNightBackup = true;
        private static bool SwapNightTextures = false;
        public static RenderTexture CurrentNightTexture;
        public static Image NightImg;
        public static Texture PlayerLightTex;
        public static float SunIntensity;

        //Player Spotlight Values
        private const float PlayerLightIntensity = .7f;
        private const int PlayerLightSize = 150;
        private const float PlayerLightScale = .6f;

        private static long _fadeTimer;


        //Init Functions
        public static void InitGraphics()
        {
            InitSfml();
            LoadEntities();
            LoadItems();
            LoadAnimations();
            LoadSpells();
            LoadFaces();
            LoadImages();
            LoadFogs();
            GameFont = new Font("Arvo-Regular.ttf");
        }
        private static void InitSfml()
        {
            if (DisplayMode < 0 || DisplayMode >= GetValidVideoModes().Count) { DisplayMode = 0; }
            MyForm = new FrmGame();
            if (GetValidVideoModes().Any())
            {
                MyForm.Width = (int)GetValidVideoModes()[DisplayMode].Width;
                MyForm.Height = (int)GetValidVideoModes()[DisplayMode].Height;
                MyForm.Text = @"Intersect Client";
                RenderWindow = new RenderWindow(MyForm.Handle);
                if (FullScreen)
                {
                    MyForm.TopMost = true;
                    MyForm.FormBorderStyle = FormBorderStyle.None;
                    MyForm.WindowState = FormWindowState.Maximized;
                    RenderWindow.SetView(new View(new FloatRect(0, 0, (int)GetValidVideoModes()[DisplayMode].Width, (int)GetValidVideoModes()[DisplayMode].Height)));
                }
                else
                {
                    RenderWindow.SetView(new View(new FloatRect(0, 0, MyForm.ClientSize.Width, MyForm.ClientSize.Height)));
                }

            }
            else
            {
                MyForm.Width = 800;
                MyForm.Height = 640;
                MyForm.Text = @"Intersect Client";
                RenderWindow = new RenderWindow(MyForm.Handle);
                RenderWindow.SetView(new View(new FloatRect(0, 0, MyForm.ClientSize.Width, MyForm.ClientSize.Height)));
            }
            if (FullScreen)
            {
                ScreenWidth = (int)GetValidVideoModes()[DisplayMode].Width;
                ScreenHeight = (int)GetValidVideoModes()[DisplayMode].Height;
            }
            else
            {
                ScreenWidth = MyForm.ClientSize.Width;
                ScreenHeight = MyForm.ClientSize.Height;
            }
            RenderWindow.KeyPressed += renderWindow_KeyPressed;
            RenderWindow.KeyReleased += renderWindow_KeyReleased;
            RenderWindow.MouseButtonPressed += renderWindow_MouseButtonPressed;
            RenderWindow.MouseButtonReleased += renderWindow_MouseButtonReleased;
            Gui.InitGwen();
        }

        //GUI Input Events
        static void renderWindow_MouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            while (MouseState.Remove(e.Button)) { }
        }
        static void renderWindow_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            MouseState.Add(e.Button);
        }
        static void renderWindow_KeyReleased(object sender, KeyEventArgs e)
        {
            while (KeyStates.Remove(e.Code)) { }
        }
        static void renderWindow_KeyPressed(object sender, KeyEventArgs e)
        {
            KeyStates.Add(e.Code);
            if (e.Code != Keyboard.Key.Return) return;
            if (Globals.GameState != 1) return;
            if (Gui.HasInputFocus() == false)
            {
                Gui._GameGui.FocusChat = true;
            }
        }

        //Game Rendering
        public static void DrawGame()
        {
            if (MustReInit)
            {
                Gui.DestroyGwen();
                RenderWindow.Close();
                MyForm.Close();
                MyForm.Dispose();
                Gui.SetupHandlers = false;
                InitSfml();
                MustReInit = false;
            }
            if (!RenderWindow.IsOpen()) return;
            RenderWindow.DispatchEvents();
            RenderWindow.Clear(Color.Black);

            if (Globals.GameState == 1 && Globals.GameLoaded)
            {
                if (LightsChanged)
                {
                    if (LightThread == null)
                    {
                        LightThread = new Thread(InitLighting);
                        //If we don't have a light texture, make a base/blank one.
                        if (NightCacheTexture == null)
                        {
                            NightCacheTexture = new RenderTexture(Constants.MapWidth * 32 * 3, Constants.MapHeight * 32 * 3);
                            NightCacheTextureBackup = new RenderTexture(Constants.MapWidth * 32 * 3, Constants.MapHeight * 32 * 3);
                            CurrentNightTexture = new RenderTexture(Constants.MapWidth * 32 * 3, Constants.MapHeight * 32 * 3);
                            var size = CalcLightWidth(PlayerLightSize);
                            var tmpLight = new Bitmap(size, size);
                            var g = System.Drawing.Graphics.FromImage(tmpLight);
                            var pth = new GraphicsPath();
                            pth.AddEllipse(0, 0, size - 1, size - 1);
                            var pgb = new PathGradientBrush(pth)
                            {
                                CenterColor =
                                    System.Drawing.Color.FromArgb((int)(255 * PlayerLightIntensity), (int)(255 * PlayerLightIntensity),
                                        (int)(255 * PlayerLightIntensity), (int)(255 * PlayerLightIntensity)),
                                SurroundColors = new[] { System.Drawing.Color.Black },
                                FocusScales = new PointF(PlayerLightScale, PlayerLightScale)
                            };
                            g.FillPath(pgb, pth);
                            g.Dispose();
                            PlayerLightTex = TexFromBitmap(tmpLight);
                        }
                    }
                    if (LightThread.ThreadState == ThreadState.Running) { LightThread.Abort(); }
                    if (LightThread.ThreadState == ThreadState.Stopped || LightThread.ThreadState == ThreadState.Aborted || LightThread.ThreadState == ThreadState.Suspended || LightThread.ThreadState == ThreadState.Unstarted)
                    {
                        LightThread = new Thread(InitLighting);
                        LightThread.Start();
                        LightsChanged = false;
                    }

                }
                //Render players, names, maps, etc.
                for (var i = 0; i < 9; i++)
                {
                    if (Globals.LocalMaps[i] > -1)
                    {
                        DrawMap(i, 0); //Lower only

                    }
                }

                for (var i = 0; i < 9; i++)
                {
                    if (Globals.LocalMaps[i] <= -1) continue;
                    for (var y = 0; y < Constants.MapHeight; y++)
                    {
                        foreach (var t in Globals.Entities)
                        {
                            if (t == null) continue;
                            if (t.CurrentMap != Globals.LocalMaps[i]) continue;
                            if (t.CurrentY == y && t.CurrentZ == 0)
                            {
                                t.Draw(i);
                            }
                        }
                        foreach (var t in Globals.Events)
                        {
                            if (t == null) continue;
                            if (t.CurrentMap != Globals.LocalMaps[i]) continue;
                            if (t.CurrentY == y)
                            {
                                t.Draw(i);
                            }
                        }
                    }
                }

                //Render the upper layer
                for (var i = 0; i < 9; i++)
                {
                    if (Globals.LocalMaps[i] > -1)
                    {
                        DrawMap(i, 1); //Upper only

                    }
                }

                for (var i = 0; i < 9; i++)
                {
                    if (Globals.LocalMaps[i] <= -1) continue;
                    for (var y = 0; y < Constants.MapHeight; y++)
                    {
                        foreach (var t in Globals.Entities)
                        {
                            if (t == null) continue;
                            if (t.CurrentMap != Globals.LocalMaps[i]) continue;
                            if (t.CurrentY == y && t.CurrentZ == 1)
                            {
                                t.Draw(i);
                            }
                        }
                    }
                }

                for (var i = 0; i < 9; i++)
                {
                    if (Globals.LocalMaps[i] <= -1) continue;
                    DrawMap(i, 2); //Peak Layers

                    for (var y = 0; y < Constants.MapHeight; y++)
                    {
                        foreach (var t in Globals.Entities)
                        {
                            if (t == null) continue;
                            if (t.CurrentMap != Globals.LocalMaps[i]) continue;
                            if (t.CurrentY != y) continue;
                            t.DrawName(i, false);
                            t.DrawHpBar(i);
                        }
                        foreach (var t in Globals.Events)
                        {
                            if (t == null) continue;
                            if (t.CurrentMap != Globals.LocalMaps[i]) continue;
                            if (t.CurrentY == y)
                            {
                                t.DrawName(i, true);
                            }
                        }
                    }
                }
                DrawNight();
            }
            else
            {
                if (ImageFileNames.IndexOf(Globals.MenuBG) > -1)
                {
                    int bgx = (int)(RenderWindow.Size.X / 2 - ImageTextures[ImageFileNames.IndexOf(Globals.MenuBG)].Size.X / 2);
                    int bgy = (int)(RenderWindow.Size.Y / 2 - ImageTextures[ImageFileNames.IndexOf(Globals.MenuBG)].Size.Y / 2);
                    int bgw = (int)ImageTextures[ImageFileNames.IndexOf(Globals.MenuBG)].Size.X;
                    int bgh = (int)ImageTextures[ImageFileNames.IndexOf(Globals.MenuBG)].Size.Y;
                    int diff = 0;
                    if (bgw < RenderWindow.Size.X){
                        diff = (int)(RenderWindow.Size.X - bgw);
                        bgx -= diff/2;
                        bgw += diff;
                    }
                    if (bgh < RenderWindow.Size.Y){
                        diff = (int)(RenderWindow.Size.Y - bgh);
                        bgy -= diff/2;
                        bgh += diff;
                    }
                    RenderTexture(ImageTextures[ImageFileNames.IndexOf(Globals.MenuBG)], new Rectangle(0, 0, (int)ImageTextures[ImageFileNames.IndexOf(Globals.MenuBG)].Size.X, (int)ImageTextures[ImageFileNames.IndexOf(Globals.MenuBG)].Size.Y),
                        new Rectangle(bgx, bgy, bgw, bgh), RenderWindow);
                }
            }

            Gui.DrawGui();


            if (FadeStage != 0)
            {
                if (_fadeTimer < Environment.TickCount)
                {
                    if (FadeStage == 1)
                    {
                        FadeAmt -= 2f;
                        if (FadeAmt <= 0)
                        {
                            FadeStage = 0;
                            FadeAmt = 0f;
                        }
                    }
                    else
                    {
                        FadeAmt += 2f;
                        if (FadeAmt >= 255)
                        {
                            FadeAmt = 255f;
                        }
                    }
                    _fadeTimer = Environment.TickCount + 10;
                }
                var myShape = new RectangleShape(new Vector2f(ScreenWidth, ScreenHeight))
                {
                    FillColor = new Color(0, 0, 0, (byte)FadeAmt)
                };
                RenderWindow.Draw(myShape);
            }

            RenderWindow.Display();
            _fpsCount++;
            if (_fpsTimer < Environment.TickCount)
            {
                Fps = _fpsCount;
                _fpsCount = 0;
                _fpsTimer = Environment.TickCount + 1000;
                RenderWindow.SetTitle("Intersect Engine - Brought to you by: http://ascensiongamedev.com - FPS: " + Fps);
            }
        }
        private static void DrawMap(int index, int layer = 0)
        {
            var mapoffsetx = CalcMapOffsetX(index);
            var mapoffsety = CalcMapOffsetY(index);

            if (Globals.LocalMaps[index] > Globals.GameMaps.Count() || Globals.LocalMaps[index] < 0) return;
            if (Globals.GameMaps[Globals.LocalMaps[index]] == null) return;
            if (Globals.GameMaps[Globals.LocalMaps[index]].MapLoaded)
            {
                Globals.GameMaps[Globals.LocalMaps[index]].Draw(mapoffsetx, mapoffsety, layer);
            }
        }

        //Graphic Loading
        public static void LoadTilesets(string[] tilesetnames)
        {
            foreach (var t in tilesetnames)
            {
                if (t == "")
                {
                    Tilesets.Add(null);
                }
                else
                {
                    if (!File.Exists("Resources/Tilesets/" + t))
                    {
                        Tilesets.Add(null);
                    }
                    else
                    {
                        Tilesets.Add(new Texture("Resources/Tilesets/" + t));
                    }
                }
            }
        }
        private static void LoadItems()
        {
            if (!Directory.Exists("Resources/Items")) { Directory.CreateDirectory("Resources/Items"); }
            var items = Directory.GetFiles("Resources/Items", "*.png");
            ItemFileNames = new List<string>();
            ItemTextures = new Texture[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                ItemFileNames.Add(items[i].Replace("Resources/Items\\", ""));
                ItemTextures[i] = new Texture(new Image("Resources/Items/" + ItemFileNames[i]));
            }
        }
        private static void LoadEntities()
        {
            if (!Directory.Exists("Resources/Entities")) { Directory.CreateDirectory("Resources/Entities"); }
            var chars = Directory.GetFiles("Resources/Entities", "*.png");
            EntityFileNames = new List<string>();
            EntityTextures = new Texture[chars.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                EntityFileNames.Add(chars[i].Replace("Resources/Entities\\", ""));
                EntityTextures[i] = new Texture(new Image("Resources/Entities/" + EntityFileNames[i]));
            }
        }
        private static void LoadSpells()
        {
            if (!Directory.Exists("Resources/Spells")) { Directory.CreateDirectory("Resources/Spells"); }
            var spells = Directory.GetFiles("Resources/Spells", "*.png");
            SpellFileNames = new List<string>();
            SpellTextures = new Texture[spells.Length];
            for (int i = 0; i < spells.Length; i++)
            {
                SpellFileNames.Add(spells[i].Replace("Resources/Spells\\", ""));
                SpellTextures[i] = new Texture(new Image("Resources/Spells/" + SpellFileNames[i]));
            }
        }
        private static void LoadAnimations()
        {
            if (!Directory.Exists("Resources/Animations")) { Directory.CreateDirectory("Resources/Animations"); }
            var animations = Directory.GetFiles("Resources/Animations", "*.png");
            AnimationFileNames = new List<string>();
            AnimationTextures = new Texture[animations.Length];
            for (int i = 0; i < animations.Length; i++)
            {
                AnimationFileNames.Add(animations[i].Replace("Resources/Animations\\", ""));
                AnimationTextures[i] = new Texture(new Image("Resources/Animations/" + AnimationFileNames[i]));
            }
        }
        private static void LoadFaces()
        {
            if (!Directory.Exists("Resources/Faces")) { Directory.CreateDirectory("Resources/Faces"); }
            var faces = Directory.GetFiles("Resources/Faces", "*.png");
            FaceFileNames = new List<string>();
            FaceTextures = new Texture[faces.Length];
            for (int i = 0; i < faces.Length; i++)
            {
                FaceFileNames.Add(faces[i].Replace("Resources/Faces\\", ""));
                FaceTextures[i] = new Texture(new Image("Resources/Faces/" + FaceFileNames[i]));
            }
        }
        private static void LoadImages()
        {
            if (!Directory.Exists("Resources/Images")) { Directory.CreateDirectory("Resources/Images"); }
            var images = Directory.GetFiles("Resources/Images", "*.png");
            ImageFileNames = new List<string>();
            ImageTextures = new Texture[images.Length];
            for (int i = 0; i < images.Length; i++)
            {
                ImageFileNames.Add(images[i].Replace("Resources/Images\\", ""));
                ImageTextures[i] = new Texture(new Image("Resources/Images/" + ImageFileNames[i]));
            }
        }
        private static void LoadFogs()
        {
            if (!Directory.Exists("Resources/Fogs")) { Directory.CreateDirectory("Resources/Fogs"); }
            var fogs = Directory.GetFiles("Resources/Fogs", "*.png");
            FogFileNames = new List<string>();
            FogTextures  = new Texture[fogs.Length];
            for (int i = 0; i < fogs.Length; i++)
            {
                FogFileNames.Add(fogs[i].Replace("Resources/Fogs\\", ""));
                FogTextures[i] = new Texture(new Image("Resources/Fogs/" + FogFileNames[i]));
            }
        }

        //Lighting
        private static void InitLighting()
        {
            RenderTexture tmpTex;
            if (UseNightBackup)
            {
                tmpTex = NightCacheTexture;
            }
            else
            {
                tmpTex = NightCacheTextureBackup;
            }

            lock (tmpTex)
            {
                tmpTex.SetActive(true);
                //If loading maps still, dont make the texture, no point
                for (var i = 0; i < 9; i++)
                {
                    if (Globals.LocalMaps[i] <= -1 || Globals.LocalMaps[i] >= Globals.GameMaps.Count()) continue;
                    if (Globals.GameMaps[Globals.LocalMaps[i]] != null)
                    {
                        if (Globals.GameMaps[Globals.LocalMaps[i]].MapLoaded)
                        {

                        }
                        else
                        {
                            LightsChanged = true;
                            return;
                        }
                    }
                    else
                    {
                        LightsChanged = true;
                        return;
                    }
                }
                tmpTex.Clear(new Color(30, 30, 30, 255));

                //Render each light.
                for (var z = 0; z < 9; z++)
                {
                    if (Globals.LocalMaps[z] <= -1 || Globals.LocalMaps[z] >= Globals.GameMaps.Count()) continue;
                    if (Globals.GameMaps[Globals.LocalMaps[z]] == null) continue;
                    if (!Globals.GameMaps[Globals.LocalMaps[z]].MapLoaded) continue;
                    foreach (var t in Globals.GameMaps[Globals.LocalMaps[z]].Lights)
                    {
                        double w = CalcLightWidth(t.Range);
                        var x = CalcMapOffsetX(z, true) + Constants.MapWidth * 32 + (t.TileX * 32 + t.OffsetX) - (int)w / 2 + 16;
                        var y = CalcMapOffsetY(z, true) + Constants.MapHeight * 32 + (t.TileY * 32 + t.OffsetY) - (int)w / 2 + 16;
                        AddLight(x, y, (int)w, t.Intensity, t, tmpTex);
                    }
                }
                tmpTex.Display();
                tmpTex.SetActive(false);
            }
            SwapNightTextures = true;
            UseNightBackup = !UseNightBackup;
        }
        private static int CalcLightWidth(int range)
        {
            //Formula that is ~equilivant to Unity spotlight widths, this is so future Unity lighting is possible.
            int[] xVals = { 0, 5, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180 };
            int[] yVals = { 1, 8, 18, 34, 50, 72, 92, 114, 135, 162, 196, 230, 268, 320, 394, 500, 658, 976, 1234, 1600 };
            int w;
            var x = 0;
            while (range >= xVals[x])
            {
                x++;
            }
            if (x > yVals.Length)
            {
                w = yVals[yVals.Length - 1];
            }
            else
            {
                w = yVals[x - 1];
                w += (int)((range - xVals[x - 1]) / ((float)xVals[x] - xVals[x - 1])) * (yVals[x] - yVals[x - 1]);
            }
            return w;
        }
        private static void DrawNight()
        {
            if (Globals.GameMaps[Globals.CurrentMap].IsIndoors) { return; } //Don't worry about day or night if indoors
            var rs = new RectangleShape(new Vector2f(3 * 32 * Constants.MapWidth, 3 * 32 * Constants.MapHeight));
            if (CurrentNightTexture == null) { return; }
            CurrentNightTexture.Clear(Color.Transparent);

            if (UseNightBackup)
            {
                if (SwapNightTextures)
                {
                    NightCacheTextureBackup.SetActive(true);
                    NightCacheTexture.SetActive(false);
                    SwapNightTextures = false;
                    NightOffsetX = 0;
                    NightOffsetY = 0;
                }
                RenderTexture(NightCacheTextureBackup.Texture, 0, 0, CurrentNightTexture); //Draw our cached map lights

            }
            else
            {
                if (SwapNightTextures)
                {
                    NightCacheTextureBackup.SetActive(false);
                    NightCacheTexture.SetActive(true);
                    SwapNightTextures = false;
                    NightOffsetX = 0;
                    NightOffsetY = 0;
                }
                RenderTexture(NightCacheTexture.Texture, 0, 0, CurrentNightTexture); //Draw our cached map lights

            }


            //Draw the light around the player (if any)
            if (PlayerLightTex != null)
            {
                RenderTexture(PlayerLightTex, (int)
                                Math.Ceiling(-NightOffsetX + Globals.Entities[Globals.MyIndex].GetCenterPos(4).X - PlayerLightTex.Size.X / 2 +
                                             Constants.MapWidth * 32), (int)
                                Math.Ceiling(-NightOffsetY + Globals.Entities[Globals.MyIndex].GetCenterPos(4).Y - PlayerLightTex.Size.Y / 2 +
                                             Constants.MapHeight * 32), CurrentNightTexture, BlendMode.Add);
            }
            rs.FillColor = new Color(255, 255, 255, (byte)(SunIntensity * 255));    //Draw a rectangle, the opacity indicates if it is day or night.
            CurrentNightTexture.Draw(rs, new RenderStates(BlendMode.Add));
            CurrentNightTexture.Display();
            RenderTexture(CurrentNightTexture.Texture, CalcMapOffsetX(0) + NightOffsetX, CalcMapOffsetY(0) + NightOffsetY, RenderWindow, BlendMode.Multiply);
        }
        private static void AddLight(int x1, int y1, int size, double intensity, LightObj light, RenderTexture myTex)
        {
            Bitmap tmpLight;
            //If not cached, create a radial gradent for the light.
            if (light.Graphic == null)
            {
                tmpLight = new Bitmap(size, size);
                var g = System.Drawing.Graphics.FromImage(tmpLight);
                var pth = new GraphicsPath();
                pth.AddEllipse(0, 0, size - 1, size - 1);
                var pgb = new PathGradientBrush(pth)
                {
                    CenterColor = System.Drawing.Color.FromArgb((int)(255 * intensity), 255, 255, 255),
                    SurroundColors = new[] { System.Drawing.Color.Transparent },
                    FocusScales = new PointF(0.8f, 0.8f)
                };
                g.FillPath(pgb, pth);
                g.Dispose();
                light.Graphic = tmpLight;
            }
            else
            {
                tmpLight = light.Graphic;
            }

            var tmpSprite = new Sprite(TexFromBitmap(tmpLight)) { Position = new Vector2f(x1, y1) };
            myTex.Draw(tmpSprite, new RenderStates(BlendMode.Add));
        }

        //Helper Functions
        private static Texture TexFromBitmap(Bitmap bmp)
        {
            var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            return new Texture(ms);
        }
        public static List<VideoMode> GetValidVideoModes()
        {
            var myList = new List<VideoMode>();
            for (var i = 0; i < VideoMode.FullscreenModes.Length; i++)
            {
                if (VideoMode.FullscreenModes[i].BitsPerPixel == 32)
                {
                    myList.Add(VideoMode.FullscreenModes[i]);
                }
            }
            myList.Reverse();
            return myList;
        }
        public static int CalcMapOffsetX(int i, bool ignorePlayerOffset = false)
        {
            if (i < 3)
            {
                if (ignorePlayerOffset)
                {
                    return ((-Constants.MapWidth * 32) + ((i) * (Constants.MapWidth * 32)));
                }
                return ((-Constants.MapWidth * 32) + ((i) * (Constants.MapWidth * 32))) + (ScreenWidth / 2) - Globals.Entities[Globals.MyIndex].CurrentX * 32 - (int)Math.Ceiling(Globals.Entities[Globals.MyIndex].OffsetX);
            }
            if (i < 6)
            {
                if (ignorePlayerOffset)
                {
                    return ((-Constants.MapWidth * 32) + ((i - 3) * (Constants.MapWidth * 32)));
                }
                return ((-Constants.MapWidth * 32) + ((i - 3) * (Constants.MapWidth * 32))) + (ScreenWidth / 2) - Globals.Entities[Globals.MyIndex].CurrentX * 32 - (int)Math.Ceiling(Globals.Entities[Globals.MyIndex].OffsetX);
            }
            if (ignorePlayerOffset)
            {
                return ((-Constants.MapWidth * 32) + ((i - 6) * (Constants.MapWidth * 32)));
            }
            return ((-Constants.MapWidth * 32) + ((i - 6) * (Constants.MapWidth * 32))) + (ScreenWidth / 2) - Globals.Entities[Globals.MyIndex].CurrentX * 32 - (int)Math.Ceiling(Globals.Entities[Globals.MyIndex].OffsetX);
        }
        public static int CalcMapOffsetY(int i, bool ignorePlayerOffset = false)
        {
            if (i < 3)
            {
                if (ignorePlayerOffset)
                {
                    return -Constants.MapHeight * 32;
                }
                return -Constants.MapHeight * 32 + (ScreenHeight / 2) - Globals.Entities[Globals.MyIndex].CurrentY * 32 - (int)Math.Ceiling(Globals.Entities[Globals.MyIndex].OffsetY);
            }
            if (i < 6)
            {
                if (ignorePlayerOffset)
                {
                    return 0;
                }
                return 0 + (ScreenHeight / 2) - Globals.Entities[Globals.MyIndex].CurrentY * 32 - (int)Math.Ceiling(Globals.Entities[Globals.MyIndex].OffsetY);
            }
            if (ignorePlayerOffset)
            {
                return Constants.MapHeight * 32;
            }
            return Constants.MapHeight * 32 + (ScreenHeight / 2) - Globals.Entities[Globals.MyIndex].CurrentY * 32 - (int)Math.Ceiling(Globals.Entities[Globals.MyIndex].OffsetY);
        }

        //Rendering Functions
        public static void RenderTexture(Texture tex, int x, int y, RenderTarget renderTarget)
        {
            var destRectangle = new Rectangle(x, y, (int)tex.Size.X, (int)tex.Size.Y);
            var srcRectangle = new Rectangle(0, 0, (int)tex.Size.X, (int)tex.Size.Y);
            RenderTexture(tex, srcRectangle, destRectangle, renderTarget);
        }
        public static void RenderTexture(Texture tex, int x, int y, RenderTarget renderTarget, BlendMode blendMode)
        {
            var destRectangle = new Rectangle(x, y, (int)tex.Size.X, (int)tex.Size.Y);
            var srcRectangle = new Rectangle(0, 0, (int)tex.Size.X, (int)tex.Size.Y);
            RenderTexture(tex, srcRectangle, destRectangle, renderTarget, blendMode);
        }
        public static void RenderTexture(Texture tex, int dx, int dy, int sx, int sy, int w, int h, RenderTarget renderTarget)
        {
            var destRectangle = new Rectangle(dx, dy, w, h);
            var srcRectangle = new Rectangle(sx, sy, w, h);
            RenderTexture(tex, srcRectangle, destRectangle, renderTarget);
        }
        public static void RenderTexture(Texture tex, Rectangle srcRectangle, Rectangle targetRect, RenderTarget renderTarget, BlendMode blendMode = BlendMode.Alpha)
        {
            var vertexCache = new Vertex[4];
            var u1 = (float)srcRectangle.X / tex.Size.X;
            var v1 = (float)srcRectangle.Y / tex.Size.Y;
            var u2 = (float)srcRectangle.Right / tex.Size.X;
            var v2 = (float)srcRectangle.Bottom / tex.Size.Y;


            u1 *= tex.Size.X;
            v1 *= tex.Size.Y;
            u2 *= tex.Size.X;
            v2 *= tex.Size.Y;

            _renderState.BlendMode = blendMode;

            if (_renderState.Texture == null || _renderState.Texture != tex)
            {
                // enable the new texture
                _renderState.Texture = tex;
            }

            var right = targetRect.X + targetRect.Width;
            var bottom = targetRect.Y + targetRect.Height;

            vertexCache[0] = new Vertex(new Vector2f(targetRect.X, targetRect.Y), new Vector2f(u1, v1));
            vertexCache[1] = new Vertex(new Vector2f(right, targetRect.Y), new Vector2f(u2, v1));
            vertexCache[2] = new Vertex(new Vector2f(right, bottom), new Vector2f(u2, v2));
            vertexCache[3] = new Vertex(new Vector2f(targetRect.X, bottom), new Vector2f(u1, v2));
            renderTarget.Draw(vertexCache, 0, 4, PrimitiveType.Quads, _renderState);
            renderTarget.ResetGLStates();
        }
    }
}
