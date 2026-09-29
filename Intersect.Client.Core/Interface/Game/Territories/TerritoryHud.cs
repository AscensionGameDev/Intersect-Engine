using Intersect.Client.Core;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Framework.Core.GameObjects.Factions;
using Intersect.Framework.Core.GameObjects.Territories;
using Intersect.Network.Packets.Server;

namespace Intersect.Client.Interface.Game.Territories;

/// <summary>
/// Small banner shown while standing on a map that has a capturable territory: who holds it, and progress
/// toward flipping it if someone is currently capturing. Drawn straight by <see cref="Graphics"/> every frame
/// (see <see cref="Draw"/>), same lightweight approach as <see cref="Dungeons.DungeonHud"/>.
/// </summary>
public partial class TerritoryHud
{
    private const int TopMargin = 40;

    private static readonly Color TextColor = Color.White;

    private static readonly Color OutlineColor = Color.FromArgb(255, 0, 0, 0);

    /// <summary>Latest status received per territory id.</summary>
    private static readonly Dictionary<Guid, TerritoryStatusPacket> LastStatusByTerritory = new();

    private string _ownerLine = string.Empty;

    private string _progressLine = string.Empty;

    private Color _ownerColor = TextColor;

    public static void OnStatus(TerritoryStatusPacket packet) => LastStatusByTerritory[packet.TerritoryId] = packet;

    public void Update()
    {
        _ownerLine = string.Empty;
        _progressLine = string.Empty;

        if (Globals.Me == null)
        {
            return;
        }

        var territory = TerritoryDescriptor.Lookup.Values
            .OfType<TerritoryDescriptor>()
            .FirstOrDefault(candidate => candidate.MapId == Globals.Me.MapId);

        if (territory == null || !LastStatusByTerritory.TryGetValue(territory.Id, out var status))
        {
            return;
        }

        var ownerFaction = status.ControllingFactionId == Guid.Empty ? null : FactionDescriptor.Get(status.ControllingFactionId);
        _ownerColor = ownerFaction?.Color ?? TextColor;
        _ownerLine = ownerFaction == null
            ? Strings.Territories.Uncontrolled.ToString(territory.Name)
            : Strings.Territories.ControlledBy.ToString(territory.Name, ownerFaction.Name);

        if (status.Contested)
        {
            _progressLine = Strings.Territories.Contested.ToString();
        }
        else if (status.CapturingFactionId != Guid.Empty && FactionDescriptor.Get(status.CapturingFactionId) is { } capturingFaction)
        {
            _progressLine = Strings.Territories.Capturing.ToString(
                capturingFaction.Name,
                status.CaptureProgressSeconds,
                territory.CaptureTimeSeconds
            );
        }
    }

    /// <summary>Draws the banner text in screen space. Called by <see cref="Graphics"/> after the GUI.</summary>
    public void Draw()
    {
        if (string.IsNullOrEmpty(_ownerLine) || Graphics.Renderer is not { } renderer)
        {
            return;
        }

        var font = Graphics.UIFont ?? Graphics.EntityNameFont;
        var ownerSize = Graphics.UIFontSize + 2;
        var progressSize = Graphics.UIFontSize;

        var ownerWidth = renderer.MeasureText(_ownerLine, font, ownerSize, 1f).X;
        var centerX = renderer.ScreenWidth / 2f;

        renderer.DrawString(_ownerLine, font, ownerSize, centerX - ownerWidth / 2f, TopMargin, 1f, _ownerColor, false, null, OutlineColor);

        if (!string.IsNullOrEmpty(_progressLine))
        {
            var progressWidth = renderer.MeasureText(_progressLine, font, progressSize, 1f).X;
            renderer.DrawString(_progressLine, font, progressSize, centerX - progressWidth / 2f, TopMargin + 20, 1f, TextColor, false, null, OutlineColor);
        }
    }
}
