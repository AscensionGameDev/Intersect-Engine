using System;
using System.Globalization;

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
        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            Type.GetType("Intersect.Server.Classes.ServerStart")?.GetMethod("Start")?.Invoke(null, new object[]{args});
        }
    }
}