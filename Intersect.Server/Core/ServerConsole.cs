using Intersect.Enums;
using Intersect.Logging;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Errors;
using Intersect.Server.Core.Commands;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using Intersect.Threading;
using JetBrains.Annotations;
using System;

namespace Intersect.Server.Core
{
    internal sealed class ServerConsole : Threaded
    {
        [NotNull]
        public CommandParser Parser { get; }

        public ServerConsole()
        {
            Console.WaitPrefix = "> ";

            Parser = new CommandParser();
            Parser.Register<AnnouncementCommand>();
            Parser.Register<BanCommand>();
            Parser.Register<ExitCommand>();
            Parser.Register<HelpCommand>();
            Parser.Register<KickCommand>();
            Parser.Register<KillCommand>();
            Parser.Register<MakePrivateCommand>();
            Parser.Register<MakePublicCommand>();
            Parser.Register<MigrateCommand>();
            Parser.Register<MuteCommand>();
            Parser.Register<NetDebugCommand>();
            Parser.Register<OnlineListCommand>();
            Parser.Register<UnbanCommand>();
            Parser.Register<UnmuteCommand>();
        }

        protected override void ThreadStart()
        {
            Console.WriteLine(Strings.Intro.consoleactive);

            while (ServerContext.Instance.IsRunning)
            {
#if !CONSOLE_EXTENSIONS
                Console.Write(Console.WaitPrefix);
#endif
                var line = Console.ReadLine()?.Trim();

                if (line == null)
                {
                    ServerContext.Instance.RequestShutdown();
                    break;
                }

                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                var result = Parser.Parse(line);
                var fatalError = false;
                result.Errors.ForEach(error =>
                {
                    if (error == null)
                    {
                        return;
                    }

                    fatalError = error.IsFatal;
                    if (!error.IsFatal || error is MissingCommandError || error is UnhandledArgumentError)
                    {
                        Console.WriteLine(error.Message);
                    }
                    else
                    {
                        Log.Warn(error.Exception, error.Message);
                    }
                });

                if (!fatalError)
                {
                    result.Command?.Handle(ServerContext.Instance, result);
                }
            }

            return;
            //Console.Write("> ");
            var command = Console.ReadLine(true);
            while (ServerContext.Instance.IsRunning)
            {
                var userFound = false;
                var ip = "";
                if (command == null)
                {
                    ServerContext.Instance.Dispose();
                    //ServerStatic.Shutdown();
                    return;
                }

                command = command.Trim();
                var commandsplit = command.Split(' ');

                if (commandsplit[0] == Strings.Commands.Power.Name) //Power Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(
                                @"    " + Strings.Commands.Power.Usage.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.Power.Description);
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
                                            switch ((Access) int.Parse(commandsplit[2]))
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
                                                PacketSender.SendGlobalMsg(
                                                    Strings.Player.admin.ToString(Globals.Clients[i].Entity.Name));
                                            else
                                                PacketSender.SendGlobalMsg(
                                                    Strings.Player.deadmin.ToString(Globals.Clients[i].Entity.Name));
                                            Console.WriteLine(
                                                @"    " + Strings.Commandoutput.powerchanged.ToString(Globals.Clients[i]
                                                    .Entity.Name));

                                            userFound = true;
                                            break;
                                        }
                                    }

                                if (userFound == false) Console.WriteLine(@"    " + Strings.Player.offline);
                            }
                            else
                            {
                                Console.WriteLine(
                                    Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(
                            Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                    }
                }
                else if (commandsplit[0] == Strings.Commands.Api.Name) //API Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.Api.Description);
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
                                                Console.WriteLine(
                                                    @"    " + Strings.Commandoutput.apigranted
                                                        .ToString(commandsplit[1]));
                                            }
                                            else
                                            {
                                                Console.WriteLine(
                                                    @"    " + Strings.Commandoutput.apirevoked
                                                        .ToString(commandsplit[1]));
                                            }

                                            LegacyDatabase.SavePlayerDatabaseAsync();
                                        }
                                        else
                                        {
                                            Console.WriteLine(
                                                @"    " + Strings.Account.notfound.ToString(commandsplit[1]));
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        Console.WriteLine(
                                            @"    " + Strings.Commandoutput.parseerror.ToString(commandsplit[0],
                                                Strings.Commands.commandinfo));
                                    }
                                else
                                    Console.WriteLine(
                                        @"    " + Strings.Commandoutput.syntaxerror.ToString(commandsplit[0],
                                            Strings.Commands.commandinfo));
                            }
                            else
                            {
                                Console.WriteLine(
                                    Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(
                            Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                    }
                }
                else if (commandsplit[0] == Strings.Commands.PowerAccount.Name) //Power Account Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(
                                @"    " + Strings.Commands.PowerAccount.Usage.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.PowerAccount.Description);
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
                                            switch ((Access) int.Parse(commandsplit[2]))
                                            {
                                                case Access.Moderator:
                                                    power = UserRights.Moderation;
                                                    break;
                                                case Access.Admin:
                                                    power = UserRights.Admin;
                                                    break;
                                            }

                                            LegacyDatabase.SetPlayerPower(commandsplit[1], power);
                                            Console.WriteLine(
                                                @"    " + Strings.Commandoutput.powerchanged.ToString(commandsplit[1]));
                                        }
                                        else
                                        {
                                            Console.WriteLine(
                                                @"    " + Strings.Account.notfound.ToString(commandsplit[1]));
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        Console.WriteLine(
                                            @"    " + Strings.Commandoutput.parseerror.ToString(commandsplit[0],
                                                Strings.Commands.commandinfo));
                                    }
                                else
                                    Console.WriteLine(
                                        @"    " + Strings.Commandoutput.syntaxerror.ToString(commandsplit[0],
                                            Strings.Commands.commandinfo));
                            }
                            else
                            {
                                Console.WriteLine(
                                    Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(
                            Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                    }
                }
                else if (commandsplit[0] == Strings.Commands.Cps.Name) //CPS Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(
                                @"    " + Strings.Commands.Cps.Usage.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.Cps.Description);
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
                            Console.WriteLine(
                                Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                        }
                    }
                    else
                    {
                        Console.WriteLine(Strings.Commandoutput.cps.ToString(Globals.Cps));
                    }
                }
                else
                {
                    Console.WriteLine(@"    " + Strings.Commandoutput.notfound);
                }

                //Console.Write("> ");
                command = Console.ReadLine(true);
            }
        }
    }
}