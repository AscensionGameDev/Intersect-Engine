using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Intersect.IO
{

    public sealed class ConsoleContext
    {

        public static readonly int DefaultInputHistoryLength = 100;

        private readonly List<string> mInputHistory;

        private readonly object mLock;

        private bool mInputHistoryEnabled;

        private int mInputHistoryLength;

        private int mInputHistoryPosition;

        public ConsoleContext(int inputHistoryLength = 0)
        {
            mLock = new object();

            mInputHistory = new List<string>(Math.Max(0, inputHistoryLength));
            InputHistoryLength = inputHistoryLength;
            InputBuffer = new List<char>(80);
        }

        private int WaitCursorLeft { get; set; }

        private int WaitCursorTop { get; set; }

        public bool IsWaitingRead { get; private set; }

        public int InputHistoryPosition
        {
            get => mInputHistoryPosition;
            set => mInputHistoryPosition = Math.Max(0, Math.Min(mInputHistory.Count, value));
        }

        public bool InputHistoryEnabled
        {
            get => mInputHistoryEnabled;
            set
            {
                mInputHistoryEnabled = value;
                mInputHistoryLength = mInputHistoryEnabled ? DefaultInputHistoryLength : 0;
            }
        }

        public int InputHistoryLength
        {
            get => mInputHistoryLength;
            set
            {
                mInputHistoryLength = value;
                mInputHistoryEnabled = mInputHistoryLength != 0;
            }
        }

        public bool InputHistoryFull => mInputHistoryLength > 0 && mInputHistoryLength <= mInputHistory.Count;

        public ImmutableList<string> InputHistory =>
            mInputHistory.ToImmutableList() ?? throw new InvalidOperationException();

        public string WaitPrefix { get; set; }

        private List<char> InputBuffer { get; }

        public void Check()
        {
            if (IsWaitingRead)
            {
                throw new InvalidOperationException("Already waiting on a Read*() operation.");
            }
        }

        public TResult Wait<TResult>(Action<string> write, Func<TResult> waitForResult)
        {
            lock (mLock)
            {
                IsWaitingRead = true;
                WritePrefix(write);
            }

            return waitForResult();
        }

        public void Clear()
        {
            lock (mLock)
            {
                IsWaitingRead = false;
            }
        }

        public void ResetWaitCursor(Action<string> write, int clearBufferLength = -1)
        {
            if (!IsWaitingRead)
            {
                return;
            }

            var onCurrentLine = Console.CursorTop == WaitCursorTop;
            var realColumnCursor = onCurrentLine ? WaitCursorLeft : 0;
            var currentLineCursor = Console.CursorTop;

            Console.SetCursorPosition(realColumnCursor, Console.CursorTop);
            write(new string(' ', (WaitPrefix?.Length ?? 0) + Math.Max(clearBufferLength, InputBuffer.Count)));
            Console.SetCursorPosition(realColumnCursor, currentLineCursor);

            if (onCurrentLine)
            {
                return;
            }

            Console.CursorLeft = WaitCursorLeft;
            Console.CursorTop = WaitCursorTop;
        }

        public async Task ResetWaitCursorAsync(Func<string, Task> writeAsync, int clearBufferLength = -1)
        {
            if (!IsWaitingRead)
            {
                return;
            }

            var onCurrentLine = Console.CursorTop == WaitCursorTop;

            var realColumnCursor = onCurrentLine ? WaitCursorLeft : 0;
            var currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(realColumnCursor, Console.CursorTop);

            var task = writeAsync(
                new string(' ', (WaitPrefix?.Length ?? 0) + Math.Max(clearBufferLength, InputBuffer.Count))
            );

            if (task != null)
            {
                await task;
            }

            Console.SetCursorPosition(realColumnCursor, currentLineCursor);

            if (onCurrentLine)
            {
                return;
            }

            Console.CursorLeft = WaitCursorLeft;
            Console.CursorTop = WaitCursorTop;
        }

        public void MoveWaitCursor(bool subtractPrefix = false)
        {
            WaitCursorTop = Console.CursorTop;
            WaitCursorLeft = Console.CursorLeft;

            if (subtractPrefix)
            {
                WaitCursorLeft -= WaitPrefix?.Length ?? 0;
            }
        }

        public void WritePrefix(Action<string> write, int bufferPosition = -1)
        {
            if (!IsWaitingRead)
            {
                return;
            }

            MoveWaitCursor();

            if (string.IsNullOrEmpty(WaitPrefix))
            {
                return;
            }

            write(WaitPrefix + new string(InputBuffer.ToArray()));
            if (bufferPosition > -1)
            {
                Console.CursorLeft -= Math.Max(0, InputBuffer.Count - bufferPosition);
            }
        }

        public async Task WritePrefixAsync(Func<string, Task> writeAsync)
        {
            if (!IsWaitingRead)
            {
                return;
            }

            MoveWaitCursor();

            if (string.IsNullOrEmpty(WaitPrefix))
            {
                return;
            }

            var task = writeAsync(WaitPrefix);
            if (task != null)
            {
                await task;
            }
        }

        public string BufferedReadLine(
            Action<string> write,
            Action writeLine,
            Func<ConsoleKeyInfo> readKey
        )
        {
            return Wait(
                write, () =>
                {
                    InputBuffer.Clear();

                    var line = "";
                    var finished = false;
                    while (!finished)
                    {
                        var keyInfo = readKey();
                        var bufferPosition = Math.Max(
                            0,
                            Math.Min(
                                InputBuffer.Count,
                                Console.BufferWidth * (Console.CursorTop - WaitCursorTop) +
                                (Console.CursorLeft - WaitCursorLeft) -
                                (WaitPrefix?.Length ?? 0)
                            )
                        );

                        var bufferLength = InputBuffer.Count;

                        switch (keyInfo.Key)
                        {
                            case ConsoleKey.Enter:
                                line = new string(InputBuffer.ToArray());
                                InputBuffer.Clear();
                                if (InputHistoryFull)
                                {
                                    mInputHistory.RemoveRange(0, mInputHistory.Count - mInputHistoryLength);
                                }

                                mInputHistory.Add(line);
                                InputHistoryPosition = mInputHistory.Count;
                                writeLine();
                                MoveWaitCursor();
                                finished = true;

                                break;

                            case ConsoleKey.Backspace:
                                --bufferPosition;
                                if (-1 < bufferPosition && bufferPosition < InputBuffer.Count)
                                {
                                    InputBuffer.RemoveAt(bufferPosition);
                                }

                                break;

                            case ConsoleKey.Delete:
                                if (bufferPosition < InputBuffer.Count)
                                {
                                    InputBuffer.RemoveAt(bufferPosition);
                                }

                                break;

                            case ConsoleKey.UpArrow:
                                --InputHistoryPosition;
                                if (mInputHistory.Count > InputHistoryPosition)
                                {
                                    InputBuffer.Clear();
                                    InputBuffer.AddRange(
                                        mInputHistory[InputHistoryPosition]?.ToCharArray() ?? Array.Empty<char>()
                                    );
                                }

                                break;

                            case ConsoleKey.DownArrow:
                                ++InputHistoryPosition;
                                if (mInputHistory.Count > InputHistoryPosition)
                                {
                                    InputBuffer.Clear();
                                    InputBuffer.AddRange(
                                        mInputHistory[InputHistoryPosition]?.ToCharArray() ?? Array.Empty<char>()
                                    );
                                }
                                else if (mInputHistory.Count == InputHistoryPosition && InputHistoryPosition > 0)
                                {
                                    InputBuffer.Clear();
                                }

                                break;

                            case ConsoleKey.LeftArrow:
                                --bufferPosition;

                                break;

                            case ConsoleKey.RightArrow:
                                ++bufferPosition;

                                break;

                            #region Default key handling

                            case ConsoleKey.Tab:
                            case ConsoleKey.Clear:
                            case ConsoleKey.Pause:
                            case ConsoleKey.Escape:
                            case ConsoleKey.Spacebar:
                            case ConsoleKey.PageUp:
                            case ConsoleKey.PageDown:
                            case ConsoleKey.End:
                            case ConsoleKey.Home:
                            case ConsoleKey.Select:
                            case ConsoleKey.Print:
                            case ConsoleKey.Execute:
                            case ConsoleKey.PrintScreen:
                            case ConsoleKey.Insert:
                            case ConsoleKey.Help:
                            case ConsoleKey.D0:
                            case ConsoleKey.D1:
                            case ConsoleKey.D2:
                            case ConsoleKey.D3:
                            case ConsoleKey.D4:
                            case ConsoleKey.D5:
                            case ConsoleKey.D6:
                            case ConsoleKey.D7:
                            case ConsoleKey.D8:
                            case ConsoleKey.D9:
                            case ConsoleKey.A:
                            case ConsoleKey.B:
                            case ConsoleKey.C:
                            case ConsoleKey.D:
                            case ConsoleKey.E:
                            case ConsoleKey.F:
                            case ConsoleKey.G:
                            case ConsoleKey.H:
                            case ConsoleKey.I:
                            case ConsoleKey.J:
                            case ConsoleKey.K:
                            case ConsoleKey.L:
                            case ConsoleKey.M:
                            case ConsoleKey.N:
                            case ConsoleKey.O:
                            case ConsoleKey.P:
                            case ConsoleKey.Q:
                            case ConsoleKey.R:
                            case ConsoleKey.S:
                            case ConsoleKey.T:
                            case ConsoleKey.U:
                            case ConsoleKey.V:
                            case ConsoleKey.W:
                            case ConsoleKey.X:
                            case ConsoleKey.Y:
                            case ConsoleKey.Z:
                            case ConsoleKey.LeftWindows:
                            case ConsoleKey.RightWindows:
                            case ConsoleKey.Applications:
                            case ConsoleKey.Sleep:
                            case ConsoleKey.NumPad0:
                            case ConsoleKey.NumPad1:
                            case ConsoleKey.NumPad2:
                            case ConsoleKey.NumPad3:
                            case ConsoleKey.NumPad4:
                            case ConsoleKey.NumPad5:
                            case ConsoleKey.NumPad6:
                            case ConsoleKey.NumPad7:
                            case ConsoleKey.NumPad8:
                            case ConsoleKey.NumPad9:
                            case ConsoleKey.Multiply:
                            case ConsoleKey.Add:
                            case ConsoleKey.Separator:
                            case ConsoleKey.Subtract:
                            case ConsoleKey.Decimal:
                            case ConsoleKey.Divide:
                            case ConsoleKey.F1:
                            case ConsoleKey.F2:
                            case ConsoleKey.F3:
                            case ConsoleKey.F4:
                            case ConsoleKey.F5:
                            case ConsoleKey.F6:
                            case ConsoleKey.F7:
                            case ConsoleKey.F8:
                            case ConsoleKey.F9:
                            case ConsoleKey.F10:
                            case ConsoleKey.F11:
                            case ConsoleKey.F12:
                            case ConsoleKey.F13:
                            case ConsoleKey.F14:
                            case ConsoleKey.F15:
                            case ConsoleKey.F16:
                            case ConsoleKey.F17:
                            case ConsoleKey.F18:
                            case ConsoleKey.F19:
                            case ConsoleKey.F20:
                            case ConsoleKey.F21:
                            case ConsoleKey.F22:
                            case ConsoleKey.F23:
                            case ConsoleKey.F24:
                            case ConsoleKey.BrowserBack:
                            case ConsoleKey.BrowserForward:
                            case ConsoleKey.BrowserRefresh:
                            case ConsoleKey.BrowserStop:
                            case ConsoleKey.BrowserSearch:
                            case ConsoleKey.BrowserFavorites:
                            case ConsoleKey.BrowserHome:
                            case ConsoleKey.VolumeMute:
                            case ConsoleKey.VolumeDown:
                            case ConsoleKey.VolumeUp:
                            case ConsoleKey.MediaNext:
                            case ConsoleKey.MediaPrevious:
                            case ConsoleKey.MediaStop:
                            case ConsoleKey.MediaPlay:
                            case ConsoleKey.LaunchMail:
                            case ConsoleKey.LaunchMediaSelect:
                            case ConsoleKey.LaunchApp1:
                            case ConsoleKey.LaunchApp2:
                            case ConsoleKey.Oem1:
                            case ConsoleKey.OemPlus:
                            case ConsoleKey.OemComma:
                            case ConsoleKey.OemMinus:
                            case ConsoleKey.OemPeriod:
                            case ConsoleKey.Oem2:
                            case ConsoleKey.Oem3:
                            case ConsoleKey.Oem4:
                            case ConsoleKey.Oem5:
                            case ConsoleKey.Oem6:
                            case ConsoleKey.Oem7:
                            case ConsoleKey.Oem8:
                            case ConsoleKey.Oem102:
                            case ConsoleKey.Process:
                            case ConsoleKey.Packet:
                            case ConsoleKey.Attention:
                            case ConsoleKey.CrSel:
                            case ConsoleKey.ExSel:
                            case ConsoleKey.EraseEndOfFile:
                            case ConsoleKey.Play:
                            case ConsoleKey.Zoom:
                            case ConsoleKey.NoName:
                            case ConsoleKey.Pa1:
                            case ConsoleKey.OemClear:

                            default:
                                InputBuffer.Add(keyInfo.KeyChar);
                                ++bufferPosition;

                                break;

                            #endregion
                        }

                        // ReSharper disable once InvertIf
                        if (!finished)
                        {
                            // TODO: Soft cursor reset and prefix rewrite
                            ResetWaitCursor(write, bufferLength);
                            WritePrefix(write, bufferPosition);
                        }
                    }

                    return line;
                }
            );
        }

    }

}
