using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Ceras;
using Intersect.GameObjects.Maps;
using Intersect.Logging;

using K4os.Compression.LZ4;

namespace Intersect.Network
{

    public sealed class CerasDefaultBufferPool : ICerasBufferPool
    {
        public byte[] RentBuffer(int minimumSize)
        {
            return System.Buffers.ArrayPool<byte>.Shared.Rent(minimumSize);
        }

        public void Return(byte[] buffer)
        {
            System.Buffers.ArrayPool<byte>.Shared.Return(buffer, false);
        }
    }

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
            return LZ4Pickler.Pickle(Serialize(obj), LZ4Level.L00_FAST);
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

    public class LegacyCeras : Ceras
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

    class CerasTypeBinder : ITypeBinder
    {
        Dictionary<string, Type> _nameToType = new Dictionary<string, Type>();

        public CerasTypeBinder(Dictionary<string, Type> nameTypeMap)
        {
            _nameToType = nameTypeMap;
        }

        public string GetBaseName(Type type)
        {
            return _nameToType.Keys.FirstOrDefault(k => _nameToType[k] == type);
        }

        public Type GetTypeFromBase(string baseTypeName)
        {
            return _nameToType[baseTypeName];
        }

        public Type GetTypeFromBaseAndArguments(string baseTypeName, params Type[] genericTypeArguments)
        {
            throw new NotSupportedException("this binder is only for debugging");
        }
    }



}
