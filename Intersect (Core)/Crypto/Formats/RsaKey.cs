using System;
using System.IO;
using System.Security.Cryptography;

using Intersect.Memory;

namespace Intersect.Crypto.Formats
{
    public class RsaKey : EncryptionKey
    {
        private RSAParameters mParameters;

        public RsaKey() : this(null)
        {
        }

        public RsaKey(RSAParameters? parameters) : base(KeyFormat.Rsa)
        {
            Parameters = parameters ?? new RSAParameters();
        }

        public RSAParameters Parameters
        {
            get => mParameters;
            set => mParameters = value;
        }

        public bool IsPublic => mParameters.D == null ||
                                mParameters.DP == null ||
                                mParameters.DQ == null ||
                                mParameters.InverseQ == null ||
                                mParameters.P == null ||
                                mParameters.Q == null;

        protected override bool InternalRead(IBuffer buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }

            if (!buffer.Read(out bool isPublic))
            {
                throw new EndOfStreamException();
            }

            if (!buffer.Read(out ushort bits))
            {
                throw new EndOfStreamException();
            }

            if (!buffer.Read(ref mParameters.Modulus, 0, bits >> 3))
            {
                throw new EndOfStreamException();
            }

            if (!buffer.Read(ref mParameters.Exponent, 0, 3))
            {
                throw new EndOfStreamException();
            }

            if (isPublic)
            {
                return true;
            }

            if (!buffer.Read(ref mParameters.D, 0, bits >> 3))
            {
                throw new EndOfStreamException();
            }

            if (!buffer.Read(ref mParameters.DP, 0, bits >> 4))
            {
                throw new EndOfStreamException();
            }

            if (!buffer.Read(ref mParameters.DQ, 0, bits >> 4))
            {
                throw new EndOfStreamException();
            }

            if (!buffer.Read(ref mParameters.InverseQ, 0, bits >> 4))
            {
                throw new EndOfStreamException();
            }

            if (!buffer.Read(ref mParameters.P, 0, bits >> 4))
            {
                throw new EndOfStreamException();
            }

            if (!buffer.Read(ref mParameters.Q, 0, bits >> 4))
            {
                throw new EndOfStreamException();
            }

            return true;
        }

        protected override bool InternalWrite(IBuffer buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }

            if (mParameters.Exponent == null || mParameters.Exponent.Length != 3)
            {
                throw new ArgumentNullException(
                    nameof(mParameters.Exponent), @"Exponent not initialized (or properly)."
                );
            }

            if (mParameters.Modulus == null || mParameters.Modulus.Length % 2 != 0)
            {
                throw new ArgumentNullException(
                    nameof(mParameters.Exponent), @"Modulus not initialized (or properly)."
                );
            }

            buffer.Write(IsPublic);

            var bits = (ushort) (mParameters.Modulus.Length << 3);
            buffer.Write(bits);
            buffer.Write(mParameters.Modulus, 0, bits >> 3);
            buffer.Write(mParameters.Exponent, 0, 3);

            if (IsPublic)
            {
                return true;
            }

            buffer.Write(mParameters.D, 0, bits >> 3);
            buffer.Write(mParameters.DP, 0, bits >> 4);
            buffer.Write(mParameters.DQ, 0, bits >> 4);
            buffer.Write(mParameters.InverseQ, 0, bits >> 4);
            buffer.Write(mParameters.P, 0, bits >> 4);
            buffer.Write(mParameters.Q, 0, bits >> 4);

            return true;
        }
    }
}
