using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Config
{
    public class LayerOptions
    {
        public const string Attributes = nameof(Attributes);
        public const string Npcs = nameof(Npcs);
        public const string Lights = nameof(Lights);
        public const string Events = nameof(Events);

        [JsonProperty]
        public List<string> LowerLayers { get; private set; } = new List<string>() { "Ground", "Mask 1", "Mask 2" };

        [JsonProperty]
        public List<string> MiddleLayers { get; private set; } = new List<string>() { "Fringe 1" };

        [JsonProperty]
        public List<string> UpperLayers { get; private set; } = new List<string>() { "Fringe 2" };

        [JsonIgnore]
        public List<string> All { get; private set; } = new List<string>();

        [JsonProperty]
        public bool DestroyOrphanedLayers { get; private set; } = false;

        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            All.Clear();
            LowerLayers.Clear();
            MiddleLayers.Clear();
            UpperLayers.Clear();
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            Validate();
        }

        public void Validate()
        {
            LowerLayers = new List<string>(LowerLayers.Distinct());
            MiddleLayers = new List<string>(MiddleLayers.Distinct());
            UpperLayers = new List<string>(UpperLayers.Distinct());

            var reservedLayers = new string[] { Attributes, Npcs, Lights, Events };
            All.Clear();
            All.AddRange(LowerLayers);
            All.AddRange(MiddleLayers);
            All.AddRange(UpperLayers);

            if (All.Count() == 0)
            {
                //Must have at least 1 map layer!
                throw new Exception("Config Error: You must have at least 1 map layer configured! Please update your server config.");
            }

            foreach (var reserved in reservedLayers)
            {
                if (All.Contains(reserved))
                {
                    throw new Exception($"Config Error: Layer '{reserved}' is reserved for editor use. Please choose different naming for map layers in your server config.");
                }
            }

            if (All.Count != All.Distinct().Count())
            {
                //Duplicate layers!
                throw new Exception("Config Error: Duplicate map layers detected! Map layers must be unique in name. Please update your server config.");
            }
        }
    }
}
