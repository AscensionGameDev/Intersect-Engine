namespace Intersect.Framework.Core.GameObjects.Dungeons;

/// <summary>
/// Who shares a dungeon run.
/// </summary>
public enum DungeonAccessType
{
    /// <summary>Each player gets their own instance.</summary>
    Solo = 0,

    /// <summary>The party leader starts the run and the whole party shares one instance.</summary>
    Party,

    /// <summary>Guild members share one instance.</summary>
    Guild,
}
