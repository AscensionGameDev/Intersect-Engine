/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
using System.Collections.Generic;
namespace Intersect_Client.Classes
{
    public static class Enums
    {

        public enum GameStates
        {
            Intro = 0,
            Menu,
            InGame
        }

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

        public static string GetStatName(int statnum)
        {
            switch (statnum)
            {
                case (int)Stats.Attack:
                    return "Attack";
                case (int)Stats.AbilityPower:
                    return "Ability Power";
                case (int)Stats.Defense:
                    return "Defense";
                case (int)Stats.MagicResist:
                    return "Magic Resist";
                case (int)Stats.Speed:
                    return "Speed";
                default:
                    return "Invalid Stat";
            }
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
            Combat=0,
            Warp=1,
        }

        public enum SpellTargetTypes
        {
            Self=0,
            SingleTarget=1,
            AOE=2,
            Linear=3,
        }

        public enum AdminActions
        {
            WarpTo = 0,
        }

        public enum EntityTypes
        {
            GlobalEntity = 0,
            Player = 1,
            Event = 2,
            Resource = 3,
            Projectile = 4,
            LocalEvent = 5,
        }

        public static int WeaponIndex = 2;
        public static int ShieldIndex = 3;
        //This is programming so indices work like this                  ---0---   ---1---  ---2---   ---3---   ---4---
        public static List<string> EquipmentSlots = new List<string>() { "Helmet", "Armor", "Weapon", "Shield", "Boots"};
        public static List<string> ToolTypes = new List<string>() { "Axe", "Picaxe", "Shovel", "Fishing Rod" };

        public static List<string> ItemBonusEffects = new List<string>() { "Cooldown Reduction", "Life Steal" };

        // Map Attribtes
        public enum MapAttributes : byte { Walkable = 0, Blocked, Item, ZDimension, NPCAvoid, Warp, Sound, Resource };
    }
}
