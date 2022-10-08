using Intersect.Logging;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Localization;

using Microsoft.Diagnostics.Runtime;

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Intersect.Server.Core.Commands
{
    internal sealed class PanicCommand : ServerCommand
    {

        public PanicCommand() : base(
            Strings.Commands.Panic,
            new VariableArgument<string>(Strings.Commands.Arguments.PanicType, RequiredIfNotHelp, true)
        )
        {
        }

        protected VariableArgument<string> PanicType => FindArgumentOrThrow<VariableArgument<string>>();

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            var panicType = result.Find(PanicType)?.ToLower();
            switch (panicType)
            {
                case "attack":
                case "stack":
                    HandleStack();
                    break;

                default:
                    Log.Warn($"'{PanicType}' is not an implemented panic type.", panicType);
                    break;
            }
        }

        private static DataTarget AttachToCurrentProcess()
        {
            var currentProcess = Process.GetCurrentProcess();
            var currentPID = currentProcess.Id;
            var dataTarget = DataTarget.CreateSnapshotAndAttach(currentPID);
            return dataTarget;
        }

        private static string StringifyThreadInfo(
            ClrThread clrThread,
            IFormatProvider formatProvider,
            int indentSize
        )
        {
            if (clrThread == default)
            {
                return new string(' ', indentSize) + "null";
            }

            return string.Format(
                formatProvider,
                "{0}Thread OS Id: 0x{1,-8:X} Managed Id: 0x{2,-8:X}",
                new string(' ', indentSize),
                clrThread.OSThreadId,
                clrThread.ManagedThreadId
            );
        }

        private static void HandleStack()
        {
            var dumpBuilder = new StringBuilder();
            var culture = CultureInfo.InvariantCulture;
            using (var dataTarget = AttachToCurrentProcess())
            {
                foreach (var clrVersion in dataTarget.ClrVersions)
                {
                    var runtime = clrVersion.CreateRuntime();
                    foreach (var thread in runtime.Threads)
                    {
                        _ = dumpBuilder.AppendLine(StringifyThreadInfo(thread, culture, 0));

                        var blockingObjects = thread.BlockingObjects.ToArray();
                        if (blockingObjects.Length > 0)
                        {
                            _ = dumpBuilder.AppendFormat(culture, "  Blocked on {0,3} locks\n", blockingObjects.Length);

                            foreach (var blockingObject in thread.BlockingObjects)
                            {
                                _ = dumpBuilder.AppendFormat(
                                    culture,
                                    "{0}ID: 0x{1,-16:X} Taken: {2} Reason: {3,-14} HasSingleOwner: {4}, Lock RecursionCount: {5,2}\n",
                                    new string(' ', 4),
                                    blockingObject.Object,
                                    blockingObject.Taken,
                                    blockingObject.Reason,
                                    blockingObject.HasSingleOwner,
                                    blockingObject.RecursionCount
                                );

                                _ = dumpBuilder.AppendLine("    Owning Threads:");

                                foreach (var owner in blockingObject.Owners)
                                {
                                    _ = dumpBuilder.AppendLine(StringifyThreadInfo(owner, culture, 6));
                                }

                                _ = dumpBuilder.AppendLine("    Waiting Threads:");

                                foreach (var waiter in blockingObject.Waiters)
                                {
                                    _ = dumpBuilder.AppendLine(StringifyThreadInfo(waiter, culture, 6));
                                }
                            }
                        }

                        _ = dumpBuilder.AppendLine("  Call Stack:");
                        var stackFrames = thread.StackTrace.ToArray();
                        foreach (var stackFrame in stackFrames)
                        {
                            _ = dumpBuilder.AppendFormat(
                                culture,
                                "    {0,13} 0x{1,-16:X} 0x{2,-16:X} {3,20} {4,-4} 0x{5,-16:X} {6}\n",
                                stackFrame.Kind,
                                stackFrame.StackPointer,
                                stackFrame.InstructionPointer,
                                stackFrame.ModuleName,
                                stackFrame.Method?.CompilationType ?? MethodCompilationType.None,
                                stackFrame.Method?.IL?.Address ?? 0,
                                stackFrame.Method?.GetFullSignature() ?? "<unknown>"
                            );
                        }
                    }
                }
            }

            Log.Warn(dumpBuilder.ToString());
        }
    }
}
