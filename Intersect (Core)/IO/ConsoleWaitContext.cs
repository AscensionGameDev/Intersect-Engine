using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Intersect.IO
{
    public sealed class ConsoleWaitContext
    {
        [NotNull] private readonly object mLock;

        private int WaitCursorLeft { get; set; }

        private int WaitCursorTop { get; set; }

        public bool IsWaitingRead { get; private set; }

        public string WaitPrefix { get; set; }

        [NotNull]
        private List<char> InputBuffer { get; }

        public ConsoleWaitContext()
        {
            mLock = new object();

            InputBuffer = new List<char>(80);
        }

        public void Check()
        {
            if (IsWaitingRead)
            {
                throw new InvalidOperationException("Already waiting on a Read*() operation.");
            }
        }

        public string BufferedReadLine([NotNull] Action<string> write, [NotNull] Action writeLine,
            [NotNull] Func<ConsoleKeyInfo> readKey)
        {
            return Wait(write, () =>
            {
                InputBuffer.Clear();

                var finished = false;
                while (!finished)
                {
                    var keyInfo = readKey();
                    var bufferPosition = Math.Max(
                        0,
                        Math.Min(
                            InputBuffer.Count,
                            Console.BufferWidth * (Console.CursorTop - WaitCursorTop) +
                            (Console.CursorLeft - WaitCursorLeft) - (WaitPrefix?.Length ?? 0)
                        )
                    );

                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.Enter:
                            writeLine();
                            finished = true;
                            break;

                        case ConsoleKey.Backspace:
                            if (bufferPosition > -1 && bufferPosition < InputBuffer.Count)
                            {
                                InputBuffer.RemoveAt(bufferPosition);
                            }
                            else
                            {
                                if (Console.CursorLeft + 1 >= Console.BufferWidth)
                                {
                                    ++Console.CursorTop;
                                }

                                Console.CursorLeft = (Console.CursorLeft + 1) % Console.BufferWidth;
                            }

                            break;

                        case ConsoleKey.Delete:
                            if (bufferPosition + 1 < InputBuffer.Count)
                            {
                                InputBuffer.RemoveAt(bufferPosition + 1);
                            }
                            else
                            {
                                if (Console.CursorLeft - 1 < 0)
                                {
                                    --Console.CursorTop;
                                }

                                Console.CursorLeft =
                                    (Console.CursorLeft + Console.BufferWidth - 1) % Console.BufferWidth;
                            }
                            break;

                        // TODO: Input history
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.DownArrow:
                        case ConsoleKey.LeftArrow:
                        case ConsoleKey.RightArrow:
                            break;

                        default:
                            InputBuffer.Add(keyInfo.KeyChar);
                            break;
                    }

                    // TODO: Soft cursor reset and prefix rewrite
                    //ResetWaitCursor(write);
                    //WritePrefix(write);
                }

                var line = new string(InputBuffer.ToArray());
                InputBuffer.Clear();
                return line;
            });
        }

        public TResult Wait<TResult>([NotNull] Action<string> write, [NotNull] Func<TResult> waitForResult)
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

        public void ResetWaitCursor([NotNull] Action<string> write)
        {
            if (!IsWaitingRead)
            {
                return;
            }

            var onCurrentLine = Console.CursorTop == WaitCursorTop;
            var realColumnCursor = onCurrentLine ? WaitCursorLeft : 0;
            var currentLineCursor = Console.CursorTop;

            Console.SetCursorPosition(realColumnCursor, Console.CursorTop);
            write(new string(' ', WaitPrefix?.Length ?? 0 + InputBuffer.Count));
            Console.SetCursorPosition(realColumnCursor, currentLineCursor);

            if (onCurrentLine)
            {
                return;
            }

            Console.CursorLeft = WaitCursorLeft;
            Console.CursorTop = WaitCursorTop;
        }

        public async Task ResetWaitCursorAsync([NotNull] Func<string, Task> writeAsync)
        {
            if (!IsWaitingRead)
            {
                return;
            }

            var onCurrentLine = Console.CursorTop == WaitCursorTop;

            var realColumnCursor = onCurrentLine ? WaitCursorLeft : 0;
            var currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(realColumnCursor, Console.CursorTop);

            var task = writeAsync(new string(' ', WaitPrefix?.Length ?? 0 + InputBuffer.Count));
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

        public void WritePrefix([NotNull] Action<string> write)
        {
            if (!IsWaitingRead)
            {
                return;
            }

            WaitCursorLeft = Console.CursorLeft;
            WaitCursorTop = Console.CursorTop;

            if (string.IsNullOrEmpty(WaitPrefix))
            {
                return;
            }

            write(WaitPrefix + new string(InputBuffer.ToArray()));
        }

        public async Task WritePrefixAsync([NotNull] Func<string, Task> writeAsync)
        {
            if (!IsWaitingRead)
            {
                return;
            }

            WaitCursorLeft = Console.CursorLeft;
            WaitCursorTop = Console.CursorTop;

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
    }
}