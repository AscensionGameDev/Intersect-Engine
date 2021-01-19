using Intersect.Client.Framework.Gwen.Control;
using Intersect.Reflection;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Intersect.Client.Interface
{

    public abstract class MutableInterface : IMutableInterface
    {

        protected internal MutableInterface(Base root)
        {
            Root = root;
        }

        internal Base Root { get; }

        /// <inheritdoc />
        public List<Base> Children => Root.Children ?? new List<Base>();

        /// <inheritdoc />
        public TElement Create<TElement>(params object[] parameters) where TElement : Base
        {
            var fullParameterList = new List<object> {Root};
            fullParameterList.AddRange(parameters);

            var fullParameters = fullParameterList.ToArray();
            var constructor = typeof(TElement).FindConstructors(fullParameters).FirstOrDefault();

            if (constructor != null)
            {
                return constructor.Invoke(fullParameters) as TElement;
            }

            throw new ArgumentNullException(
                nameof(constructor), @"Failed to find constructor that matches parameters."
            );
        }

        /// <inheritdoc />
        public TElement Find<TElement>(string name = null, bool recurse = false) where TElement : Base
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                return Root.FindChildByName(name, recurse) as TElement;
            }

            return Root.Find(child => child is TElement) as TElement;
        }

        /// <inheritdoc />
        public IEnumerable<TElement> FindAll<TElement>(bool recurse = false) where TElement : Base =>
            Root.FindAll(child => child is TElement, recurse).Select(child => child as TElement);

        /// <inheritdoc />
        public void Remove<TElement>(TElement element, bool dispose = false) where TElement : Base =>
            Root.RemoveChild(element, dispose);

    }

}
