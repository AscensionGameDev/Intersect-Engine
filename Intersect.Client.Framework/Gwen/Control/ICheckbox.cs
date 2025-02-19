using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Framework.Eventing;

namespace Intersect.Client.Framework.Gwen.Control;

public interface ICheckbox
{
    /// <summary>
    ///     Invoked when the checkbox has been checked.
    /// </summary>
    event EventHandler<ICheckbox, EventArgs>? Checked;

    /// <summary>
    ///     Invoked when the checkbox has been unchecked.
    /// </summary>
    event EventHandler<ICheckbox, EventArgs>? Unchecked;

    /// <summary>
    ///     Invoked when the checkbox state has been changed.
    /// </summary>
    event EventHandler<ICheckbox, ValueChangedEventArgs<bool>>? CheckChanged;

    bool IsChecked { get; set; }
}