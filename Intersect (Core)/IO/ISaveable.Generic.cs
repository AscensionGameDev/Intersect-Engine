using JetBrains.Annotations;

namespace Intersect.IO
{

    public interface ISaveable<in TTo> : ISaveable
    {

        bool Save([CanBeNull] TTo to = default(TTo));

    }

}
