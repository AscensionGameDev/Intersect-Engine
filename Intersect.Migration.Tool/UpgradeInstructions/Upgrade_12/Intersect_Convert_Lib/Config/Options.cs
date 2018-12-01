using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Config
{
    public class Options
    {
        public static Options _options;

        [JsonIgnore]
        public bool ExportDatabaseSettings { get; set; }

        //Public Getters
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
        public static int MaxDashSpeed => _options.CombatOpts.MaxDashSpeed;
        public static int GameBorderStyle => _options.MapOpts.GameBorderStyle;
        public static int ItemRepawnTime => _options.MapOpts.ItemSpawnTime;
        public static int ItemDespawnTime => _options.MapOpts.ItemDespawnTime;
        public static bool ZDimensionVisible => _options.MapOpts.ZDimensionVisible;
        public static int MapWidth => _options.MapOpts.Width;
        public static int MapHeight => _options.MapOpts.Height;
        public static int TileWidth => _options.MapOpts.TileWidth;
        public static int TileHeight => _options.MapOpts.TileHeight;
        public static bool UPnP => _options._upnp;
        public static bool OpenPortChecker => _options._portChecker;
        public static bool ProgressSavedMessages => _options.PlayerOpts.ProgressSavedMessages;
        public static bool ApiEnabled => _options._api;
        public static ushort ApiPort => _options._apiPort;
        public static DatabaseOptions PlayerDb => _options.PlayerDatabase;
        public static DatabaseOptions GameDb => _options.GameDatabase;


        [NotNull]
        public static PlayerOptions Player => _options.PlayerOpts;

        [NotNull]
        public static PassabilityOptions Passability => _options.PassabilityOpts;

        [NotNull]
        public static EquipmentOptions Equipment => _options.EquipmentOpts;

        [NotNull]
        public static CombatOptions Combat => _options.CombatOpts;

        [NotNull]
        public static MapOptions Map => _options.MapOpts;

        public static bool Loaded => _options != null;
        
        //Values that cannot easily be changed:
        public const int LayerCount = 5;
        public const int MaxStats = 5;
        public const int MaxHotbar = 10;

        [JsonProperty("GameName")]
        public string _gameName = "Intersect";

        [JsonProperty("ServerPort")]
        public ushort _serverPort = 5400;

        [JsonProperty("UseApi")]
        public bool _api;

        [JsonProperty("ApiPort")]
        public ushort _apiPort = 5400; //This makes sense because the server uses udp and the api can use tcp

        [JsonProperty("UPnP")]
        public bool _upnp = true;

        [JsonProperty("OpenPortChecker")]
        public bool _portChecker = true;

        [JsonProperty ("Player")]
        public PlayerOptions PlayerOpts = new PlayerOptions();

        //Passability Based on MapZones
        [JsonProperty ("Passability")]
        public PassabilityOptions PassabilityOpts = new PassabilityOptions();

        [JsonProperty("Equipment")]
        public EquipmentOptions EquipmentOpts = new EquipmentOptions();


        //Constantly Animated Sprites
        [JsonProperty("AnimatedSprites")]
        public List<string> _animatedSprites = new List<string>();

        [JsonProperty("Combat")]
        public CombatOptions CombatOpts = new CombatOptions();

        [JsonProperty("Map")]
        public MapOptions MapOpts = new MapOptions();

        public DatabaseOptions PlayerDatabase = new DatabaseOptions();
        public DatabaseOptions GameDatabase = new DatabaseOptions();


        //Caching Json
        private static string optionsCompressed = "";

        public static bool LoadFromDisk()
        {
            _options = new Options();
            if (!Directory.Exists("resources")) Directory.CreateDirectory("resources");
            if (File.Exists("resources/config.json"))
            {
                _options = JsonConvert.DeserializeObject<Options>(File.ReadAllText("resources/config.json", Encoding.UTF8));
            }
            _options.ExportDatabaseSettings = true;
            File.WriteAllText("resources/config.json", JsonConvert.SerializeObject(_options,Formatting.Indented), Encoding.UTF8);
            _options.ExportDatabaseSettings = false;
            optionsCompressed = JsonConvert.SerializeObject(_options);
            return true;
        }

        public static void SaveToDisk()
        {
            _options.ExportDatabaseSettings = true;
            File.WriteAllText("resources/config.json", JsonConvert.SerializeObject(_options, Formatting.Indented), Encoding.UTF8);
            _options.ExportDatabaseSettings = false;
            optionsCompressed = JsonConvert.SerializeObject(_options);
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

        public bool ShouldSerializePlayerDatabase()
        {
            return ExportDatabaseSettings;
        }

        public bool ShouldSerializeGameDatabase()
        {
            return ExportDatabaseSettings;
        }
    }
}