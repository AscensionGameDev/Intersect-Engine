using System;
using System.ComponentModel.DataAnnotations;

namespace Intersect.Server.Database.Migration
{
    public sealed class DataMigrationHistory
    {
        [Key] public string Id { get; private set; }

        public string Version { get; private set; }

        public DateTime Timestamp { get; private set; }

        public DataMigrationHistory(string id, string version, DateTime timestamp)
        {
            Id = id;
            Version = version;
            Timestamp = timestamp;
        }
    }
}
