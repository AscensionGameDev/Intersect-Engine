using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Client.General;
using Intersect.Enums;

namespace Intersect.Client.Entities
{
    public class Status
    {
        public string Data = "";
        public Guid SpellId;
        public long TimeRecevied = 0;
        public long TimeRemaining = 0;
        public long TotalDuration = 1;
        public StatusTypes Type;
        public int[] Shield = new int[(int)Vitals.VitalCount];

        public Status(Guid spellId, StatusTypes type, string data, long timeRemaining, long totalDuration)
        {
            SpellId = spellId;
            Type = type;
            Data = data;
            TimeRemaining = timeRemaining;
            TotalDuration = totalDuration;
            TimeRecevied = Globals.System.GetTimeMs();
        }

        public bool IsActive()
        {
            return RemainingMs() > 0;
        }

        public long RemainingMs()
        {
            var timeDiff = Globals.System.GetTimeMs() - TimeRecevied;
            return TimeRemaining - timeDiff;
        }
    }
}
