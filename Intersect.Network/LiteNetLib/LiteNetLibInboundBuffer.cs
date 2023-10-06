using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using Intersect.Memory;
using LiteNetLib.Utils;

namespace Intersect.Network.LiteNetLib;

public sealed class LiteNetLibInboundBuffer : IBuffer
{
    private readonly NetDataReader _reader;

    public LiteNetLibInboundBuffer(NetDataReader reader)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public void Dispose() {}

    public long Length => _reader.RawDataSize;

    public long Position => _reader.Position;

    public long Remaining => _reader.AvailableBytes;

    public byte[] ToBytes() => _reader.RawData;

    public bool Has(long bytes) => Remaining >= bytes;

    public bool Read(out bool value) => _reader.TryGetBool(out value);

    public bool Read(out byte value) => _reader.TryGetByte(out value);

    public bool Read(out byte[] value) => _reader.TryGetBytesWithLength(out value);

    public bool Read([NotNullWhen(true)] out byte[]? value, long count)
    {
        if (!Has(count))
        {
            value = default;
            return false;
        }

        value = new byte[count];
        _reader.GetBytes(value, value.Length);
        return true;
    }

    public bool Read(ref byte[] value, long offset, long count)
    {
        if (!Has(count))
        {
            return false;
        }

        _reader.GetBytes(value, (int)offset, (int)count);
        return true;
    }

    public bool Read(out char value) => _reader.TryGetChar(out value);

    public bool Read(out decimal value)
    {
        if (Remaining < 16)
        {
            value = default;
            return false;
        }

        var byteBuffer = _reader.GetBytesSegment(16);
        var bits = MemoryMarshal.Cast<byte, int>(byteBuffer);
        value = new decimal(bits);
        return true;
    }

    public bool Read(out double value) => _reader.TryGetDouble(out value);

    public bool Read(out float value) => _reader.TryGetFloat(out value);

    public bool Read(out int value) => _reader.TryGetInt(out value);

    public bool Read(out long value) => _reader.TryGetLong(out value);

    public bool Read(out sbyte value) => _reader.TryGetSByte(out value);

    public bool Read(out short value) => _reader.TryGetShort(out value);

    public bool Read([NotNullWhen(true)] out string? value) => _reader.TryGetString(out value);

    public bool Read([NotNullWhen(true)] out string? value, Encoding encoding, bool nullTerminated = false)
    {
        if (encoding == null)
        {
            throw new ArgumentNullException(nameof(encoding));
        }

        int length = 0;
        if (nullTerminated)
        {
            var offset = Position;
            var buffer = _reader.RawData ?? Array.Empty<byte>();
            var rawLength = buffer.Length;
            while (rawLength > offset)
            {
                if (buffer[offset] == 0)
                {
                    break;
                }

                ++offset;

                if (rawLength == offset)
                {
                    // Did not find null terminator
                    value = default;
                    return false;
                }

                ++length;
            }
        }
        else if (!Read(out length))
        {
            value = default;

            return false;
        }

        value = length switch
        {
            0 => string.Empty,
            < 0 => default!,
            _ => Read(out var bytes, length) ? encoding.GetString(bytes, 0, length) : default!
        };

        return true;
    }

    public bool Read(out uint value) => _reader.TryGetUInt(out value);

    public bool Read(out ulong value) => _reader.TryGetULong(out value);

    public bool Read(out ushort value) => _reader.TryGetUShort(out value);

    public void Write(bool value)
    {
        throw new NotSupportedException();
    }

    public void Write(byte value)
    {
        throw new NotSupportedException();
    }

    public void Write(byte[] value)
    {
        throw new NotSupportedException();
    }

    public void Write(byte[] value, long count)
    {
        throw new NotSupportedException();
    }

    public void Write(byte[] value, long offset, long count)
    {
        throw new NotSupportedException();
    }

    public void Write(char value)
    {
        throw new NotSupportedException();
    }

    public void Write(decimal value)
    {
        throw new NotSupportedException();
    }

    public void Write(double value)
    {
        throw new NotSupportedException();
    }

    public void Write(float value)
    {
        throw new NotSupportedException();
    }

    public void Write(int value)
    {
        throw new NotSupportedException();
    }

    public void Write(long value)
    {
        throw new NotSupportedException();
    }

    public void Write(sbyte value)
    {
        throw new NotSupportedException();
    }

    public void Write(short value)
    {
        throw new NotSupportedException();
    }

    public void Write(string value)
    {
        throw new NotSupportedException();
    }

    public void Write(string value, Encoding encoding, bool nullTerminated = false)
    {
        throw new NotSupportedException();
    }

    public void Write(uint value)
    {
        throw new NotSupportedException();
    }

    public void Write(ulong value)
    {
        throw new NotSupportedException();
    }

    public void Write(ushort value)
    {
        throw new NotSupportedException();
    }
}