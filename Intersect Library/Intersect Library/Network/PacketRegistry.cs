using System;
using System.Collections.Generic;

namespace Intersect.Network
{
    public sealed class PacketRegistry
    {
        private static PacketRegistry sInstance;
        public static PacketRegistry Instance
            => (sInstance ?? new PacketRegistry());
        
        private IDictionary<PacketCodes, PacketType> mLookup;

        private PacketRegistry()
        {
            lock (typeof(PacketRegistry))
            {
                sInstance = this;
            }

            mLookup = new SortedDictionary<PacketCodes, PacketType>();
        }

        // ReSharper disable once PossibleNullReferenceException
        public PacketType GetPacketType(PacketCodes code)
            => (mLookup.TryGetValue(code, out PacketType packetType))
            ? packetType : null;

        public bool Register(Type type)
        {
            return Register(PacketType.Of(type));
        }

        public bool Register(PacketType packetType)
        {
            if (mLookup == null) throw new ArgumentNullException();
            if (packetType == null) throw new ArgumentNullException();

            if (mLookup.ContainsKey(packetType.Code)) return false;
            mLookup.Add(packetType.Code, packetType);
            return true;
        }

        public bool Deregister(PacketType packetType)
        {
            if (packetType == null) throw new ArgumentNullException();
            return Deregister(packetType.Code);
        }

        public bool Deregister(PacketCodes code)
        {
            if (mLookup == null) throw new ArgumentNullException();
            return mLookup.Remove(code);
        }

        public PacketCodes GetPacketCode(PacketType packetType)
        {
            return packetType?.Code ?? PacketCodes.Unknown;
        }
    }
}