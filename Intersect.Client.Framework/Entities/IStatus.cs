using Intersect.Enums;
using System;

namespace Intersect.Client.Framework.Entities
{
    public interface IStatus
    {
        string Data { get; set; }
        int[] Shield { get; set; }
        Guid SpellId { get; set; }
        long TimeRecevied { get; set; }
        long TimeRemaining { get; set; }
        long TotalDuration { get; set; }
        SpellEffect Type { get; set; }

        bool IsActive { get; }
        long RemainingMs { get; }
    }
}