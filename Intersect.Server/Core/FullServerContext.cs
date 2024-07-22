using System.Diagnostics;
using System.Resources;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Plugins.Interfaces;
using Intersect.Rsa;
using Intersect.Server.Core.Services;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using Intersect.Server.Networking.Helpers;
using Intersect.Server.Networking.LiteNetLib;
using Intersect.Server.Web;
using Open.Nat;

namespace Intersect.Server.Core;

#if WEBSOCKETS
using Intersect.Server.Networking.Websockets;
#endif

internal class FullServerContext : ServerContext, IFullServerContext
{
    private const string AsymmetricKeyManifestResourceName = "Intersect.Server.network.handshake.bkey";

    internal FullServerContext(
        ServerCommandLineOptions startupOptions,
        Logger logger,
        IPacketHelper packetHelper
    ) : base(startupOptions, logger, packetHelper)
    {
        packetHelper.HandlerRegistry.TryRegisterAvailableMethodHandlers(typeof(NetworkedPacketHandler), new NetworkedPacketHandler(), false);
        packetHelper.HandlerRegistry.TryRegisterAvailableTypeHandlers(typeof(FullServerContext).Assembly);
    }

    public IApiService ApiService => GetExpectedService<IApiService>();

    public IConsoleService ConsoleService => GetExpectedService<IConsoleService>();

    private Thread ThreadConsole => ConsoleService.Thread;

    protected override RsaKey AsymmetricKey
    {
        get {
            using var asymmetricKeyStream = typeof(FullServerContext).Assembly.GetManifestResourceStream(AsymmetricKeyManifestResourceName);
            if (asymmetricKeyStream == default)
            {
                throw new MissingManifestResourceException(
                    $"Unable to get manifest resource stream for '{AsymmetricKeyManifestResourceName}'"
                );
            }

            RsaKey rsaKey = new(asymmetricKeyStream);
            return rsaKey;
        }
    }

    public override void WaitForConsole()
    {
        base.WaitForConsole();

        ConsoleService.Wait(true);
    }

    protected override void JoinOrKillConsoleThread(Stopwatch stopwatch)
    {
        base.JoinOrKillConsoleThread(stopwatch);

        if (!(ThreadConsole?.IsAlive ?? false))
        {
            return;
        }

        Log.Info("Shutting down the console thread..." + $" ({stopwatch.ElapsedMilliseconds}ms)");
        if (!ThreadConsole.Join(1000))
        {
            try
            {
                ThreadConsole.Interrupt();
            }
            catch (ThreadAbortException threadAbortException)
            {
                Log.Error(threadAbortException, $"{nameof(ThreadConsole)} aborted.");
            }
        }
    }

    protected override void OnDisposing(Stopwatch stopwatch)
    {
        base.OnDisposing(stopwatch);

#if WEBSOCKETS
                Log.Info("Shutting down websockets..." + $" ({stopwatch.ElapsedMilliseconds}ms)");
                WebSocketNetwork.Stop();
#endif
    }

    protected override void InternalStartNetworking()
    {
        base.InternalStartNetworking();

#if WEBSOCKETS
            WebSocketNetwork.Init(Options.ServerPort);
            Log.Pretty.Info(Strings.Intro.websocketstarted.ToString(Options.ServerPort));
            Console.WriteLine();
#endif

        CheckNetwork();

        Console.WriteLine();
    }

        #region Networking

        internal void CheckNetwork()
        {
            Console.WriteLine();

            if (Options.OpenPortChecker && !StartupOptions.NoNetworkCheck)
            {
                if (CheckPort())
                {
                    return;
                }

                if (Options.UPnP && !StartupOptions.NoNatPunchthrough)
                {
                    Log.Pretty.Info(Strings.Portchecking.PortNotOpenTryingUpnp.ToString(Options.ServerPort));
                    Console.WriteLine();

                    if (TryUPnP())
                    {
                        if (CheckPort())
                        {
                            return;
                        }

                        Log.Pretty.Warn(Strings.Portchecking.ProbablyFirewall.ToString(Options.ServerPort));
                    }
                    else
                    {
                        Log.Pretty.Warn(Strings.Portchecking.RouterUpnpFailed);
                    }
                }
                else
                {
                    Log.Pretty.Warn(Strings.Portchecking.PortNotOpenUpnpDisabled.ToString(Options.ServerPort));
                }
            }
            else if (Options.UPnP)
            {
                Log.Pretty.Info(Strings.Portchecking.TryingUpnp);
                Console.WriteLine();

                if (TryUPnP())
                {
                    Log.Pretty.Info(Strings.Portchecking.UpnpSucceededPortCheckerDisabled);
                }
                else
                {
                    Log.Pretty.Warn(Strings.Portchecking.RouterUpnpFailed);
                }
            }
            else
            {
                Log.Pretty.Warn(Strings.Portchecking.PortCheckerAndUpnpDisabled);
            }
        }

        private bool TryUPnP()
        {
            UpnP.ConnectNatDevice().Wait(5000);
#if WEBSOCKETS
            UpnP.OpenServerPort(Options.ServerPort, Protocol.Tcp).Wait(5000);
#endif
            UpnP.OpenServerPort(Options.ServerPort, Protocol.Udp).Wait(5000);

            if (UpnP.ForwardingSucceeded())
            {
                return true;
            }

            Console.WriteLine(Strings.Portchecking.CheckRouterUpnp);
            return false;
        }

        private bool CheckPort()
        {
            if (!Options.OpenPortChecker || StartupOptions.NoNetworkCheck)
            {
                return false;
            }

            var portCheckResult = PortChecker.CanYouSeeMe(Options.ServerPort, out var externalIp);

            if (!Strings.Portchecking.PortCheckerResults.TryGetValue(portCheckResult, out var portCheckResultMessage))
            {
                portCheckResultMessage = portCheckResult.ToString();
            }

            Log.Pretty.Info(portCheckResultMessage.ToString(Strings.Portchecking.DocumentationUrl));

            if (!string.IsNullOrEmpty(externalIp))
            {
                Console.WriteLine(Strings.Portchecking.ConnectionInfo);
                Console.WriteLine(Strings.Portchecking.PublicIp, externalIp);
                Console.WriteLine(Strings.Portchecking.PublicPort, Options.ServerPort);
            }

            if (portCheckResult == PortCheckResult.Inaccessible)
            {
                Console.WriteLine();

                Console.WriteLine(Strings.Portchecking.DebuggingSteps);
                Console.WriteLine(Strings.Portchecking.CheckFirewalls);
                Console.WriteLine(Strings.Portchecking.AntivirusCheck);
                Console.WriteLine(Strings.Portchecking.WithinRestrictedNetwork);
            }

            Console.WriteLine();

            return portCheckResult == PortCheckResult.Open;
        }

        #endregion
}