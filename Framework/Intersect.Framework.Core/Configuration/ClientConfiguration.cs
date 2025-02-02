using System.ComponentModel;
using System.Runtime.Serialization;
using Intersect.Enums;
using Intersect.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Intersect.Configuration;

/// <inheritdoc />
/// <summary>
/// Client configuration options
/// </summary>
public sealed partial class ClientConfiguration : IConfiguration<ClientConfiguration>
{
    public ClientConfiguration()
    {
        ConfigurationHelper.CacheName = $"{Host}.{Port}";
    }

    public static string ResourcesDirectory { get; set; } = "resources";

    public static string DefaultPath => Path.Combine(ResourcesDirectory, "config.json");

    /// <inheritdoc />
    public ClientConfiguration Load(string? filePath = default, bool failQuietly = false) =>
        ConfigurationHelper.Load(this, filePath ?? DefaultPath, failQuietly);

    /// <inheritdoc />
    public ClientConfiguration Save(string? filePath = default, bool failQuietly = false) =>
        ConfigurationHelper.Save(this, filePath ?? DefaultPath, failQuietly);

    public static ClientConfiguration LoadAndSave(string? filePath = default) =>
        ConfigurationHelper.LoadSafely(Instance, filePath ?? DefaultPath);

    #region Constants

    public const string DefaultHost = "localhost";

    public const int DefaultPort = 5400;

    public static List<DisplayDirection> DefaultEntityBarDirections =>
        Enumerable
            .Range(0, 1 + Enum.GetValues<Vital>().Length)
            .Select(_ => DisplayDirection.StartToEnd)
            .ToList();

    public const string DefaultFont = "sourcesansproblack";

    public const string DefaultUIFont = "sourcesanspro,10";

    public const int DefaultChatLines = 100;

    public const float DefaultFadeDurationMs = 3000f;

    public const DisplayMode DefaultMenuBackgroundDisplayMode = DisplayMode.Default;

    public const long DefaultMenuBackgroundFrameInterval = 50;

    public const string DefaultMenuMusic = "RPG-Theme_v001_Looping.ogg";

    public const bool DefaultTypewriterEnabled = true;

    public static List<char> DefaultTypewriterFullStopCharacters =>
    [
        '.',
        '!',
        '?',
        ':',
    ];

    public const long DefaultTypewriterFullStopDelay = 400;

    public const long DefaultTypewriterPartDelay = 6;

    public static List<char> DefaultTypewriterPauseCharacters =>
    [
        ',',
        ';',
        '-',
    ];

    public const long DefaultTypewriterPauseDelay = 80;

    public const long DefaultTypewriterResponseDelay = 600;

    public const int DefaultTypewriterSoundFrequency = 5;

    public static List<string> DefaultTypewriterSounds => ["octave-beep-tapped.wav"];

    #endregion

    #region Static Properties and Methods

    public static ClientConfiguration Instance { get; } = new();

    public void Validate()
    {
        ChatLines = MathHelper.Clamp(ChatLines, 10, 500);

        EntityBarDirections = DefaultEntityBarDirections.Select(
                (direction, index) => EntityBarDirections.Count > index ? EntityBarDirections[index] : direction
            )
            .ToList();

        GameFont = string.IsNullOrWhiteSpace(GameFont) ? DefaultFont : GameFont.Trim();
        Host = string.IsNullOrWhiteSpace(Host) ? DefaultHost : Host.Trim();
        IntroImages = [..IntroImages?.Distinct() ?? new List<string>()];
        MenuBackground =
        [
            ..MenuBackground?.Distinct() ??
              new List<string>
              {
                  "background.png"
              },
        ];
        Port = Math.Min(Math.Max(Port, (ushort)1), ushort.MaxValue);
        TypewriterFullStopCharacters = TypewriterFullStopCharacters?.Distinct()?.ToList() ?? [];
        TypewriterPauseCharacters = TypewriterPauseCharacters?.Distinct()?.ToList() ?? [];
        TypewriterSounds = [..TypewriterSounds?.Distinct() ?? new List<string>()];
        UIFont = string.IsNullOrWhiteSpace(UIFont) ? DefaultUIFont : UIFont.Trim();
    }

    #endregion

    #region Options

    /// <summary>
    /// Hostname of the server to connect to
    /// </summary>
    public string Host { get; set; } = DefaultHost;

    /// <summary>
    /// Port of the server to connect to
    /// </summary>
    public ushort Port { get; set; } = DefaultPort;

    /// <summary>
    /// The font family to use on misc non-ui rendering
    /// </summary>
    public string GameFont { get; set; } = DefaultFont;

