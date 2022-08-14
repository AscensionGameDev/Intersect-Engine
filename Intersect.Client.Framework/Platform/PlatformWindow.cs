using Intersect.Client.Framework.Graphics;
using Intersect.Numerics;

namespace Intersect.Client.Framework.Platform;

public abstract class PlatformWindow
{
    public event EventHandler Resized;

    public event EventHandler<TextInputEventArgs> TextInput;

    public abstract Rectangle Bounds { get; set; }

    public abstract GraphicsDevice GraphicsDevice { get; }

    public abstract Numerics.Point Position { get; set; }

    public abstract Numerics.Point Size { get; set; }

    public abstract string Title { get; set; }

    public abstract Viewport Viewport { get; set; }

    public abstract void Close();

    protected void OnResize(EventArgs eventArgs) =>
        Resized?.Invoke(this, eventArgs);

    protected void OnTextInput(TextInputEventArgs textInputEventArgs) =>
        TextInput?.Invoke(this, textInputEventArgs);
}
