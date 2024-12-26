using Ceras;

namespace Intersect.Network;

public partial class LegacyCeras : Ceras
{
    /// <summary>
    /// Creates a Ceras instance with legacy config (no version tolerance) in order for the server to use for database upgrades to the new serialized formats.
    /// </summary>
    public LegacyCeras()
    {
        mSerializerConfig = CreateLegacyConfig();
        mSerializer = new CerasSerializer(mSerializerConfig);
    }

    /// <summary>
    /// Creates a Ceras instant wtih legacy config (no version tolerance) in order for the server to use for database upgrades to new serialized formats.
    /// The dictionary parameters allow us to override what classes Ceras initializes by mapping old types to new ones.
    /// </summary>
    public LegacyCeras(Dictionary<string, Type> nameTypeMap)
    {
        mSerializerConfig = CreateLegacyConfig();

        var typeBinder = new CerasTypeBinder(nameTypeMap);
        mSerializerConfig.Advanced.TypeBinder = typeBinder;

        mSerializer = new CerasSerializer(mSerializerConfig);
    }

    private SerializerConfig CreateLegacyConfig()
    {
        var config = new SerializerConfig
        {
            PreserveReferences = false
        };

        config.Advanced.SealTypesWhenUsingKnownTypes = false;
        config.VersionTolerance.Mode = VersionToleranceMode.Disabled;

        return config;
    }
}