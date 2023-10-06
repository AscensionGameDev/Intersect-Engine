using LiteNetLib;

namespace Intersect.Network.LiteNetLib;

public static class TransmissionModeExtensions
{
    public static DeliveryMethod ToDeliveryMethod(this TransmissionMode transmissionMode) =>
        transmissionMode switch
        {
            TransmissionMode.All => DeliveryMethod.ReliableOrdered,
            TransmissionMode.Latest => DeliveryMethod.Sequenced,
            TransmissionMode.Any => DeliveryMethod.Unreliable,
            _ => throw new ArgumentOutOfRangeException(nameof(transmissionMode), transmissionMode, null),
        };
}