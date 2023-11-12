using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Enums;

namespace Intersect.Attributes;
internal class Attributes
{
    public class RelatedTable : Attribute
    {
        public GameObjectType TableType { get; set; }

        public RelatedTable(GameObjectType db) { TableType = db; }
    }

    public class RelatedVariableType : Attribute
    {
        public VariableType VariableType { get; set; }

        public RelatedVariableType(VariableType db) { VariableType = db; }
    }
}
