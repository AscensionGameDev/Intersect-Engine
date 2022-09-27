using System.Numerics;
using Intersect.Client.Framework.Content;
using Intersect.Numerics;

namespace Intersect.Editor.Interface;

public abstract partial class Component
{
    private readonly ComponentMetadata _metadata;

    private FloatRect _bounds;
    private FloatRect? _boundsDirty;

    protected Component(string? name = default)
    {
        _metadata = new(this);

        Name = name;

        _childrenNotifier = new(this);
        _children = new(_childrenNotifier);
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
