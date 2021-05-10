using System.Collections.Generic;
using System.IO;

using Intersect.Config;
using Intersect.Config.Guilds;
using Newtonsoft.Json;

namespace Intersect
{

    public class Options
    {

        //Caching Json
        private static string optionsCompressed = "";

        [JsonProperty("AdminOnly", Order = -3)]
        protected bool _adminOnly = false;

        //Constantly Animated Sprites
        [JsonProperty("AnimatedSprites")] protected List<string> _animatedSprites = new List<string>();

        [JsonProperty("BlockClientRegistrations", Order = -2)]
        protected bool _blockClientRegistrations = false;

        [JsonProperty("ValidPasswordResetTimeMinutes")]
        protected ushort _passResetExpirationMin = 30;

        [JsonProperty("OpenPortChecker", Order = 0)]
        protected bool _portChecker = true;

        [JsonProperty("MaxClientConnections")]
        protected int _maxConnections = 100;

        [JsonProperty("MaximumLoggedinUsers")]
        protected int _maxUsers = 50;

        [JsonProperty("UPnP", Order = -1)] protected bool _upnp = true;

        [JsonProperty("Chat")] public ChatOptions ChatOpts = new ChatOptions();

        [JsonProperty("Combat")] public CombatOptions CombatOpts = new CombatOptions();

        [JsonProperty("Equipment")] public EquipmentOptions EquipmentOpts = new EquipmentOptions();

        [JsonProperty("EventWatchdogKillThreshold")]
        public int EventKillTheshhold = 5000;

        public DatabaseOptions GameDatabase = new DatabaseOptions();

        [JsonProperty("Map")] public MapOptions MapOpts = new MapOptions();

        public DatabaseOptions PlayerDatabase = new DatabaseOptions();

        [JsonProperty("Player")] public PlayerOptions PlayerOpts = new PlayerOptions();

        [JsonProperty("Party")] public PartyOptions PartyOpts = new PartyOptions();

        [JsonProperty("Security")] public SecurityOptions SecurityOpts = new SecurityOptions();

        [JsonProperty("Loot")] public LootOptions LootOpts = new LootOptions();

        public ProcessingOptions Processing = new ProcessingOptions();

        public SpriteOptions Sprites = new SpriteOptions();

        [JsonProperty("Npc")] public NpcOptions NpcOpts = new NpcOptions();

        public MetricsOptions Metrics = new MetricsOptions();

        public PacketOptions Packets = new PacketOptions();

        public SmtpSettings SmtpSettings = new SmtpSettings();

        public QuestOptions Quest = new QuestOptions();

        public GuildOptions Guild = new GuildOptions();

        public static Options Instance { get; private set; }

        [JsonIgnore]
        public bool SendingToClient { get; set; } = true;

        //Public Getters
        public static ushort ServerPort { get => Instance._serverPort; set => Instance._serverPort = value; }

        /// <summary>
        /// Defines the maximum amount of network connections our server is allowed to handle.
        /// </summary>
        public static int MaxConnections => Instance._maxConnections;

        /// <summary>
        /// Defines the maximum amount of logged in users our server is allowed to handle.
        /// </summary>
        public static int MaxLoggedinUsers => Instance._maxUsers;

        public static int MaxStatValue => Instance.PlayerOpts.MaxStat;

        public static int MaxLevel => Instance.PlayerOpts.MaxLevel;

        public static int MaxInvItems => Instance.PlayerOpts.MaxInventory;

        public static int MaxPlayerSkills => Instance.PlayerOpts.MaxSpells;

        public static int MaxBankSlots => Instance.PlayerOpts.MaxBank;

        public static int MaxCharacters => Instance.PlayerOpts.MaxCharacters;

        public static int ItemDropChance => Instance.PlayerOpts.ItemDropChance;

        public static int RequestTimeout => Instance.PlayerOpts.RequestTimeout;

        public static int TradeRange => Instance.PlayerOpts.TradeRange;

        public static int WeaponIndex => Instance.EquipmentOpts.WeaponSlot;

        public static int ShieldIndex => Instance.EquipmentOpts.ShieldSlot;

        public static List<string> EquipmentSlots => Instance.EquipmentOpts.Slots;

        public static List<string>[] PaperdollOrder => Instance.EquipmentOpts.Paperdoll.Directions;

        public static List<string> ToolTypes => Instance.EquipmentOpts.ToolTypes;

        public static List<string> AnimatedSprites => Instance._animatedSprites;

        public static int RegenTime => Instance.CombatOpts.RegenTime;

        public static int CombatTime => Instance.CombatOpts.CombatTime;

        public static int MinAttackRate => Instance.CombatOpts.MinAttackRate;

        public static int MaxAttackRate => Instance.CombatOpts.MaxAttackRate;

        public static int BlockingSlow => Instance.CombatOpts.BlockingSlow;

        public static int MaxDashSpeed => Instance.CombatOpts.MaxDashSpeed;

