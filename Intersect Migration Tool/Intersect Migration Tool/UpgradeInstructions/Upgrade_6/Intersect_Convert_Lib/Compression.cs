using System.IO;
using System.IO.Compression;
using Intersect_Library;

namespace Intersect_Migration_Tool.UpgradeInstructions.Upgrade_6.Intersect_Convert_Lib
{
    public static class Compression
    {
        public static byte[] DecompressPacket(byte[] data)
        {
            var buff = new ByteBuffer();
            buff.WriteBytes(data);
            var len = buff.ReadInteger();
            var decompessed = new byte[len];
            var compressed = buff.ReadBytes(buff.Length());
            using (MemoryStream ms = new MemoryStream(compressed))
            {
                using (DeflateStream decompressionStream = new DeflateStream(ms, CompressionMode.Decompress))
                {

                    decompressionStream.Read(decompessed, 0, decompessed.Length);
                }
                return decompessed;
            }
        }

        public static byte[] CompressPacket(byte[] data)
        {
            var buff = new ByteBuffer();
            buff.WriteInteger(data.Length);
            var ms = new MemoryStream();
            using (DeflateStream compressionStream = new DeflateStream(ms, CompressionMode.Compress))
            {
                compressionStream.Write(data, 0, data.Length);
            }
            buff.WriteBytes(ms.ToArray());
            return buff.ToArray();
        }
    }
}
