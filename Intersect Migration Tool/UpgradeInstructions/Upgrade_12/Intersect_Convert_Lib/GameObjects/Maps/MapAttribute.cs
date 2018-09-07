using System;
using System.Collections.Generic;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Maps
{
    public abstract class MapAttribute
    {
        public abstract MapAttributes Type { get; }

        public MapAttribute()
        {

        }

        public static MapAttribute CreateAttribute(MapAttributes type)
        {
            switch (type)
            {
                case MapAttributes.Walkable:
                    return null;
                case MapAttributes.Blocked:
                    return new MapBlockedAttribute();
                case MapAttributes.Item:
                    return new MapItemAttribute();
                case MapAttributes.ZDimension:
                    return new MapZDimensionAttribute();
                case MapAttributes.NpcAvoid:
                    return new MapNpcAvoidAttribute();
                case MapAttributes.Warp:
                    return new MapWarpAttribute();
                case MapAttributes.Sound:
                    return new MapSoundAttribute();
                case MapAttributes.Resource:
                    return new MapResourceAttribute();
                case MapAttributes.Animation:
                    return new MapAnimationAttribute();
                case MapAttributes.GrappleStone:
                    return new MapGrappleStoneAttribute();
                case MapAttributes.Slide:
                    return new MapSlideAttribute();
            }
            return null;
        }

        /// <summary>
        /// Perform a deep Copy of the object, using Json as a serialisation method. NOTE: Private members are not cloned using this method.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T CloneJson<T>(T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }
    }

    public class MapBlockedAttribute : MapAttribute
    {
        public override MapAttributes Type { get; } = MapAttributes.Blocked;
    }

    public class MapItemAttribute : MapAttribute
    {
        public override MapAttributes Type { get; } = MapAttributes.Item;
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class MapZDimensionAttribute : MapAttribute
    {
        public override MapAttributes Type { get; } = MapAttributes.ZDimension;
        public int GatewayTo { get; set; }
        public int BlockedLevel { get; set; }
    }

    public class MapNpcAvoidAttribute : MapAttribute
    {
        public override MapAttributes Type { get; } = MapAttributes.NpcAvoid;
    }

    public class MapWarpAttribute : MapAttribute
    {
        public override MapAttributes Type { get; } = MapAttributes.Warp;
        public Guid MapId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public WarpDirection Direction { get; set; } = WarpDirection.Retain;
    }

    public class MapSoundAttribute : MapAttribute
    {
        public override MapAttributes Type { get; } = MapAttributes.Sound;
        public string File { get; set; }
        public int Distance { get; set; }
    }

    public class MapResourceAttribute : MapAttribute
    {
        public override MapAttributes Type { get; } = MapAttributes.Resource;
        public Guid ResourceId { get; set; }
        public int SpawnLevel { get; set; }
    }

    public class MapAnimationAttribute : MapAttribute
    {
        public override MapAttributes Type { get; } = MapAttributes.Animation;
        public Guid AnimationId { get; set; }
    }

    public class MapGrappleStoneAttribute : MapAttribute
    {
        public override MapAttributes Type { get; } = MapAttributes.GrappleStone;
    }

    public class MapSlideAttribute : MapAttribute
    {
        public override MapAttributes Type { get; } = MapAttributes.Slide;
        public byte Direction { get; set; }
    }
}