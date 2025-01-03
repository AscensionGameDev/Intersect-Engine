using Ceras;
using Intersect.Logging;

using K4os.Compression.LZ4;

namespace Intersect.Network;

public partial class Ceras
{
    private static readonly string[] BuiltInPacketNamespaces = new[]
    {
        "Intersect.Network.Packets",
        "Intersect.Network.Packets.Client",
        "Intersect.Network.Packets.Editor",
        "Intersect.Network.Packets.Server",
        "Intersect.Admin.Actions"
    };

    public static List<Type> KnownTypes { get; } = FindTypes(BuiltInPacketNamespaces).ToList();

    protected CerasSerializer mSerializer;

    protected SerializerConfig mSerializerConfig;

    public Ceras(bool forNetworking = true)
    {
        if (CerasBufferPool.Pool == null)
            CerasBufferPool.Pool = new CerasDefaultBufferPool();

        mSerializerConfig = new SerializerConfig
        {
            PreserveReferences = false

        };

        mSerializerConfig.Advanced.SealTypesWhenUsingKnownTypes = forNetworking;

        if (forNetworking)
        {
            mSerializerConfig.VersionTolerance.Mode = VersionToleranceMode.Disabled;
            mSerializerConfig.KnownTypes.AddRange(KnownTypes);
            mSerializerConfig.KnownTypes.ForEach(
                knownType =>
                    mSerializerConfig.ConfigType(knownType).TypeConstruction = TypeConstruction.ByUninitialized()
            );
        }
        else
        {
            mSerializerConfig.VersionTolerance.Mode = VersionToleranceMode.Standard;
        }

        mSerializer = new CerasSerializer(mSerializerConfig);
    }

    /// <summary>
    /// Creates Ceras with instructions on how to serialize type names. This is used for migrating legacy classes to newer versions during server migrations.
    /// </summary>
    /// <param name="nameTypeMap"></param>
    public Ceras(Dictionary<string,Type> nameTypeMap)
    {
        mSerializerConfig = new SerializerConfig
        {
            PreserveReferences = false
        };

        mSerializerConfig.Advanced.SealTypesWhenUsingKnownTypes = false;
        mSerializerConfig.VersionTolerance.Mode = VersionToleranceMode.Standard;

        mSerializerConfig.Advanced.TypeBinder = new CerasTypeBinder(nameTypeMap);

        mSerializer = new CerasSerializer(mSerializerConfig);
    }

    public static void AddKnownTypes(IReadOnlyList<Type> types) => KnownTypes.AddRange(types.Where(type => !KnownTypes.Contains(type)));

    private static IEnumerable<Type> FindTypes(IEnumerable<string> nameSpaces) => nameSpaces.SelectMany(FindTypes);

    private static IEnumerable<Type> FindTypes(string nameSpace) =>
        typeof(Ceras).Assembly.GetTypes().Where(type => type.Namespace == nameSpace);

    public byte[] Serialize(object obj)
    {
        lock (mSerializer)
        {
            return mSerializer.Serialize<object>(obj);
        }
    }

    public object Deserialize(byte[] data)
    {
        try
        {
            lock (mSerializer)
            {
                return mSerializer.Deserialize<object>(data);
            }
        }
        catch (Exception exception)
        {
            LegacyLogging.Logger?.Error(exception);

            return null;
        }
    }

    public T? Deserialize<T>(byte[] data)
    {
        try
        {
            lock (mSerializer)
            {
                return mSerializer.Deserialize<T>(data);
            }
        }
        catch (Exception exception)
        {
            LegacyLogging.Logger?.Error(exception);

            return default;
        }
    }

    public byte[] Compress(object obj)
    {
        return LZ4Pickler.Pickle(Serialize(obj), LZ4Level.L00_FAST);
    }

    public object Decompress(byte[] data)
    {
        return Deserialize(LZ4Pickler.Unpickle(data));
    }

    public T? Decompress<T>(byte[] data)
    {
        return Deserialize<T>(LZ4Pickler.Unpickle(data));
    }

}