using System;

namespace Intersect.Threading
{

    /// <inheritdoc />
    public abstract partial class Threaded<TArgument> : Threaded
    {

        /// <inheritdoc />
        protected Threaded(string name = null) : base(name)
        {
        }

        private static void CheckArgumentTypes(object[] args)
        {
            if (args == null || args.Length < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(args), $@"Expected one argument of type {typeof(TArgument).FullName}."
                );
            }

            if (!(args[0] is TArgument))
            {
                throw new ArgumentException(
                    $"Expected argument of type {typeof(TArgument).FullName}, but got {args[0].GetType().FullName}."
                );
            }
        }

        /// <inheritdoc />
        protected override void ThreadStart(params object[] args)
        {
            CheckArgumentTypes(args);
            ThreadStart((TArgument)args[0]);
        }

        protected abstract void ThreadStart(TArgument argument);

    }

}
