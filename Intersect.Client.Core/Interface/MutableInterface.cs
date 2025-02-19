using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Interface.Debugging;
using Intersect.Client.Localization;
using Intersect.Reflection;

namespace Intersect.Client.Interface;

public abstract partial class MutableInterface : IMutableInterface
{
    private static DebugWindow? _debugWindow;
    private static DebugWindow? _debugWindow2;

    public static void ReparentDebugWindow(Base parent)
    {
        if (_debugWindow is { } debugWindow)
        {
            debugWindow.Parent = parent;
        }

        if (_debugWindow2 is { } debugWindow2)
        {
            debugWindow2.Parent = parent;
        }
    }

    public static void DetachDebugWindow()
    {
        if (_debugWindow != null)
        {
            _debugWindow.Parent = default;
        }

        if (_debugWindow2 != null)
        {
            _debugWindow2.Parent = default;
        }
    }

    internal static void DisposeDebugWindow()
    {
        _debugWindow?.Dispose();
        _debugWindow2?.Dispose();
    }

    private static DebugWindow EnsureDebugWindowInitialized(Base parent)
    {
        _debugWindow ??= new DebugWindow(parent);
        _debugWindow.Parent = parent;

#if DEBUG
        if (_debugWindow2 is null)
        {
            _debugWindow2 = new DebugWindow(parent)
            {
                Alignment = [Alignments.Top, Alignments.Right],
                Title = Strings.Debug.TitleX.ToString(2),
            };
            _debugWindow2.PostLayout.Enqueue(window => window.Alignment = [], _debugWindow2);
        }
        _debugWindow2.Parent = parent;
#endif

        return _debugWindow;
    }

    protected internal MutableInterface(Base root)
    {
        Root = root;
    }

    internal Base Root { get; }

    public int NodeCount => Root.NodeCount;

    /// <inheritdoc />
    public IReadOnlyList<Base> Children => Root.Children ?? [];

    /// <inheritdoc />
    public TElement Create<TElement>(params object[] parameters) where TElement : Base
    {
        var fullParameterList = new List<object?> { Root };
        fullParameterList.AddRange(parameters);

        var fullParameters = fullParameterList.ToArray();
        var constructor = typeof(TElement).FindConstructors(fullParameters).FirstOrDefault();

        if (constructor != null)
        {
            var constructorParameters = constructor.GetParameters();
            if (fullParameters.Length != constructorParameters.Length)
            {
                fullParameters =
                [
                    .. fullParameters,
                    .. Enumerable.Range(0, constructorParameters.Length - fullParameters.Length)
                        .Select(_ => default(object))
,
                ];
            }

            return constructor.Invoke(fullParameters) is not TElement constructedElement
                ? throw new NullReferenceException("Failed to invoke constructor that matches parameters.")
                : constructedElement;
        }

        throw new NullReferenceException("Failed to find constructor that matches parameters.");
    }

    /// <inheritdoc />
    public TElement? Find<TElement>(string? name = null, bool recurse = false) where TElement : Base
    {
        return !string.IsNullOrWhiteSpace(name)
            ? Root.FindChildByName(name, recurse) as TElement
            : Root.Find(child => child is TElement) as TElement;
    }

    /// <inheritdoc />
    public IEnumerable<TElement?> FindAll<TElement>(bool recurse = false) where TElement : Base
    {
        return Root.FindAll(child => child is TElement, recurse).Select(child => child as TElement);
    }

    /// <inheritdoc />
    public void Remove<TElement>(TElement element, bool dispose = false) where TElement : Base
    {
        Root.RemoveChild(element, dispose);
    }

    public bool ToggleDebug()
    {
        var debugWindow = EnsureDebugWindowInitialized(Root);
        debugWindow.ToggleHidden();

        if (_debugWindow2 is { } dw2)
        {
            dw2.IsVisibleInTree = debugWindow.IsVisibleInTree;
        }

        return debugWindow.IsVisibleInTree;
    }
}
