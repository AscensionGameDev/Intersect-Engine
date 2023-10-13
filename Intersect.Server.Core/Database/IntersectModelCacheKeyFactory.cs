using System.Reflection;
using Intersect.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Intersect.Server.Database;

internal sealed class IntersectModelCacheKeyFactory : IModelCacheKeyFactory
{
    private static readonly Dictionary<Type, Func<DbContext, bool, IntersectModelCacheKey>> CacheKeyDelegates = new();

    private static readonly MethodInfo _methodInfoCreateTContext =
        typeof(IntersectModelCacheKeyFactory).GetMethod(nameof(Create), BindingFlags.NonPublic | BindingFlags.Static) ??
        throw new InvalidOperationException();

    private static IntersectModelCacheKey Create<TContext>(DbContext context, bool designTime)
        where TContext : IntersectDbContext<TContext>
    {
        if (context is not TContext castedContext)
        {
            throw new InvalidOperationException();
        }

        var contextOptions = castedContext.ContextOptions;
        var modelOptions = new IntersectModelOptions(contextOptions.DisableAutoInclude);
        return new(typeof(TContext), designTime, modelOptions);
    }

    public object Create(DbContext context, bool designTime)
    {
        var contextType = context.GetType();
        if (!contextType.Extends(typeof(IntersectDbContext<>)))
        {
            return new ExternalModelCacheKey(contextType, designTime);
        }

        var specificContextType = contextType.FindGenericTypeParameters(typeof(IntersectDbContext<>)).First();
        if (!CacheKeyDelegates.TryGetValue(specificContextType, out var @delegate))
        {
            @delegate = _methodInfoCreateTContext.MakeGenericMethod(specificContextType)
                    .CreateDelegate(typeof(Func<DbContext, bool, IntersectModelCacheKey>)) as
                Func<DbContext, bool, IntersectModelCacheKey>;
            CacheKeyDelegates[specificContextType] = @delegate;
        }

        if (@delegate == default)
        {
            throw new InvalidOperationException();
        }

        return @delegate(context, designTime);
    }
}
