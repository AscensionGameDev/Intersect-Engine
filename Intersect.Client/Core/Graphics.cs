using Intersect.Client.Entities;
using Intersect.Client.Entities.Events;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.General;
using Intersect.Client.Maps;
using Intersect.Configuration;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Utilities;

namespace Intersect.Client.Core;

public static partial class Graphics
{

    public static GameFont? ActionMsgFont;

    public static object AnimationLock = new();

    //Darkness Stuff
    public static float BrightnessLevel;

    public static GameFont? ChatBubbleFont;

    private static FloatRect _currentView;

    public static FloatRect CurrentView
    {
        get => _currentView;
        set
        {
            _currentView = value;
            Renderer?.SetView(_currentView);
        }
    }
    
    public static FloatRect WorldViewport => new(CurrentView.Position, CurrentView.Size / (Globals.Database?.WorldZoom ?? 1));

    public static GameShader? DefaultShader;

    //Rendering Variables
    private static GameTexture? sMenuBackground;

    public static int DrawCalls;

    public static int EntitiesDrawn;

    public static GameFont? EntityNameFont;

    //Screen Values
    public static GameFont? GameFont;

    public static object GfxLock = new();

    //Grid Switched
    public static bool GridSwitched;

    public static int LightsDrawn;

    //Animations
    public static List<Animation> LiveAnimations = [];

    public static int MapsDrawn;

    private static int sMenuBackgroundIndex;

    private static long sMenuBackgroundInterval;

    //Overlay Stuff
    public static Color OverlayColor = Color.Transparent;

    public static ColorF PlayerLightColor = ColorF.White;

    //Game Renderer
    public static GameRenderer? Renderer;

    //Cache the Y based rendering
    public static HashSet<Entity>[,] RenderingEntities = new HashSet<Entity>[6, Options.MapHeight * 5];

    private static GameContentManager sContentManager = null!;

    private static GameRenderTexture? sDarknessTexture;

    private static readonly List<LightBase> sLightQueue = [];

    //Player Spotlight Values
    private static long sLightUpdate;

    private static int sOldHeight;

    //Resolution
    private static int sOldWidth;

    private static long sOverlayUpdate;

    private static float sPlayerLightExpand;

    private static float sPlayerLightIntensity = 255;

    private static float sPlayerLightSize;

    public static GameFont? UIFont;

    public static float BaseWorldScale => Options.Instance?.MapOpts?.TileScale ?? 1;

    //Init Functions
    public static void InitGraphics()
    {
        Renderer?.Init();
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
            _ = int.TryParse(parts[1], out size);
        }

