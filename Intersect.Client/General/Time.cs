using System;

namespace Intersect.Client.General
{

    public static class Time
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
            if (sUpdateTime < Globals.System.GetTimeMs())
            {
                var ts = new TimeSpan(0, 0, 0, 0, (int) (1000 * sRate));
                sServerTime = sServerTime.Add(ts);
                sUpdateTime = Globals.System.GetTimeMs() + 1000;
            }

            float ecTime = Globals.System.GetTimeMs() - sColorUpdate;
            var valChange = 255 * ecTime / 10000f;
            sCurrentColor.A = LerpVal(sCurrentColor.A, sTargetColor.A, valChange);
            sCurrentColor.R = LerpVal(sCurrentColor.R, sTargetColor.R, valChange);
            sCurrentColor.G = LerpVal(sCurrentColor.G, sTargetColor.G, valChange);
            sCurrentColor.B = LerpVal(sCurrentColor.B, sTargetColor.B, valChange);

            sColorUpdate = Globals.System.GetTimeMs();
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
