using System;
using System.IO;
using System.Text;

namespace Intersect.Logging
{

    public static class MemoryDump
    {

        private static string sDumpDirectory;

        static MemoryDump()
        {
            DumpDirectory = "logs/dumps";
        }

        public static string DumpDirectory
        {
            get => Path.Combine(Environment.CurrentDirectory, sDumpDirectory ?? "");
            set => sDumpDirectory = value;
        }

        private static void EnsureDirectory()
        {
            if (!Directory.Exists(DumpDirectory))
            {
                Directory.CreateDirectory(DumpDirectory);
            }
        }

        private static string GetDumpFilename()
        {
            return Path.Combine(DumpDirectory, $"{DateTime.Now:yyyy_MM_dd-HH_mm_ss_fff}.log");
        }

        public static bool Dump(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException();
            }

            var written = false;
            using (var writeStream = new FileStream(GetDumpFilename(), FileMode.CreateNew, FileAccess.Write))
            {
                if (data.Length > 0)
                {
                    writeStream.Write(data, 0, data.Length);
                    written = true;
                }
                else
                {
                    var bytes = Encoding.UTF8.GetBytes("There was no data to dump.");
                    writeStream.Write(bytes, 0, bytes.Length);
                }

                writeStream.Close();
            }

            return written;
        }

        public static bool Dump(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException();
            }

            var written = false;
            using (var writeStream = new FileStream(GetDumpFilename(), FileMode.CreateNew, FileAccess.Write))
            {
                if (stream.Length > 0)
                {
                    int read;
                    var buffer = new byte[4096];
                    while (0 < (read = stream.Read(buffer, 0, 4096)))
                    {
                        writeStream.Write(buffer, 0, read);
                    }

                    written = true;
                }
                else
                {
                    var bytes = Encoding.UTF8.GetBytes("There was no data to dump.");
                    writeStream.Write(bytes, 0, bytes.Length);
                }

                writeStream.Close();
            }

            return written;
        }

    }

}
