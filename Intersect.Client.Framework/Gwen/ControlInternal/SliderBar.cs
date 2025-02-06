using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen.ControlInternal;

/// <summary>
///     Slider bar.
/// </summary>
public partial class SliderBar : Dragger
{
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
}