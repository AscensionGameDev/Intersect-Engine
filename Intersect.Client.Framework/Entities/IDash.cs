namespace Intersect.Client.Framework.Entities
{
    public interface IDash
    {
        float GetXOffset();
        float GetYOffset();
        void Start(IEntity en);
        bool Update(IEntity en);
    }
}