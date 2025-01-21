using Intersect.Config;
using Intersect.Config.Guilds;
using Intersect.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Intersect;

public partial class Options
{
    #region Transient Properties

    [JsonIgnore]
    public string OptionsData { get; private set; } = string.Empty;

    [JsonIgnore]
    public bool SendingToClient { get; set; } = true;

    #endregion Transient Properties

    #region Configuration Properties

    #region Game Core

    [JsonProperty(Order = -100)]
    public string GameName { get; set; } = DEFAULT_GAME_NAME;

    [JsonProperty(Order = -100)]
    public ushort ServerPort { get; set; } = DEFAULT_SERVER_PORT;

    #endregion Game Core

    #region Game Access

    [JsonProperty(Order = -99)]
    public bool AdminOnly { get; set; }

    [JsonProperty(Order = -99)]
    public bool BlockClientRegistrations { get; set; }

    [JsonProperty(Order = -99)]
    public int MaxClientConnections { get; set; } = 100;

    /// <summary>
    /// Defines the maximum amount of logged-in users our server is allowed to handle.
    /// </summary>
    [JsonProperty(Order = -99)]
    public int MaximumLoggedInUsers { get; set; } = 50;

    #endregion Game Access

    #region Network Visibility

    [JsonProperty(Order = -91)]
    public bool UPnP { get; set; } = true;

    [JsonProperty(Order = -91)]
    public bool OpenPortChecker { get; set; } = true;

    [JsonProperty(Order = -91, NullValueHandling = NullValueHandling.Include)]
    public string? PortCheckerUrl { get; set; }

    #endregion Network Visibility

    #region Logging and Metrics

    [JsonProperty(Order = -80)]
    public LoggingOptions Logging = new();

    [JsonProperty(Order = -80)]
    public MetricsOptions Metrics = new();

    #endregion Logging and Metrics

    #region Database

    [JsonProperty(Order = -70)]
    public DatabaseOptions GameDatabase = new();

    [JsonProperty(Order = -70)]
    public DatabaseOptions LoggingDatabase = new();

    [JsonProperty(Order = -70)]
    public DatabaseOptions PlayerDatabase = new();

    #endregion Database

    #region Security

    [JsonProperty(Order = -60)]
    public SecurityOptions Security = new();

    [JsonProperty(Order = -60)]
    public SmtpSettings SmtpSettings = new();

    #endregion Security

    #region Other Game Properties

    public List<string> AnimatedSprites { get; set; } = [];

    public PacketOptions Packets = new();

    public ChatOptions Chat = new();

    public CombatOptions Combat = new();

    public EquipmentOptions Equipment = new();

    public int EventWatchdogKillThreshold { get; set; } = 5000;

    /// <summary>
    /// Passability configuration by map zone
    /// </summary>
    public Passability Passability { get; } = new();

    public ushort ValidPasswordResetTimeMinutes { get; set; } = 30;

    public MapOptions Map = new();

    public PlayerOptions Player = new();

    public PartyOptions Party = new();

    public LootOptions Loot = new();

    public ProcessingOptions Processing = new();

    public SpriteOptions Sprites = new();

    public NpcOptions Npc = new();

    public QuestOptions Quest = new();

    public GuildOptions Guild = new();

    public BankOptions Bank = new();

    public InstancingOptions Instancing = new();

    public ItemOptions Items = new();

    #endregion Other Game Properties

    #endregion Configuration Properties

    public static Options Instance { get; private set; }

    public static bool IsLoaded => Instance != null;

    public bool SmtpValid { get; set; }

    public void FixAnimatedSprites()
    {
        for (var i = 0; i < AnimatedSprites.Count; i++)
        {
            AnimatedSprites[i] = AnimatedSprites[i].ToLower();
        }
    }

    public static string ResourcesDirectory { get; set; } = "resources";

    public static bool LoadFromDisk()
    {
        Options instance = new();
        Instance = instance;

        var pathToServerConfig = Path.Combine(ResourcesDirectory, "config.json");
        if (!Directory.Exists(ResourcesDirectory))
        {
            Directory.CreateDirectory(ResourcesDirectory);
        }
        else if (File.Exists(pathToServerConfig))
        {
            instance = JsonConvert.DeserializeObject<Options>(File.ReadAllText(pathToServerConfig)) ?? instance;
            Instance = instance;
        }

        instance.SmtpValid = instance.SmtpSettings.IsValid();
        instance.FixAnimatedSprites();

        SaveToDisk();

        return true;
    }

    public static void SaveToDisk()
    {
        if (Instance is not { } instance)
        {
            ApplicationContext.Context.Value?.Logger.LogError("Tried to save null instance to disk");
            return;
        }

        if (!Directory.Exists(ResourcesDirectory))
        {
            Directory.CreateDirectory(ResourcesDirectory);
        }

        var pathToServerConfig = Path.Combine(ResourcesDirectory, "config.json");

        instance.SendingToClient = false;
        try
        {
            File.WriteAllText(
                pathToServerConfig,
                JsonConvert.SerializeObject(instance, Formatting.Indented)
            );
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogError(
                exception,
                "Failed to save options to {OptionsPath}",
                pathToServerConfig
            );
        }
        instance.SendingToClient = true;
        instance.OptionsData = JsonConvert.SerializeObject(instance);
    }

    public static void LoadFromServer(string data)
    {
        Instance = JsonConvert.DeserializeObject<Options>(data);
    }

    // ReSharper disable once UnusedMember.Global
    public bool ShouldSerializeGameDatabase()
    {
        return !SendingToClient;
    }

    // ReSharper disable once UnusedMember.Global
    public bool ShouldSerializeLoggingDatabase()
    {
        return !SendingToClient;
    }

    // ReSharper disable once UnusedMember.Global
    public bool ShouldSerializeLogging()
    {
        return !SendingToClient;
    }

    // ReSharper disable once UnusedMember.Global
    public bool ShouldSerializePlayerDatabase()
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
    public bool ShouldSerializeSecurity()
    {
        return !SendingToClient;
    }

    #region Constants

    // TODO: Clean these up
    //Values that cannot easily be changed:

    public const string DEFAULT_GAME_NAME = "Intersect";

    public const int DEFAULT_SERVER_PORT = 5400;

    #endregion
}
