using Intersect.Client.Framework.Gwen.Control;
using Base = Intersect.Client.Framework.Gwen.Control.Base;

namespace Intersect.Client.Interface;

public abstract class Window : WindowControl
{
    private delegate void Initializer(Intersect.Client.Framework.Gwen.Skin.Base skin);

    private readonly object _initializationLock = new();
    private volatile bool _initialized;

    private Initializer? _initializer;

    protected Window(
        Base parent,
        string? title = default,
        bool modal = false,
        string? name = default
    ) : base(parent, title, modal, name)
    {
        _initializer = Initialize;

        SetTextColor(Color.White, ControlState.Active);
        SetTextColor(new Color(a: 255, r: 191, g: 191, b: 191), ControlState.Inactive);

        SkipRender();
    }

    protected override void OnPreDraw(Intersect.Client.Framework.Gwen.Skin.Base skin)
    {
        base.OnPreDraw(skin);

        if (_initializer is { } initializer)
        {
            _initializer = null;
            initializer(skin);
        }
    }

    private void Initialize(Intersect.Client.Framework.Gwen.Skin.Base skin)
    {
        lock (_initializationLock)
        {
            EnsureInitialized();
            DoLayoutIfNeeded(skin);
        }
    }

    protected abstract void EnsureInitialized();
}