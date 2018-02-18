using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Intersect.Config
{
    public class ClientOptions
    {
        private static ClientOptions _options;

        //Public Getters
        public static string Language => _options._language;
        public static string ServerHost => _options._host;
        public static ushort ServerPort => _options._port;
        public static bool RenderCache => _options._renderCache;
        public static string Font => _options._font;
		public static int ChatLines => _options._chatLines;
		public static string MenuMusic => _options._menuMusic;
        public static string MenuBackground => _options._menuBackground;
        public static List<string> IntroImages => _options._introImages;

        [JsonProperty("Language")]
        protected string _language = "English";

        [JsonProperty("Host")]
        protected string _host = "localhost";

        [JsonProperty("Port")]
        protected ushort _port = 5400;

        [JsonProperty("RenderCache")]
        protected bool _renderCache = true;

        [JsonProperty("Font")]
        protected string _font = "arial";

		[JsonProperty("ChatLines")]
		protected int _chatLines = 100;

		[JsonProperty("MenuMusic")]
        protected string _menuMusic = "";

        [JsonProperty("MenuBackground")]
        protected string _menuBackground = "background.png";

        [JsonProperty("IntroImages")]
        protected List<string> _introImages = new List<string>();

        public static void Load(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                _options = new ClientOptions();
            }
            else
            {
                _options = JsonConvert.DeserializeObject<ClientOptions>(json);
            }
        }

        public static string GetJson()
        {
            return JsonConvert.SerializeObject(_options, Formatting.Indented);
        }


        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            _introImages.Clear();
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            Validate();
        }

        public void Validate()
        {
            _introImages = new List<string>(_introImages.Distinct());
        }

    }
}
