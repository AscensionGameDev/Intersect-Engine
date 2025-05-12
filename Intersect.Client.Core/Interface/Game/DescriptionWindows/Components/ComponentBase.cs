using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Interface.Game.DescriptionWindows.Components;

public partial class ComponentBase(Base parent, string name = "") : ImagePanel(parent, name)
{
    /// <summary>
    /// Corrects the width of the component compared to the parent size.
    /// </summary>
    public virtual void CorrectWidth()
    {
        if (Parent == default)
        {
            return;
        }

        SetSize(Parent.InnerWidth, InnerHeight);
    }
}
