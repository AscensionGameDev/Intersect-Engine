using System;
using System.Collections.Generic;
using System.Linq;
using Intersect.Collections;
using Intersect.Extensions;
using Intersect.Models;

namespace Intersect.Enums
{
    public static class GameObjectTypeExtensions
    {
        static GameObjectTypeExtensions()
        {
            EnumType = typeof(GameObjectType);
            AttributeType = typeof(GameObjectInfoAttribute);
            AttributeMap = new Dictionary<GameObjectType, GameObjectInfoAttribute>();

            foreach (GameObjectType gameObjectType in Enum.GetValues(EnumType))
            {
                var memberInfo = EnumType.GetMember(Enum.GetName(EnumType, value: gameObjectType)).FirstOrDefault();
                AttributeMap[gameObjectType] =
                    (GameObjectInfoAttribute) memberInfo?.GetCustomAttributes(AttributeType, false).FirstOrDefault();
            }
        }

        private static Type EnumType { get; }
        private static Type AttributeType { get; }
        private static Dictionary<GameObjectType, GameObjectInfoAttribute> AttributeMap { get; }

        public static Type GetObjectType(this GameObjectType gameObjectType)
            => AttributeMap?[gameObjectType]?.Type;

        public static string GetTable(this GameObjectType gameObjectType)
            => AttributeMap?[gameObjectType]?.Table;

        public static DatabaseObjectLookup GetLookup(this GameObjectType gameObjectType)
            => LookupUtils.GetLookup(GetObjectType(gameObjectType));
    }
}