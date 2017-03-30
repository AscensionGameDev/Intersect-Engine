using System;
using Intersect;
using Intersect.GameObjects;
using Intersect.Server.Classes.Networking;

namespace Intersect.Server.Classes.General
{
    public static class ServerTime
    {
        private static DateTime _gameTime;
        private static long _updateTime;
        private static int _timeRange;

        public static void Init()
        {
            var timeBase = TimeBase.GetTimeBase();
            if (timeBase.SyncTime)
            {
                _gameTime = DateTime.Now;
            }
            else
            {
                _gameTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                    Globals.Rand.Next(0, 24), Globals.Rand.Next(0, 60), Globals.Rand.Next(0, 60));
            }
            _timeRange = -1;
            _updateTime = 0;
        }

        public static void Update()
        {
            var timeBase = TimeBase.GetTimeBase();
            if (Globals.System.GetTimeMs() > _updateTime)
            {
                if (!timeBase.SyncTime)
                {
                    _gameTime = _gameTime.Add(new TimeSpan(0, 0, 0, 0, (int) (1000 * timeBase.Rate)));
                        //Not sure if Rate is negative if time will go backwards but we can hope!
                }

                //Calculate what "timeRange" we should be in, if we're not then switch and notify the world
                //Gonna do this by minutes
                int minuteOfDay = _gameTime.Hour * 60 + _gameTime.Minute;
                int expectedRange = (int) Math.Floor(minuteOfDay / (float) timeBase.RangeInterval);

                if (expectedRange != _timeRange)
                {
                    _timeRange = expectedRange;
                    //Send the Update to everyone!
                    PacketSender.SendTimeToAll();
                }

                _updateTime = Globals.System.GetTimeMs() + 1000;
            }
        }

        public static Color GetTimeColor()
        {
            return TimeBase.GetTimeBase().RangeColors[_timeRange];
        }

        public static int GetTimeRange()
        {
            return _timeRange;
        }

        public static DateTime GetTime()
        {
            return _gameTime;
        }
    }
}