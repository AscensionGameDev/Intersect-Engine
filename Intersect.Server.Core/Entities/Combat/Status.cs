using System;
using System.Collections.Generic;
using System.Linq;

using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using Intersect.Utilities;

namespace Intersect.Server.Entities.Combat
{

    public partial class Status
    {

        public string Data = "";

        public long Duration;

        private Entity mEntity;

        public Entity Attacker;

        public SpellBase Spell;

        public long StartTime;

        public SpellEffect Type;

        public static List<SpellEffect> TenacityExcluded = new List<SpellEffect>()
        {
            SpellEffect.None,
            SpellEffect.Stealth,
            SpellEffect.Cleanse,
            SpellEffect.Invulnerable,
            SpellEffect.OnHit,
            SpellEffect.Shield,
            SpellEffect.Transform,
        };

        public static List<SpellEffect> InterruptStatusses = new List<SpellEffect>()
        {
            SpellEffect.Silence,
            SpellEffect.Sleep,
            SpellEffect.Stun,
        };

        public Status(Entity en, Entity attacker, SpellBase spell, SpellEffect type, int duration, string data)
        {
            mEntity = en;
            Attacker = attacker;
            Spell = spell;
            Type = type;
            Data = data;

            // Handle Player specific stuff, such as interrupting spellcasts
            var tenacity = 0f;
            if (en is Player player)
            {
                // Get our player's Tenacity stat!
                if (!TenacityExcluded.Contains(type))
                {
                    tenacity = player.GetEquipmentBonusEffect(ItemEffect.Tenacity);
                }

                // Interrupt their spellcast if we are running a Silence, Sleep or Stun!
                if (Status.InterruptStatusses.Contains(type))
                {
                    player.CastTime = 0;
                    player.CastTarget = null;
                    player.SpellCastSlot = -1;
                    PacketSender.SendEntityCancelCast(player);
                }
            }
            else if (en is Npc thisNpc)
            {
                // Get our NPC's Tenacity stat
                if (!Status.TenacityExcluded.Contains(type))
                {
                    tenacity = (float)thisNpc.Base.Tenacity;
                }
            }

            // Handle Npc specific stuff, such as loot tables!
            if (en is Npc npc)
            {
                // Add to loot map if not already eligible.
                if (!npc.LootMap.ContainsKey(Attacker.Id))
                {
                    npc.LootMap.TryAdd(Attacker.Id, true);
                    npc.LootMapCache = npc.LootMap.Keys.ToArray();
                }
            }

            // Interrupt their spellcast if we are running a Silence, Sleep or Stun!
            if (InterruptStatusses.Contains(type))
            {
                en.CastTime = 0;
                en.CastTarget = null;
                en.SpellCastSlot = -1;
                PacketSender.SendEntityCancelCast(en);
            }

            // If we're adding a shield, actually add that according to the settings.
            if (type == SpellEffect.Shield)
            {
                foreach (var vital in Enum.GetValues<Vital>())
                {
                    var vitalDiff = Math.Abs(spell.Combat.VitalDiff[(int)vital]);

                    // If the user did not configure for this vital to have a mana shield, ignore it
                    if (vitalDiff == 0 && vital == Vital.Mana)
                    {
                        continue;
                    }

                    var shieldAmount = Formulas.CalculateDamage(
                        vitalDiff, (DamageType)spell.Combat.DamageType, (Enums.Stat)spell.Combat.ScalingStat, spell.Combat.Scaling, 1.0, attacker, en
                    );

                    Shield[(int)vital] = Math.Abs(shieldAmount);
                }
            }

            // If new Cleanse spell, remove all opposite statusses. (ie friendly dispels unfriendly and vice versa)
            if (Type == SpellEffect.Cleanse)
            {
                foreach (var status in en.CachedStatuses)
                {
                    if (spell.Combat.Friendly != status.Spell.Combat.Friendly)
                    {
                        status.RemoveStatus();
                    }
                }

                foreach (var dot in en.CachedDots)
                {
                    if (spell.Combat.Friendly != dot.SpellBase.Combat.Friendly)
                    {
                        dot.Expire();
                    }
                }
            }

            // Remove existing taunts if this is one and there are any others.
            // We'll be overwriting it, baby!
            if (Type == SpellEffect.Taunt)
            {
                foreach (var status in en.CachedStatuses)
                {
                    if (status.Type == SpellEffect.Taunt)
                    {
                        status.RemoveStatus();
                    }
                }
            }

            // Calculate our final duration and pass it on!
            var finalDuration = duration - (duration * (tenacity / 100f));
            if (en.Statuses.ContainsKey(spell))
            {
                en.Statuses[spell].StartTime = Timing.Global.Milliseconds;
                en.Statuses[spell].Duration = Timing.Global.Milliseconds + (long)finalDuration;
                en.Statuses[spell].StartTime = StartTime;
                en.CachedStatuses = en.Statuses.Values.ToArray();
            }
            else
            {
                StartTime = Timing.Global.Milliseconds;
                Duration = Timing.Global.Milliseconds + (long)finalDuration;
                en.Statuses.TryAdd(Spell, this);
                en.CachedStatuses = en.Statuses.Values.ToArray();
            }

            // If this is a taunt, force the target properly for players and NPCs
            if (Type == SpellEffect.Taunt)
            {

                // If player, force send target!
                if (en is Player targetPlayer)
                {
                    en.Target = Attacker;
                    PacketSender.SetPlayerTarget(targetPlayer, Attacker.Id);
                }
                // If NPC, force assign target and make sure we have the highest threat +1 so we in theory have the highest threat.. for now..
                else if (en is Npc targetNpc)
                {
                    targetNpc.AssignTarget(Attacker);

                    if (!targetNpc.DamageMap.ContainsKey(Attacker))
                    {
                        if (targetNpc.DamageMap.Count > 0)
                        {
                            targetNpc.DamageMap.TryAdd(Attacker, targetNpc.DamageMap.ToArray().Max(x => x.Value) + 1);
                        }
                        else
                        {
                            targetNpc.DamageMap.TryAdd(Attacker, 1);
                        }
                    }
                    else
                    {
                        targetNpc.DamageMap[Attacker] = targetNpc.DamageMap.ToArray().Max(x => x.Value) + 1;
                    }
                }
                else
                {
                    en.Target = Attacker;
                }

            }
        }

