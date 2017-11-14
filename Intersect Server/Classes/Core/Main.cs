using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

// TODO: Move or change the namespace?
// ReSharper disable once CheckNamespace
namespace Intersect.Server.Classes
{
    public static class MainClass
    {
        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            
            //Place sqlite3.dll where it's needed.
            var dllname = Environment.Is64BitProcess? "sqlite3x64.dll" : "sqlite3x86.dll";
            using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Intersect.Server.resources." + dllname))
            {
                Debug.Assert(resourceStream != null, "resourceStream != null");
                using (var fileStream = new FileStream("sqlite3.dll", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    var data = new byte[resourceStream.Length];
                    resourceStream.Read(data, 0, (int)resourceStream.Length);
                    fileStream.Write(data,0,data.Length);
                }
            }

            Type.GetType("Intersect.Server.Classes.ServerStart")?.GetMethod("Start")?.Invoke(null, new object[]{args});
        }
    }
}