using JetBrains.Annotations;
using System;
using System.IO;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.IO
{
    public class ConsoleWaitWriter : TextWriter
    {
        [NotNull]
        protected ConsoleWaitContext WaitContext { get; }

        [NotNull]
        internal TextWriter TextWriter { get; }

        public override Encoding Encoding => TextWriter.Encoding;

        public override IFormatProvider FormatProvider => TextWriter.FormatProvider;

        public override string NewLine {
            get => TextWriter.NewLine;
            set => TextWriter.NewLine = value;
        }

        public ConsoleWaitWriter(
            [NotNull] ConsoleWaitContext waitContext,
            [NotNull] TextWriter textWriter
        )
        {
            WaitContext = waitContext;
            TextWriter = textWriter;
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
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(buffer);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(buffer, index, count);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void Write(string format, object arg0)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(format, arg0);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void Write(string format, object arg0, object arg1)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(format, arg0, arg1);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(format, arg0, arg1, arg2);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void Write(string format, params object[] arg)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(format, arg);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        #endregion

        #region Write(value)

        public override void Write(char value)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(value);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void Write(bool value)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(value);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void Write(int value)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(value);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void Write(uint value)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(value);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void Write(long value)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(value);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void Write(ulong value)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(value);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void Write(float value)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(value);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void Write(double value)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(value);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void Write(decimal value)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(value);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void Write(string value)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(value);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void Write(object value)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.Write(value);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        #endregion

        #region WriteAsync

        public override async Task WriteAsync(char value)
        {
            await WaitContext.ResetWaitCursorAsync(TextWriter.WriteAsync);
            var task = TextWriter.WriteAsync(value);
            if (task != null)
            {
                await task;
            }
            await WaitContext.WritePrefixAsync(TextWriter.WriteAsync);
        }

        public override async Task WriteAsync(string value)
        {
            await WaitContext.ResetWaitCursorAsync(TextWriter.WriteAsync);
            var task = TextWriter.WriteAsync(value);
            if (task != null)
            {
                await task;
            }
            await WaitContext.WritePrefixAsync(TextWriter.WriteAsync);
        }

        public override async Task WriteAsync(char[] buffer, int index, int count)
        {
            await WaitContext.ResetWaitCursorAsync(TextWriter.WriteAsync);
            var task = TextWriter.WriteAsync(buffer, index, count);
            if (task != null)
            {
                await task;
            }
            await WaitContext.WritePrefixAsync(TextWriter.WriteAsync);
        }

        #endregion

        #region WriteLine Buffer/Format

        public override void WriteLine(char[] buffer)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(buffer);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(buffer, index, count);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(string format, object arg0)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(format, arg0);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(format, arg0, arg1);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(format, arg0, arg1, arg2);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(string format, params object[] arg)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(format, arg);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        #endregion

        public override void WriteLine()
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine();
            WaitContext.WritePrefix(TextWriter.Write);
        }

        #region WriteLine(value)

        public override void WriteLine(char value)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(value);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(bool value)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(value);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(int value)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(value);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(uint value)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(value);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(long value)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(value);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(ulong value)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(value);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(float value)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(value);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(double value)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(value);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(decimal value)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(value);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(string value)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(value);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        public override void WriteLine(object value)
        {
            WaitContext.ResetWaitCursor(TextWriter.Write);
            TextWriter.WriteLine(value);
            WaitContext.WritePrefix(TextWriter.Write);
        }

        #endregion

        #region WriteLineAsync

        public override async Task WriteLineAsync(char value)
        {
            await WaitContext.ResetWaitCursorAsync(TextWriter.WriteAsync);
            var task = TextWriter.WriteLineAsync(value);
            if (task != null)
            {
                await task;
            }
            await WaitContext.WritePrefixAsync(TextWriter.WriteAsync);
        }

        public override async Task WriteLineAsync(string value)
        {
            await WaitContext.ResetWaitCursorAsync(TextWriter.WriteAsync);
            var task = TextWriter.WriteLineAsync(value);
            if (task != null)
            {
                await task;
            }
            await WaitContext.WritePrefixAsync(TextWriter.WriteAsync);
        }

        public override async Task WriteLineAsync(char[] buffer, int index, int count)
        {
            await WaitContext.ResetWaitCursorAsync(TextWriter.WriteAsync);
            var task = TextWriter.WriteLineAsync(buffer, index, count);
            if (task != null)
            {
                await task;
            }
            await WaitContext.WritePrefixAsync(TextWriter.WriteAsync);
        }

        public override async Task WriteLineAsync()
        {
            await WaitContext.ResetWaitCursorAsync(TextWriter.WriteAsync);
            var task = TextWriter.WriteLineAsync();
            if (task != null)
            {
                await task;
            }
            await WaitContext.WritePrefixAsync(TextWriter.WriteAsync);
        }

        #endregion
    }
}