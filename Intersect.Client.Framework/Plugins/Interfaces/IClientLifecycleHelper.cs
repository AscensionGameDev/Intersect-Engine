using System;

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

    public delegate void GameUpdateHandler(
        IClientPluginContext context,
        GameUpdateArgs gameUpdateArgs
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
    }
}
