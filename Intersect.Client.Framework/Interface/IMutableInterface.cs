using System.Collections.Generic;

using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Interface
{

    public interface IMutableInterface
    {

        List<Base> Children { get; }

        TElement Create<TElement>(params object[] parameters) where TElement : Base;

        TElement Find<TElement>(string name = null, bool recurse = false) where TElement : Base;

        IEnumerable<TElement> FindAll<TElement>(bool recurse = false) where TElement : Base;

        void Remove<TElement>(TElement element, bool dispose = false) where TElement : Base;

    }

}
