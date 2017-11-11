using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Intersect.Server.Classes
{
    public class MainClass
    {
        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            
            //Place sqlite3.dll where it's needed.
            var dllname = "sqlite3x64.dll";
            if (!Environment.Is64BitProcess)
            {
                dllname = "sqlite3x86.dll";
            }
            var resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Intersect.Server.resources." + dllname))
            {
                using (var fs = new FileStream("sqlite3.dll", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    var data = new byte[stream.Length];
                    stream.Read(data, 0, (int)stream.Length);
                    fs.Write(data,0,data.Length);
                }
            }
            object[] args1 = new object[1];
            args1[0] = args;
            Type.GetType("Intersect.Server.Classes.ServerStart").GetMethod("Start").Invoke(null, args1);
        }
    }
}