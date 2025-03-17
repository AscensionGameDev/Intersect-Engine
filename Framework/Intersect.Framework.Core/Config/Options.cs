using System.ComponentModel;
using Intersect.Config;
using Intersect.Config.Guilds;
using Intersect.Core;
using Intersect.Framework.Annotations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Intersect;

public partial record Options
{
    private static readonly JsonSerializerSettings PrivateIndentedSerializerSettings = new()
    {
        ContractResolver = new OptionsContractResolver(true, false),
        Formatting = Formatting.Indented,
    };

    private static readonly JsonSerializerSettings PrivateSerializerSettings = new()
    {
        ContractResolver = new OptionsContractResolver(true, false),
    };

    private static readonly JsonSerializerSettings PublicSerializerSettings = new()
    {
        ContractResolver = new OptionsContractResolver(false, true),
    };

    #region Constants

    public const string DefaultGameName = "Intersect";

    public const int DefaultServerPort = 5400;

    public const string CategoryCore = nameof(CategoryCore);

    public const string CategoryDatabase = nameof(CategoryDatabase);

    public const string CategoryGameAccess = nameof(CategoryGameAccess);

    public const string CategoryLoggingAndMetrics = nameof(CategoryLoggingAndMetrics);

    public const string CategoryNetworkVisibility = nameof(CategoryNetworkVisibility);

    public const string CategorySecurity = nameof(CategorySecurity);

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
    public bool SmtpValid { get; private set; }

    #endregion Transient Properties

    #region Configuration Properties

    #region Game Core

    [Category(CategoryCore)]
    [JsonProperty(Order = -100)]
    [RequiresRestart]
    public string GameName { get; set; } = DefaultGameName;

    [Category(CategoryCore)]
    [JsonProperty(Order = -100)]
    [RequiresRestart]
    public ushort ServerPort { get; set; } = DefaultServerPort;

    #endregion Game Core

    #region Game Access

    [Category(CategoryGameAccess)]
    [JsonProperty(Order = -99)]
    public bool AdminOnly { get; set; }


    [Category(CategoryGameAccess)]
    [JsonProperty(Order = -99)]
    public bool BlockClientRegistrations { get; set; }


    [Category(CategoryGameAccess)]
    [JsonProperty(Order = -99)]
    public int MaxClientConnections { get; set; } = 100;

    /// <summary>
    /// Defines the maximum amount of logged-in users our server is allowed to handle.
    /// </summary>
    [Category(CategoryGameAccess)]
    [JsonProperty(Order = -99)]
    public int MaximumLoggedInUsers { get; set; } = 50;

    #endregion Game Access

    #region Network Visibility

    [Category(CategoryNetworkVisibility)]
    [JsonProperty(Order = -91)]
    [RequiresRestart]
    public bool UPnP { get; set; } = true;

    [Category(CategoryNetworkVisibility)]
    [JsonProperty(Order = -91)]
    [RequiresRestart]
    public bool OpenPortChecker { get; set; } = true;

    [Category(CategoryNetworkVisibility)]
    [JsonProperty(Order = -91, NullValueHandling = NullValueHandling.Include)]
    [RequiresRestart]
    public string? PortCheckerUrl { get; set; }

    #endregion Network Visibility

    #region Logging and Metrics

    [Category(CategoryLoggingAndMetrics)]
    [JsonProperty(Order = -80)]
    public LoggingOptions Logging { get; set; } = new();

    [Category(CategoryLoggingAndMetrics)]
    [JsonProperty(Order = -80)]
    public MetricsOptions Metrics { get; set; } = new();

    #endregion Logging and Metrics

    #region Database

    [Category(CategoryDatabase)]
    [JsonProperty(Order = -70)]
    [RequiresRestart]
    public DatabaseOptions GameDatabase { get; set; } = new();

    [Category(CategoryDatabase)]
    [JsonProperty(Order = -70)]
    [RequiresRestart]
    public DatabaseOptions LoggingDatabase { get; set; } = new();

