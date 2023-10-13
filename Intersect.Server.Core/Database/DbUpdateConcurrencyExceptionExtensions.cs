using System.Text;

using Intersect.Logging;

using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Database
{
    internal static class DbUpdateConcurrencyExceptionExtensions
    {
        public static void LogError(this DbUpdateConcurrencyException concurrencyException)
        {
            var concurrencyErrors = new StringBuilder();
            _ = concurrencyErrors.AppendLine($"Entry count: {concurrencyException.Entries.Count}");
            foreach (var entry in concurrencyException.Entries)
            {
                var type = entry.GetType().FullName.ToString();
                _ = concurrencyErrors
                    .AppendLine($"Entry Type [{type}] State: {entry.State}")
                    .AppendLine("--------------------");

                var proposedValues = entry.CurrentValues;
                var databaseValues = entry.GetDatabaseValues();

                foreach (var property in proposedValues.Properties)
                {
                    _ = concurrencyErrors.AppendLine($"{property.Name} (Token: {property.IsConcurrencyToken}): Proposed: {proposedValues[property]}  Original Value: {entry.OriginalValues[property]}  Database Value: {(databaseValues != null ? databaseValues[property] : "null")}");
                }

                _ = concurrencyErrors
                    .AppendLine()
                    .AppendLine();
            }
            Log.Error(concurrencyErrors.ToString());
        }
    }
}
