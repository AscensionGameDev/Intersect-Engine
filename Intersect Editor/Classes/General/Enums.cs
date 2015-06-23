using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect_Editor.Classes
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
            GameTime,
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

        public static int WeaponIndex = 2;
        public static int ShieldIndex = 3;
        public static List<string> EquipmentSlots = new List<string>() { "Helmet", "Armor", "Weapon", "Shield", "Boots" };
        public static List<string> ToolTypes = new List<string>() { "Axe", "Picaxe", "Shovel", "Fishing Rod" };
        public static List<string> ItemBonusEffects = new List<string>() {  "Cooldown Reduction", "Life Steal" };

        // Map Attribtes
        public enum MapAttributes : byte { Walkable = 0, Blocked, Item, ZDimension, NPCAvoid, Warp, NPCSpawn };
    }
}
