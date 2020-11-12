using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Intersect.Server.Database.Migration
{
    public sealed class DataMigrationMetadata
    {
        public Type Type { get; private set; }

        public DataMigrationAttribute DataMigrationAttribute { get; private set; }

        public List<RequiresMigrationAttribute> RequiresMigrationAttributes { get; private set; }

        public DataMigrationHistory CreateHistory<TContext>() where TContext : IntersectDbContext<TContext> =>
            new DataMigrationHistory(
                DataMigrationAttribute.Id, typeof(TContext).Assembly.GetName().Version.ToString(), DateTime.UtcNow
            );

        public static DataMigrationMetadata GetMigrationMetadataFor(Type type)
        {
            var dataMigrationAttribute = type.GetCustomAttribute<DataMigrationAttribute>();
            if (dataMigrationAttribute == null)
            {
                return null;
            }

            var requiredMigrationAttributes = type.GetCustomAttributes<RequiresMigrationAttribute>(true);

            return new DataMigrationMetadata
            {
                Type = type,
                DataMigrationAttribute = dataMigrationAttribute,
                RequiresMigrationAttributes = requiredMigrationAttributes.ToList()
            };
        }

        public static List<DataMigrationMetadata> FindAvailableMigrations<TContext>()
            where TContext : IntersectDbContext<TContext> =>
            typeof(TContext).Assembly.GetTypes()
            .Select(GetMigrationMetadataFor)
            .Where(metadata => metadata?.Type.BaseType.GenericTypeArguments.FirstOrDefault() == typeof(TContext))
            .ToList();
    }
}
