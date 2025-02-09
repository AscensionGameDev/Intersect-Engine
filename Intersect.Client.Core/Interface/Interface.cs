using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Intersect.Client.Core;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Gwen.Skin;
using Intersect.Client.General;
using Intersect.Client.Interface.Game;
using Intersect.Client.Interface.Menu;
using Intersect.Client.Interface.Shared;
using Intersect.Configuration;
using Base = Intersect.Client.Framework.Gwen.Renderer.Base;

namespace Intersect.Client.Interface;


public static partial class Interface
{

    private static readonly ConcurrentQueue<Alert> _pendingErrorMessages = new();

    public static void ShowError(string message, string? header = default) => _pendingErrorMessages.Enqueue(
        new Alert(Message: message, Title: header ?? string.Empty, Type: AlertType.Error)
    );

    //GWEN GUI
    public static bool GwenInitialized;

    public static InputBase GwenInput;

    public static Base GwenRenderer;

    public static bool HideUi;

    private static Canvas sGameCanvas;

    private static Canvas sMenuCanvas;

    public static bool SetupHandlers { get; set; }

    private static Queue<Action> _onCreatedGameUi = [];

    public static GameInterface GameUi { get; private set; }

    public static MenuGuiBase MenuUi { get; private set; }

    public static MutableInterface CurrentInterface => GameUi as MutableInterface ?? MenuUi?.MainMenu;

    public static TexturedBase Skin { get; set; }

    //Input Handling
    public static readonly List<Framework.Gwen.Control.Base> FocusComponents = [];

    public static readonly List<Framework.Gwen.Control.Base> InputBlockingComponents = [];

    #region "Gwen Setup and Input"

    //Gwen Low Level Functions
    public static void InitGwen()
    {
        // Preserve the debug window
        MutableInterface.DetachDebugWindow();

        //TODO: Make it easier to modify skin.
        if (Skin == null)
        {
            Skin = TexturedBase.FindSkin(GwenRenderer, Globals.ContentManager, ClientConfiguration.Instance.UiSkin);
            Skin.DefaultFont = Graphics.UIFont;
        }

        MenuUi?.Dispose();

        GameUi?.Dispose();

        // Create a Canvas (it's root, on which all other GWEN controls are created)
        sMenuCanvas = new Canvas(Skin, "MainMenu")
        {
            Scale = 1f //(GameGraphics.Renderer.GetScreenWidth()/1920f);
        };

        sMenuCanvas.SetSize(
            (int) (Graphics.Renderer.GetScreenWidth() / sMenuCanvas.Scale),
            (int) (Graphics.Renderer.GetScreenHeight() / sMenuCanvas.Scale)
        );

        sMenuCanvas.ShouldDrawBackground = false;
        sMenuCanvas.BackgroundColor = Color.FromArgb(255, 150, 170, 170);
        sMenuCanvas.KeyboardInputEnabled = true;

        // Create the game Canvas (it's root, on which all other GWEN controls are created)
        sGameCanvas = new Canvas(Skin, "InGame");

        //_gameCanvas.Scale = (GameGraphics.Renderer.GetScreenWidth() / 1920f);
        sGameCanvas.SetSize(
            (int) (Graphics.Renderer.GetScreenWidth() / sGameCanvas.Scale),
            (int) (Graphics.Renderer.GetScreenHeight() / sGameCanvas.Scale)
        );

        sGameCanvas.ShouldDrawBackground = false;
        sGameCanvas.BackgroundColor = Color.FromArgb(255, 150, 170, 170);
        sGameCanvas.KeyboardInputEnabled = true;

        // Create GWEN input processor
        if (Globals.GameState == GameStates.Intro || Globals.GameState == GameStates.Menu)
        {
            GwenInput.Initialize(sMenuCanvas);
        }
        else
        {
            GwenInput.Initialize(sGameCanvas);
        }

        FocusComponents.Clear();
        InputBlockingComponents.Clear();

        if (Globals.GameState == GameStates.Intro || Globals.GameState == GameStates.Menu)
        {
            MenuUi = new MenuGuiBase(sMenuCanvas);
            GameUi = null;
        }
        else
        {
            GameUi = new GameInterface(sGameCanvas);
            MenuUi = null;
        }

        Globals.OnLifecycleChangeState();

        GwenInitialized = true;

        while (GameUi is not null && _onCreatedGameUi.TryDequeue(out var action))
        {
            action();
        }
    }

