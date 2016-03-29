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
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using Intersect_Client.Classes.Entities;
using Intersect_Client.Classes.Game_Objects;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Maps;
using Intersect_Client.Classes.UI;
using Event = Intersect_Client.Classes.Entities.Event;
using Object = System.Object;

namespace Intersect_Client.Classes.Core
{
    public static class GameGraphics
    {
        //Game Renderer
        public static GameRenderer Renderer;
        public static GameShader DefaultShader;

        //Screen Values
        public static bool MustReInit;
        public static GameFont GameFont;
        public static FloatRect CurrentView;

        //Game Textures
        public static GameRenderTexture WhiteTex;
        public static List<GameTexture> Tilesets = new List<GameTexture>();
        public static List<string> ItemFileNames = new List<string>();
        public static GameTexture[] ItemTextures;
        public static List<string> EntityFileNames = new List<string>();
        public static GameTexture[] EntityTextures;
        public static List<string> SpellFileNames = new List<string>();
        public static GameTexture[] SpellTextures;
        public static List<string> AnimationFileNames = new List<string>();
        public static GameTexture[] AnimationTextures;
        public static List<string> FaceFileNames = new List<string>();
        public static GameTexture[] FaceTextures;
        public static List<string> ImageFileNames = new List<string>();
        public static GameTexture[] ImageTextures;
        public static List<string> FogFileNames = new List<string>();
        public static GameTexture[] FogTextures;
        public static List<string> ResourceFileNames = new List<string>();
        public static GameTexture[] ResourceTextures;
        public static List<string> PaperdollFileNames = new List<string>();
        public static GameTexture[] PaperdollTextures;
        public static GameTexture TargetTexture;


        //Darkness Stuff
        public static int DarkOffsetX = 0;
        public static int DarkOffsetY = 0;
        public static float SunIntensity;
        public static GameRenderTexture DarknessTexture;
        private static GameShader RadialGradientShader;

        //Fog Stuff
        public static int FogOffsetX = 0;
        public static int FogOffsetY = 0;

        //Overlay Stuff
        public static Color OverlayColor = Color.Transparent;
        public static GameRenderTexture OverlayTexture;
        private static long _overlayUpdate = 0;

        //Player Spotlight Values
        private static byte PlayerLightIntensity = 255;
        private static int PlayerLightSize = 0;
        private static float PlayerLightExpand = 0f;
        public static Color PlayerLightColor = Color.White;
        private static List<Light> lightQueue = new List<Light>(); 

        private static long _fadeTimer;

        //Rendering Variables
        public static int DrawCalls = 0;
        public static int EntitiesDrawn = 0;
        public static int LightsDrawn = 0;
        public static int MapsDrawn = 0;

        //Cache the Y based rendering
        public static List<Entity>[] Layer1Entities;
        public static List<Entity>[] Layer2Entities;

        public static bool PreRenderedMapLayer = false;
        public static object GFXLock = new Object();
        public static List<GameRenderTexture> MapReleaseQueue = new List<GameRenderTexture>(); 
        public static List<GameRenderTexture> FreeMapTextures = new List<GameRenderTexture>();
        
        //Animations
        public static List<AnimationInstance> LiveAnimations = new List<AnimationInstance>();  
        

        //Init Functions
        public static void InitGraphics()
        {
            Renderer.Init();
            CreateWhiteTexture();
            Globals.ContentManager.LoadAll();
            GameFont = Renderer.LoadFont("Resources/Fonts/Arvo-Regular.ttf");
            RadialGradientShader = Renderer.LoadShader("Resources/Shaders/RadialGradient");
        }
        public static void InitInGame()
        {
            Layer1Entities = new List<Entity>[Options.MapHeight*3];
            Layer2Entities = new List<Entity>[Options.MapHeight*3];
            for (var i = 0; i < Options.MapHeight*3; i++)
            {
                Layer1Entities[i] = new List<Entity>();
                Layer2Entities[i] = new List<Entity>();
            }
            if (Globals.Database.RenderCaching) CreateMapTextures(9 * 18);
        }
        public static void CreateWhiteTexture()
        {
            WhiteTex = Renderer.CreateRenderTexture(1, 1);
            WhiteTex.Begin();
            WhiteTex.Clear(Color.White);
            WhiteTex.End();
        }

