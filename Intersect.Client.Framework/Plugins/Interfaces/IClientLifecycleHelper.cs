using System;
using System.Collections.Generic;
using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.General;
using Intersect.Client.Interface;
using Intersect.Plugins.Interfaces;

namespace Intersect.Client.Plugins.Interfaces
{
    public partial class LifecycleChangeStateArgs : EventArgs
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

    public partial class TimedArgs : EventArgs
    {
        /// <summary>
        /// Time since the last update.
        /// </summary>
        public TimeSpan Delta { get; }

        public TimedArgs(TimeSpan delta)
        {
            Delta = delta;
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
        /// <param name="deltaTime">Time passed since the last update.</param>
        void OnGameUpdate(GameStates state, IPlayer player, Dictionary<Guid, IEntity> knownEntities, TimeSpan deltaTime);

        /// <summary>
        /// Invokes <see cref="GameDraw"/> handlers for <paramref name="state"/>.
        /// </summary>
        /// <param name="state">The new <see cref="DrawStates"/>.</param>
        /// <param name="deltaTime">Time passed since the last update.</param>
        void OnGameDraw(DrawStates state, TimeSpan deltaTime);

        /// <summary>
        /// Invokes <see cref="GameDraw"/> handlers for <paramref name="state"/>.
        /// </summary>
        /// <param name="state">The new <see cref="DrawStates"/>.</param>
        /// <param name="entity">The <see cref="IEntity"/> that is being drawn.</param>
        /// <param name="deltaTime">Time passed since the last update.</param>
        void OnGameDraw(DrawStates state, IEntity entity, TimeSpan deltaTime);
    }
}
