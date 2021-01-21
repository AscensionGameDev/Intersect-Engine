namespace Intersect.Core
{
    /// <summary>
    /// Declares the API surface for applications with specialized startup options types.
    /// </summary>
    /// <typeparam name="TStartupOptions">specialized startup options type</typeparam>
    public interface IApplicationContext<out TStartupOptions> : IApplicationContext
        where TStartupOptions : ICommandLineOptions
    {
        /// <summary>
        /// The specialized options the application was started with.
        /// </summary>
        new TStartupOptions StartupOptions { get; }
    }
}
