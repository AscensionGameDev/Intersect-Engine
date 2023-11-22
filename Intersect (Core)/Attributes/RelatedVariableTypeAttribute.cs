using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Enums;

namespace Intersect.Attributes;
public sealed class RelatedVariableTypeAttribute : Attribute
{
    public VariableType VariableType { get; set; }

    public RelatedVariableTypeAttribute(VariableType db) { VariableType = db; }
}
