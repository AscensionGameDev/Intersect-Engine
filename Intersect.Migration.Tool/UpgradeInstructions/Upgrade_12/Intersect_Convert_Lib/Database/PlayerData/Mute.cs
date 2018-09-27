using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using JetBrains.Annotations;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Classes.Database.PlayerData
{
    public class Mute
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }
        public Guid PlayerId { get; private set; }
        public virtual User Player { get;  set; }
        public string Ip { get;  set; }
        public DateTime StartTime { get;  set; }
        public DateTime EndTime { get; set; }
        public string Reason { get;  set; }
        public string Muter { get;  set; }

        public Mute() { }

        public Mute(User player, string ip, string reason, int durationDays, string muter)
        {
            Player = player;
            Ip = ip;
            StartTime = DateTime.UtcNow;
            Reason = reason;
            EndTime = StartTime.AddDays(durationDays);
            Muter = muter;
        }
    }
}
