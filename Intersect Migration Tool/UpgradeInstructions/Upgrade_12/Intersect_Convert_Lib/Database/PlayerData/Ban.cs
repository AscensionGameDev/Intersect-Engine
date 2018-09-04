using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using JetBrains.Annotations;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Classes.Database.PlayerData
{
    public class Ban
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }
        public Guid PlayerId { get; private set; }
        public virtual User Player { get; private set; }
        public string Ip { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; set; }
        public string Reason { get; private set; }
        public string Banner { get; private set; }

        public Ban() { }

        public Ban(User player, string ip, string reason, int durationDays, string banner)
        {
            Player = player;
            Ip = ip;
            StartTime = DateTime.UtcNow;
            Reason = reason;
            EndTime = StartTime.AddDays(durationDays);
            Banner = banner;
        }
    }
}
