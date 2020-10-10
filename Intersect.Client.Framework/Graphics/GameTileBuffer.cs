namespace Intersect.Client.Framework.Graphics
{
    public abstract class GameTileBuffer
    {
        public static int TileBufferCount { get; set; } = 0;

        public abstract bool Supported { get; }

        public abstract ITexture GameTexture { get; protected set; }

        public abstract bool AddTile(ITexture gameTexture, float x, float y, int srcX, int srcY, int srcW, int srcH);

        public abstract bool UpdateTile(ITexture gameTexture, float x, float y, int srcX, int srcY, int srcW, int srcH);

        public abstract bool SetData();

        public abstract void Dispose();
    }
}
