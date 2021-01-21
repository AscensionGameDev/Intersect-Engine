using System;
using System.Security.Cryptography;
using System.Text;

namespace Intersect.Utilities
{

    public static class GuidUtils
    {

        /// <inheritdoc cref="CreateNamed(Guid, byte[])" />
        public static Guid CreateNamed(Guid namespaceId, string name)
        {
            return CreateNamed(namespaceId, Encoding.UTF8.GetBytes(name));
        }

        /// <summary>
        /// Version 5 UUID implementation of RFC 4122 §4.3.
        /// </summary>
        /// <param name="namespaceId">the UUID of the namespace</param>
        /// <param name="name">the name to generate the UUID for</param>
        /// <returns>a Version 5 UUID generated from the provided namespace UUID and text name</returns>
        public static Guid CreateNamed(Guid namespaceId, byte[] name)
        {
            if (name.Length < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(name));
            }

            var namespaceIdData = namespaceId.ToByteArray();

            ValueUtils.Swap(ref namespaceIdData[0], ref namespaceIdData[3]);
            ValueUtils.Swap(ref namespaceIdData[2], ref namespaceIdData[1]);
            ValueUtils.Swap(ref namespaceIdData[4], ref namespaceIdData[5]);
            ValueUtils.Swap(ref namespaceIdData[6], ref namespaceIdData[7]);

            var data = new byte[namespaceIdData.Length + name.Length];
            Buffer.BlockCopy(namespaceIdData, 0, data, 0, namespaceIdData.Length);
            Buffer.BlockCopy(name, 0, data, namespaceIdData.Length, name.Length);

            byte[] hash;
            using (var algorithm = SHA1.Create())
            {
                hash = algorithm?.ComputeHash(data) ?? throw new InvalidOperationException();
            }

            var namedIdData = new byte[16];
            Buffer.BlockCopy(hash, 0, namedIdData, 0, 16);

            namedIdData[6] &= 0x0F;
            namedIdData[6] |= 5 << 4;
            namedIdData[8] &= 0x3F;
            namedIdData[8] |= 0x80;

            ValueUtils.Swap(ref namedIdData[0], ref namedIdData[3]);
            ValueUtils.Swap(ref namedIdData[2], ref namedIdData[1]);
            ValueUtils.Swap(ref namedIdData[4], ref namedIdData[5]);
            ValueUtils.Swap(ref namedIdData[6], ref namedIdData[7]);

            return new Guid(namedIdData);
        }

    }

}
