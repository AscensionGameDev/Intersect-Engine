using Intersect.Client.General;

namespace Intersect.Client.Core
{

    public static class Fade
    {

        public enum FadeType
        {

            None = 0,

            In = 1,

            Out = 2,

        }

        private static FadeType sCurrentAction;

        private static float sFadeAmt;

        private static float sFadeRate = 3000f;

        private static long sLastUpdate;

        public static void FadeIn()
        {
            sCurrentAction = FadeType.In;
            sFadeAmt = 255f;
            sLastUpdate = Globals.System.GetTimeMs();
        }

        public static void FadeOut()
        {
            sCurrentAction = FadeType.Out;
            sFadeAmt = 0f;
            sLastUpdate = Globals.System.GetTimeMs();
        }

        public static bool DoneFading()
        {
            return sCurrentAction == FadeType.None;
        }

        public static float GetFade()
        {
            return sFadeAmt;
        }

        public static void Update()
        {
            if (sCurrentAction == FadeType.In)
            {
                sFadeAmt -= (Globals.System.GetTimeMs() - sLastUpdate) / sFadeRate * 255f;
                if (sFadeAmt <= 0f)
                {
                    sCurrentAction = FadeType.None;
                    sFadeAmt = 0f;
                }
            }
            else if (sCurrentAction == FadeType.Out)
            {
                sFadeAmt += (Globals.System.GetTimeMs() - sLastUpdate) / sFadeRate * 255f;
                if (sFadeAmt >= 255f)
                {
                    sCurrentAction = FadeType.None;
                    sFadeAmt = 255f;
                }
            }

            sLastUpdate = Globals.System.GetTimeMs();
        }

    }

}
