using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;

using Intersect.Utilities;

namespace Intersect.Client.Interface.Game
{
    /// <summary>
    /// The GUI class for the Announcement Window that can pop up on-screen during gameplay.
    /// </summary>
    public class AnnouncementWindow
    {

        //Controls
        private Canvas mGameCanvas;

        private ImagePanel mPicture;

        private Label mLabel;

        private string mLabelText;

        private long mDisplayUntil = 0;

        /// <summary>
        /// Indicates whether the control is hidden.
        /// </summary>
        public bool IsHidden
        {
            get { return mPicture.IsHidden; }
            set { mPicture.IsHidden = value; }
        }

        /// <summary>
        /// Create a new instance of the <see cref="AnnouncementWindow"/> class.
        /// </summary>
        /// <param name="gameCanvas">The <see cref="Canvas"/> to render this control on.</param>
        public AnnouncementWindow(Canvas gameCanvas)
        {
           mGameCanvas = gameCanvas;
           mPicture = new ImagePanel(gameCanvas, "AnnouncementWindow");
           mLabel = new Label(mPicture, "AnnouncementLabel");
        
           mPicture.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
        }

        /// <summary>
        /// Update this control..
        /// </summary>
        public void Update()
        {
            // Only update when we're visible to the user.
            if (!mPicture.IsHidden)
            {
                mLabel.Text = mLabelText;

                // Are we still supposed to be visible?
                if (Timing.Global.Milliseconds > mDisplayUntil)
                {
                    Hide();
                }
            }
        }

        /// <summary>
        /// Display an announcement.
        /// </summary>
        /// <param name="announcementText">The text to display.</param>
        /// <param name="displayTime">The time for which to display the announcement.</param>
        public void ShowAnnouncement(string announcementText, long displayTime)
        {
            mLabelText = announcementText;
            mDisplayUntil = Timing.Global.Milliseconds + displayTime;
            Show();
        }

        /// <summary>
        /// Hides the control.
        /// </summary>
        public void Hide()
        {
            mPicture.Hide();
        }

        /// <summary>
        /// Shows the control.
        /// </summary>
        public void Show()
        {
            mPicture.Show();
        }

    }

}