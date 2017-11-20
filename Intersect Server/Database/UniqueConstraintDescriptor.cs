using System.Collections.Generic;

namespace Intersect.Server.Database
{
    public class UniqueConstraintDescriptor : ConstraintDescriptor
    {
        public UniqueConstraintDescriptor(params string[] columnNames)
            : base(TYPE_UNIQUE, columnNames) { }

        public UniqueConstraintDescriptor(IEnumerable<string> columnNames = null)
            : base(TYPE_UNIQUE, columnNames) { }
    }
}