using Intersect.Client.Framework.Entities;
using Intersect.Client.General;

namespace Intersect.Client.Plugins.Interfaces;

public partial class GameUpdateArgs : TimedArgs
{
    public GameStates State { get; }

    public IPlayer Player { get; }

    public IReadOnlyDictionary<Guid, IEntity> KnownEntities { get; }

    public GameUpdateArgs(GameStates state, IPlayer player, Dictionary<Guid, IEntity> knownEntities, TimeSpan deltaTime) : base(deltaTime)
    {
        State = state;
        Player = player;
        KnownEntities = knownEntities;
    }
}