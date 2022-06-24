using System;
using System.Threading;

namespace Intersect.Async
{
    public partial class CancellableGenerator<TValue> : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        public CancellableGenerator(Func<CancellationToken, AsyncValueGenerator<TValue>> generatorFactory)
        {
            if (generatorFactory == default)
            {
                throw new ArgumentNullException(nameof(generatorFactory));
            }

            _cancellationTokenSource = new CancellationTokenSource();
            Generator = generatorFactory(_cancellationTokenSource.Token);
        }

        public AsyncValueGenerator<TValue> Generator { get; }

        public CancellableGenerator<TValue> Start()
        {
            _ = Generator.Start();
            return this;
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}
