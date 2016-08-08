using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntersectClientExtras.GenericClasses;

namespace Intersect_Client.Classes.UI.Game.Chat
{
    public class ChatboxMsg
    {
        private static List<ChatboxMsg> GameMessages = new List<ChatboxMsg>();
        private string _msg = "";
        private Color _msgColor;
        private string _target = "";

        public ChatboxMsg(string msg, Color clr, string target = "")
        {
            _msg = msg;
            _msgColor = clr;
            _target = target;
        }

        public string GetMessage()
        {
            return _msg;
        }

        public Color GetColor()
        {
            return _msgColor;
        }

        public string GetTarget()
        {
            return _target;
        }

        public static void AddMessage(ChatboxMsg msg)
        {
            GameMessages.Add(msg);
        }

        public static List<ChatboxMsg> GetMessages()
        {
            return GameMessages;
        }
    }
}
