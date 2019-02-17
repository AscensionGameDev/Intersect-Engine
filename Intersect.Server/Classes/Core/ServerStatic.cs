#define websockets

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Intersect.Config;
using Intersect.Enums;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Crypto;
using Intersect.Network.Crypto.Formats;
using Intersect.Server.Core;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using Intersect.Server.Networking.Helpers;
using Intersect.Server.Networking.Lidgren;
#if websockets
using Intersect.Server.Networking.Websockets;
#endif
using Intersect.Server.WebApi;
using Intersect.Utilities;
using JetBrains.Annotations;
using Open.Nat;

namespace Intersect.Server
{
    internal static class ServerStatic
    {
        public static void Start(string[] args)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;

            LegacyDatabase.CheckDirectories();

            Strings.Load();

            if (!Options.LoadFromDisk())
            {
                Console.WriteLine(Strings.Errors.errorloadingconfig);
                Console.ReadKey();
                return;
            }

            foreach (var arg in args)
                if (arg.Contains("port="))
                {
                    var port = Options.ServerPort;
                    if (ushort.TryParse(arg.Split("=".ToCharArray())[1], out port)) Options.ServerPort = port;
                }

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine(@"  _____       _                          _   ");
            Console.WriteLine(@" |_   _|     | |                        | |  ");
            Console.WriteLine(@"   | |  _ __ | |_ ___ _ __ ___  ___  ___| |_ ");
            Console.WriteLine(@"   | | | '_ \| __/ _ \ '__/ __|/ _ \/ __| __|");
            Console.WriteLine(@"  _| |_| | | | ||  __/ |  \__ \  __/ (__| |_ ");
            Console.WriteLine(@" |_____|_| |_|\__\___|_|  |___/\___|\___|\__|");
            Console.WriteLine(Strings.Intro.tagline);
            Console.WriteLine(@"Copyright (C) 2018  Ascension Game Dev");
            Console.WriteLine(Strings.Intro.version.ToString(Assembly.GetExecutingAssembly().GetName().Version));
            Console.WriteLine(Strings.Intro.support);
            Console.WriteLine(Strings.Intro.loading);
            Formulas.LoadFormulas();
            ExportDependencies(args);
            if (!LegacyDatabase.InitDatabase())
            {
                Console.ReadKey();
                return;
            }

            CustomColors.Load();
            Console.WriteLine(Strings.Commandoutput.playercount.ToString(LegacyDatabase.RegisteredPlayers));
            Console.WriteLine(Strings.Commandoutput.gametime.ToString(ServerTime.GetTime().ToString("F")));
            ServerTime.Update();
            Log.Global.AddOutput(new ConsoleOutput(Debugger.IsAttached ? LogLevel.All : LogLevel.Error));
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("Intersect.Server.private-intersect.bek"))
            {
                var rsaKey = EncryptionKey.FromStream<RsaKey>(stream);
                Debug.Assert(rsaKey != null, "rsaKey != null");
                Globals.Network = new ServerNetwork(new NetworkConfiguration(Options.ServerPort), rsaKey.Parameters);
            }

            var packetHandler = new PacketHandler();
            Globals.Network.Handlers[PacketCode.BinaryPacket] = packetHandler.HandlePacket;

#if websockets
            WebSocketNetwork.Init(Options.ServerPort);
            Console.WriteLine(Strings.Intro.websocketstarted.ToString(Options.ServerPort));
#endif

            if (!Globals.Network.Listen()) Log.Error("An error occurred while attempting to connect.");

            Console.WriteLine();

            if (Options.UPnP && !args.Contains("noupnp"))
            {
                UpnP.ConnectNatDevice().Wait(5000);
#if websockets
                UpnP.OpenServerPort(Options.ServerPort, Protocol.Tcp).Wait(5000);
#endif
                UpnP.OpenServerPort(Options.ServerPort, Protocol.Udp).Wait(5000);

                if (Options.ApiEnabled) UpnP.OpenServerPort(Options.ApiPort, Protocol.Tcp).Wait(5000);

                Console.WriteLine();
            }

