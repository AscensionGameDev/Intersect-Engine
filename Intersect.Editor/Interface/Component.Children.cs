using Intersect.Collections;

namespace Intersect.Editor.Interface;

public partial class Component
{
    private readonly NotifierList<Editor.Interface.Component> _children;
    private readonly ChildrenNotifier _childrenNotifier;

    private Editor.Interface.Component? _parent;

    public IList<Editor.Interface.Component> Children => _children;

    public Editor.Interface.Component? Parent
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

    public Editor.Interface.Component Root => _parent?.Root ?? this;

    private void Attach(Editor.Interface.Component parent)
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

    private void Detach(Editor.Interface.Component parent)
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

    protected virtual void OnAttached(Editor.Interface.Component parent)
    {

    }

    protected virtual void OnDetached(Editor.Interface.Component parent)
    {

    }

    private sealed class ChildrenNotifier : INotifierListMutationObserver<Component>
    {
        public ChildrenNotifier(Editor.Interface.Component component)
        {
            Component = component;
        }

        public Editor.Interface.Component Component { get; }

        public void OnAdd(NotifierList<Editor.Interface.Component> sender, ref Editor.Interface.Component value)
        {
            value.Attach(Component);
        }

        public void OnClear(NotifierList<Editor.Interface.Component> sender, Editor.Interface.Component[] values)
        {
            foreach (var value in values)
            {
                value.Detach(Component);
            }
        }

        public void OnInsert(NotifierList<Editor.Interface.Component> sender, int index, ref Editor.Interface.Component value)
        {
            value.Attach(Component);
        }

        public void OnRemove(NotifierList<Editor.Interface.Component> sender, ref Editor.Interface.Component value, bool result)
        {
            value.Detach(Component);
        }

        public void OnRemoveAt(NotifierList<Editor.Interface.Component> sender, int index, ref Editor.Interface.Component value)
        {
            value.Detach(Component);
        }

        public void OnSet(NotifierList<Editor.Interface.Component> sender, int index, ref Editor.Interface.Component value)
        {
            value.Attach(Component);
        }
    }
}
