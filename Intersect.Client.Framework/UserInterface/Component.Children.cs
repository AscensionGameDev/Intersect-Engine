using Intersect.Collections;

namespace Intersect.Client.Framework.UserInterface;

public partial class Component
{
    private readonly NotifierList<Component> _children;
    private readonly ChildrenNotifier _childrenNotifier;

    private Component? _parent;

    public IList<Component> Children => _children;

    public Component? Parent
    {
        get => _parent;
        set
        {
            if (_parent == value)
            {
                return;
            }

            _ = _parent?.Children.Remove(this);

            value?.Children.Add(this);
        }
    }

    public Component Root => _parent?.Root ?? this;

    private void Attach(Component parent)
    {
        if (_parent != default)
        {
            throw new InvalidOperationException();
        }

        _parent = parent;

        Invalidate();
        parent.Invalidate();

        OnAttached(parent);
    }

    private void Detach(Component parent)
    {
        if (_parent != parent)
        {
            throw new InvalidOperationException();
        }

        _parent = default;

        Invalidate();
        parent.Invalidate();

        OnDetached(parent);
    }

    protected virtual void OnAttached(Component parent)
    {

    }

    protected virtual void OnDetached(Component parent)
    {

    }

    private sealed class ChildrenNotifier : INotifierListMutationObserver<Component>
    {
        public ChildrenNotifier(Component component)
        {
            Component = component;
        }

        public Component Component { get; }

        public void OnAdd(NotifierList<Component> sender, ref Component value)
        {
            value.Attach(Component);
        }

        public void OnClear(NotifierList<Component> sender, Component[] values)
        {
            foreach (var value in values)
            {
                value.Detach(Component);
            }
        }

        public void OnInsert(NotifierList<Component> sender, int index, ref Component value)
        {
            value.Attach(Component);
        }

        public void OnRemove(NotifierList<Component> sender, ref Component value, bool result)
        {
            value.Detach(Component);
        }

        public void OnRemoveAt(NotifierList<Component> sender, int index, ref Component value)
        {
            value.Detach(Component);
        }

        public void OnSet(NotifierList<Component> sender, int index, ref Component value)
        {
            value.Attach(Component);
        }
    }
}