            if (Options.ApiEnabled)
            {
                Console.WriteLine(Strings.Intro.api.ToString(Options.ApiPort));
                Globals.Api = new ServerApi(Options.ApiPort);
                Globals.Api.Start();
                Console.WriteLine();
                DeleteIfExists("Nancy.dll");
            }

            //Check to see if AGD can see this server. If so let the owner know :)
            if (Options.OpenPortChecker && !args.Contains("noportcheck"))
            {
                var externalIp = "";
                var serverAccessible = PortChecker.CanYouSeeMe(Options.ServerPort, out externalIp);

                Console.WriteLine(Strings.Portchecking.connectioninfo);
                if (!string.IsNullOrEmpty(externalIp))
                {
                    Console.WriteLine(Strings.Portchecking.publicip, externalIp);
                    Console.WriteLine(Strings.Portchecking.publicport, Options.ServerPort);

                    Console.WriteLine();
                    if (serverAccessible)
                    {
                        Console.WriteLine(Strings.Portchecking.accessible);
                        Console.WriteLine(Strings.Portchecking.letothersjoin);
                    }
                    else
                    {
                        Console.WriteLine(Strings.Portchecking.notaccessible);
                        Console.WriteLine(Strings.Portchecking.debuggingsteps);
                        Console.WriteLine(Strings.Portchecking.checkfirewalls);
                        Console.WriteLine(Strings.Portchecking.checkantivirus);
                        Console.WriteLine(Strings.Portchecking.screwed);
                        Console.WriteLine();
                        if (!UpnP.ForwardingSucceeded())
                            Console.WriteLine(Strings.Portchecking.checkrouterupnp);
                    }
                }
                else
                {
                    Console.WriteLine(Strings.Portchecking.notconnected);
                }

                Console.WriteLine();
            }

            Console.WriteLine(Strings.Intro.started.ToString(Options.ServerPort));

            ServerContext.Instance.Start();

            var logicThread = new Thread(ServerLoop.RunServerLoop);
            logicThread.Start();
            if (ServerContext.Instance.StartupOptions.DoNotShowConsole)
            {
                return;
            }

