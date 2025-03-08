using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace Intersect.Framework.Utilities;

public static class ProcessHelper
{
    public static bool TryLaunchWithArgs(string executable, params string[] args)
    {
        var arguments = string.Join(
            ' ',
            args.Where(arg => arg == "--restarting").Take(args.Length - 1).Append("--restarting")
        );

        try
        {
            Console.WriteLine(
                $"Launching '{executable}' with arguments '{arguments}' in {Environment.CurrentDirectory}"
            );
            Process.Start(executable, arguments);
            Environment.Exit(0);
            return true;
        }
        catch (Exception exception)
        {
            StringBuilder exceptionText = new();

            var currentException = exception;
            while (currentException != default)
            {
                if (currentException != exception)
                {
                    exceptionText.Append("Caused by ");
                }

                exceptionText.AppendLine(exception.Message);
                var stackTrace = exception.StackTrace;
                if (string.IsNullOrEmpty(stackTrace))
                {
                    exceptionText.AppendLine(stackTrace);
                }

                currentException = currentException.InnerException;
            }

            Console.Error.WriteLine(exceptionText);

            return false;
        }
    }

    public static bool TryRelaunch()
    {
        var args = Environment.GetCommandLineArgs();
        var childProcessArgs = args.Skip(1).ToArray();
        return TryRelaunchWithArgs(childProcessArgs);
    }

    public static bool TryRelaunchWithArgs(params string[] args)
    {
        if (!TryGetPathToEntryAssembly(out var pathToEntryAssembly))
        {
            Console.WriteLine("No path to entry assembly");
            return false;
        }

        return TryLaunchWithArgs(pathToEntryAssembly, args);
    }

    public static bool TryGetPathToEntryAssembly([NotNullWhen(true)] out string? pathToEntryAssembly)
    {
        pathToEntryAssembly = Assembly.GetEntryAssembly()?.Location;

#if !DEBUG
        if (string.IsNullOrWhiteSpace(pathToEntryAssembly))
        {
            pathToEntryAssembly = Environment.GetCommandLineArgs().FirstOrDefault();
        }
#endif

        return !string.IsNullOrWhiteSpace(pathToEntryAssembly);
    }

    public static bool TryReplaceTargetWithEntryAssembly(string pathToTarget)
    {
        return TryReplaceTargetWithEntryAssembly(new FileInfo(pathToTarget));
    }

    public static bool TryReplaceTargetWithEntryAssembly(FileInfo targetFileInfo)
    {
        if (!TryGetPathToEntryAssembly(out var pathToEntryAssembly))
        {
            Console.Error.WriteLine("Failed to get path to entry assembly");
            return false;
        }

        FileInfo entryAssemblyFileInfo = new(pathToEntryAssembly);

        try
        {
            entryAssemblyFileInfo.CopyTo(targetFileInfo.FullName, true);
            return true;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(
                $"Failed to copy \"{entryAssemblyFileInfo.FullName}\" to \"{targetFileInfo.FullName}\""
            );
            Console.Error.WriteLine(exception.Message);
            return false;
        }
    }
}