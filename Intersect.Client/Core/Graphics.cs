using System;
using System.Collections.Generic;
using System.Linq;
using Intersect.Client.Entities;
using Intersect.Client.Entities.Events;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.General;
using Intersect.Client.Maps;
using Intersect.Configuration;
using Intersect.Enums;
using Intersect.GameObjects;

namespace Intersect.Client.Core
{

    public static class Graphics
    {

        public static GameFont ActionMsgFont;

        public static object AnimationLock = new object();

        //Darkness Stuff
        public static float BrightnessLevel;

        public static GameFont ChatBubbleFont;

        public static FloatRect CurrentView;

        public static GameShader DefaultShader;

        //Rendering Variables
        public static int DrawCalls;

        public static int EntitiesDrawn;

        public static GameFont EntityNameFont;

        //Screen Values
        public static GameFont GameFont;

        public static object GfxLock = new object();

        //Grid Switched
        public static bool GridSwitched;

        public static int LightsDrawn;

        //Animations
        public static List<Animation> LiveAnimations = new List<Animation>();

        public static int MapsDrawn;

        //Overlay Stuff
        public static Color OverlayColor = Color.Transparent;

        public static ColorF PlayerLightColor = ColorF.White;

        //Game Renderer
        public static GameRenderer Renderer;

        //Cache the Y based rendering
        public static HashSet<Entity>[,] RenderingEntities;

        private static GameContentManager sContentManager;

        private static GameRenderTexture sDarknessTexture;

        private static long sFadeTimer;

        private static List<LightBase> sLightQueue = new List<LightBase>();

        //Player Spotlight Values
        private static long sLightUpdate;

        private static int sOldHeight;

        //Resolution
        private static int sOldWidth;

        private static long sOverlayUpdate;

        private static float sPlayerLightExpand;

        private static float sPlayerLightIntensity = 255;

        private static float sPlayerLightSize;

        public static GameFont UIFont;

        //Init Functions
        public static void InitGraphics()
        {
            Renderer.Init();
            sContentManager = Globals.ContentManager;
            sContentManager.LoadAll();
            GameFont = FindFont(ClientConfiguration.Instance.GameFont);
            UIFont = FindFont(ClientConfiguration.Instance.UIFont);
            EntityNameFont = FindFont(ClientConfiguration.Instance.EntityNameFont);
            ChatBubbleFont = FindFont(ClientConfiguration.Instance.ChatBubbleFont);
            ActionMsgFont = FindFont(ClientConfiguration.Instance.ActionMsgFont);
        }

        public static GameFont FindFont(string font)
        {
            var size = 8;

            if (font.IndexOf(',') > -1)
            {
                var parts = font.Split(',');
                font = parts[0];
                int.TryParse(parts[1], out size);
            }

            return sContentManager.GetFont(font, size);
        }

        public static void InitInGame()
        {
            RenderingEntities = new HashSet<Entity>[6, Options.MapHeight * 5];
            for (var z = 0; z < 6; z++)
            {
                for (var i = 0; i < Options.MapHeight * 5; i++)
                {
                    RenderingEntities[z, i] = new HashSet<Entity>();
                }
            }
        }

        public static void DrawIntro()
        {
            var imageTex = sContentManager.GetTexture(
                GameContentManager.TextureType.Image, ClientConfiguration.Instance.IntroImages[Globals.IntroIndex]
            );

            if (imageTex != null)
            {
                DrawFullScreenTextureFitMinimum(imageTex);
            }
        }

        public static void DrawMenu()
        {
            var imageTex = sContentManager.GetTexture(
                GameContentManager.TextureType.Gui, ClientConfiguration.Instance.MenuBackground
            );

            if (imageTex != null)
            {
                DrawFullScreenTexture(imageTex);
            }
        }

