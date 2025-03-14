namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     Radio button.
/// </summary>
public partial class RadioButton : Checkbox
{

    /// <summary>
    ///     Initializes a new instance of the <see cref="RadioButton" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    public RadioButton(Base parent, string name = default, bool disableText = true) : base(parent, name, disableText)
    {
        SetSize(15, 15);
        MouseInputEnabled = true;
        IsTabable = false;
        IsToggle = true; //[halfofastaple] technically true. "Toggle" isn't the best word, "Sticky" is a better one.
    }

    /// <summary>
    ///     Determines whether unchecking is allowed.
    /// </summary>
    protected override bool AllowUncheck => false;

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
        skin.DrawRadioButton(this, IsChecked, IsHovered, IsActive);
    }

}
