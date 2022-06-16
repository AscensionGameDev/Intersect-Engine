using Intersect.Client.Framework.UserInterface.Styling;
using Intersect.Time;

namespace Intersect.Client.Framework.UserInterface;

public partial class Component
{
    private bool _dirty;

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
        if (Display == DisplayMode.None)
        {
            return;
        }

        if (LayoutBegin())
        {
            if (_dirty)
            {

                _dirty = false;
            }

            LayoutChildren(frameTime);
        }

        LayoutEnd();
    }

    private void LayoutChildren(FrameTime frameTime)
    {
        foreach (var child in Children)
        {
            child.Layout(frameTime);
        }
    }

    protected abstract bool LayoutBegin();

    protected abstract void LayoutEnd();

    protected virtual void Relayout();
}
