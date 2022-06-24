using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.General;
using Intersect.Client.Interface;
using Intersect.Client.Plugins.Interfaces;
using Intersect.Plugins.Helpers;
using System;
using System.Collections.Generic;

namespace Intersect.Client.Plugins.Helpers
{
    /// <inheritdoc cref="IClientLifecycleHelper"/>
    internal sealed partial class ClientLifecycleHelper : ContextHelper<IClientPluginContext>, IClientLifecycleHelper
    {
        /// <inheritdoc />
        public event LifecycleChangeStateHandler LifecycleChangeState;

        /// <inheritdoc />
        public event GameUpdateHandler GameUpdate;

        /// <inheritdoc />
        public event GameDrawHandler GameDraw;

        internal ClientLifecycleHelper(IClientPluginContext context) : base(context)
        {
            Globals.ClientLifecycleHelpers.Add(this);
        }

        ~ClientLifecycleHelper()
        {
            Globals.ClientLifecycleHelpers.Remove(this);
        }

        /// <inheritdoc />
        public IMutableInterface Interface =>
            Client.Interface.Interface.MenuUi ?? Client.Interface.Interface.GameUi as IMutableInterface;

        /// <inheritdoc />
        public void OnLifecycleChangeState(GameStates state)
        {
            var lifecycleChangeStateArgs = new LifecycleChangeStateArgs(state);
            LifecycleChangeState?.Invoke(Context, lifecycleChangeStateArgs);
        }

        /// <inheritdoc />
        public void OnGameUpdate(GameStates state, IPlayer player, Dictionary<Guid, IEntity> knownEntities, TimeSpan deltaTime)
        {
            var GameUpdateArgs = new GameUpdateArgs(state, player, knownEntities, deltaTime);
            GameUpdate?.Invoke(Context, GameUpdateArgs);
        }

        /// <inheritdoc />
        public void OnGameDraw(DrawStates state, TimeSpan deltaTime)
        {
            var gameDrawArgs = new GameDrawArgs(state, deltaTime);
            GameDraw?.Invoke(Context, gameDrawArgs);
        }

        /// <inheritdoc />
        public void OnGameDraw(DrawStates state, IEntity entity, TimeSpan deltaTime)
        {
            var gameDrawArgs = new GameDrawArgs(state, entity, deltaTime);
            GameDraw?.Invoke(Context, gameDrawArgs);
        }
    }
}
