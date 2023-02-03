using System;

namespace Intersect.Threading
{
    /// <summary>
    /// Utility class for checking the type of arguments passed to a new thread.
    /// </summary>
    /// <typeparam name="TArgument">Type of the argument that passes to a new thread.</typeparam>
    public static class ArgumentTypeChecker<TArgument>
    {
        /// <summary>
        /// Checks the type of each element within `args` array against the expected type `TArgument`. 
        /// If any element in the array is not of type `TArgument` -> An exception is thrown.
        /// </summary>
        /// <param name="args">Array of arguments to be checked.</param>
        public static void CheckArgumentTypes(object[] args)
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
    }
}
