using System.Text;

namespace Intersect.Memory
{
    public interface IBuffer
    {
        int Length { get; }
        int Position { get; }
        int Remaining { get; }

        byte[] ToBytes();

        bool Has(int bytes);

        #region Read(out)

        bool Read(out bool value);
        bool Read(out byte value);
        bool Read(out byte[] value);
        bool Read(out byte[] value, int count);
        bool Read(ref byte[] value, int offset, int count);
        bool Read(out char value);
        bool Read(out decimal value);
        bool Read(out double value);
        bool Read(out float value);
        bool Read(out int value);
        bool Read(out long value);
        bool Read(out sbyte value);
        bool Read(out short value);
        bool Read(out string value);
        bool Read(out string value, Encoding encoding);
        bool Read(out uint value);
        bool Read(out ulong value);
        bool Read(out ushort value);
        
        #endregion

        #region ReadX()

        bool ReadBool();
        bool ReadBoolean();
        byte ReadByte();
        byte ReadUInt8();
        byte[] ReadBytes();
        byte[] ReadBytes(int count);
        byte[] ReadBytes(ref byte[] bytes, int offset, int count);
        char ReadChar();
        char ReadCharacter();
        decimal ReadDecimal();
        double ReadDouble();
        float ReadFloat();
        float ReadSingle();
        int ReadInt();
        int ReadInt32();
        int ReadInteger();
        long ReadInt64();
        long ReadLong();
        sbyte ReadInt8();
        sbyte ReadSByte();
        short ReadInt16();
        short ReadShort();
        string ReadString();
        string ReadString(Encoding encoding);
        uint ReadUInt();
        uint ReadUInt32();
        uint ReadUnsignedInteger();
        ulong ReadULong();
        ulong ReadUInt64();
        ushort ReadUInt16();
        ushort ReadUShort();

        #endregion

        #region Write()
        
        void Write(bool value);
        void Write(byte value);
        void Write(byte[] value);
        void Write(byte[] value, int count);
        void Write(byte[] value, int offset, int count);
        void Write(char value);
        void Write(decimal value);
        void Write(double value);
        void Write(float value);
        void Write(int value);
        void Write(long value);
        void Write(sbyte value);
        void Write(short value);
        void Write(string value);
        void Write(string value, Encoding encoding);
        void Write(uint value);
        void Write(ulong value);
        void Write(ushort value);

        #endregion
    }
}