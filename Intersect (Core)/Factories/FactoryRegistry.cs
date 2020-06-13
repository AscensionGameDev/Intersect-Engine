using System;

using JetBrains.Annotations;

namespace Intersect.Factories
{
    /// <summary>
    /// Utility class that stores instances of <see cref="IFactory{TValue}"/>.
    /// </summary>
    /// <typeparam name="TValue">the type of created instances</typeparam>
    public static class FactoryRegistry<TValue>
    {
        /// <summary>
        /// The current <see cref="IFactory{TValue}"/> instance, null if one hasn't been registered.
        /// </summary>
        [CanBeNull]
        public static IFactory<TValue> Factory { get; private set; }

        /// <summary>
        /// Creates an instance of <typeparamref name="TValue"/> with the provided arguments.
        /// </summary>
        /// <param name="args">the optional creation arguments</param>
        /// <returns>an instance of <typeparamref name="TValue"/></returns>
        /// <see cref="IFactory{TValue}.Create(object[])"/>
        /// <exception cref="ArgumentNullException">thrown if there is no registered <see cref="IFactory{TValue}"/></exception>
        [NotNull]
        [Pure]
        public static TValue Create([NotNull] params object[] args)
        {
            if (Factory == null)
            {
                throw new InvalidOperationException($@"No factory registered for type: {typeof(TValue).FullName}");
            }

            return Factory.Create(args);
        }

        /// <summary>
        /// Safely attempts to create an instance of <typeparamref name="TValue"/> without throwing an exception.
        /// </summary>
        /// <param name="value">the return parameter</param>
        /// <param name="args">the creation arguments</param>
        /// <returns>if the instance was created</returns>
        /// <see cref="Create(object[])"/>
        [Pure]
        public static bool TryCreate(out TValue value, [NotNull] params object[] args)
        {
            try
            {
                value = Create(args);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// Registers an <see cref="IFactory{TValue}"/> as the current instance.
        /// </summary>
        /// <param name="factory">the <see cref="IFactory{TValue}"/> instance to register</param>
        /// <param name="overrideExisting">if the current instance should be overwritten if it exists (default false)</param>
        /// <returns>true if registration was successful, false if a factory was already registered and <paramref name="overrideExisting"/> is false</returns>
        public static bool RegisterFactory([NotNull] IFactory<TValue> factory, bool overrideExisting = false)
        {
            if (Factory != null && !overrideExisting)
            {
                return false;
            }

            Factory = factory;
            return true;
        }
    }
}
