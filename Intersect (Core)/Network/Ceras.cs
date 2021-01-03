using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Ceras;

using Intersect.Logging;

using K4os.Compression.LZ4;

namespace Intersect.Network
{

    public class Ceras
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

        private readonly CerasSerializer mSerializer;

        private readonly SerializerConfig mSerializerConfig;

        public Ceras(bool forNetworking = true)
        {
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
                Log.Error(exception);

                return null;
            }
        }

        public T Deserialize<T>(byte[] data)
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
                Log.Error(exception);

                return default(T);
            }
        }

        public byte[] Compress(object obj)
        {
            return LZ4Pickler.Pickle(Serialize(obj), LZ4Level.L12_MAX);
        }

        public object Decompress(byte[] data)
        {
            return Deserialize(LZ4Pickler.Unpickle(data));
        }

        public T Decompress<T>(byte[] data)
        {
            return Deserialize<T>(LZ4Pickler.Unpickle(data));
        }

    }

}
