namespace Intersect.IO
{

    public interface ISaveable<in TTo> : ISaveable
    {

        bool Save(TTo to = default(TTo));

    }

}
