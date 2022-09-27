using Intersect.Time;

namespace Intersect.Editor.Interface;

public partial class Component
{
    private bool _dirty;

    protected bool SkipLayoutEnd { get; set; }

    public virtual void Invalidate(bool invalidateChildren = false)
    {
        _dirty = true;

        if (invalidateChildren)
        {
            foreach (var child in Children)
            {
                child.Invalidate(invalidateChildren);
            }
        }

        _parent?.Invalidate();
    }
    public void Layout(FrameTime frameTime)
    {
        if (_dirty)
        {
            LayoutDirty(frameTime);
            _bounds = Bounds;
            _boundsDirty = null;
            _dirty = false;
        }

        if (LayoutBegin(frameTime))
        {
            LayoutChildren(frameTime);
        }

        if (!SkipLayoutEnd)
        {
            LayoutEnd(frameTime);
        }
    }

    private void LayoutChildren(FrameTime frameTime)
    {
        var childrenSnapshot = Children.ToArray();
        foreach (var child in childrenSnapshot)
        {
            child.Layout(frameTime);
        }
    }

    protected abstract bool LayoutBegin(FrameTime frameTime);

    protected virtual void LayoutDirty(FrameTime frameTime)
    {

    }

    protected abstract void LayoutEnd(FrameTime frameTime);
}
