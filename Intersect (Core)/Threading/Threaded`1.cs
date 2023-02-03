namespace Intersect.Threading
{
    /// <summary>
    /// Represents a generic base class for creating and managing new threads.
    /// </summary>
    /// <typeparam name="TArgument">Type of argument that will be passed to a new thread.</typeparam>
    public abstract partial class Threaded<TArgument> : Threaded
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Threaded{TArgument}"/> class.
        /// </summary>
        /// <param name="name">Name of new thread.</param>
        protected Threaded(string name = null) : base(name)
        {
        }

        /// <summary>
        /// Overrides the <see cref="Threaded.ThreadStart"/> method.
        /// </summary>
        protected override void ThreadStart(params object[] args)
        {
            ArgumentTypeChecker<TArgument>.CheckArgumentTypes(args);
            ThreadStart((TArgument)args[0]);
        }

        /// <summary>
        /// Main entry point for a new thread, where the argument of type <typeparamref name="TArgument"/> is passed.
        /// </summary>
        /// <param name="argument">Argument of type <typeparamref name="TArgument"/> that will be passed to a new thread.</param>
        protected abstract void ThreadStart(TArgument argument);
    }
}
