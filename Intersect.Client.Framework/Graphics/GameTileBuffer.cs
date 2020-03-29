namespace Intersect.Client.Framework.Graphics
{

    public abstract class GameTileBuffer
    {

        public static int TileBufferCount { get; set; } = 0;

        public abstract bool Supported { get; }

        public abstract GameTexture Texture { get; protected set; }

        public abstract bool AddTile(GameTexture texture, float x, float y, int srcX, int srcY, int srcW, int srcH);

        public abstract bool UpdateTile(GameTexture texture, float x, float y, int srcX, int srcY, int srcW, int srcH);

        public abstract bool SetData();

        public abstract void Dispose();

    }

}
