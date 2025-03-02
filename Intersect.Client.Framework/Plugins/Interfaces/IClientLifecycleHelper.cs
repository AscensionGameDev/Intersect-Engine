using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.General;
using Intersect.Client.Interface;
using Intersect.Plugins.Interfaces;

namespace Intersect.Client.Plugins.Interfaces;

/// <summary>
/// Defines the API for accessing client lifecycle information and events.
/// </summary>
/// <see cref="ILifecycleHelper"/>
public interface IClientLifecycleHelper : ILifecycleHelper
{
    /// <summary>
    /// Lifecycle update event.
    /// </summary>
    event GameUpdateHandler? GameUpdate;

    /// <summary>
    /// Draw update event.
    /// </summary>
    event GameDrawHandler? GameDraw;

    /// <summary>
    /// When a lifecycle change has completed initialization.
    /// </summary>
    event LifecycleChangeStateHandler? LifecycleChangedState;

    /// <summary>
    /// Event when a lifecycle change is happening, before initialization has happened.
    /// </summary>
    event LifecycleChangeStateHandler? LifecycleChangingState;

    /// <summary>
    /// A reference to the currently active interface if one is loaded.
    /// </summary>
    IMutableInterface Interface { get; }

    /// <summary>
    /// Invokes <see cref="LifecycleChangedState"/> handlers for <paramref name="state"/>.
    /// </summary>
    /// <param name="state">the new <see cref="GameStates"/></param>
    void EmitLifecycleChangedState(GameStates state);

    /// <summary>
    /// Invokes <see cref="LifecycleChangingState"/> handlers for <paramref name="state"/>.
    /// </summary>
    /// <param name="state">the new <see cref="GameStates"/></param>
    void EmitLifecycleChangingState(GameStates state);

    /// <summary>
    /// Invokes <see cref="GameUpdate"/> handlers for <paramref name="state"/>.
    /// </summary>
    /// <param name="state">The new <see cref="GameStates"/>.</param>
    /// <param name="deltaTime">Time passed since the last update.</param>
    void EmitGameUpdate(GameStates state, IPlayer player, Dictionary<Guid, IEntity> knownEntities, TimeSpan deltaTime);

    /// <summary>
    /// Invokes <see cref="GameDraw"/> handlers for <paramref name="state"/>.
    /// </summary>
    /// <param name="state">The new <see cref="DrawStates"/>.</param>
    /// <param name="deltaTime">Time passed since the last update.</param>
    void EmitGameDraw(DrawStates state, TimeSpan deltaTime);

    /// <summary>
    /// Invokes <see cref="GameDraw"/> handlers for <paramref name="state"/>.
    /// </summary>
    /// <param name="state">The new <see cref="DrawStates"/>.</param>
    /// <param name="entity">The <see cref="IEntity"/> that is being drawn.</param>
    /// <param name="deltaTime">Time passed since the last update.</param>
    void EmitGameDraw(DrawStates state, IEntity entity, TimeSpan deltaTime);
}
