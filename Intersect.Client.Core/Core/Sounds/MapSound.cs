using Intersect.Client.Framework.Core.Sounds;
using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.General;
using Intersect.Client.Maps;
using Intersect.Config;

namespace Intersect.Client.Core.Sounds;

public partial class MapSound : Sound, IMapSound
{
    private readonly int mDistance;

    private readonly IEntity? mEntity;

    private Guid mMapId;

    private int mX;

    private int mY;

    private readonly MapOptions _mapOptions = Options.Instance.Map;
    private readonly int _tileWidth;
    private readonly int _tileHeight;
    private readonly int _mapPixelWidth;
    private readonly int _mapPixelHeight;

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
        mSound.Volume = 0;
        _tileWidth = _mapOptions.TileWidth;
        _tileHeight = _mapOptions.TileHeight;
        _mapPixelWidth = _mapOptions.MapWidth * _tileWidth;
        _mapPixelHeight = _mapOptions.MapHeight * _tileHeight;
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
            if (mSound is { } sound)
            {
                sound.Volume = 0;
            }
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
            if (mSound is { } sound)
            {
                sound.Volume = 100;
            }
        }
        else
        {
            var volume = 0f;
            if (mDistance > 0 && Globals.GridMaps.ContainsKey(mMapId))
            {
                volume = 100 - 100f * CalculateSoundDistance() / (mDistance + 1);
                if (volume < 0)
                {
                    volume = 0f;
                }
            }

            if (mSound is { } sound)
            {
                sound.Volume = (int)volume;
            }
        }
    }

    private float CalculateSoundDistance()
    {
        if (Globals.Me is not {} player)
        {
            return 0f;
        }
        var soundMapId = mMapId;
        if (!MapInstance.TryGet(soundMapId, out var soundMap))
        {
            return 0f;
        }

        MapInstance? playerMap;
        var playerMapId = player.MapId;
        if (playerMapId == soundMapId)
        {
            playerMap = soundMap;
        }
        else if (!MapInstance.TryGet(playerMapId, out playerMap))
        {
            return 0f;
        }

        float distance;

        Point playerPosition = new(
            playerMap.X + player.X * _tileWidth + (_tileWidth / 2),
            playerMap.Y + player.Y * _tileHeight + (_tileHeight / 2)
        );

        if (mX == -1 || mY == -1 || mDistance == -1)
        {
            var mapRect = new Rectangle(
                soundMap.X,
                soundMap.Y,
                _mapPixelWidth,
                _mapPixelHeight
            );

            distance = DistancePointToRectangle(playerPosition, mapRect) / ((_tileHeight + _tileWidth) / 2f);
        }
        else
        {
            Point soundPosition = new(
                soundMap.X + mX * _tileWidth + (_tileWidth / 2),
                soundMap.Y + mY * _tileHeight + (_tileHeight / 2)
            );

            var delta = playerPosition - soundPosition;

            distance = (float)Math.Sqrt(Math.Pow(delta.X, 2) + Math.Pow(delta.Y, 2)) /
                       ((_tileHeight + _tileWidth) / 2f);
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
