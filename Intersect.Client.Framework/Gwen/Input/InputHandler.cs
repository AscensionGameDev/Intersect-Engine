using System.Collections.Immutable;
using System.Text;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.DragDrop;
using Intersect.Client.Framework.Input;
using Intersect.Core;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Framework.Gwen.Input;

/// <summary>
///     Input handling.
/// </summary>
public static partial class InputHandler
{

    private static readonly KeyData KeyData = new KeyData();

    private static readonly Dictionary<MouseButton, float> LastClickTime = [];

    private static Base? _focusedKeyboard;
    private static Base? _focusedMouse;

    public static event Action<Base?, FocusSource>? FocusChanged;

    /// <summary>
    ///     Control currently hovered by mouse.
    /// </summary>
    public static Base? HoveredControl { get; set; }

    /// <summary>
    ///     Control that corrently has keyboard focus.
    /// </summary>
    public static Base? KeyboardFocus
    {
        get => _focusedKeyboard;
        set => _focusedKeyboard = value;
    }

    /// <summary>
    ///     Control that currently has mouse focus.
    /// </summary>
    public static Base? MouseFocus
    {
        get => _focusedMouse;
        set => _focusedMouse = value;
    }

    public static void Focus(FocusSource focusSource, Base? control)
    {
        if (focusSource == FocusSource.Keyboard)
        {
            KeyboardFocus = control;
        }
        else
        {
            MouseFocus = control;
        }

        FocusChanged?.Invoke(control, focusSource);
    }

    /// <summary>
    ///     Current mouse position.
    /// </summary>
    public static Point MousePosition; // not property to allow modification of Point fields

    private static Point _lastClickPosition;

    public static readonly ImmutableArray<MouseButton> MouseButtons = Enum.GetValues<MouseButton>()
        .Where(
            mouseButton =>
            {
                switch (mouseButton)
                {
                    case MouseButton.Left:
                    case MouseButton.Right:
                    case MouseButton.Middle:
                    case MouseButton.X1:
                    case MouseButton.X2:
                        return true;

                    case MouseButton.None:
                    default:
                        return false;
                }
            }
        )
        .ToImmutableArray();

    /// <summary>
    ///     Maximum number of mouse buttons supported.
    /// </summary>
    public static int MaxMouseButtons => MouseButtons.Length;

    /// <summary>
    ///     Maximum time in seconds between mouse clicks to be recognized as double click.
    /// </summary>
    public static float DoubleClickSpeed => 0.5f;

    /// <summary>
    ///     Time in seconds between autorepeating of keys.
    /// </summary>
    public static float KeyRepeatRate => 0.03f;

    /// <summary>
    ///     Time in seconds before key starts to autorepeat.
    /// </summary>
    public static float KeyRepeatDelay => 0.5f;

    public static bool IsMouseButtonDown(MouseButton mouseButton) => KeyData.IsMouseButtonDown(mouseButton);

    /// <summary>
    ///     Indicates whether the left mouse button is down.
    /// </summary>
    public static bool IsLeftMouseDown => KeyData.IsMouseButtonDown(MouseButton.Left);

    /// <summary>
    ///     Indicates whether the right mouse button is down.
    /// </summary>
    public static bool IsRightMouseDown => KeyData.IsMouseButtonDown(MouseButton.Right);

    /// <summary>
    ///     Indicates whether the shift key is down.
    /// </summary>
    public static bool IsShiftDown => IsKeyDown(Key.Shift);

    /// <summary>
    ///     Indicates whether the control key is down.
    /// </summary>
    public static bool IsControlDown => IsKeyDown(Key.Control);

    /// <summary>
    ///     Checks if the given key is pressed.
    /// </summary>
    /// <param name="key">Key to check.</param>
    /// <returns>True if the key is down.</returns>
    public static bool IsKeyDown(Key key)
    {
        return KeyData.KeyState[(int) key];
    }

    /// <summary>
    ///     Handles copy, paste etc.
    /// </summary>
    /// <param name="canvas">Canvas.</param>
    /// <param name="chr">Input character.</param>
    /// <returns>True if the key was handled.</returns>
    public static bool DoSpecialKeys(Base canvas, char chr)
    {
        if (null == KeyboardFocus)
        {
            return false;
        }

        if (KeyboardFocus.GetCanvas() != canvas)
        {
            return false;
        }

        if (!KeyboardFocus.IsVisible)
        {
            return false;
        }

        if (!IsControlDown)
        {
            return false;
        }

        if (chr == 'C' || chr == 'c')
        {
            KeyboardFocus.InputCopy(null);

            return true;
        }

        if (chr == 'V' || chr == 'v')
        {
            KeyboardFocus.InputPaste(null);

            return true;
        }

        if (chr == 'X' || chr == 'x')
        {
            KeyboardFocus.InputCut(null);

            return true;
        }

        if (chr == 'A' || chr == 'a')
        {
            KeyboardFocus.InputSelectAll(null);

            return true;
        }

        return false;
    }

