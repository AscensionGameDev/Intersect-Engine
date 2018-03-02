using System;
using System.Collections.Generic;
using System.Text;

namespace Intersect
{
    public class ByteBuffer : IDisposable
    {
        readonly List<byte> mBuff;

        // To detect redundant calls
        private bool mDisposedValue;

        private byte[] mReadBytes;
        private bool mWasUpdated;
        public int Readpos;

        public ByteBuffer()
        {
            mBuff = new List<byte>();
            Readpos = 0;
        }

        #region " IDisposable Support "

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public byte[] ToArray()
        {
            return mBuff.ToArray();
        }

        public int Count()
        {
            return mBuff.Count;
        }

        public int Pos()
        {
            return Readpos;
        }

        public int Length()
        {
            return Count() - Readpos;
        }

        public void Clear()
        {
            mBuff.Clear();
            Readpos = 0;
        }

        public void WriteBytes(byte[] input)
        {
            mBuff.AddRange(input);
            mWasUpdated = true;
        }

        public void WriteByte(byte input)
        {
            mBuff.Add(input);
            mWasUpdated = true;
        }

        public void WriteBoolean(bool input)
        {
            WriteByte(Convert.ToByte(input));
        }

        public void WriteShort(short input)
        {
            mBuff.AddRange(BitConverter.GetBytes(input));
            mWasUpdated = true;
        }

        public void InsertLength()
        {
            var lenbytes = BitConverter.GetBytes((int) Count());
            for (int i = 0; i < lenbytes.Length; i++)
            {
                mBuff.Insert(i, lenbytes[i]);
            }
        }

        public void WriteInteger(int input)
        {
            mBuff.AddRange(BitConverter.GetBytes(input));
            mWasUpdated = true;
        }

        public void WriteLong(long input)
        {
            mBuff.AddRange(BitConverter.GetBytes(input));
            mWasUpdated = true;
        }

        public void WriteDouble(double input)
        {
            mBuff.AddRange(BitConverter.GetBytes(input));
            mWasUpdated = true;
        }

        public void WriteGuid(Guid guid)
        {
            WriteBytes(guid.ToByteArray());
            mWasUpdated = true;
        }

        public void WriteString(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                WriteInteger(0);
            }
            else
            {
                var data = Encoding.Unicode.GetBytes(input);
                WriteInteger(data.Length);
                WriteBytes(data);
            }
            mWasUpdated = true;
        }

        public string ReadString(bool peek = true)
        {
            var len = ReadInteger(true);
            if (mWasUpdated)
            {
                mReadBytes = mBuff.ToArray();
                mWasUpdated = false;
            }
            if (len == 0) return "";
            var ret = Encoding.Unicode.GetString(mReadBytes, Readpos, len);
            if (peek & mBuff.Count > Readpos)
            {
                if (ret.Length > 0)
                {
                    Readpos += len;
                }
            }
            return ret;
        }

        public bool ReadBoolean(bool peek = true)
        {
            return Convert.ToBoolean(ReadByte(peek));
        }

        public byte ReadByte(bool peek = true)
        {
            //check to see if this passes the byte count
            if (mBuff.Count <= Readpos) throw new Exception("Byte Buffer Past Limit!");
            var ret = mBuff[Readpos];
            if (peek & mBuff.Count > Readpos)
            {
                Readpos += 1;
            }
            return ret;
        }

        public byte[] ReadBytes(int length, bool peek = true)
        {
            var ret = mBuff.GetRange(Readpos, length).ToArray();
            if (peek)
                Readpos += length;
            return ret;
        }

        public short ReadShort(bool peek = true)
        {
            //check to see if this passes the byte count
            if (mBuff.Count <= Readpos) throw new Exception("Byte Buffer Past Limit!");
            if (mWasUpdated)
            {
                mReadBytes = mBuff.ToArray();
                mWasUpdated = false;
            }
            var ret = BitConverter.ToInt16(mReadBytes, Readpos);
            if (peek & mBuff.Count > Readpos)
            {
                Readpos += 2;
            }
            return ret;
        }

        public int ReadInteger(bool peek = true)
        {
            //check to see if this passes the byte count
            if (mBuff.Count <= Readpos) throw new Exception("Byte Buffer Past Limit!");
            if (mWasUpdated)
            {
                mReadBytes = mBuff.ToArray();
                mWasUpdated = false;
            }
            var ret = BitConverter.ToInt32(mReadBytes, Readpos);
            if (peek & mBuff.Count > Readpos)
            {
                Readpos += 4;
            }
            return ret;
        }

        public long ReadLong(bool peek = true)
        {
            //check to see if this passes the byte count
            if (mBuff.Count <= Readpos) throw new Exception("Byte Buffer Past Limit!");
            if (mWasUpdated)
            {
                mReadBytes = mBuff.ToArray();
                mWasUpdated = false;
            }
            var ret = BitConverter.ToInt64(mReadBytes, Readpos);
            if (peek & mBuff.Count > Readpos)
            {
                Readpos += 8;
            }
            return ret;
        }

        public double ReadDouble(bool peek = true)
        {
            //check to see if this passes the byte count
            if (mBuff.Count <= Readpos) throw new Exception("Byte Buffer Past Limit!");
            if (mWasUpdated)
            {
                mReadBytes = mBuff.ToArray();
                mWasUpdated = false;
            }
            var ret = BitConverter.ToDouble(mReadBytes, Readpos);
            if (peek & mBuff.Count > Readpos)
            {
                Readpos += 8;
            }
            return ret;
        }

        public Guid ReadGuid(bool peek = true)
        {
            // check to see if this passes the byte count
            if (mBuff.Count <= Readpos) throw new Exception("Byte Buffer Past Limit!");
            if (mWasUpdated)
            {
                mReadBytes = mBuff.ToArray();
                mWasUpdated = false;
            }
            return new Guid(ReadBytes(16,peek));
        }

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!mDisposedValue)
            {
                if (disposing)
                {
                    mBuff.Clear();
                }
                Readpos = 0;
            }
            mDisposedValue = true;
        }
    }
}