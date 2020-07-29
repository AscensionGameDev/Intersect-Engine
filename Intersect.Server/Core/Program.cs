using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Intersect.Server.Core
{

    /// <summary>
    /// Please do not modify this without JC's approval! If namespaces are referenced that are not SYSTEM.* then the server won't run cross platform.
    /// If you want to add startup instructions see Classes/ServerStart.cs
    /// </summary>
    public static class Program
    {

        [STAThread]
        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            try
            {
                Type.GetType("Intersect.Server.Core.Bootstrapper")
                    ?.GetMethod("Start")
                    ?.Invoke(null, new object[] {args});
            }
            catch (Exception exception)
            {
                var type = Type.GetType("Intersect.Server.Core.ServerContext", true);
                Debug.Assert(type != null, "type != null");

                var staticMethodInfo = type.GetMethod("DispatchUnhandledException", BindingFlags.Static | BindingFlags.NonPublic);
                Debug.Assert(staticMethodInfo != null, nameof(staticMethodInfo) + " != null");

                staticMethodInfo.Invoke(null, new object[] { exception.InnerException ?? exception, true });
            }
        }

    }

}
