using System;

using Intersect.Client.Localization;
using Intersect.Network;

namespace Intersect.Client.Networking
{

    public static class NetworkStatusExtensions
    {

        public static string ToLocalizedString(this NetworkStatus networkStatus)
        {
            switch (networkStatus)
            {
                case NetworkStatus.Unknown:
                    return Strings.Server.Unknown;

                case NetworkStatus.Connecting:
                    return Strings.Server.Connecting;

                case NetworkStatus.Online:
                    return Strings.Server.Online;

                case NetworkStatus.Offline:
                    return Strings.Server.Offline;

                case NetworkStatus.Failed:
                    return Strings.Server.Failed;

                case NetworkStatus.VersionMismatch:
                    return Strings.Server.VersionMismatch;

                case NetworkStatus.ServerFull:
                    return Strings.Server.ServerFull;

                case NetworkStatus.HandshakeFailure:
                    return Strings.Server.HandshakeFailure;

                case NetworkStatus.Quitting:
                    return "";

                default:
                    throw new ArgumentOutOfRangeException(nameof(networkStatus), networkStatus, null);
            }
        }

    }

}
