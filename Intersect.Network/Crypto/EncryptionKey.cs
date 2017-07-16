using Intersect.Memory;
using Intersect.Network.Crypto.Formats;
using System;
using System.IO;
using System.IO.Compression;

namespace Intersect.Network.Crypto
{
    public abstract class EncryptionKey
    {
        public KeyFormat Format { get; }
        public bool Compressed { get; set; }

        protected EncryptionKey(KeyFormat format)
        {
            Format = format;
        }

        protected abstract bool InternalRead(IBuffer buffer);

        protected abstract bool InternalWrite(IBuffer buffer);

        public bool Read(Stream stream)
        {
            if (Compressed)
                stream = new GZipStream(stream, CompressionMode.Decompress);

            using (stream)
            {
                using (var wrapper = new StreamWrapper(stream))
                {
                    return InternalRead(wrapper);
                }
            }
        }

        public bool Write(Stream stream)
        {
            var buffer = new StreamWrapper(stream);

            buffer.Write((byte)Format);
            buffer.Write(Compressed);

            if (Compressed)
                buffer = new StreamWrapper(new GZipStream(stream, CompressionLevel.Optimal));

            return InternalWrite(buffer);
        }

        public static bool ToStream(EncryptionKey encryptionKey, Stream stream)
        {
            if (encryptionKey == null) throw new ArgumentNullException();

            return encryptionKey.Write(stream);
        }

        public static EncryptionKey FromStream(Stream stream)
        {
            EncryptionKey encryptionKey;

            using (var wrapper = new StreamWrapper(stream))
            {
                if (!wrapper.Read(out byte format)) throw new EndOfStreamException();
                if (!wrapper.Read(out bool compressed)) throw new EndOfStreamException();

                switch ((KeyFormat)format)
                {
                    case KeyFormat.Aes:
                        encryptionKey = new AesKey();
                        break;

                    case KeyFormat.Rsa:
                        encryptionKey = new RsaKey();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                encryptionKey.Compressed = compressed;

                if (!encryptionKey.Read(stream))
                    throw new Exception();
            }

            return encryptionKey;
        }

        public static TKey FromStream<TKey>(Stream stream) where TKey : EncryptionKey
        {
            return FromStream(stream) as TKey;
        }
    }
}
