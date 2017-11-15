using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Intersect.Migration
{
    public static class MainClass
    {
        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

            //Place sqlite3.dll where it's needed.
            var dllname = Environment.Is64BitProcess ? "sqlite3x64.dll" : "sqlite3x86.dll";
            using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Intersect.Migration.Resources." + dllname))
            {
                Debug.Assert(resourceStream != null, "resourceStream != null");
                using (var fs = new FileStream("sqlite3.dll", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    var data = new byte[resourceStream.Length];
                    resourceStream.Read(data, 0, (int)resourceStream.Length);
                    fs.Write(data, 0, data.Length);
                }
            }

            Type.GetType("Intersect.Migration.Migrator")?.GetMethod("Start")?.Invoke(null, new object[] { args });
        }
    }
}