using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game;

/// <summary>
/// The GUI class for the Announcement Window that can pop up on-screen during gameplay.
/// </summary>
public partial class AnnouncementWindow : ImagePanel
{
    private readonly Label _label;

    private string _labelText = string.Empty;
    private long _duration = 0;

    /// <summary>
    /// Create a new instance of the <see cref="AnnouncementWindow"/> class.
    /// </summary>
    /// <param name="gameCanvas">The <see cref="Canvas"/> to render this control on.</param>
    public AnnouncementWindow(Canvas gameCanvas) : base(gameCanvas, nameof(AnnouncementWindow))
    {
        _label = new Label(this, "AnnouncementLabel");
        LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer?.GetResolutionString());
    }

    /// <summary>
    /// Update this control...
    /// </summary>
    public void Update()
    {
        if (IsHidden)
        {
            return;
        }

        if(_label.Text != _labelText)
        {
            _label.Text = _labelText;
        }

        // Are we still supposed to be visible?
        if (Timing.Global.Milliseconds > _duration)
        {
            Hide();
        }
    }

    /// <summary>
    /// Display an announcement.
    /// </summary>
    /// <param name="announcementText">The text to display.</param>
    /// <param name="displayTime">The time for which to display the announcement.</param>
    public void ShowAnnouncement(string announcementText, long displayTime)
    {
        _labelText = announcementText;
        _duration = Timing.Global.Milliseconds + displayTime;
        Show();
    }
}