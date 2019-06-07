using System;
using System.Collections.Generic;
using Intersect.Client.Entities;
using Intersect.Client.Entities.Events;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.General;
using Intersect.Client.Maps;
using Intersect.Client.UI;
using Intersect.Config;
using Intersect.Enums;
using Intersect.GameObjects;

namespace Intersect.Client
{
    public static class GameGraphics
    {
        //Game Renderer
        public static GameRenderer Renderer;

        public static GameShader DefaultShader;
        private static GameContentManager sContentManager;

        //Resolution
        private static int sOldWidth;

        private static int sOldHeight;

        //Screen Values
        public static GameFont GameFont;

        public static FloatRect CurrentView;

        //Darkness Stuff
        public static float BrightnessLevel;

        private static GameRenderTexture sDarknessTexture;

        //Overlay Stuff
        public static Color OverlayColor = Color.Transparent;

        private static long sOverlayUpdate;

        //Player Spotlight Values
        private static long sLightUpdate;

        private static float sPlayerLightIntensity = 255;
        private static float sPlayerLightSize;
        private static float sPlayerLightExpand;
        public static ColorF PlayerLightColor = ColorF.White;
        private static List<LightBase> sLightQueue = new List<LightBase>();
        private static long sFadeTimer;

        //Grid Switched
        public static bool GridSwitched;

        //Rendering Variables
        public static int DrawCalls;

        public static int EntitiesDrawn;
        public static int LightsDrawn;
        public static int MapsDrawn;

        //Cache the Y based rendering
        public static HashSet<Entity>[,] RenderingEntities;
        
        public static object GfxLock = new object();

        //Animations
        public static List<AnimationInstance> LiveAnimations = new List<AnimationInstance>();

        public static object AnimationLock = new object();

        //Init Functions
        public static void InitGraphics()
        {
            Renderer.Init();
            sContentManager = Globals.ContentManager;
            sContentManager.LoadAll();
            GameFont = sContentManager.GetFont(Gui.ActiveFont, 8);
        }

        public static void InitInGame()
        {
            RenderingEntities = new HashSet<Entity>[6, Options.MapHeight * 5];
            for (int z = 0; z < 6; z++)
            {
                for (var i = 0; i < Options.MapHeight * 5; i++)
                {
                    RenderingEntities[z, i] = new HashSet<Entity>();
                }
            }
        }

        public static void DrawIntro()
        {
            GameTexture imageTex = sContentManager.GetTexture(GameContentManager.TextureType.Image, ClientOptions.IntroImages[Globals.IntroIndex]);
            if (imageTex != null)
            {
                DrawFullScreenTextureFitMinimum(imageTex);
            }
        }

