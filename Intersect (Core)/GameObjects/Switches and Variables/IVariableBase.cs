using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Enums;
using Intersect.Models;

namespace Intersect.GameObjects.Switches_and_Variables;
public interface IVariableBase : IDatabaseObject, IFolderable
{
    public VariableDataType Type { get; set; }

    public string TextId { get; set; }
}
