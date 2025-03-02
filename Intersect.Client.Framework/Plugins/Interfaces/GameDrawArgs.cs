using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.Graphics;

namespace Intersect.Client.Plugins.Interfaces;

public partial class GameDrawArgs : TimedArgs
{
    public DrawStates State { get; }

    public IEntity Entity { get; }

    public GameDrawArgs(DrawStates state, TimeSpan deltaTime) : base(deltaTime)
    {
        State = state;
    }

    public GameDrawArgs(DrawStates state, IEntity entity, TimeSpan deltaTime) : base(deltaTime)
    {
        State = state;
        Entity = entity;
    }
}