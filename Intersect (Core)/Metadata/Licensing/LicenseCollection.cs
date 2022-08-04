using System.Reflection;
using System.Text;

using Intersect.Reflection;

using Newtonsoft.Json;

namespace Intersect.Metadata.Licensing;

public class LicenseCollection : Dictionary<string, LicensedComponentGroup>
{
    public LicenseCollection Merge(LicenseCollection other)
    {
        foreach (var kvp in other)
        {
            if (TryGetValue(kvp.Key, out var existingGroup))
            {
                _ = existingGroup.Merge(kvp.Value);
            }
            else
            {
                Add(kvp.Key, kvp.Value);
            }
        }

        return this;
    }

    public static LicenseCollection FromJson(string json)
    {
        LicenseCollection licenseCollection;
        try
        {
            licenseCollection = JsonConvert.DeserializeObject<LicenseCollection>(json);
        }
        catch
        {
            throw;
        }

        if (licenseCollection == default)
        {
            throw new ArgumentException("JSON input resolves to null dictionary.", nameof(json));
        }

        return licenseCollection;
    }

    public static LicenseCollection FromJson(Stream stream, Encoding? encoding = default, bool leaveOpen = false)
    {
        using var reader = new StreamReader(
            stream,
            encoding: encoding,
            leaveOpen: leaveOpen
        );

        var json = reader.ReadToEnd();
        var licenseCollection = FromJson(json);
        return licenseCollection;
    }

    public static LicenseCollection FromAssembly(Assembly assembly)
    {
        if (!assembly.TryFindResource("licenses.json", out var manifestResourceName))
        {
            return new LicenseCollection();
        }

        var manifestResourceStream = assembly.GetManifestResourceStream(manifestResourceName);
        return FromJson(manifestResourceStream);
    }

    public static LicenseCollection FromLoadedAssemblies()
    {
        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        var aggregateCollection = loadedAssemblies.Aggregate(
            new LicenseCollection(),
            (currentCollection, assembly) => currentCollection.Merge(FromAssembly(assembly))
        );
        return aggregateCollection;
    }
}
