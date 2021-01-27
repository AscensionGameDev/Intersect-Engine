using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Intersect.Logging;
using Intersect.Server.Core;
using Intersect.Server.Localization;

using Lidgren.Network;

using Newtonsoft.Json;

namespace Intersect.Server.Networking.Helpers
{

    public static class NetDebug
    {

        public static void GenerateDebugFile()
        {
            Console.WriteLine(Strings.NetDebug.pleasewait);
            var hasteClient = new HasteBinClient("https://hastebin.com");
            var sb = new StringBuilder();
            sb.AppendLine("Intersect Network Diagnostics");
            sb.AppendLine();
            var externalIp = "";
            var serverAccessible = PortChecker.CanYouSeeMe(Options.ServerPort, out externalIp);
            string localIP;
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                var endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }

            sb.AppendLine("External IP (from AGD): " + externalIp);
            if (Options.UPnP && !string.IsNullOrEmpty(UpnP.GetExternalIp()))
            {
                sb.AppendLine("Routers IP (from UPnP): " + UpnP.GetExternalIp());
                if (string.IsNullOrEmpty(externalIp))
                {
                    externalIp = UpnP.GetExternalIp();
                }
            }

            sb.AppendLine("Internal IP: " + localIP);
            sb.AppendLine("Server Port: " + Options.ServerPort);
            sb.AppendLine();
            var canConnectVia127 = CheckServerPlayerCount("127.0.0.1", Options.ServerPort) > -1;
            sb.AppendLine(
                "Server Status (connecting to self via localhost: 127.0.0.1:" +
                Options.ServerPort +
                "): " +
                (canConnectVia127 ? "Online" : "Offline")
            );

            var canConnectViaInternalIp = CheckServerPlayerCount(localIP, Options.ServerPort) > -1;
            sb.AppendLine(
                "Server Status (connecting to self via internal ip: " +
                localIP +
                ":" +
                Options.ServerPort +
                "): " +
                (canConnectViaInternalIp ? "Online" : "Offline")
            );

            if (Options.UPnP && !string.IsNullOrEmpty(UpnP.GetExternalIp()))
            {
                var canConnectViaRouterIp = CheckServerPlayerCount(UpnP.GetExternalIp(), Options.ServerPort) > -1;
                sb.AppendLine(
                    "Server Status (connecting to self via router ip (from UPnP): " +
                    UpnP.GetExternalIp() +
                    ":" +
                    Options.ServerPort +
                    "): " +
                    (canConnectViaRouterIp ? "Online" : "Offline")
                );
            }

            var canConnectViaExternalIp = CheckServerPlayerCount(externalIp, Options.ServerPort) > -1;
            sb.AppendLine(
                "Server Status (connecting to self via external ip (from AGD): " +
                externalIp +
                ":" +
                Options.ServerPort +
                "): " +
                (canConnectViaExternalIp ? "Online" : "Offline")
            );

            sb.AppendLine("Server Status (as seen by AGD): " + (serverAccessible ? "Online" : "Offline"));
            sb.AppendLine();
            if (Options.UPnP)
            {
                sb.AppendLine("UPnP Log:");
                sb.AppendLine(UpnP.GetLog());
            }
            else
            {
                sb.AppendLine("UPnP: Disabled");
            }

            sb.AppendLine();
            sb.AppendLine("Trace Route to AGD");
            foreach (var line in GetTraceRoute("ascensiongamedev.com"))
            {
                sb.AppendLine(line.ToString());
            }

            var result = hasteClient.Post(sb.ToString());
            result.Wait();
            if (result.Result.IsSuccess)
            {
                Bootstrapper.MainThread.NextAction = () =>
                {
                    Console.WriteLine(Strings.NetDebug.hastebin.ToString(result.Result.FullUrl));
                };
            }
            else
            {
                Console.WriteLine(Strings.NetDebug.savedtofile);
                File.WriteAllText("netdebug.txt", sb.ToString());
            }
        }

        public static IEnumerable<IPAddress> GetTraceRoute(string hostname)
        {
            // following are the defaults for the "traceroute" command in unix.
            const int timeout = 10000;
            const int maxTTL = 30;
            const int bufferSize = 32;

            var buffer = new byte[bufferSize];
            new Random().NextBytes(buffer);
            var pinger = new Ping();

            for (var ttl = 1; ttl <= maxTTL; ttl++)
            {
                var options = new PingOptions(ttl, true);
                var reply = pinger.Send(hostname, timeout, buffer, options);

                if (reply.Status == IPStatus.Success)
                {
                    // Success means the tracert has completed
                    yield return reply.Address;

                    break;
                }

                if (reply.Status == IPStatus.TtlExpired)
                {
                    // TtlExpired means we've found an address, but there are more addresses
                    yield return reply.Address;

                    continue;
                }

                if (reply.Status == IPStatus.TimedOut)
                {
                    // TimedOut means this ttl is no good, we should continue searching
                    continue;
                }

                // if we reach here, it's a status we don't recognize and we should exit.
                break;
            }
        }

        private static int CheckServerPlayerCount(string ip, int port)
        {
            var players = -1;
            if (string.IsNullOrEmpty(ip))
            {
                return -1;
            }

            var config = new NetPeerConfiguration("AGD_CanYouSeeMee");
            var client = new NetClient(config);
            try
            {
                config.EnableMessageType(NetIncomingMessageType.UnconnectedData);
                client.Start();
                var msg = client.CreateMessage();
                msg.Write("status");

                var receiver = new IPEndPoint(NetUtility.Resolve(ip), port);

                client.SendUnconnectedMessage(msg, receiver);

                NetIncomingMessage incomingmsg;
                var watch = new Stopwatch();
                watch.Start();
                while (watch.ElapsedMilliseconds < 1250)
                {
                    while ((incomingmsg = client.ReadMessage()) != null)
                    {
                        switch (incomingmsg.MessageType)
                        {
                            case NetIncomingMessageType.UnconnectedData:
                                players = incomingmsg.ReadVariableInt32();

                                return players;
                        }

                        client.Recycle(incomingmsg);
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Warn(exception);
            }
            finally
            {
                client.Shutdown("bye");
                client = null;
            }

            return -1;
        }

    }

    public class HasteBinClient
    {

        private static HttpClient _httpClient;

        private string _baseUrl;

        static HasteBinClient()
        {
            _httpClient = new HttpClient();
        }

        public HasteBinClient(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public async Task<HasteBinResult> Post(string content)
        {
            var fullUrl = _baseUrl;
            if (!fullUrl.EndsWith("/"))
            {
                fullUrl += "/";
            }

            var postUrl = $"{fullUrl}documents";

            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(postUrl));
            request.Content = new StringContent(content);
            var result = await _httpClient.SendAsync(request);

            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                var hasteBinResult = JsonConvert.DeserializeObject<HasteBinResult>(json);

                if (hasteBinResult?.Key != null)
                {
                    hasteBinResult.FullUrl = $"{fullUrl}{hasteBinResult.Key}";
                    hasteBinResult.IsSuccess = true;
                    hasteBinResult.StatusCode = 200;

                    return hasteBinResult;
                }
            }

            return new HasteBinResult()
            {
                FullUrl = fullUrl,
                IsSuccess = false,
                StatusCode = (int) result.StatusCode
            };
        }

    }

    // Define other methods and classes here
    public class HasteBinResult
    {

        public string Key { get; set; }

        public string FullUrl { get; set; }

        public bool IsSuccess { get; set; }

        public int StatusCode { get; set; }

    }

}
