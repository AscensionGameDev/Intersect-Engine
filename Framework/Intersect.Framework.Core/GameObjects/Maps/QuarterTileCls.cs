namespace Intersect.GameObjects.Maps;

public partial class QuarterTileCls
{
    public PointStruct[] QuarterTile = new PointStruct[5];

    public byte RenderState;

    public QuarterTileCls Copy()
    {
        var autotile = new QuarterTileCls();

        autotile.RenderState = RenderState;

        for (var z = 0; z < 5; z++)
        {
            autotile.QuarterTile[z] = new PointStruct()
            {
                X = QuarterTile[z].X,
                Y = QuarterTile[z].Y
            };
        }

        return autotile;
    }

    public bool Equals(QuarterTileCls quarterTile)
    {
        if (quarterTile.RenderState != RenderState)
        {
            return false;
        }

        for (var z = 0; z < 5; z++)
        {
            if (quarterTile.QuarterTile[z].X != QuarterTile[z].X)
            {
                return false;
            }

            if (quarterTile.QuarterTile[z].Y != QuarterTile[z].Y)
            {
                return false;
            }
        }

        return true;
    }
}