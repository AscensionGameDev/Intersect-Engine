using System;

using Intersect.Utilities;

namespace Intersect.Client.General
{

    public static partial class Time
    {

        private static long sColorUpdate;

        private static ColorF sCurrentColor = ColorF.White;

        private static float sRate = 1f;

        private static DateTime sServerTime = DateTime.Now;

        private static Color sTargetColor = Color.Transparent;

        private static long sUpdateTime;

        public static void LoadTime(DateTime timeUpdate, Color clr, float rate)
        {
            sServerTime = timeUpdate;
            sTargetColor = clr;
            sUpdateTime = 0;
            sRate = rate;
        }

        public static void Update()
        {
            if (sUpdateTime < Timing.Global.Milliseconds)
            {
                var ts = new TimeSpan(0, 0, 0, 0, (int) (1000 * sRate));
                sServerTime = sServerTime.Add(ts);
                sUpdateTime = Timing.Global.Milliseconds + 1000;
            }

            float ecTime = Timing.Global.MillisecondsUtc - sColorUpdate;
            var valChange = 255 * ecTime / 10000f;
            sCurrentColor.A = LerpVal(sCurrentColor.A, sTargetColor.A, valChange);
            sCurrentColor.R = LerpVal(sCurrentColor.R, sTargetColor.R, valChange);
            sCurrentColor.G = LerpVal(sCurrentColor.G, sTargetColor.G, valChange);
            sCurrentColor.B = LerpVal(sCurrentColor.B, sTargetColor.B, valChange);

            sColorUpdate = Timing.Global.MillisecondsUtc;
        }

        private static float LerpVal(float val, float target, float amt)
        {
            if (val < target)
            {
                if (val + amt > target)
                {
                    val = target;
                }
                else
                {
                    val += amt;
                }
            }

            if (val > target)
            {
                if (val - amt < target)
                {
                    val = target;
                }
                else
                {
                    val -= amt;
                }
            }

            return val;
        }

        public static string GetTime()
        {
            return sServerTime.ToString("h:mm:ss tt");
        }

        public static ColorF GetTintColor()
        {
            return sCurrentColor;
        }

    }

}
