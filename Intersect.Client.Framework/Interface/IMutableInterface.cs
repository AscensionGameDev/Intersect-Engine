using System.Collections.Generic;

using Intersect.Client.Framework.Gwen.Control;

using JetBrains.Annotations;

namespace Intersect.Client.Interface
{

    public interface IMutableInterface
    {

        [NotNull] List<Base> Children { get; }

        TElement Create<TElement>([NotNull] params object[] parameters) where TElement : Base;

        TElement Find<TElement>([CanBeNull] string name = null, bool recurse = false) where TElement : Base;

        IEnumerable<TElement> FindAll<TElement>(bool recurse = false) where TElement : Base;

        void Remove<TElement>([NotNull] TElement element, bool dispose = false) where TElement : Base;

    }

}
