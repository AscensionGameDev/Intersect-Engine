using Newtonsoft.Json;

namespace Intersect.Metadata.Licensing;

public class LicensedComponentGroup
{
    public string? CopyrightHolder { get; set; }

    public string? CopyrightYear { get; set; }

    public List<LicensedComponent> Components { get; set; } = new();

    public LicensedComponentGroup Merge(LicensedComponentGroup other)
    {
        var filteredOtherComponents = other.Components.Where(
            otherComponent => !Components.Any(
                component => string.Equals(component.Name, otherComponent.Name, StringComparison.OrdinalIgnoreCase)
            )
        );
        Components.AddRange(filteredOtherComponents);
        Components.Sort((a, b) => a.Name.CompareTo(b.Name));
        return this;
    }
}
