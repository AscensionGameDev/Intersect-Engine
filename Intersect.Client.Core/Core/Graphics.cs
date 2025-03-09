using System.Numerics;
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
using Intersect.Framework;
using Intersect.Framework.Core;
using Intersect.Framework.Core.GameObjects.Lighting;
using Intersect.Framework.Core.GameObjects.Maps;
using Intersect.GameObjects;
using Intersect.Utilities;

namespace Intersect.Client.Core;

public static partial class Graphics
{

    public static IFont? ActionMsgFont { get; set; }
    public static int ActionMsgFontSize { get; set; }

    public static object AnimationLock = new();

    //Darkness Stuff
    public static float BrightnessLevel { get; set; }

    public static IFont? ChatBubbleFont { get; set; }
    public static int ChatBubbleFontSize { get; set; }

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
    private static IGameTexture? sMenuBackground;

    public static int DrawCalls;

    public static int EntitiesDrawn;

    public static IFont? EntityNameFont { get; set; }
    public static int EntityNameFontSize { get; set; }

    //Screen Values
    public static IFont? GameFont { get; set; }
    public static int GameFontSize { get; set; }

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
    public static GameRenderer Renderer { get; set; }

    //Cache the Y based rendering
    public static HashSet<Entity>[,]? RenderingEntities;

    private static GameContentManager sContentManager = null!;

    private static IGameRenderTexture? sDarknessTexture;

    private static readonly List<LightDescriptor> sLightQueue = [];

    //Player Spotlight Values
    private static long sLightUpdate;

    private static int sOldHeight;

    //Resolution
    private static int sOldWidth;

    private static long sOverlayUpdate;

    private static float sPlayerLightExpand;

    private static float sPlayerLightIntensity = 255;

    private static float sPlayerLightSize;

    public static IFont? UIFont { get; set; }
    public static int UIFontSize { get; set; }

    public static float MinimumWorldScale => Options.Instance?.Map?.MinimumWorldScale ?? 1;

    public static float MaximumWorldScale => Options.Instance?.Map?.MaximumWorldScale ?? 1;

    //Init Functions
    public static void InitGraphics()
    {
        Renderer?.Init();
        sContentManager = Globals.ContentManager;
        sContentManager.LoadAll();
        (GameFont, GameFontSize) = FindFont(ClientConfiguration.Instance.GameFont);
        (UIFont, UIFontSize) = FindFont(ClientConfiguration.Instance.UIFont);
        (EntityNameFont, EntityNameFontSize) = FindFont(ClientConfiguration.Instance.EntityNameFont);
        (ChatBubbleFont, ChatBubbleFontSize) = FindFont(ClientConfiguration.Instance.ChatBubbleFont);
        (ActionMsgFont, ActionMsgFontSize) = FindFont(ClientConfiguration.Instance.ActionMsgFont);
    }

    private static (IFont?, int) FindFont(string font)
    {
        var size = 8;

        // ReSharper disable once InvertIf
        if (font.IndexOf(',') > 0)
        {
            var parts = font.Split(',');
            font = parts[0];
            _ = int.TryParse(parts[1], out size);
        }

        return (sContentManager.GetFont(font), size);
    }

    public static void InitInGame()
    {
        RenderingEntities = new HashSet<Entity>[6, Options.Instance.Map.MapHeight * 5];
        for (var z = 0; z < 6; z++)
        {
            for (var i = 0; i < Options.Instance.Map.MapHeight * 5; i++)
            {
                RenderingEntities[z, i] = [];
            }
        }
    }

