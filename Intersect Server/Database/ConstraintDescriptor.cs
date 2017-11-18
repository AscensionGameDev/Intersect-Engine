using System;
using System.Collections.Generic;
using System.Linq;

namespace Intersect.Server.Database
{
    public abstract class ConstraintDescriptor
    {
        public const string TYPE_UNIQUE = "UNIQUE";

        public string Name { get; }
        public List<string> Columns { get; }

        protected ConstraintDescriptor(string name, IEnumerable<string> columnNames)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            Name = name;
            Columns = new List<string>();

            if (columnNames != null)
                Columns.AddRange(columnNames);
        }

        public override string ToString()
        {
            return $"{Name} ({string.Join(", ", Columns?.Select(columnName => $"`{columnName}`") ?? new string[] {})})";
        }
    }
}