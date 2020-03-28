using System;

using Intersect.Server.Entities;

namespace Intersect.Server.Database.PlayerData.Players
{

    public interface IPlayerOwned
    {

        Player Player { get; }

        Guid PlayerId { get; }

    }

}