    [Category(CategoryDatabase)]
    [JsonProperty(Order = -70)]
    [RequiresRestart]
    public DatabaseOptions PlayerDatabase { get; set; } = new();

    #endregion Database

    #region Security

    [Category(CategorySecurity)]
    [JsonProperty(Order = -60)]
    [RequiresRestart]
    public SecurityOptions Security { get; set; } = new();

    [Category(CategorySecurity)]
    [JsonProperty(Order = -60)]
    [RequiresRestart]
    public SmtpSettings SmtpSettings { get; set; } = new();

    #endregion Security

    #region Other Game Properties

    [RequiresRestart]
    public List<string> AnimatedSprites { get; set; } = [];

    [RequiresRestart]
    public PacketOptions Packets { get; set; } = new();

    public ChatOptions Chat { get; set; } = new();

    [RequiresRestart]
    public CombatOptions Combat { get; set; } = new();

    [RequiresRestart]
    public EquipmentOptions Equipment { get; set; } = new();

    [RequiresRestart]
    public int EventWatchdogKillThreshold { get; set; } = 5000;

    /// <summary>
    /// Passability configuration by map zone
    /// </summary>
    public PassabilityOptions Passability { get; set; } = new();

    public ushort ValidPasswordResetTimeMinutes { get; set; } = 30;

    public MapOptions Map { get; set; } = new();

    public PlayerOptions Player { get; set; } = new();

    public PartyOptions Party { get; set; } = new();

    public LootOptions Loot { get; set; } = new();

    public ProcessingOptions Processing { get; set; } = new();

    public SpriteOptions Sprites { get; set; } = new();

    public NpcOptions Npc { get; set; } = new();

    public QuestOptions Quest { get; set; } = new();

    public GuildOptions Guild { get; set; } = new();

    public BankOptions Bank { get; set; } = new();

    public InstancingOptions Instancing { get; set; } = new();

    public ItemOptions Items { get; set; } = new();

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
        var instance = EnsureCreated();

        var pathToServerConfig = Path.Combine(ResourcesDirectory, "config.json");
        if (!Directory.Exists(ResourcesDirectory))
        {
            Directory.CreateDirectory(ResourcesDirectory);
        }
        else if (File.Exists(pathToServerConfig))
        {
            var rawJson = File.ReadAllText(pathToServerConfig);
            instance = JsonConvert.DeserializeObject<Options>(rawJson, PrivateSerializerSettings) ?? instance;
            Instance = instance;
        }

        instance.SmtpValid = instance.SmtpSettings.IsValid();
        instance.FixAnimatedSprites();

        SaveToDisk();

        return true;
    }

    internal static Options EnsureCreated()
    {
        Options instance = new();
        Instance = instance;
        return instance;
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

        try
        {
            var serializedPrivateConfiguration = JsonConvert.SerializeObject(instance, PrivateIndentedSerializerSettings);
            File.WriteAllText(pathToServerConfig, serializedPrivateConfiguration);
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogError(
                exception,
                "Failed to save options to {OptionsPath}",
                pathToServerConfig
            );
        }

        instance.OptionsData = JsonConvert.SerializeObject(instance, PublicSerializerSettings);
    }

    public static void LoadFromServer(string data)
    {
        try
        {
            var loadedOptions = JsonConvert.DeserializeObject<Options>(data, PublicSerializerSettings);
            Instance = loadedOptions;
            OptionsLoaded?.Invoke(loadedOptions);
        }
        catch (Exception exception)
        {
            ApplicationContext.CurrentContext.Logger.LogError(exception, "Failed to load options from server");
            throw;
        }
    }

    public static event OptionsLoadedEventHandler? OptionsLoaded;

    public Options DeepClone() => JsonConvert.DeserializeObject<Options>(
        JsonConvert.SerializeObject(this, PrivateSerializerSettings)
    );
}