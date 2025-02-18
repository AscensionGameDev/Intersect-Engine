using Intersect.Client.Interface.Data;
using Intersect.Core;
using Intersect.Framework.Reflection;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Framework.Gwen.Control;

public partial class Base
{
    private readonly HashSet<IDataProvider> _dataProviders = [];
    private readonly Dictionary<IUpdatableDataProvider, DataProviderRefCount> _updatableDataProviderRefCounts = [];

    private struct DataProviderRefCount
    {
        public int Listening;
        public int Visible;
    }

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
            ListenUpdatableDataProvider(updatableDataProvider, true);
            if (IsVisibleInTree)
            {
                VisibleUpdatableDataProvider(updatableDataProvider, true);
            }
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
            ListenUpdatableDataProvider(updatableDataProvider, false);
            if (IsVisibleInTree)
            {
                VisibleUpdatableDataProvider(updatableDataProvider, false);
            }
        }

        return true;
    }

    private void ListenUpdatableDataProviders(IEnumerable<IUpdatableDataProvider> providers, bool listen)
    {
        foreach (var provider in providers)
        {
            ListenUpdatableDataProvider(provider, listen);
        }
    }

    private void ListenUpdatableDataProvider(IUpdatableDataProvider provider, bool listen)
    {
        var refCount = _updatableDataProviderRefCounts.GetValueOrDefault(key: provider, defaultValue: default);
        if (listen)
        {
            ++refCount.Listening;
        }
        else
        {
            --refCount.Listening;
        }

        if (refCount.Listening > 0)
        {
            _updatableDataProviderRefCounts[key: provider] = refCount;
        }
        else
        {
            _updatableDataProviderRefCounts.Remove(key: provider);
        }

        var root = Root;
        if (root != this)
        {
            root.ListenUpdatableDataProvider(provider: provider, listen: listen);
        }
    }

    private void VisibleUpdatableDataProviders(IEnumerable<IUpdatableDataProvider> providers, bool visible)
    {
        foreach (var provider in providers)
        {
            VisibleUpdatableDataProvider(provider, visible);
        }
    }

    private void VisibleUpdatableDataProvider(IUpdatableDataProvider updatableDataProvider, bool visible)
    {
        if (!_updatableDataProviderRefCounts.TryGetValue(updatableDataProvider, out var refCount))
        {
            throw new InvalidOperationException("Cannot mark a data provider as visible before listening to it");
        }

        if (visible)
        {
            ++refCount.Visible;
        }
        else if (refCount.Visible > 0)
        {
            --refCount.Visible;
        }
        else
        {
            ApplicationContext.CurrentContext.Logger.LogError(
                "Tried to decrease visible count below 0 of a data provider"
            );
        }

        _updatableDataProviderRefCounts[key: updatableDataProvider] = refCount;

        var root = Root;
        if (root != this)
        {
            root.VisibleUpdatableDataProvider(updatableDataProvider: updatableDataProvider, visible: visible);
        }
    }


    protected void UpdateDataProviders(TimeSpan elapsed, TimeSpan total)
    {
        foreach (var (dataProvider, refCount) in _updatableDataProviderRefCounts)
        {
            if (refCount.Visible < 1)
            {
                continue;
            }

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