using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Framework.Core;
using Intersect.Framework.Core.GameObjects.Dungeons;
using Intersect.Network.Packets.Server;

namespace Intersect.Client.Interface.Game.Dungeons;

/// <summary>
/// Dungeon banner: the text (name, timer, lives, objective) is drawn straight by <see cref="Graphics"/> every frame
/// (see <see cref="Draw"/>), which is essentially free; only the "Abandon" button is a GWEN control.
/// </summary>
public partial class DungeonHud
{
    private const string FontName = "sourcesansproblack";

    private const int TopMargin = 8;

    private static readonly Color WarningColor = Color.FromArgb(255, 255, 80, 80);

    private static readonly Color TextColor = Color.White;

    private static readonly Color OutlineColor = Color.FromArgb(255, 0, 0, 0);

    private readonly Button _abandonButton;

    // Cached per second so the render loop never allocates.
    private DungeonStatusPacket? _lastStatus;

    private long _lastDisplayedSeconds = -1;

    private string _headerLine = string.Empty;

    private string _objectiveLine = string.Empty;

    private Color _headerColor = TextColor;

    private Color _objectiveColor = TextColor;

    public DungeonHud(Canvas gameCanvas)
    {
        _abandonButton = new Button(gameCanvas, "DungeonHudAbandon")
        {
            FontName = FontName,
            FontSize = 10,
            Alignment = [Alignments.Top, Alignments.CenterH],
            AlignmentPadding = new Padding(0, TopMargin + 48, 0, 0),
        };
        _abandonButton.SetText(Strings.Dungeons.Abandon);
        _abandonButton.SetSize(90, 22);
        _abandonButton.Clicked += AbandonButton_Clicked;
        _abandonButton.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer?.GetResolutionString());
        _abandonButton.Hide();
    }

    private void AbandonButton_Clicked(Base sender, MouseButtonState arguments)
    {
        PacketSender.SendLeaveDungeon();
    }

    /// <summary>
    /// Called once per frame from the game interface: keeps the cached strings and the button state up to date.
    /// </summary>
    public void Update()
    {
        var status = Globals.ActiveDungeon;
        if (status == null || DungeonDescriptor.Get(status.DungeonId) is not { } dungeon)
        {
            _lastStatus = null;
            if (!_abandonButton.IsHidden)
            {
                _abandonButton.Hide();
            }

            return;
        }

        var running = status.State == DungeonRunState.Running;
        if (_abandonButton.IsHidden == running)
        {
            _abandonButton.IsHidden = !running;
        }

        var sinceUpdate = Timing.Global.Milliseconds - Globals.ActiveDungeonReceivedAt;
        long displayedMs;
        var headerColor = TextColor;
        if (status.RemainingMs >= 0)
        {
            displayedMs = running ? Math.Max(0, status.RemainingMs - sinceUpdate) : status.RemainingMs;
            if (running && displayedMs <= 120_000)
            {
                headerColor = WarningColor;
            }
        }
        else
        {
            displayedMs = running ? status.ElapsedMs + sinceUpdate : status.ElapsedMs;
        }

        var displayedSeconds = displayedMs / 1000;
        if (ReferenceEquals(_lastStatus, status) && displayedSeconds == _lastDisplayedSeconds)
        {
            return;
        }

        _lastStatus = status;
        _lastDisplayedSeconds = displayedSeconds;
        _headerColor = headerColor;

        var lives = status.MaxLives > 0 ? Strings.Dungeons.HudLives.ToString(status.Lives, status.MaxLives) : string.Empty;
        _headerLine = string.IsNullOrEmpty(lives)
            ? $"{dungeon.Name}   {DungeonsWindow.FormatTime(displayedMs)}"
            : $"{dungeon.Name}   {DungeonsWindow.FormatTime(displayedMs)}   {lives}";

        _objectiveLine = status.State switch
        {
            DungeonRunState.Completed => Strings.Dungeons.HudCompleted.ToString(),
            DungeonRunState.Failed => Strings.Dungeons.HudFailed.ToString(),
            _ => string.IsNullOrWhiteSpace(status.Objective) ? Strings.Dungeons.HudNoObjective.ToString() : status.Objective,
        };
        _objectiveColor = status.State == DungeonRunState.Failed ? WarningColor : TextColor;
    }

    /// <summary>
    /// Draws the banner text in screen space. Called by <see cref="Graphics"/> after the GUI.
    /// </summary>
    public void Draw()
    {
        if (_lastStatus == null || Graphics.Renderer is not { } renderer)
        {
            return;
        }

        var font = Graphics.UIFont ?? Graphics.EntityNameFont;
        var headerSize = Graphics.UIFontSize + 4;
        var objectiveSize = Graphics.UIFontSize + 1;

        var headerWidth = renderer.MeasureText(_headerLine, font, headerSize, 1f).X;
        var objectiveWidth = renderer.MeasureText(_objectiveLine, font, objectiveSize, 1f).X;
        var centerX = renderer.ScreenWidth / 2f;

        renderer.DrawString(_headerLine, font, headerSize, centerX - headerWidth / 2f, TopMargin, 1f, _headerColor, false, null, OutlineColor);
        renderer.DrawString(_objectiveLine, font, objectiveSize, centerX - objectiveWidth / 2f, TopMargin + 24, 1f, _objectiveColor, false, null, OutlineColor);
    }
}
