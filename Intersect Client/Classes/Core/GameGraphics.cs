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
using System.Diagnostics;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace Intersect_Client.Classes
{
    public static class Graphics
    {
        public static RenderWindow RenderWindow;

        //Screen Values
        public static int ScreenWidth;
        public static int ScreenHeight;
        public static int DisplayMode = 0;
        public static bool FullScreen = false;
        public static bool MustReInit;
        public static int FadeStage = 1;
        public static float FadeAmt = 255f;
        public static List<Keyboard.Key> KeyStates = new List<Keyboard.Key>();
        public static List<Mouse.Button> MouseState = new List<Mouse.Button>();
        public static Font GameFont;
        public static int FpsLimit;
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
        public static List<string> PaperdollFileNames;
        public static Texture[] PaperdollTextures;


        //Darkness Stuff
        public static int DarkOffsetX = 0;
        public static int DarkOffsetY = 0;
        public static float SunIntensity;
        public static RenderTexture DarknessTexture;
        private static Shader RadialGradientShader;
        private static string LightingFragmentShader = @"
            uniform vec4 color;
            uniform float expand;
            uniform vec2 center;
            uniform float radius;
            uniform float windowHeight;
            void main()
            {
	            vec2 centerFromSfml = vec2(center.x, windowHeight - center.y);
	            vec2 p = (gl_FragCoord.xy - centerFromSfml) / radius;
                float r = sqrt(dot(p, p));
                if (r < 1.0)
                {
                    gl_FragColor = mix(color, gl_Color, (r - expand) / (1 - expand));
                }
                else
                {
                    gl_FragColor = gl_Color;
                }
            }";

        private static string LightingVertexShader = @"
            void main()
            {
                gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
                gl_TexCoord[0] = gl_TextureMatrix[0] * gl_MultiTexCoord0;
                gl_FrontColor = gl_Color;
            }";

        //Fog Stuff
        public static int FogOffsetX = 0;
        public static int FogOffsetY = 0;

        //Overlay Stuff
        public static Color OverlayColor = Color.Transparent;
        public static RenderTexture OverlayTexture;
        private static long _overlayUpdate = Environment.TickCount;

        //Player Spotlight Values
        private const byte PlayerLightIntensity = 255;
        private const int PlayerLightSize = 300;
        private const float PlayerLightExpand = 0f;
        public static Color PlayerLightColor = Color.White;

        private static long _fadeTimer;

        //Intro Variables
        private static int IntroIndex = 0;
        private static long IntroTime = -1;
        private static long IntroDelay = 3000;

        //Rendering Variables
        private static Vertex[] _vertexCache = new Vertex[1024];
        private static int _vertexCount = 0;
        private static Texture _curTexture;
        public static int DrawCalls = 0;
        public static int EntitiesDrawn = 0;
        public static int MapsDrawn = 0;

        //Cache the Y based rendering
        public static List<Entity>[] Layer1Entities;
        public static List<Entity>[] Layer2Entities;

        public static bool PreRenderedMapLayer = false;
        public static object GFXLock = new Object();
        public static List<RenderTexture> MapReleaseQueue = new List<RenderTexture>(); 
        public static List<RenderTexture> FreeMapTextures = new List<RenderTexture>(); 

        //Init Functions
        public static void InitGraphics()
        {
            InitSfml();
            if (Globals.RenderCaching) CreateMapTextures(9 * 18);
            LoadEntities();
            LoadItems();
            LoadAnimations();
            LoadSpells();
            LoadFaces();
            LoadImages();
            LoadFogs();
            LoadResources();
            LoadPaperdolls();
            InitRenderingLists();
            GameFont = new Font("Arvo-Regular.ttf");
        }
        private static void InitSfml()
        {
            if (DisplayMode < 0 || DisplayMode >= GetValidVideoModes().Count) { DisplayMode = GetValidVideoModes().Count -1; }
            if (GetValidVideoModes().Any())
            {
                
                if (FullScreen)
                {
                    RenderWindow = new RenderWindow(GetValidVideoModes()[DisplayMode], "Intersect Engine", Styles.Fullscreen);
                }
                else
                {
                    RenderWindow = new RenderWindow(GetValidVideoModes()[DisplayMode], "Intersect Engine", Styles.Titlebar);
                }
                ScreenWidth = (int)RenderWindow.Size.X;
                ScreenHeight = (int)RenderWindow.Size.Y;
                if (FpsLimit == 0)
                {
                    RenderWindow.SetVerticalSyncEnabled(true);
                }
                else if (FpsLimit == 1)
                {
                    RenderWindow.SetFramerateLimit(30);
                }
                else if (FpsLimit == 2)
                {
                    RenderWindow.SetFramerateLimit(60);
                }
                else if (FpsLimit == 3)
                {
                    RenderWindow.SetFramerateLimit(90);
                }
                else if (FpsLimit == 4)
                {
                    RenderWindow.SetFramerateLimit(120);
                }
            }
            RenderWindow.KeyPressed += renderWindow_KeyPressed;
            RenderWindow.KeyReleased += renderWindow_KeyReleased;
            RenderWindow.MouseButtonPressed += renderWindow_MouseButtonPressed;
            RenderWindow.MouseButtonReleased += renderWindow_MouseButtonReleased;

            //Init the lighting shader on load.
            //Load Lighting Shader
            if (Shader.IsAvailable)
            {
                RadialGradientShader = Shader.FromString(LightingVertexShader, LightingFragmentShader);
            }
            else
            {
                MessageBox.Show(@"This machine is not able to load SFML Shaders and cannot run this application.");
                Application.Exit();
            }

            CurrentView = new FloatRect(0,0,ScreenWidth,ScreenHeight);
            Gui.InitGwen();
            if (LimitResolution())
            {
                return;}
        }

        public static void FixResolution()
        {
            int maxx = Globals.MapWidth * Globals.TileWidth;
            int maxy = Globals.MapHeight * Globals.TileHeight;
            if (ScreenWidth > maxx || ScreenHeight > maxy)
            {
                int z = 0;
                if (GetValidVideoModes().Any())
                {
                    for (int i = 0; i < GetValidVideoModes().Count; i++)
                    {
                        if (GetValidVideoModes()[i].Width <= maxx && GetValidVideoModes()[i].Height <= maxy && GetValidVideoModes()[i].Width <= ScreenWidth && GetValidVideoModes()[i].Height <= ScreenHeight)
                        {
                            z = i;
                        }
                    }
                    DisplayMode = z;
                    Database.SaveOptions();
                    Graphics.MustReInit = true;
                }
            }
        }

        public static bool LimitResolution()
        {
            int maxx = (Globals.MapWidth-1)* Globals.TileWidth * 2;
            int maxy = (Globals.MapHeight-1) * Globals.TileHeight * 2;
            if (ScreenWidth > maxx || ScreenHeight > maxy)
            {
                int z = 0;
                if (GetValidVideoModes().Any())
                {
                    for (int i = 0; i < GetValidVideoModes().Count; i++)
                    {
                        if (GetValidVideoModes()[i].Width <= maxx && GetValidVideoModes()[i].Height <= maxy && GetValidVideoModes()[i].Width <= ScreenWidth && GetValidVideoModes()[i].Height <= ScreenHeight)
                        {
                            z = i;
                        }
                    }
                    DisplayMode = z;
                    Database.SaveOptions();
                    Graphics.MustReInit = true;
                    return true;
                }
            }
            return false;
        }

        private static void InitRenderingLists()
        {
            Layer1Entities = new List<Entity>[Globals.MapHeight * 3];
            Layer2Entities = new List<Entity>[Globals.MapHeight * 3];
            for (var i = 0; i < Globals.MapHeight * 3; i++)
            {
                Layer1Entities[i] = new List<Entity>();
                Layer2Entities[i] = new List<Entity>();
            }
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
                Gui.SetupHandlers = false;
                InitSfml();
                MustReInit = false;
            }
            RenderWindow.Clear(Color.Black);
            ClearDarknessTexture();
            DrawCalls = 0;
            MapsDrawn = 0;
            EntitiesDrawn = 0;
            PreRenderedMapLayer = false;
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
            if (Globals.GameState == (int)Enums.GameStates.InGame && Globals.GameLoaded && Globals.CurrentMap > -1 && Globals.GameMaps.ContainsKey(Globals.CurrentMap))
            {
                UpdateView();

                if (Globals.RenderCaching)
                {
                    for (var i = 0; i < 9; i++)
                    {
                        if (Globals.LocalMaps[i] > -1)
                        {
                            if (Globals.GameMaps.ContainsKey(Globals.LocalMaps[i]) && !Globals.GameMaps[Globals.LocalMaps[i]].MapRendered)
                            {
                                if (!Graphics.PreRenderedMapLayer) { Globals.GameMaps[Globals.LocalMaps[i]].PreRenderMap(); }
                            }
                        }
                    }
                }


                RenderWindow.Clear(Color.Black);

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


                for (int x = 0; x < Layer1Entities.Length; x++)
                {
                    for (int y = 0; y < Layer1Entities[x].Count; y++)
                    {
                        Layer1Entities[x][y].Draw();
                        EntitiesDrawn++;
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

                for (int x = 0; x < Layer2Entities.Length; x++)
                {
                    for (int y = 0; y < Layer2Entities[x].Count; y++)
                    {
                        Layer2Entities[x][y].Draw();
                        EntitiesDrawn++;
                    }
                }

                for (var i = 0; i < 9; i++)
                {
                    if (Globals.LocalMaps[i] <= -1) continue;
                    DrawMap(i, 2); //Peak Layers
                }

                DrawOverlay();

                RenderCurrentBatch();

                for (int x = 0; x < Layer1Entities.Length; x++)
                {
                    for (int y = 0; y < Layer1Entities[x].Count; y++)
                    {
                        Layer1Entities[x][y].DrawName();
                        if (Layer1Entities[x][y].GetType() != typeof(Event)) 
                        { 
                            Layer1Entities[x][y].DrawHpBar();
                            Layer1Entities[x][y].DrawCastingBar();
                        }
                    }
                }
                for (int x = 0; x < Layer2Entities.Length; x++)
                {
                    for (int y = 0; y < Layer2Entities[x].Count; y++)
                    {
                        Layer2Entities[x][y].DrawName();
                        if (Layer2Entities[x][y].GetType() != typeof(Event)) 
                        { 
                            Layer2Entities[x][y].DrawHpBar();
                            Layer2Entities[x][y].DrawCastingBar();
                        }
                    }
                }
                RenderCurrentBatch();
                OverlayDarkness();
                RenderCurrentBatch();
            }

            Gui.DrawGui();
            RenderTexture(Gui.GwenTexture.Texture,CurrentView.Left,CurrentView.Top,RenderWindow);

            RenderCurrentBatch();

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
            RenderWindow.DispatchEvents();
            _fpsCount++;
            if (_fpsTimer < Environment.TickCount)
            {
                Fps = _fpsCount;
                _fpsCount = 0;
                _fpsTimer = Environment.TickCount + 1000;
                RenderWindow.SetTitle("Intersect Engine");
            }
        }
        private static void DrawMap(int index, int layer = 0)
        {
            if (Globals.LocalMaps[index] < 0) return;
            if (!Globals.GameMaps.ContainsKey(Globals.LocalMaps[index])) return;
            if (!new FloatRect(Globals.GameMaps[Globals.LocalMaps[index]].GetX(), Globals.GameMaps[Globals.LocalMaps[index]].GetY(), Globals.TileWidth * Globals.MapWidth, Globals.TileHeight * Globals.MapHeight).Intersects(CurrentView)) return;
            Globals.GameMaps[Globals.LocalMaps[index]].Draw(layer);
            if (layer == 0) { MapsDrawn++; }
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
            View newView;
            Player en = (Player) Globals.Entities[Globals.MyIndex];
            float x = Globals.GameMaps[Globals.CurrentMap].GetX() - Globals.MapWidth * Globals.TileWidth;
            float y = Globals.GameMaps[Globals.CurrentMap].GetY() - Globals.MapHeight * Globals.TileHeight;
            float x1 = Globals.GameMaps[Globals.CurrentMap].GetX() + (Globals.MapWidth * Globals.TileWidth) * 2;
            float y1 = Globals.GameMaps[Globals.CurrentMap].GetY() + (Globals.MapHeight * Globals.TileHeight) * 2;
            if (Globals.GameMaps[Globals.CurrentMap].HoldUp == 1) { y += Globals.MapHeight * Globals.TileHeight; }
            if (Globals.GameMaps[Globals.CurrentMap].HoldLeft == 1) { x += Globals.MapWidth * Globals.TileWidth; }
            if (Globals.GameMaps[Globals.CurrentMap].HoldRight == 1) { x1 -= Globals.MapWidth * Globals.TileWidth; }
            if (Globals.GameMaps[Globals.CurrentMap].HoldDown == 1) { y1 -= Globals.MapHeight * Globals.TileHeight; }
            float w = x1 - x;
            float h = y1 - y;
            var RestrictView = new FloatRect(x, y, w, h);
            CurrentView = new FloatRect((int)Math.Ceiling(en.GetCenterPos().X - ScreenWidth / 2f), (int)Math.Ceiling(en.GetCenterPos().Y - ScreenHeight / 2f), (int)ScreenWidth, (int)ScreenHeight);
            if (RestrictView.Width >= CurrentView.Width)
            {
                if (CurrentView.Left < RestrictView.Left)
                {
                    CurrentView.Left = RestrictView.Left;
                }
                if (CurrentView.Left + CurrentView.Width > RestrictView.Left + RestrictView.Width)
                {
                    CurrentView.Left -= (CurrentView.Left + CurrentView.Width) - (RestrictView.Left + RestrictView.Width);
                }
            }
            if (RestrictView.Height >= CurrentView.Height)
            {
                if (CurrentView.Top < RestrictView.Top)
                {
                    CurrentView.Top = RestrictView.Top;
                }
                if (CurrentView.Top + CurrentView.Height > RestrictView.Top + RestrictView.Height)
                {
                    CurrentView.Top -= (CurrentView.Top + CurrentView.Height) - (RestrictView.Top + RestrictView.Height);
                }
            }
            newView = new View(CurrentView);
            newView.Zoom(1f);
            RenderWindow.SetView(newView);
        }

        private static void CreateMapTextures(int count)
        {
            for (int i = 0; i < count; i++)
            {
                ReleaseMapTexture(new RenderTexture((uint) Globals.TileWidth*(uint) Globals.MapWidth,
                    (uint) Globals.TileHeight*(uint) Globals.MapHeight));
            }
        }
        public static bool GetMapTexture(ref RenderTexture replaceme)
        {
            if (FreeMapTextures.Count > 0)
            {
                replaceme = FreeMapTextures[0];
                FreeMapTextures.RemoveAt(0);
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void ReleaseMapTexture(RenderTexture releaseTex)
        {
            if (releaseTex.SetActive(false))
            {
                FreeMapTextures.Add(releaseTex);
            }
            else
            {
                Debug.Print("Error!");
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
        private static void LoadPaperdolls()
        {
            if (!Directory.Exists("Resources/Paperdolls")) { Directory.CreateDirectory("Resources/Paperdolls"); }
            var resources = Directory.GetFiles("Resources/Paperdolls", "*.png");
            PaperdollFileNames = new List<string>();
            PaperdollTextures = new Texture[resources.Length];
            for (int i = 0; i < resources.Length; i++)
            {
                PaperdollFileNames.Add(resources[i].Replace("Resources/Paperdolls\\", ""));
                PaperdollTextures[i] = new Texture(new Image("Resources/Paperdolls/" + PaperdollFileNames[i]));
            }
        }

        //Lighting
        private static void ClearDarknessTexture()
        {
            if (DarknessTexture == null)
            {
                DarknessTexture = new RenderTexture((uint)ScreenWidth, (uint)ScreenHeight);
            }
            DarknessTexture.Clear(Color.Black);
        }
        private static void OverlayDarkness()
        {
            if (Globals.GameMaps[Globals.CurrentMap].IsIndoors) { return; }
            if (DarknessTexture == null) { return; }
            var rs = new RectangleShape(new Vector2f(DarknessTexture.Size.X, DarknessTexture.Size.Y));

            //Draw Light Around Player
            DrawLight((int)Math.Ceiling(-DarkOffsetX + Globals.Entities[Globals.MyIndex].GetCenterPos().X - PlayerLightSize / 2 ), (int)
                                Math.Ceiling(-DarkOffsetY + Globals.Entities[Globals.MyIndex].GetCenterPos().Y - PlayerLightSize / 2 ), (int)PlayerLightSize,PlayerLightIntensity,PlayerLightExpand, PlayerLightColor);

            rs.FillColor = new Color(255, 255, 255, (byte)(SunIntensity * 255));    //Draw a rectangle, the opacity indicates if it is day or night.
            //CurrentDarknessTexture.Draw(rs);
            DarknessTexture.Display();
            RenderTexture(DarknessTexture.Texture, CurrentView.Left, CurrentView.Top, RenderWindow, BlendMode.Multiply);
        }
        public static void DrawLight(int x, int y, int size, byte intensity, float Expand, Color color)
        {
            CircleShape circle = new CircleShape(size);
            circle.Origin = new Vector2f(circle.Radius, circle.Radius);
            x -= (int)CurrentView.Left;
            y -= (int)CurrentView.Top;
            circle.Position = new Vector2f(x + size/2,
                y + size/2);
            circle.FillColor = Color.Transparent;
            RadialGradientShader.SetParameter("color", new Color(color.R,color.G,color.B,intensity));
            RadialGradientShader.SetParameter("center", circle.Position);
            RadialGradientShader.SetParameter("radius", circle.Radius);
            RadialGradientShader.SetParameter("expand", Expand/100f);
            RadialGradientShader.SetParameter("windowHeight", DarknessTexture.Size.Y); // this must be set, but only needs to be set once (or whenever the size of the window changes)
            DarknessTexture.Draw(circle, new RenderStates(BlendMode.Add, Transform.Identity, null, RadialGradientShader));
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
            myList.Add(new VideoMode(800, 600));
            myList.Add(new VideoMode(1024, 768));
            myList.Add(new VideoMode(1280, 720));
            myList.Add(new VideoMode(1280, 768));
            myList.Add(new VideoMode(1280, 1024));
            myList.Add(new VideoMode(1360, 768));
            myList.Add(new VideoMode(1366, 768));
            myList.Add(new VideoMode(1400, 1050));
            myList.Add(new VideoMode(1440, 900));
            myList.Add(new VideoMode(1600, 900));
            myList.Add(new VideoMode(1680, 1050));
            myList.Add(new VideoMode(1920, 1080));
            return myList;
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
            if (tex == null) return;
            var u1 = (float)srcRectangle.X / tex.Size.X;
            var v1 = (float)srcRectangle.Y / tex.Size.Y;
            var u2 = (float)srcRectangle.Right / tex.Size.X;
            var v2 = (float)srcRectangle.Bottom / tex.Size.Y;


            u1 *= tex.Size.X;
            v1 *= tex.Size.Y;
            u2 *= tex.Size.X;
            v2 *= tex.Size.Y;

            _renderState.BlendMode = blendMode;

            if (renderTarget == RenderWindow)
            {
                if (
                !targetRect.IntersectsWith(new RectangleF(CurrentView.Left, CurrentView.Top, CurrentView.Width,
                    CurrentView.Height)))
                {
                    return;
                }
                if (_renderState.Texture == null || _renderState.Texture != tex || _vertexCount >= 1024 - 4)
                {
                    RenderCurrentBatch();
                    _renderState.Texture = tex;
                }


                var right = targetRect.X + targetRect.Width;
                var bottom = targetRect.Y + targetRect.Height;

                _vertexCache[_vertexCount++] = new Vertex(new Vector2f(targetRect.X, targetRect.Y), new Vector2f(u1, v1));
                _vertexCache[_vertexCount++] = new Vertex(new Vector2f(right, targetRect.Y), new Vector2f(u2, v1));
                _vertexCache[_vertexCount++] = new Vertex(new Vector2f(right, bottom), new Vector2f(u2, v2));
                _vertexCache[_vertexCount++] = new Vertex(new Vector2f(targetRect.X, bottom), new Vector2f(u1, v2));
            }
            else
            {
                if (_renderState.Texture == null || _renderState.Texture != tex )
                {
                    // enable the new texture

                    _renderState.Texture = tex;
                }
                var right = targetRect.X + targetRect.Width;
                var bottom = targetRect.Y + targetRect.Height;
                var vertexCache = new Vertex[4];
                vertexCache[0] = new Vertex(new Vector2f(targetRect.X, targetRect.Y), new Vector2f(u1, v1));
                vertexCache[1] = new Vertex(new Vector2f(right, targetRect.Y), new Vector2f(u2, v1));
                vertexCache[2] = new Vertex(new Vector2f(right, bottom), new Vector2f(u2, v2));
                vertexCache[3] = new Vertex(new Vector2f(targetRect.X, bottom), new Vector2f(u1, v2));
                DrawCalls++;
                renderTarget.Draw(vertexCache, 0, 4, PrimitiveType.Quads, _renderState);
                renderTarget.ResetGLStates();
            }
        }
        private static void RenderCurrentBatch()
        {
            if (_vertexCount > 0)
            {
                RenderWindow.Draw(_vertexCache, 0, (uint)_vertexCount, PrimitiveType.Quads, _renderState);
                DrawCalls++;
                RenderWindow.ResetGLStates();
                _vertexCount = 0;
            }
        }
    }
}
