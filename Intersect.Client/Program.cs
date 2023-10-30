using Intersect.Logging;
using Intersect.Utilities;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Intersect.Client.Core;
using Intersect.Configuration;
using Intersect.Extensions;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Client
{
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
        internal static void Main(string[] args)
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
                var type = Type.GetType("Intersect.Client.Core.Bootstrapper", true);
                Debug.Assert(type != null, "type != null");
                var method = type.GetMethod("Start");
                Debug.Assert(method != null, "method != null");
                method.Invoke(null, new object[] { args });
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
                Log.Warn(exception);
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

            var folder = Environment.Is64BitProcess ? "x64" : "x86";

            switch (platformId)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    ExportDependency("SDL2.dll", folder);
                    ExportDependency("soft_oal.dll", folder);

                    break;

                case PlatformID.MacOSX:
                    ExportDependency("libopenal.1.dylib", "");
                    ExportDependency("libSDL2.dylib", "");
                    ExportDependency("openal32.dll", "");
                    ExportDependency("MonoGame.Framework.dll.config", "", "MonoGame.Framework.Client.dll.config");

                    break;

                case PlatformID.Xbox:
                    break;

                case PlatformID.Unix:
                default:
                    ExportDependency("libopenal.so.1", "");
                    ExportDependency("libSDL2-2.0.so.0", "");
                    ExportDependency("openal32.dll", "");
                    ExportDependency("MonoGame.Framework.dll.config", "", "MonoGame.Framework.Client.dll.config");

                    break;
            }

            ExportDependency("MonoGame.Framework.dll", "", "MonoGame.Framework.Client.dll");
        }

        private static void ExportDependency(string filename, string folder, string nameoverride = null)
        {
            /* If it failed it means the file already exists and can't be deleted for whatever reason. */
            var path = string.IsNullOrEmpty(nameoverride) ? filename : nameoverride;
            if (!DeleteIfExists(path))
            {
                return;
            }

            Debug.Assert(filename != null, "filename != null");

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName =
                $"Intersect.Client.Resources.{(string.IsNullOrEmpty(folder) ? "" : $"{folder}.")}{filename}";

            if (assembly.GetManifestResourceNames().Contains(resourceName))
            {
                Console.WriteLine($@"Resource: {resourceName}");
                using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    Debug.Assert(resourceStream != null, "resourceStream != null");
                    using (var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        var data = new byte[resourceStream.Length];
                        resourceStream.Read(data, 0, (int)resourceStream.Length);
                        fileStream.Write(data, 0, data.Length);
                    }
                }
            }
            else
            {
                Log.Warn($"Was looking for '{resourceName}' but only the following resources were found:\n{string.Join("\n\t", assembly.GetManifestResourceNames())}");
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

                    using (var fs = new FileStream(
                        string.IsNullOrEmpty(nameoverride) ? filename : nameoverride, FileMode.OpenOrCreate,
                        FileAccess.ReadWrite
                    ))
                    {
                        var memoryStream = (UnmanagedMemoryStream)enumerator.Value;
                        if (memoryStream == null)
                        {
                            continue;
                        }

                        var data = new byte[memoryStream.Length];
                        memoryStream.Read(data, 0, (int)memoryStream.Length);
                        fs.Write(data, 0, data.Length);
                    }
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

}
