using System;
using System.IO;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.IO
{

    public class ConsoleWriter : TextWriter
    {

        public ConsoleWriter(ConsoleContext context, TextWriter textWriter)
        {
            Context = context;
            TextWriter = textWriter;
        }

        protected ConsoleContext Context { get; }

        internal TextWriter TextWriter { get; }

        internal bool SkipNextWriteChar { get; set; }

        public override Encoding Encoding => TextWriter.Encoding;

        public override IFormatProvider FormatProvider => TextWriter.FormatProvider;

        public override string NewLine
        {
            get => TextWriter.NewLine;
            set => TextWriter.NewLine = value;
        }

        public override void WriteLine()
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine();
            Context.WritePrefix(TextWriter.Write);
        }

        #region Passthrough

        public override string ToString()
        {
            return TextWriter.ToString();
        }

        public override bool Equals(object obj)
        {
            return TextWriter.Equals(obj);
        }

        public override int GetHashCode()
        {
            return TextWriter.GetHashCode();
        }

        public override object InitializeLifetimeService()
        {
            return TextWriter.InitializeLifetimeService();
        }

        public override ObjRef CreateObjRef(Type requestedType)
        {
            return TextWriter.CreateObjRef(requestedType);
        }

        public override void Close()
        {
            TextWriter.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                TextWriter.Dispose();
            }
        }

        #region Flush

        public override void Flush()
        {
            TextWriter.Flush();
        }

        public override Task FlushAsync()
        {
            return TextWriter.FlushAsync();
        }

        #endregion

        #endregion

        #region Write Buffer/Format

        public override void Write(char[] buffer)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(buffer);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(buffer, index, count);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void Write(string format, object arg0)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(format, arg0);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void Write(string format, object arg0, object arg1)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(format, arg0, arg1);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(format, arg0, arg1, arg2);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void Write(string format, params object[] arg)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(format, arg);
            Context.WritePrefix(TextWriter.Write);
        }

        #endregion

        #region Write(value)

        public override void Write(char value)
        {
            var skip = SkipNextWriteChar;
            if (skip)
            {
                SkipNextWriteChar = false;
            }
            else
            {
                Context.ResetWaitCursor(TextWriter.Write);
                TextWriter.Write(value);
                Context.WritePrefix(TextWriter.Write);
            }
        }

        public override void Write(bool value)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(value);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void Write(int value)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(value);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void Write(uint value)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(value);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void Write(long value)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(value);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void Write(ulong value)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(value);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void Write(float value)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(value);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void Write(double value)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(value);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void Write(decimal value)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(value);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void Write(string value)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(value);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void Write(object value)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(value);
            Context.WritePrefix(TextWriter.Write);
        }

        #endregion

        #region WriteAsync

        public override async Task WriteAsync(char value)
        {
            await Context.ResetWaitCursorAsync(TextWriter.WriteAsync);
            var task = TextWriter.WriteAsync(value);
            if (task != null)
            {
                await task;
            }

            await Context.WritePrefixAsync(TextWriter.WriteAsync);
        }

        public override async Task WriteAsync(string value)
        {
            await Context.ResetWaitCursorAsync(TextWriter.WriteAsync);
            var task = TextWriter.WriteAsync(value);
            if (task != null)
            {
                await task;
            }

            await Context.WritePrefixAsync(TextWriter.WriteAsync);
        }

        public override async Task WriteAsync(char[] buffer, int index, int count)
        {
            await Context.ResetWaitCursorAsync(TextWriter.WriteAsync);
            var task = TextWriter.WriteAsync(buffer, index, count);
            if (task != null)
            {
                await task;
            }

            await Context.WritePrefixAsync(TextWriter.WriteAsync);
        }

        #endregion

        #region WriteLine Buffer/Format

        public override void WriteLine(char[] buffer)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(buffer);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(buffer, index, count);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(string format, object arg0)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(format, arg0);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(format, arg0, arg1);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(format, arg0, arg1, arg2);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(string format, params object[] arg)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(format, arg);
            Context.WritePrefix(TextWriter.Write);
        }

        #endregion

        #region WriteLine(value)

        public override void WriteLine(char value)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(value);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(bool value)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(value);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(int value)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(value);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(uint value)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(value);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(long value)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(value);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(ulong value)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(value);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(float value)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(value);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(double value)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(value);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(decimal value)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(value);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(string value)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(value);
            Context.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(object value)
        {
            Context.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(value);
            Context.WritePrefix(TextWriter.Write);
        }

        #endregion

        #region WriteLineAsync

        public override async Task WriteLineAsync(char value)
        {
            await Context.ResetWaitCursorAsync(TextWriter.WriteAsync);
            var task = TextWriter.WriteLineAsync(value);
            if (task != null)
            {
                await task;
            }

            await Context.WritePrefixAsync(TextWriter.WriteAsync);
        }

        public override async Task WriteLineAsync(string value)
        {
            await Context.ResetWaitCursorAsync(TextWriter.WriteAsync);
            var task = TextWriter.WriteLineAsync(value);
            if (task != null)
            {
                await task;
            }

            await Context.WritePrefixAsync(TextWriter.WriteAsync);
        }

        public override async Task WriteLineAsync(char[] buffer, int index, int count)
        {
            await Context.ResetWaitCursorAsync(TextWriter.WriteAsync);
            var task = TextWriter.WriteLineAsync(buffer, index, count);
            if (task != null)
            {
                await task;
            }

            await Context.WritePrefixAsync(TextWriter.WriteAsync);
        }

        public override async Task WriteLineAsync()
        {
            await Context.ResetWaitCursorAsync(TextWriter.WriteAsync);
            var task = TextWriter.WriteLineAsync();
            if (task != null)
            {
                await task;
            }

            await Context.WritePrefixAsync(TextWriter.WriteAsync);
        }

        #endregion

    }

}
