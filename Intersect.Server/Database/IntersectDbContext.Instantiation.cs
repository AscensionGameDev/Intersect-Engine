using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;

using Intersect.Config;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Intersect.Server.Database;

public delegate DbContext DbContextConstructor(
    DbConnectionStringBuilder connectionStringBuilder,
    DatabaseOptions.DatabaseType? databaseType,
    bool autoDetectChanges = false,
    bool explicitLoad = false,
    bool lazyLoading = false,
    bool readOnly = false,
    ILoggerFactory? loggerFactory = default,
    QueryTrackingBehavior? queryTrackingBehavior = default
);

public partial class IntersectDbContext<TDbContext>
{
    private static readonly Dictionary<Type, DbContextConstructor> _constructorCache = new();
    private static readonly ParameterInfo[] _createContextParameters =
        typeof(IntersectDbContext<>)
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
            .First()
            .GetParameters()
            .ToArray();

    protected IntersectDbContext(
        DbConnectionStringBuilder connectionStringBuilder,
        DatabaseOptions.DatabaseType? databaseType = default,
        bool autoDetectChanges = false,
        bool explicitLoad = false,
        bool lazyLoading = false,
        bool readOnly = false,
        ILoggerFactory? loggerFactory = default,
        QueryTrackingBehavior? queryTrackingBehavior = default
    )
    {
        ConnectionStringBuilder = connectionStringBuilder;
        DatabaseType = databaseType ?? DatabaseOptions.DatabaseType.SQLite;

        _loggerFactory = loggerFactory;

        ReadOnly = readOnly;

        if (queryTrackingBehavior != default)
        {
            ChangeTracker.QueryTrackingBehavior = queryTrackingBehavior.Value;
        }

        ChangeTracker.AutoDetectChangesEnabled = autoDetectChanges || ReadOnly;

        if (ReadOnly && !explicitLoad)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        ChangeTracker.LazyLoadingEnabled = lazyLoading || !ReadOnly;
    }

    public static TDbContext Create(
        DbConnectionStringBuilder connectionStringBuilder = default,
        DatabaseOptions.DatabaseType? databaseType = default,
        bool autoDetectChanges = false,
        bool explicitLoad = false,
        bool lazyLoading = false,
        bool readOnly = false,
        ILoggerFactory? loggerFactory = default,
        QueryTrackingBehavior? queryTrackingBehavior = default
    )
    {
        var dbContextType = typeof(TDbContext);
        if (!_constructorCache.TryGetValue(dbContextType, out var constructorDelegate))
        {
            var constructor = dbContextType.GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                _createContextParameters
                    .Select(parameterInfo => parameterInfo.ParameterType)
                    .ToArray()
            );

            if (constructor == default)
            {
                var parametersString = string.Join(
                    ", ",
                    _createContextParameters.Select(
                        parameterInfo => parameterInfo.ParameterType.FullName
                    )
                );
                throw new MissingMethodException(
                    dbContextType.Name,
                    $"Missing constructor with the following parameters: {parametersString}"
                );
            }

            var parameterExpressions = _createContextParameters.Select(
                parameterInfo => Expression.Parameter(
                    parameterInfo.ParameterType,
                    parameterInfo.Name
                )
            ).ToArray();

            var newExpression = Expression.New(
                constructor,
                parameterExpressions
            );
            var stronglyTypedLambdaExpression = Expression.Lambda(
                newExpression,
                parameterExpressions
            );
            var stronglyTypedInvocationExpression = Expression.Invoke(
                stronglyTypedLambdaExpression,
                parameterExpressions
            );
            var asDbContextExpression = Expression.TypeAs(
                stronglyTypedInvocationExpression,
                typeof(DbContext)
            );
            var weaklyTypedLambdaExpression = Expression.Lambda(
                asDbContextExpression,
                parameterExpressions
            );

            var compiledDelegate = weaklyTypedLambdaExpression.Compile();
            var castedDelegate = compiledDelegate as Func<
                DbConnectionStringBuilder,
                DatabaseOptions.DatabaseType?,
                bool,
                bool,
                bool,
                bool,
                ILoggerFactory?,
                QueryTrackingBehavior?,
                DbContext
            >;
            constructorDelegate = new DbContextConstructor(castedDelegate);
            _constructorCache[dbContextType] = constructorDelegate;
        }

        return constructorDelegate(
            autoDetectChanges: autoDetectChanges,
            connectionStringBuilder: connectionStringBuilder ?? _fallbackConnectionStringBuilder,
            databaseType: databaseType ?? _fallbackDatabaseType,
            explicitLoad: explicitLoad,
            lazyLoading: lazyLoading,
            loggerFactory: loggerFactory,
            readOnly: readOnly,
            queryTrackingBehavior: queryTrackingBehavior
        ) as TDbContext ?? throw new InvalidOperationException();
    }
}
