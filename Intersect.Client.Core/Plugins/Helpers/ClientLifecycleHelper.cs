using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.General;
using Intersect.Client.Interface;
using Intersect.Client.Plugins.Interfaces;
using Intersect.Plugins.Helpers;

namespace Intersect.Client.Plugins.Helpers;

/// <inheritdoc cref="IClientLifecycleHelper"/>
internal sealed partial class ClientLifecycleHelper : ContextHelper<IClientPluginContext>, IClientLifecycleHelper
{
    /// <inheritdoc />
    public event GameUpdateHandler? GameUpdate;

    /// <inheritdoc />
    public event GameDrawHandler? GameDraw;

    /// <inheritdoc />
    public event LifecycleChangeStateHandler? LifecycleChangedState;

    /// <inheritdoc />
    public event LifecycleChangeStateHandler? LifecycleChangingState;

    internal ClientLifecycleHelper(IClientPluginContext context) : base(context)
    {
        Globals.ClientLifecycleHelpers.Add(this);
    }

    ~ClientLifecycleHelper()
    {
        Globals.ClientLifecycleHelpers.Remove(this);
    }

    /// <inheritdoc />
    public IMutableInterface Interface => Client.Interface.Interface.CurrentInterface;

    /// <inheritdoc />
    public void EmitLifecycleChangedState(GameStates state)
    {
        var lifecycleChangeStateArgs = new LifecycleChangeStateArgs(state);
        LifecycleChangedState?.Invoke(Context, lifecycleChangeStateArgs);
    }

    /// <inheritdoc />
    public void EmitLifecycleChangingState(GameStates state)
    {
        var lifecycleChangeStateArgs = new LifecycleChangeStateArgs(state);
        LifecycleChangingState?.Invoke(Context, lifecycleChangeStateArgs);
    }

    /// <inheritdoc />
    public void EmitGameUpdate(GameStates state, IPlayer player, Dictionary<Guid, IEntity> knownEntities, TimeSpan deltaTime)
    {
        var GameUpdateArgs = new GameUpdateArgs(state, player, knownEntities, deltaTime);
        GameUpdate?.Invoke(Context, GameUpdateArgs);
    }

    /// <inheritdoc />
    public void EmitGameDraw(DrawStates state, TimeSpan deltaTime)
    {
        var gameDrawArgs = new GameDrawArgs(state, deltaTime);
        GameDraw?.Invoke(Context, gameDrawArgs);
    }

    /// <inheritdoc />
    public void EmitGameDraw(DrawStates state, IEntity entity, TimeSpan deltaTime)
    {
        var gameDrawArgs = new GameDrawArgs(state, entity, deltaTime);
        GameDraw?.Invoke(Context, gameDrawArgs);
    }
}
