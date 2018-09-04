using System;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Extensions
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