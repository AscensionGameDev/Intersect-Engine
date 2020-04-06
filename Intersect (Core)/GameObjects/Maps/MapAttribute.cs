﻿using System;

using Intersect.Enums;

using Newtonsoft.Json;

namespace Intersect.GameObjects.Maps
{

    public abstract class MapAttribute
    {
        public abstract MapAttributes Type { get; }

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

        public virtual MapAttribute Clone()
        {
            return CreateAttribute(this.Type);
        }

        public string Data()
        {
            return JsonConvert.SerializeObject(this);
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

        public override MapAttribute Clone()
        {
            var att = (MapItemAttribute) base.Clone();
            att.ItemId = ItemId;
            att.Quantity = Quantity;

            return att;
        }

    }

    public class MapZDimensionAttribute : MapAttribute
    {

        public override MapAttributes Type { get; } = MapAttributes.ZDimension;

        public byte GatewayTo { get; set; }

        public byte BlockedLevel { get; set; }

        public override MapAttribute Clone()
        {
            var att = (MapZDimensionAttribute) base.Clone();
            att.GatewayTo = GatewayTo;
            att.BlockedLevel = BlockedLevel;

            return att;
        }

    }

    public class MapNpcAvoidAttribute : MapAttribute
    {

        public override MapAttributes Type { get; } = MapAttributes.NpcAvoid;

    }

    public class MapWarpAttribute : MapAttribute
    {

        public override MapAttributes Type { get; } = MapAttributes.Warp;

        public Guid MapId { get; set; }

        public byte X { get; set; }

        public byte Y { get; set; }

        public WarpDirection Direction { get; set; } = WarpDirection.Retain;

        public override MapAttribute Clone()
        {
            var att = (MapWarpAttribute) base.Clone();
            att.MapId = MapId;
            att.X = X;
            att.Y = Y;
            att.Direction = Direction;

            return att;
        }

    }

    public class MapSoundAttribute : MapAttribute
    {

        public override MapAttributes Type { get; } = MapAttributes.Sound;

        public string File { get; set; }

        public byte Distance { get; set; }

        public override MapAttribute Clone()
        {
            var att = (MapSoundAttribute) base.Clone();
            att.File = File;
            att.Distance = Distance;

            return att;
        }

    }

    public class MapResourceAttribute : MapAttribute
    {

        public override MapAttributes Type { get; } = MapAttributes.Resource;

        public Guid ResourceId { get; set; }

        public byte SpawnLevel { get; set; }

        public override MapAttribute Clone()
        {
            var att = (MapResourceAttribute) base.Clone();
            att.ResourceId = ResourceId;
            att.SpawnLevel = SpawnLevel;

            return att;
        }

    }

    public class MapAnimationAttribute : MapAttribute
    {

        public override MapAttributes Type { get; } = MapAttributes.Animation;

        public Guid AnimationId { get; set; }

        public override MapAttribute Clone()
        {
            var att = (MapAnimationAttribute) base.Clone();
            att.AnimationId = AnimationId;

            return att;
        }

    }

    public class MapGrappleStoneAttribute : MapAttribute
    {

        public override MapAttributes Type { get; } = MapAttributes.GrappleStone;

    }

    public class MapSlideAttribute : MapAttribute
    {

        public override MapAttributes Type { get; } = MapAttributes.Slide;

        public byte Direction { get; set; }

        public override MapAttribute Clone()
        {
            var att = (MapSlideAttribute) base.Clone();
            att.Direction = Direction;

            return att;
        }

    }

}
