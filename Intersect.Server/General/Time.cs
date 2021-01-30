using System;

using Intersect.GameObjects;
using Intersect.Server.Networking;
using Intersect.Utilities;

namespace Intersect.Server.General
{

    public static class Time
    {

        private static DateTime sGameTime;

        private static int sTimeRange;

        private static long sUpdateTime;

        public static string Hour = "00";
        public static string MilitaryHour = "00";
        public static string Minute = "00";
        public static string Second = "00";


        public static void Init()
        {
            var timeBase = TimeBase.GetTimeBase();
            if (timeBase.SyncTime)
            {
                sGameTime = DateTime.Now;
            }
            else
            {
                sGameTime = new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day,
                    Randomization.Next(0, 24),
                    Randomization.Next(0, 60),
                    Randomization.Next(0, 60)
                );
            }

            sTimeRange = -1;
            sUpdateTime = 0;
        }

        public static void Update()
        {
            var timeBase = TimeBase.GetTimeBase();
            if (Globals.Timing.Milliseconds > sUpdateTime)
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
                var minuteOfDay = sGameTime.Hour * 60 + sGameTime.Minute;
                var expectedRange = (int) Math.Floor(minuteOfDay / (float) timeBase.RangeInterval);

                if (expectedRange != sTimeRange)
                {
                    sTimeRange = expectedRange;

                    //Send the Update to everyone!
                    PacketSender.SendTimeToAll();
                }

                Hour = sGameTime.ToString("%h");
                MilitaryHour = sGameTime.ToString("HH");
                Minute = sGameTime.ToString("mm");
                Second = sGameTime.ToString("ss");

                sUpdateTime = Globals.Timing.Milliseconds + 1000;
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
