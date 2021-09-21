namespace Intersect.Client.Framework.Core.Sounds
{
    public interface ISound
    {
        bool Loaded { get; set; }
        bool Loop { get; set; }

        void Stop();
        bool Update();
    }
}