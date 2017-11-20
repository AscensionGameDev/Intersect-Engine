using System;
using System.Diagnostics;
using System.Globalization;
using Intersect.Logging;
using Intersect.Utilities;

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
            if (!ReflectionUtils.ExtractResource($"Intersect.Server.Resources.{dllname}", "sqlite3.dll"))
            {
                Log.Error("Failed to extract sqlite library, terminating startup.");
                Environment.Exit(-0x1000);
            }

            Type.GetType("Intersect.Server.Classes.ServerStart")?.GetMethod("Start")?.Invoke(null, new object[]{args});
        }
    }
}