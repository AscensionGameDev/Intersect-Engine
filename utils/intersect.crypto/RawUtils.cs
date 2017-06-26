using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace Intersect.Crypto
{
    public class RawUtils
    {
        public void Write(RSAParameters parameters, string file, bool compressed = true)
        {
            using (var fileStream = new FileStream(file, FileMode.OpenOrCreate))
            {
                if (!compressed)
                {
                    Write(parameters, fileStream);
                    return;
                }

                using (var stream = new GZipStream(fileStream, CompressionMode.Compress))
                {
                    Write(parameters, stream);
                }
            }
        }

        public void Write(RSAParameters parameters, ref byte[] buffer)
        {
            using (var stream = new MemoryStream(buffer))
            {
                Write(parameters, stream);

                buffer = stream.ToArray();
            }
        }

        public void Write(RSAParameters parameters, Stream stream)
        {
            var isPublic = (parameters.D == null) ||
                           (parameters.DP == null) ||
                           (parameters.DQ == null) ||
                           (parameters.InverseQ == null) ||
                           (parameters.P == null) ||
                           (parameters.Q == null);

            var bits = parameters.Modulus.Length * 8;
            var packed = (bits & 0x7FFF) | (isPublic ? 0x8000 : 0x0000);

            using (var writer = new BinaryWriter(stream))
            {
                writer.Write((short) packed);
                writer.Write(parameters.Exponent, 0, 3);
                writer.Write(parameters.Modulus, 0, bits / 8);

                if (isPublic) return;

                writer.Write(parameters.D, 0, bits / 8);
                writer.Write(parameters.DP, 0, bits / 16);
                writer.Write(parameters.DQ, 0, bits / 16);
                writer.Write(parameters.InverseQ, 0, bits / 16);
                writer.Write(parameters.P, 0, bits / 16);
                writer.Write(parameters.Q, 0, bits / 16);
            }
        }

        public RSAParameters Read(string file, bool compressed = true)
        {
            using (var fileStream = new FileStream(file, FileMode.Open))
            {
                if (!compressed) return Read(fileStream);
                using (var stream = new GZipStream(fileStream, CompressionMode.Decompress))
                {
                    return Read(stream);
                }
            }
        }

        public RSAParameters Read(byte[] buffer)
        {
            using (var stream = new MemoryStream(buffer))
            {
                return Read(stream);
            }
        }

        public RSAParameters Read(Stream stream)
        {
            using (var reader = new BinaryReader(stream))
            {
                var packed = reader.ReadInt16();
                var bits = packed & 0x7FFF;
                var isPublic = 0 != (packed & 0x8000);

                var parameters = new RSAParameters
                {
                    Exponent = reader.ReadBytes(3),
                    Modulus = reader.ReadBytes(bits / 8),
                };

                if (isPublic) return parameters;

                parameters.D = reader.ReadBytes(bits / 8);
                parameters.DP = reader.ReadBytes(bits / 16);
                parameters.DQ = reader.ReadBytes(bits / 16);
                parameters.InverseQ = reader.ReadBytes(bits / 16);
                parameters.P = reader.ReadBytes(bits / 16);
                parameters.Q = reader.ReadBytes(bits / 16);

                return parameters;
            }
        }
    }
}