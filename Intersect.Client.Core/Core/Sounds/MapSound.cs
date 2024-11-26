using Intersect.Client.Framework.Core.Sounds;
using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.General;
using Intersect.Client.Maps;

namespace Intersect.Client.Core.Sounds;

public partial class MapSound : Sound, IMapSound
{
    private readonly int mDistance;

    private readonly IEntity? mEntity;

    private Guid mMapId;

    private int mX;

    private int mY;

    public MapSound(
        string filename,
        int x,
        int y,
        Guid mapId,
        bool loop,
        int loopInterval,
        int distance,
        IEntity? parent = null
    ) : base(filename, loop, loopInterval)
    {
        if (string.IsNullOrEmpty(filename) || mSound == null)
        {
            return;
        }

        mDistance = distance;
        mX = x;
        mY = y;
        mMapId = mapId;
        mEntity = parent;
        mSound.SetVolume(0);
    }

    public void UpdatePosition(int x, int y, Guid mapId)
    {
        mX = x;
        mY = y;
        mMapId = mapId;
    }

    public override bool Update()
    {
        if (base.Update())
        {
            UpdateSoundVolume();

            return true;
        }

        return false;
    }

    private void UpdateSoundVolume()
    {
        if (mMapId == Guid.Empty)
        {
            mSound?.SetVolume(0);
            return;
        }

        var map = MapInstance.Get(mMapId);
        if (map == null && mEntity != Globals.Me || Globals.Me == null || Globals.MapGrid == default)
        {
            Stop();
            return;
        }

        if (mX == -1 || mY == -1 || mDistance <= 0)
        {
            mSound?.SetVolume(100);
        }
        else
        {
            if (mDistance > 0 && Globals.GridMaps.Contains(mMapId))
            {
                var volume = 100 - 100 / (mDistance + 1) * CalculateSoundDistance();
                if (volume < 0)
                {
                    volume = 0f;
                }

                mSound?.SetVolume((int)volume);
            }
            else
            {
                mSound?.SetVolume(0);
            }
        }
    }

    private float CalculateSoundDistance()
    {
        if (Globals.Me == null)
        {
            return 0f;
        }

        var distance = 0f;
        var map = MapInstance.Get(mMapId);
        var pMap = MapInstance.Get(Globals.Me.MapId);
        if (map != null && pMap != null)
        {
            var playerx = pMap.GetX() + Globals.Me.X * Options.TileWidth + (Options.TileWidth / 2);
            var playery = pMap.GetY() + Globals.Me.Y * Options.TileHeight + (Options.TileHeight / 2);
            if (mX == -1 || mY == -1 || mDistance == -1)
            {
                var player = new Point() {
                    X = (int)playerx,
                    Y = (int)playery
                };

                var mapRect = new Rectangle(
                    (int)map.GetX(), (int)map.GetY(), Options.MapWidth * Options.TileWidth,
                    Options.MapHeight * Options.TileHeight
                );

                distance = DistancePointToRectangle(player, mapRect) /
                           ((Options.TileHeight + Options.TileWidth) / 2f);
            }
            else
            {
                var soundx = map.GetX() + mX * Options.TileWidth + (Options.TileWidth / 2);
                var soundy = map.GetY() + mY * Options.TileHeight + (Options.TileHeight / 2);
                distance = (float) Math.Sqrt(Math.Pow(playerx - soundx, 2) + Math.Pow(playery - soundy, 2)) /
                           ((Options.TileHeight + Options.TileWidth) / 2f);
            }
        }

        return distance;
    }

    //Code Courtesy of  Philip Peterson. -- Released under MIT license.
    //Obtained, 06/27/2015 from http://wiki.unity3d.com/index.php/Distance_from_a_point_to_a_rectangle
    public static float DistancePointToRectangle(Point point, Rectangle rect)
    {
        //  Calculate a distance between a point and a rectangle.
        //  The area around/in the rectangle is defined in terms of
        //  several regions:
        //
        //  O--x
        //  |
        //  y
        //
        //
        //        I   |    II    |  III
        //      ======+==========+======   --yMin
        //       VIII |  IX (in) |  IV
        //      ======+==========+======   --yMax
        //       VII  |    VI    |   V
        //
        //
        //  Note that the +y direction is down because of Unity's GUI coordinates.

        if (point.X < rect.X)
        {
            // Region I, VIII, or VII
            if (point.Y < rect.Y)
            {
                // I
                point.X -= rect.X;
                point.Y -= rect.Y;

                return (float)Math.Sqrt(point.X * point.X + point.Y * point.Y);
            }
            else if (point.Y > rect.Y + rect.Height)
            {
                // VII
                point.X -= rect.X;
                point.Y -= (rect.Y + rect.Height);

                return (float)Math.Sqrt(point.X * point.X + point.Y * point.Y);
            }
            else
            {
                // VIII
                return rect.X - point.X;
            }
        }
        else if (point.X > rect.X + rect.Width)
        {
            // Region III, IV, or V
            if (point.Y < rect.Y)
            {
                // III
                point.X -= (rect.X + rect.Width);
                point.Y -= rect.Y;

                return (float)Math.Sqrt(point.X * point.X + point.Y * point.Y);
            }
            else if (point.Y > rect.Y + rect.Height)
            {
                // V
                point.X -= (rect.X + rect.Width);
                point.Y -= (rect.Y + rect.Height);

                return (float)Math.Sqrt(point.X * point.X + point.Y * point.Y);
            }
            else
            {
                // IV
                return point.X - (rect.X + rect.Width);
            }
        }
        else
        {
            // Region II, IX, or VI
            if (point.Y < rect.Y)
            {
                // II
                return rect.Y - point.Y;
            }
            else if (point.Y > rect.Y + rect.Height)
            {
                // VI
                return point.Y - (rect.Y + rect.Height);
            }
            else
            {
                // IX
                return 0f;
            }
        }
    }
}
