using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace Intersect.Collections
{

    public sealed class NamedInstanceStore<TInstance>
    {

        [NotNull] private Func<TInstance> mFactory;

        [NotNull] private IDictionary<string, TInstance> mInstances;

        public NamedInstanceStore([NotNull] Func<TInstance> factory)
        {
            mFactory = factory;
            mInstances = new ConcurrentDictionary<string, TInstance>();
        }

        public bool TryGetValue([NotNull] string name, out TInstance instance)
        {
            if (!mInstances.TryGetValue(name, out instance))
            {
                instance = mFactory();
            }

            return instance != null;
        }

        public TInstance GetInstance([NotNull] string name)
        {
            TryGetValue(name, out var instance);

            return instance;
        }

    }

}
