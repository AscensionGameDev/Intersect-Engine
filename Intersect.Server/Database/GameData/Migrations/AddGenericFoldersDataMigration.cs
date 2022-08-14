using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Logging;
using Intersect.Models;
using Intersect.Server.Migrations.Game;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json.Linq;

using SqlKata.Execution;

namespace Intersect.Server.Database.GameData.Migrations;

[SchemaMigration(typeof(AddGenericFolders))]
public sealed partial class AddGenericFoldersDataMigration : IDataMigration<GameContext>
{
    private class IdFolder
    {
        public Guid Id { get; set; }

        public string Folder { get; set; }
    }

    public void Down(GameContext context)
    {
        throw new NotImplementedException();
    }

    public void Up(GameContext context)
    {
        var mapLists = QueryMapLists(context);
        foreach (var mapList in mapLists)
        {
            ConvertMapList(context, mapList, default);
        }

        AggregateFolders<AnimationBase>(context);
        AggregateFolders<CraftBase>(context);
        AggregateFolders<CraftingTableBase>(context);
        AggregateFolders<ClassBase>(context);
        AggregateFolders<EventBase>(context);
        AggregateFolders<GuildVariableBase>(context);
        AggregateFolders<ItemBase>(context);
        AggregateFolders<NpcBase>(context);
        AggregateFolders<PlayerVariableBase>(context);
        AggregateFolders<ProjectileBase>(context);
        AggregateFolders<QuestBase>(context);
        AggregateFolders<ResourceBase>(context);
        AggregateFolders<ServerVariableBase>(context);
        AggregateFolders<ShopBase>(context);
        AggregateFolders<SpellBase>(context);
    }

    public void AggregateFolders<TDescriptor>(GameContext context)
        where TDescriptor : Descriptor
    {
        var dbConnection = context.Database.GetDbConnection();
        var descriptorTable = context.Model
            .FindEntityType(typeof(TDescriptor))
            .GetTableName() ?? throw new InvalidOperationException($"Missing table for {typeof(TDescriptor).FullName}.");
        var queryCompiler = context.DatabaseType.CreateQueryCompiler();
        var queryFactory = new QueryFactory(dbConnection, queryCompiler);
        var legacyFolders = queryFactory
            .Query(descriptorTable)
            .Select("Id", "Folder")
            .Get<(byte[] Id, string? Folder)>()
            .Select(row => new IdFolder
            {
                Id = new Guid(row.Id),
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
                folder = new Folder
                {
                    DescriptorType = group.First().Type,
                    Name = new ContentString(group.Key.Trim())
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
