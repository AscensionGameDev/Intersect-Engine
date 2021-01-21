namespace Intersect.IO
{

    public interface ISaveableProvider
    {

        ISaveable DefaultSaveable { get; }

        ISaveable<TTo> AsSaveable<TTo>();

    }

}
