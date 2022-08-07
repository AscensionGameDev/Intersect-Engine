namespace Intersect.Server.Database;

internal partial interface IDataMigration<TContext>
    where TContext : IntersectDbContext<TContext>
{
    void Down(TContext context);

    void Up(TContext context);
}
