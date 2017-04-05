using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace Intersect.Crypto
{
    public class GenRsa
    {
        public int Bits { get; set; }

        public GenRsa(int bits = 4096)
        {
            Bits = bits;
        }

        public PrivateKeyPair Generate()
        {
            using (var rsa = new RSACryptoServiceProvider(Bits))
            {
                var timestamp = $"{DateTime.Now:yyyyMMddHHmmssfff}";

                using (var privateWriter = new StreamWriter(new FileStream($"{timestamp}_private.key", FileMode.CreateNew)))
                {
                    Pem.ExportPrivateKey(rsa, privateWriter);
                    privateWriter.Close();
                }

                using (var privateBKeyWriter = new BinaryWriter(new GZipStream(new FileStream($"{timestamp}_private.bkey", FileMode.CreateNew), CompressionMode.Compress)))
                {
                    var pk = rsa.ExportParameters(true);

                    privateBKeyWriter.Write((short) (pk.Modulus.Length * 8));
                    privateBKeyWriter.Write(pk.D);
                    privateBKeyWriter.Write(pk.DP);
                    privateBKeyWriter.Write(pk.DQ);
                    privateBKeyWriter.Write(pk.Exponent);
                    privateBKeyWriter.Write(pk.InverseQ);
                    privateBKeyWriter.Write(pk.Modulus);
                    privateBKeyWriter.Write(pk.P);
                    privateBKeyWriter.Write(pk.Q);
                    privateBKeyWriter.Close();
                }

                /*using (var privateBKeyReader = new BinaryReader(new FileStream($"{timestamp}_private.bkey", FileMode.Open)))
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

                    var newRsa = new RSACryptoServiceProvider();
                    newRsa.ImportParameters(pk);
                }*/

                using (var publicWriter = new StreamWriter(new FileStream($"{timestamp}_public.key", FileMode.CreateNew)))
                {
                    Pem.ExportPublicKey(rsa, publicWriter);
                    publicWriter.Close();
                }

                using (var publicBKeyWriter = new BinaryWriter(new GZipStream(new FileStream($"{timestamp}_public.bkey", FileMode.CreateNew), CompressionMode.Compress)))
                {
                    var pk = rsa.ExportParameters(false);

                    publicBKeyWriter.Write((short)(pk.Modulus.Length * 8));
                    publicBKeyWriter.Write(pk.Exponent);
                    publicBKeyWriter.Write(pk.Modulus);
                    publicBKeyWriter.Close();
                }

                /*using (var publicBKeyReader = new BinaryReader(new FileStream($"{timestamp}_public.bkey", FileMode.Open)))
                {
                    var bits = publicBKeyReader.ReadInt16();

                    var pk = new RSAParameters
                    {
                        Exponent = publicBKeyReader.ReadBytes(3),
                        Modulus = publicBKeyReader.ReadBytes(bits / 8)
                    };

                    publicBKeyReader.Close();

                    var newRsa = new RSACryptoServiceProvider();
                    newRsa.ImportParameters(pk);

                    using (var publicWriter = new StreamWriter(new FileStream($"{timestamp}_public.2key", FileMode.CreateNew)))
                    {
                        Pem.ExportPublicKey(newRsa, publicWriter);
                        publicWriter.Close();
                    }
                }*/
            }

            return new PrivateKeyPair();
        }
    }
}