        public static void DrawMenu()
        {
            GameTexture imageTex = sContentManager.GetTexture(GameContentManager.TextureType.Gui, ClientOptions.MenuBackground);
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
                return;
            }
            if (GridSwitched)
            {
                //Brightness
                byte brightnessTarget = (byte) ((currentMap.Brightness / 100f) * 255);
                BrightnessLevel = brightnessTarget;
                PlayerLightColor.R = currentMap.PlayerLightColor.R;
                PlayerLightColor.G = currentMap.PlayerLightColor.G;
                PlayerLightColor.B = currentMap.PlayerLightColor.B;
                sPlayerLightSize = currentMap.PlayerLightSize;
                sPlayerLightIntensity = currentMap.PlayerLightIntensity;
                sPlayerLightExpand = currentMap.PlayerLightExpand;

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
            GenerateLightMap();

            var gridX = currentMap.MapGridX;
            var gridY = currentMap.MapGridY;
            //Draw Panoramas First...
            for (var x = gridX - 1; x <= gridX + 1; x++)
            {
                for (var y = gridY - 1; y <= gridY + 1; y++)
                {
                    if (x >= 0 && x < Globals.MapGridWidth && y >= 0 && y < Globals.MapGridHeight &&
                        Globals.MapGrid[x, y] != Guid.Empty)
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
                        Globals.MapGrid[x, y] != Guid.Empty)
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
            
            for (int y = 0; y < Options.MapHeight * 5; y++)
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

            lock (AnimationLock)
            {
                foreach (AnimationInstance animInstance in LiveAnimations)
                {
                    animInstance.Draw(false,true);
                    animInstance.Draw(true, true);
                }
            }

            for (var x = gridX - 1; x <= gridX + 1; x++)
            {
                for (var y = gridY - 1; y <= gridY + 1; y++)
                {
                    if (x >= 0 && x < Globals.MapGridWidth && y >= 0 && y < Globals.MapGridHeight &&
                        Globals.MapGrid[x, y] != Guid.Empty)
                    {
                        DrawMap(Globals.MapGrid[x, y], 1);
                    }
                }
            }
            
            for (int y = 0; y < Options.MapHeight * 5; y++)
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
                        Globals.MapGrid[x, y] != Guid.Empty)
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
                        Globals.MapGrid[x, y] != Guid.Empty)
                    {
                        MapInstance map = MapInstance.Get(Globals.MapGrid[x, y]);
                        if (map != null)
                        {
                            map.DrawWeather();
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

            for (int y = 0; y < Options.MapHeight * 5; y++)
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

            for (int y = 0; y < Options.MapHeight * 5; y++)
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
                        Globals.MapGrid[x, y] == Guid.Empty) continue;
                    var map = MapInstance.Get(Globals.MapGrid[x, y]);
                    map?.DrawActionMsgs();
                }
            }
        }

        //Game Rendering
        public static void Render()
        {
            var takingScreenshot = false;
            if (Renderer?.ScreenshotRequests.Count > 0)
            {
                takingScreenshot = Renderer.BeginScreenshot();
            }

            if (!(Renderer?.Begin() ?? false)) return;
            if (Renderer.GetScreenWidth() != sOldWidth || Renderer.GetScreenHeight() != sOldHeight ||
                Renderer.DisplayModeChanged())
            {
                sDarknessTexture = null;
                Gui.DestroyGwen();
                Gui.InitGwen();
                sOldWidth = Renderer.GetScreenWidth();
                sOldHeight = Renderer.GetScreenHeight();
            }
            Renderer.Clear(Color.Black);
            DrawCalls = 0;
            MapsDrawn = 0;
            EntitiesDrawn = 0;
            LightsDrawn = 0;

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

            if (takingScreenshot)
            {
                Renderer.EndScreenshot();
            }
        }

        private static void DrawMap(Guid mapId, int layer = 0)
        {
            var map = MapInstance.Get(mapId);
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

        private static void DrawMapPanorama(Guid mapId)
        {
            var map = MapInstance.Get(mapId);
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
            var map = MapInstance.Get(Globals.Me.CurrentMap);
            if (map != null)
            {
                float ecTime = Globals.System.GetTimeMs() - sOverlayUpdate;

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
            sOverlayUpdate = Globals.System.GetTimeMs();
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
                Renderer.SetView(CurrentView);
                return;
            }
            var map = MapInstance.Get(Globals.Me.CurrentMap);
            if (Globals.GameState == GameStates.InGame && map != null)
            {
                Player en = Globals.Me;
                float x = map.GetX() - Options.MapWidth * Options.TileWidth;
                float y = map.GetY() - Options.MapHeight * Options.TileHeight;
                float x1 = map.GetX() + (Options.MapWidth * Options.TileWidth) * 2;
                float y1 = map.GetY() + (Options.MapHeight * Options.TileHeight) * 2;
                if (map.CameraHolds[(int)Directions.Up])
                {
                    y += Options.MapHeight * Options.TileHeight;
                }
                if (map.CameraHolds[(int)Directions.Left])
                {
                    x += Options.MapWidth * Options.TileWidth;
                }
                if (map.CameraHolds[(int)Directions.Right])
                {
                    x1 -= Options.MapWidth * Options.TileWidth;
                }
                if (map.CameraHolds[(int)Directions.Down])
                {
                    y1 -= Options.MapHeight * Options.TileHeight;
                }
                float w = x1 - x;
                float h = y1 - y;
                var restrictView = new FloatRect(x, y, w, h);
                CurrentView = new FloatRect((int)Math.Ceiling(en.GetCenterPos().X - Renderer.GetScreenWidth() / 2f),
                    (int)Math.Ceiling(en.GetCenterPos().Y - Renderer.GetScreenHeight() / 2f), Renderer.GetScreenWidth(),
                    Renderer.GetScreenHeight());
                if (restrictView.Width >= CurrentView.Width)
                {
                    if (CurrentView.Left < restrictView.Left)
                    {
                        CurrentView.X = restrictView.Left;
                    }
                    if (CurrentView.Left + CurrentView.Width > restrictView.Left + restrictView.Width)
                    {
                        CurrentView.X -= (CurrentView.Left + CurrentView.Width) -
                                         (restrictView.Left + restrictView.Width);
                    }
                }
                if (restrictView.Height >= CurrentView.Height)
                {
                    if (CurrentView.Top < restrictView.Top)
                    {
                        CurrentView.Y = restrictView.Top;
                    }
                    if (CurrentView.Top + CurrentView.Height > restrictView.Top + restrictView.Height)
                    {
                        CurrentView.Y -= (CurrentView.Top + CurrentView.Height) -
                                         (restrictView.Top + restrictView.Height);
                    }
                }
            }
            else
            {
                CurrentView = new FloatRect(0, 0, Renderer.GetScreenWidth(), Renderer.GetScreenHeight());
            }
            Renderer.SetView(CurrentView);
        }

        //Lighting
        private static void ClearDarknessTexture()
        {
            if (sDarknessTexture == null)
            {
                sDarknessTexture = Renderer.CreateRenderTexture(Renderer.GetScreenWidth(), Renderer.GetScreenHeight());
            }
            sDarknessTexture.Clear(Color.Black);
        }

        private static void GenerateLightMap()
        {
            var map = MapInstance.Get(Globals.Me.CurrentMap);
            if (map == null) return;
            if (sDarknessTexture == null)
            {
                return;
            }

            if (map.IsIndoors)
            {
                DrawGameTexture(Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                    new FloatRect(0, 0, sDarknessTexture.GetWidth(), sDarknessTexture.GetHeight()),
                    new Color((byte)(BrightnessLevel), 255, 255, 255), sDarknessTexture, GameBlendModes.Add);
            }
            else
            {
                DrawGameTexture(Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                    new FloatRect(0, 0, sDarknessTexture.GetWidth(), sDarknessTexture.GetHeight()),
                    new Color(255, 255, 255, 255), sDarknessTexture, GameBlendModes.Add);
                DrawGameTexture(Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                    new FloatRect(0, 0, sDarknessTexture.GetWidth(), sDarknessTexture.GetHeight()),
                    new Color((int)ClientTime.GetTintColor().A, (int)ClientTime.GetTintColor().R,
                        (int)ClientTime.GetTintColor().G, (int)ClientTime.GetTintColor().B), sDarknessTexture,
                    GameBlendModes.None);
            }
            AddLight((int)Math.Ceiling(Globals.Me.GetCenterPos().X), (int)Math.Ceiling(Globals.Me.GetCenterPos().Y),
                (int)sPlayerLightSize, (byte)sPlayerLightIntensity, sPlayerLightExpand,
                Color.FromArgb((int)PlayerLightColor.A, (int)PlayerLightColor.R, (int)PlayerLightColor.G,
                    (int)PlayerLightColor.B));

            DrawLights();
            sDarknessTexture.End();
        }

        public static void DrawDarkness()
        {
            GameShader radialShader = Globals.ContentManager.GetShader("radialgradient");
            if (radialShader != null)
            {
                DrawGameTexture(sDarknessTexture, CurrentView.Left, CurrentView.Top, null, GameBlendModes.Multiply);
            }
        }

        public static void AddLight(int x, int y, int size, byte intensity, float expand, Color color)
        {
            sLightQueue.Add(new LightBase(0, 0, x, y, intensity, size, expand, color));
            LightsDrawn++;
        }

        private static void DrawLights()
        {
            GameShader radialShader = Globals.ContentManager.GetShader("radialgradient");
            if (radialShader != null)
            {
                foreach (LightBase l in sLightQueue)
                {
                    int x = l.OffsetX - ((int)CurrentView.Left + l.Size);
                    int y = l.OffsetY - ((int)CurrentView.Top + l.Size);

                    radialShader.SetColor("LightColor", new Color(l.Intensity, l.Color.R, l.Color.G, l.Color.B));
                    radialShader.SetFloat("Expand", l.Expand / 100f);

                    DrawGameTexture(Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                        new FloatRect(x, y, l.Size * 2, l.Size * 2), new Color(255, 255, 255, 255),
                        sDarknessTexture, GameBlendModes.Add, radialShader,0,true);
                }
            }
            sLightQueue.Clear();
        }

        public static void UpdatePlayerLight()
        {
            //Draw Light Around Player
            var map = MapInstance.Get(Globals.Me.CurrentMap);
            if (map != null)
            {
                float ecTime = Globals.System.GetTimeMs() - sLightUpdate;
                float valChange = (255 * ecTime / 2000f);
                byte brightnessTarget = (byte)((map.Brightness / 100f) * 255);
                if (BrightnessLevel < brightnessTarget)
                {
                    if (BrightnessLevel + valChange > brightnessTarget)
                    {
                        BrightnessLevel = brightnessTarget;
                    }
                    else
                    {
                        BrightnessLevel += valChange;
                    }
                }

                if (BrightnessLevel > brightnessTarget)
                {
                    if (BrightnessLevel - valChange < brightnessTarget)
                    {
                        BrightnessLevel = brightnessTarget;
                    }
                    else
                    {
                        BrightnessLevel -= valChange;
                    }
                }

                if (PlayerLightColor.R != map.PlayerLightColor.R ||
                    PlayerLightColor.G != map.PlayerLightColor.G ||
                    PlayerLightColor.B != map.PlayerLightColor.B)
                {
                    if (PlayerLightColor.R < map.PlayerLightColor.R)
                    {
                        if (PlayerLightColor.R + valChange > map.PlayerLightColor.R)
                        {
                            PlayerLightColor.R = map.PlayerLightColor.R;
                        }
                        else
                        {
                            PlayerLightColor.R += valChange;
                        }
                    }

                    if (PlayerLightColor.R > map.PlayerLightColor.R)
                    {
                        if (PlayerLightColor.R - valChange < map.PlayerLightColor.R)
                        {
                            PlayerLightColor.R = map.PlayerLightColor.R;
                        }
                        else
                        {
                            PlayerLightColor.R -= valChange;
                        }
                    }

                    if (PlayerLightColor.G < map.PlayerLightColor.G)
                    {
                        if (PlayerLightColor.G + valChange > map.PlayerLightColor.G)
                        {
                            PlayerLightColor.G = map.PlayerLightColor.G;
                        }
                        else
                        {
                            PlayerLightColor.G += valChange;
                        }
                    }

                    if (PlayerLightColor.G > map.PlayerLightColor.G)
                    {
                        if (PlayerLightColor.G - valChange < map.PlayerLightColor.G)
                        {
                            PlayerLightColor.G = map.PlayerLightColor.G;
                        }
                        else
                        {
                            PlayerLightColor.G -= valChange;
                        }
                    }

                    if (PlayerLightColor.B < map.PlayerLightColor.B)
                    {
                        if (PlayerLightColor.B + valChange > map.PlayerLightColor.B)
                        {
                            PlayerLightColor.B = map.PlayerLightColor.B;
                        }
                        else
                        {
                            PlayerLightColor.B += valChange;
                        }
                    }

                    if (PlayerLightColor.B > map.PlayerLightColor.B)
                    {
                        if (PlayerLightColor.B - valChange < map.PlayerLightColor.B)
                        {
                            PlayerLightColor.B = map.PlayerLightColor.B;
                        }
                        else
                        {
                            PlayerLightColor.B -= valChange;
                        }
                    }
                }

                if (sPlayerLightSize != map.PlayerLightSize)
                {
                    if (sPlayerLightSize < map.PlayerLightSize)
                    {
                        if (sPlayerLightSize + (500 * ecTime / 2000f) > map.PlayerLightSize)
                        {
                            sPlayerLightSize = map.PlayerLightSize;
                        }
                        else
                        {
                            sPlayerLightSize += (500 * ecTime / 2000f);
                        }
                    }

                    if (sPlayerLightSize > map.PlayerLightSize)
                    {
                        if (sPlayerLightSize - (500 * ecTime / 2000f) < map.PlayerLightSize)
                        {
                            sPlayerLightSize = map.PlayerLightSize;
                        }
                        else
                        {
                            sPlayerLightSize -= (500 * ecTime / 2000f);
                        }
                    }
                }

                if (sPlayerLightIntensity < map.PlayerLightIntensity)
                {
                    if (sPlayerLightIntensity + valChange > map.PlayerLightIntensity)
                    {
                        sPlayerLightIntensity = map.PlayerLightIntensity;
                    }
                    else
                    {
                        sPlayerLightIntensity += valChange;
                    }
                }

                if (sPlayerLightIntensity > map.AHue)
                {
                    if (sPlayerLightIntensity - valChange < map.PlayerLightIntensity)
                    {
                        sPlayerLightIntensity = map.PlayerLightIntensity;
                    }
                    else
                    {
                        sPlayerLightIntensity -= valChange;
                    }
                }

                if (sPlayerLightExpand < map.PlayerLightExpand)
                {
                    if (sPlayerLightExpand + (100f * ecTime / 2000f) > map.PlayerLightExpand)
                    {
                        sPlayerLightExpand = map.PlayerLightExpand;
                    }
                    else
                    {
                        sPlayerLightExpand += (100f * ecTime / 2000f);
                    }
                }

                if (sPlayerLightExpand > map.PlayerLightExpand)
                {
                    if (sPlayerLightExpand - (100f * ecTime / 2000f) < map.PlayerLightExpand)
                    {
                        sPlayerLightExpand = map.PlayerLightExpand;
                    }
                    else
                    {
                        sPlayerLightExpand -= (100f * ecTime / 2000f);
                    }
                }
                sLightUpdate = Globals.System.GetTimeMs();
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
            if (tex == null) return;
            Renderer.DrawTexture(tex, sx,sy,w,h,dx,dy,w,h,Color.White, renderTarget, blendMode,shader,rotationDegrees, false, drawImmediate);
        }

        public static void DrawGameTexture(GameTexture tex, FloatRect srcRectangle, FloatRect targetRect,
            Color renderColor, GameRenderTexture renderTarget = null,
            GameBlendModes blendMode = GameBlendModes.None,
            GameShader shader = null, float rotationDegrees = 0.0f, bool drawImmediate = false)
        {
            if (tex == null) return;
            Renderer.DrawTexture(tex,srcRectangle.X,srcRectangle.Y,srcRectangle.Width,srcRectangle.Height,targetRect.X,targetRect.Y,targetRect.Width,targetRect.Height,Color.FromArgb(renderColor.A, renderColor.R, renderColor.G, renderColor.B), renderTarget, blendMode,shader,rotationDegrees,false,drawImmediate);
        }
    }
}