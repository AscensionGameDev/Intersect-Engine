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
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using Intersect_Client.Classes.Entities;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.UI;
using Event = Intersect_Client.Classes.Entities.Event;
using Object = System.Object;
using Intersect_Library.GameObjects;

namespace Intersect_Client.Classes.Core
{
    public static class GameGraphics
    {
        //Game Renderer
        public static GameRenderer Renderer;
        public static GameShader DefaultShader;
        private static GameContentManager contentManager;

        //Resolution
        private static int _oldWidth = 0;
        private static int _oldHeight = 0;

        //Screen Values
        public static GameFont GameFont;
        public static FloatRect CurrentView;

        //Darkness Stuff
        public static float _brightnessLevel;
        private static GameRenderTexture _darknessTexture;

        //Overlay Stuff
        public static Color OverlayColor = Color.Transparent;
        public static GameRenderTexture OverlayTexture;
        private static long _overlayUpdate = 0;

        //Player Spotlight Values
        private static long _lightUpdate = 0;
        private static byte _playerLightIntensity = 255;
        private static int _playerLightSize = 0;
        private static float _playerLightExpand = 0f;
        public static Color _playerLightColor = Color.White;
        private static List<Light> _lightQueue = new List<Light>();

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
            contentManager = Globals.ContentManager;
            contentManager.LoadAll();
            GameFont = contentManager.GetFont("calibri.xnb");
        }
        public static void InitInGame()
        {
            Layer1Entities = new List<Entity>[Options.MapHeight * 3];
            Layer2Entities = new List<Entity>[Options.MapHeight * 3];
            for (var i = 0; i < Options.MapHeight * 3; i++)
            {
                Layer1Entities[i] = new List<Entity>();
                Layer2Entities[i] = new List<Entity>();
            }
            if (Globals.Database.RenderCaching) CreateMapTextures(9 * 18);
        }

        public static void DrawIntro()
        {
            GameTexture imageTex = contentManager.GetTexture(GameContentManager.TextureType.Image,
                Globals.Database.IntroBG[Globals.IntroIndex]);
            if (imageTex != null)
            {
                DrawFullScreenTextureStretched(imageTex);
            }
        }

        public static void DrawMenu()
        {
            GameTexture imageTex = contentManager.GetTexture(GameContentManager.TextureType.Image,
                Globals.Database.MenuBG);
            if (imageTex != null)
            {
                DrawFullScreenTextureStretched(imageTex);
            }
        }

        public static void DrawInGame()
        {
            ClearDarknessTexture();

            TryPreRendering();

            GenerateLightMap();

            if (Globals.CurrentMap > -1 && Globals.GameMaps.ContainsKey(Globals.CurrentMap) && Globals.NeedsMaps == false)
            {
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

                DrawOverlay();


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
                DrawDarkness();
            }
        }

