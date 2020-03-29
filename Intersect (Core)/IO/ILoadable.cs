using JetBrains.Annotations;

namespace Intersect.IO
{

    public interface ILoadable<in TFrom>
    {

        bool Load([CanBeNull] TFrom from = default(TFrom));

    }

}
