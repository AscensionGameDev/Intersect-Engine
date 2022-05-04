using Intersect.Client.Framework.UserInterface.Styling;
using Intersect.Time;

namespace Intersect.Client.Framework.UserInterface;

public partial class Component
{
    public void Draw(FrameTime frameTime)
    {
        if (_dirty)
        {
            LayoutBegin();
            Layout();
            LayoutEnd();

            _dirty = false;
        }

        if (DisplayMode != DisplayMode.None)
        {

            if (DrawBegin())
            {
                DrawBehindChildren(frameTime);

                DrawChildren(frameTime);

                DrawAboveChildren(frameTime);
            }

            DrawEnd();
        }
    }

    protected virtual void DrawAboveChildren(FrameTime frameTime)
    {

    }

    protected abstract bool DrawBegin();

    protected virtual void DrawBehindChildren(FrameTime frameTime)
    {

    }

    protected virtual void DrawChildren(FrameTime frameTime)
    {
        foreach (var child in Children)
        {
            child.Draw(frameTime);
        }
    }

    protected abstract void DrawEnd();
}
