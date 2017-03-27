using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect.Models
{
    public interface IIndexedGameObject : IGameObject
    {
        int Index { get; }
    }
}
