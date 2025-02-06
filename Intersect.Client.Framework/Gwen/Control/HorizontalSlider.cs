using Intersect.Client.Framework.Input;

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

    protected override void OnMouseClicked(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        base.OnMouseClicked(mouseButton, mousePosition, userAction);

        if (mouseButton != MouseButton.Left)
        {
            return;
        }

        var localCoordinates = CanvasPosToLocal(mousePosition);
        _sliderBar.MoveTo((int) (localCoordinates.X - _sliderBar.Width * 0.5), _sliderBar.Y);
        _sliderBar.InputNonUserMouseClicked(mouseButton, mousePosition, true);
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
