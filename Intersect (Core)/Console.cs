using System;
using System.IO;
using System.Text;

using Intersect.IO;

namespace Intersect
{

    using SystemConsole = System.Console;

    // TODO: Make this instanceable
    public static class Console
    {

        //#region Instance Management

        //private static Console sInstance;

        //public static Console Instance
        //{
        //    get => sInstance;
        //    set => sInstance = value ?? sInstance;
        //}

        private static readonly TextWriter mDefaultError;

        private static readonly TextWriter mDefaultOut;

        static Console()
        {
            //Instance = new Console();
            SystemConsole.CancelKeyPress += (sender, args) => CancelKeyPress?.Invoke(sender, args);

            mDefaultError = Error;
            mDefaultOut = Out;

            Context = new ConsoleContext();

#if CONSOLE_EXTENSIONS
            Error = new ConsoleWriter(Context, Error);
            ContextOut = new ConsoleWriter(Context, Out);
            Out = ContextOut;
#endif
        }

        //#endregion

        #region Instance

        #region Extensions

        public static bool InputHistoryEnabled
        {
            get => Context.InputHistoryEnabled;
            set => Context.InputHistoryEnabled = value;
        }

        public static int InputHistoryLength
        {
            get => Context.InputHistoryLength;
            set => Context.InputHistoryLength = value;
        }

        #endregion

        #region Color

        public static ConsoleColor BackgroundColor
        {
            get => SystemConsole.BackgroundColor;
            set => SystemConsole.BackgroundColor = value;
        }

        public static ConsoleColor ForegroundColor
        {
            get => SystemConsole.ForegroundColor;
            set => SystemConsole.ForegroundColor = value;
        }

        public static void ResetColor()
        {
            SystemConsole.ResetColor();
        }

        #endregion

        #region Buffer

        #region Buffer Size

        public static int BufferHeight
        {
            get => SystemConsole.BufferHeight;
            set => SystemConsole.BufferHeight = value;
        }

        public static int BufferWidth
        {
            get => SystemConsole.BufferWidth;
            set => SystemConsole.BufferWidth = value;
        }

        public static void SetBufferSize(int width, int height)
        {
            SystemConsole.SetBufferSize(width, height);
        }

        #endregion

        #endregion

        #region Window

        #region Window Position

        public static int WindowLeft
        {
            get => SystemConsole.WindowLeft;
            set => SystemConsole.WindowLeft = value;
        }

        public static int WindowTop
        {
            get => SystemConsole.WindowTop;
            set => SystemConsole.WindowTop = value;
        }

        public static void SetWindowPosition(int left, int top)
        {
            SystemConsole.SetWindowPosition(left, top);
        }

        #endregion

        #region Window Size

        public static int WindowHeight
        {
            get => SystemConsole.WindowHeight;
            set => SystemConsole.WindowHeight = value;
        }

        public static int WindowWidth
        {
            get => SystemConsole.WindowWidth;
            set => SystemConsole.WindowWidth = value;
        }

        public static void SetWindowSize(int width, int height)
        {
            SystemConsole.SetWindowSize(width, height);
        }

        public static int LargestWindowHeight => SystemConsole.LargestWindowHeight;

        public static int LargestWindowWidth => SystemConsole.LargestWindowWidth;

        #endregion

        #endregion

        #region Cursor

        #region Cursor Position

        public static int CursorLeft
        {
            get => SystemConsole.CursorLeft;
            set
            {
                var top = SystemConsole.CursorTop;
                var left = value;

                if (left < 0)
                {
                    --top;
                    left += BufferWidth;
                }
                else if (left > BufferWidth)
                {
                    ++top;
                    left -= BufferWidth;
                }

                SetCursorPosition(left, top);
            }
        }

        public static int CursorTop
        {
            get => SystemConsole.CursorTop;
            set => SystemConsole.CursorTop = value;
        }

