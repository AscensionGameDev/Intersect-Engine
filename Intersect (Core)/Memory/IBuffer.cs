using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Intersect.Memory
{

    public interface IBuffer : IDisposable
    {

        long Length { get; }

        long Position { get; }

        long Remaining { get; }

        byte[] ToBytes();

        bool Has(long bytes);

        #region Read(out)

        bool Read(out bool value);

        bool Read(out byte value);

        bool Read(out byte[] value);

        bool Read(out byte[] value, long count);

        bool Read(ref byte[] value, long offset, long count);

        bool Read(out char value);

        bool Read(out decimal value);

        bool Read(out double value);

        bool Read(out float value);

        bool Read(out int value);

        bool Read(out long value);

        bool Read(out sbyte value);

        bool Read(out short value);

        bool Read([NotNullWhen(true)] out string? value);

        bool Read([NotNullWhen(true)] out string? value, Encoding encoding, bool nullTerminated = false);

        bool Read(out uint value);

        bool Read(out ulong value);

        bool Read(out ushort value);

        #endregion

        #region ReadX()

        bool ReadBool() => Read(out bool value) ? value : default;

        bool ReadBoolean() => Read(out bool value) ? value : default;

        byte ReadByte() => Read(out byte value) ? value : default;

        byte ReadUInt8() => Read(out byte value) ? value : default;

        byte[] ReadBytes() => Read(out byte[] value) ? value : default;

        byte[] ReadBytes(long count) => Read(out byte[] value, count) ? value : default;

        byte[] ReadBytes(ref byte[] bytes, long offset, long count) => Read(ref bytes, offset, count) ? bytes : default;

        char ReadChar() => Read(out char value) ? value : default;

        char ReadCharacter() => Read(out char value) ? value : default;

        decimal ReadDecimal() => Read(out decimal value) ? value : default;

        double ReadDouble() => Read(out double value) ? value : default;

        float ReadFloat() => Read(out float value) ? value : default;

        float ReadSingle() => Read(out float value) ? value : default;

        int ReadInt() => Read(out int value) ? value : default;

        int ReadInt32() => Read(out int value) ? value : default;

        int ReadInteger() => Read(out int value) ? value : default;

        long ReadInt64() => Read(out long value) ? value : default;

        long ReadLong() => Read(out long value) ? value : default;

        sbyte ReadInt8() => Read(out sbyte value) ? value : default;

        sbyte ReadSByte() => Read(out sbyte value) ? value : default;

        short ReadInt16() => Read(out short value) ? value : default;

        short ReadShort() => Read(out short value) ? value : default;

        string? ReadString() => Read(out string? value) ? value : default;

        string? ReadString(Encoding encoding, bool nullTerminated = false) => Read(out var value, encoding, nullTerminated) ? value : default;

        uint ReadUInt() => Read(out uint value) ? value : default;

        uint ReadUInt32() => Read(out uint value) ? value : default;

        uint ReadUnsignedInteger() => Read(out uint value) ? value : default;

        ulong ReadULong() => Read(out ulong value) ? value : default;

        ulong ReadUInt64() => Read(out ulong value) ? value : default;

        ushort ReadUInt16() => Read(out ushort value) ? value : default;

        ushort ReadUShort() => Read(out ushort value) ? value : default;

        #endregion

        #region Write()

        void Write(bool value);

        void Write(byte value);

        void Write(byte[] value);

        void Write(byte[] value, long count);

        void Write(byte[] value, long offset, long count);

        void Write(char value);

        void Write(decimal value);

        void Write(double value);

        void Write(float value);

        void Write(int value);

        void Write(long value);

        void Write(sbyte value);

        void Write(short value);

        void Write(string value);

        void Write(string value, Encoding encoding, bool nullTerminated = false);

        void Write(uint value);

        void Write(ulong value);

        void Write(ushort value);

        #endregion

    }

}
