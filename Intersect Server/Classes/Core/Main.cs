/*
    Intersect Game Engine (Server)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
#define websockets
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Intersect_Library;
using Intersect_Server.Classes.Core;
using Intersect_Server.Classes.General;
using Intersect_Server.Classes.Networking;

namespace Intersect_Server.Classes
{
    class MainClass
    {

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Thread logicThread;
            Console.WriteLine(@"  _____       _                          _   ");
            Console.WriteLine(@" |_   _|     | |                        | |  ");
            Console.WriteLine(@"   | |  _ __ | |_ ___ _ __ ___  ___  ___| |_ ");
            Console.WriteLine(@"   | | | '_ \| __/ _ \ '__/ __|/ _ \/ __| __|");
            Console.WriteLine(@"  _| |_| | | | ||  __/ |  \__ \  __/ (__| |_ ");
            Console.WriteLine(@" |_____|_| |_|\__\___|_|  |___/\___|\___|\__|");
            Console.WriteLine(@"                          free 2d orpg engine");
            Console.WriteLine("Copyright (C) 2015  JC Snider, Joe Bridges");
            Console.WriteLine("Version " + Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine("For help, support, and updates visit: http://ascensiongamedev.com");
            Console.WriteLine("Loading, please wait.");
            Database.CheckDirectories();
            if (!ServerOptions.LoadOptions())
            {
                Console.WriteLine("Failed to load server options! Press any key to shut down.");
                Console.ReadKey();
                return;
            }
            if (!Database.InitDatabase())
            {
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Server has " + Database.GetRegisteredPlayers() + " registered players.");
            SocketServer.Init();
            Console.WriteLine("Server Started. Using Port #" + Options.ServerPort);
#if websockets
            WebSocketServer.Init();
            Console.WriteLine("Websocket Listener started for Unity WebGL Clients. Using Port #" +
                              (Options.ServerPort + 1));
#endif
            logicThread = new Thread(() => ServerLoop.RunServerLoop());
            logicThread.Start();
            if (Environment.UserInteractive)
            {
                Console.WriteLine("Type exit to shutdown the server, or help for a list of commands.");
                Console.Write("> ");
                string command = Console.ReadLine();
                while (true)
                {
                    bool userFound = false;
                    string ip = "";
                    command = command.Trim();
                    string[] commandsplit = command.Split(' ');
                    switch (commandsplit[0])
                    {
                        case "announcement":
                            if (commandsplit.Length > 1)
                            {
                                switch (commandsplit[1])
                                {
                                    case "/?":
                                        Console.WriteLine(@"    Usage: announcement [message] [/?]");
                                        Console.WriteLine(
                                            @"    Desc: Sends a global message to all users playing the game.");
                                        break;
                                    default:
                                        PacketSender.SendGlobalMsg(command.Remove(0, 12));
                                        break;
                                }
                            }
                            break;
                        case "onlinelist":
                            Console.WriteLine(@"|ID | Account | Screen name|");
                            Console.WriteLine(@"----------------------------");
                            for (int i = 0; i < Globals.Clients.Count; i++)
                            {
                                Console.WriteLine(@"#" + i + ") " + Globals.Clients[i].MyAccount + " | " + Globals.Clients[i].Entity.MyName);
                            }
                            break;
                        case "kill":
                            if (commandsplit.Length > 1)
                            {
                                switch (commandsplit[1])
                                {
                                    case "/?":
                                        Console.WriteLine(@"    Usage: kill [username] [/?]");
                                        Console.WriteLine(
                                            @"    Desc: Kills a player on the server.");
                                        break;
                                    default:
                                        for (int i = 0; i < Globals.Clients.Count; i++)
                                        {
                                            string user = Globals.Clients[i].Entity.MyName.ToLower();
                                            if (user == commandsplit[1].ToLower())
                                            {
                                                Globals.Clients[i].Entity.Die();
                                                PacketSender.SendGlobalMsg(Globals.Clients[i].Entity.MyName + " has been killed by the server!");
                                                Console.WriteLine(@"    " + Globals.Clients[i].Entity.MyName + " has been killed!");
                                                userFound = true;
                                                break;
                                            }
                                        }
                                        if (userFound == false) { Console.WriteLine(@"    User not online!"); }
                                        break;
                                }
                            }
                            break;
                        case "kick":
                            if (commandsplit.Length > 1)
                            {
                                switch (commandsplit[1])
                                {
                                    case "/?":
                                        Console.WriteLine(@"    Usage: kick [username] [/?]");
                                        Console.WriteLine(
                                            @"    Desc: Kicks a player from the server.");
                                        break;
                                    default:
                                        for (int i = 0; i < Globals.Clients.Count; i++)
                                        {
                                            string user = Globals.Clients[i].Entity.MyName.ToLower();
                                            if (user == commandsplit[1].ToLower())
                                            {
                                                PacketSender.SendGlobalMsg(Globals.Clients[i].Entity.MyName + " has been kicked from the server!");
                                                Console.WriteLine(@"    " + Globals.Clients[i].Entity.MyName + " has been kicked from the server!");
                                                Globals.Clients[i].Disconnect(); //Kick em'
                                                userFound = true;
                                                break;
                                            }
                                        }
                                        if (userFound == false) { Console.WriteLine(@"    User not online!"); }
                                        break;
                                }
                            }
                            break;
                        case "unban":
                            if (commandsplit.Length > 1)
                            {
                                switch (commandsplit[1])
                                {
                                    case "/?":
                                        Console.WriteLine(@"    Usage: unban [account] [/?]");
                                        Console.WriteLine(
                                            @"    Desc: Unbans a player from the server.");
                                        break;
                                    default:
                                        if (Database.AccountExists(commandsplit[1]))
                                        {
                                            Database.DeleteBan(commandsplit[1]);
                                            Console.WriteLine(@"    " + commandsplit[1] + " has been unbanned from the server!");
                                        }
                                        else
                                        {
                                            Console.WriteLine("    Error: Account " + commandsplit[1] +
                                                              " was not found!");
                                        }
                                        break;
                                }
                            }
                            break;
                        case "ban":
                            if (commandsplit.Length > 1)
                            {
                                switch (commandsplit[1])
                                {
                                    case "/?":
                                        Console.WriteLine(@"    Usage: ban [username] [duration (days)] [IP Ban? (True/False)] [reason] [/?]");
                                        Console.WriteLine(
                                            @"    Desc: Bans a player from the server.");
                                        break;
                                    default:
                                        if (commandsplit.Length > 3)
                                        {
                                            for (int i = 0; i < Globals.Clients.Count; i++)
                                            {
                                                string user = Globals.Clients[i].Entity.MyName.ToLower();
                                                if (user == commandsplit[1].ToLower())
                                                {
                                                    string reason = "";
                                                    for (int n = 4; n < commandsplit.Length; n++)
                                                    {
                                                        reason += commandsplit[n] + " ";
                                                    }
                                                    if (Convert.ToBoolean(commandsplit[3]))
                                                    {
                                                        ip = Globals.Clients[i].GetIP();
                                                    }
                                                    Database.AddBan(Globals.Clients[i], Convert.ToInt32(commandsplit[2]), reason, "the server", ip);
                                                    PacketSender.SendGlobalMsg(Globals.Clients[i].Entity.MyName + " has been banned from the server!");
                                                    Console.WriteLine(@"    " + Globals.Clients[i].Entity.MyName + " has been banned from the server!");
                                                    Globals.Clients[i].Disconnect(); //Kick em'
                                                    userFound = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (userFound == false) { Console.WriteLine(@"    User not online!"); }
                                        break;
                                }
                            }
                            break;
                        case "unmute":
                            if (commandsplit.Length > 1)
                            {
                                switch (commandsplit[1])
                                {
                                    case "/?":
                                        Console.WriteLine(@"    Usage: unmute [username] [/?]");
                                        Console.WriteLine(
                                            @"    Desc: unmutes a player allowing them to talk.");
                                        break;
                                    default:
                                        for (int i = 0; i < Globals.Clients.Count; i++)
                                        {
                                            string user = Globals.Clients[i].Entity.MyName.ToLower();
                                            if (user == commandsplit[1].ToLower())
                                            {
                                                Database.DeleteMute(Globals.Clients[i].MyAccount);
                                                Globals.Clients[i].Muted = false;
                                                Globals.Clients[i].MuteReason = "";
                                                PacketSender.SendGlobalMsg(Globals.Clients[i].Entity.MyName + " has been unmuted!");
                                                Console.WriteLine(@"    " + Globals.Clients[i].Entity.MyName + " has been unmuted!");
                                                userFound = true;
                                                break;
                                            }
                                        }
                                        if (userFound == false) { Console.WriteLine(@"    User not online!"); }
                                        break;
                                }
                            }
                            break;
                        case "mute":
                            if (commandsplit.Length > 1)
                            {
                                switch (commandsplit[1])
                                {
                                    case "/?":
                                        Console.WriteLine(@"    Usage: unmute [username] [/?]");
                                        Console.WriteLine(
                                            @"    Desc: mutes a player preventing them from talking.");
                                        break;
                                    default:
                                        if (commandsplit.Length > 3)
                                        {
                                            for (int i = 0; i < Globals.Clients.Count; i++)
                                            {
                                                string user = Globals.Clients[i].Entity.MyName.ToLower();
                                                if (user == commandsplit[1].ToLower())
                                                {
                                                    string reason = "";
                                                    for (int n = 4; n < commandsplit.Length; n++)
                                                    {
                                                        reason += commandsplit[n] + " ";
                                                    }
                                                    if (Convert.ToBoolean(commandsplit[3]))
                                                    {
                                                        ip = Globals.Clients[i].GetIP();
                                                    }
                                                    Database.AddMute(Globals.Clients[i], Convert.ToInt32(commandsplit[2]), reason, "the server", ip);
                                                    Globals.Clients[i].Muted = true; //Cut out their tongues!
                                                    Globals.Clients[i].MuteReason = Database.CheckMute(Globals.Clients[i].MyAccount, Globals.Clients[i].GetIP());
                                                    PacketSender.SendGlobalMsg(Globals.Clients[i].Entity.MyName + " has been muted!");
                                                    Console.WriteLine(@"    " + Globals.Clients[i].Entity.MyName + " has been muted!");
                                                    userFound = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (userFound == false) { Console.WriteLine(@"    User not online!"); }
                                        break;
                                }
                            }
                            break;
                        case "power":
                            if (commandsplit.Length > 1)
                            {
                                switch (commandsplit[1])
                                {
                                    case "/?":
                                        Console.WriteLine(@"    Usage: power [username] [level] [/?]");
                                        Console.WriteLine(
                                            @"    Desc: Sets the power or access of a selected account. Power 0 is regular user. Power 1 is in-game moderator. Power 2 is owner/designer and allows editor access.");
                                        break;
                                    default:
                                        //Try to admin the player
                                        if (commandsplit.Length > 2)
                                        {
                                            for (int i = 0; i < Globals.Clients.Count; i++)
                                            {
                                                string user = Globals.Clients[i].Entity.MyName.ToLower();
                                                if (user == commandsplit[1].ToLower())
                                                {
                                                    Database.SetPlayerPower(commandsplit[1], Int32.Parse(commandsplit[2]));
                                                    if (Globals.Clients[i].Power > 0)
                                                    {
                                                        PacketSender.SendGlobalMsg(Globals.Clients[i].Entity.MyName + " has been given administrative powers!");
                                                    }
                                                    else
                                                    {
                                                        PacketSender.SendGlobalMsg(Globals.Clients[i].Entity.MyName + " has had their administrative poweres revoked!");
                                                    }
                                                    Console.WriteLine(@"    " + Globals.Clients[i].Entity.MyName + " has had their power updated!");
                                                    userFound = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (userFound == false) { Console.WriteLine(@"    User not online!"); }
                                        break;
                                }
                            }
                            else
                            {
                                Console.WriteLine(@"    Syntax Error: Expected parameter not found. Type " + commandsplit[0] +
                                                  " /? for usage information.");
                            }
                            break;
                        case "poweracc":
                            if (commandsplit.Length > 1)
                            {
                                switch (commandsplit[1])
                                {
                                    case "/?":
                                        Console.WriteLine(@"    Usage: power [login] [level] [/?]");
                                        Console.WriteLine(
                                            @"    Desc: Sets the power or access of a selected account. Power 0 is regular user. Power 1 is in-game moderator. Power 2 is owner/designer and allows editor access.");
                                        break;
                                    default:
                                        //Try to admin the player
                                        if (commandsplit.Length > 2)
                                        {
                                            try
                                            {
                                                if (Database.AccountExists(commandsplit[1]))
                                                {
                                                    Database.SetPlayerPower(commandsplit[1], Int32.Parse(commandsplit[2]));
                                                }
                                                else
                                                {
                                                    Console.WriteLine("    Error: Account " + commandsplit[1] +
                                                                      " was not found!");
                                                }
                                            }
                                            catch (Exception)
                                            {
                                                Console.WriteLine(@"    Parse Error: Parameter could not be read. Type " +
                                                                  commandsplit[0] + " /? for usage information.");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine(@"    Syntax Error: Expected parameter not found. Type " +
                                                              commandsplit[0] + " /? for usage information.");
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                Console.WriteLine(@"    Syntax Error: Expected parameter not found. Type " + commandsplit[0] +
                                                  " /? for usage information.");
                            }
                            break;
                        case "cps":
                            if (commandsplit.Length > 1)
                            {
                                switch (commandsplit[1])
                                {
                                    case "/?":
                                        Console.WriteLine(@"    Usage: cps [status] [lock] [unlock] [/?]");
                                        Console.WriteLine(
                                            @"    Desc: Prints the current CPS. The status flag tells if the server loop is locked or unlocked. The lock flag locks the cps while the unlock flag unlocks it.");
                                        break;
                                    case "lock":
                                        Globals.CPSLock = true;
                                        break;
                                    case "unlock":
                                        Globals.CPSLock = false;
                                        break;
                                    case "status":
                                        if (Globals.CPSLock)
                                        {
                                            Console.WriteLine(@"CPS Locked.");
                                        }
                                        else
                                        {
                                            Console.WriteLine(@"CPS Unlocked.");
                                        }
                                        break;
                                    default:
                                        //Try to admin the player
                                        break;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Current CPS: " + Globals.CPS);
                            }
                            break;
                        case "exit":
                            if (commandsplit.Length > 1)
                            {
                                switch (commandsplit[1])
                                {
                                    case "/?":
                                        Console.WriteLine(@"    Usage: exit [/?]");
                                        Console.WriteLine(@"    Desc: Closes down the server.");
                                        break;
                                    default:
                                        Console.WriteLine(@"    Syntax Error: Parameter not recoginized. Type " +
                                                          commandsplit[0] + " /? for usage information.");
                                        break;
                                }
                            }
                            else
                            {
                                Globals.ServerStarted = false;
                                return;
                            }
                            break;
                        case "help":
                            if (commandsplit.Length > 1)
                            {
                                switch (commandsplit[1])
                                {
                                    case "/?":
                                        Console.WriteLine(@"    Usage: help [/?]");
                                        Console.WriteLine(@"    Desc: Displays the list of available commands.");
                                        break;
                                    default:
                                        Console.WriteLine(@"    Syntax Error: Parameter not recoginized. Type " +
                                                          commandsplit[0] + " /? for usage information.");
                                        break;
                                }
                            }
                            else
                            {
                                Console.WriteLine(@"    List of available commands:");
                                Console.WriteLine(@"    cps          - prints the current server cps");
                                Console.WriteLine(@"    exit         - closes the server");
                                Console.WriteLine(@"    help         - displays list of available commands");
                                Console.WriteLine(@"    power        - sets the administrative access of a user");
                                Console.WriteLine(@"    poweracc     - sets the administrative access of an account");
                                Console.WriteLine(@"    announcement - sends a global message to all players");
                                Console.WriteLine(@"    onlinelist   - shows all players online");
                                Console.WriteLine(@"    kick         - kicks a player from the server");
                                Console.WriteLine(@"    ban          - bans a player from the server");
                                Console.WriteLine(@"    mute         - mutes a player preventing them from talking");
                                Console.WriteLine(@"    unban        - unbans a player from the server");
                                Console.WriteLine(@"    unmute       - unmutes a player allowing them to talk");
                                Console.WriteLine(@"    kill         - kills a player on the server");
                                Console.WriteLine(
                                    @"    Type in any command followed by /? for parameters and usage information.");
                            }
                            break;
                        default:
                            Console.WriteLine("    Command not recoginized. Enter help for a list of commands.");
                            break;
                    }
                    Console.Write("> ");
                    command = Console.ReadLine();
                }
            }
        }

        //Really basic error handler for debugging purposes
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (!Directory.Exists("resources")) Directory.CreateDirectory("resources");
            using (StreamWriter writer = new StreamWriter("resources/errors.log", true))
            {
                writer.WriteLine("Message :" + ((Exception)e.ExceptionObject).Message + "<br/>" + Environment.NewLine +
                                 "StackTrace :" + ((Exception)e.ExceptionObject).StackTrace +
                                 "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                writer.WriteLine(Environment.NewLine +
                                 "-----------------------------------------------------------------------------" +
                                 Environment.NewLine);
            }
            Console.WriteLine("The Intersect server has encountered an error and must close. Error information can be found in resources/errors.log. Press any key to exit.");
            Console.ReadKey();
            Environment.Exit(-1);
        }
    }
}