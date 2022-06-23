using System;
using System.Threading;
using System.Threading.Tasks;

using Intersect.Async;

namespace Intersect.Client.Framework.Gwen.Control.Data
{
    public partial class ValueTableCellDataProvider<TValue> : ITableCellDataProvider
    {
        public ValueTableCellDataProvider(Func<TValue> provideValue, int delay = 100)
        {
            AsyncValueGenerator<TValue> CreateValueGenerator(CancellationToken cancellationToken)
            {
                return new AsyncValueGenerator<TValue>(
                    () => Task.Delay(delay).ContinueWith((_) => cancellationToken.IsCancellationRequested ? default : provideValue(), TaskScheduler.Current),
                    value => DataChanged?.Invoke(this, new CellDataChangedEventArgs(default, value)),
                    cancellationToken
                );
            }

            Generator = new CancellableGenerator<TValue>(CreateValueGenerator);
        }

        public ValueTableCellDataProvider(Func<CancellationToken, TValue> provideValue)
        {
            AsyncValueGenerator<TValue> CreateValueGenerator(CancellationToken cancellationToken)
            {
                return new AsyncValueGenerator<TValue>(
                    () => Task.Delay(100).ContinueWith((_) => cancellationToken.IsCancellationRequested ? default : provideValue(cancellationToken), TaskScheduler.Current),
                    value => DataChanged?.Invoke(this, new CellDataChangedEventArgs(default, value)),
                    cancellationToken
                );
            }

            Generator = new CancellableGenerator<TValue>(CreateValueGenerator);
        }

        public event TableCellDataChangedEventHandler DataChanged;

        public CancellableGenerator<TValue> Generator { get; }
    }
}
