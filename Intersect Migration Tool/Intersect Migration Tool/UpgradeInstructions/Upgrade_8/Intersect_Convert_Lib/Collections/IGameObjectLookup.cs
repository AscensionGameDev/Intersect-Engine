using System;
using Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.Models;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.Collections
{
    public interface IGameObjectLookup<TValue> : ILookup<Guid, TValue> where TValue : IGameObject
    {
    }

    public interface IGameObjectLookup : IGameObjectLookup<IGameObject>
    {
    }
}