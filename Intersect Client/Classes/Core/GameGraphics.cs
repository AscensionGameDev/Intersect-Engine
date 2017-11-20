using System;
using System.Collections.Generic;
using Intersect;
using Intersect.GameObjects;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using Intersect_Client.Classes.Entities;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Maps;
using Intersect_Client.Classes.UI;
using Color = Intersect.Color;

namespace Intersect_Client.Classes.Core
{
    public static class GameGraphics
    {
        //Game Renderer
        public static GameRenderer Renderer;

        public static GameShader DefaultShader;
        private static GameContentManager contentManager;

        //Resolution
        private static int _oldWidth;

        private static int _oldHeight;

        //Screen Values
        public static GameFont GameFont;

        public static FloatRect CurrentView;

        //Darkness Stuff
        public static float _brightnessLevel;

        private static GameRenderTexture _darknessTexture;

        //Overlay Stuff
        public static Color OverlayColor = Color.Transparent;

        private static long _overlayUpdate;

        //Player Spotlight Values
        private static long _lightUpdate;

        private static float _playerLightIntensity = 255;
        private static float _playerLightSize;
        private static float _playerLightExpand;
        public static ColorF _playerLightColor = ColorF.White;
        private static List<LightBase> _lightQueue = new List<LightBase>();
        private static long _fadeTimer;

        //Grid Switched
        public static bool GridSwitched;

        //Rendering Variables
        public static int DrawCalls;

        public static int EntitiesDrawn;
        public static int LightsDrawn;
        public static int MapsDrawn;

        //Cache the Y based rendering
        public static HashSet<Entity>[,] RenderingEntities;

        public static bool PreRenderedMapLayer;
        public static object GFXLock = new object();
        public static List<GameRenderTexture> MapReleaseQueue = new List<GameRenderTexture>();
        public static List<GameRenderTexture> FreeMapTextures = new List<GameRenderTexture>();

        //Animations
        public static List<AnimationInstance> LiveAnimations = new List<AnimationInstance>();

        public static object AnimationLock = new object();

        //Init Functions
        public static void InitGraphics()
        {
            Renderer.Init();
            contentManager = Globals.ContentManager;
            contentManager.LoadAll();
            GameFont = contentManager.GetFont(Gui.ActiveFont, 8);
        }

        public static void InitInGame()
        {
            RenderingEntities = new HashSet<Entity>[6, Options.MapHeight * 3];
            for (int z = 0; z < 6; z++)
            {
                for (var i = 0; i < Options.MapHeight * 3; i++)
                {
                    RenderingEntities[z, i] = new HashSet<Entity>();
                }
            }
        }

        public static void DrawIntro()
        {
            GameTexture imageTex = contentManager.GetTexture(GameContentManager.TextureType.Image, Globals.Database.IntroBG[Globals.IntroIndex]);
            if (imageTex != null)
            {
                DrawFullScreenTextureFitMinimum(imageTex);
            }
        }

        public static void DrawMenu()
        {
            GameTexture imageTex = contentManager.GetTexture(GameContentManager.TextureType.Gui,
                Globals.Database.MenuBG);
            if (imageTex != null)
            {
                DrawFullScreenTexture(imageTex);
            }
        }

