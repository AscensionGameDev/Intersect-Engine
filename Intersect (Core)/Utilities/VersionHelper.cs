using System;
using System.Reflection;

namespace Intersect.Utilities
{

    public static class VersionHelper
    {

        public static Version LibraryVersion => Assembly.GetAssembly(typeof(VersionHelper))?.GetName().Version;

        public static Version ExecutableVersion => Assembly.GetEntryAssembly()?.GetName().Version ??
                                                   Assembly.GetExecutingAssembly().GetName().Version;

    }

}
