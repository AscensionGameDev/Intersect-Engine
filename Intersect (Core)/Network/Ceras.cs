using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Ceras;

using Intersect.Network.Packets;
using Intersect.Network.Packets.Client;

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
    }
}
