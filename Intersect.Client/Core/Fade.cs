using Intersect.Client.Networking;
using Intersect.Utilities;

namespace Intersect.Client.Core
{

    public static partial class Fade
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

        private static bool InformServer { get; set; }

        public static void FadeIn(bool informServer = false)
        {
            sCurrentAction = FadeType.In;
            sFadeAmt = 255f;
            sLastUpdate = Timing.Global.MillisecondsUtc;
            InformServer = informServer;
        }

        public static void FadeOut(bool informServer = false)
        {
            sCurrentAction = FadeType.Out;
            sFadeAmt = 0f;
            sLastUpdate = Timing.Global.MillisecondsUtc;
            InformServer = informServer;
        }

        public static void Cancel()
        {
            sCurrentAction = FadeType.None;
            sFadeAmt = default;
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
                sFadeAmt -= (Timing.Global.MillisecondsUtc - sLastUpdate) / sFadeRate * 255f;
                if (sFadeAmt <= 0f)
                {
                    sCurrentAction = FadeType.None;
                    sFadeAmt = 0f;

                    if (InformServer)
                    {
                        PacketSender.SendFadeCompletePacket();
                    }
                }
            }
            else if (sCurrentAction == FadeType.Out)
            {
                sFadeAmt += (Timing.Global.MillisecondsUtc - sLastUpdate) / sFadeRate * 255f;
                if (sFadeAmt >= 255f)
                {
                    sCurrentAction = FadeType.None;
                    sFadeAmt = 255f;

                    if (InformServer)
                    {
                        PacketSender.SendFadeCompletePacket();
                    }
                }
            }

            sLastUpdate = Timing.Global.MillisecondsUtc;
        }

    }

}
