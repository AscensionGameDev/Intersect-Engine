using System;

using Intersect.Client.General;
using Intersect.Client.Interface;
using Intersect.Plugins.Interfaces;

using JetBrains.Annotations;

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
        [NotNull] IClientPluginContext context,
        [NotNull] LifecycleChangeStateArgs lifecycleChangeStateArgs
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
        /// A reference to the currently active interface if one is loaded.
        /// </summary>
        [CanBeNull]
        IMutableInterface Interface { get; }

        /// <summary>
        /// Invokes <see cref="LifecycleChangeState"/> handlers for <paramref name="state"/>.
        /// </summary>
        /// <param name="state">the new <see cref="GameStates"/></param>
        void OnLifecycleChangeState(GameStates state);
    }
}
