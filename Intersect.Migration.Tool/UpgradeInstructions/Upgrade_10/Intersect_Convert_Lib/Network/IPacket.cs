using System;
using Intersect.Migration.UpgradeInstructions.Upgrade_10.Intersect_Convert_Lib.Memory;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_10.Intersect_Convert_Lib.Network
{
    public interface IPacket : IDisposable
    {
        IConnection Connection { get; }

        double Timestamp { get; }

        int EstimatedSize { get; }

        PacketCode Code { get; }

        PacketType Type { get; }

        bool Read(ref IBuffer buffer);
        bool Write(ref IBuffer buffer);
    }
}