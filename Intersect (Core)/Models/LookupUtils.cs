using System;
using System.Collections.Generic;
using System.Linq;

using Intersect.Collections;
using Intersect.Enums;
using Intersect.Logging;

namespace Intersect.Models
{

    public static class LookupUtils
    {

        private static readonly object Lock = new object();

        private static Dictionary<Type, GameObjectType> sEnumMap;

        private static Dictionary<Type, DatabaseObjectLookup> sLookupMap;

        public static Dictionary<Type, DatabaseObjectLookup> LookupMap => sLookupMap =
            sLookupMap ?? new Dictionary<Type, DatabaseObjectLookup>();

        public static Dictionary<Type, GameObjectType> EnumMap => sEnumMap =
            sEnumMap ?? new Dictionary<Type, GameObjectType>();

        public static DatabaseObjectLookup GetLookup(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (LookupMap == null)
            {
                throw new ArgumentNullException(nameof(LookupMap));
            }

            lock (Lock)
            {
                try
                {
                    if (!LookupMap.ContainsKey(type))
                    {
                        LookupMap[type] = new DatabaseObjectLookup(type);
                    }
                }
                catch (Exception exception)
                {
                    Log.Error($"Impossible NPE... [LookupMap={LookupMap}, type={type}]");
                    if (exception.InnerException != null)
                    {
                        Log.Error(exception.InnerException);
                    }

                    Log.Error(exception);
                    Log.Error($"{nameof(LookupMap)}={LookupMap},{nameof(type)}={type}");

                    throw;
                }

                return LookupMap[type];
            }
        }

        public static GameObjectType GetGameObjectType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException();
            }

            if (LookupMap == null)
            {
                throw new ArgumentNullException();
            }

            if (EnumMap == null)
            {
                throw new ArgumentNullException();
            }

            if (EnumMap.ContainsKey(type))
            {
                return EnumMap[type];
            }

            var values = Enum.GetValues(typeof(GameObjectType));
            foreach (GameObjectType gameObjectType in values)
            {
                if (type != gameObjectType.GetObjectType())
                {
                    continue;
                }

                EnumMap[type] = gameObjectType;

                return gameObjectType;
            }

            throw new ArgumentOutOfRangeException();
        }

        public static string[] GetNameList(GameObjectType type)
        {
            return GetNameList(type.GetObjectType());
        }

        public static string[] GetNameList(Type type)
        {
            return GetLookup(type)?.Select(pair => pair.Value?.Name).ToArray();
        }

    }

}
