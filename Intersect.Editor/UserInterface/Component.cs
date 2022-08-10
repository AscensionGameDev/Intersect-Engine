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
    private FloatRect? _boundsDirty;
    private DisplayMode _displayMode;

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
        get => _boundsDirty ?? _bounds;
        set
        {
            if (Bounds == value)
            {
                return;
            }

            _boundsDirty = value;
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
        }
    }

    public Vector2 Position
    {
        get => Bounds.Position;
        set
        {
            if (Position == value)
            {
                return;
            }

            _boundsDirty = new(value, Size);
            Invalidate();
        }
    }

    public string? Name { get; }

    public Vector2 Size
    {
        get => Bounds.Size;
        set
        {
            if (Size == value)
            {
                return;
            }

            _boundsDirty = new(Position, value);
            Invalidate();
        }
    }
}