            /*Console.WriteLine(Strings.Intro.consoleactive);
            Console.Write("> ");
            var command = Console.ReadLine();
            while (true)
            {
                var userFound = false;
                var ip = "";
                if (command == null)
                {
                    Shutdown();
                    return;
                }

                command = command.Trim();
                var commandsplit = command.Split(' ');

                if (commandsplit[0] == Strings.Commands.announcement) //Announcement Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.announcementusage.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.announcementdesc);
                        }
                        else
                        {
                            PacketSender.SendGlobalMsg(command.Remove(0, 12));
                        }
                    }
                    else
                    {
                        Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                    }
                }
                else if (commandsplit[0] == Strings.Commands.netdebug) //Output network debug info
                {
                    NetDebug.GenerateDebugFile();
                }
                else if (commandsplit[0] == Strings.Commands.onlinelist) //Online List Command
                {
                    Console.WriteLine(string.Format("{0,-10}", Strings.Commandoutput.listid) + string.Format("{0,-28}", Strings.Commandoutput.listaccount) + string.Format("{0,-28}", Strings.Commandoutput.listcharacter));
                    Console.WriteLine(new string('-', 66));
                    for (var i = 0; i < Globals.Clients.Count; i++)
                        if (Globals.Clients[i] != null)
                        {
                            var name = Globals.Clients[i].Entity != null ? Globals.Clients[i].Entity.Name : "";
                            Console.WriteLine(string.Format("{0,-10}", "#" + i) + string.Format("{0,-28}", Globals.Clients[i].Name) + string.Format("{0,-28}", name));
                        }
                }
                else if (commandsplit[0] == Strings.Commands.kill) //Kill Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.killusage.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.killdesc);
                        }
                        else
                        {
                            for (var i = 0; i < Globals.Clients.Count; i++)
                                if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                                {
                                    var user = Globals.Clients[i].Entity.Name.ToLower();
                                    if (user == commandsplit[1].ToLower())
                                    {
                                        Globals.Clients[i].Entity.Die();
                                        PacketSender.SendGlobalMsg(@"    " + Strings.Player.serverkilled.ToString(Globals.Clients[i].Entity.Name));
                                        Console.WriteLine(@"    " + Strings.Commandoutput.killsuccess.ToString(Globals.Clients[i].Entity.Name));
                                        userFound = true;
                                        break;
                                    }
                                }

                            if (userFound == false) Console.WriteLine(@"    " + Strings.Player.offline);
                        }
                    }
                    else
                    {
                        Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                    }
                }
                else if (commandsplit[0] == Strings.Commands.kick) //Kick Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.kickusage.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.kickdesc);
                        }
                        else
                        {
                            for (var i = 0; i < Globals.Clients.Count; i++)
                                if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                                {
                                    var user = Globals.Clients[i].Entity.Name.ToLower();
                                    if (user == commandsplit[1].ToLower())
                                    {
                                        PacketSender.SendGlobalMsg(Strings.Player.serverkicked.ToString(Globals.Clients[i].Entity.Name));
                                        Console.WriteLine(@"    " + Strings.Player.serverkicked.ToString(Globals.Clients[i].Entity.Name));
                                        Globals.Clients[i].Disconnect(); //Kick em'
                                        userFound = true;
                                        break;
                                    }
                                }

                            if (userFound == false) Console.WriteLine(@"    " + Strings.Player.offline);
                        }
                    }
                    else
                    {
                        Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                    }
                }
                else if (commandsplit[0] == Strings.Commands.unban) //Unban Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.unbanusage.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.unbandesc);
                        }
                        else
                        {
                            var unbannedUser = LegacyDatabase.GetUser(commandsplit[1]);
                            if (unbannedUser != null)
                            {
                                Ban.DeleteBan(unbannedUser);
                                Console.WriteLine(@"    " + Strings.Account.unbanned.ToString(commandsplit[1]));
                            }
                            else
                            {
                                Console.WriteLine("    " + Strings.Account.notfound.ToString(commandsplit[1]));
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                    }
                }
                else if (commandsplit[0] == Strings.Commands.ban) //Ban Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.banusage.ToString(Strings.Commands.True, Strings.Commands.False, Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.bandesc);
                        }
                        else
                        {
                            if (commandsplit.Length > 3)
                            {
                                for (var i = 0; i < Globals.Clients.Count; i++)
                                    if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                                    {
                                        var user = Globals.Clients[i].Entity.Name.ToLower();
                                        if (user == commandsplit[1].ToLower())
                                        {
                                            var reason = "";
                                            for (var n = 4; n < commandsplit.Length; n++) reason += commandsplit[n] + " ";
                                            if (commandsplit[3] == Strings.Commands.True) ip = Globals.Clients[i].GetIp();
                                            Ban.AddBan(Globals.Clients[i], Convert.ToInt32(commandsplit[2]), reason, Strings.Commands.banuser, ip);
                                            PacketSender.SendGlobalMsg(Strings.Account.banned.ToString(Globals.Clients[i].Entity.Name));
                                            Console.WriteLine(@"    " + Strings.Account.banned.ToString(Globals.Clients[i].Entity.Name));
                                            Globals.Clients[i].Disconnect(); //Kick em'
                                            userFound = true;
                                            break;
                                        }
                                    }

                                if (userFound == false) Console.WriteLine(@"    " + Strings.Player.offline);
                            }
                            else
                            {
                                Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                    }
                }
                else if (commandsplit[0] == Strings.Commands.unmute) //Unmute Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.unmuteusage.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.unmutedesc);
                        }
                        else
                        {
                            var unmutedUser = LegacyDatabase.GetUser(commandsplit[1]);
                            if (unmutedUser != null)
                            {
                                Mute.DeleteMute(unmutedUser);
                                Console.WriteLine(@"    " + Strings.Account.unmuted.ToString(unmutedUser.Name));
                            }
                            else
                            {
                                Console.WriteLine(@"    " + Strings.Account.notfound);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                    }
                }
                else if (commandsplit[0] == Strings.Commands.mute) //Mute Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.muteusage.ToString(Strings.Commands.True, Strings.Commands.False, Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.mutedesc);
                        }
                        else
                        {
                            if (commandsplit.Length > 3)
                            {
                                for (var i = 0; i < Globals.Clients.Count; i++)
                                    if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                                    {
                                        var user = Globals.Clients[i].Entity.Name.ToLower();
                                        if (user == commandsplit[1].ToLower())
                                        {
                                            var reason = "";
                                            for (var n = 4; n < commandsplit.Length; n++) reason += commandsplit[n] + " ";
                                            if (commandsplit[3] == Strings.Commands.True) ip = Globals.Clients[i].GetIp();
                                            Mute.AddMute(Globals.Clients[i], Convert.ToInt32(commandsplit[2]), reason, Strings.Commands.muteuser, ip);
                                            PacketSender.SendGlobalMsg(Strings.Account.muted.ToString(Globals.Clients[i].Entity.Name));
                                            Console.WriteLine(@"    " + Strings.Account.muted.ToString(Globals.Clients[i].Entity.Name));
                                            userFound = true;
                                            break;
                                        }
                                    }

                                if (userFound == false) Console.WriteLine(@"    " + Strings.Player.offline);
                            }
                            else
                            {
                                Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                    }
                }
                else if (commandsplit[0] == Strings.Commands.power) //Power Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.powerusage.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.powerdesc);
                        }
                        else
                        {
                            if (commandsplit.Length > 2)
                            {
                                for (var i = 0; i < Globals.Clients.Count; i++)
                                    if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                                    {
                                        var user = Globals.Clients[i].Entity.Name.ToLower();
                                        if (user == commandsplit[1].ToLower())
                                        {
                                            var power = UserRights.None;
                                            switch ((Access)int.Parse(commandsplit[2]))
                                            {
                                                case Access.Moderator:
                                                    power = UserRights.Moderation;
                                                    break;
                                                case Access.Admin:
                                                    power = UserRights.Admin;
                                                    break;
                                            }
                                            LegacyDatabase.SetPlayerPower(Globals.Clients[i].Name, power);
                                            PacketSender.SendEntityDataToProximity(Globals.Clients[i].Entity);
                                            if (power != UserRights.None)
                                                PacketSender.SendGlobalMsg(Strings.Player.admin.ToString(Globals.Clients[i].Entity.Name));
                                            else
                                                PacketSender.SendGlobalMsg(Strings.Player.deadmin.ToString(Globals.Clients[i].Entity.Name));
                                            Console.WriteLine(@"    " + Strings.Commandoutput.powerchanged.ToString(Globals.Clients[i].Entity.Name));

                                            userFound = true;
                                            break;
                                        }
                                    }

                                if (userFound == false) Console.WriteLine(@"    " + Strings.Player.offline);
                            }
                            else
                            {
                                Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                    }
                }
                else if (commandsplit[0] == Strings.Commands.makeprivate)
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.api.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.makeprivatedesc);
                        }
                        else
                        {
                            Console.WriteLine(@"    " + Strings.Commands.makeprivateusage);
                        }
                    }
                    else
                    {
                        Console.WriteLine(@"    " + Strings.Commands.madeprivate);
                        Options.AdminOnly = true;
                        Options.SaveToDisk();
                    }
                }
                else if (commandsplit[0] == Strings.Commands.makepublic)
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.makepublicdesc);
                        }
                        else
                        {
                            Console.WriteLine(@"    " + Strings.Commands.makepublicusage);
                        }
                    }
                    else
                    {
                        Console.WriteLine(@"    " + Strings.Commands.madepublic);
                        Options.AdminOnly = false;
                        Options.SaveToDisk();
                    }
                }
                else if (commandsplit[0] == Strings.Commands.api) //API Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.apidesc);
                        }
                        else
                        {
                            if (commandsplit.Length > 2)
                            {
                                if (commandsplit.Length > 2)
                                    try
                                    {
                                        if (LegacyDatabase.AccountExists(commandsplit[1]))
                                        {
                                            var access = Convert.ToBoolean(int.Parse(commandsplit[2]));
                                            var account = LegacyDatabase.GetUser(commandsplit[1]);
                                            account.Power.Api = access;
                                            if (access)
                                            {
                                                Console.WriteLine(@"    " + Strings.Commandoutput.apigranted.ToString(commandsplit[1]));
                                            }
                                            else
                                            {
                                                Console.WriteLine(@"    " + Strings.Commandoutput.apirevoked.ToString(commandsplit[1]));
                                            }
                                            LegacyDatabase.SavePlayerDatabaseAsync();
                                        }
                                        else
                                        {
                                            Console.WriteLine(@"    " + Strings.Account.notfound.ToString(commandsplit[1]));
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        Console.WriteLine(@"    " + Strings.Commandoutput.parseerror.ToString(commandsplit[0], Strings.Commands.commandinfo));
                                    }
                                else
                                    Console.WriteLine(@"    " + Strings.Commandoutput.syntaxerror.ToString(commandsplit[0], Strings.Commands.commandinfo));
                            }
                            else
                            {
                                Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                    }
                }
                else if (commandsplit[0] == Strings.Commands.poweracc) //Power Account Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.poweraccusage.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.poweraccdesc);
                        }
                        else
                        {
                            if (commandsplit.Length > 2)
                            {
                                if (commandsplit.Length > 2)
                                    try
                                    {
                                        if (LegacyDatabase.AccountExists(commandsplit[1]))
                                        {
                                            var power = UserRights.None;
                                            switch ((Access)int.Parse(commandsplit[2]))
                                            {
                                                case Access.Moderator:
                                                    power = UserRights.Moderation;
                                                    break;
                                                case Access.Admin:
                                                    power = UserRights.Admin;
                                                    break;
                                            }
                                            LegacyDatabase.SetPlayerPower(commandsplit[1], power);
                                            Console.WriteLine(@"    " + Strings.Commandoutput.powerchanged.ToString(commandsplit[1]));
                                        }
                                        else
                                        {
                                            Console.WriteLine(@"    " + Strings.Account.notfound.ToString(commandsplit[1]));
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        Console.WriteLine(@"    " + Strings.Commandoutput.parseerror.ToString(commandsplit[0], Strings.Commands.commandinfo));
                                    }
                                else
                                    Console.WriteLine(@"    " + Strings.Commandoutput.syntaxerror.ToString(commandsplit[0], Strings.Commands.commandinfo));
                            }
                            else
                            {
                                Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                    }
                }
                else if (commandsplit[0] == Strings.Commands.cps) //CPS Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.cpsusage.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.cpsdesc);
                        }
                        else if (commandsplit[1] == Strings.Commands.cpslock)
                        {
                            Globals.CpsLock = true;
                        }
                        else if (commandsplit[1] == Strings.Commands.cpsunlock)
                        {
                            Globals.CpsLock = false;
                        }
                        else if (commandsplit[1] == Strings.Commands.cpsstatus)
                        {
                            if (Globals.CpsLock)
                                Console.WriteLine(Strings.Commandoutput.cpslocked);
                            else
                                Console.WriteLine(Strings.Commandoutput.cpsunlocked);
                        }
                        else
                        {
                            Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                        }
                    }
                    else
                    {
                        Console.WriteLine(Strings.Commandoutput.cps.ToString(Globals.Cps));
                    }
                }
                else if (commandsplit[0] == Strings.Commands.exit) //Exit Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.exitusage.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.exitdesc);
                        }
                        else
                        {
                            Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                        }
                    }
                    else
                    {
                        Shutdown();
                        return;
                    }
                }
                else if (commandsplit[0] == Strings.Commands.migrate) //Migrate Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.migrateusage.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.migratedesc);
                        }
                        else
                        {
                            Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                        }
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine(Strings.Migration.selectdb);
                        Console.WriteLine();
                        Console.WriteLine(Strings.Migration.selectgamedb.ToString(Options.GameDb.Type == DatabaseOptions.DatabaseType.sqlite ? Strings.Migration.currentlysqlite : Strings.Migration.currentlymysql));
                        Console.WriteLine(Strings.Migration.selectplayerdb.ToString(Options.PlayerDb.Type == DatabaseOptions.DatabaseType.sqlite ? Strings.Migration.currentlysqlite : Strings.Migration.currentlymysql));
                        Console.WriteLine();
                        Console.WriteLine(Strings.Migration.cancel);
                        Console.Write("> ");
                        var selection = Console.ReadKey().KeyChar;
                        Console.WriteLine();
                        DatabaseOptions db = null;
                        if (selection.ToString() == Strings.Migration.selectgamedbkey.ToString())
                        {
                            db = Options.GameDb;
                        }
                        else if (selection.ToString() == Strings.Migration.selectplayerdbkey.ToString())
                        {
                            db = Options.PlayerDb;
                        }
                        if (db != null)
                        {
                            var dbString = db == Options.GameDb ? Strings.Migration.gamedb : Strings.Migration.playerdb;
                            Console.WriteLine();
                            Console.WriteLine(Strings.Migration.selectdbengine.ToString(dbString));
                            Console.WriteLine(Strings.Migration.migratetosqlite);
                            Console.WriteLine(Strings.Migration.migratetomysql);
                            Console.WriteLine();
                            Console.WriteLine(Strings.Migration.cancel);
                            Console.Write("> ");
                            selection = Console.ReadKey().KeyChar;
                            Console.WriteLine();
                            DatabaseOptions.DatabaseType dbengine = DatabaseOptions.DatabaseType.sqlite;
                            if (selection.ToString() == Strings.Migration.selectsqlitekey.ToString() || selection.ToString() == Strings.Migration.selectmysqlkey.ToString())
                            {
                                if (selection.ToString() == Strings.Migration.selectmysqlkey.ToString()) dbengine = DatabaseOptions.DatabaseType.mysql;
                                if (db.Type == dbengine)
                                {
                                    var engineString = dbengine == DatabaseOptions.DatabaseType.sqlite ? Strings.Migration.sqlite : Strings.Migration.mysql;
                                    Console.WriteLine();
                                    Console.WriteLine(Strings.Migration.alreadyusingengine.ToString(dbString, engineString));
                                    Console.WriteLine();
                                }
                                else
                                {
                                    LegacyDatabase.Migrate(db, dbengine);
                                }
                            }
                        }


                    }
                }
                else if (commandsplit[0] == Strings.Commands.help) //Help Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.helpusage.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.helpdesc);
                        }
                        else
                        {
                            Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                        }
                    }
                    else
                    {
                        Console.WriteLine(@"    " + Strings.Commandoutput.helpheader);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.help) + " - " + Strings.Commands.helphelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.exit) + " - " + Strings.Commands.exithelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.api) + " - " + Strings.Commands.apihelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.announcement) + " - " + Strings.Commands.announcementhelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.cps) + " - " + Strings.Commands.cpshelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.power) + " - " + Strings.Commands.powerhelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.poweracc) + " - " + Strings.Commands.poweracchelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.onlinelist) + " - " + Strings.Commands.onlinelisthelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.kick) + " - " + Strings.Commands.kickhelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.ban) + " - " + Strings.Commands.banhelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.unban) + " - " + Strings.Commands.unbanhelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.mute) + " - " + Strings.Commands.mutehelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.unmute) + " - " + Strings.Commands.unmutehelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.kill) + " - " + Strings.Commands.killhelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.makeprivate) + " - " + Strings.Commands.makeprivatehelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.makepublic) + " - " + Strings.Commands.makepublichelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.migrate) + " - " + Strings.Commands.migratehelp);
                        Console.WriteLine(@"    " + Strings.Commandoutput.helpfooter.ToString(Strings.Commands.commandinfo));
                    }
                }
                else
                {
                    Console.WriteLine(@"    " + Strings.Commandoutput.notfound);
                }

                Console.Write("> ");
                command = Console.ReadLine();
            }*/
        }

        private static void Console_CancelKeyPress([NotNull] object sender, [NotNull] ConsoleCancelEventArgs cancelEvent)
        {
            ServerContext.Instance.Dispose();
            //Shutdown();
            cancelEvent.Cancel = true;
        }

        internal static void Shutdown(bool showExiting = true)
        {
            if (showExiting)
            {
                Console.WriteLine();
                Console.WriteLine(Strings.Commands.exiting);
                Console.WriteLine();
            }

            Globals.Network?.Dispose();
            LegacyDatabase.SavePlayerDatabase();
            LegacyDatabase.SaveGameDatabase();
            Globals.Api?.Stop();

            //ServerContext.Instance.Dispose();
        }

        #region "dependencies"

        private static void ClearDlls()
        {
            DeleteIfExists("libe_sqlite3.so");
            DeleteIfExists("e_sqlite3.dll");
            DeleteIfExists("libe_sqlite3.dylib");
            DeleteIfExists("Nancy.dll");
        }

        private static string ReadProcessOutput(string name)
        {
            try
            {
                Debug.Assert(name != null, "name != null");
                var p = new Process
                {
                    StartInfo =
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        FileName = name
                    }
                };
                p.Start();
                // Do not wait for the child process to exit before
                // reading to the end of its redirected stream.
                // p.WaitForExit();
                // Read the output stream first and then wait.
                var output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                output = output.Trim();
                return output;
            }
            catch
            {
                return "";
            }
        }

        private static void ExportDependencies(string[] args)
        {
            ClearDlls();

            string dllname = "";

            var os = Environment.OSVersion;
            var platformId = os.Platform;
            if (platformId == PlatformID.Unix)
            {
                var unixName = ReadProcessOutput("uname");
                if (unixName?.Contains("Darwin") ?? false) platformId = PlatformID.MacOSX;
            }

            if (Options.ApiEnabled)
            {
                if (!ReflectionUtils.ExtractCosturaResource("costura.nancy.dll.compressed", "Nancy.dll"))
                {
                    Log.Error("Failed to extract Nancy, terminating startup.");
                    Environment.Exit(-0x1001);
                }
            }

            switch (platformId)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    //Place sqlite3.dll where it's needed.
                    dllname = Environment.Is64BitProcess ? "e_sqlite3x64.dll" : "e_sqlite3x86.dll";
                    if (!ReflectionUtils.ExtractResource($"Intersect.Server.Resources.{dllname}", "e_sqlite3.dll"))
                    {
                        Log.Error("Failed to extract sqlite library, terminating startup.");
                        Environment.Exit(-0x1000);
                    }

                    break;

                case PlatformID.MacOSX:
                    if (!ReflectionUtils.ExtractResource("Intersect.Server.Resources.libe_sqlite3.dylib", "libe_sqlite3.dylib"))
                    {
                        Log.Error("Failed to extract lib_sqlite3.dylib, terminating startup.");
                        Environment.Exit(-0x1001);
                    }
                    break;

                case PlatformID.Xbox:
                    break;

                case PlatformID.Unix:
                    //Place libe_sqlite3.so where it's needed.
                    dllname = Environment.Is64BitProcess ? "libe_sqlite3_x64.so" : "libe_sqlite3_x86.so";
                    if (args.Contains("alpine")) dllname = "libe_sqlite3_alpine.so";
                    if (!ReflectionUtils.ExtractResource($"Intersect.Server.Resources.{dllname}", "libe_sqlite3.so"))
                    {
                        Log.Error("Failed to extract libe_sqlite.so library, terminating startup.");
                        Environment.Exit(-0x1000);
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(platformId));
            }
        }

        private static bool DeleteIfExists(string filename)
        {
            try
            {
                Debug.Assert(filename != null, "filename != null");
                if (File.Exists(filename)) File.Delete(filename);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}