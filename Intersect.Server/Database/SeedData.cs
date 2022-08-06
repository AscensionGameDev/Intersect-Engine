using System.Linq;

using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Database
{
    public abstract partial class SeedData<TValue> where TValue : class
    {

        public void SeedIfEmpty(ISeedableContext context)
        {
            var dbSet = context.GetDbSet<TValue>();

            if (dbSet == null)
            {
                return;
            }

            if (dbSet.Any())
            {
                return;
            }

            Seed(dbSet);
        }

        public abstract void Seed(DbSet<TValue> dbSet);

    }

    public abstract partial class SeedData<TValue1, TValue2>
        where TValue1 : class
        where TValue2 : class
    {
        public void SeedIfEmpty(ISeedableContext context)
        {
            var dbSet1 = context.GetDbSet<TValue1>() ?? throw new MissingMemberException();
            var dbSet2 = context.GetDbSet<TValue2>() ?? throw new MissingMemberException();

            if (dbSet1.Any() || dbSet2.Any())
            {
                return;
            }

            Seed(dbSet1, dbSet2);
        }

        public abstract void Seed(
            DbSet<TValue1> dbSet1,
            DbSet<TValue2> dbSet2
        );

    }
}