    public static void DrawIntro()
    {
        var introImages = ClientConfiguration.Instance.IntroImages;
        if (introImages.Count <= Globals.IntroIndex)
        {
            return;
        }

        if (!sContentManager.TryGetTexture(TextureType.Image, introImages[Globals.IntroIndex], out var texture))
        {
            return;
        }

        DrawFullScreenTextureFitMinimum(texture);
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

        if (Globals.NeedsMaps || Globals.MapGrid == null || RenderingEntities == null)
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
        var mapGridWidth = Globals.MapGridWidth;
        var mapGridHeight = Globals.MapGridHeight;

        for (var x = Math.Max(gridX - 1, 0); x <= Math.Min(gridX + 1, mapGridWidth - 1); x++)
        {
            for (var y = Math.Max(gridY - 1, 0); y <= Math.Min(gridY + 1, mapGridHeight - 1); y++)
            {
                var mapId = Globals.MapGrid[x, y];
                if (mapId == Guid.Empty)
                {
                    continue;
                }

                DrawMapPanorama(Globals.MapGrid[x, y]);
            }
        }

        for (var x = Math.Max(gridX - 1, 0); x <= Math.Min(gridX + 1, mapGridWidth - 1); x++)
        {
            for (var y = Math.Max(gridY - 1, 0); y <= Math.Min(gridY + 1, mapGridHeight - 1); y++)
            {
                var mapId = Globals.MapGrid[x, y];
                if (mapId == Guid.Empty)
                {
                    continue;
                }

                DrawMap(mapId, 0);
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

        var mapHeight = Options.Instance.Map.MapHeight;
        for (var y = 0; y < mapHeight * 5; y++)
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

                if (x == 0 && y > 0 && y % mapHeight == 0)
                {
                    for (var x1 = gridX - 1; x1 <= gridX + 1; x1++)
                    {
                        var y1 = gridY - 2 + (int)Math.Floor(y / (float)mapHeight);
                        if (x1 >= 0 &&
                            x1 < mapGridWidth &&
                            y1 >= 0 &&
                            y1 < mapGridHeight &&
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

        for (var x = Math.Max(gridX - 1, 0); x <= Math.Min(gridX + 1, mapGridWidth - 1); x++)
        {
            for (var y = Math.Max(gridY - 1, 0); y <= Math.Min(gridY + 1, mapGridHeight - 1); y++)
            {
                var mapId = Globals.MapGrid[x, y];
                if (mapId == Guid.Empty)
                {
                    continue;
                }

                DrawMap(Globals.MapGrid[x, y], 1);
            }
        }

        for (var y = 0; y < mapHeight * 5; y++)
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

        for (var x = Math.Max(gridX - 1, 0); x <= Math.Min(gridX + 1, mapGridWidth - 1); x++)
        {
            for (var y = Math.Max(gridY - 1, 0); y <= Math.Min(gridY + 1, mapGridHeight - 1); y++)
            {
                var mapId = Globals.MapGrid[x, y];
                if (mapId == Guid.Empty)
                {
                    continue;
                }

                DrawMap(mapId, 2);
            }
        }
        // Handle our plugin drawing.
        Globals.OnGameDraw(DrawStates.FringeLayers, deltaTime);

        foreach (var animInstance in animations)
        {
            animInstance.Draw(true);
        }

        for (var x = Math.Max(gridX - 1, 0); x <= Math.Min(gridX + 1, mapGridWidth - 1); x++)
        {
            for (var y = Math.Max(gridY - 1, 0); y <= Math.Min(gridY + 1, mapGridHeight - 1); y++)
            {
                var mapId = Globals.MapGrid[x, y];
                if (mapId == Guid.Empty)
                {
                    continue;
                }

                if (!MapInstance.TryGet(mapId, out var map))
                {
                    continue;
                }

                map.DrawWeather();
                map.DrawFog();
                map.DrawOverlayGraphic();
                map.DrawItemNames();
            }
        }

        //Draw the players targets
        Globals.Me?.DrawTargets();

        DrawOverlay();

        // Draw lighting effects.
        GenerateLightMap();
        DrawDarkness();

        for (var y = 0; y < mapHeight * 5; y++)
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

        for (var y = 0; y < mapHeight * 5; y++)
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
        for (var x = Math.Max(gridX - 1, 0); x <= Math.Min(gridX + 1, mapGridWidth - 1); x++)
        {
            for (var y = Math.Max(gridY - 1, 0); y <= Math.Min(gridY + 1, mapGridHeight - 1); y++)
            {
                var mapId = Globals.MapGrid[x, y];
                if (mapId == Guid.Empty)
                {
                    continue;
                }

                if (!MapInstance.TryGet(mapId, out var map))
                {
                    continue;
                }

                map.DrawActionMsgs();
            }
        }

        foreach (var animInstance in animations)
        {
            animInstance.EndDraw();
        }
    }

    //Game Rendering
    public static void Render(TimeSpan deltaTime, TimeSpan totalTime)
    {
        if (Renderer is not { } renderer)
        {
            return;
        }

        var takingScreenshot = false;
        if (renderer is { HasScreenshotRequests: true })
        {
            takingScreenshot = renderer.BeginScreenshot();
        }

        var gameState = Globals.GameState;

        renderer.Scale = gameState == GameStates.InGame ? Globals.Database.WorldZoom : 1.0f;

        if (!renderer.Begin())
        {
            return;
        }

        if (renderer.ScreenWidth != sOldWidth ||
            renderer.ScreenHeight != sOldHeight ||
            renderer.DisplayModeChanged())
        {
            sDarknessTexture = null;
            Interface.Interface.DestroyGwen();
            Interface.Interface.InitGwen();
            sOldWidth = renderer.ScreenWidth;
            sOldHeight = renderer.ScreenHeight;
        }

        renderer.Clear(Color.Black);
        DrawCalls = 0;
        MapsDrawn = 0;
        EntitiesDrawn = 0;
        LightsDrawn = 0;

        UpdateView();

        switch (gameState)
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
                throw Exceptions.UnreachableInvalidEnum(gameState);
        }

        renderer.Scale = Globals.Database.UIScale;

        Interface.Interface.DrawGui(deltaTime, totalTime);

        DrawGameTexture(
            tex: renderer.WhitePixel,
            srcRectangle: new FloatRect(0, 0, 1, 1),
            targetRect: CurrentView,
            renderColor: new Color((int)Fade.Alpha, 0, 0, 0),
            renderTarget: null,
            blendMode: GameBlendModes.None
        );

        // Draw our mousecursor at the very end, but not when taking screenshots.
        if (!takingScreenshot && !string.IsNullOrWhiteSpace(ClientConfiguration.Instance.MouseCursor))
        {
            var cursorTexture = Globals.ContentManager.GetTexture(
                TextureType.Misc,
                ClientConfiguration.Instance.MouseCursor
            );

            if (cursorTexture is not null)
            {
                var cursorPosition = ConvertToWorldPointNoZoom(Globals.InputManager.GetMousePosition());
                DrawGameTexture(
                    cursorTexture,
                    cursorPosition.X,
                    cursorPosition.Y
                );
            }
        }

        renderer.End();

        if (takingScreenshot)
        {
            renderer.EndScreenshot();
        }
    }

    private static void DrawMap(Guid mapId, int layer)
    {
        if (!MapInstance.TryGet(mapId, out var map))
        {
            return;
        }

        if (!new FloatRect(
            map.X, map.Y, Options.Instance.Map.TileWidth * Options.Instance.Map.MapWidth, Options.Instance.Map.TileHeight * Options.Instance.Map.MapHeight
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
        if (!MapInstance.TryGet(mapId, out var map))
        {
            return;
        }

        var mapBounds = new FloatRect(
            map.X,
            map.Y,
            Options.Instance.Map.TileWidth * Options.Instance.Map.MapWidth,
            Options.Instance.Map.TileHeight * Options.Instance.Map.MapHeight
        );

        if (!mapBounds.IntersectsWith(WorldViewport))
        {
            return;
        }

        map.DrawPanorama();
    }

    public static void DrawOverlay()
    {
        if (Renderer == default)
        {
            return;
        }

        if (MapInstance.TryGet(Globals.Me?.MapId ?? default, out var map))
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

        DrawGameTexture(Renderer.WhitePixel, new FloatRect(0, 0, 1, 1), CurrentView, OverlayColor, null);
        sOverlayUpdate = Timing.Global.MillisecondsUtc;
    }

    public static FloatRect GetSourceRect(IGameTexture gameTexture)
    {
        return gameTexture == null
            ? new FloatRect()
            : new FloatRect(0, 0, gameTexture.Width, gameTexture.Height);
    }

    public static void DrawFullScreenTexture(IGameTexture tex, float alpha = 1f)
    {
        if (Renderer == default)
        {
            return;
        }

        var bgx = Renderer.ScreenWidth / 2 - tex.Width / 2;
        var bgy = Renderer.ScreenHeight / 2 - tex.Height / 2;
        var bgw = tex.Width;
        var bgh = tex.Height;
        int diff;

        if (bgw < Renderer.ScreenWidth)
        {
            diff = Renderer.ScreenWidth - bgw;
            bgx -= diff / 2;
            bgw += diff;
        }

        if (bgh < Renderer.ScreenHeight)
        {
            diff = Renderer.ScreenHeight - bgh;
            bgy -= diff / 2;
            bgh += diff;
        }

        DrawGameTexture(
            tex, GetSourceRect(tex),
            new FloatRect(bgx + Renderer.GetView().X, bgy + Renderer.GetView().Y, bgw, bgh),
            new Color((int)(alpha * 255f), 255, 255, 255)
        );
    }

    public static void DrawFullScreenTextureCentered(IGameTexture tex, float alpha = 1f)
    {
        if (Renderer == default)
        {
            return;
        }

        var bgx = Renderer.ScreenWidth / 2 - tex.Width / 2;
        var bgy = Renderer.ScreenHeight / 2 - tex.Height / 2;
        var bgw = tex.Width;
        var bgh = tex.Height;

        DrawGameTexture(
            tex, GetSourceRect(tex),
            new FloatRect(bgx + Renderer.GetView().X, bgy + Renderer.GetView().Y, bgw, bgh),
            new Color((int)(alpha * 255f), 255, 255, 255)
        );
    }

    public static void DrawFullScreenTextureStretched(IGameTexture tex)
    {
        if (Renderer == default)
        {
            return;
        }

        DrawGameTexture(
            tex, GetSourceRect(tex),
            new FloatRect(
                Renderer.GetView().X, Renderer.GetView().Y, Renderer.ScreenWidth, Renderer.ScreenHeight
            ), Color.White
        );
    }

    public static void DrawFullScreenTextureFitWidth(IGameTexture tex)
    {
        if (Renderer == default)
        {
            return;
        }

        var scale = Renderer.ScreenWidth / (float)tex.Width;
        var scaledHeight = tex.Height * scale;
        var offsetY = (Renderer.ScreenHeight - tex.Height) / 2f;
        DrawGameTexture(
            tex, GetSourceRect(tex),
            new FloatRect(
                Renderer.GetView().X, Renderer.GetView().Y + offsetY, Renderer.ScreenWidth, scaledHeight
            ), Color.White
        );
    }

    public static void DrawFullScreenTextureFitHeight(IGameTexture tex)
    {
        if (Renderer == default)
        {
            return;
        }

        var scale = Renderer.ScreenHeight / (float)tex.Height;
        var scaledWidth = tex.Width * scale;
        var offsetX = (Renderer.ScreenWidth - scaledWidth) / 2f;
        DrawGameTexture(
            tex, GetSourceRect(tex),
            new FloatRect(
                Renderer.GetView().X + offsetX, Renderer.GetView().Y, scaledWidth, Renderer.ScreenHeight
            ), Color.White
        );
    }

    public static void DrawFullScreenTextureFitMinimum(IGameTexture tex)
    {
        if (Renderer == default)
        {
            return;
        }

        if (Renderer.ScreenWidth > Renderer.ScreenHeight)
        {
            DrawFullScreenTextureFitHeight(tex);
        }
        else
        {
            DrawFullScreenTextureFitWidth(tex);
        }
    }

    public static void DrawFullScreenTextureFitMaximum(IGameTexture tex)
    {
        if (Renderer == default)
        {
            return;
        }

        if (Renderer.ScreenWidth < Renderer.ScreenHeight)
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
            var sw = Renderer.ScreenWidth;
            var sh = Renderer.ScreenHeight;
            var sx = 0;
            var sy = 0;
            CurrentView = new FloatRect(sx, sy, sw / scale, sh / scale);
            return;
        }

        var mapWidth = Options.Instance.Map.MapWidth * Options.Instance.Map.TileWidth;
        var mapHeight = Options.Instance.Map.MapHeight * Options.Instance.Map.TileHeight;

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

        x = map.X - x;
        y = map.Y - y;
        x1 = map.X + x1;
        y1 = map.Y + y1;

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
        else if (Options.Instance.Map.GameBorderStyle == GameBorderStyle.Seamed)
        {
            newView.X = restrictView.X - (newView.Width - restrictView.Width) / 2;
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
        else if (Options.Instance.Map.GameBorderStyle == GameBorderStyle.Seamed)
        {
            newView.Y = restrictView.Y - (newView.Height - restrictView.Height) / 2;
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

        sDarknessTexture ??= Renderer.CreateRenderTexture(Renderer.ScreenWidth, Renderer.ScreenHeight);
        sDarknessTexture.Clear(Color.Black);
    }

    private static void GenerateLightMap()
    {
        if (Renderer == default)
        {
            return;
        }

        if (sDarknessTexture == default)
        {
            return;
        }

        var mapId = Globals.Me?.MapId ?? default;
        if (mapId == default)
        {
            return;
        }

        if (!Globals.Database.EnableLighting)
        {
            return;
        }

        if (!MapInstance.TryGet(mapId, out var map))
        {
            return;
        }

        var destRect = new FloatRect(new Vector2(), sDarknessTexture.Dimensions / Globals.Database.WorldZoom);
        if (map.IsIndoors)
        {
            DrawGameTexture(
                Renderer.WhitePixel, new FloatRect(0, 0, 1, 1),
                destRect,
                new Color((byte)BrightnessLevel, 255, 255, 255), sDarknessTexture, GameBlendModes.Add
            );
        }
        else
        {
            DrawGameTexture(
                Renderer.WhitePixel, new FloatRect(0, 0, 1, 1),
                destRect,
                new Color(255, 255, 255, 255), sDarknessTexture, GameBlendModes.Add
            );

            DrawGameTexture(
                Renderer.WhitePixel, new FloatRect(0, 0, 1, 1),
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

        sLightQueue.Add(new LightDescriptor(0, 0, x, y, intensity, size, expand, color));
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
                        Renderer.WhitePixel, new FloatRect(0, 0, 1, 1),
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
        if (MapInstance.TryGet(Globals.Me.MapId, out var map))
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
    public static Vector2 ConvertToWorldPoint(Vector2 windowPoint)
    {
        return new Vector2(
            (int)Math.Floor(windowPoint.X / Globals.Database.WorldZoom + CurrentView.Left),
            (int)Math.Floor(windowPoint.Y / Globals.Database.WorldZoom + CurrentView.Top)
        );
    }

    /// <summary>
    /// Converts a point in the window to its place in the world view without respecting zoom
    /// </summary>
    /// <param name="windowPoint">The point to convert.</param>
    /// <returns>The converted point.</returns>
    public static Vector2 ConvertToWorldPointNoZoom(Vector2 windowPoint)
    {
        return new Vector2((int)Math.Floor(windowPoint.X + CurrentView.Left), (int)Math.Floor(windowPoint.Y + CurrentView.Top));
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
        IGameTexture tex,
        float x,
        float y,
        IGameRenderTexture? renderTarget = null,
        GameBlendModes blendMode = GameBlendModes.None,
        GameShader? shader = null,
        float rotationDegrees = 0.0f,
        bool drawImmediate = false
    )
    {
        var destRectangle = new FloatRect(x, y, tex.Width, tex.Height);
        var srcRectangle = new FloatRect(0, 0, tex.Width, tex.Height);

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
        IGameTexture tex,
        float x,
        float y,
        Color renderColor,
        IGameRenderTexture? renderTarget = null,
        GameBlendModes blendMode = GameBlendModes.None,
        GameShader? shader = null,
        float rotationDegrees = 0.0f,
        bool drawImmediate = false
    )
    {
        var destRectangle = new FloatRect(x, y, tex.Width, tex.Height);
        var srcRectangle = new FloatRect(0, 0, tex.Width, tex.Height);
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
        IGameTexture tex,
        float dx,
        float dy,
        float sx,
        float sy,
        float w,
        float h,
        IGameRenderTexture? renderTarget = null,
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
        IGameTexture tex,
        FloatRect srcRectangle,
        FloatRect targetRect,
        Color renderColor,
        IGameRenderTexture? renderTarget = null,
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
