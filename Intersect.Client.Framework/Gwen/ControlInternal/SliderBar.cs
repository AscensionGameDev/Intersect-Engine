using System.Numerics;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen.ControlInternal;

/// <summary>
///     Slider bar.
/// </summary>
public partial class SliderBar : Dragger
{
    private Vector2? _anchorAxis;
    private Orientation _orientation;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SliderBar" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="name"></param>
    public SliderBar(Slider parent, string? name = default) : base(parent, name)
    {
        Target = this;
        RestrictToParent = true;
        Orientation = parent.Orientation;
    }

    public Vector2? AnchorAxis
    {
        get => _anchorAxis;
        set => SetAndDoIfChanged(ref _anchorAxis, value, Invalidate);
    }

    public Orientation Orientation
    {
        get => _orientation;
        set => SetAndDoIfChanged(ref _orientation, value, Invalidate);
    }

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin) => skin.DrawSliderButton(this);

    // protected override void Layout(Skin.Base skin)
    // {
    //     base.Layout(skin);
    //
    //     SetBounds(Bounds);
    // }

    public override bool SetBounds(int x, int y, int width, int height)
    {
        if (Parent is { } parent && AnchorAxis is { } anchorAxis)
        {
            switch (Orientation)
            {
                case Orientation.LeftToRight:
                case Orientation.RightToLeft:
                    y = (int)(anchorAxis.Y * parent.Height - Height) - 1;
                    break;

                case Orientation.TopToBottom:
                case Orientation.BottomToTop:
                    x = (int)(anchorAxis.X * parent.Width - Width) - 1;
                    break;
            }
        }

        return base.SetBounds(x, y, width, height);
    }
}