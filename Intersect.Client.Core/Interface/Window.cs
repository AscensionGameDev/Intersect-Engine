using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Interface;

public abstract class Window : WindowControl
{
    private readonly object _initializationLock = new();
    private volatile bool _initialized;

    protected Window(
        Base parent,
        string? title = default,
        bool modal = false,
        string? name = default
    ) : base(parent, title, modal, name)
    {
        SetTextColor(Color.White, ControlState.Active);
        SetTextColor(new Color(a: 255, r: 191, g: 191, b: 191), ControlState.Inactive);
    }

    protected override void Render(Framework.Gwen.Skin.Base skin)
    {
        lock (_initializationLock)
        {
            if (!_initialized)
            {
                _initialized = true;
                EnsureInitialized();
            }
        }

        base.Render(skin);
    }

    protected abstract void EnsureInitialized();
}