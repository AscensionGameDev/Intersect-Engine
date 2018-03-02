#define websockets
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Threading;
using Intersect.Server.Classes.Localization;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Crypto;
using Intersect.Network.Crypto.Formats;
using Intersect.Server.Classes.Core;
using Intersect.Server.Classes.General;
using Intersect.Server.Classes.Networking;
using Intersect.Server.Network;
using Open.Nat;
using Intersect.Utilities;

namespace Intersect.Server.Classes
{
    using Database = Intersect.Server.Classes.Core.LegacyDatabase;

    public class ServerStart
    {
        private static bool sErrorHalt = true;
        public static ServerNetwork SocketServer;

        public static void Start(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            if (RunningOnWindows()) SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);
            Console.CancelKeyPress += Console_CancelKeyPress;

            ExportDependencies();

            LegacyDatabase.CheckDirectories();
            Thread logicThread;
            if (!Options.LoadFromDisk())
            {
                Console.WriteLine("Failed to load server options! Press any key to shut down.");
                Console.ReadKey();
                return;
            }

            foreach (var arg in args)
            {
                if (arg.Contains("port="))
                {
                    ushort port = Options.ServerPort;
                    if (ushort.TryParse(arg.Split("=".ToCharArray())[1],out port))
                    {
                        Options.ServerPort = port;
                    }
                }
            }

            Strings.Load(Options.Language);
            Console.WriteLine(@"  _____       _                          _   ");
            Console.WriteLine(@" |_   _|     | |                        | |  ");
            Console.WriteLine(@"   | |  _ __ | |_ ___ _ __ ___  ___  ___| |_ ");
            Console.WriteLine(@"   | | | '_ \| __/ _ \ '__/ __|/ _ \/ __| __|");
            Console.WriteLine(@"  _| |_| | | | ||  __/ |  \__ \  __/ (__| |_ ");
            Console.WriteLine(@" |_____|_| |_|\__\___|_|  |___/\___|\___|\__|");
            Console.WriteLine(Strings.Intro.tagline);
            Console.WriteLine("Copyright (C) 2018  Ascension Game Dev");
            Console.WriteLine(Strings.Intro.version.ToString( Assembly.GetExecutingAssembly().GetName().Version));
            Console.WriteLine(Strings.Intro.support);
            Console.WriteLine(Strings.Intro.loading);
            Formulas.LoadFormulas();
            if (!LegacyDatabase.InitDatabase())
            {
                Console.ReadKey();
                return;
            }
            CustomColors.Load();
            Console.WriteLine(Strings.Commandoutput.playercount.ToString(LegacyDatabase.GetRegisteredPlayers()));
            Console.WriteLine(Strings.Commandoutput.gametime.ToString( ServerTime.GetTime().ToString("F")));
            ServerTime.Update();
            Log.Global.AddOutput(new ConsoleOutput(Debugger.IsAttached ? LogLevel.All : LogLevel.Error));
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("Intersect.Server.private-intersect.bek"))
            {
                var rsaKey = EncryptionKey.FromStream<RsaKey>(stream);
                Debug.Assert(rsaKey != null, "rsaKey != null");
                SocketServer = new ServerNetwork(new NetworkConfiguration(Options.ServerPort), rsaKey.Parameters);
            }

            var packetHander = new PacketHandler();
            SocketServer.Handlers[PacketCode.BinaryPacket] = packetHander.HandlePacket;

#if websockets
            WebSocketNetwork.Init(Options.ServerPort);
            Console.WriteLine(Strings.Intro.websocketstarted.ToString( Options.ServerPort));
#endif

            if (!SocketServer.Listen())
            {
                Log.Error("An error occurred while attempting to connect.");
            }

            Console.WriteLine();

            if (Options.UPnP && !args.Contains("noupnp"))
            {
                UpnP.ConnectNatDevice().Wait(5000);
                UpnP.OpenServerPort(Options.ServerPort, Protocol.Tcp).Wait(5000);
                UpnP.OpenServerPort(Options.ServerPort, Protocol.Udp).Wait(5000);
                Console.WriteLine();
            }

