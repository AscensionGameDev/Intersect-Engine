using Intersect.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Intersect.Client.Interface.Game.Chat
{

    public class ChatboxMsg
    {

        private static List<ChatboxMsg> sGameMessages = new List<ChatboxMsg>();

        // TODO: Move to a configuration file to make this player configurable?
        /// <summary>
        /// Contains the configuration of which message types to display in each chat tab.
        /// </summary>
        private static Dictionary<ChatboxTab, ChatMessageType[]> sTabMessageTypes = new Dictionary<ChatboxTab, ChatMessageType[]>() {
            // All has ALL tabs unlocked, so really we don't have to worry about that one.
            { ChatboxTab.Local, new ChatMessageType[] { ChatMessageType.Local, ChatMessageType.PM, ChatMessageType.Admin } },
            { ChatboxTab.Party, new ChatMessageType[] { ChatMessageType.Party, ChatMessageType.PM, ChatMessageType.Admin } },
            { ChatboxTab.Global, new ChatMessageType[] { ChatMessageType.Global, ChatMessageType.PM, ChatMessageType.Admin } },
            { ChatboxTab.Guild, new ChatMessageType[] { ChatMessageType.Guild, ChatMessageType.PM, ChatMessageType.Admin } },
            { ChatboxTab.System, new ChatMessageType[] { 
                ChatMessageType.Experience, ChatMessageType.Loot, ChatMessageType.Inventory, ChatMessageType.Bank, 
                ChatMessageType.Combat, ChatMessageType.Quest, ChatMessageType.Crafting, ChatMessageType.Trading, 
                ChatMessageType.Friend, ChatMessageType.Spells, ChatMessageType.Notice, ChatMessageType.Error,
                ChatMessageType.Admin } },
        };

        private string mMsg = "";

        private Color mMsgColor;

        private string mTarget = "";

        private ChatMessageType mType;

        /// <summary>
        /// Creates a new instance of the <see cref="ChatboxMsg"/> class.
        /// </summary>
        /// <param name="msg">The message to add.</param>
        /// <param name="clr">The color of the message.</param>
        /// <param name="type">The type of the message.</param>
        /// <param name="target">The target of the message.</param>
        public ChatboxMsg(string msg, Color clr, ChatMessageType type, string target = "")
        {
            mMsg = msg;
            mMsgColor = clr;
            mTarget = target;
            mType = type;
        }

        /// <summary>
        /// The contents of this message.
        /// </summary>
        public string Message => mMsg;

        /// <summary>
        /// The color of this message.
        /// </summary>
        public Color Color => mMsgColor;

        // The target of this message.
        public string Target => mTarget;

        /// <summary>
        /// The type of this message.
        /// </summary>
        public ChatMessageType Type => mType;

        /// <summary>
        /// Adds a new chat message to the stored list.
        /// </summary>
        /// <param name="msg">The message to add.</param>
        public static void AddMessage(ChatboxMsg msg)
        {
            sGameMessages.Add(msg);
        }

        /// <summary>
        /// Retrieves all chat messages.
        /// </summary>
        /// <returns>Returns a list of chat messages.</returns>
        public static List<ChatboxMsg> GetMessages()
        {
            return sGameMessages;
        }

        /// <summary>
        /// Retrieves all messages that should be displayed in the provided tab.
        /// </summary>
        /// <param name="tab">The tab for which to retrieve all messages.</param>
        /// <returns>Returns a list of chat messages.</returns>
        public static List<ChatboxMsg> GetMessages(ChatboxTab tab)
        {
            var output = new List<ChatboxMsg>();

            // Are we looking for all messages?
            if (tab == ChatboxTab.All)
            {
                output = GetMessages();
            }
            else
            {
                // No, sort them out! Select what we want to display in this tab.
                foreach (var message in sGameMessages)
                {
                    if (sTabMessageTypes[tab].Contains(message.Type))
                    {
                        output.Add(message);
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// Clears all stored messages.
        /// </summary>
        public static void ClearMessages()
        {
            sGameMessages.Clear();
        }
    }

}
