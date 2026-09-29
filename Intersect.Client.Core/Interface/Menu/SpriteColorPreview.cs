using Intersect.Client.Entities;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Network.Packets;

namespace Intersect.Client.Interface.Menu;

/// <summary>
/// Stacks the customisation overlays of a sprite (tinted colour layers, parts) on top of the <see cref="ImagePanel"/>
/// that shows the sprite itself (character selection and creation previews). The overlays are children of that panel
/// so they follow it.
/// </summary>
internal static class SpriteColorPreview
{
    private const string LayerName = "SpriteColorLayer";

    public static void Apply(ImagePanel spritePanel, IReadOnlyList<Color> colors, IReadOnlyList<SpritePart> parts, int frameWidth, int frameHeight)
    {
        var existing = spritePanel.Children.OfType<ImagePanel>().Where(child => child.Name == LayerName).ToList();
        var overlays = spritePanel.Texture == null ? [] : SpriteColorLayers.GetOverlays(spritePanel.Texture, colors, parts);

        for (var index = 0; index < overlays.Count; index++)
        {
            ImagePanel layerPanel;
            if (index < existing.Count)
            {
                layerPanel = existing[index];
            }
            else
            {
                layerPanel = new ImagePanel(spritePanel, LayerName) { MouseInputEnabled = false };
                existing.Add(layerPanel);
            }

            layerPanel.Texture = overlays[index].Texture;
            layerPanel.SetTextureRect(0, 0, frameWidth, frameHeight);
            layerPanel.SetBounds(0, 0, frameWidth, frameHeight);
            layerPanel.RenderColor = overlays[index].Tint;
            layerPanel.Show();
        }

        for (var index = overlays.Count; index < existing.Count; index++)
        {
            existing[index].Texture = default;
            existing[index].Hide();
        }
    }

    public static void Clear(ImagePanel spritePanel)
    {
        foreach (var child in spritePanel.Children.OfType<ImagePanel>().Where(child => child.Name == LayerName))
        {
            child.Texture = default;
            child.Hide();
        }
    }
}
