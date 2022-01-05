using System;
using Intersect.Client.Framework.Core.Sounds;
using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.General;
using Intersect.Client.Maps;

namespace Intersect.Client.Core.Sounds
{

    public class MapSound : Sound, IMapSound
    {

        private int mDistance;

        private IEntity mEntity;

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
            IEntity parent = null
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
                mSound.SetVolume(0);

                return;
            }

            var map = MapInstance.Get(mMapId);
            if (map == null && mEntity != Globals.Me || Globals.Me == null)
            {
                Stop();

                return;
            }

            var sameMap = mMapId == Globals.Me.MapId;
            var inGrid = sameMap;
            if (!inGrid && Globals.Me.MapInstance != null)
            {
                var gridX = Globals.Me.MapInstance.GridX;
                var gridY = Globals.Me.MapInstance.GridY;
                for (var x = gridX - 1; x <= gridX + 1; x++)
                {
                    for (var y = gridY - 1; y <= gridY + 1; y++)
                    {
                        if (x >= 0 &&
                            x < Globals.MapGridWidth &&
                            y >= 0 &&
                            y < Globals.MapGridHeight &&
                            Globals.MapGrid[x, y] != Guid.Empty)
                        {
                            if (Globals.MapGrid[x, y] == mMapId)
                            {
                                inGrid = true;

                                break;
                            }
                        }
                    }
                }
            }

            if ((mX == -1 || mY == -1 || mDistance <= 0) && sameMap)
            {
                mSound.SetVolume(100);
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

                    mSound.SetVolume((int)volume);
                }
                else
                {
                    mSound.SetVolume(0);
                }
            }
        }

        private float CalculateSoundDistance()
        {
            var distance = 0f;
            var playerx = 0f;
            var playery = 0f;
            float soundx = 0;
            float soundy = 0;
            var map = MapInstance.Get(mMapId);
            var pMap = MapInstance.Get(Globals.Me.MapId);
            if (map != null && pMap != null)
            {
                playerx = pMap.GetX() + Globals.Me.X * Options.TileWidth + 16;
                playery = pMap.GetY() + Globals.Me.Y * Options.TileHeight + 16;
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

                    distance = (float)DistancePointToRectangle(player, mapRect) / 32f;
                }
                else
                {
                    soundx = map.GetX() + mX * Options.TileWidth + 16;
                    soundy = map.GetY() + mY * Options.TileHeight + 16;
                    distance = (float)Math.Sqrt(Math.Pow(playerx - soundx, 2) + Math.Pow(playery - soundy, 2)) / 32f;
                }
            }

            return distance;
        }

        private static float DistancePointToRectangle(Point p, Rectangle r)
        {
            var distance = 0f;
            var x = p.X;
            var y = p.Y;
            var left = r.X;
            var top = r.Y;
            var right = r.X + r.Width;
            var bottom = r.Y + r.Height;
            if (x < left)
            {
                distance += left - x;
            }
            else if (x > right)
            {
                distance += x - right;
            }

            if (y < top)
            {
                distance += top - y;
            }
            else if (y > bottom)
            {
                distance += y - bottom;
            }

            return distance;
        }
    }
}