        public static void DrawInGame()
        {
            var currentMap = Globals.Me.MapInstance;
            if (currentMap == null) return;
            if (Globals.NeedsMaps)
            {
                TryPreRendering(false);
                return;
            }
            if (GridSwitched)
            {
                //Brightness
                byte brightnessTarget = (byte) ((currentMap.Brightness / 100f) * 255);
                _brightnessLevel = brightnessTarget;
                _playerLightColor.R = currentMap.PlayerLightColor.R;
                _playerLightColor.G = currentMap.PlayerLightColor.G;
                _playerLightColor.B = currentMap.PlayerLightColor.B;
                _playerLightSize = currentMap.PlayerLightSize;
                _playerLightIntensity = currentMap.PlayerLightIntensity;
                _playerLightExpand = currentMap.PlayerLightExpand;

                //Overlay
                OverlayColor.A = (byte) currentMap.AHue;
                OverlayColor.R = (byte) currentMap.RHue;
                OverlayColor.G = (byte) currentMap.GHue;
                OverlayColor.B = (byte) currentMap.BHue;

                //Fog && Panorama
                currentMap.GridSwitched();
                GridSwitched = false;
            }


            lock (AnimationLock)
            {
                var animations = LiveAnimations.ToArray();
                foreach (AnimationInstance animInstance in animations)
                {
                    if (animInstance.ParentGone())
                    {
                        animInstance.Dispose();
                    }
                }
            }

            ClearDarknessTexture();
            TryPreRendering();
            FixAutotiles();
            GenerateLightMap();

            var gridX = currentMap.MapGridX;
            var gridY = currentMap.MapGridY;
            //Draw Panoramas First...
            for (var x = gridX - 1; x <= gridX + 1; x++)
            {
                for (var y = gridY - 1; y <= gridY + 1; y++)
                {
                    if (x >= 0 && x < Globals.MapGridWidth && y >= 0 && y < Globals.MapGridHeight &&
                        Globals.MapGrid[x, y] != -1)
                    {
                        DrawMapPanorama(Globals.MapGrid[x, y]);
                    }
                }
            }

            for (var x = gridX - 1; x <= gridX + 1; x++)
            {
                for (var y = gridY - 1; y <= gridY + 1; y++)
                {
                    if (x >= 0 && x < Globals.MapGridWidth && y >= 0 && y < Globals.MapGridHeight &&
                        Globals.MapGrid[x, y] != -1)
                    {
                        DrawMap(Globals.MapGrid[x, y], 0);
                    }
                }
            }

            lock (AnimationLock)
            {
                foreach (AnimationInstance animInstance in LiveAnimations)
                {
                    animInstance.Draw(false);
                }
            }
            
            for (int y = 0; y < Options.MapHeight * 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    foreach (var entity in RenderingEntities[x, y])
                    {
                        entity.Draw();
                        EntitiesDrawn++;
                    }
                }
            }

            for (var x = gridX - 1; x <= gridX + 1; x++)
            {
                for (var y = gridY - 1; y <= gridY + 1; y++)
                {
                    if (x >= 0 && x < Globals.MapGridWidth && y >= 0 && y < Globals.MapGridHeight &&
                        Globals.MapGrid[x, y] != -1)
                    {
                        DrawMap(Globals.MapGrid[x, y], 1);
                    }
                }
            }
            
            for (int y = 0; y < Options.MapHeight * 3; y++)
            {
                for (int x = 3; x < 6; x++)
                {
                    foreach (var entity in RenderingEntities[x, y])
                    {
                        entity.Draw();
                        EntitiesDrawn++;
                    }
                }
            }

            for (var x = gridX - 1; x <= gridX + 1; x++)
            {
                for (var y = gridY - 1; y <= gridY + 1; y++)
                {
                    if (x >= 0 && x < Globals.MapGridWidth && y >= 0 && y < Globals.MapGridHeight &&
                        Globals.MapGrid[x, y] != -1)
                    {
                        DrawMap(Globals.MapGrid[x, y], 2);
                    }
                }
            }

            lock (AnimationLock)
            {
                foreach (AnimationInstance animInstance in LiveAnimations)
                {
                    animInstance.Draw(true);
                }
            }

            for (var x = gridX - 1; x <= gridX + 1; x++)
            {
                for (var y = gridY - 1; y <= gridY + 1; y++)
                {
                    if (x >= 0 && x < Globals.MapGridWidth && y >= 0 && y < Globals.MapGridHeight &&
                        Globals.MapGrid[x, y] != -1)
                    {
                        MapInstance map = MapInstance.Lookup.Get<MapInstance>(Globals.MapGrid[x, y]);
                        if (map != null)
                        {
                            map.DrawFog();
                            map.DrawOverlayGraphic();
                        }
                    }
                }
            }

            //Draw the players targets
            Globals.Me.DrawTargets();

            DrawOverlay();

            DrawDarkness();
            
            for (int y = 0; y < Options.MapHeight * 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    foreach (var entity in RenderingEntities[x, y])
                    {
                        entity.DrawName(null);
                        if (entity.GetType() != typeof(Event))
                        {
                            entity.DrawHpBar();
                            entity.DrawCastingBar();
                        }
                        entity.DrawChatBubbles();
                    }
                }
            }
            