    /// <summary>
    /// The font family to use on entity names
    /// </summary>
    public string EntityNameFont { get; set; } = DefaultFont;

    /// <summary>
    /// The font family to use on chat bubbles
    /// </summary>
    public string ChatBubbleFont { get; set; } = DefaultFont;

    /// <summary>
    /// The font family to use on action messages
    /// </summary>
    public string ActionMsgFont { get; set; } = DefaultFont;

    /// <summary>
    /// The font family to use on unstyled windows such as the debug menu/admin window
    /// </summary>
    public string UIFont { get; set; } = DefaultUIFont;

    /// <summary>
    /// Number of lines to save for chat scrollback
    /// </summary>
    public int ChatLines { get; set; } = DefaultChatLines;

    /// <summary>
    /// Menu music file name
    /// </summary>
    public string MenuMusic { get; set; } = DefaultMenuMusic;

    /// <summary>
    /// Sets the main menu's background texture, if the the index of the list is bigger than 1,
    /// the background will be animated by sequentially drawing the texture files from the list.
    /// Static background Example: { "background.png" },
    /// Animated background Example: { "background_0.png", "background_1.png", "background_2.png" },
    /// </summary>
    public List<string> MenuBackground { get; set; } = ["background.png"];

    /// <summary>
    /// Sets the display mode of the main menu's background.
    /// </summary>
    public DisplayMode MenuBackgroundDisplayMode { get; set; } = DefaultMenuBackgroundDisplayMode;

    /// <summary>
    /// Sets the frames interval (milliseconds) of the main menu's animated background.
    /// </summary>
    public long MenuBackgroundFrameInterval { get; set; } = DefaultMenuBackgroundFrameInterval;

    /// <summary>
    /// A list from which introductory images are drawn when the game client is launched.
    /// </summary>
    public List<string> IntroImages { get; set; } = [];

    /// <summary>
    /// The address for the update manifest file generated by the editor.
    /// </summary>
    public string UpdateUrl { get; set; } = string.Empty;

    /// <summary>
    /// Sets a custom mouse cursor.
    /// </summary>
    public string MouseCursor { get; set; } = string.Empty;

    /// <summary>
    /// Determines the time it takes to fade-in or fade-out a song when no other instructions are given.
    /// </summary>
    public int MusicFadeTimer { get; set; } = 1500;

    /// <summary>
    /// The default duration of fades. Controls duration of non-event fades like on login/logout
    /// </summary>
    public float FadeDurationMs { get; set; } = DefaultFadeDurationMs;

    /// <summary>
    /// Configures whether or not the context menus are enabled upon right-clicking certain elements.
    /// </summary>
    public bool EnableContextMenus { get; set; } = true;

    /// <summary>
    /// Configures whether the highlighted even rows of list elements should be marked differently or not.
    /// </summary>
    public bool EnableZebraStripedRows { get; set; } = true;

    /// <summary>
    /// Configures the name of the skin or skin texture (must end in .png) to use.
    /// </summary>
    public string UiSkin { get; set; } = "IntersectSkin";

    /// <summary>
    /// Configures the rendering direction of entity bars, vitals in their order, then experience
    /// </summary>
    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<DisplayDirection> EntityBarDirections { get; set; } =
        Enumerable
            .Range(0, 1 + Enum.GetValues<Vital>().Length)
            .Select(_ => DisplayDirection.StartToEnd)
            .ToList();

    public bool TypewriterEnabled { get; set; } = DefaultTypewriterEnabled;

    public List<char> TypewriterFullStopCharacters { get; set; } = DefaultTypewriterFullStopCharacters;

    public long TypewriterFullStopDelay { get; set; } = DefaultTypewriterFullStopDelay;

    public long TypewriterPartDelay { get; set; } = DefaultTypewriterPartDelay;

    public List<char> TypewriterPauseCharacters { get; set; } = DefaultTypewriterPauseCharacters;

    public long TypewriterPauseDelay { get; set; } = DefaultTypewriterPauseDelay;

    public long TypewriterResponseDelay { get; set; } = DefaultTypewriterResponseDelay;

    public int TypewriterSoundFrequency { get; set; } = DefaultTypewriterSoundFrequency;

    public List<string> TypewriterSounds { get; set; } = DefaultTypewriterSounds;

    #region Hidden Properties

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    [DefaultValue(LogLevel.Information)]
    public LogLevel LogLevel { get; set; } = LogLevel.Information;

    #endregion

    #endregion

    #region Serialization Hooks

    [OnDeserializing]
    internal void OnDeserializing(StreamingContext context)
    {
        MenuBackground?.Clear();
        IntroImages?.Clear();
    }

    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
        Validate();
    }

    #endregion
}
