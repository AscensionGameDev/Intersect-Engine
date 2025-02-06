namespace Intersect.Client.Framework.Gwen.Control.Property;


/// <summary>
///     Checkable property.
/// </summary>
public partial class Check : Base
{

    protected readonly Checkbox Checkbox;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Check" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    public Check(Control.Base parent) : base(parent)
    {
        Checkbox = new Checkbox(this);
        Checkbox.ShouldDrawBackground = false;
        Checkbox.CheckChanged += OnValueChanged;
        Checkbox.IsTabable = true;
        Checkbox.KeyboardInputEnabled = true;
        Checkbox.SetPosition(2, 1);

        Height = 18;
    }

    /// <summary>
    ///     Property value.
    /// </summary>
    public override string Value
    {
        get => Checkbox.IsChecked ? "1" : "0";
        set => base.Value = value;
    }

    /// <summary>
    ///     Indicates whether the property value is being edited.
    /// </summary>
    public override bool IsEditing => Checkbox.HasFocus;

    /// <summary>
    ///     Indicates whether the control is hovered by mouse pointer.
    /// </summary>
    public override bool IsHovered => base.IsHovered || Checkbox.IsHovered;

    /// <summary>
    ///     Sets the property value.
    /// </summary>
    /// <param name="value">Value to set.</param>
    /// <param name="fireEvents">Determines whether to fire "value changed" event.</param>
    public override void SetValue(string value, bool fireEvents = false)
    {
        if (value == "1" || value.ToLower() == "true" || value.ToLower() == "yes")
        {
            Checkbox.IsChecked = true;
        }
        else
        {
            Checkbox.IsChecked = false;
        }
    }

}