        //Game Rendering
        public static void Render()
        {
            if (Renderer.Begin())
            {
                if (Renderer.GetScreenWidth() != _oldWidth || Renderer.GetScreenHeight() != _oldHeight)
                {
                    Gui.DestroyGwen();
                    Gui.InitGwen();
                    _oldWidth = Renderer.GetScreenWidth();
                    _oldHeight = Renderer.GetScreenHeight();
                }
                Renderer.Clear(Color.Black);
                DrawCalls = 0;
                MapsDrawn = 0;
                EntitiesDrawn = 0;
                LightsDrawn = 0;
                PreRenderedMapLayer = false;

                UpdateView();

                if (Globals.GameState == GameStates.Intro)
                {
                    DrawIntro();
                }
                else if (Globals.GameState == GameStates.Menu)
                {
                    DrawMenu();
                }
                else if (Globals.GameState == GameStates.Loading)
                {

                }
                if (Globals.GameState == GameStates.InGame)
                {
                    DrawInGame();
                }

                Gui.DrawGui();

                DrawGameTexture(Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1), CurrentView, new Color((int)GameFade.GetFade(), 0, 0, 0), null, GameBlendModes.Alpha);
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
            if (Globals.GameMaps.ContainsKey(Globals.CurrentMap))
            {
                float ecTime = Globals.System.GetTimeMS() - _overlayUpdate;

                if (OverlayColor.A != Globals.GameMaps[Globals.CurrentMap].AHue ||
                    OverlayColor.R != Globals.GameMaps[Globals.CurrentMap].RHue ||
                    OverlayColor.G != Globals.GameMaps[Globals.CurrentMap].GHue ||
                    OverlayColor.B != Globals.GameMaps[Globals.CurrentMap].BHue)
                {
                    if (OverlayColor.A < Globals.GameMaps[Globals.CurrentMap].AHue)
                    {
                        if ((int)OverlayColor.A + (int)(255 * ecTime / 2000f) > Globals.GameMaps[Globals.CurrentMap].AHue)
                        {
                            OverlayColor.A = (byte)Globals.GameMaps[Globals.CurrentMap].AHue;
                        }
                        else
                        {
                            OverlayColor.A += (byte)(255 * ecTime / 2000f);
                        }
                    }

                    if (OverlayColor.A > Globals.GameMaps[Globals.CurrentMap].AHue)
                    {
                        if ((int)OverlayColor.A - (int)(255 * ecTime / 2000f) < Globals.GameMaps[Globals.CurrentMap].AHue)
                        {
                            OverlayColor.A = (byte)Globals.GameMaps[Globals.CurrentMap].AHue;
                        }
                        else
                        {
                            OverlayColor.A -= (byte)(255 * ecTime / 2000f);
                        }
                    }

                    if (OverlayColor.R < Globals.GameMaps[Globals.CurrentMap].RHue)
                    {
                        if ((int)OverlayColor.R + (int)(255 * ecTime / 2000f) > Globals.GameMaps[Globals.CurrentMap].RHue)
                        {
                            OverlayColor.R = (byte)Globals.GameMaps[Globals.CurrentMap].RHue;
                        }
                        else
                        {
                            OverlayColor.R += (byte)(255 * ecTime / 2000f);
                        }
                    }

                    if (OverlayColor.R > Globals.GameMaps[Globals.CurrentMap].RHue)
                    {
                        if ((int)OverlayColor.R - (int)(255 * ecTime / 2000f) < Globals.GameMaps[Globals.CurrentMap].RHue)
                        {
                            OverlayColor.R = (byte)Globals.GameMaps[Globals.CurrentMap].RHue;
                        }
                        else
                        {
                            OverlayColor.R -= (byte)(255 * ecTime / 2000f);
                        }
                    }

                    if (OverlayColor.G < Globals.GameMaps[Globals.CurrentMap].GHue)
                    {
                        if ((int)OverlayColor.G + (int)(255 * ecTime / 2000f) > Globals.GameMaps[Globals.CurrentMap].GHue)
                        {
                            OverlayColor.G = (byte)Globals.GameMaps[Globals.CurrentMap].GHue;
                        }
                        else
                        {
                            OverlayColor.G += (byte)(255 * ecTime / 2000f);
                        }
                    }

                    if (OverlayColor.G > Globals.GameMaps[Globals.CurrentMap].GHue)
                    {
                        if ((int)OverlayColor.G - (int)(255 * ecTime / 2000f) < Globals.GameMaps[Globals.CurrentMap].GHue)
                        {
                            OverlayColor.G = (byte)Globals.GameMaps[Globals.CurrentMap].GHue;
                        }
                        else
                        {
                            OverlayColor.G -= (byte)(255 * ecTime / 2000f);
                        }
                    }

                    if (OverlayColor.B < Globals.GameMaps[Globals.CurrentMap].BHue)
                    {
                        if ((int)OverlayColor.B + (int)(255 * ecTime / 2000f) > Globals.GameMaps[Globals.CurrentMap].BHue)
                        {
                            OverlayColor.B = (byte)Globals.GameMaps[Globals.CurrentMap].BHue;
                        }
                        else
                        {
                            OverlayColor.B += (byte)(255 * ecTime / 2000f);
                        }
                    }

                    if (OverlayColor.B > Globals.GameMaps[Globals.CurrentMap].BHue)
                    {
                        if ((int)OverlayColor.B - (int)(255 * ecTime / 2000f) < Globals.GameMaps[Globals.CurrentMap].BHue)
                        {
                            OverlayColor.B = (byte)Globals.GameMaps[Globals.CurrentMap].BHue;
                        }
                        else
                        {
                            OverlayColor.B -= (byte)(255 * ecTime / 2000f);
                        }
                    }
                }
            }

            DrawGameTexture(Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1), CurrentView, OverlayColor, null);
            _overlayUpdate = Globals.System.GetTimeMS();
        }
        public static void DrawFullScreenTexture(GameTexture tex, float alpha = 1f)
        {
            int bgx = (int)(Renderer.GetScreenWidth() / 2 - tex.GetWidth() / 2);
            int bgy = (int)(Renderer.GetScreenHeight() / 2 - tex.GetHeight() / 2);
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
                new FloatRect(bgx + Renderer.GetView().X, bgy + Renderer.GetView().Y, bgw, bgh), new Color((int)(alpha * 255f), 255, 255, 255));
        }
        public static void DrawFullScreenTextureStretched(GameTexture tex)
        {
            DrawGameTexture(tex, new FloatRect(0, 0, (int)tex.GetWidth(), (int)tex.GetHeight()),
                new FloatRect(Renderer.GetView().X, Renderer.GetView().Y, Renderer.GetScreenWidth(), Renderer.GetScreenHeight()), Color.White);
        }

        private static void UpdateView()
        {
            if (Globals.GameState == GameStates.InGame && Globals.GameMaps.ContainsKey(Globals.CurrentMap))
            {
                Player en = (Player)Globals.Entities[Globals.MyIndex];
                float x = Globals.GameMaps[Globals.CurrentMap].GetX() - Options.MapWidth * Options.TileWidth;
                float y = Globals.GameMaps[Globals.CurrentMap].GetY() - Options.MapHeight * Options.TileHeight;
                float x1 = Globals.GameMaps[Globals.CurrentMap].GetX() + (Options.MapWidth * Options.TileWidth) * 2;
                float y1 = Globals.GameMaps[Globals.CurrentMap].GetY() + (Options.MapHeight * Options.TileHeight) * 2;
                if (Globals.GameMaps[Globals.CurrentMap].HoldUp == 1)
                {
                    y += Options.MapHeight * Options.TileHeight;
                }
                if (Globals.GameMaps[Globals.CurrentMap].HoldLeft == 1)
                {
                    x += Options.MapWidth * Options.TileWidth;
                }
                if (Globals.GameMaps[Globals.CurrentMap].HoldRight == 1)
                {
                    x1 -= Options.MapWidth * Options.TileWidth;
                }
                if (Globals.GameMaps[Globals.CurrentMap].HoldDown == 1)
                {
                    y1 -= Options.MapHeight * Options.TileHeight;
                }
                float w = x1 - x;
                float h = y1 - y;
                var RestrictView = new FloatRect(x, y, w, h);
                CurrentView = new FloatRect((int)Math.Ceiling(en.GetCenterPos().X - Renderer.GetScreenWidth() / 2f),
                    (int)Math.Ceiling(en.GetCenterPos().Y - Renderer.GetScreenHeight() / 2f), Renderer.GetScreenWidth(),
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
                ReleaseMapTexture(Renderer.CreateRenderTexture(Options.TileWidth * Options.MapWidth,
                    Options.TileHeight * Options.MapHeight));
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
            if (_darknessTexture == null)
            {
                _darknessTexture = Renderer.CreateRenderTexture(Renderer.GetScreenWidth(), Renderer.GetScreenHeight());
            }
            _darknessTexture.Clear(Color.Black);
        }
        private static void GenerateLightMap()
        {
            if (!Globals.GameMaps.ContainsKey(Globals.CurrentMap)) return;
            if (_darknessTexture == null) { return; }

            UpdatePlayerLight();
            AddLight((int)Math.Ceiling(Globals.Entities[Globals.MyIndex].GetCenterPos().X), (int)Math.Ceiling(Globals.Entities[Globals.MyIndex].GetCenterPos().Y), (int)_playerLightSize, _playerLightIntensity, _playerLightExpand, Intersect_Library.Color.FromArgb(_playerLightColor.A, _playerLightColor.R, _playerLightColor.G, _playerLightColor.B));

            DrawLights();
            DrawGameTexture(Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                new FloatRect(0, 0, _darknessTexture.GetWidth(), _darknessTexture.GetHeight()),
               new Color((byte)(_brightnessLevel), 255, 255, 255), _darknessTexture, GameBlendModes.Add);
            _darknessTexture.End();

        }
        public static void DrawDarkness()
        {
            GameShader radialShader = Globals.ContentManager.GetShader("radialgradient.xnb");
            if (radialShader != null)
            {
                DrawGameTexture(_darknessTexture, CurrentView.Left, CurrentView.Top, null, GameBlendModes.Multiply);
            }
        }
        public static void AddLight(int x, int y, int size, byte intensity, float expand, Intersect_Library.Color color)
        {
            _lightQueue.Add(new Light(0, 0, x, y, intensity, size, expand, color));
            LightsDrawn++;
        }

        private static void DrawLights()
        {
            GameShader radialShader = Globals.ContentManager.GetShader("radialgradient.xnb");
            if (radialShader != null)
            {
                foreach (Light l in _lightQueue)
                {
                    int x = l.OffsetX - ((int) CurrentView.Left + l.Size);
                    int y = l.OffsetY - ((int) CurrentView.Top + l.Size);

                    radialShader.SetColor("_Color", new Color(l.Color.R, l.Color.G, l.Color.B, l.Intensity));
                    radialShader.SetFloat("_Expand", l.Expand/100f);

                    DrawGameTexture(Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                        new FloatRect(x, y, l.Size*2, l.Size*2), Color.Transparent,
                        _darknessTexture, GameBlendModes.Add, radialShader);
                }
            }
            _lightQueue.Clear();
        }

        private static void UpdatePlayerLight()
        {
            //Draw Light Around Player
            if (Globals.GameMaps.ContainsKey(Globals.CurrentMap))
            {
                float ecTime = Globals.System.GetTimeMS() - _lightUpdate;

                byte brightnessTarget = (byte)(((float)Globals.GameMaps[Globals.CurrentMap].Brightness / 100f) * 255);
                if (_brightnessLevel < brightnessTarget)
                {
                    if ((int)_brightnessLevel + (int)(255 * ecTime / 2000f) > brightnessTarget)
                    {
                        _brightnessLevel = brightnessTarget;
                    }
                    else
                    {
                        _brightnessLevel += (byte)(255 * ecTime / 2000f);
                    }
                }

                if (_brightnessLevel > brightnessTarget)
                {
                    if ((int)_brightnessLevel - (int)(255 * ecTime / 2000f) < brightnessTarget)
                    {
                        _brightnessLevel = brightnessTarget;
                    }
                    else
                    {
                        _brightnessLevel -= (byte)(255 * ecTime / 2000f);
                    }
                }

                if (_playerLightColor.R != Globals.GameMaps[Globals.CurrentMap].PlayerLightColor.R ||
                    _playerLightColor.G != Globals.GameMaps[Globals.CurrentMap].PlayerLightColor.G ||
                    _playerLightColor.B != Globals.GameMaps[Globals.CurrentMap].PlayerLightColor.B)
                {
                    if (_playerLightColor.R < Globals.GameMaps[Globals.CurrentMap].PlayerLightColor.R)
                    {
                        if ((int)_playerLightColor.R + (int)(255 * ecTime / 2000f) > Globals.GameMaps[Globals.CurrentMap].PlayerLightColor.R)
                        {
                            _playerLightColor.R = (byte)Globals.GameMaps[Globals.CurrentMap].PlayerLightColor.R;
                        }
                        else
                        {
                            _playerLightColor.R += (byte)(255 * ecTime / 2000f);
                        }
                    }

                    if (_playerLightColor.R > Globals.GameMaps[Globals.CurrentMap].PlayerLightColor.R)
                    {
                        if ((int)_playerLightColor.R - (int)(255 * ecTime / 2000f) < Globals.GameMaps[Globals.CurrentMap].PlayerLightColor.R)
                        {
                            _playerLightColor.R = (byte)Globals.GameMaps[Globals.CurrentMap].PlayerLightColor.R;
                        }
                        else
                        {
                            _playerLightColor.R -= (byte)(255 * ecTime / 2000f);
                        }
                    }

                    if (_playerLightColor.G < Globals.GameMaps[Globals.CurrentMap].PlayerLightColor.G)
                    {
                        if ((int)_playerLightColor.G + (int)(255 * ecTime / 2000f) > Globals.GameMaps[Globals.CurrentMap].PlayerLightColor.G)
                        {
                            _playerLightColor.G = (byte)Globals.GameMaps[Globals.CurrentMap].PlayerLightColor.G;
                        }
                        else
                        {
                            _playerLightColor.G += (byte)(255 * ecTime / 2000f);
                        }
                    }

                    if (_playerLightColor.G > Globals.GameMaps[Globals.CurrentMap].PlayerLightColor.G)
                    {
                        if ((int)_playerLightColor.G - (int)(255 * ecTime / 2000f) < Globals.GameMaps[Globals.CurrentMap].PlayerLightColor.G)
                        {
                            _playerLightColor.G = (byte)Globals.GameMaps[Globals.CurrentMap].PlayerLightColor.G;
                        }
                        else
                        {
                            _playerLightColor.G -= (byte)(255 * ecTime / 2000f);
                        }
                    }

                    if (_playerLightColor.B < Globals.GameMaps[Globals.CurrentMap].PlayerLightColor.B)
                    {
                        if ((int)_playerLightColor.B + (int)(255 * ecTime / 2000f) > Globals.GameMaps[Globals.CurrentMap].PlayerLightColor.B)
                        {
                            _playerLightColor.B = (byte)Globals.GameMaps[Globals.CurrentMap].PlayerLightColor.B;
                        }
                        else
                        {
                            _playerLightColor.B += (byte)(255 * ecTime / 2000f);
                        }
                    }

                    if (_playerLightColor.B > Globals.GameMaps[Globals.CurrentMap].PlayerLightColor.B)
                    {
                        if ((int)_playerLightColor.B - (int)(255 * ecTime / 2000f) < Globals.GameMaps[Globals.CurrentMap].PlayerLightColor.B)
                        {
                            _playerLightColor.B = (byte)Globals.GameMaps[Globals.CurrentMap].PlayerLightColor.B;
                        }
                        else
                        {
                            _playerLightColor.B -= (byte)(255 * ecTime / 2000f);
                        }
                    }
                }

                if (_playerLightSize != Globals.GameMaps[Globals.CurrentMap].PlayerLightSize)
                {
                    if (_playerLightSize < Globals.GameMaps[Globals.CurrentMap].PlayerLightSize)
                    {
                        if (_playerLightSize + (int)(500 * ecTime / 2000f) > Globals.GameMaps[Globals.CurrentMap].PlayerLightSize)
                        {
                            _playerLightSize = Globals.GameMaps[Globals.CurrentMap].PlayerLightSize;
                        }
                        else
                        {
                            _playerLightSize += (int)(500 * ecTime / 2000f);
                        }
                    }

                    if (_playerLightSize > Globals.GameMaps[Globals.CurrentMap].PlayerLightSize)
                    {
                        if (_playerLightSize - (int)(500 * ecTime / 2000f) < Globals.GameMaps[Globals.CurrentMap].PlayerLightSize)
                        {
                            _playerLightSize = Globals.GameMaps[Globals.CurrentMap].PlayerLightSize;
                        }
                        else
                        {
                            _playerLightSize -= (int)(500 * ecTime / 2000f);
                        }
                    }
                }

                if (_playerLightIntensity < Globals.GameMaps[Globals.CurrentMap].PlayerLightIntensity)
                {
                    if ((int)_playerLightIntensity + (int)(255 * ecTime / 2000f) > Globals.GameMaps[Globals.CurrentMap].PlayerLightIntensity)
                    {
                        _playerLightIntensity = (byte)Globals.GameMaps[Globals.CurrentMap].PlayerLightIntensity;
                    }
                    else
                    {
                        _playerLightIntensity += (byte)(255 * ecTime / 2000f);
                    }
                }

                if (_playerLightIntensity > Globals.GameMaps[Globals.CurrentMap].AHue)
                {
                    if ((int)_playerLightIntensity - (int)(255 * ecTime / 2000f) < Globals.GameMaps[Globals.CurrentMap].PlayerLightIntensity)
                    {
                        _playerLightIntensity = (byte)Globals.GameMaps[Globals.CurrentMap].PlayerLightIntensity;
                    }
                    else
                    {
                        _playerLightIntensity -= (byte)(255 * ecTime / 2000f);
                    }
                }

                if (_playerLightExpand < Globals.GameMaps[Globals.CurrentMap].PlayerLightExpand)
                {
                    if ((int)_playerLightExpand + (float)(100f * ecTime / 2000f) > Globals.GameMaps[Globals.CurrentMap].PlayerLightExpand)
                    {
                        _playerLightExpand = Globals.GameMaps[Globals.CurrentMap].PlayerLightExpand;
                    }
                    else
                    {
                        _playerLightExpand += (100f * ecTime / 2000f);
                    }
                }

                if (_playerLightExpand > Globals.GameMaps[Globals.CurrentMap].PlayerLightExpand)
                {
                    if ((int)_playerLightExpand - (float)(100f * ecTime / 2000f) < Globals.GameMaps[Globals.CurrentMap].PlayerLightExpand)
                    {
                        _playerLightExpand = Globals.GameMaps[Globals.CurrentMap].PlayerLightExpand;
                    }
                    else
                    {
                        _playerLightExpand -= (100f * ecTime / 2000f);
                    }
                }
                _lightUpdate = Globals.System.GetTimeMS();
            }
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
        public static void DrawGameTexture(GameTexture tex, float x, float y, Color renderColor, GameRenderTexture renderTarget = null, GameBlendModes blendMode = GameBlendModes.Alpha, GameShader shader = null, float rotationDegrees = 0.0f)
        {
            var destRectangle = new FloatRect(x, y, (int)tex.GetWidth(), (int)tex.GetHeight());
            var srcRectangle = new FloatRect(0, 0, (int)tex.GetWidth(), (int)tex.GetHeight());
            DrawGameTexture(tex, srcRectangle, destRectangle, renderColor, renderTarget, blendMode, shader, rotationDegrees);
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
            DrawGameTexture(tex, srcRectangle, destRectangle, Color.White, renderTarget, blendMode, shader, rotationDegrees);
        }


        public static void DrawGameTexture(GameTexture tex, FloatRect srcRectangle, FloatRect targetRect, Color renderColor, GameRenderTexture renderTarget = null, GameBlendModes blendMode = GameBlendModes.Alpha, GameShader shader = null, float rotationDegrees = 0.0f)
        {
            if (tex == null) return;
            Renderer.DrawTexture(tex, srcRectangle, targetRect, renderColor, renderTarget, blendMode, shader, rotationDegrees);
        }
    }
}
