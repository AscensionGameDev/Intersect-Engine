using System;

namespace Intersect_Client.Classes.Entities
{
    public class EventDialog
    {
        public Guid EventId;
        
        public string Face = "";
        public string Opt1 = "";
        public string Opt2 = "";
        public string Opt3 = "";
        public string Opt4 = "";
        public string Prompt = "";
        public int ResponseSent;
        public int Type;
    }
}