namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     Vertical slider.
/// </summary>
public partial class VerticalSlider : Slider
{

    /// <summary>
    ///     Initializes a new instance of the <see cref="VerticalSlider" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    public VerticalSlider(Base parent) : base(parent)
    {
        _sliderBar.IsHorizontal = false;
    }

    protected override float CalculateValue()
    {
        return 1 - _sliderBar.Y / (float) (Height - _sliderBar.Height);
    }

    protected override void UpdateBarFromValue()
    {
        _sliderBar.MoveTo(_sliderBar.X, (int) ((Height - _sliderBar.Height) * (1 - _value)));
    }

    /// <summary>
    ///     Handler invoked on mouse click (left) event.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <param name="down">If set to <c>true</c> mouse button is down.</param>
    protected override void OnMouseClickedLeft(int x, int y, bool down, bool automated = false)
    {
        base.OnMouseClickedLeft(x, y, down);
        _sliderBar.MoveTo(_sliderBar.X, (int) (CanvasPosToLocal(new Point(x, y)).Y - _sliderBar.Height * 0.5));
        _sliderBar.InputMouseClickedLeft(x, y, down);
        SliderBarOnDragged(_sliderBar, EventArgs.Empty);
    }

    /// <summary>
    ///     Lays out the control's interior according to alignment, padding, dock etc.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Layout(Skin.Base skin)
    {
        _sliderBar.SetSize(Width, 15);
        UpdateBarFromValue();
    }

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
        skin.DrawSlider(this, false, Notches, _snapToNotches ? _notchCount : 0, _sliderBar.Height);
    }

}
