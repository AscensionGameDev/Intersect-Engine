﻿namespace Intersect.Network.Packets.Server
{
    /// <summary>
    /// Defines the layout for an AnnouncementPacket.
    /// </summary>
    public class AnnouncementPacket : CerasPacket
    {

        /// <summary>
        /// The announcement message to send.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The time (in milliseconds) for the announcement to display.
        /// </summary>
        public long Duration { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="AnnouncementPacket"/> class.
        /// </summary>
        /// <param name="message">The message to send to the client.</param>
        /// <param name="duration">The duration for this message to appear for on the client.</param>
        public AnnouncementPacket(string message, long duration)
        {
            Message = message;
            Duration = duration;
        }

    }
}