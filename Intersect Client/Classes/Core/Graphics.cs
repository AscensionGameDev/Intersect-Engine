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
using SFML.System;

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
        public static FloatRect CurrentView;

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
        public static List<string> ResourceFileNames;
        public static Texture[] ResourceTextures;


        //Darkness Stuff
        public static bool LightsChanged = true;
        public static int DarkOffsetX = 0;
        public static int DarkOffsetY = 0;
        private static Thread LightThread;
        public static RenderTexture DarkCacheTexture;
        private static RenderTexture DarkCacheTextureBackup;
        private static bool UseDarknessBackup = true;
        private static bool SwapDarknessTextures = false;
        public static RenderTexture CurrentDarknexxTexture;
        public static Texture PlayerLightTex;
        public static float SunIntensity;

        //Fog Stuff
        public static int FogOffsetX = 0;
        public static int FogOffsetY = 0;

        //Overlay Stuff
        public static Color OverlayColor = Color.Transparent;
        public static RenderTexture OverlayTexture;
        private static long _overlayUpdate = Environment.TickCount;

        //Player Spotlight Values
        private const float PlayerLightIntensity = .7f;
        private const int PlayerLightSize = 150;
        private const float PlayerLightScale = .6f;

        private static long _fadeTimer;

        //Intro Variables
        private static int IntroIndex = 0;
        private static long IntroTime = -1;
        private static long IntroDelay = 3000;



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
            LoadResources();
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
            if (Globals.GameState != (int)Enums.GameStates.InGame) return;
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
            //if (!RenderWindow.HasFocus()) return;
            RenderWindow.DispatchEvents();
            RenderWindow.Clear(Color.Black);

            if (Globals.GameState == (int)Enums.GameStates.Intro)
            {
                if (ImageFileNames.IndexOf(Globals.IntroBG[IntroIndex]) > -1)
                {
                    DrawFullScreenTextureStretched(ImageTextures[ImageFileNames.IndexOf(Globals.IntroBG[IntroIndex])]);
                }
                if (FadeStage == 0 && IntroTime == -1)
                {
                    IntroTime = Environment.TickCount + IntroDelay;
                }
                else if (FadeStage == 0 && IntroTime > 0)
                {
                    if (IntroTime <= Environment.TickCount)
                    {
                        FadeStage = 2;
                        IntroTime = 0;
                    }
                }
                else if (FadeStage == 2 && FadeAmt == 255 && IntroTime == 0)
                {
                    IntroIndex++;
                    FadeStage = 1;
                    IntroTime = -1;
                    if (IntroIndex >= Globals.IntroBG.Count) { Globals.GameState = (int)Enums.GameStates.Menu; }
                }
            }
            else if (Globals.GameState == (int)Enums.GameStates.Menu)
            {
                if (ImageFileNames.IndexOf(Globals.MenuBG) > -1)
                {
                    DrawFullScreenTexture(ImageTextures[ImageFileNames.IndexOf(Globals.MenuBG)]);
                }
            }
            if (Globals.GameState == (int)Enums.GameStates.InGame && Globals.GameLoaded && Globals.CurrentMap > -1 && Globals.GameMaps[Globals.CurrentMap] != null)
            {
                UpdateView();
                if (LightsChanged)
                {
                    if (LightThread == null)
                    {
                        LightThread = new Thread(InitLighting);
                        //If we don't have a light texture, make a base/blank one.
                        if (DarkCacheTexture == null)
                        {
                            DarkCacheTexture = new RenderTexture((uint)Globals.MapWidth * (uint)Globals.TileWidth * 3, (uint)Globals.MapHeight * (uint)Globals.TileHeight * 3);
                            DarkCacheTextureBackup = new RenderTexture((uint)Globals.MapWidth * (uint)Globals.TileWidth * 3, (uint)Globals.MapHeight * (uint)Globals.TileHeight * 3);
                            CurrentDarknexxTexture = new RenderTexture((uint)Globals.MapWidth * (uint)Globals.TileWidth * 3, (uint)Globals.MapHeight * (uint)Globals.TileHeight * 3);
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
                        LightThread.Start();
                    }
                }

                if (Globals.CurrentMap > -1)
                {
                    if (Globals.GameMaps[Globals.CurrentMap] != null)
                    {
                        if (ImageFileNames.IndexOf(Globals.GameMaps[Globals.CurrentMap].Panorama) > -1)
                        {
                            DrawFullScreenTexture(ImageTextures[ImageFileNames.IndexOf(Globals.GameMaps[Globals.CurrentMap].Panorama)]);
                        }
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

                for (var n = 0; n <= 1; n++)
                {
                    for (var i = 0; i < 9; i++)
                    {
                        if (Globals.LocalMaps[i] <= -1) continue;
                        for (var y = 0; y < Globals.MapHeight; y++)
                        {
                            foreach (var t in Globals.Entities)
                            {
                                if (t == null) continue;
                                if (t.CurrentMap != Globals.LocalMaps[i]) continue;
                                if (t.Passable == n) continue;
                                if (t.CurrentY == y && t.CurrentZ == 0)
                                {
                                    t.Draw(i);
                                }
                            }
                            foreach (var t in Globals.Events)
                            {
                                if (t == null) continue;
                                if (t.CurrentMap != Globals.LocalMaps[i]) continue;
                                if (t.Passable == n) continue;
                                if (t.CurrentY == y)
                                {
                                    t.Draw(i);
                                }
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
                    for (var y = 0; y < Globals.MapHeight; y++)
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
                }

                DrawOverlay();


                for (var i = 0; i < 9; i++)
                {
                    if (Globals.LocalMaps[i] <= -1) continue;
                    for (var y = 0; y < Globals.MapHeight; y++)
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
                DrawDarkness();
            }

            Gui.DrawGui();
            RenderTexture(Gui.GwenTexture.Texture,CurrentView.Left,CurrentView.Top,RenderWindow);


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
                myShape.Position = new Vector2f(CurrentView.Left,CurrentView.Top);
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
        public static void DrawOverlay()
        {
            float ecTime = Environment.TickCount - _overlayUpdate;
            if (OverlayTexture == null)
            {
                //Init Overlay
                OverlayTexture = new RenderTexture(1, 1);
                OverlayTexture.Clear(OverlayColor);
                OverlayTexture.Display();
            }

            if (OverlayColor.A != Globals.GameMaps[Globals.CurrentMap].AHue || OverlayColor.R != Globals.GameMaps[Globals.CurrentMap].RHue ||
                OverlayColor.G != Globals.GameMaps[Globals.CurrentMap].GHue || OverlayColor.B != Globals.GameMaps[Globals.CurrentMap].BHue)
            {
                if (OverlayColor.A < Globals.GameMaps[Globals.CurrentMap].AHue) { 
                    OverlayColor.A += (byte)(255 * ecTime/ 2000f);
                    if (OverlayColor.A > Globals.GameMaps[Globals.CurrentMap].AHue) { OverlayColor.A = (byte)Globals.GameMaps[Globals.CurrentMap].AHue; }
                }
                if (OverlayColor.A > Globals.GameMaps[Globals.CurrentMap].AHue) { 
                    OverlayColor.A -= (byte)(255 * ecTime / 2000f);
                    if (OverlayColor.A < Globals.GameMaps[Globals.CurrentMap].AHue) { OverlayColor.A = (byte)Globals.GameMaps[Globals.CurrentMap].AHue; }
                }
                if (OverlayColor.R < Globals.GameMaps[Globals.CurrentMap].RHue) { 
                    OverlayColor.R += (byte)(255 * ecTime / 2000f);
                    if (OverlayColor.R > Globals.GameMaps[Globals.CurrentMap].RHue) { OverlayColor.R = (byte)Globals.GameMaps[Globals.CurrentMap].RHue; }
                }
                if (OverlayColor.R > Globals.GameMaps[Globals.CurrentMap].RHue) { 
                    OverlayColor.R -= (byte)(255 * ecTime / 2000f);
                    if (OverlayColor.R < Globals.GameMaps[Globals.CurrentMap].RHue) { OverlayColor.R = (byte)Globals.GameMaps[Globals.CurrentMap].RHue; }
                }
                if (OverlayColor.G < Globals.GameMaps[Globals.CurrentMap].GHue)
                {
                    OverlayColor.G += (byte)(255 * ecTime / 2000f);
                    if (OverlayColor.G > Globals.GameMaps[Globals.CurrentMap].GHue) { OverlayColor.G = (byte)Globals.GameMaps[Globals.CurrentMap].GHue; }
                }
                if (OverlayColor.G > Globals.GameMaps[Globals.CurrentMap].GHue)
                {
                    OverlayColor.G -= (byte)(255 * ecTime / 2000f);
                    if (OverlayColor.G < Globals.GameMaps[Globals.CurrentMap].GHue) { OverlayColor.G = (byte)Globals.GameMaps[Globals.CurrentMap].GHue; }
                }
                if (OverlayColor.B < Globals.GameMaps[Globals.CurrentMap].BHue)
                {
                    OverlayColor.B += (byte)(255 * ecTime / 2000f);
                    if (OverlayColor.B > Globals.GameMaps[Globals.CurrentMap].BHue) { OverlayColor.B = (byte)Globals.GameMaps[Globals.CurrentMap].BHue; }
                }
                if (OverlayColor.B > Globals.GameMaps[Globals.CurrentMap].BHue)
                {
                    OverlayColor.B -= (byte)(255 * ecTime / 2000f);
                    if (OverlayColor.B < Globals.GameMaps[Globals.CurrentMap].BHue) { OverlayColor.B = (byte)Globals.GameMaps[Globals.CurrentMap].BHue; }
                }
                OverlayTexture.Clear(OverlayColor);
                OverlayTexture.Display();
            }

            RenderTexture(OverlayTexture.Texture, new Rectangle(0, 0, 1, 1), new Rectangle(0, 0, (int)RenderWindow.Size.X, (int)RenderWindow.Size.Y), RenderWindow);
            _overlayUpdate = Environment.TickCount;
        }
        public static void DrawFullScreenTexture(Texture tex)
        {
            int bgx = (int)(RenderWindow.Size.X / 2 - tex.Size.X / 2);
            int bgy = (int)(RenderWindow.Size.Y / 2 - tex.Size.Y / 2);
            int bgw = (int)tex.Size.X;
            int bgh = (int)tex.Size.Y;
            int diff = 0;
            if (bgw < RenderWindow.Size.X)
            {
                diff = (int)(RenderWindow.Size.X - bgw);
                bgx -= diff / 2;
                bgw += diff;
            }
            if (bgh < RenderWindow.Size.Y)
            {
                diff = (int)(RenderWindow.Size.Y - bgh);
                bgy -= diff / 2;
                bgh += diff;
            }
            RenderTexture(tex, new Rectangle(0, 0, (int)tex.Size.X, (int)tex.Size.Y),
                new Rectangle(bgx, bgy, bgw, bgh), RenderWindow);
        }
        public static void DrawFullScreenTextureStretched(Texture tex)
        {
            RenderTexture(tex, new Rectangle(0, 0, (int)tex.Size.X, (int)tex.Size.Y),
                new Rectangle(0, 0, ScreenWidth, ScreenHeight), RenderWindow);
        }

        private static void UpdateView()
        {
            Player en = (Player) Globals.Entities[Globals.MyIndex];
            CurrentView = new FloatRect(en.GetCenterPos(4).X - ScreenWidth/2f,en.GetCenterPos(4).Y - ScreenHeight/2f,ScreenWidth,ScreenHeight);
            RenderWindow.SetView(new View(CurrentView));
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
            FogTextures = new Texture[fogs.Length];
            for (int i = 0; i < fogs.Length; i++)
            {
                FogFileNames.Add(fogs[i].Replace("Resources/Fogs\\", ""));
                FogTextures[i] = new Texture(new Image("Resources/Fogs/" + FogFileNames[i]));
            }
        }
        private static void LoadResources()
        {
            if (!Directory.Exists("Resources/Resources")) { Directory.CreateDirectory("Resources/Resources"); }
            var resources = Directory.GetFiles("Resources/Resources", "*.png");
            ResourceFileNames = new List<string>();
            ResourceTextures = new Texture[resources.Length];
            for (int i = 0; i < resources.Length; i++)
            {
                ResourceFileNames.Add(resources[i].Replace("Resources/Resources\\", ""));
                ResourceTextures[i] = new Texture(new Image("Resources/Resources/" + ResourceFileNames[i]));
            }
        }

        //Lighting
        private static void InitLighting()
        {
            return;
            RenderTexture tmpTex;
            do
            {
                if (LightsChanged)
                {
                    LightsChanged = false;
                    if (UseDarknessBackup)
                    {
                        tmpTex = DarkCacheTexture;
                    }
                    else
                    {
                        tmpTex = DarkCacheTextureBackup;
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
                                    break;
                                }
                            }
                            else
                            {
                                LightsChanged = true;
                                break;
                            }
                        }
                        byte val = (byte)(((float)Globals.GameMaps[Globals.LocalMaps[4]].Brightness / 100f) * 255f);
                        tmpTex.Clear(new Color(val, val, val, 255));

                        if (!LightsChanged)
                        {
                            //Render each light.
                            for (var z = 0; z < 9; z++)
                            {
                                if (Globals.LocalMaps[z] <= -1 || Globals.LocalMaps[z] >= Globals.GameMaps.Count()) continue;
                                if (Globals.GameMaps[Globals.LocalMaps[z]] == null) continue;
                                if (!Globals.GameMaps[Globals.LocalMaps[z]].MapLoaded) continue;
                                foreach (var t in Globals.GameMaps[Globals.LocalMaps[z]].Lights)
                                {
                                    if (LightsChanged) { break; }
                                    double w = CalcLightWidth(t.Range);
                                    var x = CalcMapOffsetX(z) + Globals.MapWidth * Globals.TileWidth + (t.TileX * Globals.TileWidth + t.OffsetX) - (int)w / 2 + 16;
                                    var y = CalcMapOffsetY(z) + Globals.MapHeight * Globals.TileHeight + (t.TileY * Globals.TileHeight + t.OffsetY) - (int)w / 2 + 16;
                                    AddLight((int)x, (int)y, (int)w, t.Intensity, t, tmpTex);
                                }
                            }
                            tmpTex.Display();
                            tmpTex.SetActive(false);
                        }
                    }
                    SwapDarknessTextures = true;
                    UseDarknessBackup = !UseDarknessBackup;
                }
                if (!LightsChanged) { System.Threading.Thread.Sleep(1); }
            } while (GameMain.IsRunning);
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
        private static void DrawDarkness()
        {
            return;
            if (Globals.GameMaps[Globals.CurrentMap].IsIndoors) { return; }
            var rs = new RectangleShape(new Vector2f(3 * Globals.TileWidth * Globals.MapWidth, 3 * Globals.TileHeight * Globals.MapHeight));
            if (CurrentDarknexxTexture == null) { return; }
            CurrentDarknexxTexture.Clear(Color.Transparent);

            if (UseDarknessBackup)
            {
                if (SwapDarknessTextures)
                {
                    DarkCacheTextureBackup.SetActive(true);
                    DarkCacheTexture.SetActive(false);
                    SwapDarknessTextures = false;
                    DarkOffsetX = 0;
                    DarkOffsetY = 0;
                }
                RenderTexture(DarkCacheTextureBackup.Texture, 0, 0, CurrentDarknexxTexture); //Draw our cached map lights

            }
            else
            {
                if (SwapDarknessTextures)
                {
                    DarkCacheTextureBackup.SetActive(false);
                    DarkCacheTexture.SetActive(true);
                    SwapDarknessTextures = false;
                    DarkOffsetX = 0;
                    DarkOffsetY = 0;
                }
                RenderTexture(DarkCacheTexture.Texture, 0, 0, CurrentDarknexxTexture); //Draw our cached map lights

            }


            //Draw the light around the player (if any)
            if (PlayerLightTex != null)
            {
                RenderTexture(PlayerLightTex, (int)
                                Math.Ceiling(-DarkOffsetX + Globals.Entities[Globals.MyIndex].GetCenterPos(4).X - PlayerLightTex.Size.X / 2 +
                                             Globals.MapWidth * Globals.TileWidth), (int)
                                Math.Ceiling(-DarkOffsetY + Globals.Entities[Globals.MyIndex].GetCenterPos(4).Y - PlayerLightTex.Size.Y / 2 +
                                             Globals.MapHeight * Globals.TileHeight), CurrentDarknexxTexture, BlendMode.Add);
            }
            rs.FillColor = new Color(255, 255, 255, (byte)(SunIntensity * 255));    //Draw a rectangle, the opacity indicates if it is day or night.
            CurrentDarknexxTexture.Draw(rs, new RenderStates(BlendMode.Add));
            CurrentDarknexxTexture.Display();
            RenderTexture(CurrentDarknexxTexture.Texture, CalcMapOffsetX(0) + DarkOffsetX, CalcMapOffsetY(0) + DarkOffsetY, RenderWindow, BlendMode.Multiply);
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
                myList.Add(VideoMode.FullscreenModes[i]);
            }
            myList.Reverse();
            return myList;
        }
        public static float CalcMapOffsetX(int i)
        {
            if (i < 3)
            {
                return ((-Globals.MapWidth * Globals.TileWidth) + ((i) * (Globals.MapWidth * Globals.TileWidth)));
            }
            if (i < 6)
            {
                return ((-Globals.MapWidth * Globals.TileWidth) + ((i - 3) * (Globals.MapWidth * Globals.TileWidth)));
            }
            return ((-Globals.MapWidth * Globals.TileWidth) + ((i - 6) * (Globals.MapWidth * Globals.TileWidth)));
        }
        public static float CalcMapOffsetY(int i)
        {
            if (i < 3)
            {
                return -Globals.MapHeight * Globals.TileHeight;
            }
            if (i < 6)
            {
                return 0;
            }
            return Globals.MapHeight * Globals.TileHeight;
        }

        //Rendering Functions
        public static void RenderTexture(Texture tex, float x, float y, RenderTarget renderTarget)
        {
            var destRectangle = new RectangleF(x, y, (int)tex.Size.X, (int)tex.Size.Y);
            var srcRectangle = new RectangleF(0, 0, (int)tex.Size.X, (int)tex.Size.Y);
            RenderTexture(tex, srcRectangle, destRectangle, renderTarget);
        }
        public static void RenderTexture(Texture tex, float x, float y, RenderTarget renderTarget, BlendMode blendMode)
        {
            var destRectangle = new RectangleF(x, y, (int)tex.Size.X, (int)tex.Size.Y);
            var srcRectangle = new RectangleF(0, 0, (int)tex.Size.X, (int)tex.Size.Y);
            RenderTexture(tex, srcRectangle, destRectangle, renderTarget, blendMode);
        }
        public static void RenderTexture(Texture tex, float dx, float dy, float sx, float sy, float w, float h, RenderTarget renderTarget)
        {
            var destRectangle = new RectangleF(dx, dy, w, h);
            var srcRectangle = new RectangleF(sx, sy, w, h);
            RenderTexture(tex, srcRectangle, destRectangle, renderTarget);
        }
        public static void RenderTexture(Texture tex, RectangleF srcRectangle, RectangleF targetRect, RenderTarget renderTarget)
        {
            RenderTexture(tex, srcRectangle, targetRect, renderTarget, BlendMode.Alpha);
        }
        public static void RenderTexture(Texture tex, RectangleF srcRectangle, RectangleF targetRect, RenderTarget renderTarget, BlendMode blendMode)
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