        public static void SetCursorPosition(int left, int top)
        {
            var position = Math.Max(0, top * BufferWidth + left);
            var bufferWidth = BufferWidth == 0 ? 1 : BufferWidth;
            SystemConsole.SetCursorPosition(position % bufferWidth, position / bufferWidth);
        }

        #endregion

        #region Cursor Appearance

        public static int CursorSize
        {
            get => SystemConsole.CursorSize;
            set => SystemConsole.CursorSize = value;
        }

        public static bool CursorVisible
        {
            get => SystemConsole.CursorVisible;
            set => SystemConsole.CursorVisible = value;
        }

        #endregion

        #endregion

        #region General

        public static string Title
        {
            get => SystemConsole.Title;
            set => SystemConsole.Title = value;
        }

        #region Encoding

        /// <summary>Gets or sets the encoding the console uses to read input. </summary>
        /// <returns>The encoding used to read console input.</returns>
        /// <exception cref="T:System.ArgumentNullException">The property value in a set operation is <see langword="null" />.</exception>
        /// <exception cref="T:System.IO.IOException">An error occurred during the execution of this operation.</exception>
        /// <exception cref="T:System.Security.SecurityException">Your application does not have permission to perform this operation.</exception>
        public static Encoding InputEncoding
        {
            get => SystemConsole.InputEncoding;
            set => SystemConsole.InputEncoding = value;
        }

        /// <summary>Gets or sets the encoding the console uses to write output. </summary>
        /// <returns>The encoding used to write console output.</returns>
        /// <exception cref="T:System.ArgumentNullException">The property value in a set operation is <see langword="null" />.</exception>
        /// <exception cref="T:System.IO.IOException">An error occurred during the execution of this operation.</exception>
        /// <exception cref="T:System.Security.SecurityException">Your application does not have permission to perform this operation.</exception>
        public static Encoding OutputEncoding
        {
            get => SystemConsole.OutputEncoding;
            set => SystemConsole.OutputEncoding = value;
        }

        #endregion

        #region Beep

        /// <summary>Plays the sound of a beep through the console speaker.</summary>
        /// <exception cref="T:System.Security.HostProtectionException">This method was executed on a server, such as SQL Server, that does not permit access to a user interface.</exception>
        public static void Beep()
        {
            SystemConsole.Beep();
        }

        /// <summary>Plays the sound of a beep of a specified frequency and duration through the console speaker.</summary>
        /// <param name="frequency">The frequency of the beep, ranging from 37 to 32767 hertz.</param>
        /// <param name="duration">The duration of the beep measured in milliseconds.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="frequency" /> is less than 37 or more than 32767 hertz.-or-
        /// <paramref name="duration" /> is less than or equal to zero.</exception>
        /// <exception cref="T:System.Security.HostProtectionException">This method was executed on a server, such as SQL Server, that does not permit access to the console.</exception>
        public static void Beep(int frequency, int duration)
        {
            SystemConsole.Beep(frequency, duration);
        }

        #endregion

        #endregion

        #region Input

        #region Keys

        public static ConsoleKeyInfo ReadKey()
        {
            return SystemConsole.ReadKey();
        }

        private static ConsoleKeyInfo ReadKeySkipPrint()
        {
            ContextOut.SkipNextWriteChar = true;

            return ReadKey();
        }

        public static ConsoleKeyInfo ReadKey(bool intercept)
        {
            return SystemConsole.ReadKey(intercept);
        }

        /// <summary>Gets a value indicating whether a key press is available in the input stream.</summary>
        /// <returns>
        /// <see langword="true" /> if a key press is available; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        /// <exception cref="T:System.InvalidOperationException">Standard input is redirected to a file instead of the keyboard. </exception>
        public static bool KeyAvailable => SystemConsole.KeyAvailable;

        /// <summary>Gets a value indicating whether the NUM LOCK keyboard toggle is turned on or turned off.</summary>
        /// <returns>
        /// <see langword="true" /> if NUM LOCK is turned on; <see langword="false" /> if NUM LOCK is turned off.</returns>
        public static bool NumberLock => SystemConsole.NumberLock;

