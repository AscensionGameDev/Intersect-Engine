using Intersect.Logging;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Commands;
using Intersect.Server.Core.CommandParsing.Errors;
using Intersect.Server.Core.Commands;
using Intersect.Server.Localization;
using Intersect.Threading;

using System;
using System.Linq;

namespace Intersect.Server.Core
{
    internal sealed partial class ConsoleService
    {
        internal sealed class ConsoleThread : Threaded<ServerContext>
        {

            private readonly object mInputLock;

            private bool mDoNotContinue;

            internal ConsoleThread() : base(nameof(ConsoleService))
            {
                mInputLock = new object();

                Console.WaitPrefix = "> ";

                Parser = new CommandParser(new ParserSettings(localization: Strings.Commands.Parsing));

                Parser.Register<AnnouncementCommand>();
                Parser.Register<ApiCommand>();
                Parser.Register<ApiGrantCommand>();
                Parser.Register<ApiRevokeCommand>();
                Parser.Register<ApiRolesCommand>();
                Parser.Register<BanCommand>();
                Parser.Register<CpsCommand>();
                Parser.Register<ExitCommand>();
                Parser.Register<ExperimentsCommand>();
                Parser.Register<HelpCommand>(Parser.Settings);
                Parser.Register<KickCommand>();
                Parser.Register<KillCommand>();
                Parser.Register<MetricsCommand>();
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

            public CommandParser Parser { get; }

            public void Wait(bool doNotContinue = false)
            {
                mDoNotContinue = mDoNotContinue || doNotContinue;

                lock (mInputLock)
                {
                }
            }

            protected override void ThreadStart(ServerContext serverContext)
            {
                if (serverContext == null)
                {
                    throw new ArgumentNullException(nameof(serverContext));
                }

                Console.WriteLine(Strings.Intro.consoleactive);
                try
                {
                    while (serverContext.IsRunning && !mDoNotContinue)
                    {
                        string line;
                        lock (mInputLock)
                        {
#if !CONSOLE_EXTENSIONS
                            Console.Write(Console.WaitPrefix);
#endif

                            line = Console.ReadLine()?.Trim();

                            if (mDoNotContinue)
                            {
                                return;
                            }
                        }

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
                        var shouldHelp = result.Command is IHelpableCommand helpable && result.Find(helpable.Help);
                        if (result.Missing.IsEmpty)
                        {
                            var fatalError = false;
                            result.Errors.ForEach(
                                error =>
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

                                    if (error.Exception != null)
                                    {
                                        Log.Warn(error.Exception);
                                    }

                                    Console.WriteLine(error.Message);
                                }
                            );

                            if (!fatalError)
                            {
                                if (!shouldHelp)
                                {
                                    result.Command?.Handle(ServerContext.Instance, result);

                                    continue;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine(
                                Strings.Commands.Parsing.Errors.MissingArguments.ToString(
                                    string.Join(
                                        Strings.Commands.Parsing.Errors.MissingArgumentsDelimeter, result.Missing.Select(
                                            argument =>
                                            {
                                                var typeName =
                                                    argument?.ValueType.Name ?? Strings.Commands.Parsing.TypeUnknown;

                                                if (Strings.Commands.Parsing.TypeNames.TryGetValue(
                                                    typeName, out var localizedType
                                                ))
                                                {
                                                    typeName = localizedType;
                                                }

                                                return argument?.Name +
                                                       Strings.Commands.Parsing.Formatting.Type.ToString(typeName);
                                            }
                                        )
                                    )
                                )
                            );
                        }

                        if (result.Command == null)
                        {
                            continue;
                        }

                        var command = result.Command;
                        Console.WriteLine(command.FormatUsage(Parser.Settings, result.AsContext(true), true));

                        if (!shouldHelp)
                        {
                            continue;
                        }

                        Console.WriteLine($@"    {command.Description}");
                        Console.WriteLine();

                        var requiredBuffer = command.Arguments.Count == 1
                            ? ""
                            : new string(' ', Strings.Commands.RequiredInfo.ToString().Length);

                        command.UnsortedArguments.ForEach(
                            argument =>
                            {
                                if (argument == null)
                                {
                                    return;
                                }

                                var shortName = argument.HasShortName ? argument.ShortName.ToString() : null;
                                var name = argument.Name;

                                var typeName = argument.ValueType.Name;
                                if (argument.IsFlag)
                                {
                                    typeName = Strings.Commands.FlagInfo;
                                }
                                else if (Strings.Commands.Parsing.TypeNames.TryGetValue(typeName, out var localizedType))
                                {
                                    typeName = localizedType;
                                }

                                if (!argument.IsPositional)
                                {
                                    shortName = Parser.Settings.PrefixShort + shortName;
                                    name = Parser.Settings.PrefixLong + name;
                                }

                                var names = string.Join(
                                    ", ",
                                    new[] { shortName, name }.Where(nameString => !string.IsNullOrWhiteSpace(nameString))
                                );

                                var required = argument.IsRequiredByDefault
                                    ? Strings.Commands.RequiredInfo.ToString()
                                    : requiredBuffer;

                                var descriptionSegment = string.IsNullOrEmpty(argument.Description)
                                    ? ""
                                    : $@" - {argument.Description}";

                                Console.WriteLine($@"    {names,-16} {typeName,-12} {required}{descriptionSegment}");
                            }
                        );

                        Console.WriteLine();
                    }
                }
                catch (Exception exception)
                {
                    ServerContext.DispatchUnhandledException(exception);
                }
            }

        }
    }
}
