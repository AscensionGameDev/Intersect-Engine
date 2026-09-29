using System.Text.RegularExpressions;
using Intersect.Client.Core;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.General;
using Intersect.Network.Packets;

namespace Intersect.Client.Entities;

/// <summary>
/// Dofus-style sprite customisation, driven purely by file names in resources/entities:
/// <list type="bullet">
/// <item>"hero_c1.png", "hero_c2.png", ... are colour layers of "hero.png" (white/grey shapes), each tinted with a
/// colour the player picked.</item>
/// <item>"hero_hair_1.png", "hero_hair_2.png", ... are interchangeable parts of category "hair"; a part can have its own
/// colour layers "hero_hair_1_c1.png".</item>
/// </list>
/// Animation sheets follow along: "hero_attack.png" uses "hero_attack_c1.png" and "hero_attack_hair_1.png".
/// </summary>
public static class SpriteColorLayers
{
    public const int MaxLayers = 8;

    private static readonly Dictionary<string, IGameTexture[]> LayerCache = new(StringComparer.OrdinalIgnoreCase);

    private static readonly Dictionary<string, IReadOnlyList<SpritePartCategory>> CatalogCache = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>One customisable category of a sprite and how many variants exist for it.</summary>
    public sealed record SpritePartCategory(string Category, int MaxIndex);

    /// <summary>
    /// The colour layers of the entity texture with that name (e.g. "hero.png" or "hero_hair_1.png"), in order.
    /// </summary>
    public static IGameTexture[] GetLayers(string? textureName)
    {
        if (string.IsNullOrWhiteSpace(textureName))
        {
            return [];
        }

        if (LayerCache.TryGetValue(textureName, out var cached))
        {
            return cached;
        }

        var baseName = Path.GetFileNameWithoutExtension(textureName);
        var layers = new List<IGameTexture>();
        for (var index = 1; index <= MaxLayers; index++)
        {
            var layer = Globals.ContentManager.GetTexture(TextureType.Entity, $"{baseName}_c{index}.png");
            if (layer == null)
            {
                break;
            }

            layers.Add(layer);
        }

        var result = layers.ToArray();
        LayerCache[textureName] = result;

        return result;
    }

    public static int CountLayers(string? textureName) => GetLayers(textureName).Length;

    /// <summary>
    /// Texture name of a part variant for the given sheet ("hero.png", "hair", 2 → "hero_hair_2.png").
    /// </summary>
    public static string PartTextureName(string textureName, string category, int index) =>
        $"{Path.GetFileNameWithoutExtension(textureName)}_{category}_{index}.png";

    /// <summary>
    /// Part categories available for a sprite, in draw order, found by scanning the loaded entity textures.
    /// </summary>
    public static IReadOnlyList<SpritePartCategory> GetPartCatalog(string? spriteName)
    {
        if (string.IsNullOrWhiteSpace(spriteName))
        {
            return [];
        }

        if (CatalogCache.TryGetValue(spriteName, out var cached))
        {
            return cached;
        }

        var baseName = Path.GetFileNameWithoutExtension(spriteName);
        var pattern = new Regex($"^{Regex.Escape(baseName)}_([a-z]+)_([0-9]+)\\.png$", RegexOptions.IgnoreCase);
        var maxIndexByCategory = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in Globals.ContentManager.GetTextureNames(TextureType.Entity))
        {
            var match = pattern.Match(name);
            if (!match.Success || !int.TryParse(match.Groups[2].Value, out var index) || index <= 0)
            {
                continue;
            }

            var category = match.Groups[1].Value.ToLowerInvariant();
            maxIndexByCategory[category] = Math.Max(maxIndexByCategory.GetValueOrDefault(category), index);
        }

        var catalog = maxIndexByCategory
            .Select(pair => new SpritePartCategory(pair.Key, pair.Value))
            .OrderBy(category => CategoryOrder(category.Category))
            .ThenBy(category => category.Category, StringComparer.Ordinal)
            .ToArray();
        CatalogCache[spriteName] = catalog;

        return catalog;
    }

    private static int CategoryOrder(string category)
    {
        var order = Options.Instance.Player.SpritePartOrder;
        var index = order.FindIndex(entry => string.Equals(entry, category, StringComparison.OrdinalIgnoreCase));

        return index < 0 ? int.MaxValue : index;
    }

    /// <summary>
    /// Everything to draw on top of one frame of <paramref name="texture"/>, bottom to top: the sprite's own colour
    /// layers, then each part with its colour layers. Textures that don't match the sheet size are skipped.
    /// </summary>
    public static List<(IGameTexture Texture, Color Tint)> GetOverlays(IGameTexture texture, IReadOnlyList<Color> colors, IReadOnlyList<SpritePart> parts)
    {
        var overlays = new List<(IGameTexture, Color)>();
        AddLayers(overlays, texture, texture.Name, colors);

        foreach (var part in parts.OrderBy(part => CategoryOrder(part.Category)).ThenBy(part => part.Category, StringComparer.Ordinal))
        {
            if (part.Index <= 0)
            {
                continue;
            }

            var partName = PartTextureName(texture.Name, part.Category, part.Index);
            var partTexture = Globals.ContentManager.GetTexture(TextureType.Entity, partName);
            if (partTexture == null || !SameSize(partTexture, texture))
            {
                continue;
            }

            overlays.Add((partTexture, Color.White));
            AddLayers(overlays, texture, partName, part.Colors);
        }

        return overlays;
    }

    private static void AddLayers(List<(IGameTexture, Color)> overlays, IGameTexture sheet, string textureName, IReadOnlyList<Color> colors)
    {
        var layers = GetLayers(textureName);
        for (var index = 0; index < layers.Length && index < colors.Count; index++)
        {
            if (SameSize(layers[index], sheet))
            {
                overlays.Add((layers[index], colors[index]));
            }
        }
    }

    private static bool SameSize(IGameTexture a, IGameTexture b) => a.Width == b.Width && a.Height == b.Height;

    /// <summary>
    /// Draws the customisation overlays of <paramref name="texture"/> over the frame that was just drawn from it.
    /// </summary>
    public static void Draw(IGameTexture texture, FloatRect source, FloatRect destination, IReadOnlyList<Color> colors, IReadOnlyList<SpritePart> parts, Color renderColor)
    {
        if (colors.Count == 0 && parts.Count == 0)
        {
            return;
        }

        foreach (var (overlay, tint) in GetOverlays(texture, colors, parts))
        {
            Graphics.DrawGameTexture(overlay, source, destination, tint * renderColor);
        }
    }

    public static void ClearCache()
    {
        LayerCache.Clear();
        CatalogCache.Clear();
    }
}
