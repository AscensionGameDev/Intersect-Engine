namespace Intersect.Client.Framework.Gwen.Control;

public partial class TabControl
{
    private sealed class TabControlRenderOrderComparer : IComparer<Base>
    {
        public int Compare(Base? x, Base? y)
        {
            if (x is not TabButton xTabButton)
            {
                return y is null or not TabButton ? 0 : 1;
            }

            if (y is not TabButton)
            {
                return -1;
            }

            return xTabButton.IsActive ? 1 : -1;
        }
    }

    private static readonly TabControlRenderOrderComparer RenderOrderComparer = new();

    protected override Base[] OrderChildrenForRendering(IEnumerable<Base> children)
    {
        return children.Order(RenderOrderComparer).ToArray();
    }
}