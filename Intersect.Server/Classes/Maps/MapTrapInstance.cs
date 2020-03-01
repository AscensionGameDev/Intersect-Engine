using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.GameObjects;
using Intersect.Network.Packets.Client;
using Intersect.Server.Entities;
using Intersect.Server.General;
using Intersect.Server.Maps;
using Intersect.Server.Networking;

namespace Intersect.Server.Classes.Maps
{
    public class MapTrapInstance
    {
        public EntityInstance Owner;
        public SpellBase ParentSpell;

        public Guid MapId;
        public byte X;
        public byte Y;
        public byte Z;

        public bool Triggered = false;

        private long Duration;

        public MapTrapInstance(EntityInstance owner, SpellBase parentSpell, Guid mapId, byte x, byte y, byte z)
        {
            Owner = owner;
            ParentSpell = parentSpell;
            Duration = Globals.Timing.TimeMs + ParentSpell.Combat.TrapDuration;
            MapId = mapId;
            X = x;
            Y = y;
            Z = z;
        }

        public void CheckEntityHasDetonatedTrap(EntityInstance entity)
        {
            if (!Triggered)
            {
                if (entity.MapId == MapId && entity.X == X && entity.Y == Y && entity.Z == Z)
                {
                    if (entity.GetType() == typeof(Player) && Owner.GetType() == typeof(Player))
                    {
                        //Don't detonate on yourself and party members on non-friendly spells!
                        if (Owner == entity || ((Player)Owner).InParty((Player)entity))
                        {
                            if (!ParentSpell.Combat.Friendly) return;
                        }
                    }

                    Owner.TryAttack(entity, ParentSpell, false, true);
                    Triggered = true;
                }
            }
        }

        public void Update()
        {
            if (Triggered)
                MapInstance.Get(MapId).RemoveTrap(this);

            if (Globals.Timing.TimeMs > Duration)
                MapInstance.Get(MapId).RemoveTrap(this);
        }
    }
}
