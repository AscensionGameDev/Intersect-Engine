using System;
using Intersect.GameObjects;
using Intersect.Server.Networking;

namespace Intersect.Server.General
{
    public static class ServerTime
    {
        private static DateTime sGameTime;
        private static long sUpdateTime;
        private static int sTimeRange;

        public static void Init()
        {
            var timeBase = TimeBase.GetTimeBase();
            if (timeBase.SyncTime)
            {
                sGameTime = DateTime.Now;
            }
            else
            {
                sGameTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                    Globals.Rand.Next(0, 24), Globals.Rand.Next(0, 60), Globals.Rand.Next(0, 60));
            }
            sTimeRange = -1;
            sUpdateTime = 0;
        }

        public static void Update()
        {
            var timeBase = TimeBase.GetTimeBase();
            if (Globals.Timing.TimeMs > sUpdateTime)
            {
                if (!timeBase.SyncTime)
                {
                    sGameTime = sGameTime.Add(new TimeSpan(0, 0, 0, 0, (int) (1000 * timeBase.Rate)));
                    //Not sure if Rate is negative if time will go backwards but we can hope!
                }
                else
                {
                    sGameTime = DateTime.Now;
                }

                //Calculate what "timeRange" we should be in, if we're not then switch and notify the world
                //Gonna do this by minutes
                int minuteOfDay = sGameTime.Hour * 60 + sGameTime.Minute;
                int expectedRange = (int) Math.Floor(minuteOfDay / (float) timeBase.RangeInterval);

                if (expectedRange != sTimeRange)
                {
                    sTimeRange = expectedRange;
                    //Send the Update to everyone!
                    PacketSender.SendTimeToAll();
                }

                sUpdateTime = Globals.Timing.TimeMs + 1000;
            }
        }

        public static Color GetTimeColor()
        {
            return TimeBase.GetTimeBase().DaylightHues[sTimeRange];
        }

        public static int GetTimeRange()
        {
            return sTimeRange;
        }

        public static DateTime GetTime()
        {
            return sGameTime;
        }
    }
}