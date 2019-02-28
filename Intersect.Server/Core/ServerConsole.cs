using Intersect.Logging;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Errors;
using Intersect.Server.Core.Commands;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Threading;
using JetBrains.Annotations;
using System;
using System.Linq;

namespace Intersect.Server.Core
{
    internal sealed class ServerConsole : Threaded
    {
        [NotNull]
        public CommandParser Parser { get; }

        public ServerConsole()
        {
            Console.WaitPrefix = "> ";

            Parser = new CommandParser(
                new ParserSettings(
                    localization: Strings.Commands.Parsing
                )
            );
            Parser.Register<AnnouncementCommand>();
            Parser.Register<BanCommand>();
            Parser.Register<CpsCommand>();
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
            Parser.Register<PowerAccountCommand>();
            Parser.Register<PowerCommand>();
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
                if (result.Missing.IsEmpty)
                {
                    var fatalError = false;
                    result.Errors.ForEach(error =>
                    {
                        if (error == null)
                        {
                            return;
                        }

                        fatalError = error.IsFatal;
                        if (error is MissingArgumentError)
                        {
                            return;
                        }

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
                else
                {
                    Console.WriteLine(
                        Strings.Commands.Parsing.Errors.MissingArguments.ToString(
                            string.Join(
                                Strings.Commands.Parsing.Errors.MissingArgumentsDelimeter,
                                result.Missing.Select(argument =>
                                    Strings.Commands.Parsing.Errors.MissingArgumentNameTypeFormat.ToString(
                                        argument?.Name,
                                        argument?.ValueType.Name
                                    )
                                )
                            )
                        )
                    );
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

                if (commandsplit[0] == Strings.Commands.Api.Name) //API Command
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