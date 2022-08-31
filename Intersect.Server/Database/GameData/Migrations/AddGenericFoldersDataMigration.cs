using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;
using Intersect.GameObjects.Events;
using Intersect.Logging;
using Intersect.Models;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json.Linq;

using SqlKata.Execution;

namespace Intersect.Server.Database.GameData.Migrations;

[SchemaMigration(typeof(Intersect.Server.Migrations.MySql.Game.AddGenericFolders))]
[SchemaMigration(typeof(Intersect.Server.Migrations.Sqlite.Game.AddGenericFolders))]
public sealed partial class AddGenericFoldersDataMigration : IDataMigration<GameContext>
{
    private class IdFolder
    {
        public Guid Id { get; set; }

        public string Folder { get; set; }
    }

    public void Down(DatabaseContextOptions databaseContextOptions)
    {
        throw new NotImplementedException();
    }

    public void Up(DatabaseContextOptions databaseContextOptions)
    {
        using var context = GameContext.Create(databaseContextOptions);
        var mapLists = QueryMapLists(context);
        foreach (var mapList in mapLists)
        {
            ConvertMapList(context, mapList, default);
        }

        var changes = context.SaveChanges();
        Log.Info($"Saved {changes} to maps.");

        AggregateFolders<AnimationBase>(databaseContextOptions);
        AggregateFolders<CraftBase>(databaseContextOptions);
        AggregateFolders<CraftingTableBase>(databaseContextOptions);
        AggregateFolders<ClassBase>(databaseContextOptions);
        AggregateFolders<EventBase>(databaseContextOptions);
        AggregateFolders<GuildVariableBase>(databaseContextOptions);
        AggregateFolders<ItemBase>(databaseContextOptions);
        AggregateFolders<NpcBase>(databaseContextOptions);
        AggregateFolders<PlayerVariableBase>(databaseContextOptions);
        AggregateFolders<ProjectileBase>(databaseContextOptions);
        AggregateFolders<QuestBase>(databaseContextOptions);
        AggregateFolders<ResourceBase>(databaseContextOptions);
        AggregateFolders<ServerVariableBase>(databaseContextOptions);
        AggregateFolders<ShopBase>(databaseContextOptions);
        AggregateFolders<SpellBase>(databaseContextOptions);
    }

    public void AggregateFolders<TDescriptor>(DatabaseContextOptions databaseContextOptions)
        where TDescriptor : Descriptor
    {
        using var context = GameContext.Create(databaseContextOptions);

        var dbConnection = context.Database.GetDbConnection();
        var descriptorTable = context.Model
            .FindEntityType(typeof(TDescriptor))?
            .GetTableName() ?? throw new InvalidOperationException($"Missing table for {typeof(TDescriptor).FullName}.");
        var queryCompiler = context.DatabaseType.CreateQueryCompiler();
        var queryFactory = new QueryFactory(dbConnection, queryCompiler);
        var legacyFolders = queryFactory
            .Query(descriptorTable)
            .Select("Id", "Folder")
            .Get<(string Id, string? Folder)>()
            .Select(row => new IdFolder
            {
                Id = Guid.Parse(row.Id),
                Folder = row.Folder,
            })
            .ToList();
        var descriptors = context.GetDbSet<TDescriptor>().ToList();
        var groupedByFolder = descriptors
            .GroupBy(descriptor => legacyFolders.FirstOrDefault(idf => idf.Id == descriptor.Id)?.Folder);
        foreach (var group in groupedByFolder)
        {
            Folder? folder = default;

            if (!string.IsNullOrWhiteSpace(group.Key))
            {
                folder = new()
                {
                    DescriptorType = group.First().Type,
                    Name = new(group.Key.Trim())
                    {
                        Comment = group.Key.Trim(),
                    },
                    Parent = default,
                };
                _ = context.Folders.Add(folder);
            }

            foreach (var descriptor in group)
            {
                descriptor.Parent = folder;
            }
        }

        context.ChangeTracker.DetectChanges();
        var changes = context.SaveChanges();
        Log.Info($"{changes} updates applied.");
    }

    private IEnumerable<JObject> QueryMapLists(GameContext context)
    {
        var dbConnection = context.Database.GetDbConnection();
        var queryCompiler = context.DatabaseType.CreateQueryCompiler();
        var queryFactory = new QueryFactory(dbConnection, queryCompiler);
        return queryFactory
            .Query("MapFolders")
            .Select("JsonData")
            .Get<string>()
            .Select(JObject.Parse)
            .ToList();
    }

    private void ConvertMapList(GameContext context, JObject mapList, Folder? parent)
    {
        var items = mapList["Items"] as JArray;
        foreach (var mapListItem in items)
        {
            ConvertMapListItem(context, mapListItem as JObject, parent);
        }
    }

    private void ConvertMapListItem(GameContext context, JObject mapListItem, Folder? parent)
    {
        var type = mapListItem["Type"]?.Value<int>();
        var name = mapListItem["Name"].Value<string>();
        switch (type)
        {
            case 0:
            case null:
                var folder = new Folder
                {
                    DescriptorType = GameObjectType.Map,
                    Name = new ContentString(name)
                    {
                        Comment = name,
                    },
                    Parent = parent,
                };
                _ = context.Folders.Add(folder);
                var children = mapListItem["Children"] as JObject;
                if (children != default)
                {
                    ConvertMapList(context, children, folder);
                }
                break;

            case 1:
                var mapId = Guid.Parse(mapListItem["MapId"].Value<string>());
                var maps = context.Maps.ToList();
                var map = context.Maps.FirstOrDefault(map => map.Id == mapId);
                if (map == default)
                {
                    Log.Warn($"Map no longer exists: {mapId} \"{name}\"");
                }
                else
                {
                    map.Parent = parent;
                }
                break;

            default: throw new InvalidOperationException();
        }
    }
}
