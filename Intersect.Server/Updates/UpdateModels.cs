using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Intersect.Server.Updates
{
    public class UpdateManifest
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("files")]
        public List Files { get; set; } = new List();

        [JsonProperty("generated")]
        public DateTime Generated { get; set; }
    }

    public class UpdateFile
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class UploadUpdateRequest
    {
        public string Version { get; set; }
        public Dictionary Files { get; set; } = new Dictionary();
    }
}
