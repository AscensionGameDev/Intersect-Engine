using System;
using System.Collections.Generic;
using System.ComponentModel;
using Intersect.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intersect.GameObjects.Maps
{
    public class Attribute
    {
        public MapAttributes Type { get; set; }

        //Special Flags
        public AttributeItemFlags Item { get; set; } = new AttributeItemFlags();
        public AttributeZDimensionFlags ZDimension { get; set; } = new AttributeZDimensionFlags();
        public AttributeWarpFlags Warp { get; set; } = new AttributeWarpFlags();
        public AttributeSoundFlags  Sound { get; set; } = new AttributeSoundFlags();
        public AttributeResourceFlags Resource { get; set; } = new AttributeResourceFlags();
        public AttributeAnimationFlags Animation { get; set; } = new AttributeAnimationFlags();
        public AttributeSlideFlags Slide { get; set; } = new AttributeSlideFlags();

        public Attribute()
        {
            
        }

        public Attribute(string json)
        {
            Load(json);
        }

        public string Data()
        {
            var serializationSettings = new JsonSerializerSettings();
            serializationSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
            serializationSettings.NullValueHandling = NullValueHandling.Ignore;

            //Not sure how to clear out the empty stuff
            var jObject = JObject.FromObject(this, JsonSerializer.Create(serializationSettings));
            
            var keystoRemove = new List<string>();
            foreach (var key in jObject)
            {
                if (key.Value.Type == JTokenType.Object && !key.Value.HasValues)
                {
                    keystoRemove.Add(key.Key);
                }
            }
            foreach (var key in keystoRemove)
                jObject.Remove(key);

            return JsonConvert.SerializeObject(jObject, serializationSettings);
        }

        public void Load(string json)
        {
            JsonConvert.PopulateObject(json, this);
        }
    }

    public class AttributeItemFlags
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class AttributeZDimensionFlags
    {
        public int GatewayTo { get; set; }
        public int BlockedLevel { get; set; }
    }

    public class AttributeWarpFlags
    {
        public Guid MapId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public byte Dir { get; set; }
    }

    public class AttributeSoundFlags
    {
        public string File { get; set; }
        public int Distance { get; set; }
    }

    public class AttributeResourceFlags
    {
        public Guid ResourceId { get; set; }
        public int SpawnLevel { get; set; }
    }

    public class AttributeAnimationFlags
    {
        public Guid AnimationId { get; set; }
    }

    public class AttributeSlideFlags
    {
        public byte Direction { get; set; }
    }
}