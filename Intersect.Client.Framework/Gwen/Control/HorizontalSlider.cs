namespace Intersect.Client.Framework.Gwen.Control;

/// <summary>
///     Horizontal slider.
/// </summary>
public partial class HorizontalSlider : Slider
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="HorizontalSlider" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="name"></param>
    public HorizontalSlider(Base parent, string? name = default) : base(parent, name)
    {
        _sliderBar.IsHorizontal = true;
    }

    protected override float CalculateValue()
    {
        return (float) _sliderBar.X / (Width - _sliderBar.Width);
    }

    protected override void UpdateBarFromValue()
    {
        _sliderBar.MoveTo((int) ((Width - _sliderBar.Width) * _value), _sliderBar.Y);
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
        _sliderBar.MoveTo((int) (CanvasPosToLocal(new Point(x, y)).X - _sliderBar.Width * 0.5), _sliderBar.Y);
        _sliderBar.InputMouseClickedLeft(x, y, down, true);
        SliderBarOnDragged(_sliderBar, EventArgs.Empty);
    }

    /// <summary>
    ///     Lays out the control's interior according to alignment, padding, dock etc.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Layout(Skin.Base skin)
    {
        //m_SliderBar.SetSize(15, Height);
        UpdateBarFromValue();
    }

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
        skin.DrawSlider(this, true, Notches, _snapToNotches ? _notchCount : 0, _sliderBar.Width);
    }

}
