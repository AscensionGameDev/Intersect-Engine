namespace Intersect.Plugins.Helpers
{
    /// <summary>
    /// Partial implementation class for helpers that require a known <see cref="IPluginContext"/>.
    /// </summary>
    /// <typeparam name="TContext">the type of <see cref="IPluginContext"/> needed</typeparam>
    public abstract class ContextHelper<TContext> where TContext : IPluginContext
    {
        /// <summary>
        /// Reference to the current <see cref="IPluginContext"/>.
        /// </summary>
        protected TContext Context { get; }

        /// <summary>
        /// Partially instantiates a <see cref="ContextHelper{TContext}"/>.
        /// </summary>
        /// <param name="context">the required <typeparamref name="TContext"/></param>
        protected ContextHelper(TContext context)
        {
            Context = context;
        }
    }
}
