namespace Intersect.Config
{

    public partial class ChatOptions
    {

        /// <summary>
        /// The maximum length of a chat message in characters.
        /// </summary>
        public int MaxChatLength = 120;

        /// <summary>
        /// The minimum amount of time (in milliseconds) between sending chat messages.
        /// </summary>
        public int MinIntervalBetweenChats = 400;

        /// <summary>
        /// Is the client allowed to show in-game banners for announcements made?
        /// </summary>
        public bool ShowAnnouncementBanners = true;

        /// <summary>
        /// The time (in milliseconds) the announcement banners should display, if enabled.
        /// </summary>
        public int AnnouncementDisplayDuration = 5000;
    }

}
