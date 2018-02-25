using System;
using Intersect.Client.Classes.Localization;

namespace Intersect.Client.Network
{
    public enum NetworkStatus
    {
        Unknown = 0,
        Connecting,
        Online,
        Offline,
        Failed
    }

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

                default:
                    throw new ArgumentOutOfRangeException(nameof(networkStatus), networkStatus, null);
            }
        }
    }
}
