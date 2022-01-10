using System;

using Intersect.GameObjects;
using Intersect.Server.Entities;
using Intersect.Server.Entities.Events;
using Intersect.Server.General;
using Intersect.Server.Maps;
using Intersect.Utilities;

namespace Intersect.Server.Classes.Maps
{

    public partial class MapTrapInstance
    {
        public Guid Id { get; } = Guid.NewGuid();

        private long Duration;

        public Guid MapId;

        public Entity Owner;

        public SpellBase ParentSpell;

        public bool Triggered = false;

        public byte X;

        public byte Y;

        public byte Z;

        public MapTrapInstance(Entity owner, SpellBase parentSpell, Guid mapId, Guid mapLayerId, byte x, byte y, byte z)
        {
            Owner = owner;
            ParentSpell = parentSpell;
            Duration = Timing.Global.Milliseconds + ParentSpell.Combat.TrapDuration;
            MapId = mapId;
            X = x;
            Y = y;
            Z = z;
        }

        public void CheckEntityHasDetonatedTrap(Entity entity)
        {
            if (!Triggered)
            {
                if (entity.MapId == MapId && entity.X == X && entity.Y == Y && entity.Z == Z)
                {
                    if (entity.GetType() == typeof(Player) && Owner.GetType() == typeof(Player))
                    {
                        //Don't detonate on yourself and party members on non-friendly spells!
                        if (Owner == entity || ((Player) Owner).InParty((Player) entity))
                        {
                            if (!ParentSpell.Combat.Friendly)
                            {
                                return;
                            }
                        }
                    }

                    if (entity is EventPageInstance)
                    {
                        return;
                    }

                    Owner.TryAttack(entity, ParentSpell, false, true);
                    Triggered = true;
                }
            }
        }

        public void Update()
        {
            if (MapInstance.Get(MapId) != null && MapInstance.Get(MapId).TryGetRelevantProcessingLayer(Owner.InstanceLayer, out var mapProcessingLayer))
            {
                if (Triggered)
                {
                    mapProcessingLayer.RemoveTrap(this);
                }

                if (Timing.Global.Milliseconds > Duration)
                {
                    mapProcessingLayer.RemoveTrap(this);
                }
            }
        }

    }

}
