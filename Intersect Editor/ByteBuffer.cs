using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;

public class ByteBuffer
{

    List<byte> Buff;
    int readpos;
    public ByteBuffer()
    {
        Buff = new List<byte>();
        readpos = 0;
    }
    public byte[] ToArray()
    {
        return Buff.ToArray();
    }
    public int Count()
    {
        return Buff.Count;
    }
    public int Pos()
    {
        return readpos;
    }
    public int Length()
    {
        return Count() - readpos;
    }
    public void Clear()
    {
        Buff.Clear();
        readpos = 0;
    }
    public void WriteBytes(byte[] Input)
    {
        Buff.AddRange(Input);
    }
    public void WriteByte(byte Input)
    {
        Buff.Add(Input);
    }
    public void WriteShort(short Input)
    {
        Buff.AddRange(BitConverter.GetBytes(Input));
    }
    public void WriteInteger(int Input)
    {
        Buff.AddRange(BitConverter.GetBytes(Input));
    }
    public void WriteLong(long Input)
    {
        Buff.AddRange(BitConverter.GetBytes(Input));
    }
    public void WriteDouble(double Input)
    {
        Buff.AddRange(BitConverter.GetBytes(Input));
    }
    public void WriteString(string Input)
    {
        Buff.AddRange(BitConverter.GetBytes(Input.Length));
        Buff.AddRange(Encoding.ASCII.GetBytes(Input));
    }
    public string ReadString()
    {
        return ReadString(true);
    }
    public string ReadString(bool Peek)
    {
        int Len = ReadInteger(true);
        string ret = Encoding.ASCII.GetString(Buff.ToArray(), readpos, Len);
        if (Peek & Buff.Count > readpos)
        {
            if (ret.Length > 0)
            {
                readpos += Len;
            }
        }
        return ret;
    }
    public byte ReadByte()
    {
        return ReadByte(true);
    }
    public byte ReadByte(bool peek)
    {
        //check to see if this passes the byte count
        if (Buff.Count > readpos)
        {
            byte ret = Buff[readpos];
            if (peek & Buff.Count > readpos)
            {
                readpos += 1;
            }
            return ret;
        }
        else
        {
            throw new Exception("Byte Buffer Past Limit!");
            //past byte count throw a new exception
        }
    }
    public byte[] ReadBytes(int Length)
    {
        return ReadBytes(Length, true);
    }
    public byte[] ReadBytes(int Length, bool Peek)
    {
        byte[] ret = Buff.GetRange(readpos, Length).ToArray();
        if (Peek)
            readpos += Length;
        return ret;
    }
    public short ReadShort()
    {
        return ReadShort(true);
    }
    public short ReadShort(bool peek)
    {
        //check to see if this passes the byte count
        if (Buff.Count > readpos)
        {
            short ret = BitConverter.ToInt16(Buff.ToArray(), readpos);
            if (peek & Buff.Count > readpos)
            {
                readpos += 2;
            }
            return ret;
        }
        else
        {
            throw new Exception("Byte Buffer Past Limit!");
            //past byte count throw a new exception
        }
    }
    public int ReadInteger()
    {
        return ReadInteger(true);
    }
    public int ReadInteger(bool peek)
    {
        //check to see if this passes the byte count
        if (Buff.Count > readpos)
        {
            int ret = BitConverter.ToInt32(Buff.ToArray(), readpos);
            if (peek & Buff.Count > readpos)
            {
                readpos += 4;
            }
            return ret;
        }
        else
        {
            throw new Exception("Byte Buffer Past Limit!");
            //past byte count throw a new exception
        }
    }
    public long ReadLong()
    {
        return ReadLong(true);
    }
    public long ReadLong(bool peek)
    {
        //check to see if this passes the byte count
        if (Buff.Count > readpos)
        {
            long ret = BitConverter.ToInt64(Buff.ToArray(), readpos);
            if (peek & Buff.Count > readpos)
            {
                readpos += 8;
            }
            return ret;
        }
        else
        {
            throw new Exception("Byte Buffer Past Limit!");
            //past byte count throw a new exception
        }
    }
    public double ReadDouble()
    {
        return ReadDouble(true);
    }
    public double ReadDouble(bool peek)
    {
        //check to see if this passes the byte count
        if (Buff.Count > readpos)
        {
            double ret = BitConverter.ToDouble(Buff.ToArray(), readpos);
            if (peek & Buff.Count > readpos)
            {
                readpos += 8;
            }
            return ret;
        }
        else
        {
            throw new Exception("Byte Buffer Past Limit!");
            //past byte count throw a new exception
        }
    }
    // To detect redundant calls
    private bool disposedValue = false;

    // IDisposable
    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                Buff.Clear();
            }
            readpos = 0;
        }
        this.disposedValue = true;
    }


}
