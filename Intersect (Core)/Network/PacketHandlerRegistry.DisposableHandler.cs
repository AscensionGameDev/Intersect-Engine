using System;

namespace Intersect.Network
{
    public partial class PacketHandlerRegistry
    {
        #region DisposableHandler

        private struct DisposableHandler : IDisposable
        {
            public Type PacketType { get; }

            public IDisposable Reference { get; }

            public DisposableHandler(Type packetType, IDisposable reference)
            {
                PacketType = packetType ?? throw new ArgumentNullException(nameof(packetType));
                Reference = reference ?? throw new ArgumentNullException(nameof(reference));
            }

            public void Dispose()
            {
                Reference.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }
}