        /// <summary>Gets a value indicating whether the CAPS LOCK keyboard toggle is turned on or turned off.</summary>
        /// <returns>
        /// <see langword="true" /> if CAPS LOCK is turned on; <see langword="false" /> if CAPS LOCK is turned off.</returns>
        public static bool CapsLock => SystemConsole.CapsLock;

        /// <summary>Gets or sets a value indicating whether the combination of the <see cref="F:System.ConsoleModifiers.Control" /> modifier key and <see cref="F:System.ConsoleKey.C" /> console key (Ctrl+C) is treated as ordinary input or as an interruption that is handled by the operating system.</summary>
        /// <returns>
        /// <see langword="true" /> if Ctrl+C is treated as ordinary input; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.IO.IOException">Unable to get or set the input mode of the console input buffer. </exception>
        public static bool TreatControlCAsInput
        {
            get => SystemConsole.TreatControlCAsInput;
            set => SystemConsole.TreatControlCAsInput = value;
        }

        /// <summary>Occurs when the <see cref="F:System.ConsoleModifiers.Control" /> modifier key (Ctrl) and either the <see cref="F:System.ConsoleKey.C" /> console key (C) or the Break key are pressed simultaneously (Ctrl+C or Ctrl+Break).</summary>
        public static event ConsoleCancelEventHandler CancelKeyPress;

        #endregion

        #region Char/String

        /// <summary>Reads the next character from the standard input stream.</summary>
        /// <returns>The next character from the input stream, or negative one (-1) if there are currently no more characters to be read.</returns>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static int Read()
        {
            return SystemConsole.Read();
        }

        /// <summary>Reads the next line of characters from the standard input stream.</summary>
        /// <returns>The next line of characters from the input stream, or <see langword="null" /> if no more lines are available.</returns>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        /// <exception cref="T:System.OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The number of characters in the next line of characters is greater than <see cref="F:System.Int32.MaxValue" />.</exception>
        public static string ReadLine()
        {
#if CONSOLE_EXTENSIONS
            return ReadLine(true);
#else
            return SystemConsole.ReadLine();
#endif
        }

        #endregion

        #region Wait

        private static ConsoleContext Context { get; }

        private static ConsoleWriter ContextOut { get; }

        public static string WaitPrefix
        {
            get => Context.WaitPrefix;
            set => Context.WaitPrefix = value;
        }

        // TODO: Implement Wait ReadKey
        public static ConsoleKeyInfo ReadKeyWait()
        {
            Context.Check();

            var result = Context.Wait(ContextOut.TextWriter.Write, ReadKey);

            Context.Clear();

            return result;
        }

        // TODO: Implement Wait Read

        public static string ReadLine(bool withPrefix)
        {
            // TODO: Actually write a ReadLine that saves the input buffer so that when a Write inevitably happens the input isn't obliterated off the screen (while it still persists in memory).

            Context.Check();

            if (!withPrefix)
            {
                return ReadLine();
            }

            var result = Context.BufferedReadLine(
                ContextOut.TextWriter.Write, ContextOut.TextWriter.WriteLine, ReadKeySkipPrint
            );

            Context.Clear();

            return result;
        }

        #endregion

        #endregion

        #region Output

        public static void Clear()
        {
            SystemConsole.Clear();

            Context.WritePrefix(Out.Write);
        }

        #region WriteLine

        /// <summary>Writes the current line terminator to the standard output stream.</summary>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void WriteLine()
        {
            SystemConsole.WriteLine();
        }

        /// <summary>Writes the text representation of the specified array of objects, followed by the current line terminator, to the standard output stream using the specified format information.</summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg">An array of objects to write using <paramref name="format" />. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="format" /> or <paramref name="arg" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.FormatException">The format specification in <paramref name="format" /> is invalid. </exception>
        public static void WriteLine(string format, params object[] arg)
        {
            SystemConsole.WriteLine(format, arg);
        }

