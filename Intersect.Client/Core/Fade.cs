using Intersect.Client.Networking;
using Intersect.Configuration;
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

        private static float sFadeRate = ClientConfiguration.DEFAULT_FADE_SPEED_MS;

        private static long sLastUpdate;

        private static bool InformServer { get; set; }

        public static void FadeIn(float speed, bool informServer = false)
        {
            sFadeRate = speed;
            sCurrentAction = FadeType.In;
            sFadeAmt = 255f;
            sLastUpdate = Timing.Global.MillisecondsUtc;
            InformServer = informServer;
        }

        public static void FadeOut(float speed, bool informServer = false)
        {
            sFadeRate = speed;
            sCurrentAction = FadeType.Out;
            sFadeAmt = 0f;
            sLastUpdate = Timing.Global.MillisecondsUtc;
            InformServer = informServer;
        }

        public static void Cancel(bool informServer = false)
        {
            sCurrentAction = FadeType.None;
            sFadeAmt = default;
            if (informServer)
            {
                InformServerOfCompletion();
            }
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
                        InformServerOfCompletion();
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
                        InformServerOfCompletion();
                    }
                }
            }

            sLastUpdate = Timing.Global.MillisecondsUtc;
        }

        private static void InformServerOfCompletion()
        {
            PacketSender.SendFadeCompletePacket();
            InformServer = false;
        }
    }

}
