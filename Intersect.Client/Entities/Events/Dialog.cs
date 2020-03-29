using System;

namespace Intersect.Client.Entities.Events
{

    public class Dialog
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
