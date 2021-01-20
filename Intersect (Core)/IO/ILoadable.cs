namespace Intersect.IO
{

    public interface ILoadable<in TFrom>
    {

        bool Load(TFrom from = default(TFrom));

    }

}
