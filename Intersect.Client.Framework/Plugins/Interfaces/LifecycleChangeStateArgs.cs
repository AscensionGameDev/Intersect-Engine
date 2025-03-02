using Intersect.Client.General;

namespace Intersect.Client.Plugins.Interfaces;

public partial class LifecycleChangeStateArgs : EventArgs
{
    public GameStates State { get; }

    public LifecycleChangeStateArgs(GameStates state)
    {
        State = state;
    }
}