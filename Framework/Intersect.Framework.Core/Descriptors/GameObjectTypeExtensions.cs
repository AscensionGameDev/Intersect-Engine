using System.Diagnostics.CodeAnalysis;
using Intersect.Collections;
using Intersect.Extensions;
using Intersect.GameObjects;
using Intersect.GameObjects.Switches_and_Variables;
using Intersect.Models;

namespace Intersect.Enums;

public static partial class GameObjectTypeExtensions
{
    static GameObjectTypeExtensions()
    {
        EnumType = typeof(GameObjectType);
        AttributeType = typeof(GameObjectInfoAttribute);
        AttributeMap = new Dictionary<GameObjectType, GameObjectInfoAttribute>();

        foreach (GameObjectType gameObjectType in Enum.GetValues(EnumType))
        {
            var memberInfo = EnumType.GetMember(Enum.GetName(EnumType, value: gameObjectType)).FirstOrDefault();
            var gameObjectInfoAttribute = (GameObjectInfoAttribute) memberInfo?.GetCustomAttributes(AttributeType, false).FirstOrDefault();
            AttributeMap[gameObjectType] = gameObjectInfoAttribute;
            if (gameObjectInfoAttribute != default)
            {
                _gameObjectTypeLookup[gameObjectInfoAttribute.Type] = gameObjectType;
            }
        }
    }

    private static Type EnumType { get; }

    private static Type AttributeType { get; }

    private static Dictionary<GameObjectType, GameObjectInfoAttribute> AttributeMap { get; }

    private static readonly Dictionary<Type, GameObjectType> _gameObjectTypeLookup = [];

    public static bool TryGetGameObjectType(this Type type, out GameObjectType gameObjectType) =>
        _gameObjectTypeLookup.TryGetValue(type, out gameObjectType);

    public static Type GetObjectType(this GameObjectType gameObjectType)
    {
        return AttributeMap?[gameObjectType]?.Type;
    }

    public static string GetTable(this GameObjectType gameObjectType)
    {
        return AttributeMap?[gameObjectType]?.Table;
    }

    public static DatabaseObjectLookup GetLookup(this GameObjectType gameObjectType)
    {
        return DatabaseObjectLookup.GetLookup(GetObjectType(gameObjectType));
    }

    public static IDatabaseObject CreateNew(this GameObjectType gameObjectType)
    {
        var instance = Activator.CreateInstance(
            AttributeMap?[gameObjectType]?.Type,
            new object[] { }
        );

        return instance as IDatabaseObject;
    }

    public static int ListIndex(this GameObjectType gameObjectType, Guid id, VariableDataType dataTypeFilter = 0)
    {
        var lookup = gameObjectType.GetLookup();

        if (dataTypeFilter == 0)
        {
            return lookup.KeyList.OrderBy(pairs => lookup[pairs]?.Name).ToList().IndexOf(id);
        }

        return lookup
            .OrderBy(kv => kv.Value?.Name)
            .Select(kv => kv.Value)
            .OfType<IVariableBase>()
            .Where(desc => desc.Type == dataTypeFilter)
            .Select(desc => desc.Id)
            .ToList()
            .IndexOf(id);
    }

    public static VariableDataType GetVariableType(this GameObjectType gameObjectType, Guid variableDescriptorId)
    {
        var lookup = gameObjectType.GetLookup();

        return lookup.ValueList
                .OfType<IVariableBase>()
                .FirstOrDefault(var => var.Id == variableDescriptorId)?.Type ?? 0;
    }

    public static Guid IdFromList(this GameObjectType gameObjectType, int listIndex, VariableDataType dataTypeFilter = 0)
    {
        var lookup = gameObjectType.GetLookup();

        if (listIndex < 0 || listIndex >= lookup.KeyList.Count)
        {
            return Guid.Empty;
        }

        if (dataTypeFilter == 0)
        {
            return lookup.KeyList.OrderBy(pairs => lookup[pairs]?.Name).ToArray()[listIndex];
        }

        return lookup
            .OrderBy(kv => kv.Value?.Name)
            .Select(kv => kv.Value)
            .OfType<IVariableBase>()
            .Where(desc => desc.Type == dataTypeFilter)
            .Select(desc => desc.Id)
            .Skip(listIndex)
            .FirstOrDefault();
    }

    public static string[] Names(this GameObjectType gameObjectType, VariableDataType dataTypeFilter = 0)
    {
        if (dataTypeFilter == 0)
        {
            return gameObjectType
                .GetLookup()
                .OrderBy(p => p.Value?.Name)
                .Select(pair => pair.Value?.Name ?? PlayerVariableBase.Deleted)
                .ToArray();
        }

        return gameObjectType
            .GetLookup()
            .Select(kv => kv.Value)
            .OfType<IVariableBase>()
            .Where(desc => desc.Type == dataTypeFilter)
            .OrderBy(p => p?.Name)
            .Select(pair => pair?.Name ?? PlayerVariableBase.Deleted)
            .ToArray();
    }
}
