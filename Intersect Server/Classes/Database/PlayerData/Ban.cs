using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intersect.Server.Classes.Database.PlayerData
{
    public class Ban
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public User Player { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Reason { get; set; }
        public string Banner { get; set; }
    }
}
