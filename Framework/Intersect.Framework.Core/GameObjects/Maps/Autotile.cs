namespace Intersect.Framework.Core.GameObjects.Maps;

public partial class Autotile
{
    public Point16i[] QuarterTile = new Point16i[5];

    public byte RenderState;

    public Autotile Copy()
    {
        var autotile = new Autotile();

        autotile.RenderState = RenderState;

        for (var z = 0; z < 5; z++)
        {
            autotile.QuarterTile[z] = new Point16i()
            {
                X = QuarterTile[z].X,
                Y = QuarterTile[z].Y
            };
        }

        return autotile;
    }

    public bool Equals(Autotile quarterTile)
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