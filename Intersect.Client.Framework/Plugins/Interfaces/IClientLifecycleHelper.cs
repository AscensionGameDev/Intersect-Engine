using System;
using Intersect.Client.Framework.Entities;
using Intersect.Client.General;
using Intersect.Client.Interface;
using Intersect.Plugins.Interfaces;

namespace Intersect.Client.Plugins.Interfaces
{
    public class LifecycleChangeStateArgs : EventArgs
    {
        public GameStates State { get; }

        public LifecycleChangeStateArgs(GameStates state)
        {
            State = state;
        }
    }

    public delegate void LifecycleChangeStateHandler(
        IClientPluginContext context,
        LifecycleChangeStateArgs lifecycleChangeStateArgs
    );

    public class GameUpdateArgs : EventArgs
    {
        public GameStates State { get; }

        public GameUpdateArgs(GameStates state)
        {
            State = state;
        }
    }

    public class GameDrawArgs : EventArgs
    {
        public DrawStates State { get; }

        public IEntity Entity { get; }

        public GameDrawArgs(DrawStates state)
        {
            State = state;
        }

        public GameDrawArgs(DrawStates state, IEntity entity)
        {
            State = state;
            Entity = entity;
        }
    }

    public delegate void GameUpdateHandler(
        IClientPluginContext context,
        GameUpdateArgs gameUpdateArgs
    );

    public delegate void GameDrawHandler(
        IClientPluginContext context,
        GameDrawArgs drawGameArgs
    );

    /// <summary>
    /// Defines the API for accessing client lifecycle information and events.
    /// </summary>
    /// <see cref="ILifecycleHelper"/>
    public interface IClientLifecycleHelper : ILifecycleHelper
    {
        /// <summary>
        /// Lifecycle change state event
        /// </summary>
        event LifecycleChangeStateHandler LifecycleChangeState;

        /// <summary>
        /// Lifecycle update event.
        /// </summary>
        event GameUpdateHandler GameUpdate;

        /// <summary>
        /// Draw update event.
        /// </summary>
        event GameDrawHandler GameDraw;

        /// <summary>
        /// A reference to the currently active interface if one is loaded.
        /// </summary>
        IMutableInterface Interface { get; }

        /// <summary>
        /// Invokes <see cref="LifecycleChangeState"/> handlers for <paramref name="state"/>.
        /// </summary>
        /// <param name="state">the new <see cref="GameStates"/></param>
        void OnLifecycleChangeState(GameStates state);

        /// <summary>
        /// Invokes <see cref="GameUpdate"/> handlers for <paramref name="state"/>.
        /// </summary>
        /// <param name="state">The new <see cref="GameStates"/>.</param>
        void OnGameUpdate(GameStates state);

        /// <summary>
        /// Invokes <see cref="GameDraw"/> handlers for <paramref name="state"/>.
        /// </summary>
        /// <param name="state">The new <see cref="DrawStates"/>.</param>
        void OnGameDraw(DrawStates state);

        /// <summary>
        /// Invokes <see cref="GameDraw"/> handlers for <paramref name="state"/>.
        /// </summary>
        /// <param name="state">The new <see cref="DrawStates"/>.</param>
        /// <param name="entity">The <see cref="IEntity"/> that is being drawn.</param>
        void OnGameDraw(DrawStates state, IEntity entity);
    }
}
