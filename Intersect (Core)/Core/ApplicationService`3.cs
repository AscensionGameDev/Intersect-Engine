
using Microsoft;

using System;

namespace Intersect.Core
{
    /// <summary>
    /// Partially implements <see cref="IApplicationService"/>.
    /// </summary>
    /// <typeparam name="TApplicationContext">the application context type</typeparam>
    /// <typeparam name="TServiceInterface">the service interface type</typeparam>
    /// <typeparam name="TServiceImplementation">the service implementation type</typeparam>
    public abstract class
        ApplicationService<TApplicationContext, TServiceInterface, TServiceImplementation> : ApplicationService<
            TServiceInterface, TServiceImplementation>
        where TServiceImplementation :
        ApplicationService<TApplicationContext, TServiceInterface, TServiceImplementation>, TServiceInterface
    {
        #region Internal Lifecycle Methods

        /// <exception cref="ArgumentNullException">throws an exception if <paramref name="applicationContext"/> is not an instance of <typeparamref name="TApplicationContext"/></exception>
        /// <inheritdoc />
        protected override void TaskStart([ValidatedNotNull] IApplicationContext applicationContext)
        {
            if (applicationContext is TApplicationContext typedContext)
            {
                TaskStart(typedContext);
                return;
            }

            throw new ArgumentException(
                $@"Invalid context, expected type {typeof(TApplicationContext).FullName} but received {applicationContext.GetType().FullName}.",
                nameof(applicationContext)
            );
        }

        /// <inheritdoc />
        protected override void TaskStop([ValidatedNotNull] IApplicationContext applicationContext)
        {
            if (applicationContext is TApplicationContext typedContext)
            {
                TaskStop(typedContext);
                return;
            }

            throw new ArgumentException(
                $@"Invalid context, expected type {typeof(TApplicationContext).FullName} but received {applicationContext.GetType().FullName}.",
                nameof(applicationContext)
            );
        }

        #endregion Internal Lifecycle Methods

        #region Specialized Internal Lifecycle Methods

        /// <summary>
        /// Specialized internal startup handler declaration.
        /// </summary>
        /// <param name="applicationContext">the application context the service is being started in</param>
        protected abstract void TaskStart(TApplicationContext applicationContext);

        /// <summary>
        /// Specialized internal shutdown handler declaration.
        /// </summary>
        /// <param name="applicationContext">the application context the service is being shutdown in</param>
        protected abstract void TaskStop(TApplicationContext applicationContext);

        #endregion Specialized Internal Lifecycle Methods
    }
}
