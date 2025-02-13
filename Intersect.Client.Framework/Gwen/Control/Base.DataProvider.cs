using Intersect.Client.Interface.Data;
using Intersect.Core;
using Intersect.Framework.Reflection;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Framework.Gwen.Control;

public partial class Base
{
    private readonly HashSet<IDataProvider> _dataProviders = [];
    private readonly Dictionary<IUpdatableDataProvider, int> _updatableDataProviders = [];

    /// <summary>
    /// Adds a <see cref="IDataProvider"/> to this node.
    /// </summary>
    /// <param name="dataProvider">The <see cref="IDataProvider"/> to add.</param>
    /// <returns>If the data provider was added, returns false if it was already added.</returns>
    public bool AddDataProvider(IDataProvider dataProvider)
    {
        if (!_dataProviders.Add(dataProvider))
        {
            return false;
        }

        if (dataProvider is IUpdatableDataProvider updatableDataProvider)
        {
            AddUpdatableDataProvider(updatableDataProvider);
        }

        return true;
    }

    /// <summary>
    /// Removes a <see cref="IDataProvider"/> from this node.
    /// </summary>
    /// <param name="dataProvider">The <see cref="IDataProvider"/> to remove.</param>
    /// <returns>If the data provider was removed, returns false if it was not already added.</returns>
    public bool RemoveDataProvider(IDataProvider dataProvider)
    {
        if (!_dataProviders.Remove(dataProvider))
        {
            return false;
        }

        if (dataProvider is IUpdatableDataProvider updatableDataProvider)
        {
            RemoveUpdatableDataProvider(updatableDataProvider);
        }

        return true;
    }

    private void AddUpdatableDataProvider(IUpdatableDataProvider updatableDataProvider)
    {
        var refCount = _updatableDataProviders.GetValueOrDefault(updatableDataProvider, 0);
        ++refCount;
        _updatableDataProviders[updatableDataProvider] = refCount;

        var root = Root;
        if (root != this)
        {
            root.AddUpdatableDataProvider(updatableDataProvider);
        }
    }

    private void AddUpdatableDataProviders(IEnumerable<IUpdatableDataProvider> updatableDataProviders)
    {
        foreach (var updatableDataProvider in updatableDataProviders)
        {
            AddUpdatableDataProvider(updatableDataProvider);
        }
    }

    private void RemoveUpdatableDataProvider(IUpdatableDataProvider updatableDataProvider)
    {
        var refCount = _updatableDataProviders.GetValueOrDefault(updatableDataProvider, 0);
        --refCount;

        if (refCount > 0)
        {
            _updatableDataProviders[updatableDataProvider] = refCount;
        }
        else
        {
            _updatableDataProviders.Remove(updatableDataProvider);
        }

        var root = Root;
        if (root != this)
        {
            root.RemoveUpdatableDataProvider(updatableDataProvider);
        }
    }

    private void RemoveUpdatableDataProviders(IEnumerable<IUpdatableDataProvider> updatableDataProviders)
    {
        foreach (var updatableDataProvider in updatableDataProviders)
        {
            RemoveUpdatableDataProvider(updatableDataProvider);
        }
    }

    protected void UpdateDataProviders(TimeSpan elapsed, TimeSpan total)
    {
        foreach (var (dataProvider, _) in _updatableDataProviders)
        {
            try
            {
                if (dataProvider.TryUpdate(elapsed, total))
                {
                    // TODO: LogTrace() when the value has changed
                }
                else
                {
                    // TODO: LogTrace() when the value has not changed
                }
            }
            catch (Exception exception)
            {
                ApplicationContext.Context.Value?.Logger.LogWarning(
                    exception,
                    "An exception was improperly thrown from {DataProviderType}.TryUpdate(TimeSpan, TimeSpan)",
                    dataProvider.GetType().GetName(qualified: true)
                );
            }
        }
    }
}