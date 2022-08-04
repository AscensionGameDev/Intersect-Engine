namespace Intersect.Metadata.Licensing;

public sealed class LicensedComponent
{
    public string? CopyrightHolder { get; set; }

    public string? CopyrightYear { get; set; }

    public LicenseType License { get; set; }

    public string Name { get; set; }
}
