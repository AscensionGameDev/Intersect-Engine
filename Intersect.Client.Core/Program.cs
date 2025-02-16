
using Intersect.Utilities;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Intersect.Client.ThirdParty;
using Intersect.Configuration;
using Intersect.Core;
using Intersect.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Client.Core;

/// <summary>
///     The main class.
/// </summary>
static class Program
{
    public static string OpenALLink { get; set; }= string.Empty;

    public static string OpenGLLink { get; set; }= string.Empty;

    /// <summary>
    ///     The main entry point for the application.
    /// </summary>
    [STAThread]
    internal static void Main(Assembly entryAssembly, string[] args)
    {
        var waitForDebugger = args.Contains("--debugger");

        while (waitForDebugger && !Debugger.IsAttached)
        {
            System.Console.WriteLine("Waiting for debugger, sleeping 5000ms...");
            Thread.Sleep(5000);
        }

        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

        ExportDependencies();
        Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, "MonoGame.Framework.Client.dll"));

        try
        {
            Bootstrapper.Start(entryAssembly, args);
        }
        catch (NoSuitableGraphicsDeviceException noSuitableGraphicsDeviceException)
        {
            try
            {
                var message = noSuitableGraphicsDeviceException.AsFullStackString();
                Console.Error.WriteLine(message);
                File.WriteAllText("graphics-error.txt", message);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception);
            }
            finally
            {
                if (!string.IsNullOrEmpty(OpenGLLink))
                {
                    BrowserUtils.Open(OpenGLLink);
                }

                Environment.Exit(-1);
            }
        }
        catch (NoAudioHardwareException noAudioHardwareException)
        {
            try
            {
                var message = noAudioHardwareException.AsFullStackString();
                Console.Error.WriteLine(message);
                File.WriteAllText("audio-error.txt", message);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception);
            }
            finally
            {
                if (!string.IsNullOrEmpty(OpenALLink))
                {
                    BrowserUtils.Open(OpenALLink);
                }

                Environment.Exit(-1);
            }
        }
        catch (Exception exception)
        {
            ClientContext.DispatchUnhandledException(exception, true, true);
        }
    }

    private static void ClearDlls()
    {
        //Delete any files that exist
        DeleteIfExists("libopenal.so.1");
        DeleteIfExists("libSDL2-2.0.so.0");
        DeleteIfExists("SDL2.dll");
        DeleteIfExists("soft_oal.dll");
        DeleteIfExists("libopenal.1.dylib");
        DeleteIfExists("libSDL2-2.0.0.dylib");
        DeleteIfExists("openal32.dll");
        DeleteIfExists("MonoGame.Framework.Client.dll.config");
        DeleteIfExists("MonoGame.Framework.Client.dll");

        DeleteIfExists("libsdkencryptedappticket.dylib");
        DeleteIfExists("libsteam_api.dylib");
        DeleteIfExists("libsdkencryptedappticket.so");
        DeleteIfExists("libsteam_api.so");
        DeleteIfExists("sdkencryptedappticket64.dll");
        DeleteIfExists("steam_api64.dll");
    }

    private static string ReadProcessOutput(string name)
    {
        Debug.Assert(name != null, "name != null");
        var processStartInfo = new ProcessStartInfo
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            FileName = name
        };

        try
        {

            using (var p = new Process { StartInfo = processStartInfo })
            {
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
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Error reading process output from '{Name}'",
                name
            );
            return string.Empty;
        }
    }

    private static void ExportDependencies()
    {
        // Delete any files that exist
        ClearDlls();

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

        if (!Environment.Is64BitProcess)
        {
            throw new PlatformNotSupportedException("x86 (32-bit) systems are not supported.");
        }

        switch (platformId)
        {
            case PlatformID.Win32NT:
            case PlatformID.Win32S:
            case PlatformID.Win32Windows:
            case PlatformID.WinCE:
                ExportDependency("SDL2.dll", "x64");
                ExportDependency("soft_oal.dll", "x64");
                if (Steam.SupportedAttribute.IsPresent(typeof(Program).Assembly))
                {
                    ExportDependency("sdkencryptedappticket64.dll", "runtimes/win-x64/native");
                    ExportDependency("steam_api64.dll", "runtimes/win-x64/native");
                }
                break;

            case PlatformID.MacOSX:
                ExportDependency("libopenal.1.dylib");
                ExportDependency("libSDL2.dylib");
                ExportDependency("openal32.dll");
                ExportDependency("MonoGame.Framework.dll.config", nameoverride: "MonoGame.Framework.Client.dll.config");
                if (Steam.SupportedAttribute.IsPresent(typeof(Program).Assembly))
                {
                    ExportDependency("libsdkencryptedappticket.dylib", "runtimes/osx/native");
                    ExportDependency("libsteam_api.dylib", "runtimes/osx/native");
                }
                break;

            case PlatformID.Xbox:
                break;

            case PlatformID.Unix:
            default:
                ExportDependency("libopenal.so.1");
                ExportDependency("libSDL2-2.0.so.0");
                ExportDependency("openal32.dll");
                ExportDependency("MonoGame.Framework.dll.config", nameoverride: "MonoGame.Framework.Client.dll.config");
                if (Steam.SupportedAttribute.IsPresent(typeof(Program).Assembly))
                {
                    ExportDependency("libsdkencryptedappticket.so", "runtimes/linux-x64/native");
                    ExportDependency("libsteam_api.so", "runtimes/linux-x64/native");
                }
                break;
        }

        ExportDependency("MonoGame.Framework.dll", "", "MonoGame.Framework.Client.dll");
    }

    private static void ExportDependency(string filename, string? folder = default, string? nameoverride = default)
    {
        /* If it failed it means the file already exists and can't be deleted for whatever reason. */
        var path = string.IsNullOrEmpty(nameoverride) ? filename : nameoverride;
        if (!DeleteIfExists(path))
        {
            return;
        }

        Debug.Assert(filename != null, "filename != null");

        var assembly = Assembly.GetExecutingAssembly();
        var cleanFolder = folder?.Trim().Replace('/', '.').Replace('\\', '.').Replace('-', '_') ?? string.Empty;
        if (cleanFolder.Length > 0)
        {
            cleanFolder += '.';
        }
        var resourceName = $"Intersect.Client.Resources.{cleanFolder}{filename}";

        if (assembly.GetManifestResourceNames().Contains(resourceName))
        {
            Console.WriteLine($@"Resource: {resourceName}");
            using var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            Debug.Assert(resourceStream != null, "resourceStream != null");
            using var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            var data = new byte[resourceStream.Length];
            resourceStream.Read(data, 0, (int)resourceStream.Length);
            fileStream.Write(data, 0, data.Length);
        }
        else
        {
            ApplicationContext.Context.Value?.Logger.LogWarning($"Was looking for '{resourceName}' but only the following resources were found:\n{string.Join("\n\t", assembly.GetManifestResourceNames())}");
            var resourceStream = assembly.GetManifestResourceStream("Intersect Client.g.resources");
            Debug.Assert(resourceStream != null, "resourceStream != null");
            var resources = new ResourceSet(resourceStream);

            path = Path.Combine(ClientConfiguration.ResourcesDirectory, folder, filename.Split('.')[0].Split('-')[0]);

            path = path.ToLower();

            var enumerator = resources.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Console.WriteLine(enumerator.Key);
                if (enumerator.Key == null || enumerator.Key.ToString().Trim() != path.Trim())
                {
                    continue;
                }

                using var fs = new FileStream(
                    string.IsNullOrEmpty(nameoverride) ? filename : nameoverride, FileMode.OpenOrCreate,
                    FileAccess.ReadWrite
                );
                var memoryStream = (UnmanagedMemoryStream)enumerator.Value;
                if (memoryStream == null)
                {
                    continue;
                }

                var data = new byte[memoryStream.Length];
                var read = memoryStream.Read(data, 0, (int)memoryStream.Length);
                Debug.Assert(read == memoryStream.Length);
                fs.Write(data, 0, read);
            }
        }
    }

    private static bool DeleteIfExists(string filename)
    {
        try
        {
            Debug.Assert(filename != null, "filename != null");
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

}
