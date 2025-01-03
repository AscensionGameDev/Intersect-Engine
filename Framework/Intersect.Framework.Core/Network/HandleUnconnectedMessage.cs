using Intersect.Memory;

namespace Intersect.Network;

public delegate void HandleUnconnectedMessage(UnconnectedMessageSender sender, IBuffer message);