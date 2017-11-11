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

            //Delete any files that exist
            DeleteIfExists("libopenal.so.1");
            DeleteIfExists("libSDL2-2.0.so.0");
            DeleteIfExists("SDL2.dll");
            DeleteIfExists("soft_oal.dll");
            DeleteIfExists("libopenal.1.dylib");
            DeleteIfExists("libSDL2-2.0.0.dylib");
            DeleteIfExists("openal32.dll");
            DeleteIfExists("MonoGame.Framework.dll.config");

            OperatingSystem os = Environment.OSVersion;
            PlatformID pid = os.Platform;
            Console.WriteLine(pid);
            if (pid == PlatformID.Unix)
            {
                string UnixName = ReadProcessOutput("uname");
                if (UnixName.Contains("Darwin"))
                {
                    pid = PlatformID.MacOSX;
                }
            }
            switch (pid)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    if (Environment.Is64BitProcess)
                    {
                        ExportDependency("SDL2.dll", "x64");
                        ExportDependency("soft_oal.dll", "x64");
                    }
                    else
                    {
                        ExportDependency("SDL2.dll", "x86");
                        ExportDependency("soft_oal.dll", "x86");
                    }
                    break;
                case PlatformID.MacOSX:
                    ExportDependency("libopenal.1.dylib", "");
                    ExportDependency("libSDL2-2.0.0.dylib", "");
                    ExportDependency("openal32.dll", "");
                    ExportDependency("MonoGame.Framework.dll.config","");
                    break;
                default:
                    if (Environment.Is64BitProcess)
                    {
                        ExportDependency("libopenal.so.1", "x64");
                        ExportDependency("libSDL2-2.0.so.0", "x64");
                    }
                    else
                    {
                        ExportDependency("libopenal.so.1", "x86");
                        ExportDependency("libSDL2-2.0.so.0", "x86");
                    }
                    ExportDependency("MonoGame.Framework.dll.config", "");
                    ExportDependency("openal32.dll", "");
                    break;
            }
            ExportDependency("MonoGame.Framework.dll", "");

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

        private static void ExportDependency(string filename, string folder)
        {
            var res = "Intersect.Client.Resources." + (string.IsNullOrEmpty(folder) ? "" : folder + ".") + filename;
            if (Assembly.GetExecutingAssembly().GetManifestResourceNames().Contains(res))
            {
                Console.WriteLine("Resource: " + res);
                var ass = Assembly.GetExecutingAssembly();
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(res))
                {
                    using (var fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite))
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
                        using (var fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite))
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

        static void DeleteIfExists(string filename)
        {
            if (File.Exists(filename))
                File.Delete(filename);
        }
    }
#endif
}