        public static int GameBorderStyle => Instance.MapOpts.GameBorderStyle;

        public static bool ZDimensionVisible => Instance.MapOpts.ZDimensionVisible;

        public static int MapWidth => Instance?.MapOpts?.Width ?? 32;

        public static int MapHeight => Instance?.MapOpts?.Height ?? 26;

        public static int TileWidth => Instance.MapOpts.TileWidth;

        public static int TileHeight => Instance.MapOpts.TileHeight;

        public static int EventWatchdogKillThreshhold => Instance.EventKillTheshhold;

        public static int MaxChatLength => Instance.ChatOpts.MaxChatLength;

        public static int MinChatInterval => Instance.ChatOpts.MinIntervalBetweenChats;

        public static LootOptions Loot => Instance.LootOpts;

        public static NpcOptions Npc => Instance.NpcOpts;

        public static PartyOptions Party => Instance.PartyOpts;

        public static ChatOptions Chat => Instance.ChatOpts;

        public static bool UPnP => Instance._upnp;

        public static bool OpenPortChecker => Instance._portChecker;

        public static SmtpSettings Smtp => Instance.SmtpSettings;

        public static int PasswordResetExpirationMinutes => Instance._passResetExpirationMin;

        public static bool AdminOnly { get => Instance._adminOnly; set => Instance._adminOnly = value; }

        public static bool BlockClientRegistrations
        {
            get => Instance._blockClientRegistrations;
            set => Instance._blockClientRegistrations = value;
        }

        public static DatabaseOptions PlayerDb
        {
            get => Instance.PlayerDatabase;
            set => Instance.PlayerDatabase = value;
        }

        public static DatabaseOptions GameDb
        {
            get => Instance.GameDatabase;
            set => Instance.GameDatabase = value;
        }

        public static PlayerOptions Player => Instance.PlayerOpts;

        public static EquipmentOptions Equipment => Instance.EquipmentOpts;

        public static CombatOptions Combat => Instance.CombatOpts;

        public static MapOptions Map => Instance.MapOpts;

        public static bool Loaded => Instance != null;

        [JsonProperty("GameName", Order = -5)]
        public string GameName { get; set; } = DEFAULT_GAME_NAME;

        [JsonProperty("ServerPort", Order = -4)]
        public ushort _serverPort { get; set; } = DEFAULT_SERVER_PORT;

        /// <summary>
        /// Passability configuration by map zone
        /// </summary>
        public Passability Passability { get; } = new Passability();

        public bool SmtpValid { get; set; }

        public static string OptionsData => optionsCompressed;

        public void FixAnimatedSprites()
        {
            for (var i = 0; i < _animatedSprites.Count; i++)
            {
                _animatedSprites[i] = _animatedSprites[i].ToLower();
            }
        }

        public static bool LoadFromDisk()
        {
            Instance = new Options();
            if (!Directory.Exists("resources"))
            {
                Directory.CreateDirectory("resources");
            }

            if (File.Exists("resources/config.json"))
            {
                Instance = JsonConvert.DeserializeObject<Options>(File.ReadAllText("resources/config.json"));
            }

            Instance.SmtpValid = Instance.SmtpSettings.IsValid();
            Instance.SendingToClient = false;
            Instance.FixAnimatedSprites();
            File.WriteAllText("resources/config.json", JsonConvert.SerializeObject(Instance, Formatting.Indented));
            Instance.SendingToClient = true;
            optionsCompressed = JsonConvert.SerializeObject(Instance);

            return true;
        }

        public static void SaveToDisk()
        {
            Instance.SendingToClient = false;
            File.WriteAllText("resources/config.json", JsonConvert.SerializeObject(Instance, Formatting.Indented));
            Instance.SendingToClient = true;
            optionsCompressed = JsonConvert.SerializeObject(Instance);
        }

        public static void LoadFromServer(string data)
        {
            Instance = JsonConvert.DeserializeObject<Options>(data);
        }

        // ReSharper disable once UnusedMember.Global
        public bool ShouldSerializePlayerDatabase()
        {
            return !SendingToClient;
        }

        // ReSharper disable once UnusedMember.Global
        public bool ShouldSerializeGameDatabase()
        {
            return !SendingToClient;
        }

        // ReSharper disable once UnusedMember.Global
        public bool ShouldSerializeSmtpSettings()
        {
            return !SendingToClient;
        }

        // ReSharper disable once UnusedMember.Global
        public bool ShouldSerializeSmtpValid()
        {
            return SendingToClient;
        }

        // ReSharper disable once UnusedMember.Global
        public bool ShouldSerializeSecurityOpts()
        {
            return !SendingToClient;
        }

        #region Constants

        // TODO: Clean these up
        //Values that cannot easily be changed:

        public const int MaxHotbar = 10;

        public const string DEFAULT_GAME_NAME = "Intersect";

        public const int DEFAULT_SERVER_PORT = 5400;

        #endregion

    }

}
