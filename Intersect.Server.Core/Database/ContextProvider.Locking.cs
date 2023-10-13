namespace Intersect.Server.Database
{
    public sealed partial class ContextProvider
    {
        private partial class LockProvider<TContext, UContext> where TContext : IntersectDbContext<TContext>, UContext
        {
            public static readonly object Lock = new object();
        }
    }
}