        public static void DrawInGame()
        {
            var currentMap = Globals.Me.MapInstance;
            if (currentMap == null)
            {
                return;
            }

            if (Globals.NeedsMaps)
            {
                return;
            }

            if (GridSwitched)
            {
                //Brightness
                var brightnessTarget = (byte) (currentMap.Brightness / 100f * 255);
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

            var animations = LiveAnimations.ToArray();
            foreach (var animInstance in animations)
            {
                if (animInstance.ParentGone())
                {
                    animInstance.Dispose();
                }
            }

            ClearDarknessTexture();

            var gridX = currentMap.MapGridX;
            var gridY = currentMap.MapGridY;

            //Draw Panoramas First...
            for (var x = gridX - 1; x <= gridX + 1; x++)
            {
                for (var y = gridY - 1; y <= gridY + 1; y++)
                {
                    if (x >= 0 &&
                        x < Globals.MapGridWidth &&
                        y >= 0 &&
                        y < Globals.MapGridHeight &&
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
                    if (x >= 0 &&
                        x < Globals.MapGridWidth &&
                        y >= 0 &&
                        y < Globals.MapGridHeight &&
                        Globals.MapGrid[x, y] != Guid.Empty)
                    {
                        DrawMap(Globals.MapGrid[x, y], 0);
                    }
                }
            }

            foreach (var animInstance in animations)
            {
                animInstance.Draw(false);
            }

            for (var y = 0; y < Options.MapHeight * 5; y++)
            {
                for (var x = 0; x < 3; x++)
                {
                    foreach (var entity in RenderingEntities[x, y])
                    {
                        entity.Draw();
                        EntitiesDrawn++;
                    }

                    if (x == 0 && y > 0 && y % Options.MapHeight == 0)
                    {
                        for (var x1 = gridX - 1; x1 <= gridX + 1; x1++)
                        {
                            var y1 = gridY - 2 + (int) Math.Floor(y / (float) Options.MapHeight);
                            if (x1 >= 0 &&
                                x1 < Globals.MapGridWidth &&
                                y1 >= 0 &&
                                y1 < Globals.MapGridHeight &&
                                Globals.MapGrid[x1, y1] != Guid.Empty)
                            {
                                var map = MapInstance.Get(Globals.MapGrid[x1, y1]);
                                if (map != null)
                                {
                                    map.DrawItemsAndLights();
                                }
                            }
                        }
                    }
                }
            }

            foreach (var animInstance in animations)
            {
                animInstance.Draw(false, true);
                animInstance.Draw(true, true);
            }

            for (var x = gridX - 1; x <= gridX + 1; x++)
            {
                for (var y = gridY - 1; y <= gridY + 1; y++)
                {
                    if (x >= 0 &&
                        x < Globals.MapGridWidth &&
                        y >= 0 &&
                        y < Globals.MapGridHeight &&
                        Globals.MapGrid[x, y] != Guid.Empty)
                    {
                        DrawMap(Globals.MapGrid[x, y], 1);
                    }
                }
            }

            for (var y = 0; y < Options.MapHeight * 5; y++)
            {
                for (var x = 3; x < 6; x++)
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
                    if (x >= 0 &&
                        x < Globals.MapGridWidth &&
                        y >= 0 &&
                        y < Globals.MapGridHeight &&
                        Globals.MapGrid[x, y] != Guid.Empty)
                    {
                        DrawMap(Globals.MapGrid[x, y], 2);
                    }
                }
            }

            foreach (var animInstance in animations)
            {
                animInstance.Draw(true);
            }
            

            for (var x = gridX - 1; x <= gridX + 1; x++)
            {
                for (var y = gridY - 1; y <= gridY + 1; y++)
                {
                    if (x >= 0 &&
                        x < Globals.MapGridWidth &&
                        y >= 0 &&
                        y < Globals.MapGridHeight &&
                        Globals.MapGrid[x, y] != Guid.Empty)
                    {
                        var map = MapInstance.Get(Globals.MapGrid[x, y]);
                        if (map != null)
                        {
                            map.DrawWeather();
                            map.DrawFog();
                            map.DrawOverlayGraphic();
                            map.DrawItemNames();
                        }
                    }
                }
            }

            //Draw the players targets
            Globals.Me.DrawTargets();

            DrawOverlay();

            GenerateLightMap();
            DrawDarkness();

            for (var y = 0; y < Options.MapHeight * 5; y++)
            {
                for (var x = 0; x < 3; x++)
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

            for (var y = 0; y < Options.MapHeight * 5; y++)
            {
                for (var x = 3; x < 6; x++)
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
                    if (x < 0 ||
                        x >= Globals.MapGridWidth ||
                        y < 0 ||
                        y >= Globals.MapGridHeight ||
                        Globals.MapGrid[x, y] == Guid.Empty)
                    {
                        continue;
                    }

                    var map = MapInstance.Get(Globals.MapGrid[x, y]);
                    map?.DrawActionMsgs();
                }
            }

            foreach (var animInstance in animations)
            {
                animInstance.EndDraw();
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

            if (!(Renderer?.Begin() ?? false))
            {
                return;
            }

            if (Renderer.GetScreenWidth() != sOldWidth ||
                Renderer.GetScreenHeight() != sOldHeight ||
                Renderer.DisplayModeChanged())
            {
                sDarknessTexture = null;
                Interface.Interface.DestroyGwen();
                Interface.Interface.InitGwen();
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

            Interface.Interface.DrawGui();

            DrawGameTexture(
                Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1), CurrentView,
                new Color((int) Fade.GetFade(), 0, 0, 0), null, GameBlendModes.None
            );

            // Draw our mousecursor at the very end, but not when taking screenshots.
            if (!takingScreenshot && !string.IsNullOrWhiteSpace(ClientConfiguration.Instance.MouseCursor))
            {
                var renderLoc = ConvertToWorldPoint(Globals.InputManager.GetMousePosition());
                DrawGameTexture(
                    Globals.ContentManager.GetTexture(GameContentManager.TextureType.Misc, ClientConfiguration.Instance.MouseCursor), renderLoc.X, renderLoc.Y
               );
            }
            
            Renderer.End();

            if (takingScreenshot)
            {
                Renderer.EndScreenshot();
            }
        }

        private static void DrawMap(Guid mapId, int layer = 0)
        {
            var map = MapInstance.Get(mapId);
            if (map == null)
            {
                return;
            }

            if (!new FloatRect(
                map.GetX(), map.GetY(), Options.TileWidth * Options.MapWidth, Options.TileHeight * Options.MapHeight
            ).IntersectsWith(CurrentView))
            {
                return;
            }

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
                if (!new FloatRect(
                    map.GetX(), map.GetY(), Options.TileWidth * Options.MapWidth, Options.TileHeight * Options.MapHeight
                ).IntersectsWith(CurrentView))
                {
                    return;
                }

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
                        if (OverlayColor.A + (int) (255 * ecTime / 2000f) > map.AHue)
                        {
                            OverlayColor.A = (byte) map.AHue;
                        }
                        else
                        {
                            OverlayColor.A += (byte) (255 * ecTime / 2000f);
                        }
                    }

                    if (OverlayColor.A > map.AHue)
                    {
                        if (OverlayColor.A - (int) (255 * ecTime / 2000f) < map.AHue)
                        {
                            OverlayColor.A = (byte) map.AHue;
                        }
                        else
                        {
                            OverlayColor.A -= (byte) (255 * ecTime / 2000f);
                        }
                    }

                    if (OverlayColor.R < map.RHue)
                    {
                        if (OverlayColor.R + (int) (255 * ecTime / 2000f) > map.RHue)
                        {
                            OverlayColor.R = (byte) map.RHue;
                        }
                        else
                        {
                            OverlayColor.R += (byte) (255 * ecTime / 2000f);
                        }
                    }

                    if (OverlayColor.R > map.RHue)
                    {
                        if (OverlayColor.R - (int) (255 * ecTime / 2000f) < map.RHue)
                        {
                            OverlayColor.R = (byte) map.RHue;
                        }
                        else
                        {
                            OverlayColor.R -= (byte) (255 * ecTime / 2000f);
                        }
                    }

                    if (OverlayColor.G < map.GHue)
                    {
                        if (OverlayColor.G + (int) (255 * ecTime / 2000f) > map.GHue)
                        {
                            OverlayColor.G = (byte) map.GHue;
                        }
                        else
                        {
                            OverlayColor.G += (byte) (255 * ecTime / 2000f);
                        }
                    }

                    if (OverlayColor.G > map.GHue)
                    {
                        if (OverlayColor.G - (int) (255 * ecTime / 2000f) < map.GHue)
                        {
                            OverlayColor.G = (byte) map.GHue;
                        }
                        else
                        {
                            OverlayColor.G -= (byte) (255 * ecTime / 2000f);
                        }
                    }

                    if (OverlayColor.B < map.BHue)
                    {
                        if (OverlayColor.B + (int) (255 * ecTime / 2000f) > map.BHue)
                        {
                            OverlayColor.B = (byte) map.BHue;
                        }
                        else
                        {
                            OverlayColor.B += (byte) (255 * ecTime / 2000f);
                        }
                    }

                    if (OverlayColor.B > map.BHue)
                    {
                        if (OverlayColor.B - (int) (255 * ecTime / 2000f) < map.BHue)
                        {
                            OverlayColor.B = (byte) map.BHue;
                        }
                        else
                        {
                            OverlayColor.B -= (byte) (255 * ecTime / 2000f);
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
            var bgx = Renderer.GetScreenWidth() / 2 - tex.GetWidth() / 2;
            var bgy = Renderer.GetScreenHeight() / 2 - tex.GetHeight() / 2;
            var bgw = tex.GetWidth();
            var bgh = tex.GetHeight();
            var diff = 0;
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

            DrawGameTexture(
                tex, GetSourceRect(tex),
                new FloatRect(bgx + Renderer.GetView().X, bgy + Renderer.GetView().Y, bgw, bgh),
                new Color((int) (alpha * 255f), 255, 255, 255)
            );
        }

        public static void DrawFullScreenTextureStretched(GameTexture tex)
        {
            DrawGameTexture(
                tex, GetSourceRect(tex),
                new FloatRect(
                    Renderer.GetView().X, Renderer.GetView().Y, Renderer.GetScreenWidth(), Renderer.GetScreenHeight()
                ), Color.White
            );
        }

        public static void DrawFullScreenTextureFitWidth(GameTexture tex)
        {
            var scale = Renderer.GetScreenWidth() / (float) tex.GetWidth();
            var scaledHeight = tex.GetHeight() * scale;
            var offsetY = (Renderer.GetScreenHeight() - tex.GetHeight()) / 2f;
            DrawGameTexture(
                tex, GetSourceRect(tex),
                new FloatRect(
                    Renderer.GetView().X, Renderer.GetView().Y + offsetY, Renderer.GetScreenWidth(), scaledHeight
                ), Color.White
            );
        }

        public static void DrawFullScreenTextureFitHeight(GameTexture tex)
        {
            var scale = Renderer.GetScreenHeight() / (float) tex.GetHeight();
            var scaledWidth = tex.GetWidth() * scale;
            var offsetX = (Renderer.GetScreenWidth() - scaledWidth) / 2f;
            DrawGameTexture(
                tex, GetSourceRect(tex),
                new FloatRect(
                    Renderer.GetView().X + offsetX, Renderer.GetView().Y, scaledWidth, Renderer.GetScreenHeight()
                ), Color.White
            );
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
                var en = Globals.Me;
                var x = map.GetX() - Options.MapWidth * Options.TileWidth;
                var y = map.GetY() - Options.MapHeight * Options.TileHeight;
                var x1 = map.GetX() + Options.MapWidth * Options.TileWidth * 2;
                var y1 = map.GetY() + Options.MapHeight * Options.TileHeight * 2;
                if (map.CameraHolds[(int) Directions.Up])
                {
                    y += Options.MapHeight * Options.TileHeight;
                }

                if (map.CameraHolds[(int) Directions.Left])
                {
                    x += Options.MapWidth * Options.TileWidth;
                }

                if (map.CameraHolds[(int) Directions.Right])
                {
                    x1 -= Options.MapWidth * Options.TileWidth;
                }

                if (map.CameraHolds[(int) Directions.Down])
                {
                    y1 -= Options.MapHeight * Options.TileHeight;
                }

                var w = x1 - x;
                var h = y1 - y;
                var restrictView = new FloatRect(x, y, w, h);
                CurrentView = new FloatRect(
                    (int) Math.Ceiling(en.GetCenterPos().X - Renderer.GetScreenWidth() / 2f),
                    (int) Math.Ceiling(en.GetCenterPos().Y - Renderer.GetScreenHeight() / 2f),
                    Renderer.GetScreenWidth(), Renderer.GetScreenHeight()
                );

                if (restrictView.Width >= CurrentView.Width)
                {
                    if (CurrentView.Left < restrictView.Left)
                    {
                        CurrentView.X = restrictView.Left;
                    }

                    if (CurrentView.Left + CurrentView.Width > restrictView.Left + restrictView.Width)
                    {
                        CurrentView.X -=
                            CurrentView.Left + CurrentView.Width - (restrictView.Left + restrictView.Width);
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
                        CurrentView.Y -=
                            CurrentView.Top + CurrentView.Height - (restrictView.Top + restrictView.Height);
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
            if (map == null)
            {
                return;
            }

            if (sDarknessTexture == null)
            {
                return;
            }

            if (map.IsIndoors)
            {
                DrawGameTexture(
                    Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                    new FloatRect(0, 0, sDarknessTexture.GetWidth(), sDarknessTexture.GetHeight()),
                    new Color((byte) BrightnessLevel, 255, 255, 255), sDarknessTexture, GameBlendModes.Add
                );
            }
            else
            {
                DrawGameTexture(
                    Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                    new FloatRect(0, 0, sDarknessTexture.GetWidth(), sDarknessTexture.GetHeight()),
                    new Color(255, 255, 255, 255), sDarknessTexture, GameBlendModes.Add
                );

                DrawGameTexture(
                    Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                    new FloatRect(0, 0, sDarknessTexture.GetWidth(), sDarknessTexture.GetHeight()),
                    new Color(
                        (int) Time.GetTintColor().A, (int) Time.GetTintColor().R, (int) Time.GetTintColor().G,
                        (int) Time.GetTintColor().B
                    ), sDarknessTexture, GameBlendModes.None
                );
            }

            AddLight(
                (int) Math.Ceiling(Globals.Me.GetCenterPos().X), (int) Math.Ceiling(Globals.Me.GetCenterPos().Y),
                (int) sPlayerLightSize, (byte) sPlayerLightIntensity, sPlayerLightExpand,
                Color.FromArgb(
                    (int) PlayerLightColor.A, (int) PlayerLightColor.R, (int) PlayerLightColor.G,
                    (int) PlayerLightColor.B
                )
            );

            DrawLights();
            sDarknessTexture.End();
        }

        public static void DrawDarkness()
        {
            var radialShader = Globals.ContentManager.GetShader("radialgradient");
            if (radialShader != null)
            {
                DrawGameTexture(sDarknessTexture, CurrentView.Left, CurrentView.Top, null, GameBlendModes.Multiply);
            }
        }

        public static void AddLight(int x, int y, int size, byte intensity, float expand, Color color)
        {
            if (size == 0)
            {
                return;
            }

            sLightQueue.Add(new LightBase(0, 0, x, y, intensity, size, expand, color));
            LightsDrawn++;
        }

        private static void DrawLights()
        {
            var radialShader = Globals.ContentManager.GetShader("radialgradient");
            if (radialShader != null)
            {
                foreach (var light in sLightQueue.GroupBy(c => c.GetHashCode()))
                {
                    foreach (var l in light)
                    {
                        var x = l.OffsetX - ((int)CurrentView.Left + l.Size);
                        var y = l.OffsetY - ((int)CurrentView.Top + l.Size);

                        radialShader.SetColor("LightColor", new Color(l.Intensity, l.Color.R, l.Color.G, l.Color.B));
                        radialShader.SetFloat("Expand", l.Expand / 100f);

                        DrawGameTexture(
                            Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                            new FloatRect(x, y, l.Size * 2, l.Size * 2), new Color(255, 255, 255, 255), sDarknessTexture, GameBlendModes.Add, radialShader, 0, false
                        );

                    }
                }

                sLightQueue.Clear();
            }
        }

        public static void UpdatePlayerLight()
        {
            //Draw Light Around Player
            var map = MapInstance.Get(Globals.Me.CurrentMap);
            if (map != null)
            {
                float ecTime = Globals.System.GetTimeMs() - sLightUpdate;
                var valChange = 255 * ecTime / 2000f;
                var brightnessTarget = (byte) (map.Brightness / 100f * 255);
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
                        if (sPlayerLightSize + 500 * ecTime / 2000f > map.PlayerLightSize)
                        {
                            sPlayerLightSize = map.PlayerLightSize;
                        }
                        else
                        {
                            sPlayerLightSize += 500 * ecTime / 2000f;
                        }
                    }

                    if (sPlayerLightSize > map.PlayerLightSize)
                    {
                        if (sPlayerLightSize - 500 * ecTime / 2000f < map.PlayerLightSize)
                        {
                            sPlayerLightSize = map.PlayerLightSize;
                        }
                        else
                        {
                            sPlayerLightSize -= 500 * ecTime / 2000f;
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
                    if (sPlayerLightExpand + 100f * ecTime / 2000f > map.PlayerLightExpand)
                    {
                        sPlayerLightExpand = map.PlayerLightExpand;
                    }
                    else
                    {
                        sPlayerLightExpand += 100f * ecTime / 2000f;
                    }
                }

                if (sPlayerLightExpand > map.PlayerLightExpand)
                {
                    if (sPlayerLightExpand - 100f * ecTime / 2000f < map.PlayerLightExpand)
                    {
                        sPlayerLightExpand = map.PlayerLightExpand;
                    }
                    else
                    {
                        sPlayerLightExpand -= 100f * ecTime / 2000f;
                    }
                }

                sLightUpdate = Globals.System.GetTimeMs();
            }
        }

        //Helper Functions
        /// <summary>
        /// Convert a position on the screen to a position on the actual map for rendering.
        /// </summary>
        /// <param name="windowPoint">The point to convert.</param>
        /// <returns>The converted point.</returns>
        public static Pointf ConvertToWorldPoint(Pointf windowPoint)
        {
            return new Pointf((int)Math.Floor(windowPoint.X + CurrentView.Left), (int)Math.Floor(windowPoint.Y + CurrentView.Top));
        }

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
        public static void DrawGameTexture(
            GameTexture tex,
            float x,
            float y,
            GameRenderTexture renderTarget = null,
            GameBlendModes blendMode = GameBlendModes.None,
            GameShader shader = null,
            float rotationDegrees = 0.0f,
            bool drawImmediate = false
        )
        {
            var destRectangle = new FloatRect(x, y, tex.GetWidth(), tex.GetHeight());
            var srcRectangle = new FloatRect(0, 0, tex.GetWidth(), tex.GetHeight());
            DrawGameTexture(
                tex, srcRectangle, destRectangle, Color.White, renderTarget, blendMode, shader, rotationDegrees,
                drawImmediate
            );
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
        public static void DrawGameTexture(
            GameTexture tex,
            float x,
            float y,
            Color renderColor,
            GameRenderTexture renderTarget = null,
            GameBlendModes blendMode = GameBlendModes.None,
            GameShader shader = null,
            float rotationDegrees = 0.0f,
            bool drawImmediate = false
        )
        {
            var destRectangle = new FloatRect(x, y, tex.GetWidth(), tex.GetHeight());
            var srcRectangle = new FloatRect(0, 0, tex.GetWidth(), tex.GetHeight());
            DrawGameTexture(
                tex, srcRectangle, destRectangle, renderColor, renderTarget, blendMode, shader, rotationDegrees,
                drawImmediate
            );
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
        public static void DrawGameTexture(
            GameTexture tex,
            float dx,
            float dy,
            float sx,
            float sy,
            float w,
            float h,
            GameRenderTexture renderTarget = null,
            GameBlendModes blendMode = GameBlendModes.None,
            GameShader shader = null,
            float rotationDegrees = 0.0f,
            bool drawImmediate = false
        )
        {
            if (tex == null)
            {
                return;
            }

            Renderer.DrawTexture(
                tex, sx, sy, w, h, dx, dy, w, h, Color.White, renderTarget, blendMode, shader, rotationDegrees, false,
                drawImmediate
            );
        }

        public static void DrawGameTexture(
            GameTexture tex,
            FloatRect srcRectangle,
            FloatRect targetRect,
            Color renderColor,
            GameRenderTexture renderTarget = null,
            GameBlendModes blendMode = GameBlendModes.None,
            GameShader shader = null,
            float rotationDegrees = 0.0f,
            bool drawImmediate = false
        )
        {
            if (tex == null)
            {
                return;
            }

            Renderer.DrawTexture(
                tex, srcRectangle.X, srcRectangle.Y, srcRectangle.Width, srcRectangle.Height, targetRect.X,
                targetRect.Y, targetRect.Width, targetRect.Height,
                Color.FromArgb(renderColor.A, renderColor.R, renderColor.G, renderColor.B), renderTarget, blendMode,
                shader, rotationDegrees, false, drawImmediate
            );
        }

    }

}