    /// <summary>
    ///     Handles accelerator input.
    /// </summary>
    /// <param name="canvas">Canvas.</param>
    /// <param name="chr">Input character.</param>
    /// <returns>True if the key was handled.</returns>
    public static bool HandleAccelerator(Base canvas, char chr)
    {
        //Build the accelerator search string
        var accelString = new StringBuilder();
        if (IsControlDown)
        {
            accelString.Append("CTRL+");
        }

        if (IsShiftDown)
        {
            accelString.Append("SHIFT+");
        }

        // [omeg] todo: alt?

        accelString.Append(chr);
        var acc = accelString.ToString();

        //Debug::Msg("Accelerator string :%S\n", accelString.c_str());)

        if (KeyboardFocus != null && KeyboardFocus.HandleAccelerator(acc))
        {
            return true;
        }

        if (MouseFocus != null && MouseFocus.HandleAccelerator(acc))
        {
            return true;
        }

        if (canvas.HandleAccelerator(acc))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Mouse moved handler.
    /// </summary>
    /// <param name="canvas">Canvas.</param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="dx"></param>
    /// <param name="dy"></param>
    public static void OnMouseMoved(Base canvas, int x, int y, int dx, int dy)
    {
        // Send input to canvas for study
        MousePosition.X = x;
        MousePosition.Y = y;

        UpdateHoveredControl(canvas);
    }

    /// <summary>
    ///     Handles focus updating and key autorepeats.
    /// </summary>
    /// <param name="control">Unused.</param>
    public static void OnCanvasThink(Base control)
    {
        if (MouseFocus != null && !MouseFocus.IsVisible)
        {
            MouseFocus = null;
        }

        if (KeyboardFocus != null && (!KeyboardFocus.IsVisible || !KeyboardFocus.KeyboardInputEnabled))
        {
            // KeyboardFocus = null;
        }

        if (null == KeyboardFocus)
        {
            return;
        }

        if (KeyboardFocus.GetCanvas() != control)
        {
            return;
        }

        var time = Platform.Neutral.GetTimeInSeconds();

        //
        // Simulate Key-Repeats
        //
        for (var i = 0; i < (int) Key.Count; i++)
        {
            if (KeyData.KeyState[i] && KeyData.Target != KeyboardFocus)
            {
                KeyData.KeyState[i] = false;

                continue;
            }

            if (KeyData.KeyState[i] && time > KeyData.NextRepeat[i])
            {
                KeyData.NextRepeat[i] = Platform.Neutral.GetTimeInSeconds() + KeyRepeatRate;

                if (KeyboardFocus != null)
                {
                    KeyboardFocus.InputKeyPressed((Key) i);
                }
            }
        }

        UpdateHoveredControl(control);
    }

    /// <summary>
    ///     Mouse click handler.
    /// </summary>
    /// <param name="canvas">Canvas.</param>
    /// <param name="mouseButton">Mouse button number.</param>
    /// <param name="pressed">Specifies if the button is down.</param>
    /// <returns>True if handled.</returns>
    public static bool OnMouseButtonStateChanged(Base canvas, MouseButton mouseButton, bool pressed)
    {
        var hoveredControl = HoveredControl;

        // If we click on a control that isn't a menu we want to close
        // all the open menus. Menus are children of the canvas.
        if (pressed && hoveredControl is not { IsMenuComponent: true })
        {
            canvas.CloseMenus();
        }

        if (hoveredControl == null || hoveredControl == canvas)
        {
            return false;
        }

        if (hoveredControl.Canvas != canvas)
        {
            return false;
        }

        if (!hoveredControl.IsVisible)
        {
            return false;
        }

        if (!Enum.IsDefined(mouseButton))
        {
            return false;
        }

        var mousePosition = MousePosition;

        if (KeyData.SetMouseButtonState(mouseButton, pressed))
        {
            hoveredControl.InputMouseButtonState(MouseButton.Left, mousePosition, pressed);
        }

        // Double click.
        // Todo: Shouldn't double click if mouse has moved significantly
        var isDoubleClick = pressed &&
                            _lastClickPosition == mousePosition &&
                            GetDeltaClickTime(mouseButton) < DoubleClickSpeed;

        if (pressed)
        {
            if (!isDoubleClick)
            {
                LastClickTime[mouseButton] = Platform.Neutral.GetTimeInSeconds();
                _lastClickPosition = mousePosition;
            }

            FindKeyboardFocus(hoveredControl);
        }

        hoveredControl.UpdateCursor();

        // This tells the child it has been touched, which
        // in turn tells its parents, who tell their parents.
        // This is basically so that Windows can pop themselves
        // to the top when one of their children have been clicked.
        if (pressed)
        {
            hoveredControl.Touch();
        }

        if (mouseButton == MouseButton.Left)
        {
            if (DragAndDrop.OnMouseButton(hoveredControl, mousePosition.X, mousePosition.Y, pressed))
            {
                return true;
            }
        }

        if (isDoubleClick)
        {
            hoveredControl.InputMouseDoubleClicked(mouseButton, mousePosition);
        }

#if GWEN_HOOKSYSTEM
        if (bDown)
        {
            if (Hook::CallHook(&Hook::BaseHook::OnControlClicked, HoveredControl, MousePosition.x,
                               MousePosition.y))
                return true;
        }
#endif

        return false;
    }

    private static float GetDeltaClickTime(MouseButton mouseButton) =>
        Platform.Neutral.GetTimeInSeconds() - LastClickTime.GetValueOrDefault(mouseButton, 0f);

    /// <summary>
    ///     Mouse click handler.
    /// </summary>
    /// <param name="canvas">Canvas.</param>
    /// <param name="mouseButton">Mouse button number.</param>
    /// <param name="down">Specifies if the button is down.</param>
    /// <returns>True if handled.</returns>
    public static bool OnMouseScroll(Base canvas, int deltaX, int deltaY)
    {

        if (canvas == null ||
            HoveredControl == null ||
            HoveredControl.GetCanvas() != canvas ||
            !canvas.IsVisible
        )
        {
            return false;
        }

        if (deltaY != 0)
        {
            HoveredControl.InputMouseWheeled(deltaY);
        }

        if (deltaX != 0)
        {
            HoveredControl.InputMouseHWheeled(deltaX);
        }

        return true;
    }

        /// <summary>
        ///     Key handler.
        /// </summary>
        /// <param name="canvas">Canvas.</param>
        /// <param name="key">Key.</param>
        /// <param name="down">True if the key is down.</param>
        /// <returns>True if handled.</returns>
        public static bool OnKeyEvent(Base canvas, Key key, bool down)
    {
        if (null == KeyboardFocus)
        {
            return false;
        }

        if (KeyboardFocus.GetCanvas() != canvas)
        {
            return false;
        }

        if (!KeyboardFocus.IsVisible)
        {
            return false;
        }

        var iKey = (int) key;
        if (down)
        {
            if (!KeyData.KeyState[iKey])
            {
                KeyData.KeyState[iKey] = true;
                KeyData.NextRepeat[iKey] = Platform.Neutral.GetTimeInSeconds() + KeyRepeatDelay;
                KeyData.Target = KeyboardFocus;

                return KeyboardFocus.InputKeyPressed(key);
            }
        }
        else
        {
            if (KeyData.KeyState[iKey])
            {
                KeyData.KeyState[iKey] = false;

                // BUG BUG. This causes shift left arrow in textboxes
                // to not work. What is disabling it here breaking?
                //m_KeyData.Target = NULL;

                return KeyboardFocus.InputKeyPressed(key, false);
            }
        }

        return false;
    }

    private static void UpdateHoveredControl(Base inCanvas)
    {
        Base? componentAtPosition = inCanvas.GetComponentAt(MousePosition.X, MousePosition.Y);

        var mouseFocusedControl = MouseFocus;
        if (mouseFocusedControl?.Canvas != inCanvas)
        {
            mouseFocusedControl = null;
        }

        var nextHoveredControl = mouseFocusedControl ?? componentAtPosition;
        var previouslyHoveredControl = HoveredControl;
        if (nextHoveredControl != previouslyHoveredControl)
        {
            HoveredControl = null;
            previouslyHoveredControl?.Redraw();
            previouslyHoveredControl?.InputMouseLeft();

            HoveredControl = nextHoveredControl;

            if (nextHoveredControl != mouseFocusedControl)
            {
                nextHoveredControl?.InputMouseEntered();
            }

            nextHoveredControl?.Redraw();

            ApplicationContext.CurrentContext.Logger.LogTrace(
                "Updating hovered component from {PreviousComponentName} to {NextComponentName} (Mouse focused component is '{MouseFocusedComponentName}')",
                previouslyHoveredControl?.CanonicalName ?? "N/A",
                nextHoveredControl?.CanonicalName ?? "N/A",
                mouseFocusedControl?.CanonicalName ?? "N/A"
            );
        }

        if (nextHoveredControl is null)
        {
            return;
        }

        if (nextHoveredControl.KeepFocusOnMouseExit)
        {
            return;
        }

        foreach (var mouseButton in MouseButtons)
        {
            var isMouseButtonDown = KeyData.IsMouseButtonDown(mouseButton);
            nextHoveredControl.InputMouseButtonState(mouseButton, MousePosition, isMouseButtonDown);
        }
    }

    private static void FindKeyboardFocus(Base control)
    {
        if (null == control)
        {
            return;
        }

        if (control.KeyboardInputEnabled)
        {
            //Make sure none of our children have keyboard focus first - todo recursive
            if (control.Children.Any(child => child == KeyboardFocus))
            {
                return;
            }

            control.Focus();

            return;
        }

        FindKeyboardFocus(control.Parent);

        return;
    }

}
