using JetBrains.Annotations;

namespace Intersect.IO
{

    public interface ISaveableProvider
    {

        [NotNull]
        ISaveable DefaultSaveable { get; }

        [CanBeNull]
        ISaveable<TTo> AsSaveable<TTo>();

    }

}
