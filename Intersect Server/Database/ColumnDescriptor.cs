using System.Text;

namespace Intersect.Server.Database
{
    public class ColumnDescriptor
    {
        public const string KEYWORD_NOT_NULL = "NOT NULL";
        public const string KEYWORD_DEFAULT = "DEFAULT";

        public string Name { get; }
        public DataType Type { get; set; }
        public bool PrimaryKey { get; set; }
        public bool Autoincrement { get; set; }
        public bool Unique { get; set; }
        public bool NotNull { get; set; }
        public object Default { get; set; }

        public ColumnDescriptor(string name, DataType type = DataType.Blob)
        {
            Name = name;
            Type = type;
            PrimaryKey = false;
            Autoincrement = false;
            Unique = false;
            NotNull = false;
            Default = null;
        }

        public override string ToString()
        {
            var builder = new StringBuilder($"{Name} {Type.ToSql()}");
            if (PrimaryKey)
            {
                builder.Append($" {PrimaryKeyDescriptor.KEYWORD_PRIMARY_KEY}");
                if (Autoincrement) builder.Append($" {PrimaryKeyDescriptor.KEYWORD_AUTOINCREMENT}");
            }

            if (Unique) builder.Append($" {ConstraintDescriptor.TYPE_UNIQUE}");
            if (NotNull) builder.Append($" {KEYWORD_NOT_NULL}");
            if (Default != null) builder.Append($" {KEYWORD_DEFAULT} {Default}");
            return builder.ToString();
        }
    }
}
