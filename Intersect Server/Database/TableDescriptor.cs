using System;
using System.Collections.Generic;
using System.Text;

namespace Intersect.Server.Database
{
    public class TableDescriptor
    {
        public string Name { get; }
        public List<ColumnDescriptor> Columns { get; }
        public PrimaryKeyDescriptor PrimaryKeyDescriptor { get; set; }
        public List<UniqueConstraintDescriptor> UniqueConstraints { get; }

        public TableDescriptor(string name,
            IEnumerable<ColumnDescriptor> columnDescriptors = null,
            params UniqueConstraintDescriptor[] uniqueConstraints)
            : this(name, columnDescriptors, null, uniqueConstraints)
        {
        }

        public TableDescriptor(string name,
            IEnumerable<ColumnDescriptor> columnDescriptors = null,
            PrimaryKeyDescriptor primaryKeyDescriptor = null,
            IEnumerable<UniqueConstraintDescriptor> uniqueConstraints = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            Name = name;

            Columns = new List<ColumnDescriptor>();
            if (columnDescriptors != null)
                Columns.AddRange(columnDescriptors);

            UniqueConstraints = new List<UniqueConstraintDescriptor>();
            if (uniqueConstraints != null)
                UniqueConstraints.AddRange(uniqueConstraints);

            PrimaryKeyDescriptor = primaryKeyDescriptor;
        }

        public override string ToString()
        {
            var builder = new StringBuilder(Name);

            builder.Append("(");

            var columnBuilder = new StringBuilder();
            Columns?.ForEach(column =>
            {
                if (columnBuilder.Length > 0) columnBuilder.Append(", ");
                columnBuilder.Append(column);
            });
            builder.Append(columnBuilder);

            if (PrimaryKeyDescriptor != null)
                builder.Append(", ").Append(PrimaryKeyDescriptor);

            UniqueConstraints?.ForEach(constraint =>
            {
                builder.Append(", ").Append(constraint);
            });

            builder.Append(")");

            return builder.ToString();
        }
    }
}