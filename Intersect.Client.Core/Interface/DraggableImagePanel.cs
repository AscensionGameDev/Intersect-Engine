using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Gwen.Skin;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Base = Intersect.Client.Framework.Gwen.Control.Base;

namespace Intersect.Client.Interface;

/// <summary>
/// ImagePanel with Alt+drag functionality for UI customization.
/// </summary>
public class DraggableImagePanel : ImagePanel
{
    // Alt+drag state
    private bool _isAltDragging;
    private Point _altDragStartPos;
    private Point _altDragStartWindowPos;
    private string? _preferenceKey;
    private bool _positionLoaded;

    public DraggableImagePanel(Base parent, string? name = default, string? preferenceKey = null) 
        : base(parent, name)
    {
        _preferenceKey = preferenceKey ?? name;
        MouseInputEnabled = true;
    }

    protected override void OnPreDraw(Intersect.Client.Framework.Gwen.Skin.Base skin)
    {
        base.OnPreDraw(skin);
        
        // Load saved position after first draw (only once)
        if (!_positionLoaded && !string.IsNullOrWhiteSpace(_preferenceKey))
        {
            LoadSavedPosition();
            _positionLoaded = true;
        }
    }

    protected override void OnChildTouched(Base control)
    {
        base.OnChildTouched(control);

        // If Alt is held when a child is clicked, start Alt+drag
        if (Globals.InputManager != null && 
            (Globals.InputManager.IsKeyDown(Keys.Alt) || Globals.InputManager.IsKeyDown(Keys.LMenu)) &&
            InputHandler.MousePosition is { } mousePosition)
        {
            // Remove alignments so control can be freely positioned
            RemoveAlignments();
            
            // Start Alt+drag - store the hold position relative to the control
            _isAltDragging = true;
            _altDragStartPos = CanvasPosToLocal(mousePosition);
            _altDragStartWindowPos = new Point(X, Y);
            InputHandler.MouseFocus = this;
        }
    }

    protected override void OnMouseDown(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        base.OnMouseDown(mouseButton, mousePosition, userAction);

        // Check if Alt key is held and left mouse button is pressed
        if (userAction && mouseButton == MouseButton.Left && 
            Globals.InputManager != null && 
            (Globals.InputManager.IsKeyDown(Keys.Alt) || Globals.InputManager.IsKeyDown(Keys.LMenu)))
        {
            // Remove alignments so control can be freely positioned
            RemoveAlignments();
            
            // Start Alt+drag - store the hold position relative to the control
            _isAltDragging = true;
            _altDragStartPos = CanvasPosToLocal(mousePosition);
            _altDragStartWindowPos = new Point(X, Y);
            InputHandler.MouseFocus = this;
        }
    }

    protected override void OnMouseUp(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        base.OnMouseUp(mouseButton, mousePosition, userAction);

        if (_isAltDragging && mouseButton == MouseButton.Left)
        {
            _isAltDragging = false;
            InputHandler.MouseFocus = null;
            
            // Save position when drag ends
            SavePosition();
        }
    }

    protected override void OnMouseMoved(int x, int y, int dx, int dy)
    {
        base.OnMouseMoved(x, y, dx, dy);

        if (_isAltDragging && IsActive)
        {
            // Calculate new position based on mouse movement (similar to Dragger)
            // x, y are canvas coordinates
            Point position = new(x - _altDragStartPos.X, y - _altDragStartPos.Y);
            if (Parent is { } parent)
            {
                position = parent.ToLocal(position.X, position.Y);
            }

            // Clamp to parent bounds if RestrictToParent is enabled
            var newX = position.X;
            var newY = position.Y;

            if (Parent != null && RestrictToParent)
            {
                var windowWidth = Bounds.Width;
                var windowHeight = Bounds.Height;
                var parentWidth = Parent.Bounds.Width;
                var parentHeight = Parent.Bounds.Height;

                if (newX + windowWidth > parentWidth)
                {
                    newX = parentWidth - windowWidth;
                }
                if (newX < 0)
                {
                    newX = 0;
                }
                if (newY + windowHeight > parentHeight)
                {
                    newY = parentHeight - windowHeight;
                }
                if (newY < 0)
                {
                    newY = 0;
                }
            }

            MoveTo(newX, newY);
        }
    }

    private void SavePosition()
    {
        if (string.IsNullOrWhiteSpace(_preferenceKey) || Globals.Database == null)
        {
            return;
        }

        var positionKey = $"WindowPosition_{_preferenceKey}";
        var positionValue = $"{X},{Y}";
        Globals.Database.SavePreference(positionKey, positionValue);
    }

    private void LoadSavedPosition()
    {
        if (string.IsNullOrWhiteSpace(_preferenceKey) || Globals.Database == null)
        {
            return;
        }

        var positionKey = $"WindowPosition_{_preferenceKey}";
        var positionValue = Globals.Database.LoadPreference(positionKey);
        
        if (string.IsNullOrWhiteSpace(positionValue))
        {
            return;
        }

        // Parse position (format: "X,Y")
        var parts = positionValue.Split(',');
        if (parts.Length == 2 && 
            int.TryParse(parts[0], out var x) && 
            int.TryParse(parts[1], out var y))
        {
            // Validate position is within parent bounds
            if (Parent != null)
            {
                var windowWidth = Bounds.Width;
                var windowHeight = Bounds.Height;
                var parentWidth = Parent.Bounds.Width;
                var parentHeight = Parent.Bounds.Height;

                if (x + windowWidth > parentWidth)
                {
                    x = parentWidth - windowWidth;
                }
                if (x < 0)
                {
                    x = 0;
                }
                if (y + windowHeight > parentHeight)
                {
                    y = parentHeight - windowHeight;
                }
                if (y < 0)
                {
                    y = 0;
                }
            }

            // Remove alignments when loading custom position
            RemoveAlignments();
            MoveTo(x, y);
        }
    }

    protected override void OnJsonReloaded()
    {
        base.OnJsonReloaded();
        // Load saved position after JSON UI is loaded so it overrides default positions
        if (!string.IsNullOrWhiteSpace(_preferenceKey))
        {
            LoadSavedPosition();
        }
    }
}

