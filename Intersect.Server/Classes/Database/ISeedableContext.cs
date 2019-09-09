using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Classes.Database
{
    public interface ISeedableContext
    {

        DbSet<TType> GetDbSet<TType>() where TType : class;

    }
}
