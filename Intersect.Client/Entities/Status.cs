﻿using Intersect.Client.Framework.Entities;
using Intersect.Enums;
using Intersect.Utilities;

namespace Intersect.Client.Entities;


public partial class Status : IStatus
{

    public string Data { get; set; } = "";

    public long[] Shield { get; set; } = new long[Enum.GetValues<Vital>().Length];

    public Guid SpellId { get; set; }

    public long TimeRecevied { get; set; } = 0;

    public long TimeRemaining { get; set; } = 0;

    public long TotalDuration { get; set; } = 1;

    public SpellEffect Type { get; set; }

    public Status(Guid spellId, SpellEffect type, string data, long timeRemaining, long totalDuration)
    {
        SpellId = spellId;
        Type = type;
        Data = data;
        TimeRemaining = timeRemaining;
        TotalDuration = totalDuration;
        TimeRecevied = Timing.Global.Milliseconds;
    }

    public bool IsActive => RemainingMs > 0;

    public long RemainingMs => TimeRemaining - (Timing.Global.Milliseconds - TimeRecevied);

}
