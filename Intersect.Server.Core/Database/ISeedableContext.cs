using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Database
{

    public interface ISeedableContext
    {

        DbSet<TType> GetDbSet<TType>() where TType : class;

    }

}
