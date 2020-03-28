using System;
using System.Linq;
using System.Reflection;

using Ceras;

using Intersect.Logging;

using JetBrains.Annotations;

using K4os.Compression.LZ4;

namespace Intersect.Network
{

    public class Ceras
    {

        [NotNull] private readonly CerasSerializer mSerializer;

        [NotNull] private readonly SerializerConfig mSerializerConfig;

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

}
