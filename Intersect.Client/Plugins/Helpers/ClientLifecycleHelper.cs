using Intersect.Client.General;
using Intersect.Client.Interface;
using Intersect.Client.Plugins.Interfaces;
using Intersect.Plugins.Helpers;

namespace Intersect.Client.Plugins.Helpers
{
    /// <inheritdoc cref="IClientLifecycleHelper"/>
    internal sealed class ClientLifecycleHelper : ContextHelper<IClientPluginContext>, IClientLifecycleHelper
    {
        /// <inheritdoc />
        public event LifecycleChangeStateHandler LifecycleChangeState;

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
    }
}
