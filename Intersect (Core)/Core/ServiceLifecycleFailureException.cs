
using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

using Intersect.Properties;

namespace Intersect.Core
{
    /// <summary>
    /// Represents the container exception that occurs when services fail during a lifecycle method.
    /// </summary>
    [Serializable]
    public class ServiceLifecycleFailureException : Exception
    {
        #region Constants

#pragma warning disable 618
        private const ServiceLifecycleStage DefaultServiceLifecycleStage = ServiceLifecycleStage.Unknown;
#pragma warning restore 618

        #endregion Constants

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="ServiceLifecycleFailureException"/> with a stage, name and inner exception.
        /// </summary>
        /// <param name="serviceLifecycleStage">the stage in which the service failed</param>
        /// <param name="serviceName">the name of the service throwing the exception</param>
        /// <param name="innerException">the cause of the failure</param>
        public ServiceLifecycleFailureException(
            ServiceLifecycleStage serviceLifecycleStage,
            string serviceName,
            Exception innerException
        ) : base($"Failure occurred during service {serviceLifecycleStage} in {serviceName}.", innerException)
        {
            ServiceLifecycleStage = serviceLifecycleStage;
            ServiceName = serviceName;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// The stage in which the service failed.
        /// </summary>
        public ServiceLifecycleStage ServiceLifecycleStage { get; }

        /// <summary>
        /// The name of the service throwing the exception.
        /// </summary>
        public string ServiceName { get; }

        #endregion Properties

        #region Standard Exception Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="ServiceLifecycleFailureException"/>.
        /// </summary>
        [Obsolete("Use ServiceLifecycleFailureException(ServiceLifecycleStage, string, Exception) instead.", true)]
        public ServiceLifecycleFailureException()
        {
            Debug.Assert(
                !string.IsNullOrWhiteSpace(DeveloperStrings.ServiceLifecycleFailureExceptionUnknknownServiceName)
            );

            ServiceLifecycleStage = DefaultServiceLifecycleStage;
            ServiceName = DeveloperStrings.ServiceLifecycleFailureExceptionUnknknownServiceName;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ServiceLifecycleFailureException"/> with a descriptive message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        [Obsolete("Use ServiceLifecycleFailureException(ServiceLifecycleStage, string, Exception) instead.", true)]
        public ServiceLifecycleFailureException(string message) : base(message)
        {
            Debug.Assert(
                !string.IsNullOrWhiteSpace(DeveloperStrings.ServiceLifecycleFailureExceptionUnknknownServiceName)
            );

            ServiceLifecycleStage = DefaultServiceLifecycleStage;
            ServiceName = DeveloperStrings.ServiceLifecycleFailureExceptionUnknknownServiceName;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ServiceLifecycleFailureException"/> with a descriptive message and inner exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. For this type of exception this should not be used.</param>
        [Obsolete("Use ServiceLifecycleFailureException(ServiceLifecycleStage, string, Exception) instead.", true)]
        public ServiceLifecycleFailureException(string message, Exception innerException) : base(
            message, innerException
        )
        {
            Debug.Assert(
                !string.IsNullOrWhiteSpace(DeveloperStrings.ServiceLifecycleFailureExceptionUnknknownServiceName)
            );

            ServiceLifecycleStage = DefaultServiceLifecycleStage;
            ServiceName = DeveloperStrings.ServiceLifecycleFailureExceptionUnknknownServiceName;
        }

        #endregion Standard Exception Constructors

        #region Serialization

        /// <summary>
        /// Initializes a new instance of <see cref="ServiceLifecycleFailureException"/> with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected ServiceLifecycleFailureException(SerializationInfo info, StreamingContext context) : base(
            info, context
        )
        {
            Contract.Assert(info != null);

            Debug.Assert(
                !string.IsNullOrWhiteSpace(DeveloperStrings.ServiceLifecycleFailureExceptionUnknknownServiceName)
            );

            ServiceLifecycleStage =
                (ServiceLifecycleStage) (info.GetValue(nameof(ServiceLifecycleStage), typeof(ServiceLifecycleStage)) ??
                                         DefaultServiceLifecycleStage);

            ServiceName = info.GetString(nameof(ServiceName)) ??
                          DeveloperStrings.ServiceLifecycleFailureExceptionUnknknownServiceName;
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Contract.Assert(info != null);

            info.AddValue(nameof(ServiceLifecycleStage), ServiceLifecycleStage);
            info.AddValue(nameof(ServiceName), ServiceName);

            base.GetObjectData(info, context);
        }

        #endregion Serialization
    }
}
