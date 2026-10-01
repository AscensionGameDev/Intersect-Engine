namespace Intersect.Framework.Core.GameObjects.Crafting;

/// <summary>
/// The trade skills of the game (see the "Artisanat" section of the game bible). Gathering professions are free,
/// crafting professions are limited per player (<see cref="Professions.MaxCraftingProfessions"/>).
/// </summary>
public enum Profession : byte
{
    None = 0,

    // Gathering
    Mining = 1,
    Herbalism = 2,
    Skinning = 3,
    Fishing = 4,
    AetherShards = 5,

    // Crafting
    Blacksmithing = 6,
    Leatherworking = 7,
    Alchemy = 8,
    Enchanting = 9,
    Cooking = 10,
    Jewelcrafting = 11,
}

public static class Professions
{
    public const int MaxSkill = 300;

    public const int MaxCraftingProfessions = 2;

    /// <summary>Highest skill reachable at each rank (Apprenti, Compagnon, Expert, Artisan).</summary>
    public static readonly int[] RankCaps = [75, 150, 225, 300];

    private static readonly string[] RankNames = ["Apprenti", "Compagnon", "Expert", "Artisan"];

    public static IEnumerable<Profession> All => Enum.GetValues<Profession>().Where(profession => profession != Profession.None);

    public static bool IsCrafting(Profession profession) => profession >= Profession.Blacksmithing;

    public static string GetName(Profession profession) => profession switch
    {
        Profession.Mining => "Minage",
        Profession.Herbalism => "Herboristerie",
        Profession.Skinning => "Écorchage",
        Profession.Fishing => "Pêche",
        Profession.AetherShards => "Collecte d'Éclats",
        Profession.Blacksmithing => "Forge",
        Profession.Leatherworking => "Maroquinerie",
        Profession.Alchemy => "Alchimie",
        Profession.Enchanting => "Enchantement",
        Profession.Cooking => "Cuisine",
        Profession.Jewelcrafting => "Joaillerie",
        _ => "Aucun",
    };

    public static string GetRankName(int skill)
    {
        for (var index = 0; index < RankCaps.Length; index++)
        {
            if (skill <= RankCaps[index])
            {
                return RankNames[index];
            }
        }

        return RankNames[^1];
    }

    /// <summary>The highest skill the player can reach with the rank unlocked by their trainer (all ranks until trainers exist).</summary>
    public static int ClampSkill(int skill) => Math.Clamp(skill, 0, MaxSkill);
}

/// <summary>WoW-style recipe difficulty: orange always raises the skill, yellow often, green rarely, grey never.</summary>
public enum RecipeDifficulty : byte
{
    Orange = 0,
    Yellow = 1,
    Green = 2,
    Grey = 3,
}
