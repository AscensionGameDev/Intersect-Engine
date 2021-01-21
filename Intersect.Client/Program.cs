using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace Intersect.Client
{

#if WINDOWS || LINUX
    /// <summary>
    ///     The main class.
    /// </summary>
    public static class Program
    {

        public static string OpenALLink = "";

        public static string OpenGLLink = "";

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CosturaUtility.Initialize();

            ExportDependencies();
            Assembly.LoadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MonoGame.Framework.Client.dll"));

            try
            {
                var type = Type.GetType("Intersect.Client.Core.Bootstrapper", true);
                Debug.Assert(type != null, "type != null");
                var method = type.GetMethod("Start");
                Debug.Assert(method != null, "method != null");
                method.Invoke(null, new object[] {args});
            }
            catch (Exception exception)
            {
                switch (exception.InnerException?.GetType().Name)
                {
                    case "NoSuitableGraphicsDeviceException":
                    {
                        var txt = "NoSuitableGraphicsDeviceException" + Environment.NewLine;
                        txt += exception.InnerException.ToString();
                        txt += exception.InnerException.InnerException?.ToString();
                        File.WriteAllText("gfxerror.txt", txt);

                        if (!string.IsNullOrEmpty(OpenGLLink))
                        {
                            Process.Start(OpenGLLink);
                        }

                        Environment.Exit(-1);
                        break;
                    }

                    case "NoAudioHardwareException":
                    {
                        if (!string.IsNullOrEmpty(OpenALLink))
                        {
                            Process.Start(OpenALLink);
                        }

                        Environment.Exit(-1);
                        break;
                    }
                }

                var type = Type.GetType("Intersect.Client.Core.ClientContext", true);
                Debug.Assert(type != null, "type != null");

                var staticMethodInfo = type.GetMethod(
                    "DispatchUnhandledException", BindingFlags.Static | BindingFlags.NonPublic
                );

                Debug.Assert(staticMethodInfo != null, nameof(staticMethodInfo) + " != null");

                staticMethodInfo.Invoke(null, new object[] {exception.InnerException ?? exception, true, true});
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
            try
            {
                Debug.Assert(name != null, "name != null");
                var p = new Process
                {
                    StartInfo =
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        FileName = name
                    }
                };

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
            catch
            {
                return "";
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
                    ExportDependency("libSDL2-2.0.0.dylib", "");
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
                        resourceStream.Read(data, 0, (int) resourceStream.Length);
                        fileStream.Write(data, 0, data.Length);
                    }
                }
            }
            else
            {
                var resourceStream = assembly.GetManifestResourceStream("Intersect Client.g.resources");
                Debug.Assert(resourceStream != null, "resourceStream != null");
                var resources = new ResourceSet(resourceStream);

                path = Path.Combine("resources", folder, filename.Split('.')[0].Split('-')[0]);

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
                        var memoryStream = (UnmanagedMemoryStream) enumerator.Value;
                        if (memoryStream == null)
                        {
                            continue;
                        }

                        var data = new byte[memoryStream.Length];
                        memoryStream.Read(data, 0, (int) memoryStream.Length);
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
#endif

}
