namespace Intersect_Client.Classes
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
            InventoryUpdate,
            SpellUpdate,
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

        public enum ItemTypes : int
        {
            None = 0,
            Weapon = 1,
            Armor = 2,
            Helmet = 3,
            Shield = 4,
            Consumable = 5,
            Currency = 6,
            Spell = 7
        }

        // Map Attribtes
        public enum MapAttributes : byte { Walkable = 0, Blocked, Item };
    }
}
