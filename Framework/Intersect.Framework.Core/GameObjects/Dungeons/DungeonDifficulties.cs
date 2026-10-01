namespace Intersect.Framework.Core.GameObjects.Dungeons;

/// <summary>
/// The three difficulties every dungeon can be run at. A difficulty sets the level every monster of the run is scaled
/// to (through the engine's NPC level scaling) and multiplies the experience and item rewards.
/// </summary>
public static class DungeonDifficulties
{
    public const byte Easy = 0;

    public const byte Normal = 1;

    public const byte Hard = 2;

    public const int Count = 3;

    /// <summary>Any unknown value falls back to <see cref="Easy"/>.</summary>
    public static byte Clamp(byte difficulty) => difficulty < Count ? difficulty : Easy;

    /// <summary>Level the monsters of the run are scaled to: Easy 10, Normal 30, Hard 60.</summary>
    public static int MonsterLevel(byte difficulty) => Clamp(difficulty) switch
    {
        Normal => 30,
        Hard => 60,
        _ => 10,
    };

    public static int ExperienceMultiplier(byte difficulty) => Clamp(difficulty) switch
    {
        Normal => 3,
        Hard => 6,
        _ => 1,
    };

    public static int ItemMultiplier(byte difficulty) => Clamp(difficulty) switch
    {
        Normal => 2,
        Hard => 3,
        _ => 1,
    };

    /// <summary>
    /// Records, cooldowns and leaderboards are kept per dungeon AND difficulty. Easy keeps the dungeon's own id (so
    /// existing records stay valid); the other difficulties use a stable id derived from it.
    /// </summary>
    public static Guid RecordKey(Guid dungeonId, byte difficulty)
    {
        difficulty = Clamp(difficulty);
        if (difficulty == Easy)
        {
            return dungeonId;
        }

        var bytes = dungeonId.ToByteArray();
        bytes[0] ^= (byte)(0x5A * difficulty);
        bytes[7] ^= (byte)(0xA5 * difficulty);
        bytes[15] ^= (byte)(0x3C * difficulty);

        return new Guid(bytes);
    }
}
