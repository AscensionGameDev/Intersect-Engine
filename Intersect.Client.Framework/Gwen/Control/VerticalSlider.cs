using Intersect.Client.Framework.Input;

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

    protected override void OnMouseClicked(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        base.OnMouseClicked(mouseButton, mousePosition, userAction);

        if (mouseButton != MouseButton.Left)
        {
            return;
        }

        var localCoordinates = CanvasPosToLocal(mousePosition);
        _sliderBar.MoveTo(_sliderBar.X, (int) (localCoordinates.Y - _sliderBar.Height * 0.5));
        _sliderBar.InputNonUserMouseClicked(mouseButton, mousePosition, true);
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
