using System.Diagnostics;
using System.Resources;
using Intersect.Core;
using Intersect.Plugins.Interfaces;
using Intersect.Rsa;
using Intersect.Server.Core.Services;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using Intersect.Server.Networking.Helpers;
using Intersect.Server.Web;
using Open.Nat;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Intersect.Server.Core;

#if WEBSOCKETS
using Intersect.Server.Networking.Websockets;
#endif

internal class FullServerContext : ServerContext, IFullServerContext
{
    private const string AsymmetricKeyManifestResourceName = "Intersect.Server.network.handshake.bkey";

    internal FullServerContext(
        ServerCommandLineOptions startupOptions,
        ILogger logger,
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

        ApplicationContext.Context.Value?.Logger.LogInformation("Shutting down the console thread..." + $" ({stopwatch.ElapsedMilliseconds}ms)");
        if (!ThreadConsole.Join(1000))
        {
            try
            {
                ThreadConsole.Interrupt();
            }
            catch (ThreadAbortException threadAbortException)
            {
                ApplicationContext.Context.Value?.Logger.LogError(threadAbortException, $"{nameof(ThreadConsole)} aborted.");
            }
        }
    }

    protected override void OnDisposing(Stopwatch stopwatch)
    {
        base.OnDisposing(stopwatch);

#if WEBSOCKETS
                ApplicationContext.Context.Value?.Logger.LogInformation("Shutting down websockets..." + $" ({stopwatch.ElapsedMilliseconds}ms)");
                WebSocketNetwork.Stop();
#endif
    }

    protected override void InternalStartNetworking()
    {
        base.InternalStartNetworking();

#if WEBSOCKETS
            WebSocketNetwork.Init(Options.Instance.ServerPort);
            ApplicationContext.Context.Value?.Logger.LogInformation(Strings.Intro.websocketstarted.ToString(Options.Instance.ServerPort));
            Console.WriteLine();
#endif

        CheckNetwork();

        Console.WriteLine();
    }

        #region Networking

        internal void CheckNetwork()
        {
            Console.WriteLine();

            if (Options.Instance.OpenPortChecker && !StartupOptions.NoNetworkCheck)
            {
                if (CheckPort())
                {
                    return;
                }

                if (Options.Instance.UPnP && !StartupOptions.NoNatPunchthrough)
                {
                    ApplicationContext.Context.Value?.Logger.LogInformation(Strings.Portchecking.PortNotOpenTryingUpnp.ToString(Options.Instance.ServerPort));
                    Console.WriteLine();

                    if (TryUPnP())
                    {
                        if (CheckPort())
                        {
                            return;
                        }

                        ApplicationContext.Context.Value?.Logger.LogWarning(Strings.Portchecking.ProbablyFirewall.ToString(Options.Instance.ServerPort));
                    }
                    else
                    {
                        ApplicationContext.Context.Value?.Logger.LogWarning(Strings.Portchecking.RouterUpnpFailed);
                    }
                }
                else
                {
                    ApplicationContext.Context.Value?.Logger.LogWarning(Strings.Portchecking.PortNotOpenUpnpDisabled.ToString(Options.Instance.ServerPort));
                }
            }
            else if (Options.Instance.UPnP)
            {
                ApplicationContext.Context.Value?.Logger.LogInformation(Strings.Portchecking.TryingUpnp);
                Console.WriteLine();

                if (TryUPnP())
                {
                    ApplicationContext.Context.Value?.Logger.LogInformation(Strings.Portchecking.UpnpSucceededPortCheckerDisabled);
                }
                else
                {
                    ApplicationContext.Context.Value?.Logger.LogWarning(Strings.Portchecking.RouterUpnpFailed);
                }
            }
            else
            {
                ApplicationContext.Context.Value?.Logger.LogWarning(Strings.Portchecking.PortCheckerAndUpnpDisabled);
            }
        }

        private bool TryUPnP()
        {
            UpnP.ConnectNatDevice().Wait(5000);
#if WEBSOCKETS
            UpnP.OpenServerPort(Options.Instance.ServerPort, Protocol.Tcp).Wait(5000);
#endif
            UpnP.OpenServerPort(Options.Instance.ServerPort, Protocol.Udp).Wait(5000);

            if (UpnP.ForwardingSucceeded())
            {
                return true;
            }

            Console.WriteLine(Strings.Portchecking.CheckRouterUpnp);
            return false;
        }

        private bool CheckPort()
        {
            if (!Options.Instance.OpenPortChecker || StartupOptions.NoNetworkCheck)
            {
                return false;
            }

            var portCheckResult = PortChecker.CanYouSeeMe(Options.Instance.ServerPort, out var externalIp);

            if (!Strings.Portchecking.PortCheckerResults.TryGetValue(portCheckResult, out var portCheckResultMessage))
            {
                portCheckResultMessage = portCheckResult.ToString();
            }

            ApplicationContext.Context.Value?.Logger.LogInformation(portCheckResultMessage.ToString(Strings.Portchecking.DocumentationUrl));

            if (!string.IsNullOrEmpty(externalIp))
            {
                Console.WriteLine(Strings.Portchecking.ConnectionInfo);
                Console.WriteLine(Strings.Portchecking.PublicIp, externalIp);
                Console.WriteLine(Strings.Portchecking.PublicPort, Options.Instance.ServerPort);
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