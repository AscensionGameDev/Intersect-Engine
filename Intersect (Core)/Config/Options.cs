using System.Collections.Generic;
using System.IO;
using Intersect.Config;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Intersect
{
    public class Options
    {
        private static Options _options;

        [JsonIgnore]
        public bool SendingToClient { get; set; } = true;

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
        public static int MapWidth => _options != null ?_options.MapOpts.Width : 32;
        public static int MapHeight => _options != null ? _options.MapOpts.Height : 26;
        public static int TileWidth => _options.MapOpts.TileWidth;
        public static int TileHeight => _options.MapOpts.TileHeight;

        public static bool UPnP => _options._upnp;

        public static bool NoPunchthrough { get; set; }
        public static bool NoNetworkCheck { get; set; }

        public static bool OpenPortChecker => _options._portChecker;
        public static SmtpSettings Smtp => _options.SmtpSettings;
        public static int PasswordResetExpirationMinutes => _options._passResetExpirationMin;
        public static bool SmtpValid => _options._smtpValid;
        public static bool AdminOnly { get => _options._adminOnly; set => _options._adminOnly = value; }

        public static DatabaseOptions PlayerDb
        {
            get => _options.PlayerDatabase;
            set => _options.PlayerDatabase = value;
        }

        public static DatabaseOptions GameDb
        {
            get => _options.GameDatabase;
            set => _options.GameDatabase = value;
        }


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
        protected string _gameName = "Intersect";

        [JsonProperty("ServerPort")]
        protected ushort _serverPort = 5400;

        [JsonProperty("AdminOnly")]
        protected bool _adminOnly = false;

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
        [JsonProperty("AnimatedSprites")]
        protected List<string> _animatedSprites = new List<string>();

        [JsonProperty("Combat")]
        public CombatOptions CombatOpts = new CombatOptions();

        [JsonProperty("Map")]
        public MapOptions MapOpts = new MapOptions();

        [JsonProperty("ValidPasswordResetTimeMinutes")]
        protected ushort _passResetExpirationMin = 30;

        [JsonProperty("SmtpValid")]
        protected bool _smtpValid { get; set; }
        public SmtpSettings SmtpSettings = new SmtpSettings();
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
                _options = JsonConvert.DeserializeObject<Options>(File.ReadAllText("resources/config.json"));
            }
            _options.SendingToClient = false;
            File.WriteAllText("resources/config.json", JsonConvert.SerializeObject(_options,Formatting.Indented));
            _options.SendingToClient = true;
            _options._smtpValid = Smtp.IsValid();
            optionsCompressed = JsonConvert.SerializeObject(_options);
            return true;
        }

        public static void SaveToDisk()
        {
            _options.SendingToClient = false;
            File.WriteAllText("resources/config.json", JsonConvert.SerializeObject(_options, Formatting.Indented));
            _options.SendingToClient = true;
            optionsCompressed = JsonConvert.SerializeObject(_options);
        }

        public static string OptionsData => optionsCompressed;

        public static void LoadFromServer(string data)
        {
            _options = JsonConvert.DeserializeObject<Options>(data);
        }

        public bool ShouldSerializePlayerDatabase => !SendingToClient;

        public bool ShouldSerializeGameDatabase => !SendingToClient;

        public bool ShouldSerializeSmtpSettings => !SendingToClient;

        public bool ShouldSerializeSmtpValid => SendingToClient;
    }
}