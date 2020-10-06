using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Ceras;
using Intersect.GameObjects.Maps;
using Intersect.Logging;

using JetBrains.Annotations;

using K4os.Compression.LZ4;

namespace Intersect.Network
{

    public class Ceras
    {

        [NotNull] protected CerasSerializer mSerializer;

        [NotNull] protected SerializerConfig mSerializerConfig;

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

                //TODO: Cleanup?
                AddKnownTypes(mSerializerConfig, "Intersect.Network.Packets");
                AddKnownTypes(mSerializerConfig, "Intersect.Network.Packets.Client");
                AddKnownTypes(mSerializerConfig, "Intersect.Network.Packets.Editor");
                AddKnownTypes(mSerializerConfig, "Intersect.Network.Packets.Server");
                AddKnownTypes(mSerializerConfig, "Intersect.Admin.Actions");
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

        private void AddKnownTypes(SerializerConfig config, string nameSpce)
        {
            var packetTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == nameSpce).ToList();
            foreach (var typ in packetTypes)
            {
                config.KnownTypes.Add(typ);
                mSerializerConfig.ConfigType(typ).TypeConstruction = TypeConstruction.ByUninitialized();
            }
        }

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