        public int[] Shield { get; set; } = new int[Enum.GetValues<Vital>().Length];

        public void TryRemoveStatus()
        {
            if (Duration <= Timing.Global.Milliseconds) //Check the timer
            {
                RemoveStatus();
            }

            //If shield check for out of hp
            if (Type == SpellEffect.Shield)
            {
                for (var i = (int)Vital.Health; i < Enum.GetValues<Vital>().Length; i++)
                {
                    if (Shield[i] > 0)
                    {
                        return;
                    }
                }

                RemoveStatus();
            }
        }

        public void RemoveStatus()
        {
            mEntity.Statuses.TryRemove(Spell, out Status val);
            mEntity.CachedStatuses = mEntity.Statuses.Values.ToArray();

            // if this was a taunt status being removed, we have to scan for a new target!
            if (mEntity is Npc npc && Type == SpellEffect.Taunt)
            {
                npc.TryFindNewTarget(0, Guid.Empty, true);
            }
        }

        public void DamageShield(Vital vital, ref int amount)
        {
            if (Type == SpellEffect.Shield)
            {
                Shield[(int)vital] -= amount;
                if (Shield[(int)vital] <= 0)
                {
                    amount = -Shield[(int)vital]; //Return piercing damage.
                    Shield[(int)vital] = 0;
                    TryRemoveStatus();
                }
                else
                {
                    amount = 0; //Sheild is stronger than the damage dealt, so no piercing damage.
                }
            }
        }

    }

}
