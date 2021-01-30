using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;

using Intersect.Extensions;
using Intersect.Logging;

namespace Intersect.IO.Files
{
    public static class FileSystemHelper
    {
        private static readonly string[] SizePrefixes = {"", "K", "M", "G", "T", "P", "E"};

        internal static IFileSystem FileSystem { get; set; } = new FileSystem();

        public static string FormatSize(long bytes, bool binary = false)
        {
            double sizeBase = binary ? 1024 : 1000;
            var group = Math.Max(0, (long) Math.Floor(Math.Log(Math.Abs(Math.Max(-long.MaxValue, bytes)), sizeBase)));

            var prefix = SizePrefixes[group] + (binary && group != 0 ? "i" : "");

            var fractional = bytes / Math.Max(1, Math.Pow(sizeBase, group));

            return $"{fractional:0.###}{prefix}B";
        }

        /// <summary>
        /// Checks to see if the directory exists, and creates it otherwise.
        /// </summary>
        /// <param name="directoryPath">the directory path to check</param>
        /// <returns>
        /// <code>false</code> if the path is a file or creation fails
        /// <code>true</code> if the path is a directory or creation succeeded
        /// </returns>
        public static bool EnsureDirectoryExists(string directoryPath)
        {
            Debug.Assert(FileSystem.Directory != null, "FileSystem.Directory != null");

            if (FileSystem.Directory.Exists(directoryPath))
            {
                return true;
            }

            if (File.Exists(directoryPath))
            {
                return false;
            }

            try
            {
                return FileSystem.Directory.CreateDirectory(directoryPath).Exists;
            }
            catch (Exception exception)
            {
                Log.Error(exception);

                return false;
            }
        }

        public static string RelativePath(string from, string to)
        {
            Contract.Requires(from != null);
            Contract.Requires(to != null);

            if (string.IsNullOrWhiteSpace(from))
            {
                throw new ArgumentException(nameof(from));
            }

            if (string.IsNullOrWhiteSpace(to))
            {
                throw new ArgumentException(nameof(to));
            }

            var fullFrom = CleanPath(Path.GetFullPath(from));

            var uriFrom = new Uri($"file://{fullFrom}");

            var fullTo = CleanPath(Path.GetFullPath(to));

            var uriTo = new Uri($"file://{fullTo}");

            var uriRelative = uriFrom.MakeRelativeUri(uriTo);

            var relative = Uri.UnescapeDataString(uriRelative.ToString())
                .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            return relative;
        }

        public static string CleanPath(string path)
        {
            Contract.Requires(path != null);
            var cleaned = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            return cleaned.TerminateWith(Path.AltDirectorySeparatorChar, !IsFile(cleaned, false));
        }

        public static bool IsFile(string path) => IsFile(path, true);

        private static bool IsFile(string path, bool doClean)
        {
            Contract.Requires(path != null);

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(nameof(path));
            }

            var cleaned = doClean ? CleanPath(path) : path;

            if (cleaned.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
            {
                return false;
            }

            if (Path.HasExtension(cleaned))
            {
                return true;
            }

            try
            {
                Debug.Assert(FileSystem.File != null, "FileSystem.File != null");
                var attributes = FileSystem.File.GetAttributes(cleaned);

                return !attributes.HasFlag(FileAttributes.Directory);
            }
            catch
            {
                return true;
            }
        }

        public static string WriteToTemporaryFolder(string name, Stream stream)
        {
            var temporaryDirectoryPath = Path.Combine(Path.GetTempPath(), Assembly.GetEntryAssembly().GetName().Name);
            if (!EnsureDirectoryExists(temporaryDirectoryPath))
            {
                throw new Exception();
            }

            var temporaryFilePath = Path.Combine(temporaryDirectoryPath, name);

            var buffer = new byte[4096];
            int read;
            using (var temporaryFileStream = File.OpenWrite(temporaryFilePath))
            {
                while (0 != (read = stream.Read(buffer, 0, buffer.Length)))
                {
                    temporaryFileStream.Write(buffer, 0, read);
                }
            }
            
            return temporaryFilePath;
        }
    }
}
