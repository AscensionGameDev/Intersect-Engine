﻿namespace Intersect.Client.Framework.Graphics;


public abstract partial class GameTileBuffer
{

    public static int TileBufferCount { get; set; }

    public abstract bool Supported { get; }

    public abstract bool TryAddTile(GameTexture texture, int x, int y, int srcX, int srcY, int srcW, int srcH);

    public abstract bool TryUpdateTile(GameTexture texture, int x, int y, int srcX, int srcY, int srcW, int srcH);

    public abstract bool SetData();

    public abstract void Dispose();

}
