using System.Numerics;

using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.UserInterface.Styling;
using Intersect.Collections;
using Intersect.Numerics;

namespace Intersect.Client.Framework.UserInterface;

public abstract partial class Component
{
    private readonly ComponentMetadata _metadata;

    private FloatRect _bounds;
    private DisplayMode _displayMode;
    private FloatRect _imguiBounds;

    protected Component(string? name = default)
    {
        _metadata = new ComponentMetadata(this);

        Name = name;

        _childrenNotifier = new ChildrenNotifier(this);
        _children = new NotifierList<Component>(_childrenNotifier);
        _displayMode = DisplayMode.Initial;
    }

    public FloatRect Bounds
    {
        get => new(Position, Size);
        set
        {
            if (_bounds == value)
            {
                return;
            }

            _bounds = value;
            Invalidate();
        }
    }

    protected virtual IContentManager? ContentManager => Parent?.ContentManager;

    public DisplayMode Display
    {
        get => _displayMode;
        set
        {
            if (value == _displayMode)
            {
                return;
            }

            _displayMode = value;
            Invalidate(true);
        }
    }

    public Vector2 Position
    {
        get => _bounds.Position;
        set
        {
            if (Position == value)
            {
                return;
            }

            _bounds = new(value, Size);
            Invalidate();
        }
    }

    public string? Name { get; }

    public Vector2 Size
    {
        get => _bounds.Size;
        set
        {
            if (Size == value)
            {
                return;
            }

            _bounds = new(Position, value);
            Invalidate();
        }
    }

    protected bool SynchronizeBounds(FloatRect bounds)
    {
        if (_imguiBounds == bounds)
        {
            var shouldSynchronize = _bounds != bounds;
            if (shouldSynchronize)
            {
                _imguiBounds = _bounds;
            }
            return true;
        }

        _imguiBounds = bounds;
        _bounds = bounds;
        return false;
    }

    protected bool SynchronizeBounds(Vector2 position, Vector2 size) => SynchronizeBounds(new(position, size));
}
