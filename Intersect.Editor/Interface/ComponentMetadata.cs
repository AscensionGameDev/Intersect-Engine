using System.Runtime.CompilerServices;

namespace Intersect.Editor.Interface;

internal partial class ComponentMetadata
{
    private static readonly ConditionalWeakTable<Editor.Interface.Component, ComponentMetadata> _componentMetadata = new();

    public ComponentMetadata(Editor.Interface.Component component)
    {
        _componentMetadata.Add(component, this);
        Id = $"{component.GetType().FullName}_{component.Name}_{Guid.NewGuid()}";
    }

    public string Id { get; }
}
