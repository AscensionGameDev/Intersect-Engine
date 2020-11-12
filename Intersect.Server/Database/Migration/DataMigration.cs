namespace Intersect.Server.Database.Migration
{
    public abstract class DataMigration<TContext> where TContext : IntersectDbContext<TContext>
    {
        public virtual bool SupportsRollback => false;

        public virtual bool Up(TContext context) => false;

        public virtual bool Down(TContext context) => false;
    }
}
