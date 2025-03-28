using System.Runtime.Serialization;
using Intersect.Framework.Annotations;

namespace Intersect.Config;

[RequiresRestart]
public partial class InstancingOptions
{
    /// <summary>
    ///  Intersect default for instance lives
    /// </summary>
    public const int DefaultInstanceLives = 3;

    /// <summary>
    /// Whether dying in a shared instance "respawns" you at the instance entrance. Useful for dungeon implementations
    /// </summary>
    public bool SharedInstanceRespawnInInstance { get; set; } = true;

    /// <summary>
    /// Whether a player that leaves a shared instance can come back in to that instance after leaving.
    /// </summary>
    public bool RejoinableSharedInstances { get; set; } = false;

    /// <summary>
    /// How many lives a party has in a shared instance, if enabled
    /// </summary>
    public int MaxSharedInstanceLives { get; set; } = DefaultInstanceLives;

    /// <summary>
    /// Whether all party members get booted out of an instance on lives reaching -1
    /// </summary>
    public bool BootAllFromInstanceWhenOutOfLives { get; set; } = true;

    /// <summary>
    /// Whether you lose experience on death in a shared instance
    /// </summary>
    public bool LoseExpOnInstanceDeath { get; set; } = false;

    public bool BlockPartyInvitesInInstance { get; set; } = false;

    public bool KickPartyMembersOnKick { get; set; } = true;

    [OnDeserialized]
    internal void OnDeserializedMethod(StreamingContext context)
    {
        Validate();
    }

    private void Validate()
    {
    }
}
