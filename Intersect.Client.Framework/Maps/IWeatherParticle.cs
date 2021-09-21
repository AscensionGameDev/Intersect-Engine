namespace Intersect.Client.Framework.Maps
{
    public interface IWeatherParticle
    {
        float X { get; set; }
        float Y { get; set; }

        void Dispose();
        void Update();
    }
}