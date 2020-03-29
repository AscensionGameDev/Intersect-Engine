using System;
using System.Collections.Generic;
using System.Linq;

namespace Intersect.Network
{

    public sealed class PacketDispatcher
    {

        private readonly IDictionary<Type, IList<HandlePacket>> mHandlers;

        public PacketDispatcher()
        {
            mHandlers = new Dictionary<Type, IList<HandlePacket>>();
        }

        private IList<HandlePacket> GetHandlers(Type type)
        {
            if (mHandlers == null)
            {
                throw new ArgumentNullException();
            }

            if (type == null)
            {
                throw new ArgumentNullException();
            }

            if (!mHandlers.TryGetValue(type, out var handlers))
            {
                handlers = new List<HandlePacket>();
                mHandlers.Add(type, handlers);
            }

            return handlers;
        }

        public bool RegisterHandler(Type type, HandlePacket handler)
        {
            var handlers = GetHandlers(type);
            if (handlers?.Contains(handler) ?? false)
            {
                return false;
            }

            handlers?.Add(handler);

            return true;
        }

        public void DeregisterHandlers(Type type)
        {
            GetHandlers(type)?.Clear();
        }

        public bool DeregisterHandler(Type type, HandlePacket handler)
        {
            var handlers = GetHandlers(type);

            return (handlers?.Contains(handler) ?? false) && handlers.Remove(handler);
        }

        public bool Dispatch(IPacket packet)
        {
            if (packet == null)
            {
                throw new ArgumentNullException();
            }

            return GetHandlers(packet.GetType())?.Any(handler => handler != null && handler(null, packet)) ?? false;
        }

    }

}
