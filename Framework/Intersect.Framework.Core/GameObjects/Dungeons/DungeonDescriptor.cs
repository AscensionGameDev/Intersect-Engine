using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Enums;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Dungeons;

/// <summary>
/// Describes an instanced dungeon: where it starts, who can enter, how long it lasts, how many lives the group
/// has, how often it can be run and what everyone gets for finishing it. The dungeon's actual script (bosses,
/// doors, objectives) is driven by regular map events using the dungeon event commands.
/// </summary>
public partial class DungeonDescriptor : DatabaseObject<DungeonDescriptor>, IFolderable
{
    [JsonConstructor]
    public DungeonDescriptor(Guid id) : base(id)
    {
        Name = "New Dungeon";
    }

    //EF wants NO PARAMETERS!!!!!
    public DungeonDescriptor()
    {
        Name = "New Dungeon";
    }

    /// <summary>Shown in the client's dungeon window.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Optional picture (resources/misc) shown in the client's dungeon window.</summary>
    public string Icon { get; set; } = string.Empty;

    public DungeonAccessType AccessType { get; set; } = DungeonAccessType.Party;

    // Where participants are put when the run starts (and where they respawn while it lasts).
    public Guid EntryMapId { get; set; }

    public int EntryX { get; set; }

    public int EntryY { get; set; }

    public Direction EntryDirection { get; set; } = Direction.Down;

    /// <summary>
    /// When enabled, a fresh room-and-corridor layout is generated on every run instead of using
    /// <see cref="EntryMapId"/> as the dungeon map. <see cref="EntryMapId"/>/<see cref="EntryX"/>/<see cref="EntryY"/>/
    /// <see cref="EntryDirection"/> then instead describe the point participants are sent back to when they leave
    /// the generated dungeon.
    /// </summary>
    public bool ProceduralGeneration { get; set; }

    public Guid FloorTilesetId { get; set; }

    public int FloorTileX { get; set; }

    public int FloorTileY { get; set; }

    /// <summary>When set, <see cref="FloorTileX"/>/<see cref="FloorTileY"/> is treated as the top-left corner of an
    /// autotile block (same convention as painting an autotile in the map editor) instead of a plain single tile,
    /// so floor tiles blend at their edges instead of repeating identically everywhere.</summary>
    public bool FloorTileIsAutotile { get; set; }

    public Guid WallTilesetId { get; set; }

    public int WallTileX { get; set; }

    public int WallTileY { get; set; }

    /// <summary>Same as <see cref="FloorTileIsAutotile"/> but for the wall tile.</summary>
    public bool WallTileIsAutotile { get; set; }

    public int MinRooms { get; set; } = 6;

    public int MaxRooms { get; set; } = 10;

    public int MinRoomSize { get; set; } = 5;

    public int MaxRoomSize { get; set; } = 9;

    /// <summary>NPCs to randomly pick from when populating generated rooms (not the entry or boss room).</summary>
    [NotMapped]
    public List<Guid> MonsterPoolNpcIds { get; set; } = [];

    [Column("MonsterPoolNpcIds")]
    [JsonIgnore]
    public string MonsterPoolNpcIdsJson
    {
        get => JsonConvert.SerializeObject(MonsterPoolNpcIds);
        set => MonsterPoolNpcIds = JsonConvert.DeserializeObject<List<Guid>>(value ?? "[]") ?? [];
    }

    public int MinNpcsPerRoom { get; set; } = 1;

    public int MaxNpcsPerRoom { get; set; } = 3;

    /// <summary>NPC spawned in the last generated room, empty = no boss.</summary>
    public Guid BossNpcId { get; set; } = Guid.Empty;

    /// <summary>Minimum party size for <see cref="DungeonAccessType.Party"/> dungeons (1 = can be soloed).</summary>
    public int MinPlayers { get; set; } = 1;

    /// <summary>Maximum party size for <see cref="DungeonAccessType.Party"/> dungeons (0 = no limit).</summary>
    public int MaxPlayers { get; set; } = 4;

    public int RequiredLevel { get; set; } = 1;

    /// <summary>Seconds before the run fails, 0 = no time limit.</summary>
    public int TimeLimitSeconds { get; set; } = 0;

    /// <summary>Deaths the group can afford before the run fails, 0 = unlimited.</summary>
    public int Lives { get; set; } = 3;

    /// <summary>Seconds a player has to wait after starting a run before starting this dungeon again, 0 = none.</summary>
    public int CooldownSeconds { get; set; } = 0;

    /// <summary>Seconds participants stay inside after completion/failure before being sent back out.</summary>
    public int ExitDelaySeconds { get; set; } = 8;

    public long RewardExperience { get; set; }

    /// <summary>Title unlocked on completion, empty = none.</summary>
    public Guid RewardTitleId { get; set; } = Guid.Empty;

    /// <summary>Achievement unlocked on completion, empty = none.</summary>
    public Guid RewardAchievementId { get; set; } = Guid.Empty;

    [NotMapped]
    public List<DungeonReward> Rewards { get; set; } = [];

    [Column("Rewards")]
    [JsonIgnore]
    public string RewardsJson
    {
        get => JsonConvert.SerializeObject(Rewards);
        set => Rewards = JsonConvert.DeserializeObject<List<DungeonReward>>(value ?? "[]") ?? [];
    }

    /// <inheritdoc />
    public string Folder { get; set; } = string.Empty;
}
