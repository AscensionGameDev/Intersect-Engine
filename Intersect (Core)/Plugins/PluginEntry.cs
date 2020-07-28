using System;

namespace Intersect.Plugins
{
    /// <summary>
    /// Abstract class that virtually defines all of the methods declared by <see cref="IPluginEntry"/>.
    /// </summary>
    public abstract class PluginEntry : IPluginEntry
    {
        private bool mDisposed;

        /// <inheritdoc />
        public virtual void OnBootstrap(IPluginBootstrapContext context) { }

        /// <inheritdoc />
        public virtual void OnStart(IPluginContext context) { }

        /// <inheritdoc />
        public virtual void OnStop(IPluginContext context) { }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc cref="IDisposable.Dispose" />
        /// <param name="disposing">if it is disposing or finalizing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (mDisposed)
            {
                return;
            }

            mDisposed = true;
        }
    }
}