        return sContentManager.GetFont(font, size);
    }

    public static void InitInGame()
    {
        for (var z = 0; z < 6; z++)
        {
            for (var i = 0; i < Options.MapHeight * 5; i++)
            {
                RenderingEntities[z, i] = [];
            }
        }
    }

    public static void DrawIntro()
    {
        var imageTex = sContentManager.GetTexture(
            Framework.Content.TextureType.Image, ClientConfiguration.Instance.IntroImages[Globals.IntroIndex]
        );

        if (imageTex != null)
        {
            DrawFullScreenTextureFitMinimum(imageTex);
        }
    }

    private static void DrawMenu()
    {
        // No background in the main menu.
        if (ClientConfiguration.Instance.MenuBackground.Count == 0)
        {
            return;
        }

        // Animated background in the main menu.
        if (ClientConfiguration.Instance.MenuBackground.Count > 1)
        {
            sMenuBackground = sContentManager.GetTexture(
                TextureType.Gui, ClientConfiguration.Instance.MenuBackground[sMenuBackgroundIndex]
            );

            if (sMenuBackground == null)
            {
                return;
            }

            var currentTimeMs = Timing.Global.MillisecondsUtc;

            if (sMenuBackgroundInterval < currentTimeMs)
            {
                sMenuBackgroundIndex =
                    (sMenuBackgroundIndex + 1) % ClientConfiguration.Instance.MenuBackground.Count;

                sMenuBackgroundInterval = currentTimeMs + ClientConfiguration.Instance.MenuBackgroundFrameInterval;
            }
        }

        // Static background in the main menu.
        else
        {
            sMenuBackground = sContentManager.GetTexture(
                TextureType.Gui, ClientConfiguration.Instance.MenuBackground[0]
            );

            if (sMenuBackground == null)
            {
                return;
            }
        }

        // Switch between the preferred display mode, then render the fullscreen texture.
        switch (ClientConfiguration.Instance.MenuBackgroundDisplayMode)
        {
            case DisplayMode.Default:
                DrawFullScreenTexture(sMenuBackground);
                break;

            case DisplayMode.Center:
                DrawFullScreenTextureCentered(sMenuBackground);
                break;

            case DisplayMode.Stretch:
                DrawFullScreenTextureStretched(sMenuBackground);
                break;

            case DisplayMode.FitHeight:
                DrawFullScreenTextureFitHeight(sMenuBackground);
                break;

            case DisplayMode.FitWidth:
                DrawFullScreenTextureFitWidth(sMenuBackground);
                break;

            case DisplayMode.Fit:
                DrawFullScreenTextureFitMaximum(sMenuBackground);
                break;

            case DisplayMode.Cover:
                DrawFullScreenTextureFitMinimum(sMenuBackground);
                break;
        }
    }

    public static void DrawInGame(TimeSpan deltaTime)
    {
        if (Globals.Me?.MapInstance is not MapInstance currentMap)
        {
            return;
        }

        if (Globals.NeedsMaps || Globals.MapGrid == default)
        {
            return;
        }

        if (GridSwitched)
        {
            //Brightness
            var brightnessTarget = (byte)(currentMap.Brightness / 100f * 255);
            BrightnessLevel = brightnessTarget;
            PlayerLightColor.R = currentMap.PlayerLightColor.R;
            PlayerLightColor.G = currentMap.PlayerLightColor.G;
            PlayerLightColor.B = currentMap.PlayerLightColor.B;
            sPlayerLightSize = currentMap.PlayerLightSize;
            sPlayerLightIntensity = currentMap.PlayerLightIntensity;
            sPlayerLightExpand = currentMap.PlayerLightExpand;

            //Overlay
            OverlayColor.A = (byte)currentMap.AHue;
            OverlayColor.R = (byte)currentMap.RHue;
            OverlayColor.G = (byte)currentMap.GHue;
            OverlayColor.B = (byte)currentMap.BHue;

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

        // Clear our previous darkness texture.
        ClearDarknessTexture();

        var gridX = currentMap.GridX;
        var gridY = currentMap.GridY;

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

        // Handle our plugin drawing.
        Globals.OnGameDraw(DrawStates.GroundLayers, deltaTime);

        foreach (var animInstance in animations)
        {
            animInstance.Draw(false);
        }

        // Handle our plugin drawing.
        Globals.OnGameDraw(DrawStates.BelowPlayer, deltaTime);

        for (var y = 0; y < Options.MapHeight * 5; y++)
        {
            for (var x = 0; x < 3; x++)
            {
                foreach (var entity in RenderingEntities[x, y])
                {
                    // Handle our plugin drawing.
                    Globals.OnGameDraw(DrawStates.BeforeEntity, entity, deltaTime);

                    entity.Draw();

                    // Handle our plugin drawing.
                    Globals.OnGameDraw(DrawStates.AfterEntity, entity, deltaTime);

                    EntitiesDrawn++;
                }

                if (x == 0 && y > 0 && y % Options.MapHeight == 0)
                {
                    for (var x1 = gridX - 1; x1 <= gridX + 1; x1++)
                    {
                        var y1 = gridY - 2 + (int)Math.Floor(y / (float)Options.MapHeight);
                        if (x1 >= 0 &&
                            x1 < Globals.MapGridWidth &&
                            y1 >= 0 &&
                            y1 < Globals.MapGridHeight &&
                            Globals.MapGrid[x1, y1] != Guid.Empty)
                        {
                            var map = MapInstance.Get(Globals.MapGrid[x1, y1]);
                            map?.DrawItemsAndLights();
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
                    // Handle our plugin drawing.
                    Globals.OnGameDraw(DrawStates.BeforeEntity, entity, deltaTime);

                    entity.Draw();

                    // Handle our plugin drawing.
                    Globals.OnGameDraw(DrawStates.AfterEntity, entity, deltaTime);

                    EntitiesDrawn++;
                }
            }
        }

        // Handle our plugin drawing.
        Globals.OnGameDraw(DrawStates.AbovePlayer, deltaTime);

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
        // Handle our plugin drawing.
        Globals.OnGameDraw(DrawStates.FringeLayers, deltaTime);

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
        Globals.Me?.DrawTargets();

        DrawOverlay();

        // Draw lighting effects.
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
    public static void Render(TimeSpan deltaTime, TimeSpan _)
    {
        var takingScreenshot = false;
        if (Renderer?.ScreenshotRequests.Count > 0)
        {
            takingScreenshot = Renderer.BeginScreenshot();
        }

        if (Renderer == default)
        {
            return;
        }

        Renderer.Scale = Globals.GameState == GameStates.InGame ? Globals.Database.WorldZoom : 1.0f;

        if (!Renderer.Begin())
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
                DrawInGame(deltaTime);

                break;
            case GameStates.Error:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        Renderer.Scale = Globals.Database.UIScale;

        Interface.Interface.DrawGui();

        DrawGameTexture(
            Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1), CurrentView,
            new Color((int)Fade.Alpha, 0, 0, 0), null, GameBlendModes.None
        );

        // Draw our mousecursor at the very end, but not when taking screenshots.
        if (!takingScreenshot && !string.IsNullOrWhiteSpace(ClientConfiguration.Instance.MouseCursor))
        {
            var renderLoc = ConvertToWorldPointNoZoom(Globals.InputManager.GetMousePosition());
            DrawGameTexture(
                Globals.ContentManager.GetTexture(Framework.Content.TextureType.Misc, ClientConfiguration.Instance.MouseCursor), renderLoc.X, renderLoc.Y
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
        ).IntersectsWith(WorldViewport))
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
            ).IntersectsWith(WorldViewport))
            {
                return;
            }

            map.DrawPanorama();
        }
    }

    public static void DrawOverlay()
    {
        if (Renderer == default)
        {
            return;
        }

        var map = MapInstance.Get(Globals.Me?.MapId ?? Guid.Empty);
        if (map != null)
        {
            float ecTime = Timing.Global.MillisecondsUtc - sOverlayUpdate;

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
        sOverlayUpdate = Timing.Global.MillisecondsUtc;
    }

    public static FloatRect GetSourceRect(GameTexture gameTexture)
    {
        return gameTexture == null
            ? new FloatRect()
            : new FloatRect(0, 0, gameTexture.GetWidth(), gameTexture.GetHeight());
    }

    public static void DrawFullScreenTexture(GameTexture tex, float alpha = 1f)
    {
        if (Renderer == default)
        {
            return;
        }

        var bgx = Renderer.GetScreenWidth() / 2 - tex.GetWidth() / 2;
        var bgy = Renderer.GetScreenHeight() / 2 - tex.GetHeight() / 2;
        var bgw = tex.GetWidth();
        var bgh = tex.GetHeight();
        int diff;

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
            new Color((int)(alpha * 255f), 255, 255, 255)
        );
    }

    public static void DrawFullScreenTextureCentered(GameTexture tex, float alpha = 1f)
    {
        if (Renderer == default)
        {
            return;
        }

        var bgx = Renderer.GetScreenWidth() / 2 - tex.GetWidth() / 2;
        var bgy = Renderer.GetScreenHeight() / 2 - tex.GetHeight() / 2;
        var bgw = tex.GetWidth();
        var bgh = tex.GetHeight();

        DrawGameTexture(
            tex, GetSourceRect(tex),
            new FloatRect(bgx + Renderer.GetView().X, bgy + Renderer.GetView().Y, bgw, bgh),
            new Color((int)(alpha * 255f), 255, 255, 255)
        );
    }

    public static void DrawFullScreenTextureStretched(GameTexture tex)
    {
        if (Renderer == default)
        {
            return;
        }

        DrawGameTexture(
            tex, GetSourceRect(tex),
            new FloatRect(
                Renderer.GetView().X, Renderer.GetView().Y, Renderer.GetScreenWidth(), Renderer.GetScreenHeight()
            ), Color.White
        );
    }

    public static void DrawFullScreenTextureFitWidth(GameTexture tex)
    {
        if (Renderer == default)
        {
            return;
        }

        var scale = Renderer.GetScreenWidth() / (float)tex.GetWidth();
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
        if (Renderer == default)
        {
            return;
        }

        var scale = Renderer.GetScreenHeight() / (float)tex.GetHeight();
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
        if (Renderer == default)
        {
            return;
        }

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
        if (Renderer == default)
        {
            return;
        }

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
        if (Renderer == default)
        {
            return;
        }

        var scale = Renderer.Scale;

        if (Globals.GameState != GameStates.InGame || !MapInstance.TryGet(Globals.Me?.MapId ?? Guid.Empty, out var map))
        {
            var sw = Renderer.GetScreenWidth();
            var sh = Renderer.GetScreenHeight();
            var sx = 0;
            var sy = 0;
            CurrentView = new FloatRect(sx, sy, sw / scale, sh / scale);
            return;
        }

        var mapWidth = Options.MapWidth * Options.TileWidth;
        var mapHeight = Options.MapHeight * Options.TileHeight;

        var en = Globals.Me;

        if (en == null)
        {
            return;
        }

        float x = mapWidth;
        float y = mapHeight;
        float x1 = mapWidth * 2;
        float y1 = mapHeight * 2;

        if (map.CameraHolds[(int)Direction.Left])
        {
            x -= mapWidth;
        }

        if (map.CameraHolds[(int)Direction.Up])
        {
            y -= mapHeight;
        }

        if (map.CameraHolds[(int)Direction.Right])
        {
            x1 -= mapWidth;
        }

        if (map.CameraHolds[(int)Direction.Down])
        {
            y1 -= mapHeight;
        }

        x = map.GetX() - x;
        y = map.GetY() - y;
        x1 = map.GetX() + x1;
        y1 = map.GetY() + y1;

        var w = x1 - x;
        var h = y1 - y;
        var restrictView = new FloatRect(x, y, w, h );
        var newView = new FloatRect(
            (int)Math.Ceiling(en.Center.X - Renderer.ScreenWidth / scale / 2f),
            (int)Math.Ceiling(en.Center.Y - Renderer.ScreenHeight / scale / 2f),
            Renderer.ScreenWidth / scale,
            Renderer.ScreenHeight / scale
        );

        if (restrictView.Width >= newView.Width)
        {
            if (newView.Left < restrictView.Left)
            {
                newView.X = restrictView.Left;
            }

            if (newView.Right > restrictView.Right)
            {
                newView.X = restrictView.Right - newView.Width;
            }
        }

        if (restrictView.Height >= newView.Height)
        {
            if (newView.Top < restrictView.Top)
            {
                newView.Y = restrictView.Top;
            }

            if (newView.Bottom > restrictView.Bottom)
            {
                newView.Y = restrictView.Bottom - newView.Height;
            }
        }

        CurrentView = new FloatRect(
            newView.X,
            newView.Y,
            newView.Width * scale,
            newView.Height * scale
        );
    }

    //Lighting
    private static void ClearDarknessTexture()
    {
        // If we're not allowed to draw lighting, exit out.
        if (!Globals.Database.EnableLighting || Renderer == default)
        {
            return;
        }

        sDarknessTexture ??= Renderer.CreateRenderTexture(Renderer.GetScreenWidth(), Renderer.GetScreenHeight());
        sDarknessTexture.Clear(Color.Black);
    }

    private static void GenerateLightMap()
    {
        // If we're not allowed to draw lighting, exit out.
        if (!Globals.Database.EnableLighting || Renderer == default || Globals.Me == default)
        {
            return;
        }

        var map = MapInstance.Get(Globals.Me.MapId);
        if (map == null || sDarknessTexture == null)
        {
            return;
        }

        var destRect = new FloatRect(new Pointf(), sDarknessTexture.Dimensions / Globals.Database.WorldZoom);
        if (map.IsIndoors)
        {
            DrawGameTexture(
                Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                destRect,
                new Color((byte)BrightnessLevel, 255, 255, 255), sDarknessTexture, GameBlendModes.Add
            );
        }
        else
        {
            DrawGameTexture(
                Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                destRect,
                new Color(255, 255, 255, 255), sDarknessTexture, GameBlendModes.Add
            );

            DrawGameTexture(
                Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                destRect,
                new Color(
                    (int)Time.GetTintColor().A, (int)Time.GetTintColor().R, (int)Time.GetTintColor().G,
                    (int)Time.GetTintColor().B
                ), sDarknessTexture, GameBlendModes.None
            );
        }

        AddLight(
            (int)Math.Ceiling(Globals.Me.Center.X), (int)Math.Ceiling(Globals.Me.Center.Y),
            (int)sPlayerLightSize, (byte)sPlayerLightIntensity, sPlayerLightExpand,
            Color.FromArgb(
                (int)PlayerLightColor.A, (int)PlayerLightColor.R, (int)PlayerLightColor.G,
                (int)PlayerLightColor.B
            )
        );

        DrawLights();
        sDarknessTexture.End();
    }

    public static void DrawDarkness()
    {
        // If we're not allowed to draw lighting, exit out.
        if (!Globals.Database.EnableLighting || sDarknessTexture == default)
        {
            return;
        }

        var radialShader = Globals.ContentManager.GetShader("radialgradient");
        if (radialShader != null)
        {
            DrawGameTexture(
                sDarknessTexture,
                sDarknessTexture.Bounds,
                WorldViewport,
                Color.White,
                blendMode: GameBlendModes.Multiply
            );
        }
    }

    public static void AddLight(int x, int y, int size, byte intensity, float expand, Color color)
    {
        // If we're not allowed to draw lighting, exit out.
        if (!Globals.Database.EnableLighting)
        {
            return;
        }

        if (size == 0)
        {
            return;
        }

        sLightQueue.Add(new LightBase(0, 0, x, y, intensity, size, expand, color));
        LightsDrawn++;
    }

    private static void DrawLights()
    {
        // If we're not allowed to draw lighting, exit out.
        if (!Globals.Database.EnableLighting || Renderer == default)
        {
            return;
        }

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
        // If we're not allowed to draw lighting, exit out.
        if (!Globals.Database.EnableLighting || Globals.Me == default)
        {
            return;
        }

        //Draw Light Around Player
        var map = MapInstance.Get(Globals.Me.MapId);
        if (map != null)
        {
            float ecTime = Timing.Global.MillisecondsUtc - sLightUpdate;
            var valChange = 255 * ecTime / 2000f;
            var brightnessTarget = (byte)(map.Brightness / 100f * 255);
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

            // Cap instensity between 0 and 255 so as not to overflow (as it is an alpha value)
            sPlayerLightIntensity = (float)MathHelper.Clamp(sPlayerLightIntensity, 0f, 255f);
            sLightUpdate = Timing.Global.MillisecondsUtc;
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
        return new Pointf(
            (int)Math.Floor(windowPoint.X / Globals.Database.WorldZoom + CurrentView.Left),
            (int)Math.Floor(windowPoint.Y / Globals.Database.WorldZoom + CurrentView.Top)
        );
    }

    /// <summary>
    /// Converts a point in the window to its place in the world view without respecting zoom
    /// </summary>
    /// <param name="windowPoint">The point to convert.</param>
    /// <returns>The converted point.</returns>
    public static Pointf ConvertToWorldPointNoZoom(Pointf windowPoint)
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
    /// <param name="shader">Which shader to use when rendering</param>
    /// <param name="rotationDegrees">How much to rotate the texture in degrees</param>
    /// <param name="drawImmediate">If true, the texture will be drawn immediately. If false, it will be queued for drawing.</param>
    public static void DrawGameTexture(
        GameTexture tex,
        float x,
        float y,
        GameRenderTexture? renderTarget = null,
        GameBlendModes blendMode = GameBlendModes.None,
        GameShader? shader = null,
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
    /// <param name="shader">Which shader to use when rendering</param>
    /// <param name="rotationDegrees">How much to rotate the texture in degrees</param>
    /// <param name="drawImmediate">If true, the texture will be drawn immediately. If false, it will be queued for drawing.</param>
    public static void DrawGameTexture(
        GameTexture tex,
        float x,
        float y,
        Color renderColor,
        GameRenderTexture? renderTarget = null,
        GameBlendModes blendMode = GameBlendModes.None,
        GameShader? shader = null,
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
    /// <param name="shader">Which shader to use when rendering</param>
    /// <param name="rotationDegrees">How much to rotate the texture in degrees</param>
    /// <param name="drawImmediate">If true, the texture will be drawn immediately. If false, it will be queued for drawing.</param>
    public static void DrawGameTexture(
        GameTexture tex,
        float dx,
        float dy,
        float sx,
        float sy,
        float w,
        float h,
        GameRenderTexture? renderTarget = null,
        GameBlendModes blendMode = GameBlendModes.None,
        GameShader? shader = null,
        float rotationDegrees = 0.0f,
        bool drawImmediate = false
    )
    {
        if (tex == null)
        {
            return;
        }

        Renderer?.DrawTexture(
            tex, sx, sy, w, h, dx, dy, w, h, Color.White, renderTarget, blendMode, shader, rotationDegrees, false,
            drawImmediate
        );
    }

    public static void DrawGameTexture(
        GameTexture tex,
        FloatRect srcRectangle,
        FloatRect targetRect,
        Color renderColor,
        GameRenderTexture? renderTarget = null,
        GameBlendModes blendMode = GameBlendModes.None,
        GameShader? shader = null,
        float rotationDegrees = 0.0f,
        bool drawImmediate = false
    )
    {
        if (tex == null)
        {
            return;
        }

        Renderer?.DrawTexture(
            tex, srcRectangle.X, srcRectangle.Y, srcRectangle.Width, srcRectangle.Height, targetRect.X,
            targetRect.Y, targetRect.Width, targetRect.Height,
            Color.FromArgb(renderColor.A, renderColor.R, renderColor.G, renderColor.B), renderTarget, blendMode,
            shader, rotationDegrees, false, drawImmediate
        );
    }
}
