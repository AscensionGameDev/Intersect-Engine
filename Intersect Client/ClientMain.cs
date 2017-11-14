using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace Intersect.Client
{
#if WINDOWS || LINUX
    /// <summary>
    ///     The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CosturaUtility.Initialize();

            ExportDependencies();
            Assembly.LoadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MonoGame.Framework.Client.dll"));

            // Get the type contained in the name string
            Type type = Type.GetType("Intersect.Client.IntersectGame", true);

            // create an instance of that type
            object instance = Activator.CreateInstance(type);

            try
            {
                Type[] prms = new Type[0];
                var methodinfo = type.GetMethod("Run",prms);
                methodinfo.Invoke(instance, null);
            }
            catch (PlatformNotSupportedException)
            {
                System.Windows.Forms.MessageBox.Show("OpenGL Initialialization Error! Try updating your graphics drivers!");
            }
        }

        public static void ClearDlls()
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
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = name;
                p.Start();
                // Do not wait for the child process to exit before
                // reading to the end of its redirected stream.
                // p.WaitForExit();
                // Read the output stream first and then wait.
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                if (output == null) output = "";
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

                default:
                    ExportDependency("libopenal.so.1", folder);
                    ExportDependency("libSDL2-2.0.so.0", folder);
                    ExportDependency("openal32.dll", "");
                    ExportDependency("MonoGame.Framework.dll.config", "", "MonoGame.Framework.Client.dll.config");
                    break;
            }

            ExportDependency("MonoGame.Framework.dll", "", "MonoGame.Framework.Client.dll");
        }

        private static void ExportDependency(string filename, string folder, string nameoverride = null)
        {
            if (!DeleteIfExists(string.IsNullOrEmpty(nameoverride) ? filename : nameoverride))
            {
                /* If it failed it means the file already exists and can't be deleted for whatever reason. */
                return;
            }
            var res = "Intersect.Client.Resources." + (string.IsNullOrEmpty(folder) ? "" : folder + ".") + filename;
            if (Assembly.GetExecutingAssembly().GetManifestResourceNames().Contains(res))
            {
                Console.WriteLine("Resource: " + res);
                var ass = Assembly.GetExecutingAssembly();
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(res))
                {
                    using (var fs = new FileStream(string.IsNullOrEmpty(nameoverride) ? filename : nameoverride, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        var data = new byte[stream.Length];
                        stream.Read(data, 0, (int) stream.Length);
                        fs.Write(data, 0, data.Length);
                    }
                }
            }
            else
            {
                ResourceSet resources = new ResourceSet(Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("Intersect Client.g.resources"));

                var path = "resources/" + folder + "/" + filename.Split(".".ToCharArray())[0].Split("-".ToCharArray())[0];
                path = path.ToLower();
                IDictionaryEnumerator enumerator =
                    resources.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    Console.WriteLine(enumerator.Key);
                    if (enumerator.Key != null && enumerator.Key.ToString().Trim() == path.Trim())
                    {
                        using (var fs = new FileStream(string.IsNullOrEmpty(nameoverride) ? filename : nameoverride, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            var data = new byte[((System.IO.UnmanagedMemoryStream) enumerator.Value).Length];
                            ((System.IO.UnmanagedMemoryStream) enumerator.Value).Read(data, 0,
                                (int) ((System.IO.UnmanagedMemoryStream) enumerator.Value).Length);
                            fs.Write(data, 0, data.Length);
                        }
                    }
                }

            }
        }

        static bool DeleteIfExists(string filename)
        {
            try
            {
                if (File.Exists(filename))
                    File.Delete(filename);
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }
    }
#endif
}