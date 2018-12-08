using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Intersect.Config
{
    public class ClientOptions
    {
        [NotNull]
        private static ClientOptions sOptions;

        static ClientOptions() => sOptions = new ClientOptions();

        //Public Getters
        public static string ServerHost => sOptions.mHost;
        public static ushort ServerPort => sOptions.mPort;
        public static string Font => sOptions.mFont;
        public static int ChatLines => sOptions.mChatLines;
        public static string MenuMusic => sOptions.mMenuMusic;
        public static string MenuBackground => sOptions.mMenuBackground;
        public static List<string> IntroImages => sOptions.mIntroImages;

        [JsonProperty("Host")]
        protected string mHost = "localhost";

        [JsonProperty("Port")]
        protected ushort mPort = 5400;

        [JsonProperty("Font")]
        protected string mFont = "arial";

        [JsonProperty("ChatLines")]
        protected int mChatLines = 100;

        [JsonProperty("MenuMusic")]
        protected string mMenuMusic = "";

        [JsonProperty("MenuBackground")]
        protected string mMenuBackground = "background.png";

        [JsonProperty("IntroImages")]
        protected List<string> mIntroImages = new List<string>();

        public static void LoadFrom(string json)
        {
            sOptions = (string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<ClientOptions>(json)) ?? sOptions;
        }

        public static string ToJson()
        {
            return JsonConvert.SerializeObject(sOptions, Formatting.Indented);
        }
        
        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            mIntroImages?.Clear();
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            Validate();
        }

        public void Validate()
        {
            mIntroImages = new List<string>(mIntroImages?.Distinct() ?? new List<string>());
        }

    }
}
