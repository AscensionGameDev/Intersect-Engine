using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Intersect.Utilities.Scripts;

namespace Intersect.Utilities
{
    class Program
    {
        private static readonly IDictionary<string, Script> mScripts;

        static Program()
        {
            mScripts = new Dictionary<string, Script>();
            RegisterScript(new Exit());
            RegisterScript(new Help());
            RegisterScript(new GenRsa());
        }

        static void RegisterScript(Script script)
        {
            mScripts.Add(script.Name, script);
        }

        class Decorator : TextWriter
        {
            private TextWriter mWriter;

            public override Encoding Encoding
                => mWriter.Encoding;

            public Decorator(TextWriter writer)
            {
                mWriter = writer;
            }

            public override void WriteLine(string value)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                mWriter.WriteLine(value);
                Console.ForegroundColor = color;
            }
        }

        static void Main(string[] args)
        {
            if (mScripts.Count < 1)
            {
                Environment.Exit(-1);
            }

            Console.SetError(new Decorator(Console.Error));

            for (;;)
            {
                var line = Console.ReadLine();
                if (line == null) break;
                if (string.IsNullOrWhiteSpace(line)) continue;

                var match = Script.COMMAND_REGEX.Match(line);
                var scriptName = match.Groups[1].Value;
                if (!mScripts.TryGetValue(scriptName, out Script script))
                {
                    Console.Error?.WriteLine($"No available script with name '{scriptName}'.");
                    continue;
                }

                var scriptArgsGroup = match.Groups[2];
                var scriptArgsList = new List<string>(scriptArgsGroup.Captures.Count);
                scriptArgsList.AddRange(
                    from Capture capture
                    in scriptArgsGroup.Captures
                    where !string.IsNullOrWhiteSpace(capture?.Value)
                    select capture.Value);
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
                            Console.Error?.WriteLine(result.Exception.StackTrace);
                        break;
                }
            }
        }
    }
}
