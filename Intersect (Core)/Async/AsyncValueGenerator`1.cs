using System;
using System.Threading;
using System.Threading.Tasks;

namespace Intersect.Async
{
    public partial class AsyncValueGenerator<TValue> : IDisposable
    {
        private readonly CancellationToken _cancellationToken;
        private readonly Task _task;
        private readonly Func<Task<TValue>> _valueGenerator;
        private readonly Action<TValue> _valueHandler;

        public AsyncValueGenerator(Func<Task<TValue>> valueGenerator, Action<TValue> valueHandler, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _task = new Task(DoLoop, cancellationToken);
            _valueGenerator = valueGenerator;
            _valueHandler = valueHandler;
        }

        public void Dispose() => _task?.Dispose();

        private async void DoLoop()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                var task = _valueGenerator();
                var value = await task.ConfigureAwait(false);
                _valueHandler(value);
            }
        }

        public AsyncValueGenerator<TValue> Start()
        {
            _task.Start();
            return this;
        }
    }
}
