using Intersect.Core;
using Intersect.Framework.Core;
using Intersect.GameObjects;
using Intersect.Server.Networking;
using Intersect.Utilities;
using Microsoft.Extensions.Logging;

namespace Intersect.Server.General;


public static partial class Time
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
        var timeBase = DaylightCycleDescriptor.Instance;
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

        sTimeRange = 0;
        sUpdateTime = 0;
    }

    public static void Update()
    {
        var timeBase = DaylightCycleDescriptor.Instance;
        if (Timing.Global.Milliseconds > sUpdateTime)
        {
            if (timeBase.SyncTime)
            {
                sGameTime = DateTime.Now;
            }
            else
            {
                var timeWas = sGameTime;
                var timeRate = timeBase.Rate;
                var addedTime = new TimeSpan(0, 0, 0, 0, (int)(1000 * timeRate));

                // Not sure if Rate is negative if time will go backwards but we can hope!
                try
                {
                    sGameTime = sGameTime.Add(addedTime);
                }
                catch (ArgumentOutOfRangeException exception)
                {
                    // Log the error with the value of timeBase.Rate and pass the exception
                    ApplicationContext.Context.Value?.Logger.LogError(exception, $"Failed to update game time. Time was {timeWas}, Added Rate was {timeRate}, Added time span was {addedTime}");
                    // Rethrow the exception to crash the server o_o !!!
                    throw;
                }
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

            sUpdateTime = Timing.Global.Milliseconds + 1000;
        }
    }

    public static Color GetTimeColor()
    {
        var time = DaylightCycleDescriptor.Instance;
        return time.DaylightHues[sTimeRange];
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