        /// <summary>Writes the specified array of Unicode characters, followed by the current line terminator, to the standard output stream.</summary>
        /// <param name="buffer">A Unicode character array. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void WriteLine(char[] buffer)
        {
            SystemConsole.WriteLine(buffer);
        }

        /// <summary>Writes the specified subarray of Unicode characters, followed by the current line terminator, to the standard output stream.</summary>
        /// <param name="buffer">An array of Unicode characters. </param>
        /// <param name="index">The starting position in <paramref name="buffer" />. </param>
        /// <param name="count">The number of characters to write. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="buffer" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> or <paramref name="count" /> is less than zero. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="index" /> plus <paramref name="count" /> specify a position that is not within <paramref name="buffer" />. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void WriteLine(char[] buffer, int index, int count)
        {
            SystemConsole.WriteLine(buffer, index, count);
        }

        /// <summary>Writes the text representation of the specified Boolean value, followed by the current line terminator, to the standard output stream.</summary>
        /// <param name="value">The value to write. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void WriteLine(bool value)
        {
            SystemConsole.WriteLine(value);
        }

        /// <summary>Writes the specified Unicode character, followed by the current line terminator, value to the standard output stream.</summary>
        /// <param name="value">The value to write. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void WriteLine(char value)
        {
            SystemConsole.WriteLine(value);
        }

        /// <summary>Writes the text representation of the specified <see cref="T:System.Decimal" /> value, followed by the current line terminator, to the standard output stream.</summary>
        /// <param name="value">The value to write. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void WriteLine(decimal value)
        {
            SystemConsole.WriteLine(value);
        }

        /// <summary>Writes the text representation of the specified double-precision floating-point value, followed by the current line terminator, to the standard output stream.</summary>
        /// <param name="value">The value to write. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void WriteLine(double value)
        {
            SystemConsole.WriteLine(value);
        }

        /// <summary>Writes the text representation of the specified single-precision floating-point value, followed by the current line terminator, to the standard output stream.</summary>
        /// <param name="value">The value to write. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void WriteLine(float value)
        {
            SystemConsole.WriteLine(value);
        }

        /// <summary>Writes the text representation of the specified 32-bit signed integer value, followed by the current line terminator, to the standard output stream.</summary>
        /// <param name="value">The value to write. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void WriteLine(int value)
        {
            SystemConsole.WriteLine(value);
        }

        /// <summary>Writes the text representation of the specified 32-bit unsigned integer value, followed by the current line terminator, to the standard output stream.</summary>
        /// <param name="value">The value to write. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void WriteLine(uint value)
        {
            SystemConsole.WriteLine(value);
        }

        /// <summary>Writes the text representation of the specified 64-bit signed integer value, followed by the current line terminator, to the standard output stream.</summary>
        /// <param name="value">The value to write. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void WriteLine(long value)
        {
            SystemConsole.WriteLine(value);
        }

        /// <summary>Writes the text representation of the specified 64-bit unsigned integer value, followed by the current line terminator, to the standard output stream.</summary>
        /// <param name="value">The value to write. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void WriteLine(ulong value)
        {
            SystemConsole.WriteLine(value);
        }

        /// <summary>Writes the text representation of the specified object, followed by the current line terminator, to the standard output stream.</summary>
        /// <param name="value">The value to write. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void WriteLine(object value)
        {
            SystemConsole.WriteLine(value);
        }

        /// <summary>Writes the specified string value, followed by the current line terminator, to the standard output stream.</summary>
        /// <param name="value">The value to write. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void WriteLine(string value)
        {
            SystemConsole.WriteLine(value);
        }

        #endregion

        #region Write

