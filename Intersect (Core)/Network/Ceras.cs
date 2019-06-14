using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Ceras;

using Intersect.Network.Packets;
using Intersect.Network.Packets.Client;

using K4os.Compression.LZ4;

namespace Intersect.Network
{
    public class Ceras
    {
        private SerializerConfig mSerializerConfig;
        private CerasSerializer mSerializer;

        public Ceras(bool forNetworking = true)
        {
            mSerializerConfig = new SerializerConfig();
            mSerializerConfig.PreserveReferences = false;

            if (forNetworking)
            {
                mSerializerConfig.VersionTolerance.Mode = VersionToleranceMode.Disabled;
                mSerializerConfig.Advanced.SealTypesWhenUsingKnownTypes = forNetworking;

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
            return mSerializer.Serialize<object>(obj);
        }

        public object Deserialize(byte[] data)
        {
            return mSerializer.Deserialize<object>(data);
        }

        public T Deserialize<T>(byte[] data)
        {
            return mSerializer.Deserialize<T>(data);
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
