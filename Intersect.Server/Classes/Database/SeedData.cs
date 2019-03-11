using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Intersect.Server.Classes.Database
{
    public abstract class SeedData<TType>
        where TType : class
    {
        public void SeedIfEmpty(
            [NotNull] ISeedableContext context,
            [NotNull] EntityTypeBuilder<TType> entityTypeBuilder
        )
        {
            if (context.GetDbSet<TType>()?.FirstOrDefault() != null)
            {
                return;
            }

            Seed(entityTypeBuilder);
        }

        public abstract void Seed([NotNull] EntityTypeBuilder<TType> entityTypeBuilder);
    }
}