        /// <summary>Writes the text representation of the specified array of objects to the standard output stream using the specified format information.</summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg">An array of objects to write using <paramref name="format" />. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="format" /> or <paramref name="arg" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.FormatException">The format specification in <paramref name="format" /> is invalid. </exception>
        public static void Write(string format, params object[] arg)
        {
            SystemConsole.Write(format, arg);
        }

        /// <summary>Writes the specified array of Unicode characters to the standard output stream.</summary>
        /// <param name="buffer">A Unicode character array. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void Write(char[] buffer)
        {
            SystemConsole.Write(buffer);
        }

        /// <summary>Writes the specified subarray of Unicode characters to the standard output stream.</summary>
        /// <param name="buffer">An array of Unicode characters. </param>
        /// <param name="index">The starting position in <paramref name="buffer" />. </param>
        /// <param name="count">The number of characters to write. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="buffer" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> or <paramref name="count" /> is less than zero. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="index" /> plus <paramref name="count" /> specify a position that is not within <paramref name="buffer" />. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void Write(char[] buffer, int index, int count)
        {
            SystemConsole.Write(buffer, index, count);
        }

        /// <summary>Writes the text representation of the specified Boolean value to the standard output stream.</summary>
        /// <param name="value">The value to write. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void Write(bool value)
        {
            SystemConsole.Write(value);
        }

        /// <summary>Writes the specified Unicode character value to the standard output stream.</summary>
        /// <param name="value">The value to write. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void Write(char value)
        {
            SystemConsole.Write(value);
        }

        /// <summary>Writes the text representation of the specified double-precision floating-point value to the standard output stream.</summary>
        /// <param name="value">The value to write. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void Write(double value)
        {
            SystemConsole.Write(value);
        }

        /// <summary>Writes the text representation of the specified <see cref="T:System.Decimal" /> value to the standard output stream.</summary>
        /// <param name="value">The value to write. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void Write(decimal value)
        {
            SystemConsole.Write(value);
        }

        /// <summary>Writes the text representation of the specified single-precision floating-point value to the standard output stream.</summary>
        /// <param name="value">The value to write. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void Write(float value)
        {
            SystemConsole.Write(value);
        }

        /// <summary>Writes the text representation of the specified 32-bit signed integer value to the standard output stream.</summary>
        /// <param name="value">The value to write. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void Write(int value)
        {
            SystemConsole.Write(value);
        }

        /// <summary>Writes the text representation of the specified 32-bit unsigned integer value to the standard output stream.</summary>
        /// <param name="value">The value to write. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void Write(uint value)
        {
            SystemConsole.Write(value);
        }

        /// <summary>Writes the text representation of the specified 64-bit signed integer value to the standard output stream.</summary>
        /// <param name="value">The value to write. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void Write(long value)
        {
            SystemConsole.Write(value);
        }

        /// <summary>Writes the text representation of the specified 64-bit unsigned integer value to the standard output stream.</summary>
        /// <param name="value">The value to write. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void Write(ulong value)
        {
            SystemConsole.Write(value);
        }

        /// <summary>Writes the text representation of the specified object to the standard output stream.</summary>
        /// <param name="value">The value to write, or <see langword="null" />. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void Write(object value)
        {
            SystemConsole.Write(value);
        }

        /// <summary>Writes the specified string value to the standard output stream.</summary>
        /// <param name="value">The value to write. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public static void Write(string value)
        {
            SystemConsole.Write(value);
        }

        #endregion

        #endregion

        #region Streams

        /// <summary>Acquires the standard error stream.</summary>
        /// <returns>The standard error stream.</returns>
        public static Stream OpenStandardError()
        {
            return SystemConsole.OpenStandardError();
        }

        /// <summary>Acquires the standard error stream, which is set to a specified buffer size.</summary>
        /// <param name="bufferSize">The internal stream buffer size. </param>
        /// <returns>The standard error stream.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is less than or equal to zero. </exception>
        public static Stream OpenStandardError(int bufferSize)
        {
            return SystemConsole.OpenStandardError(bufferSize);
        }