        public static void DrawIntro()
        {
            if (ImageFileNames.IndexOf(Globals.Database.IntroBG[Globals.IntroIndex]) > -1)
            {
                DrawFullScreenTextureStretched(
                    ImageTextures[ImageFileNames.IndexOf(Globals.Database.IntroBG[Globals.IntroIndex])]);
            }
        }

        public static void DrawMenu()
        {
            if (ImageFileNames.IndexOf(Globals.Database.MenuBG) > -1)
            {
                DrawFullScreenTexture(ImageTextures[ImageFileNames.IndexOf(Globals.Database.MenuBG)]);
            }
        }

        public static void DrawInGame()
        {
            ClearDarknessTexture();

            TryPreRendering();

            if (Globals.CurrentMap > -1 && Globals.GameMaps.ContainsKey(Globals.CurrentMap))
            {
                if (Globals.GameMaps[Globals.CurrentMap] != null)
                {
                    if (ImageFileNames.IndexOf(Globals.GameMaps[Globals.CurrentMap].Panorama) > -1)
                    {
                        DrawFullScreenTexture(
                            ImageTextures[ImageFileNames.IndexOf(Globals.GameMaps[Globals.CurrentMap].Panorama)]);
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

                foreach (AnimationInstance animInstance in LiveAnimations)
                {
                    animInstance.Draw(false);
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

                foreach (AnimationInstance animInstance in LiveAnimations)
                {
                    animInstance.Draw(true);
                }

                //Draw the players targets
                ((Player)Globals.Entities[Globals.MyIndex]).DrawTargets();

                if (Globals.GameMaps[Globals.CurrentMap] != null)
                {
                    if (ImageFileNames.IndexOf(Globals.GameMaps[Globals.CurrentMap].OverlayGraphic) > -1)
                    {
                        DrawFullScreenTexture(
                            ImageTextures[ImageFileNames.IndexOf(Globals.GameMaps[Globals.CurrentMap].OverlayGraphic)]);
                    }
                }

                DrawOverlay();


                for (int x = 0; x < Layer1Entities.Length; x++)
                {
                    for (int y = 0; y < Layer1Entities[x].Count; y++)
                    {
                        Layer1Entities[x][y].DrawName();
                        if (Layer1Entities[x][y].GetType() != typeof (Event))
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
                        if (Layer2Entities[x][y].GetType() != typeof (Event))
                        {
                            Layer2Entities[x][y].DrawHpBar();
                            Layer2Entities[x][y].DrawCastingBar();
                        }
                    }
                }
                OverlayDarkness();
            }
        }

        //Game Rendering
        public static void Render()
        {
            if (Renderer.Begin())
            {
                Renderer.Clear(Color.Black);
                DrawCalls = 0;
                MapsDrawn = 0;
                EntitiesDrawn = 0;
                LightsDrawn = 0;
                PreRenderedMapLayer = false;

                UpdateView();

                if (Globals.GameState == Enums.GameStates.Intro)
                {
                    DrawIntro();
                }
                else if (Globals.GameState == Enums.GameStates.Menu)
                {
                    DrawMenu();
                }
                else if (Globals.GameState == Enums.GameStates.Loading)
                {

                }
                if (Globals.GameState == Enums.GameStates.InGame)
                {
                    DrawInGame();
                }

                Gui.DrawGui();

                DrawGameTexture(WhiteTex,new FloatRect(0,0,1,1),CurrentView, new Color((int)GameFade.GetFade(), 0, 0, 0),null,GameBlendModes.Alpha);
                Renderer.End();
            }
        }

        private static void TryPreRendering()
        {
            if (Globals.Database.RenderCaching)
            {
                for (var i = 0; i < 9; i++)
                {
                    if (Globals.LocalMaps[i] > -1)
                    {
                        if (Globals.GameMaps.ContainsKey(Globals.LocalMaps[i]) &&
                            !Globals.GameMaps[Globals.LocalMaps[i]].MapRendered)
                        {
                            if (!GameGraphics.PreRenderedMapLayer)
                            {
                                Globals.GameMaps[Globals.LocalMaps[i]].PreRenderMap();
                            }
                        }
                    }
                }
            }
        }
        private static void DrawMap(int index, int layer = 0)
        {
            if (Globals.LocalMaps[index] < 0) return;
            if (!Globals.GameMaps.ContainsKey(Globals.LocalMaps[index])) return;
            if (!new FloatRect(Globals.GameMaps[Globals.LocalMaps[index]].GetX(), Globals.GameMaps[Globals.LocalMaps[index]].GetY(), Options.TileWidth * Options.MapWidth, Options.TileHeight * Options.MapHeight).IntersectsWith(CurrentView)) return;
            Globals.GameMaps[Globals.LocalMaps[index]].Draw(layer);
            if (layer == 0) { MapsDrawn++; }
        }
        public static void DrawOverlay()
        {
            if (!Globals.GameMaps.ContainsKey(Globals.CurrentMap)) return;
            float ecTime = Globals.System.GetTimeMS() - _overlayUpdate;

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
            }

            DrawGameTexture(WhiteTex, new FloatRect(0, 0, 1, 1), CurrentView, OverlayColor,null);
            _overlayUpdate = Globals.System.GetTimeMS();
        }
        public static void DrawFullScreenTexture(GameTexture tex)
        {
            int bgx = (int)(Renderer.GetScreenWidth() / 2 - tex.GetWidth() / 2);
            int bgy = (int)(Renderer.GetScreenHeight()/ 2 - tex.GetHeight() / 2);
            int bgw = (int)tex.GetWidth();
            int bgh = (int)tex.GetHeight();
            int diff = 0;
            if (bgw < Renderer.GetScreenWidth())
            {
                diff = (int)(Renderer.GetScreenWidth() - bgw);
                bgx -= diff / 2;
                bgw += diff;
            }
            if (bgh < Renderer.GetScreenHeight())
            {
                diff = (int)(Renderer.GetScreenHeight() - bgh);
                bgy -= diff / 2;
                bgh += diff;
            }
            DrawGameTexture(tex, new FloatRect(0, 0, tex.GetWidth(), tex.GetHeight()),
                new FloatRect(bgx + Renderer.GetView().X, bgy + Renderer.GetView().Y, bgw, bgh),Color.White);
        }
        public static void DrawFullScreenTextureStretched(GameTexture tex)
        {
            DrawGameTexture(tex, new FloatRect(0, 0, (int)tex.GetWidth(), (int)tex.GetHeight()),
                new FloatRect(Renderer.GetView().X, Renderer.GetView().Y, Renderer.GetScreenWidth(), Renderer.GetScreenHeight()), Color.White);
        }

        private static void UpdateView()
        {
            if (Globals.GameState == Enums.GameStates.InGame && Globals.GameMaps.ContainsKey(Globals.CurrentMap))
            {
                Player en = (Player) Globals.Entities[Globals.MyIndex];
                float x = Globals.GameMaps[Globals.CurrentMap].GetX() - Options.MapWidth*Options.TileWidth;
                float y = Globals.GameMaps[Globals.CurrentMap].GetY() - Options.MapHeight*Options.TileHeight;
                float x1 = Globals.GameMaps[Globals.CurrentMap].GetX() + (Options.MapWidth*Options.TileWidth)*2;
                float y1 = Globals.GameMaps[Globals.CurrentMap].GetY() + (Options.MapHeight*Options.TileHeight)*2;
                if (Globals.GameMaps[Globals.CurrentMap].HoldUp == 1)
                {
                    y += Options.MapHeight*Options.TileHeight;
                }
                if (Globals.GameMaps[Globals.CurrentMap].HoldLeft == 1)
                {
                    x += Options.MapWidth*Options.TileWidth;
                }
                if (Globals.GameMaps[Globals.CurrentMap].HoldRight == 1)
                {
                    x1 -= Options.MapWidth*Options.TileWidth;
                }
                if (Globals.GameMaps[Globals.CurrentMap].HoldDown == 1)
                {
                    y1 -= Options.MapHeight*Options.TileHeight;
                }
                float w = x1 - x;
                float h = y1 - y;
                var RestrictView = new FloatRect(x, y, w, h);
                CurrentView = new FloatRect((int) Math.Ceiling(en.GetCenterPos().X - Renderer.GetScreenWidth()/2f),
                    (int) Math.Ceiling(en.GetCenterPos().Y - Renderer.GetScreenHeight()/2f), Renderer.GetScreenWidth(),
                    Renderer.GetScreenHeight());
                if (RestrictView.Width >= CurrentView.Width)
                {
                    if (CurrentView.Left < RestrictView.Left)
                    {
                        CurrentView.X = RestrictView.Left;
                    }
                    if (CurrentView.Left + CurrentView.Width > RestrictView.Left + RestrictView.Width)
                    {
                        CurrentView.X -= (CurrentView.Left + CurrentView.Width) -
                                         (RestrictView.Left + RestrictView.Width);
                    }
                }
                if (RestrictView.Height >= CurrentView.Height)
                {
                    if (CurrentView.Top < RestrictView.Top)
                    {
                        CurrentView.Y = RestrictView.Top;
                    }
                    if (CurrentView.Top + CurrentView.Height > RestrictView.Top + RestrictView.Height)
                    {
                        CurrentView.Y -= (CurrentView.Top + CurrentView.Height) -
                                         (RestrictView.Top + RestrictView.Height);
                    }
                }
            }
            else
            {
                CurrentView = new FloatRect(0, 0, Renderer.GetScreenWidth(), Renderer.GetScreenHeight());
            }
            Renderer.SetView(CurrentView);
        }

        private static void CreateMapTextures(int count)
        {
            for (int i = 0; i < count; i++)
            {
                ReleaseMapTexture(Renderer.CreateRenderTexture(Options.TileWidth*Options.MapWidth,
                    Options.TileHeight*Options.MapHeight));
            }
        }
        public static bool GetMapTexture(ref GameRenderTexture replaceme)
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
        public static void ReleaseMapTexture(GameRenderTexture releaseTex)
        {
            if (releaseTex.SetActive(false))
            {
                FreeMapTextures.Add(releaseTex);
            }
            else
            {
                //Debug.Print("Error!");
            }
        }

        //Lighting
        private static void ClearDarknessTexture()
        {
            if (DarknessTexture == null)
            {
                DarknessTexture = Renderer.CreateRenderTexture(Renderer.GetScreenWidth(), Renderer.GetScreenHeight());
            }
            DarknessTexture.Clear(Color.Black);
         }
        private static void OverlayDarkness()
        {
            if (!Globals.GameMaps.ContainsKey(Globals.CurrentMap)) return;
            if (DarknessTexture == null) { return; }

            //Draw Light Around Player
            if (Globals.GameMaps.ContainsKey(Globals.CurrentMap))
            {
                PlayerLightSize = Globals.GameMaps[Globals.CurrentMap].PlayerLightSize;
                PlayerLightIntensity = Globals.GameMaps[Globals.CurrentMap].PlayerLightIntensity;
                PlayerLightColor = Globals.GameMaps[Globals.CurrentMap].PlayerLightColor;
                PlayerLightExpand = Globals.GameMaps[Globals.CurrentMap].PlayerLightExpand ;
                DrawLight((int)Math.Ceiling(Globals.Entities[Globals.MyIndex].GetCenterPos().X), (int)
                    Math.Ceiling(Globals.Entities[Globals.MyIndex].GetCenterPos().Y), (int)PlayerLightSize, PlayerLightIntensity, PlayerLightExpand, PlayerLightColor);
            }

            DrawLights();
            DrawGameTexture(WhiteTex, new FloatRect(0, 0, 1, 1),
                new FloatRect(0, 0, DarknessTexture.GetWidth(), DarknessTexture.GetHeight()),
                new Color((byte)(SunIntensity),255, 255, 255),DarknessTexture);
            DarknessTexture.End();
            DrawGameTexture(DarknessTexture, CurrentView.Left, CurrentView.Top, null,GameBlendModes.Multiply);
        }
        public static void DrawLight(int x, int y, int size, byte intensity, float expand, Color color)
        {
            lightQueue.Add(new Light(0, 0, x, y, intensity, size, expand, color));
            LightsDrawn++;
        }

        private static void DrawLights()
        {
            foreach (Light l in lightQueue)
            {
                int x = l.OffsetX - ((int)CurrentView.Left + l.Size);
                int y = l.OffsetY - ((int)CurrentView.Top + l.Size);
                RadialGradientShader.SetColor("_Color", new Color(l.Intensity, l.Color.R, l.Color.G, l.Color.B));
                RadialGradientShader.SetVector2("_Center", new Pointf(x + l.Size, y + l.Size));
                RadialGradientShader.SetFloat("_Radius", l.Size);
                RadialGradientShader.SetFloat("_Expand", l.Expand / 100f);
                RadialGradientShader.SetFloat("_WindowHeight", DarknessTexture.GetHeight());

                DrawGameTexture(WhiteTex, new FloatRect(0, 0, 1, 1), new FloatRect(x, y, l.Size * 2, l.Size * 2), Color.Transparent,
                    DarknessTexture, GameBlendModes.Add, RadialGradientShader);
            }
            lightQueue.Clear();
        }

        //Helper Functions


        //Rendering Functions


        /// <summary>
        /// Renders a specified texture onto a RenderTexture or the GameScreen (if renderTarget is passed as null) at the coordinates given using a specified blending mode.
        /// </summary>
        /// <param name="tex">The texture to draw</param>
        /// <param name="x">X coordinate on the render target to draw to</param>
        /// <param name="y">Y coordinate on the render target to draw to</param>
        /// <param name="renderTarget">Where to draw to. If null it this will draw to the game screen.</param>
        /// <param name="blendMode">Which blend mode to use when rendering</param>
        public static void DrawGameTexture(GameTexture tex, float x, float y, GameRenderTexture renderTarget = null, GameBlendModes blendMode = GameBlendModes.Alpha, GameShader shader = null, float rotationDegrees = 0.0f)
        {
            var destRectangle = new FloatRect(x, y, (int)tex.GetWidth(), (int)tex.GetHeight());
            var srcRectangle = new FloatRect(0, 0, (int)tex.GetWidth(), (int)tex.GetHeight());
            DrawGameTexture(tex, srcRectangle, destRectangle, Color.White, renderTarget, blendMode, shader, rotationDegrees);
        }

        /// <summary>
        /// Renders a specified texture onto a RenderTexture or the GameScreen (if renderTarget is passed as null) at the coordinates given using a specified blending mode.
        /// </summary>
        /// <param name="tex">The texture to draw</param>
        /// <param name="x">X coordinate on the render target to draw to</param>
        /// <param name="y">Y coordinate on the render target to draw to</param>
        /// <param name="renderColor">Color mask to draw with. Default is Color.White</param>
        /// <param name="renderTarget">Where to draw to. If null it this will draw to the game screen.</param>
        /// <param name="blendMode">Which blend mode to use when rendering</param>
        public static void DrawGameTexture(GameTexture tex, float x, float y,   Color renderColor, GameRenderTexture renderTarget = null, GameBlendModes blendMode = GameBlendModes.Alpha, GameShader shader = null, float rotationDegrees = 0.0f)
        {
            var destRectangle = new FloatRect(x, y, (int)tex.GetWidth(), (int)tex.GetHeight());
            var srcRectangle = new FloatRect(0, 0, (int)tex.GetWidth(), (int)tex.GetHeight());
            DrawGameTexture(tex, srcRectangle, destRectangle, renderColor, renderTarget, blendMode, shader,rotationDegrees);
        }

        /// <summary>
        /// Renders a specified texture onto a RenderTexture or the GameScreen (if renderTarget is passed as null) at the coordinates given using a specified blending mode.
        /// </summary>
        /// <param name="tex">The texture to draw</param>
        /// <param name="dx">X coordinate on the renderTarget to draw to.</param>
        /// <param name="dy">Y coordinate on the renderTarget to draw to.</param>
        /// <param name="sx">X coordinate on the source texture to grab from.</param>
        /// <param name="sy">Y coordinate on the source texture to grab from.</param>
        /// <param name="w">Width of the texture part we are rendering.</param>
        /// <param name="h">Height of the texture part we are rendering.</param>
        /// <param name="renderTarget">>Where to draw to. If null it this will draw to the game screen.</param>
        /// <param name="blendMode">Which blend mode to use when rendering</param>
        public static void DrawGameTexture(GameTexture tex, float dx, float dy, float sx, float sy, float w, float h, GameRenderTexture renderTarget = null, GameBlendModes blendMode = GameBlendModes.Alpha, GameShader shader = null, float rotationDegrees = 0.0f)
        {
            var destRectangle = new FloatRect(dx, dy, w, h);
            var srcRectangle = new FloatRect(sx, sy, w, h);
            DrawGameTexture(tex, srcRectangle, destRectangle, Color.White, renderTarget, blendMode, shader,rotationDegrees);
        }


        public static void DrawGameTexture(GameTexture tex, FloatRect srcRectangle, FloatRect targetRect,Color renderColor, GameRenderTexture renderTarget = null, GameBlendModes blendMode = GameBlendModes.Alpha, GameShader shader = null, float rotationDegrees = 0.0f)
        {
            if (tex == null) return;
            Renderer.DrawTexture(tex, srcRectangle, targetRect, renderColor, renderTarget, blendMode, shader,rotationDegrees);
        }
    }
}
