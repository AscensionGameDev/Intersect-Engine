using System.Collections.Generic;
using System.Text;

namespace Intersect.Server.Database
{
    public class PrimaryKeyDescriptor
    {
        public const string KEYWORD_PRIMARY_KEY = "PRIMARY KEY";
        public const string KEYWORD_AUTOINCREMENT = "AUTOINCREMENT";

        public List<string> Columns { get; }
        public bool Autoincrement { get; set; }

        public PrimaryKeyDescriptor() : this(new string[] {}) { }

        public PrimaryKeyDescriptor(string columnName)
            : this(new[] { columnName }) { }

        public PrimaryKeyDescriptor(IEnumerable<string> columnNames)
        {
            Columns = new List<string>();

            if (columnNames != null)
                Columns.AddRange(columnNames);
        }

        public override string ToString()
        {
            var builder = new StringBuilder(KEYWORD_PRIMARY_KEY);
            
            var columnBuilder = new StringBuilder();
            Columns?.ForEach(columnName =>
            {
                if (columnBuilder.Length > 0) columnBuilder.Append(", ");
                columnBuilder.Append(columnName);
            });
            builder.Append(" (").Append(columnBuilder).Append(")");

            if (Autoincrement) builder.Append($" {KEYWORD_AUTOINCREMENT}");
            return builder.ToString();
        }
    }
}