        /// <summary>Acquires the standard input stream.</summary>
        /// <returns>The standard input stream.</returns>
        public static Stream OpenStandardInput()
        {
            return SystemConsole.OpenStandardInput();
        }

        /// <summary>Acquires the standard input stream, which is set to a specified buffer size.</summary>
        /// <param name="bufferSize">The internal stream buffer size. </param>
        /// <returns>The standard input stream.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is less than or equal to zero. </exception>
        public static Stream OpenStandardInput(int bufferSize)
        {
            return SystemConsole.OpenStandardInput(bufferSize);
        }

        /// <summary>Acquires the standard output stream.</summary>
        /// <returns>The standard output stream.</returns>
        public static Stream OpenStandardOutput()
        {
            return SystemConsole.OpenStandardOutput();
        }

        /// <summary>Acquires the standard output stream, which is set to a specified buffer size.</summary>
        /// <param name="bufferSize">The internal stream buffer size. </param>
        /// <returns>The standard output stream.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is less than or equal to zero. </exception>
        public static Stream OpenStandardOutput(int bufferSize)
        {
            return SystemConsole.OpenStandardOutput(bufferSize);
        }

        #endregion

        #region Reader/Writer

        /// <summary>Gets a value that indicates whether input has been redirected from the standard input stream.</summary>
        /// <returns>
        /// <see langword="true" /> if input is redirected; otherwise, <see langword="false" />.</returns>
        public static bool IsInputRedirected => SystemConsole.IsInputRedirected;

        /// <summary>Gets a value that indicates whether output has been redirected from the standard output stream.</summary>
        /// <returns>
        /// <see langword="true" /> if output is redirected; otherwise, <see langword="false" />.</returns>
        public static bool IsOutputRedirected => SystemConsole.IsOutputRedirected;

        /// <summary>Gets a value that indicates whether the error output stream has been redirected from the standard error stream.</summary>
        /// <returns>
        /// <see langword="true" /> if error output is redirected; otherwise, <see langword="false" />.</returns>
        public static bool IsErrorRedirected => SystemConsole.IsErrorRedirected;

        /// <summary>Gets the standard input stream.</summary>
        /// <returns>A <see cref="T:System.IO.TextReader" /> that represents the standard input stream.</returns>
        public static TextReader In
        {
            get => SystemConsole.In;
            set => SetIn(value);
        }

        /// <summary>Gets the standard output stream.</summary>
        /// <returns>A <see cref="T:System.IO.TextWriter" /> that represents the standard output stream.</returns>
        public static TextWriter Out
        {
            get => SystemConsole.Out;
            set => SetOut(value);
        }

        /// <summary>Gets the standard error output stream.</summary>
        /// <returns>A <see cref="T:System.IO.TextWriter" /> that represents the standard error output stream.</returns>
        public static TextWriter Error
        {
            get => SystemConsole.Error;
            set => SetError(value);
        }

        /// <summary>Sets the <see cref="P:System.Console.In" /> property to the specified <see cref="T:System.IO.TextReader" /> object.</summary>
        /// <param name="newIn">A stream that is the new standard input. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="newIn" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public static void SetIn(TextReader newIn)
        {
            SystemConsole.SetIn(newIn);
        }

        /// <summary>Sets the <see cref="P:System.Console.Out" /> property to the specified <see cref="T:System.IO.TextWriter" /> object.</summary>
        /// <param name="newOut">A stream that is the new standard output. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="newOut" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public static void SetOut(TextWriter newOut)
        {
            SystemConsole.SetOut(newOut);
        }

        /// <summary>Sets the <see cref="P:System.Console.Error" /> property to the specified <see cref="T:System.IO.TextWriter" /> object.</summary>
        /// <param name="newError">A stream that is the new standard error output. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="newError" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public static void SetError(TextWriter newError)
        {
            SystemConsole.SetError(newError);
        }

        #endregion

        #endregion

    }

}
