namespace Intersect.Config
{

    public class ChatOptions
    {

        //Chat
        public int MaxChatLength = 120; //120 characters

        public int MinIntervalBetweenChats = 400; //400 ms

        /// <summary>
        /// Is the client allowed to show in-game banners for announcements made?
        /// </summary>
        public bool ShowAnnouncementBanners = true;

        /// <summary>
        /// The time (in milliseconds) the announcement banners should display, if enabled.
        /// </summary>
        public int AnnouncementDisplayDuration = 5000;

        /// <summary>
        /// Whether or not chat messages should be logged to the database.
        /// </summary>
        public bool LogChatMessagesToDatabase = true;
    }

}
