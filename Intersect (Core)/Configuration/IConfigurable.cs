namespace Intersect.Configuration
{

    /// <summary>
    /// Base interface for configurable objects
    /// </summary>
    /// <typeparam name="TConfiguration">Configuration type</typeparam>
    public interface IConfigurable<out TConfiguration> where TConfiguration : IConfiguration<TConfiguration>
    {

        /// <summary>
        /// The configuration instance for this object
        /// </summary>
        TConfiguration Configuration { get; }

    }

}
