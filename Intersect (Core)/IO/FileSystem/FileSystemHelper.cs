using System;
using System.IO;

using Intersect.Logging;

using JetBrains.Annotations;

namespace Intersect.IO.FileSystem
{

    public static class FileSystemHelper
    {

        /// <summary>
        /// Checks to see if the directory exists, and creates it otherwise.
        /// </summary>
        /// <param name="directoryPath">the directory path to check</param>
        /// <returns>
        /// <code>false</code> if the path is a file or creation fails
        /// <code>true</code> if the path is a directory or creation succeeded
        /// </returns>
        public static bool EnsureDirectoryExists([NotNull] string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                return true;
            }

            if (File.Exists(directoryPath))
            {
                return false;
            }

            try
            {
                return Directory.CreateDirectory(directoryPath).Exists;
            }
            catch (Exception exception)
            {
                Log.Error(exception);

                return false;
            }
        }

    }

}