            //Check to see if AGD can see this server. If so let the owner know :)
            if (Options.OpenPortChecker && !args.Contains("noportcheck"))
            {
                var externalIp = "";
                var serverAccessible = PortChecker.CanYouSeeMe(Options.ServerPort, out externalIp);

                Console.WriteLine(Strings.Portchecking.connectioninfo);
                if (!String.IsNullOrEmpty(externalIp))
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
            Console.WriteLine(Strings.Intro.started.ToString( Options.ServerPort));

            logicThread = new Thread(() => ServerLoop.RunServerLoop());
            logicThread.Start();
            if (args.Contains("nohalt"))
            {
                sErrorHalt = false;
            }
            if (!args.Contains("noconsole"))
            {
                Console.WriteLine(Strings.Intro.consoleactive);
                Console.Write("> ");
                string command = Console.ReadLine();
                while (true)
                {
                    bool userFound = false;
                    string ip = "";
                    if (command == null)
                    {
                        ShutDown();
                        return;
                    }
                    command = command.Trim();
                    string[] commandsplit = command.Split(' ');

                    if (commandsplit[0] == Strings.Commands.announcement) //Announcement Command
                    {
                        if (commandsplit.Length > 1)
                        {
                            if (commandsplit[1] == Strings.Commands.commandinfo)
                            {
                                Console.WriteLine(@"    " +
                                                  Strings.Commands.announcementusage.ToString(
                                                      Strings.Commands.commandinfo));
                                Console.WriteLine(@"    " + Strings.Commands.announcementdesc);
                            }
                            else
                            {
                                PacketSender.SendGlobalMsg(command.Remove(0, 12));
                            }
                        }
                        else
                        {
                            Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(
                                Strings.Commands.commandinfo));
                        }
                    }
                    else if (commandsplit[0] == Strings.Commands.onlinelist) //Online List Command
                    {
                        Console.WriteLine(string.Format("{0,-10}", Strings.Commandoutput.listid) +
                                          string.Format("{0,-28}", Strings.Commandoutput.listaccount) +
                                          string.Format("{0,-28}", Strings.Commandoutput.listcharacter));
                        Console.WriteLine(new string('-', 66));
                        for (int i = 0; i < Globals.Clients.Count; i++)
                        {
                            if (Globals.Clients[i] != null)
                            {
                                var name = Globals.Clients[i].Entity != null
                                    ? Globals.Clients[i].Entity.Name
                                    : "";
                                Console.WriteLine(string.Format("{0,-10}", "#" + i) +
                                                  string.Format("{0,-28}", Globals.Clients[i].MyAccount) +
                                                  string.Format("{0,-28}", name));
                            }
                        }
                    }
                    else if (commandsplit[0] == Strings.Commands.kill) //Kill Command
                    {
                        if (commandsplit.Length > 1)
                        {
                            if (commandsplit[1] == Strings.Commands.commandinfo)
                            {
                                Console.WriteLine(@"    " +
                                                  Strings.Commands.killusage.ToString(
                                                      Strings.Commands.commandinfo));
                                Console.WriteLine(@"    " + Strings.Commands.killdesc);
                            }
                            else
                            {
                                for (int i = 0; i < Globals.Clients.Count; i++)
                                {
                                    if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                                    {
                                        string user = Globals.Clients[i].Entity.Name.ToLower();
                                        if (user == commandsplit[1].ToLower())
                                        {
                                            Globals.Clients[i].Entity.Die();
                                            PacketSender.SendGlobalMsg(@"    " +
                                                                       Strings.Player.serverkilled.ToString(
                                                                           Globals.Clients[i].Entity.Name));
                                            Console.WriteLine(@"    " +
                                                              Strings.Commandoutput.killsuccess.ToString(
                                                                  Globals.Clients[i].Entity.Name));
                                            userFound = true;
                                            break;
                                        }
                                    }
                                }
                                if (userFound == false)
                                {
                                    Console.WriteLine(@"    " + Strings.Player.offline);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(
                                Strings.Commands.commandinfo));
                        }
                    }
                    else if (commandsplit[0] == Strings.Commands.kick) //Kick Command
                    {
                        if (commandsplit.Length > 1)
                        {
                            if (commandsplit[1] == Strings.Commands.commandinfo)
                            {
                                Console.WriteLine(@"    " +
                                                  Strings.Commands.kickusage.ToString(
                                                      Strings.Commands.commandinfo));
                                Console.WriteLine(@"    " + Strings.Commands.kickdesc);
                            }
                            else
                            {
                                for (int i = 0; i < Globals.Clients.Count; i++)
                                {
                                    if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                                    {
                                        string user = Globals.Clients[i].Entity.Name.ToLower();
                                        if (user == commandsplit[1].ToLower())
                                        {
                                            PacketSender.SendGlobalMsg(Strings.Player.serverkicked.ToString(
                                                Globals.Clients[i].Entity.Name));
                                            Console.WriteLine(@"    " +
                                                              Strings.Player.serverkicked.ToString(
                                                                  Globals.Clients[i].Entity.Name));
                                            Globals.Clients[i].Disconnect(); //Kick em'
                                            userFound = true;
                                            break;
                                        }
                                    }
                                }
                                if (userFound == false)
                                {
                                    Console.WriteLine(@"    " + Strings.Player.offline);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(
                                Strings.Commands.commandinfo));
                        }
                    }
                    else if (commandsplit[0] == Strings.Commands.unban) //Unban Command
                    {
                        if (commandsplit.Length > 1)
                        {
                            if (commandsplit[1] == Strings.Commands.commandinfo)
                            {
                                Console.WriteLine(@"    " +
                                                  Strings.Commands.unbanusage.ToString(
                                                      Strings.Commands.commandinfo));
                                Console.WriteLine(@"    " + Strings.Commands.unbandesc);
                            }
                            else
                            {
                                if (LegacyDatabase.AccountExists(commandsplit[1]))
                                {
                                    LegacyDatabase.DeleteBan(commandsplit[1]);
                                    Console.WriteLine(
                                        @"    " + Strings.Account.unbanned.ToString( commandsplit[1]));
                                }
                                else
                                {
                                    Console.WriteLine("    " + Strings.Account.notfound.ToString( commandsplit[1]));
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(
                                Strings.Commands.commandinfo));
                        }
                    }
                    else if (commandsplit[0] == Strings.Commands.ban) //Ban Command
                    {
                        if (commandsplit.Length > 1)
                        {
                            if (commandsplit[1] == Strings.Commands.commandinfo)
                            {
                                Console.WriteLine(@"    " +
                                                  Strings.Commands.banusage.ToString(
                                                      Strings.Commands.True,
                                                      Strings.Commands.False,
                                                      Strings.Commands.commandinfo));
                                Console.WriteLine(@"    " + Strings.Commands.bandesc);
                            }
                            else
                            {
                                if (commandsplit.Length > 3)
                                {
                                    for (int i = 0; i < Globals.Clients.Count; i++)
                                    {
                                        if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                                        {
                                            string user = Globals.Clients[i].Entity.Name.ToLower();
                                            if (user == commandsplit[1].ToLower())
                                            {
                                                string reason = "";
                                                for (int n = 4; n < commandsplit.Length; n++)
                                                {
                                                    reason += commandsplit[n] + " ";
                                                }
                                                if (commandsplit[3] == Strings.Commands.True)
                                                {
                                                    ip = Globals.Clients[i].GetIp();
                                                }
                                                LegacyDatabase.AddBan(Globals.Clients[i],
                                                    Convert.ToInt32(commandsplit[2]),
                                                    reason,
                                                    Strings.Commands.banuser, ip);
                                                PacketSender.SendGlobalMsg(Strings.Account.banned.ToString(
                                                    Globals.Clients[i].Entity.Name));
                                                Console.WriteLine(@"    " +
                                                                  Strings.Account.banned.ToString(
                                                                      Globals.Clients[i].Entity.Name));
                                                Globals.Clients[i].Disconnect(); //Kick em'
                                                userFound = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (userFound == false)
                                    {
                                        Console.WriteLine(@"    " + Strings.Player.offline);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(
                                        Strings.Commands.commandinfo));
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(
                                Strings.Commands.commandinfo));
                        }
                    }
                    else if (commandsplit[0] == Strings.Commands.unmute) //Unmute Command
                    {
                        if (commandsplit.Length > 1)
                        {
                            if (commandsplit[1] == Strings.Commands.commandinfo)
                            {
                                Console.WriteLine(@"    " +
                                                  Strings.Commands.unmuteusage.ToString(
                                                      Strings.Commands.commandinfo));
                                Console.WriteLine(@"    " + Strings.Commands.unmutedesc);
                            }
                            else
                            {
                                for (int i = 0; i < Globals.Clients.Count; i++)
                                {
                                    if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                                    {
                                        string user = Globals.Clients[i].Entity.Name.ToLower();
                                        if (user == commandsplit[1].ToLower())
                                        {
                                            LegacyDatabase.DeleteMute(Globals.Clients[i].MyAccount);
                                            Globals.Clients[i].Muted = false;
                                            Globals.Clients[i].MuteReason = "";
                                            PacketSender.SendGlobalMsg(Strings.Account.unmuted.ToString(
                                                Globals.Clients[i].Entity.Name));
                                            Console.WriteLine(@"    " +
                                                              Strings.Account.unmuted.ToString(
                                                                  Globals.Clients[i].Entity.Name));
                                            userFound = true;
                                            break;
                                        }
                                    }
                                }
                                if (userFound == false)
                                {
                                    Console.WriteLine(@"    " + Strings.Player.offline);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(
                                Strings.Commands.commandinfo));
                        }
                    }
                    else if (commandsplit[0] == Strings.Commands.mute) //Mute Command
                    {
                        if (commandsplit.Length > 1)
                        {
                            if (commandsplit[1] == Strings.Commands.commandinfo)
                            {
                                Console.WriteLine(@"    " +
                                                  Strings.Commands.muteusage.ToString(
                                                      Strings.Commands.True,
                                                      Strings.Commands.False,
                                                      Strings.Commands.commandinfo));
                                Console.WriteLine(@"    " + Strings.Commands.mutedesc);
                            }
                            else
                            {
                                if (commandsplit.Length > 3)
                                {
                                    for (int i = 0; i < Globals.Clients.Count; i++)
                                    {
                                        if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                                        {
                                            string user = Globals.Clients[i].Entity.Name.ToLower();
                                            if (user == commandsplit[1].ToLower())
                                            {
                                                string reason = "";
                                                for (int n = 4; n < commandsplit.Length; n++)
                                                {
                                                    reason += commandsplit[n] + " ";
                                                }
                                                if (commandsplit[3] == Strings.Commands.True)
                                                {
                                                    ip = Globals.Clients[i].GetIp();
                                                }
                                                LegacyDatabase.AddMute(Globals.Clients[i],
                                                    Convert.ToInt32(commandsplit[2]),
                                                    reason, Strings.Commands.muteuser, ip);
                                                Globals.Clients[i].Muted = true; //Cut out their tongues!
                                                Globals.Clients[i].MuteReason =
                                                    LegacyDatabase.CheckMute(Globals.Clients[i].MyAccount,
                                                        Globals.Clients[i].GetIp());
                                                PacketSender.SendGlobalMsg(Strings.Account.muted.ToString(
                                                    Globals.Clients[i].Entity.Name));
                                                Console.WriteLine(@"    " +
                                                                  Strings.Account.muted.ToString(
                                                                      Globals.Clients[i].Entity.Name));
                                                userFound = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (userFound == false)
                                    {
                                        Console.WriteLine(@"    " + Strings.Player.offline);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(
                                        Strings.Commands.commandinfo));
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(
                                Strings.Commands.commandinfo));
                        }
                    }
                    else if (commandsplit[0] == Strings.Commands.power) //Power Command
                    {
                        if (commandsplit.Length > 1)
                        {
                            if (commandsplit[1] == Strings.Commands.commandinfo)
                            {
                                Console.WriteLine(@"    " +
                                                  Strings.Commands.powerusage.ToString(
                                                      Strings.Commands.commandinfo));
                                Console.WriteLine(@"    " + Strings.Commands.powerdesc);
                            }
                            else
                            {
                                if (commandsplit.Length > 2)
                                {
                                    for (int i = 0; i < Globals.Clients.Count; i++)
                                    {
                                        if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                                        {
                                            string user = Globals.Clients[i].Entity.Name.ToLower();
                                            if (user == commandsplit[1].ToLower())
                                            {
                                                LegacyDatabase.SetPlayerPower(Globals.Clients[i].MyAccount,
                                                    int.Parse(commandsplit[2]));
                                                PacketSender.SendEntityDataToProximity(Globals.Clients[i].Entity);
                                                if (Globals.Clients[i].Power > 0)
                                                {
                                                    PacketSender.SendGlobalMsg(Strings.Player.admin.ToString(
                                                        Globals.Clients[i].Entity.Name));
                                                }
                                                else
                                                {
                                                    PacketSender.SendGlobalMsg(Strings.Player.deadmin.ToString(
                                                        Globals.Clients[i].Entity.Name));
                                                }
                                                Console.WriteLine(@"    " +
                                                                  Strings.Commandoutput.powerchanged.ToString(
                                                                      Globals.Clients[i].Entity.Name));

                                                userFound = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (userFound == false)
                                    {
                                        Console.WriteLine(@"    " + Strings.Player.offline);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(
                                        Strings.Commands.commandinfo));
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(
                                Strings.Commands.commandinfo));
                        }
                    }
                    else if (commandsplit[0] == Strings.Commands.poweracc) //Power Account Command
                    {
                        if (commandsplit.Length > 1)
                        {
                            if (commandsplit[1] == Strings.Commands.commandinfo)
                            {
                                Console.WriteLine(@"    " +
                                                  Strings.Commands.poweraccusage.ToString(
                                                      Strings.Commands.commandinfo));
                                Console.WriteLine(@"    " + Strings.Commands.poweraccdesc);
                            }
                            else
                            {
                                if (commandsplit.Length > 2)
                                {
                                    if (commandsplit.Length > 2)
                                    {
                                        try
                                        {
                                            if (LegacyDatabase.AccountExists(commandsplit[1]))
                                            {
                                                LegacyDatabase.SetPlayerPower(commandsplit[1],
                                                    int.Parse(commandsplit[2]));
                                                Console.WriteLine(@"    " +
                                                                  Strings.Commandoutput.powerchanged.ToString(
                                                                      commandsplit[1]));
                                            }
                                            else
                                            {
                                                Console.WriteLine(@"    " +
                                                                  Strings.Account.notfound.ToString(
                                                                      commandsplit[1]));
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            Console.WriteLine(@"    " +
                                                              Strings.Commandoutput.parseerror.ToString(
                                                                  commandsplit[0],
                                                                  Strings.Commands.commandinfo));
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine(@"    " +
                                                          Strings.Commandoutput.syntaxerror.ToString(
                                                              commandsplit[0],
                                                              Strings.Commands.commandinfo));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(
                                        Strings.Commands.commandinfo));
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(
                                Strings.Commands.commandinfo));
                        }
                    }
                    else if (commandsplit[0] == Strings.Commands.cps) //CPS Command
                    {
                        if (commandsplit.Length > 1)
                        {
                            if (commandsplit[1] == Strings.Commands.commandinfo)
                            {
                                Console.WriteLine(@"    " +
                                                  Strings.Commands.cpsusage.ToString(
                                                      Strings.Commands.commandinfo));
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
                                {
                                    Console.WriteLine(Strings.Commandoutput.cpslocked);
                                }
                                else
                                {
                                    Console.WriteLine(Strings.Commandoutput.cpsunlocked);
                                }
                            }
                            else
                            {
                                Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(
                                    Strings.Commands.commandinfo));
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
                                Console.WriteLine(@"    " +
                                                  Strings.Commands.exitusage.ToString(
                                                      Strings.Commands.commandinfo));
                                Console.WriteLine(@"    " + Strings.Commands.exitdesc);
                            }
                            else
                            {
                                Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(
                                    Strings.Commands.commandinfo));
                            }
                        }
                        else
                        {
                            ShutDown();
                            return;
                        }
                    }
                    else if (commandsplit[0] == Strings.Commands.help) //Help Command
                    {
                        if (commandsplit.Length > 1)
                        {
                            if (commandsplit[1] == Strings.Commands.commandinfo)
                            {
                                Console.WriteLine(@"    " +
                                                  Strings.Commands.helpusage.ToString(
                                                      Strings.Commands.commandinfo));
                                Console.WriteLine(@"    " + Strings.Commands.helpdesc);
                            }
                            else
                            {
                                Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(
                                    Strings.Commands.commandinfo));
                            }
                        }
                        else
                        {
                            Console.WriteLine(@"    " + Strings.Commandoutput.helpheader);
                            Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.help) +
                                              " - " + Strings.Commands.helphelp);
                            Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.exit) +
                                              " - " + Strings.Commands.exithelp);
                            Console.WriteLine(@"    " +
                                              string.Format("{0,-20}", Strings.Commands.announcement) +
                                              " - " +
                                              Strings.Commands.announcementhelp);
                            Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.cps) +
                                              " - " +
                                              Strings.Commands.cpshelp);
                            Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.power) +
                                              " - " + Strings.Commands.powerhelp);
                            Console.WriteLine(
                                @"    " + string.Format("{0,-20}", Strings.Commands.poweracc) +
                                " - " + Strings.Commands.poweracchelp);
                            Console.WriteLine(
                                @"    " + string.Format("{0,-20}", Strings.Commands.onlinelist) +
                                " - " + Strings.Commands.onlinelisthelp);
                            Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.kick) +
                                              " - " + Strings.Commands.kickhelp);
                            Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.ban) +
                                              " - " +
                                              Strings.Commands.banhelp);
                            Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.unban) +
                                              " - " + Strings.Commands.unbanhelp);
                            Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.mute) +
                                              " - " + Strings.Commands.mutehelp);
                            Console.WriteLine(
                                @"    " + string.Format("{0,-20}", Strings.Commands.unmute) +
                                " - " + Strings.Commands.unmutehelp);
                            Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.kill) +
                                              " - " + Strings.Commands.killhelp);
                            Console.WriteLine(@"    " +
                                              Strings.Commandoutput.helpfooter.ToString(
                                                  Strings.Commands.commandinfo));
                        }
                    }
                    else
                    {
                        Console.WriteLine(@"    " + Strings.Commandoutput.notfound);
                    }
                    Console.Write("> ");
                    command = Console.ReadLine();
                }
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            ShutDown();
            e.Cancel = true;
        }

        private static void ShutDown()
        {
            //Save all online players
            Globals.Clients?.FindAll(client => client?.Entity != null).ForEach(client =>
            {
                LegacyDatabase.SaveCharacter(client?.Entity);
            });

            Globals.ServerStarted = false;
            SocketServer?.Dispose();
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // Ignore missing resources
            if (args.Name.Contains(".resources"))
                return null;

            // check for assemblies already loaded
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            if (assembly != null)
                return assembly;

            string filename = args.Name.Split(',')[0] + ".dll".ToLower();

            //Try Loading from libs/server first
            var libsFolder = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "libs", "server");
            if (File.Exists(Path.Combine(libsFolder, filename)))
            {
                return Assembly.LoadFile(Path.Combine(libsFolder, filename));
            }
            else
            {
                string archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                    Environment.Is64BitProcess
                        ? Path.Combine("libs", "server", "x64")
                        : Path.Combine("libs", "server", "x86"),
                    filename);
                if (File.Exists(archSpecificPath))
                {
                    return Assembly.LoadFile(archSpecificPath);
                }
                else
                {
                    return null;
                }
            }
        }

        //Really basic error handler for debugging purposes
        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error((Exception)e?.ExceptionObject);
            if (e.IsTerminating)
            {
                if (sErrorHalt)
                {
                    Console.WriteLine(Strings.Errors.errorservercrash);
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine(Strings.Errors.errorservercrashnohalt);
                }
                ShutDown();
            }
            else
            {
                Console.WriteLine(Strings.Errors.errorlogged);
            }
        }

        private static bool RunningOnWindows()
        {
            OperatingSystem os = Environment.OSVersion;
            PlatformID pid = os.Platform;
            switch (pid)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    return true;
            }
            return false;
        }

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            // Put your own handler here
            switch (ctrlType)
            {
                case CtrlTypes.CtrlCEvent:
                    //Handled Elsewhere
                    break;

                case CtrlTypes.CtrlBreakEvent:
                    //Handled Elsewhere
                    break;

                case CtrlTypes.CtrlCloseEvent:
                    ShutDown();
                    break;

                case CtrlTypes.CtrlLogoffEvent:
                case CtrlTypes.CtrlShutdownEvent:
                    ShutDown();
                    break;

            }
            return true;
        }

        #region "dependencies"
        private static void ClearDlls()
        {
            DeleteIfExists("libe_sqlite3.so");
            DeleteIfExists("e_sqlite3.dll");
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
            catch { return ""; }
        }

        private static void ExportDependencies()
        {
            ClearDlls();

            //Place sqlite3.dll where it's needed.
            var dllname = Environment.Is64BitProcess ? "sqlite3x64.dll" : "sqlite3x86.dll";
            if (!ReflectionUtils.ExtractResource($"Intersect.Server.Resources.{dllname}", "sqlite3.dll"))
            {
                Log.Error("Failed to extract sqlite library, terminating startup.");
                Environment.Exit(-0x1000);
            }

            var os = Environment.OSVersion;
            var platformId = os.Platform;
            if (platformId == PlatformID.Unix)
            {
                var unixName = ReadProcessOutput("uname");
                if (unixName?.Contains("Darwin") ?? false)
                {
                    platformId = PlatformID.MacOSX;
                }
            }

            switch (platformId)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    //Place e_sqlite3.dll where it's needed.
                    dllname = Environment.Is64BitProcess ? "e_sqlite3x64.dll" : "e_sqlite3x86.dll";
                    if (!ReflectionUtils.ExtractResource($"Intersect.Server.Resources.{dllname}", "e_sqlite3.dll"))
                    {
                        Log.Error("Failed to extract e_sqlite library, terminating startup.");
                        Environment.Exit(-0x1000);
                    }
                    break;

                case PlatformID.MacOSX:
                    break;

                case PlatformID.Xbox:
                    break;

                case PlatformID.Unix:
                    //Place libe_sqlite3.so where it's needed.
                    dllname = "libe_sqlite3.so";
                    if (!ReflectionUtils.ExtractResource($"Intersect.Server.Resources.{dllname}", "libe_sqlite.so"))
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
            catch { return false; }
        }
        #endregion

        #region unmanaged
        // Declare the SetConsoleCtrlHandler function
        // as external and receiving a delegate.

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine handler, bool add);

        // A delegate type to be used as the handler routine
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes ctrlType);

        // An enumerated type for the control messages
        // sent to the handler routine.
        public enum CtrlTypes
        {
            CtrlCEvent = 0,
            CtrlBreakEvent,
            CtrlCloseEvent,
            CtrlLogoffEvent = 5,
            CtrlShutdownEvent
        }

        #endregion
    }
}