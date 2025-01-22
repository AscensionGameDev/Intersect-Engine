using System.ComponentModel;
using Intersect.Config;
using Intersect.Config.Guilds;
using Intersect.Core;
using Intersect.Framework.Annotations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Intersect;

public partial class Options
{
    #region Constants

    public const string DefaultGameName = "Intersect";

    public const int DefaultServerPort = 5400;

    #endregion

    #region Static Properties

    public static string ResourcesDirectory { get; set; } = "resources";

    public static Options Instance { get; private set; }

    public static Options? PendingChanges { get; private set; }

    public static bool IsLoaded => Instance != null;

    #endregion Static Properties

    #region Transient Properties

    [Ignore]
    [JsonIgnore]
    public string OptionsData { get; private set; } = string.Empty;

    [Ignore]
    [JsonIgnore]
    public bool SendingToClient { get; set; } = true;

    [Ignore]
    public bool SmtpValid { get; private set; }

    #endregion Transient Properties

    #region Configuration Properties

    #region Game Core

    [JsonProperty(Order = -100)]
    [RequiresRestart]
    public string GameName { get; set; } = DefaultGameName;

    [JsonProperty(Order = -100)]
    [RequiresRestart]
    public ushort ServerPort { get; set; } = DefaultServerPort;

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
    [RequiresRestart]
    public bool UPnP { get; set; } = true;

    [JsonProperty(Order = -91)]
    [RequiresRestart]
    public bool OpenPortChecker { get; set; } = true;

    [JsonProperty(Order = -91, NullValueHandling = NullValueHandling.Include)]
    [RequiresRestart]
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
    [RequiresRestart]
    public DatabaseOptions GameDatabase = new();

    [JsonProperty(Order = -70)]
    [RequiresRestart]
    public DatabaseOptions LoggingDatabase = new();

    [JsonProperty(Order = -70)]
    [RequiresRestart]
    public DatabaseOptions PlayerDatabase = new();

    #endregion Database

    #region Security

    [JsonProperty(Order = -60)]
    [RequiresRestart]
    public SecurityOptions Security = new();

    [JsonProperty(Order = -60)]
    [RequiresRestart]
    public SmtpSettings SmtpSettings = new();

    #endregion Security

    #region Other Game Properties

    [RequiresRestart]
    public List<string> AnimatedSprites { get; set; } = [];

    [RequiresRestart]
    public PacketOptions Packets = new();

    public ChatOptions Chat = new();

    [RequiresRestart]
    public CombatOptions Combat = new();

    [RequiresRestart]
    public EquipmentOptions Equipment = new();

    [RequiresRestart]
    public int EventWatchdogKillThreshold { get; set; } = 5000;

    /// <summary>
    /// Passability configuration by map zone
    /// </summary>
    public PassabilityOptions Passability { get; } = new();

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

    public void FixAnimatedSprites()
    {
        for (var i = 0; i < AnimatedSprites.Count; i++)
        {
            AnimatedSprites[i] = AnimatedSprites[i].ToLower();
        }
    }

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
}
