using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

// TODO: Move or change the namespace?
// ReSharper disable once CheckNamespace
namespace Intersect.Server.Classes
{
    /// <summary>
    /// Please do not modify this without JC's approval! If namespaces are referenced that are not SYSTEM.* then the server won't run cross platform.
    /// If you want to add startup instructions see Classes/ServerStart.cs
    /// </summary>
    public static class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            try
            {
                Type.GetType("Intersect.Server.Classes.ServerStart")?.GetMethod("Start")
                    ?.Invoke(null, new object[] {args});
            }
            catch (Exception ex)
            {
                var type = Type.GetType("Intersect.Server.Classes.ServerStart", true);
                Debug.Assert(type != null, "type != null");
                MethodInfo staticMethodInfo = type.GetMethod("CurrentDomain_UnhandledException");
                staticMethodInfo.Invoke(null, new object[] { null, new UnhandledExceptionEventArgs(ex.InnerException != null ? ex.InnerException : ex, true) });
            }
        }
    }
}