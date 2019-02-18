using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Intersect.IO
{
    public sealed class ConsoleWaitContext
    {
        [NotNull] private readonly object mLock;

        private int WaitCursorLeft { get; set; }

        private int WaitCursorTop { get; set; }

        public bool IsWaitingRead { get; private set; }

        public string WaitPrefix { get; set; }

        public ConsoleWaitContext()
        {
            mLock = new object();
        }

        public void Check()
        {
            if (IsWaitingRead)
            {
                throw new InvalidOperationException("Already waiting on a Read*() operation.");
            }
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
            write(new string(' ', WaitPrefix?.Length ?? 0));
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

            var task = writeAsync(new string(' ', WaitPrefix?.Length ?? 0));
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

            write(WaitPrefix);
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
