namespace Intersect.Server.Database;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
internal sealed partial class SchemaMigrationAttribute : SchemaMigrationTypeAttribute
{
    public SchemaMigrationAttribute(Type schemaMigrationType)
        : base(schemaMigrationType) { }

    public bool ApplyIfLast { get; set; }
}
