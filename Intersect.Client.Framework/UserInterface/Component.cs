using Intersect.Collections;

namespace Intersect.Client.Framework.UserInterface;

public abstract partial class Component
{
    protected Component(string? name = default)
    {
        Name = name;

        _childrenNotifier = new ChildrenNotifier(this);
        _children = new NotifierList<Component>(_childrenNotifier);
    }

    public string? Name { get; }
}
