namespace Intersect.Configuration
{

    /// <summary>
    /// Base interface for configuration structures
    /// </summary>
    /// <typeparam name="TConfiguration">Configuration type</typeparam>
    public interface IConfiguration<out TConfiguration>
    {

        /// <summary>
        /// Loads configuration into this instance from the specified file.
        /// </summary>
        /// <param name="filePath">the file to load from</param>
        /// <param name="failQuietly">do not throw an exception if an error is encountered, default false</param>
        /// <returns></returns>
        TConfiguration Load(string filePath, bool failQuietly = false);

        /// <summary>
        /// Persists configuration from this instance into the specified file.
        /// </summary>
        /// <param name="filePath">the file to save to</param>
        /// <param name="failQuietly">do not throw an exception if an error is encountered, default false</param>
        /// <returns></returns>
        TConfiguration Save(string filePath, bool failQuietly = false);

    }

}
