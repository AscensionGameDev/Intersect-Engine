using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Input;

namespace Intersect.Client.Framework.Gwen.ControlInternal;

/// <summary>
///     Scrollbar bar.
/// </summary>
public partial class ScrollBarBar : Dragger
{
    private bool mHorizontal;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ScrollBarBar" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    public ScrollBarBar(Base parent) : base(parent)
    {
        RestrictToParent = true;
        Target = this;

        SetSound(ButtonSoundState.Hover, "octave-tap-resonant.wav");
        SetSound(ButtonSoundState.MouseDown, "octave-tap-warm.wav");
        SetSound(ButtonSoundState.MouseUp, "octave-tap-warm.wav");
    }

    /// <summary>
    ///     Indicates whether the bar is horizontal.
    /// </summary>
    public bool IsHorizontal
    {
        get => mHorizontal;
        set => mHorizontal = value;
    }

    /// <summary>
    ///     Indicates whether the bar is vertical.
    /// </summary>
    public bool IsVertical
    {
        get => !mHorizontal;
        set => mHorizontal = !value;
    }

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
        skin.DrawScrollBarBar(this);
        base.Render(skin);
    }

    /// <summary>
    ///     Handler invoked on mouse moved event.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <param name="dx">X change.</param>
    /// <param name="dy">Y change.</param>
    protected override void OnMouseMoved(int x, int y, int dx, int dy)
    {
        base.OnMouseMoved(x, y, dx, dy);
        if (!IsActive)
        {
            return;
        }

        InvalidateParent();
    }

    /// <summary>
    ///     Lays out the control's interior according to alignment, padding, dock etc.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Layout(Skin.Base skin)
    {
        if (null == Parent)
        {
            return;
        }

        //Move to our current position to force clamping - is this a hack?
        MoveTo(X, Y);
    }

    protected override void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        base.OnBoundsChanged(oldBounds, newBounds);

        InvalidateParent();
    }
}
