namespace Intersect.Server.Database;

internal partial interface IDataMigration<TContext>
    where TContext : IntersectDbContext<TContext>
{
    void Down(DatabaseContextOptions databaseContextOptions);

    void Up(DatabaseContextOptions databaseContextOptions);
}
