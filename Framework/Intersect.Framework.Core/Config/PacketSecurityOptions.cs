using Newtonsoft.Json;

namespace Intersect.Config;

public partial class PacketSecurityOptions
{
    #region "Packet Sanitization and Hacking Detection Options"

    /// <summary>
    /// Assumed minimum ping a client will have when communicating with the server.
    /// </summary>
    public int MinimumPing { get; set; } = 10;

    /// <summary>
    /// This factor is multiplied by the client ping in order to determine the acceptable error margin in packet timing.
    /// </summary>
    public float ErrorMarginFactor { get; set; } = 0.25f;

    /// <summary>
    /// Lower bounds of adjusted packet times with pings taken into account for packets to be considered natural.
    /// </summary>
    public int NaturalLowerMargin { get; set; } = 0;

    /// <summary>
    /// Upper bounds of adjusted packet times with pings taken into account for packets to be considered natural.
    /// </summary>
    public int NaturalUpperMargin { get; set; } = 500;

    /// <summary>
    /// Number of consecutive unnatural packets that are accepted in short intervals before being dropped.
    /// </summary>
    public int AllowedSpikePackets { get; set; } = 5;

    /// <summary>
    /// The base amount of time in ms that we will forgive the client for being desyned
    /// </summary>
    public int BaseDesyncForegiveness { get; set; } = 400;

    /// <summary>
    /// A factor in whch we will allow the client time to shift from the servers before dropping packets with configured error margins taken into account.
    /// </summary>
    public float DesyncForgivenessFactor { get; set; } = 2.0f;

    /// <summary>
    /// A value measured in milliseconds in which a packet that is out of sync will be forgiven (no matter how out of sync) and timing will sent to the client.
    /// </summary>
    public int DesyncForgivenessInterval { get; set; } = 5000;

    #endregion

    #region "Packet Flooding Thresholds"

    #region Obsolete remove in 0.9-beta

    [JsonProperty]
    [Obsolete($"Use {nameof(EditorThresholds)} instead, the EditorFloodThreshholds property will be removed in 0.9-beta", error: true)]
    private FloodThresholdOptions EditorFloodThreshholds
    {
        set => EditorThresholds = value;
    }

    [JsonProperty]
    [Obsolete($"Use {nameof(PlayerThresholds)} instead, the PlayerFloodThreshholds property will be removed in 0.9-beta", error: true)]
    private FloodThresholdOptions PlayerFloodThreshholds
    {
        set => PlayerThresholds = value;
    }

    [JsonProperty]
    [Obsolete($"Use {nameof(ModAdminThresholds)} instead, the ModAdminFloodThreshholds property will be removed in 0.9-beta", error: true)]
    private FloodThresholdOptions ModAdminFloodThreshholds
    {
        set => ModAdminThresholds = value;
    }

    [JsonProperty]
    [Obsolete($"Use {nameof(DefaultThresholds)} instead, the FloodThreshholds property will be removed in 0.9-beta", error: true)]
    private FloodThresholdOptions FloodThreshholds
    {
        set => DefaultThresholds = value;
    }

    #endregion Obsolete remove in 0.9-beta

    /// <summary>
    /// Packet flooding detection thresholds for the game editor. (No Restrictions.)
    /// </summary>
    public FloodThresholdOptions EditorThresholds { get; set; } = FloodThresholdOptions.Editor();

    /// <summary>
    /// Packet flooding detection thresholds for general players. Pretty strict.
    /// Might need to be adjusted if there is a lot of high paced actions/movement/combat in your game.
    /// </summary>
    public FloodThresholdOptions PlayerThresholds { get; set; } = new();

    /// <summary>
    /// Packet flooding detection thresholds for mods/admins. Hopefully you trust these guys.
    /// Limits need to be higher than general players since they can warp quickly around the world with shift click.
    /// </summary>
    public FloodThresholdOptions ModAdminThresholds { get; set; } = FloodThresholdOptions.Editor();

    /// <summary>
    /// Packet flooding detection threshholds for all users who are not yet logged in.
    /// </summary>
    public FloodThresholdOptions DefaultThresholds { get; set; } = FloodThresholdOptions.NotLoggedIn();

    #endregion
}