    public static void DestroyGwen(bool exiting = false)
    {
        // Preserve the debug window
        MutableInterface.DetachDebugWindow();

        //The canvases dispose of all of their children.
        sMenuCanvas?.Dispose();
        sGameCanvas?.Dispose();
        GameUi?.Dispose();

        // Destroy our target UI as well! Above code does NOT appear to clear this properly.
        if (Globals.Me != null)
        {
            Globals.Me.ClearTarget();
            Globals.Me.TargetBox?.Dispose();
            Globals.Me.TargetBox = null;
        }

        GwenInitialized = false;

        if (exiting)
        {
            MutableInterface.DisposeDebugWindow();
        }
    }

    public static bool HasInputFocus() =>
        FocusComponents.Any(component => component is { MouseInputEnabled: true, HasFocus: true }) ||
        InputBlockingComponents.Any(component => component is { IsVisible: true, IsBlockingInput: true });

    #endregion

    #region "GUI Functions"

    //Actual Drawing Function
    public static void DrawGui()
    {
        if (!GwenInitialized)
        {
            InitGwen();
        }

        if (Globals.GameState == GameStates.Menu)
        {
            MenuUi.Update();
        }
        else if (Globals.GameState == GameStates.InGame)
        {
            GameUi.Update();
        }

        //Do not allow hiding of UI under several conditions
        var forceShowUi = Globals.InCraft || Globals.InBank || Globals.InShop || Globals.InTrade || Globals.InBag || Globals.EventDialogs?.Count > 0 || HasInputFocus() || (!Interface.GameUi?.EscapeMenu?.IsHidden ?? true);

        AlertWindow.OpenPendingAlertWindowsFrom(_pendingErrorMessages);

        sGameCanvas.RestrictToParent = false;
        if (Globals.GameState == GameStates.Menu)
        {
            MenuUi.Draw();
        }
        else if (Globals.GameState == GameStates.InGame)
        {
            if (HideUi && !forceShowUi)
            {
                if (sGameCanvas.IsVisible)
                {
                    sGameCanvas.Hide();
                }
            }
            else
            {
                if (!sGameCanvas.IsVisible)
                {
                    sGameCanvas.Show();
                }
                GameUi.Draw();
            }
        }
    }

    public static void SetHandleInput(bool val) => GwenInput.HandleInput = val;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool DoesMouseHitInterface() => DoesMouseHitComponentOrChildren(sGameCanvas);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool DoesMouseHitComponentOrChildren(Framework.Gwen.Control.Base? component) =>
        DoesComponentOrChildrenContainMousePoint(component, InputHandler.MousePosition);

    public static bool DoesComponentOrChildrenContainMousePoint(Framework.Gwen.Control.Base? component, Point position)
    {
        if (component == default)
        {
            return false;
        }

        if (component.IsHidden)
        {
            return false;
        }

        if (!component.Bounds.Contains(position))
        {
            return false;
        }

        if (component.MouseInputEnabled)
        {
            return true;
        }

        var localPosition = position;
        localPosition.X -= component.X;
        localPosition.Y -= component.Y;

        return component.Children.Any(child => DoesComponentOrChildrenContainMousePoint(child, localPosition));
    }

    #endregion

    public static void EnqueueInGame(Action action)
    {
        if (GameUi != null)
        {
            action();
            return;
        }

        _onCreatedGameUi.Enqueue(action);
    }

    public static Framework.Gwen.Control.Base? FindComponentUnderCursor(ComponentStateFilters filters = default)
    {
        var cursor = new Point(InputHandler.MousePosition.X, InputHandler.MousePosition.Y);
        var componentUnderCursor = CurrentInterface?.Root.GetComponentAt(cursor, filters);
        return componentUnderCursor;
    }
}
