using System;

namespace Intersect.Extensions
{

    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public sealed class GameObjectInfoAttribute : Attribute
    {

        public GameObjectInfoAttribute(Type type, string table)
        {
            Type = type;
            Table = table;
        }

        public Type Type { get; }

        public string Table { get; }

    }

}
