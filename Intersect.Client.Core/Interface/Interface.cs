using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
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
    private static StatisticsPanel? _statisticsPanel;
    private static readonly ConcurrentQueue<Alert> PendingErrorMessages = new();

    private static bool _initialized;

    public static InputBase GwenInput { get; set; } = default!;

    public static Base GwenRenderer { get; set; } = default!;

    public static bool HideUi { get; set; }

    private static Canvas? _canvasInGame;

    private static Canvas? _canvasMainMenu;

    private static readonly Queue<Action> PendingActionsForInGameInterface = [];

    //Input Handling
    public static readonly HashSet<Framework.Gwen.Control.Base> FocusComponents = [];

    public static readonly HashSet<Framework.Gwen.Control.Base> InputBlockingComponents = [];
    private static GameInterface? _uiInGame;
    private static MenuGuiBase? _uiMainMenu;
    private static TexturedBase? _skin;

    public static bool SetupHandlers { get; set; }

    public static bool HasInGameUI => _uiInGame != null;

    public static bool HasMainMenuUI => _uiMainMenu != null;

    public static GameInterface GameUi
    {
        get
        {
            if (_uiInGame == null)
            {
                throw new InvalidOperationException("In-game UI not initialized");
            }

            return _uiInGame;
        }
        private set => _uiInGame = value;
    }

    public static MenuGuiBase MenuUi
    {
        get
        {
            if (_uiMainMenu == null)
            {
                throw new InvalidOperationException("Menu UI not initialized");
            }

            return _uiMainMenu;
        }
        private set => _uiMainMenu = value;
    }

    private static MutableInterface? NullableCurrentInterface => _uiInGame as MutableInterface ?? _uiMainMenu?.MainMenu;

    public static MutableInterface CurrentInterface => NullableCurrentInterface ??
                                                       throw new InvalidOperationException("No current UI initialized");

    private static bool _showStatisticsPanelFPS;
    private static bool _showStatisticsPanelPing;

    public static bool ShowStatisticsPanelFPS
    {
        get => _showStatisticsPanelFPS;
        set
        {
            if (_showStatisticsPanelFPS == value)
            {
                return;
            }

            _showStatisticsPanelFPS = value;
            if (_statisticsPanel is { } panel)
            {
                panel.IsFPSEnabled = value;
            }
        }
    }

    public static bool ShowStatisticsPanelPing
    {
        get => _showStatisticsPanelPing;
        set
        {
            if (_showStatisticsPanelPing == value)
            {
                return;
            }

            _showStatisticsPanelPing = value;
            if (_statisticsPanel is { } panel)
            {
                panel.IsPingEnabled = value;
            }
        }
    }

    private static bool HasCurrentInterface => NullableCurrentInterface is not null;

    public static TexturedBase Skin
    {
        get => _skin ?? throw new InvalidOperationException("No skin");
        set => _skin = value;
    }

    public static void ShowAlert(string message, string? title = default, AlertType alertType = AlertType.Error)
    {
        PendingErrorMessages.Enqueue(new Alert(message, title ?? string.Empty, alertType));
    }

    public static void EnqueueInGame(Action action)
    {
        if (_uiInGame != null)
        {
            action();
            return;
        }

        PendingActionsForInGameInterface.Enqueue(action);
    }

    public static void EnqueueInGame(Action<GameInterface> action)
    {
        if (_uiInGame is {} uiInGame)
        {
            action(uiInGame);
            return;
        }

        PendingActionsForInGameInterface.Enqueue(() => action(GameUi));
    }

    public static void EnqueueInGame<TArg0, TArg1>(Action<GameInterface> action, Action<TArg0, TArg1> onDeferred, TArg0 arg0, TArg1 arg1)
    {
        if (_uiInGame is {} uiInGame)
        {
            action(uiInGame);
            return;
        }

        PendingActionsForInGameInterface.Enqueue(() => action(GameUi));
        onDeferred(arg0, arg1);
    }

    public static Framework.Gwen.Control.Base? FindComponentUnderCursor(NodeFilter filters = default)
    {
        var cursor = new Point(InputHandler.MousePosition.X, InputHandler.MousePosition.Y);
        var componentUnderCursor = NullableCurrentInterface?.Root.GetComponentAt(cursor, filters);
        return componentUnderCursor;
    }

    #region "Gwen Setup and Input"

    //Gwen Low Level Functions
    public static void InitGwen()
    {
        // Preserve the debug window
        MutableInterface.DetachDebugWindow();

        //TODO: Make it easier to modify skin.
        if (_skin == null)
        {
            _skin = TexturedBase.FindSkin(
                GwenRenderer,
                GameContentManager.Current,
                ClientConfiguration.Instance.UiSkin
            );
            _skin.DefaultFont = Graphics.UIFont ?? _skin.DefaultFont;
            if (Graphics.UIFontSize != default)
            {
                _skin.DefaultFontSize = Graphics.UIFontSize;
            }
        }

        _uiMainMenu?.Dispose();
        _uiInGame?.Dispose();

        // Create a Canvas (it's root, on which all other GWEN controls are created)
        _canvasMainMenu = new Canvas(Skin, "MainMenu")
        {
            Scale = 1f, //(GameGraphics.Renderer.GetScreenWidth()/1920f);
        };

        _canvasMainMenu.SetSize(
            (int)(Graphics.Renderer.ScreenWidth / _canvasMainMenu.Scale),
            (int)(Graphics.Renderer.ScreenHeight / _canvasMainMenu.Scale)
        );

        _canvasMainMenu.ShouldDrawBackground = false;
        _canvasMainMenu.BackgroundColor = Color.FromArgb(255, 150, 170, 170);
        _canvasMainMenu.KeyboardInputEnabled = true;

        // Create the game Canvas (it's root, on which all other GWEN controls are created)
        _canvasInGame = new Canvas(Skin, "InGame");

        //_gameCanvas.Scale = (GameGraphics.Renderer.GetScreenWidth() / 1920f);
        _canvasInGame.SetSize(
            (int)(Graphics.Renderer.ScreenWidth / _canvasInGame.Scale),
            (int)(Graphics.Renderer.ScreenHeight / _canvasInGame.Scale)
        );

        _canvasInGame.ShouldDrawBackground = false;
        _canvasInGame.BackgroundColor = Color.FromArgb(255, 150, 170, 170);
        _canvasInGame.KeyboardInputEnabled = true;

        // Create GWEN input processor
        if (Globals.GameState == GameStates.Intro || Globals.GameState == GameStates.Menu)
        {
            GwenInput.Initialize(_canvasMainMenu);
            MutableInterface.ReparentDebugWindow(_canvasMainMenu);
        }
        else
        {
            GwenInput.Initialize(_canvasInGame);
            MutableInterface.ReparentDebugWindow(_canvasInGame);
        }

        FocusComponents.Clear();
        InputBlockingComponents.Clear();

        if (Globals.GameState == GameStates.Intro || Globals.GameState == GameStates.Menu)
        {
            MenuUi = new MenuGuiBase(_canvasMainMenu);
            _uiInGame = null;
        }
        else
        {
            GameUi = new GameInterface(_canvasInGame);
            _uiMainMenu = null;
        }

        _showStatisticsPanelFPS = Globals.Database?.ShowFPSCounter ?? false;
        _showStatisticsPanelPing = Globals.Database?.ShowPingCounter ?? false;
        _statisticsPanel = new StatisticsPanel(CurrentInterface.Root)
        {
            IsFPSEnabled = _showStatisticsPanelFPS,
            IsPingEnabled = _showStatisticsPanelPing,
        };

        Globals.EmitLifecycleChangedState();

        _initialized = true;

        while (_uiInGame is not null && PendingActionsForInGameInterface.TryDequeue(out var action))
        {
            action();
        }
    }

    public static void DestroyGwen(bool exiting = false)
    {
        if (!exiting)
        {
            // Preserve the debug window if not exiting
            MutableInterface.DetachDebugWindow();
        }

        //The canvases dispose of all of their children.
        if (_uiMainMenu is { } menuUi)
        {
            menuUi.Dispose();
        }
        else
        {
            _canvasMainMenu?.Dispose();
        }

        _canvasMainMenu = null;
        _uiMainMenu = null;

        // Destroy our target UI as well! Above code does NOT appear to clear this properly.
        if (Globals.Me != null)
        {
            Globals.Me.ClearTarget();
            Globals.Me.TargetBox?.Dispose();
            Globals.Me.TargetBox = null;
        }

        if (_uiInGame is { } gameUi)
        {
            gameUi.Dispose();
        }
        else
        {
            _canvasInGame?.Dispose();
        }

        _canvasInGame = null;
        _uiInGame = null;

        _initialized = false;
    }

    public static bool HasInputFocus()
    {
        return FocusComponents.Any(component => component is { MouseInputEnabled: true, HasFocus: true }) ||
               InputBlockingComponents.Any(component => component is { IsVisibleInTree: true, IsBlockingInput: true });
    }

    #endregion

    #region "GUI Functions"

    //Actual Drawing Function
    public static void DrawGui(TimeSpan elapsed, TimeSpan total)
    {
        if (!_initialized)
        {
            InitGwen();
        }

        if (Globals.GameState == GameStates.Menu)
        {
            MenuUi.Update(elapsed, total);
        }
        else if (Globals.GameState == GameStates.InGame)
        {
            GameUi.Update(elapsed, total);
        }

        //Do not allow hiding of UI under several conditions
        var forceShowUi = Globals.InCraft ||
                          Globals.InBank ||
                          Globals.InShop ||
                          Globals.InTrade ||
                          Globals.InBag ||
                          Globals.EventDialogs.Count > 0 ||
                          HasInputFocus() ||
                          _uiInGame is { EscapeMenu.IsVisibleInTree: true };

        AlertWindow.OpenPendingAlertWindowsFrom(PendingErrorMessages);

        if (_canvasInGame is { } canvasInGame)
        {
            canvasInGame.RestrictToParent = false;
        }

        if (Globals.GameState == GameStates.Menu)
        {
            MenuUi.Draw(elapsed, total);
        }
        else if (Globals.GameState == GameStates.InGame)
        {
            if (HideUi && !forceShowUi)
            {
                if (_canvasInGame is { IsVisibleInTree: true })
                {
                    _canvasInGame.Hide();
                }
            }
            else
            {
                if (_canvasInGame is { IsVisibleInTree: false })
                {
                    _canvasInGame.Show();
                }

                GameUi.Draw(elapsed, total);
            }
        }
    }

    public static void SetHandleInput(bool val)
    {
        GwenInput.HandleInput = val;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInBounds(int x, int y)
    {
        return HasCurrentInterface && CurrentInterface.Root.Bounds.Contains(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInBounds(Point point)
    {
        return HasCurrentInterface && CurrentInterface.Root.Bounds.Contains(point);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool DoesMouseHitInterface()
    {
        return DoesMouseHitComponentOrChildren(_canvasInGame);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // ReSharper disable once MemberCanBePrivate.Global
    public static bool DoesMouseHitComponentOrChildren(Framework.Gwen.Control.Base? component)
    {
        return DoesComponentOrChildrenContainMousePoint(component, InputHandler.MousePosition);
    }

    // ReSharper disable once MemberCanBePrivate.Global
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
}