            for (int y = 0; y < Options.MapHeight * 3; y++)
            {
                for (int x = 3; x < 6; x++)
                {
                    foreach (var entity in RenderingEntities[x, y])
                    {
                        entity.DrawName(null);
                        if (entity.GetType() != typeof(Event))
                        {
                            entity.DrawHpBar();
                            entity.DrawCastingBar();
                        }
                        entity.DrawChatBubbles();
                    }
                }
            }

            //Draw action msg's
            for (var x = gridX - 1; x <= gridX + 1; x++)
            {
                for (var y = gridY - 1; y <= gridY + 1; y++)
                {
                    if (x < 0 || x >= Globals.MapGridWidth || y < 0 || y >= Globals.MapGridHeight ||
                        Globals.MapGrid[x, y] == -1) continue;
                    var map = MapInstance.Lookup.Get<MapInstance>(Globals.MapGrid[x, y]);
                    map?.DrawActionMsgs();
                }
            }
        }

        //Game Rendering
        public static void Render()
        {
            if (!(Renderer?.Begin() ?? false)) return;
            if (Renderer.GetScreenWidth() != _oldWidth || Renderer.GetScreenHeight() != _oldHeight ||
                Renderer.DisplayModeChanged())
            {
                _darknessTexture = null;
                Gui.DestroyGwen();
                Gui.InitGwen();
                _oldWidth = Renderer.GetScreenWidth();
                _oldHeight = Renderer.GetScreenHeight();
            }
            Renderer.Clear(IntersectClientExtras.GenericClasses.Color.Black);
            DrawCalls = 0;
            MapsDrawn = 0;
            EntitiesDrawn = 0;
            LightsDrawn = 0;
            PreRenderedMapLayer = false;

            UpdateView();

            switch (Globals.GameState)
            {
                case GameStates.Intro:
                    DrawIntro();
                    break;
                case GameStates.Menu:
                    DrawMenu();
                    break;
                case GameStates.Loading:
                    break;
                case GameStates.InGame:
                    DrawInGame();
                    break;
                case GameStates.Error:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Gui.DrawGui();

            DrawGameTexture(Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1), CurrentView,
                new Color((int)GameFade.GetFade(), 0, 0, 0), null, GameBlendModes.None);
            Renderer.End();
        }

        private static void TryPreRendering(bool takeItEasy = true)
        {
            if (Globals.Database.RenderCaching && Globals.Me != null && Globals.Me.MapInstance != null)
            {
                var gridX = Globals.Me.MapInstance.MapGridX;
                var gridY = Globals.Me.MapInstance.MapGridY;
                for (int x = gridX - 1; x <= gridX + 1; x++)
                {
                    for (int y = gridY - 1; y <= gridY + 1; y++)
                    {
                        if (x >= 0 && x < Globals.MapGridWidth && y >= 0 && y < Globals.MapGridHeight &&
                            Globals.MapGrid[x, y] != -1)
                        {
                            var map = MapInstance.Lookup.Get<MapInstance>(Globals.MapGrid[x, y]);
                            if (map != null && !map.MapRendered)
                            {
                                if (!PreRenderedMapLayer || !takeItEasy)
                                {
                                    lock (map.MapLock)
                                    {
                                        if (!map.PreRenderMap()) return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void FixAutotiles()
        {
            if (Globals.Database.RenderCaching && Globals.Me != null &&
                MapInstance.Lookup.Get<MapInstance>(Globals.Me.CurrentMap) != null)
            {
                var gridX = MapInstance.Lookup.Get<MapInstance>(Globals.Me.CurrentMap).MapGridX;
                var gridY = MapInstance.Lookup.Get<MapInstance>(Globals.Me.CurrentMap).MapGridY;
                for (int x = gridX - 1; x <= gridX + 1; x++)
                {
                    for (int y = gridY - 1; y <= gridY + 1; y++)
                    {
                        if (x >= 0 && x < Globals.MapGridWidth && y >= 0 && y < Globals.MapGridHeight &&
                            Globals.MapGrid[x, y] != -1)
                        {
                            var map = MapInstance.Lookup.Get<MapInstance>(Globals.MapGrid[x, y]);
                            if (map != null && map.MapRendered)
                            {
                                map.FixAutotiles();
                            }
                        }
                    }
                }
            }
        }

        private static void DrawMap(int mapNum, int layer = 0)
        {
            var map = MapInstance.Lookup.Get<MapInstance>(mapNum);
            if (map == null) return;
            if (
                !new FloatRect(map.GetX(), map.GetY(), Options.TileWidth * Options.MapWidth,
                    Options.TileHeight * Options.MapHeight).IntersectsWith(CurrentView)) return;
            map.Draw(layer);
            if (layer == 0)
            {
                MapsDrawn++;
            }
        }

        private static void DrawMapPanorama(int mapNum)
        {
            var map = MapInstance.Lookup.Get<MapInstance>(mapNum);
            if (map != null)
            {
                if (
                    !new FloatRect(map.GetX(), map.GetY(), Options.TileWidth * Options.MapWidth,
                        Options.TileHeight * Options.MapHeight).IntersectsWith(CurrentView)) return;
                map.DrawPanorama();
            }
        }

        public static void DrawOverlay()
        {
            var map = MapInstance.Lookup.Get<MapInstance>(Globals.Me.CurrentMap);
            if (map != null)
            {
                float ecTime = Globals.System.GetTimeMS() - _overlayUpdate;

                if (OverlayColor.A != map.AHue ||
                    OverlayColor.R != map.RHue ||
                    OverlayColor.G != map.GHue ||
                    OverlayColor.B != map.BHue)
                {
                    if (OverlayColor.A < map.AHue)
                    {
                        if (OverlayColor.A + (int)(255 * ecTime / 2000f) > map.AHue)
                        {
                            OverlayColor.A = (byte)map.AHue;
                        }
                        else
                        {
                            OverlayColor.A += (byte)(255 * ecTime / 2000f);
                        }
                    }

                    if (OverlayColor.A > map.AHue)
                    {
                        if (OverlayColor.A - (int)(255 * ecTime / 2000f) < map.AHue)
                        {
                            OverlayColor.A = (byte)map.AHue;
                        }
                        else
                        {
                            OverlayColor.A -= (byte)(255 * ecTime / 2000f);
                        }
                    }

                    if (OverlayColor.R < map.RHue)
                    {
                        if (OverlayColor.R + (int)(255 * ecTime / 2000f) > map.RHue)
                        {
                            OverlayColor.R = (byte)map.RHue;
                        }
                        else
                        {
                            OverlayColor.R += (byte)(255 * ecTime / 2000f);
                        }
                    }

                    if (OverlayColor.R > map.RHue)
                    {
                        if (OverlayColor.R - (int)(255 * ecTime / 2000f) < map.RHue)
                        {
                            OverlayColor.R = (byte)map.RHue;
                        }
                        else
                        {
                            OverlayColor.R -= (byte)(255 * ecTime / 2000f);
                        }
                    }

                    if (OverlayColor.G < map.GHue)
                    {
                        if (OverlayColor.G + (int)(255 * ecTime / 2000f) > map.GHue)
                        {
                            OverlayColor.G = (byte)map.GHue;
                        }
                        else
                        {
                            OverlayColor.G += (byte)(255 * ecTime / 2000f);
                        }
                    }

                    if (OverlayColor.G > map.GHue)
                    {
                        if (OverlayColor.G - (int)(255 * ecTime / 2000f) < map.GHue)
                        {
                            OverlayColor.G = (byte)map.GHue;
                        }
                        else
                        {
                            OverlayColor.G -= (byte)(255 * ecTime / 2000f);
                        }
                    }

                    if (OverlayColor.B < map.BHue)
                    {
                        if (OverlayColor.B + (int)(255 * ecTime / 2000f) > map.BHue)
                        {
                            OverlayColor.B = (byte)map.BHue;
                        }
                        else
                        {
                            OverlayColor.B += (byte)(255 * ecTime / 2000f);
                        }
                    }

                    if (OverlayColor.B > map.BHue)
                    {
                        if (OverlayColor.B - (int)(255 * ecTime / 2000f) < map.BHue)
                        {
                            OverlayColor.B = (byte)map.BHue;
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

        public static FloatRect GetSourceRect(GameTexture gameTexture)
        {
            return gameTexture == null
                ? new FloatRect()
                : new FloatRect(0, 0, gameTexture.GetWidth(), gameTexture.GetHeight());
        }

        public static void DrawFullScreenTexture(GameTexture tex, float alpha = 1f)
        {
            int bgx = Renderer.GetScreenWidth() / 2 - tex.GetWidth() / 2;
            int bgy = Renderer.GetScreenHeight() / 2 - tex.GetHeight() / 2;
            int bgw = tex.GetWidth();
            int bgh = tex.GetHeight();
            int diff = 0;
            if (bgw < Renderer.GetScreenWidth())
            {
                diff = Renderer.GetScreenWidth() - bgw;
                bgx -= diff / 2;
                bgw += diff;
            }
            if (bgh < Renderer.GetScreenHeight())
            {
                diff = Renderer.GetScreenHeight() - bgh;
                bgy -= diff / 2;
                bgh += diff;
            }
            DrawGameTexture(tex, GetSourceRect(tex),
                new FloatRect(bgx + Renderer.GetView().X, bgy + Renderer.GetView().Y, bgw, bgh),
                new Color((int)(alpha * 255f), 255, 255, 255));
        }

        public static void DrawFullScreenTextureStretched(GameTexture tex)
        {
            DrawGameTexture(tex, GetSourceRect(tex),
                new FloatRect(Renderer.GetView().X, Renderer.GetView().Y, Renderer.GetScreenWidth(),
                    Renderer.GetScreenHeight()), Color.White);
        }

        public static void DrawFullScreenTextureFitWidth(GameTexture tex)
        {
            var scale = Renderer.GetScreenWidth() / (float)tex.GetWidth();
            var scaledHeight = tex.GetHeight() * scale;
            var offsetY = (Renderer.GetScreenHeight() - tex.GetHeight()) / 2f;
            DrawGameTexture(tex, GetSourceRect(tex),
                new FloatRect(
                    Renderer.GetView().X,
                    Renderer.GetView().Y + offsetY,
                    Renderer.GetScreenWidth(),
                    scaledHeight
                    ),
                Color.White);
        }

        public static void DrawFullScreenTextureFitHeight(GameTexture tex)
        {
            var scale = Renderer.GetScreenHeight() / (float)tex.GetHeight();
            var scaledWidth = tex.GetWidth() * scale;
            var offsetX = (Renderer.GetScreenWidth() - scaledWidth) / 2f;
            DrawGameTexture(tex, GetSourceRect(tex),
                new FloatRect(
                    Renderer.GetView().X + offsetX,
                    Renderer.GetView().Y,
                    scaledWidth,
                    Renderer.GetScreenHeight()
                    ),
                Color.White);
        }

        public static void DrawFullScreenTextureFitMinimum(GameTexture tex)
        {
            if (Renderer.GetScreenWidth() > Renderer.GetScreenHeight())
            {
                DrawFullScreenTextureFitHeight(tex);
            }
            else
            {
                DrawFullScreenTextureFitWidth(tex);
            }
        }

        public static void DrawFullScreenTextureFitMaximum(GameTexture tex)
        {
            if (Renderer.GetScreenWidth() < Renderer.GetScreenHeight())
            {
                DrawFullScreenTextureFitHeight(tex);
            }
            else
            {
                DrawFullScreenTextureFitWidth(tex);
            }
        }

        private static void UpdateView()
        {
            if (Globals.Me == null)
            {
                CurrentView = new FloatRect(0, 0, Renderer.GetScreenWidth(), Renderer.GetScreenHeight());
                return;
            }
            var map = MapInstance.Lookup.Get<MapInstance>(Globals.Me.CurrentMap);
            if (Globals.GameState == GameStates.InGame && map != null)
            {
                Player en = Globals.Me;
                float x = map.GetX() - Options.MapWidth * Options.TileWidth;
                float y = map.GetY() - Options.MapHeight * Options.TileHeight;
                float x1 = map.GetX() + (Options.MapWidth * Options.TileWidth) * 2;
                float y1 = map.GetY() + (Options.MapHeight * Options.TileHeight) * 2;
                if (map.HoldUp == 1)
                {
                    y += Options.MapHeight * Options.TileHeight;
                }
                if (map.HoldLeft == 1)
                {
                    x += Options.MapWidth * Options.TileWidth;
                }
                if (map.HoldRight == 1)
                {
                    x1 -= Options.MapWidth * Options.TileWidth;
                }
                if (map.HoldDown == 1)
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

        public static void CreateMapTextures(int count)
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
            return false;
        }

        public static void ReleaseMapTexture(GameRenderTexture releaseTex)
        {
            if (releaseTex.SetActive(false))
            {
                FreeMapTextures.Add(releaseTex);
            }
        }

        //Lighting
        private static void ClearDarknessTexture()
        {
            if (_darknessTexture == null)
            {
                _darknessTexture = Renderer.CreateRenderTexture(Renderer.GetScreenWidth(), Renderer.GetScreenHeight());
            }
            _darknessTexture.Clear(IntersectClientExtras.GenericClasses.Color.Black);
        }

        private static void GenerateLightMap()
        {
            var map = MapInstance.Lookup.Get<MapInstance>(Globals.Me.CurrentMap);
            if (map == null) return;
            if (_darknessTexture == null)
            {
                return;
            }

            if (map.IsIndoors)
            {
                DrawGameTexture(Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                    new FloatRect(0, 0, _darknessTexture.GetWidth(), _darknessTexture.GetHeight()),
                    new Color((byte)(_brightnessLevel), 255, 255, 255), _darknessTexture, GameBlendModes.Add);
            }
            else
            {
                DrawGameTexture(Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                    new FloatRect(0, 0, _darknessTexture.GetWidth(), _darknessTexture.GetHeight()),
                    new Color(255, 255, 255, 255), _darknessTexture, GameBlendModes.Add);
                DrawGameTexture(Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                    new FloatRect(0, 0, _darknessTexture.GetWidth(), _darknessTexture.GetHeight()),
                    new Color((int)ClientTime.GetTintColor().A, (int)ClientTime.GetTintColor().R,
                        (int)ClientTime.GetTintColor().G, (int)ClientTime.GetTintColor().B), _darknessTexture,
                    GameBlendModes.None);
            }
            AddLight((int)Math.Ceiling(Globals.Me.GetCenterPos().X), (int)Math.Ceiling(Globals.Me.GetCenterPos().Y),
                (int)_playerLightSize, (byte)_playerLightIntensity, _playerLightExpand,
                Color.FromArgb((int)_playerLightColor.A, (int)_playerLightColor.R, (int)_playerLightColor.G,
                    (int)_playerLightColor.B));

            DrawLights();
            _darknessTexture.End();
        }

        public static void DrawDarkness()
        {
            GameShader radialShader = Globals.ContentManager.GetShader("radialgradient");
            if (radialShader != null)
            {
                DrawGameTexture(_darknessTexture, CurrentView.Left, CurrentView.Top, null, GameBlendModes.Multiply);
            }
        }

        public static void AddLight(int x, int y, int size, byte intensity, float expand, Color color)
        {
            _lightQueue.Add(new LightBase(0, 0, x, y, intensity, size, expand, color));
            LightsDrawn++;
        }

        private static void DrawLights()
        {
            GameShader radialShader = Globals.ContentManager.GetShader("radialgradient");
            if (radialShader != null)
            {
                foreach (LightBase l in _lightQueue)
                {
                    int x = l.OffsetX - ((int)CurrentView.Left + l.Size);
                    int y = l.OffsetY - ((int)CurrentView.Top + l.Size);

                    radialShader.SetColor("LightColor", new IntersectClientExtras.GenericClasses.Color(l.Intensity, l.Color.R, l.Color.G, l.Color.B));
                    radialShader.SetFloat("Expand", l.Expand / 100f);

                    DrawGameTexture(Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                        new FloatRect(x, y, l.Size * 2, l.Size * 2), new Color(255, 255, 255, 255),
                        _darknessTexture, GameBlendModes.Add, radialShader,0,true);
                }
            }
            _lightQueue.Clear();
        }

        public static void UpdatePlayerLight()
        {
            //Draw Light Around Player
            var map = MapInstance.Lookup.Get<MapInstance>(Globals.Me.CurrentMap);
            if (map != null)
            {
                float ecTime = Globals.System.GetTimeMS() - _lightUpdate;
                float valChange = (255 * ecTime / 2000f);
                byte brightnessTarget = (byte)((map.Brightness / 100f) * 255);
                if (_brightnessLevel < brightnessTarget)
                {
                    if (_brightnessLevel + valChange > brightnessTarget)
                    {
                        _brightnessLevel = brightnessTarget;
                    }
                    else
                    {
                        _brightnessLevel += valChange;
                    }
                }

                if (_brightnessLevel > brightnessTarget)
                {
                    if (_brightnessLevel - valChange < brightnessTarget)
                    {
                        _brightnessLevel = brightnessTarget;
                    }
                    else
                    {
                        _brightnessLevel -= valChange;
                    }
                }

                if (_playerLightColor.R != map.PlayerLightColor.R ||
                    _playerLightColor.G != map.PlayerLightColor.G ||
                    _playerLightColor.B != map.PlayerLightColor.B)
                {
                    if (_playerLightColor.R < map.PlayerLightColor.R)
                    {
                        if (_playerLightColor.R + valChange > map.PlayerLightColor.R)
                        {
                            _playerLightColor.R = map.PlayerLightColor.R;
                        }
                        else
                        {
                            _playerLightColor.R += valChange;
                        }
                    }

                    if (_playerLightColor.R > map.PlayerLightColor.R)
                    {
                        if (_playerLightColor.R - valChange < map.PlayerLightColor.R)
                        {
                            _playerLightColor.R = map.PlayerLightColor.R;
                        }
                        else
                        {
                            _playerLightColor.R -= valChange;
                        }
                    }

                    if (_playerLightColor.G < map.PlayerLightColor.G)
                    {
                        if (_playerLightColor.G + valChange > map.PlayerLightColor.G)
                        {
                            _playerLightColor.G = map.PlayerLightColor.G;
                        }
                        else
                        {
                            _playerLightColor.G += valChange;
                        }
                    }

                    if (_playerLightColor.G > map.PlayerLightColor.G)
                    {
                        if (_playerLightColor.G - valChange < map.PlayerLightColor.G)
                        {
                            _playerLightColor.G = map.PlayerLightColor.G;
                        }
                        else
                        {
                            _playerLightColor.G -= valChange;
                        }
                    }

                    if (_playerLightColor.B < map.PlayerLightColor.B)
                    {
                        if (_playerLightColor.B + valChange > map.PlayerLightColor.B)
                        {
                            _playerLightColor.B = map.PlayerLightColor.B;
                        }
                        else
                        {
                            _playerLightColor.B += valChange;
                        }
                    }

                    if (_playerLightColor.B > map.PlayerLightColor.B)
                    {
                        if (_playerLightColor.B - valChange < map.PlayerLightColor.B)
                        {
                            _playerLightColor.B = map.PlayerLightColor.B;
                        }
                        else
                        {
                            _playerLightColor.B -= valChange;
                        }
                    }
                }

                if (_playerLightSize != map.PlayerLightSize)
                {
                    if (_playerLightSize < map.PlayerLightSize)
                    {
                        if (_playerLightSize + (500 * ecTime / 2000f) > map.PlayerLightSize)
                        {
                            _playerLightSize = map.PlayerLightSize;
                        }
                        else
                        {
                            _playerLightSize += (500 * ecTime / 2000f);
                        }
                    }

                    if (_playerLightSize > map.PlayerLightSize)
                    {
                        if (_playerLightSize - (500 * ecTime / 2000f) < map.PlayerLightSize)
                        {
                            _playerLightSize = map.PlayerLightSize;
                        }
                        else
                        {
                            _playerLightSize -= (500 * ecTime / 2000f);
                        }
                    }
                }

                if (_playerLightIntensity < map.PlayerLightIntensity)
                {
                    if (_playerLightIntensity + valChange > map.PlayerLightIntensity)
                    {
                        _playerLightIntensity = map.PlayerLightIntensity;
                    }
                    else
                    {
                        _playerLightIntensity += valChange;
                    }
                }

                if (_playerLightIntensity > map.AHue)
                {
                    if (_playerLightIntensity - valChange < map.PlayerLightIntensity)
                    {
                        _playerLightIntensity = map.PlayerLightIntensity;
                    }
                    else
                    {
                        _playerLightIntensity -= valChange;
                    }
                }

                if (_playerLightExpand < map.PlayerLightExpand)
                {
                    if (_playerLightExpand + (100f * ecTime / 2000f) > map.PlayerLightExpand)
                    {
                        _playerLightExpand = map.PlayerLightExpand;
                    }
                    else
                    {
                        _playerLightExpand += (100f * ecTime / 2000f);
                    }
                }

                if (_playerLightExpand > map.PlayerLightExpand)
                {
                    if (_playerLightExpand - (100f * ecTime / 2000f) < map.PlayerLightExpand)
                    {
                        _playerLightExpand = map.PlayerLightExpand;
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
        ///     Renders a specified texture onto a RenderTexture or the GameScreen (if renderTarget is passed as null) at the
        ///     coordinates given using a specified blending mode.
        /// </summary>
        /// <param name="tex">The texture to draw</param>
        /// <param name="x">X coordinate on the render target to draw to</param>
        /// <param name="y">Y coordinate on the render target to draw to</param>
        /// <param name="renderTarget">Where to draw to. If null it this will draw to the game screen.</param>
        /// <param name="blendMode">Which blend mode to use when rendering</param>
        public static void DrawGameTexture(GameTexture tex, float x, float y, GameRenderTexture renderTarget = null,
            GameBlendModes blendMode = GameBlendModes.None, GameShader shader = null, float rotationDegrees = 0.0f, bool drawImmediate = false)
        {
            var destRectangle = new FloatRect(x, y, tex.GetWidth(), tex.GetHeight());
            var srcRectangle = new FloatRect(0, 0, tex.GetWidth(), tex.GetHeight());
            DrawGameTexture(tex, srcRectangle, destRectangle, Color.White, renderTarget, blendMode, shader,
                rotationDegrees, drawImmediate);
        }

        /// <summary>
        ///     Renders a specified texture onto a RenderTexture or the GameScreen (if renderTarget is passed as null) at the
        ///     coordinates given using a specified blending mode.
        /// </summary>
        /// <param name="tex">The texture to draw</param>
        /// <param name="x">X coordinate on the render target to draw to</param>
        /// <param name="y">Y coordinate on the render target to draw to</param>
        /// <param name="renderColor">Color mask to draw with. Default is Color.White</param>
        /// <param name="renderTarget">Where to draw to. If null it this will draw to the game screen.</param>
        /// <param name="blendMode">Which blend mode to use when rendering</param>
        public static void DrawGameTexture(GameTexture tex, float x, float y, Color renderColor,
            GameRenderTexture renderTarget = null, GameBlendModes blendMode = GameBlendModes.None,
            GameShader shader = null, float rotationDegrees = 0.0f, bool drawImmediate = false)
        {
            var destRectangle = new FloatRect(x, y, tex.GetWidth(), tex.GetHeight());
            var srcRectangle = new FloatRect(0, 0, tex.GetWidth(), tex.GetHeight());
            DrawGameTexture(tex, srcRectangle, destRectangle, renderColor, renderTarget, blendMode, shader,
                rotationDegrees, drawImmediate);
        }

        /// <summary>
        ///     Renders a specified texture onto a RenderTexture or the GameScreen (if renderTarget is passed as null) at the
        ///     coordinates given using a specified blending mode.
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
        public static void DrawGameTexture(GameTexture tex, float dx, float dy, float sx, float sy, float w, float h,
            GameRenderTexture renderTarget = null, GameBlendModes blendMode = GameBlendModes.None,
            GameShader shader = null, float rotationDegrees = 0.0f, bool drawImmediate = false)
        {
            var destRectangle = new FloatRect(dx, dy, w, h);
            var srcRectangle = new FloatRect(sx, sy, w, h);
            DrawGameTexture(tex, srcRectangle, destRectangle, Color.White, renderTarget, blendMode, shader,
                rotationDegrees, drawImmediate);
        }

        public static void DrawGameTexture(GameTexture tex, FloatRect srcRectangle, FloatRect targetRect,
            Color renderColor, GameRenderTexture renderTarget = null,
            GameBlendModes blendMode = GameBlendModes.None,
            GameShader shader = null, float rotationDegrees = 0.0f, bool drawImmediate = false)
        {
            if (tex == null) return;
            Renderer.DrawTexture(tex, srcRectangle, targetRect,
                IntersectClientExtras.GenericClasses.Color.FromArgb(renderColor.A, renderColor.R, renderColor.G, renderColor.B), renderTarget, blendMode,
                shader,
                rotationDegrees,false,drawImmediate);
        }
    }
}