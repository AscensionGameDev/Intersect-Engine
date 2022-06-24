using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Intersect.Async
{
    public sealed partial class ExceptionRepeater
    {
        private readonly int _delay;
        private readonly List<Exception> _exceptions;
        private readonly int _limit;

        private ExceptionRepeater(int limit, int delay = 1000)
        {
            _delay = delay;
            _exceptions = new List<Exception>();
            _limit = limit;
        }

        public void Run(Action action)
        {
            while (_exceptions.Count < _limit)
            {
                try
                {
                    action();
                    return;
                }
                catch (Exception exception)
                {
                    _exceptions.Add(exception);
                    if (_delay > 0)
                    {
                        Task.Delay(_delay).Wait();
                    }
                }
            }

            throw new AggregateException(_exceptions.ToArray());
        }

        public T Run<T>(Func<T> func)
        {
            while (_exceptions.Count < _limit)
            {
                try
                {
                    return func();
                }
                catch (Exception exception)
                {
                    _exceptions.Add(exception);
                    if (_delay > 0)
                    {
                        Task.Delay(_delay).Wait();
                    }
                }
            }

            throw new AggregateException(_exceptions.ToArray());
        }

        public static void Run(Action action, int limit, int delay = 1000) => new ExceptionRepeater(limit, delay).Run(action);

        public static T Run<T>(Func<T> func, int limit, int delay = 1000) => new ExceptionRepeater(limit, delay).Run(func);
    }
}
