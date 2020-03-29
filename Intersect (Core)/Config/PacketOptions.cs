using Newtonsoft.Json;

namespace Intersect.Config
{

    public class PacketOptions
    {

        [JsonProperty("EditorFloodThreshholds")]
        public FloodThreshholds EditorThreshholds = FloodThreshholds.Editor();

        [JsonProperty("PlayerFloodThreshholds")]
        public FloodThreshholds PlayerThreshholds = new FloodThreshholds();

        [JsonProperty("FloodThreshholds")] public FloodThreshholds Threshholds = FloodThreshholds.NotLoggedIn();

    }

    public class FloodThreshholds
    {

        public int MaxPacketSize { get; set; } = 10240;

        public int MaxPacketPerSec { get; set; } = 30;

        public int KickAvgPacketPerSec { get; set; } = 20;

        public static FloodThreshholds Editor()
        {
            return new FloodThreshholds()
            {
                MaxPacketSize = int.MaxValue,
                MaxPacketPerSec = int.MaxValue,
                KickAvgPacketPerSec = int.MaxValue
            };
        }

        public static FloodThreshholds NotLoggedIn()
        {
            return new FloodThreshholds()
            {
                MaxPacketSize = 10240,
                MaxPacketPerSec = 5,
                KickAvgPacketPerSec = 3,
            };
        }

    }

}
