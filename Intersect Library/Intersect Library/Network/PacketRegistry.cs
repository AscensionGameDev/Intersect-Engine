using System;
using System.Collections.Generic;

namespace Intersect.Network
{
    public sealed class PacketRegistry
    {
        private static PacketRegistry sInstance;
        public static PacketRegistry Instance
            => (sInstance ?? new PacketRegistry());
        
        private IDictionary<PacketGroups, IPacketGroup> mGroupMap;

        private PacketRegistry()
        {
            lock (typeof(PacketRegistry))
            {
                sInstance = this;
            }

            mGroupMap = new SortedDictionary<PacketGroups, IPacketGroup>();
        }

        // ReSharper disable once PossibleNullReferenceException
        public IPacketGroup GetGroup(PacketGroups group)
            => (mGroupMap.TryGetValue(group, out IPacketGroup packetGroup))
            ? packetGroup : null;

        public bool Register(IPacketGroup packetGroup)
        {
            if (mGroupMap == null) throw new ArgumentNullException();
            if (packetGroup == null) throw new ArgumentNullException();

            if (mGroupMap.ContainsKey(packetGroup.Group)) return false;
            mGroupMap.Add(packetGroup.Group, packetGroup);
            return true;
        }

        public bool Deregister(IPacketGroup packetGroup)
        {
            if (packetGroup == null) throw new ArgumentNullException();
            return Deregister(packetGroup.Group);
        }

        public bool Deregister(PacketGroups group)
        {
            if (mGroupMap == null) throw new ArgumentNullException();
            return mGroupMap.Remove(group);
        }
    }
}