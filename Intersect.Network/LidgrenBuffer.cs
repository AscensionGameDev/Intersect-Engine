using System;
using System.Text;
using Intersect.Memory;
using Lidgren.Network;

namespace Intersect.Network
{
    public class LidgrenBuffer : IBuffer
    {
        public NetBuffer Buffer { get; }

        public LidgrenBuffer(NetBuffer buffer)
        {
            Buffer = buffer;
        }

        public int Length
            => Buffer?.LengthBytes ?? -1;

        public int Position
            => Buffer?.PositionInBytes ?? -1;

        public int Remaining
            => Length - Position;

        public byte[] ToBytes()
            => Buffer?.Data;

        public bool Has(int bytes)
            => bytes <= Remaining;

        public bool Read(out bool value)
        {
            try
            {
                // ReSharper disable once PossibleNullReferenceException
                value = Buffer.ReadBoolean();
                return true;
            }
            catch (Exception)
            {
                value = default(bool);
                return false;
            }
        }

        public bool Read(out byte value)
        {
            try
            {
                // ReSharper disable once PossibleNullReferenceException
                value = Buffer.ReadByte();
                return true;
            }
            catch (Exception)
            {
                value = default(byte);
                return false;
            }
        }

        public bool Read(out byte[] value)
        {
            if (Read(out int count)) return Read(out value, count);
            value = default(byte[]);
            return false;
        }

        public bool Read(out byte[] value, int count)
        {
            value = new byte[count];
            return count < 1 || Read(ref value, 0, count);
        }

