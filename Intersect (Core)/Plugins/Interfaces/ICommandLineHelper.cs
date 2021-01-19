namespace Intersect.Plugins.Interfaces
{
    /// <summary>
    /// Defines the API for accessing command line arguments.
    /// </summary>
    public interface ICommandLineHelper
    {
        /// <summary>
        /// Parses the command line arguments used to start the application into an instance of the provided type.
        /// </summary>
        /// <typeparam name="TArguments">a custom arguments type</typeparam>
        /// <returns>an instance of <typeparamref name="TArguments"/> if valid</returns>
        TArguments ParseArguments<TArguments>();
    }
}
