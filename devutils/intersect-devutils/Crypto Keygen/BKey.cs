using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace Crypto_Keygen
{
    public class BKey
    {
        public RSACryptoServiceProvider ReadPublic(string file)
        {
            using (var publicBKeyReader = new BinaryReader(new GZipStream(new FileStream(file, FileMode.Open), CompressionMode.Decompress)))
            {
                var bits = publicBKeyReader.ReadInt16();

                var pk = new RSAParameters
                {
                    Exponent = publicBKeyReader.ReadBytes(3),
                    Modulus = publicBKeyReader.ReadBytes(bits / 8)
                };

                publicBKeyReader.Close();

                var rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(pk);
                return rsa;
            }
        }

        public RSACryptoServiceProvider ReadPrivate(string file)
        {
            using (var privateBKeyReader = new BinaryReader(new GZipStream(new FileStream(file, FileMode.Open), CompressionMode.Decompress)))
            {
                var bits = privateBKeyReader.ReadInt16();

                var pk = new RSAParameters
                {
                    D = privateBKeyReader.ReadBytes(bits / 8),
                    DP = privateBKeyReader.ReadBytes(bits / 16),
                    DQ = privateBKeyReader.ReadBytes(bits / 16),
                    Exponent = privateBKeyReader.ReadBytes(3),
                    InverseQ = privateBKeyReader.ReadBytes(bits / 16),
                    Modulus = privateBKeyReader.ReadBytes(bits / 8),
                    P = privateBKeyReader.ReadBytes(bits / 16),
                    Q = privateBKeyReader.ReadBytes(bits / 16)
                };

                privateBKeyReader.Close();

                var rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(pk);
                return rsa;
            }
        }
    }
}