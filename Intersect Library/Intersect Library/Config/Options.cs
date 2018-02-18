using System.Collections.Generic;
using System.IO;
using Intersect.Config;
using Newtonsoft.Json;

namespace Intersect
{
    public class Options
    {
        private static Options _options;

        //Public Getters
        public static string Language => _options._language;
        public static ushort ServerPort { get => _options._serverPort; set => _options._serverPort = value; }
        public static string GameName => _options._gameName;
        public static int MaxStatValue => _options.PlayerOpts.MaxStat;
        public static int MaxLevel => _options.PlayerOpts.MaxLevel;
        public static int MaxInvItems => _options.PlayerOpts.MaxInventory;
        public static int MaxPlayerSkills => _options.PlayerOpts.MaxSpells;
        public static int MaxBankSlots => _options.PlayerOpts.MaxBank;
        public static int MaxCharacters => _options.PlayerOpts.MaxCharacters;
        public static int ItemDropChance => _options.PlayerOpts.ItemDropChance;
        public static int WeaponIndex => _options.EquipmentOpts.WeaponSlot;
        public static int ShieldIndex => _options.EquipmentOpts.ShieldSlot;
        public static List<string> EquipmentSlots => _options.EquipmentOpts.Slots;
        public static List<string>[] PaperdollOrder => _options.EquipmentOpts.Paperdoll.Directions;
        public static List<string> ToolTypes => _options.EquipmentOpts.ToolTypes;
        public static List<string> AnimatedSprites => _options._animatedSprites;
        public static bool[] PlayerPassable => _options.PassabilityOpts.Passable;
        public static int RegenTime => _options.CombatOpts.RegenTime;
        public static int MinAttackRate => _options.CombatOpts.MinAttackRate;
        public static int MaxAttackRate => _options.CombatOpts.MaxAttackRate;
        public static int BlockingSlow => _options.CombatOpts.BlockingSlow;
        public static int CritChance => _options.CombatOpts.CritChance;
        public static int CritMultiplier => _options.CombatOpts.CritMultiplier;
        public static int MaxDashSpeed => _options.CombatOpts.MaxDashSpeed;
        public static int GameBorderStyle => _options.MapOpts.GameBorderStyle;
        public static int ItemRepawnTime => _options.MapOpts.ItemSpawnTime;
        public static int ItemDespawnTime => _options.MapOpts.ItemDespawnTime;
        public static bool ZDimensionVisible => _options.MapOpts.ZDimensionVisible;
        public static int MapWidth => _options.MapOpts.MapWidth;
        public static int MapHeight => _options.MapOpts.MapHeight;
        public static int TileWidth => _options.MapOpts.TileWidth;
        public static int TileHeight => _options.MapOpts.TileHeight;
        public static bool UPnP => _options._upnp;
        public static bool OpenPortChecker => _options._portChecker;
        public static bool ProgressSavedMessages => _options.PlayerOpts.ProgressSavedMessages;

        public static bool Loaded => _options != null;
        
        //Values that cannot easily be changed:
        public const int LayerCount = 5;
        public const int MaxStats = 5;
        public const int MaxHotbar = 10;

        //Game Settings
        [JsonProperty("Language")]
        protected string _language = "English";

        [JsonProperty("GameName")]
        protected string _gameName = "Intersect";

        [JsonProperty("ServerPort")]
        protected ushort _serverPort = 5400;

        [JsonProperty("UPnP")]
        protected bool _upnp = true;

        [JsonProperty("OpenPortChecker")]
        protected bool _portChecker = true;

        [JsonProperty ("Player")]
        public PlayerOptions PlayerOpts = new PlayerOptions();

        //Passability Based on MapZones
        [JsonProperty ("Passability")]
        public PassabilityOptions PassabilityOpts = new PassabilityOptions();

        [JsonProperty("Equipment")]
        public EquipmentOptions EquipmentOpts = new EquipmentOptions();
        

        //Constantly Animated Sprites
        protected List<string> _animatedSprites = new List<string>();

        [JsonProperty("Combat")]
        public CombatOptions CombatOpts = new CombatOptions();

        [JsonProperty("Map")]
        public MapOptions MapOpts = new MapOptions();


        //Caching Json
        private static string optionsCompressed = "";

        public static bool LoadFromDisk()
        {
            _options = new Options();
            if (File.Exists("resources/config.json"))
            {
                _options = JsonConvert.DeserializeObject<Options>(File.ReadAllText("resources/config.json"));
            }
            File.WriteAllText("resources/config.json", JsonConvert.SerializeObject(_options,Formatting.Indented));
            optionsCompressed = JsonConvert.SerializeObject(_options);
            return true;
        }

        public static byte[] GetOptionsData()
        {
            var bf = new ByteBuffer();
            bf.WriteString(optionsCompressed);
            return bf.ToArray();
        }

        public static void LoadFromServer(ByteBuffer bf)
        {
            _options = JsonConvert.DeserializeObject<Options>(bf.ReadString());
        }
    }
}