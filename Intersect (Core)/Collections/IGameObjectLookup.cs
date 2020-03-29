using System;

using Intersect.Models;

namespace Intersect.Collections
{

    public interface IGameObjectLookup<TValue> : ILookup<Guid, TValue> where TValue : INamedObject
    {

    }

}
