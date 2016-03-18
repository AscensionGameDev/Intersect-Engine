/*
    Intersect Game Engine (Server)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect_Server.Classes
{
    public static class Enums
    {
        public enum ClientPackets
        {
            Ping = 0,
            Login,
            NeedMap,
            SendMove,
            LocalMessage,
            EditorLogin,
            SaveTilesetArray,
            SaveMap,
            CreateMap,
            EnterMap,
            TryAttack,
            SendDir,
            EnterGame,
            ActivateEvent,
            EventResponse,
            CreateAccount,
            OpenItemEditor,
            SaveItem,
            OpenNpcEditor,
            SaveNpc,
            OpenSpellEditor,
            SaveSpell,
            OpenAnimationEditor,
            SaveAnimation,
            PickupItem,
            SwapItems,
            DropItems,
            UseItem,
            SwapSpells,
            ForgetSpell,
            UseSpell,
            UnequipItem,
            UpgradeStat,
            HotbarChange,
            OpenResourceEditor,
            SaveResource,
            OpenClassEditor,
            SaveClass,
            MapListUpdate,
            CreateCharacter,
            OpenQuestEditor,
            SaveQuest,
            OpenAdminWindow,
            AdminAction,
            NeedGrid,
            OpenProjectileEditor,
            SaveProjectile,
        }

        public enum ServerPackets
        {
            RequestPing = 0,
            JoinGame,
            MapData,
            EntityData,
            EntityPosition,
            EntityLeave,
            ChatMessage,
            GameData,
            TilesetArray,
            EnterMap,
            EntityMove,
            EntityVitals,
            EntityStats,
            EntityDir,
            EventDialog,
            MapList,
            LoginError,
            OpenItemEditor,
            ItemData,
            ItemList,
            OpenNpcEditor,
            NpcData,
            NpcList,
            OpenSpellEditor,
            SpellData,
            SpellList,
            OpenAnimationEditor,
            AnimationData,
            AnimationList,
            MapItems,
            MapItemUpdate,
            InventoryUpdate,
            SpellUpdate,
            PlayerEquipment,
            StatPoints,
            HotbarSlots,
            OpenResourceEditor,
            ResourceData,
            OpenClassEditor,
            ClassData,
            CreateCharacter,
            OpenQuestEditor,
            QuestData,
            OpenAdminWindow,
            MapGrid,
            OpenProjectileEditor,
            ProjectileData,
            CastTime,
            SendSpellCooldown,
            Experience,
            ProjectileSpawnDead,
        }

        public enum Stats
        {
            Attack = 0,
            AbilityPower,
            Defense,
            MagicResist,
            Speed,
            StatCount
        }

        public enum Vitals
        {
            Health,
            Mana,
            VitalCount
        }

        public enum ItemTypes
        {
            None = 0,
            Equipment = 1,
            Consumable = 2,
            Currency = 3,
            Spell = 4
        }

        public enum SpellTypes
        {
            CombatSpell = 0,
            Warp = 1,
            Dash = 2,
        }

        public enum TargetTypes
        {
            Self = 0,
            Single = 1,
            AoE = 2,
            Projectile = 3,
        }

        public enum MapListUpdates
        {
            MoveItem = 0,
            AddFolder = 1,
            Rename = 2,
            Delete = 3,
        }

        public enum AdminActions
        {
            WarpTo = 0,
        }

        public enum EntityTypes
        {
            GlobalEntity = 0,
            Player = 1,
            Resource = 2,
            Projectile = 3,
            Event = 4,
        }

        public enum Directions
        {
            Up = 0,
            Down,
            Left,
            Right
        }

        public const int WeaponIndex = 2;
        public const int ShieldIndex = 3;
        public static List<string> EquipmentSlots = new List<string>() { "Helmet", "Armor", "Weapon", "Shield", "Boots" };
        public static List<string> ToolTypes = new List<string>() { "Axe", "Picaxe", "Shovel", "Fishing Rod" };
        public static List<string> ItemBonusEffects = new List<string>() { "Cooldown Reduction", "Life Steal" };

        // Map Attribtes
        public enum MapAttributes : byte { Walkable = 0, Blocked, Item, ZDimension, NPCAvoid, Warp, Sound, Resource, Animation };
    }
}
