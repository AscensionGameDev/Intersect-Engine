using System;

namespace Intersect.Threading
{

    /// <inheritdoc />
    public abstract class Threaded<TArgument> : Threaded
    {

        /// <inheritdoc />
        protected Threaded(string name = null) : base(name)
        {
        }

        /// <inheritdoc />
        protected override void ThreadStart(params object[] args)
        {
            // TODO: Generic utility that checks arg types against expected types
            if (args == null || args.Length < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(args), $@"Expected one argument of type {typeof(TArgument).FullName}."
                );
            }

            ThreadStart((TArgument) args[0]);
        }

        protected abstract void ThreadStart(TArgument argument);

    }

}
