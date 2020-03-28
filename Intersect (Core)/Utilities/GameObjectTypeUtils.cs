using System;
using System.Collections.Generic;
using System.Data;

using Intersect.Enums;

namespace Intersect.Utilities
{

    public class GameObjectTypeException : Exception
    {

        public GameObjectTypeException() : base()
        {
        }

        public GameObjectTypeException(string message) : base(message)
        {
        }

    }

    public static class GameObjectTypeUtils
    {

        private static readonly IDictionary<string, GameObjectType> mNameToTypeCache;

        static GameObjectTypeUtils()
        {
            mNameToTypeCache = new Dictionary<string, GameObjectType>();
            var enumGameObjectType = typeof(GameObjectType);
            var types = Enum.GetValues(enumGameObjectType);
            foreach (GameObjectType type in types)
            {
                var typeName = Enum.GetName(enumGameObjectType, type);
                if (typeName == null)
                {
                    throw new NoNullAllowedException();
                }

                mNameToTypeCache[typeName] = type;
                mNameToTypeCache[typeName.ToLower()] = type;
                mNameToTypeCache[typeName.ToUpper()] = type;
            }
        }

        public static GameObjectType TypeFromName(string name)
        {
            var type = GameObjectType.Animation;
            if (mNameToTypeCache.TryGetValue(name, out type))
            {
                return type;
            }

            if (mNameToTypeCache.TryGetValue(name.ToLower(), out type))
            {
                mNameToTypeCache[name.ToLower()] = type;

                return type;
            }

            // ReSharper disable once InvertIf
            if (mNameToTypeCache.TryGetValue(name.ToUpper(), out type))
            {
                mNameToTypeCache[name.ToUpper()] = type;

                return type;
            }

            throw new GameObjectTypeException($"GameObjectType with name '{name}' not found.");
        }

    }

}
