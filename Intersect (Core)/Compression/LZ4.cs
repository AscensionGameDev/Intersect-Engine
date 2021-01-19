using K4os.Compression.LZ4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Compression
{
    public static class LZ4
    {
        public static byte[] Pickle(byte[] data)
        {
            return LZ4Pickler.Pickle(data, LZ4Level.L12_MAX);
        }

        public static byte[] UnPickle(byte[] data)
        {
            return LZ4Pickler.Unpickle(data);
        }

        public static byte[] PickleString(string data)
        {
            return LZ4Pickler.Pickle(Encoding.Default.GetBytes(data), LZ4Level.L00_FAST);
        }

        public static string UnPickleString(byte[] data)
        {
            return Encoding.Default.GetString(LZ4Pickler.Unpickle(data));
        }
    }
}
