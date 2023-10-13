using Intersect.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Database;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
internal abstract partial class SchemaMigrationTypeAttribute : Attribute
{
    public Type DbContextType { get; }

    public Type SchemaMigrationType { get; }

    public string SchemaMigrationName { get; }

    protected SchemaMigrationTypeAttribute(Type schemaMigrationType)
    {
        SchemaMigrationType = schemaMigrationType ?? throw new ArgumentNullException(nameof(schemaMigrationType));

        var migrationAttribute = SchemaMigrationType
            .GetCustomAttributes(typeof(MigrationAttribute), true)
            .OfType<MigrationAttribute>()
            .FirstOrDefault();

        if (migrationAttribute == default)
        {
            throw new InvalidOperationException($"Missing [Migration] attribute.");
        }

        SchemaMigrationName = migrationAttribute.Id;
        if (string.IsNullOrWhiteSpace(SchemaMigrationName))
        {
            throw new InvalidOperationException("Migration name must not be empty.");
        }

        var dbContextAttribute = SchemaMigrationType
            .GetCustomAttributes(typeof(DbContextAttribute), true)
            .OfType<DbContextAttribute>()
            .FirstOrDefault();

        if (dbContextAttribute == default)
        {
            throw new InvalidOperationException($"Missing [DbContext] attribute.");
        }

        DbContextType = dbContextAttribute.ContextType;

        if (!DbContextType.Extends(typeof(IntersectDbContext<>)))
        {
            throw new NotSupportedException($"Unsupported context type: {DbContextType.FullName}");
        }
    }
}
