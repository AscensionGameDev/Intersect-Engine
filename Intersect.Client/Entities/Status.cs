﻿using System;
using Intersect.Client.Framework.Entities;
using Intersect.Client.General;
using Intersect.Enums;

namespace Intersect.Client.Entities
{

    public partial class Status : IStatus
    {

        public string Data { get; set; } = "";

        public int[] Shield { get; set; } = new int[(int)Vitals.VitalCount];

        public Guid SpellId { get; set; }

        public long TimeRecevied { get; set; } = 0;

        public long TimeRemaining { get; set; } = 0;

        public long TotalDuration { get; set; } = 1;

        public StatusTypes Type { get; set; }

        public Status(Guid spellId, StatusTypes type, string data, long timeRemaining, long totalDuration)
        {
            SpellId = spellId;
            Type = type;
            Data = data;
            TimeRemaining = timeRemaining;
            TotalDuration = totalDuration;
            TimeRecevied = Globals.System.GetTimeMs();
        }

        public bool IsActive => RemainingMs > 0;

        public long RemainingMs => TimeRemaining - (Globals.System.GetTimeMs() - TimeRecevied);

    }

}
