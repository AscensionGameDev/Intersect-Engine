using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.GameObjects
{
    public interface IGameObject<K, V> where V : IGameObject<K, V>
    {
        K Id { get; }
    }
}