        public bool Read(ref byte[] value, int offset, int count)
        {
            try
            {
                // ReSharper disable once PossibleNullReferenceException
                Buffer.ReadBytes(value, offset, count);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Read(out char value)
        {
            if (Has(sizeof(char)) && Read(out byte[] bytes, sizeof(char)))
            {
                value = BitConverter.ToChar(bytes, 0);
                return true;
            }

            value = default(char);
            return false;
        }

        public bool Read(out decimal value)
        {
            value = default(decimal);

            var bits = new int[4];
            if (!Read(out bits[0])) return false;
            if (!Read(out bits[1])) return false;
            if (!Read(out bits[2])) return false;
            if (!Read(out bits[3])) return false;

            value = new decimal(bits);
            return true;
        }

        public bool Read(out double value)
        {
            try
            {
                // ReSharper disable once PossibleNullReferenceException
                value = Buffer.ReadDouble();
                return true;
            }
            catch (Exception)
            {
                value = default(double);
                return false;
            }
        }

        public bool Read(out float value)
        {
            try
            {
                // ReSharper disable once PossibleNullReferenceException
                value = Buffer.ReadFloat();
                return true;
            }
            catch (Exception)
            {
                value = default(float);
                return false;
            }
        }

        public bool Read(out int value)
        {
            try
            {
                // ReSharper disable once PossibleNullReferenceException
                value = Buffer.ReadInt32();
                return true;
            }
            catch (Exception)
            {
                value = default(int);
                return false;
            }
        }

        public bool Read(out long value)
        {
            try
            {
                // ReSharper disable once PossibleNullReferenceException
                value = Buffer.ReadInt64();
                return true;
            }
            catch (Exception)
            {
                value = default(long);
                return false;
            }
        }

        public bool Read(out sbyte value)
        {
            try
            {
                // ReSharper disable once PossibleNullReferenceException
                value = Buffer.ReadSByte();
                return true;
            }
            catch (Exception)
            {
                value = default(sbyte);
                return false;
            }
        }

        public bool Read(out short value)
        {
            try
            {
                // ReSharper disable once PossibleNullReferenceException
                value = Buffer.ReadInt16();
                return true;
            }
            catch (Exception)
            {
                value = default(short);
                return false;
            }
        }

        public bool Read(out string value)
        {
            value = null;
            return Buffer?.ReadString(out value) ?? false;
        }

        public bool Read(out string value, Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException();

            if (Read(out int length) && Read(out byte[] bytes, length))
            {
                value = encoding.GetString(bytes, 0, length);
                return true;
            }

            value = default(string);
            return false;
        }

        public bool Read(out uint value)
        {
            try
            {
                // ReSharper disable once PossibleNullReferenceException
                value = Buffer.ReadUInt32();
                return true;
            }
            catch (Exception)
            {
                value = default(uint);
                return false;
            }
        }

        public bool Read(out ulong value)
        {
            try
            {
                // ReSharper disable once PossibleNullReferenceException
                value = Buffer.ReadUInt64();
                return true;
            }
            catch (Exception)
            {
                value = default(ulong);
                return false;
            }
        }

        public bool Read(out ushort value)
        {
            try
            {
                // ReSharper disable once PossibleNullReferenceException
                value = Buffer.ReadUInt16();
                return true;
            }
            catch (Exception)
            {
                value = default(ushort);
                return false;
            }
        }

        public bool ReadBool()
        {
            if (Read(out bool value)) return value;
            throw new OutOfMemoryException();
        }

        public bool ReadBoolean()
        {
            if (Read(out bool value)) return value;
            throw new OutOfMemoryException();
        }

        public byte ReadByte()
        {
            if (Read(out byte value)) return value;
            throw new OutOfMemoryException();
        }

        public byte ReadUInt8()
        {
            if (Read(out byte value)) return value;
            throw new OutOfMemoryException();
        }

        public byte[] ReadBytes()
        {
            if (Read(out byte[] value)) return value;
            throw new OutOfMemoryException();
        }

        public byte[] ReadBytes(int count)
        {
            if (Read(out byte[] value, count)) return value;
            throw new OutOfMemoryException();
        }

        public byte[] ReadBytes(ref byte[] bytes, int offset, int count)
        {
            if (Read(ref bytes, offset, count)) return bytes;
            throw new OutOfMemoryException();
        }

        public char ReadChar()
        {
            if (Read(out char value)) return value;
            throw new OutOfMemoryException();
        }

        public char ReadCharacter()
        {
            if (Read(out char value)) return value;
            throw new OutOfMemoryException();
        }

        public decimal ReadDecimal()
        {
            if (Read(out decimal value)) return value;
            throw new OutOfMemoryException();
        }

        public double ReadDouble()
        {
            if (Read(out double value)) return value;
            throw new OutOfMemoryException();
        }

        public float ReadFloat()
        {
            if (Read(out float value)) return value;
            throw new OutOfMemoryException();
        }

        public float ReadSingle()
        {
            if (Read(out float value)) return value;
            throw new OutOfMemoryException();
        }

        public int ReadInt()
        {
            if (Read(out int value)) return value;
            throw new OutOfMemoryException();
        }

        public int ReadInt32()
        {
            if (Read(out int value)) return value;
            throw new OutOfMemoryException();
        }

        public int ReadInteger()
        {
            if (Read(out int value)) return value;
            throw new OutOfMemoryException();
        }

        public long ReadInt64()
        {
            if (Read(out long value)) return value;
            throw new OutOfMemoryException();
        }

        public long ReadLong()
        {
            if (Read(out long value)) return value;
            throw new OutOfMemoryException();
        }

        public sbyte ReadInt8()
        {
            if (Read(out sbyte value)) return value;
            throw new OutOfMemoryException();
        }

        public sbyte ReadSByte()
        {
            if (Read(out sbyte value)) return value;
            throw new OutOfMemoryException();
        }

        public short ReadInt16()
        {
            if (Read(out short value)) return value;
            throw new OutOfMemoryException();
        }

        public short ReadShort()
        {
            if (Read(out short value)) return value;
            throw new OutOfMemoryException();
        }

        public string ReadString()
        {
            if (Read(out string value)) return value;
            throw new OutOfMemoryException();
        }

        public string ReadString(Encoding encoding)
        {
            if (Read(out string value, encoding)) return value;
            throw new OutOfMemoryException();
        }

        public uint ReadUInt()
        {
            if (Read(out uint value)) return value;
            throw new OutOfMemoryException();
        }

        public uint ReadUInt32()
        {
            if (Read(out uint value)) return value;
            throw new OutOfMemoryException();
        }

        public uint ReadUnsignedInteger()
        {
            if (Read(out uint value)) return value;
            throw new OutOfMemoryException();
        }

        public ulong ReadULong()
        {
            if (Read(out ulong value)) return value;
            throw new OutOfMemoryException();
        }

        public ulong ReadUInt64()
        {
            if (Read(out ulong value)) return value;
            throw new OutOfMemoryException();
        }

        public ushort ReadUInt16()
        {
            if (Read(out ushort value)) return value;
            throw new OutOfMemoryException();
        }

        public ushort ReadUShort()
        {
            if (Read(out ushort value)) return value;
            throw new OutOfMemoryException();
        }

        public void Write(bool value)
            => Buffer?.Write(value);

        public void Write(byte value)
            => Buffer?.Write(value);

        public void Write(byte[] value)
            => Buffer?.Write(value);

        public void Write(byte[] value, int count)
            => Write(value, 0, count);

        public void Write(byte[] value, int offset, int count)
            => Buffer?.Write(value, offset, count);

        public void Write(char value)
            => Buffer?.Write(BitConverter.GetBytes(value));

        public void Write(decimal value)
        {
            var bits = decimal.GetBits(value);
            Write(bits[0]);
            Write(bits[1]);
            Write(bits[2]);
            Write(bits[3]);
        }

        public void Write(double value)
            => Buffer?.Write(value);

        public void Write(float value)
            => Buffer?.Write(value);

        public void Write(int value)
            => Buffer?.Write(value);

        public void Write(long value)
            => Buffer?.Write(value);

        public void Write(sbyte value)
            => Buffer?.Write(value);

        public void Write(short value)
            => Buffer?.Write(value);

        public void Write(string value)
            => Buffer?.Write(value);

        public void Write(string value, Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException();
            Write(value?.Length ?? 0);
            if (!(value?.Length > 0)) return;
            var bytes = encoding.GetBytes(value);
            Write(bytes, bytes.Length);
        }

        public void Write(uint value)
            => Buffer?.Write(value);

        public void Write(ulong value)
            => Buffer?.Write(value);

        public void Write(ushort value)
            => Buffer?.Write(value);
    }
}