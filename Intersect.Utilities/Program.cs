using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using Intersect.Enums;
using Intersect.Utilities.Scripts;

namespace Intersect.Utilities
{

    class Program
    {

        private static readonly IDictionary<string, Script> Scripts;

        static Program()
        {
            Scripts = new Dictionary<string, Script>();
            RegisterScript(new Exit());
            RegisterScript(new Help());
            RegisterScript(new GenRsa());
        }

        static void RegisterScript(Script script)
        {
            Scripts.Add(script.Name, script);
        }

        private static void NameOfGeneric<TGeneric>()
        {
            Console.WriteLine(typeof(TGeneric).Name);
            Console.Read();
        }

        private static void Enumerators()
        {
            var defaults = new Dictionary<int, string>()
            {
                {0, "test"},
                {2, "test"},
                {3, "test"},
                {1, "test"}
            };

            var values = new Dictionary<int, string>()
            {
                {3, "taest"},
                {2, "taest"},
                {1, "taest"}
            };

            using (var enumeratorDefaults = defaults.GetEnumerator())
            {
                using (var enumeratorValues = values.GetEnumerator())
                {
                    Console.WriteLine(defaults.Count);
                    Console.WriteLine(values.Count);
                }
            }
        }

        static void Main(string[] args)
        {
            {
                var type = typeof(Access);
                var valueType = Access.Admin.GetType();
                var bet = typeof(ByteEnum);
                var iet = typeof(IntEnum);
                var toParseA = "Admin";
                var toParseN = "2";
                switch ((object) Access.Admin)
                {
                    case byte dbyte:
                        return;

                    case int dint:
                        return;

                    case Enum denum:
                        return;

                    default:
                        return;
                }

                return;
            }

            {
                Console.InputHistoryEnabled = true;
                Console.WaitPrefix = "> ";
                string line;
                while (null != (line = Console.ReadLine(true)))
                {
                    if (line == "exit")
                    {
                        break;
                    }

                    Console.WriteLine("Received: " + line);
                }
            }

            return;

            //Enumerators();
            //return;
            //Strings.Load();
            //return;

            NameOfGeneric<Program>();

            return;

            new Thread(
                () =>
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("321Test123");
                }
            ).Start();

            Console.WaitPrefix = "> ";
            Console.WriteLine("Test");
            Console.ReadLine(true);
            Console.Write("1234Test4321");
            new Thread(
                () =>
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("xyzzy90210");
                    Thread.Sleep(1000);
                    Console.WriteLine("noonoo");
                }
            ).Start();

            Console.ReadLine(true);

            return;

            if (Scripts.Count < 1)
            {
                Environment.Exit(-1);
            }

            Console.SetError(new Decorator(Console.Error));

            for (;;)
            {
                var line = Console.ReadLine();
                if (line == null)
                {
                    break;
                }

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var match = Script.CommandRegex.Match(line);
                var scriptName = match.Groups[1].Value;
                if (!Scripts.TryGetValue(scriptName, out var script))
                {
                    Console.Error?.WriteLine($"No available script with name '{scriptName}'.");

                    continue;
                }

                var scriptArgsGroup = match.Groups[2];
                var scriptArgsList = new List<string>(scriptArgsGroup.Captures.Count);
                scriptArgsList.AddRange(
                    from Capture capture in scriptArgsGroup.Captures
                    where !string.IsNullOrWhiteSpace(capture?.Value)
                    select capture.Value
                );

                var scriptArgs = scriptArgsList.ToArray();

                var result = script.Run(args, scriptArgs);
                switch (result.Code)
                {
                    case 0:
                        Console.Out?.WriteLine(result.Message ?? $"'{scriptName}' completed.");

                        break;

                    default:
                        Console.Error?.WriteLine($"'{scriptName}' failed with exit code {result.Code}.");
                        Console.Error?.WriteLine(result.Message);
                        if (result.Exception != null)
                        {
                            Console.Error?.WriteLine(result.Exception.StackTrace);
                        }

                        break;
                }
            }
        }

        private enum ByteEnum : byte
        {

            X,

            Z

        }

        private enum IntEnum : int
        {

            O,

            P

        }

        class Decorator : TextWriter
        {

            private TextWriter mWriter;

            public Decorator(TextWriter writer)
            {
                mWriter = writer;
            }

            public override Encoding Encoding => mWriter.Encoding;

            public override void WriteLine(string value)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                mWriter.WriteLine(value);
                Console.ForegroundColor = color;
            }

        }

    }

}
