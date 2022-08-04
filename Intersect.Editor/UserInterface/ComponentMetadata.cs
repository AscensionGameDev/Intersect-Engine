using System.Runtime.CompilerServices;


namespace Intersect.Client.Framework.UserInterface;

internal partial class ComponentMetadata
{
    private static readonly ConditionalWeakTable<Component, ComponentMetadata> _componentMetadata = new();

    public ComponentMetadata(Component component)
    {
        _componentMetadata.Add(component, this);
        Id = $"{component.GetType().FullName}_{component.Name}_{Guid.NewGuid()}";
    }

    public string Id { get; }
}
