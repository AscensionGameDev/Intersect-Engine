namespace Intersect.Client.Framework.Windows;

public abstract class SystemWindow
{
    public event EventHandler<TextInputEventArgs> TextInput;

    public abstract string Title { get; set; }

    protected void OnTextInput(TextInputEventArgs textInputEventArgs)
    {
        TextInput?.Invoke(this, textInputEventArgs);
    }
}
