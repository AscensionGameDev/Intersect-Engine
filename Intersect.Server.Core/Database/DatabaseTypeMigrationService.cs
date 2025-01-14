using System.Reflection;
using Intersect.Extensions;
using Intersect.Logging;
using Intersect.Reflection;
using Microsoft.EntityFrameworkCore;
using Intersect.Server.Localization;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Intersect.Server.Database;

public class DatabaseTypeMigrationService
{
    private static readonly MethodInfo MethodInfoMigrateDbSet = typeof(DatabaseTypeMigrationService)
                                                                     .GetMethod(nameof(MigrateDbSet),
                                                                         BindingFlags.NonPublic | BindingFlags.Static)
                                                                 ?? throw new InvalidOperationException();

    private async Task<bool> CheckIfNotEmpty<TContext>(DatabaseContextOptions options)
        where TContext : IntersectDbContext<TContext>
    {
        await using var context = IntersectDbContext<TContext>.Create(options);

        try
        {
            if (context.IsEmpty)
            {
                // _ = await context.Database.EnsureDeletedAsync();
                await context.Database.MigrateAsync();
                return false;
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception);
            return true;
        }

        Log.Error(Strings.Migration.MySqlNotEmpty);
        return true;
    }

    private class ModelGraphNode : Dictionary<Type, ModelGraphNode>
    {
        public ModelGraphNode(IEntityType entityType)
        {
            EntityType = entityType;
        }

        public IEntityType EntityType { get; }

        public bool IsRoot => !EntityType.GetForeignKeys().Any();

        public Type Type => EntityType.ClrType;

        public void AddSubgraphTypes(HashSet<IEntityType> knownTypes)
        {
            if (!knownTypes.Add(EntityType))
            {
                return;
            }

            foreach (var child in Values)
            {
                child.AddSubgraphTypes(knownTypes);
            }
        }
    }

    private static ModelGraphNode[] GetModelGraph<TContext>(DatabaseContextOptions databaseContextOptions)
        where TContext : IntersectDbContext<TContext>
    {
        using var context = IntersectDbContext<TContext>.Create(databaseContextOptions);
        var model = context.Model;
        var entityTypes = model.GetEntityTypes().ToArray();
        Dictionary<Type, ModelGraphNode> modelGraphNodes = new(entityTypes.Length);

        foreach (var entityType in entityTypes)
        {
            if (!modelGraphNodes.TryGetValue(entityType.ClrType, out var entityNode))
            {
                entityNode = new ModelGraphNode(entityType);
                modelGraphNodes.Add(entityType.ClrType, entityNode);
            }

            var foreignKeys = entityType.GetForeignKeys();
            foreach (var foreignKey in foreignKeys)
            {
                var foreignKeyType = foreignKey.PrincipalEntityType;
                if (!modelGraphNodes.TryGetValue(foreignKeyType.ClrType, out var foreignKeyNode))
                {
                    foreignKeyNode = new ModelGraphNode(foreignKeyType);
                    modelGraphNodes.Add(foreignKeyType.ClrType, foreignKeyNode);
                }

                foreignKeyNode[entityType.ClrType] = entityNode;
            }
        }

        List<ModelGraphNode> modelGraphRoots = [];
        HashSet<IEntityType> knownTypes = [];

        var missingNodes = modelGraphNodes.Values.OrderBy(node => node.IsRoot ? 0 : 1).ToList();

        while (missingNodes.Count > 0)
        {
            var currentNode = missingNodes[0];
            if (!knownTypes.Contains(currentNode.EntityType))
            {
                modelGraphRoots.Add(currentNode);
                currentNode.AddSubgraphTypes(knownTypes);
            }
            missingNodes.RemoveAt(0);
        }

        return modelGraphRoots.ToArray();
    }

    private static Type[] Flatten(ModelGraphNode[] nodes)
    {
        HashSet<ModelGraphNode> addedNodes = [..nodes];
        List<ModelGraphNode> flattened = [..addedNodes];
        for (var index = 0; index < flattened.Count; ++index)
        {
            foreach (var child in flattened[index].Values.Where(child => addedNodes.Add(child)))
            {
                flattened.Add(child);
            }
        }

        flattened = flattened.Distinct().ToList();
        return flattened.Select(node => node.Type).ToArray();
    }

    public async Task<bool> TryMigrate<TContext>(DatabaseContextOptions fromOptions, DatabaseContextOptions toOptions)
        where TContext : IntersectDbContext<TContext>
    {
        try
        {
            if (await CheckIfNotEmpty<TContext>(toOptions))
            {
                return false;
            }

            var modelGraphRoots = GetModelGraph<TContext>(fromOptions);
            var modelGraphSortedTypes = Flatten(modelGraphRoots);

            var dbSetInfos = typeof(TContext).GetProperties()
                .Where(propertyInfo => propertyInfo.PropertyType.Extends(typeof(DbSet<>)))
                .ToArray();

            var sortedDbSetInfos = dbSetInfos.OrderBy(
                    propertyInfo =>
                    {
                        var type = propertyInfo.PropertyType.GenericTypeArguments[0];
                        return modelGraphSortedTypes.IndexOf(type);
                    }
                )
                .ToArray();

            foreach (var dbSetInfo in sortedDbSetInfos)
            {
                Log.Info(Strings.Migration.MigratingDbSet.ToString(dbSetInfo.Name));

                try
                {
                    var dbSetContainedType = dbSetInfo.PropertyType.FindGenericTypeParameters(typeof(DbSet<>)).First();
                    var migrateDbSetMethod =
                        MethodInfoMigrateDbSet.MakeGenericMethod(typeof(TContext), dbSetContainedType);
                    var migrateTask = migrateDbSetMethod.Invoke(
                        null,
                        new object[] { fromOptions, toOptions, dbSetInfo }
                    ) as Task;
                    await (migrateTask ?? throw new InvalidOperationException());
                }
                catch (Exception exception)
                {
                    Log.Error(exception);
                    throw;
                }
            }

            return true;
        }
        catch (Exception exception)
        {
            Log.Error(exception);
            throw;
        }
    }

    private static async Task MigrateDbSet<TContext, T>(
        DatabaseContextOptions fromOptions,
        DatabaseContextOptions toOptions,
        PropertyInfo dbSetInfo
    )
        where TContext : IntersectDbContext<TContext>
        where T : class
    {
        await using var fromContext = IntersectDbContext<TContext>.Create(fromOptions with
        {
            DisableAutoInclude = true,
            LoggerFactory = new IntersectLoggerFactory(typeof(TContext).Name),
        });
        await using var toContext = IntersectDbContext<TContext>.Create(toOptions with
        {
            DisableAutoInclude = true,
            EnableDetailedErrors = true,
            EnableSensitiveDataLogging = true,
            LoggerFactory = new IntersectLoggerFactory(typeof(TContext).Name),
        });

        if (dbSetInfo.GetValue(fromContext) is not DbSet<T> fromDbSet)
        {
            throw new InvalidOperationException();
        }

        if (dbSetInfo.GetValue(toContext) is not DbSet<T> toDbSet)
        {
            throw new InvalidOperationException();
        }

        foreach (var item in fromDbSet)
        {
            fromContext.Entry(item).State = EntityState.Detached;
            toDbSet.Add(item);
        }

        try
        {
            toContext.ChangeTracker.DetectChanges();
            await toContext.SaveChangesAsync();
        }
        catch (Exception exception)
        {
            Log.Error(exception);
            throw;
        }
    }
}
