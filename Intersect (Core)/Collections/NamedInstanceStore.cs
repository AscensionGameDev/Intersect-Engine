using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Intersect.Collections
{

    public sealed class NamedInstanceStore<TInstance>
    {

        private Func<TInstance> mFactory;

        private IDictionary<string, TInstance> mInstances;

        public NamedInstanceStore(Func<TInstance> factory)
        {
            mFactory = factory;
            mInstances = new ConcurrentDictionary<string, TInstance>();
        }

        public bool TryGetValue(string name, out TInstance instance)
        {
            if (!mInstances.TryGetValue(name, out instance))
            {
                instance = mFactory();
            }

            return instance != null;
        }

        public TInstance GetInstance(string name)
        {
            TryGetValue(name, out var instance);

            return instance;
        }

    }

}
