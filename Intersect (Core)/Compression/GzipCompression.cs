using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Intersect.Compression
{
    /// <summary>
    /// Contains several wrapper methods used to generalize code in areas of the engine related to compressing and decompressing files and data.
    /// </summary>
    public static class GzipCompression
    {

        // Our cryptographic provider, and the key to be used to encrypt. Note, it NEEDS to be 16 characters or more!
        private static AesCryptoServiceProvider cryptoProvider;
        private static string cryptoKey = "T3ZncHUsIGdueHIgdmcgYmUgeXJuaXIgdmcu";

        /// <summary>
        /// Initialize our Cryptographic Provider and make it usable.
        /// </summary>
        private static void CreateProvider()
        {
            cryptoProvider = new AesCryptoServiceProvider();

            // Take a few bytes out of this delicious morsel and grow stronk.
            var keyBytes = ASCIIEncoding.ASCII.GetBytes(cryptoKey);
            cryptoProvider.Key = keyBytes.Take(16).ToArray();    
            cryptoProvider.IV = keyBytes.Reverse().Take(16).ToArray();
        }

        /// <summary>
        /// Read a decompressed unencrypted string from a specified file.
        /// </summary>
        /// <param name="fileName">The file to decompress.</param>
        /// <returns>Returns the decompressed file's content as a string.</returns>
        public static string ReadDecompressedString(string fileName)
        {
            using (var reader = new StreamReader(CreateDecompressedFileStream(fileName)))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Read a decompressed unencrypted stream from a specified file.
        /// </summary>
        /// <param name="fileName">The file to decompress.</param>
        /// <returns>Returns a decompressed <see cref="CryptoStream"/> of the file's content.</returns>
        public static CryptoStream CreateDecompressedFileStream(string fileName)
        {
            if (cryptoProvider == null)
            {
                CreateProvider();
            }

            return new CryptoStream(new GZipStream(File.OpenRead(fileName), CompressionMode.Decompress, false), cryptoProvider.CreateDecryptor(), CryptoStreamMode.Read);
        }

        /// <summary>
        /// Read decompressed unencrypted data from an existing filestream.
        /// </summary>
        /// <param name="stream">The Filestream to write data from.</param>
        /// <returns>Returns a decompressed <see cref="CryptoStream"/> of the stream's content.</returns>
        public static CryptoStream CreateDecompressedFileStream(FileStream stream)
        {
            if (cryptoProvider == null)
            {
                CreateProvider();
            }

            return new CryptoStream(new GZipStream(stream, CompressionMode.Decompress, false), cryptoProvider.CreateDecryptor(), CryptoStreamMode.Read);
        }

        /// <summary>
        /// Writes the given string to a compressed encrypted file.
        /// </summary>
        /// <param name="fileName">The file to write the string to.</param>
        /// <param name="data">The string to compress and write to the file.</param>
        public static void WriteCompressedString(string fileName, string data)
        {
            using (var stream = CreateCompressedFileStream(fileName))
            {
                var bytes = Encoding.UTF8.GetBytes(data);
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// Creates a compressed encrypted FileStream to write data to.
        /// </summary>
        /// <param name="fileName">The file to write the data to.</param>
        /// <returns>Returns a <see cref="CryptoStream"/> to write data to, saving compressed data to a file.</returns>
        public static CryptoStream CreateCompressedFileStream(string fileName)
        {
            if (cryptoProvider == null)
            {
                CreateProvider();
            }

            return new CryptoStream(new GZipStream(new FileStream(fileName, FileMode.Create), CompressionMode.Compress, false), cryptoProvider.CreateEncryptor(), CryptoStreamMode.Write);
        }

    }
}
