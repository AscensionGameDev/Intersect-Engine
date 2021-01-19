namespace Intersect.Factories
{
    /// <summary>
    /// Defines the interface for a factory class.
    /// </summary>
    /// <typeparam name="TValue">type of the created instances</typeparam>
    public interface IFactory<out TValue>
    {
        /// <summary>
        /// Creates a not-null instance of <typeparamref name="TValue"/> given the input <paramref name="args"/>.
        /// </summary>
        /// <param name="args">the optional arguments used in creating a <typeparamref name="TValue"/></param>
        /// <returns>a not-null <typeparamref name="TValue"/></returns>
        TValue Create(params object[] args);
    }
}
