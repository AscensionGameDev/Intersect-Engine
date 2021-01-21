using System;
using System.Reflection;
using System.Runtime.Serialization;

using Intersect.Extensions;
using Intersect.Properties;

using Microsoft;

namespace Intersect.Plugins.Loaders
{
    /// <summary>
    /// Exception class used for when a <see cref="IPluginEntry"/> cannot be found in a plugin <see cref="Assembly"/>.
    /// </summary>
    [Serializable]
    public class MissingPluginEntryException : Exception, ISerializable
    {
        /// <summary>
        /// The name of the <see cref="Assembly"/> that is missing the plugin entry type.
        /// </summary>
        public string AssemblyName { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="MissingPluginEntryException"/>.
        /// </summary>
        public MissingPluginEntryException() : base(ExceptionMessages.MissingPluginEntryExceptionDefault)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="MissingPluginEntryException"/> for the specified <see cref="Assembly"/>.
        /// </summary>
        /// <param name="assembly">the <see cref="Assembly"/> for the exception instance</param>
        public MissingPluginEntryException([ValidatedNotNull] Assembly assembly) : base(assembly.FullName)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="MissingPluginEntryException"/> for the specified <see cref="Assembly"/> name.
        /// </summary>
        /// <param name="assemblyName">the name of the <see cref="Assembly"/> for the exception instance</param>
        public MissingPluginEntryException(string assemblyName) : base(
            ExceptionMessages.MissingPluginEntryExceptionAssemblyName.Format(assemblyName)
        )
        {
            AssemblyName = assemblyName;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="MissingPluginEntryException"/> for the specified <see cref="Assembly"/> name and message.
        /// </summary>
        /// <param name="assemblyName">the name of the <see cref="Assembly"/> for the exception instance</param>
        /// <param name="message">the message for the exception instance</param>
        public MissingPluginEntryException(string assemblyName, string message) : base(message)
        {
            AssemblyName = assemblyName;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="MissingPluginEntryException"/> with the specified message and inner <see cref="Exception"/>.
        /// </summary>
        /// <param name="message">the message for the exception instance</param>
        /// <param name="innerException">the <see cref="Exception"/> that caused this one</param>
        public MissingPluginEntryException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="MissingPluginEntryException"/> during deserialization.
        /// </summary>
        /// <param name="serializationInfo">The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="streamingContext">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected MissingPluginEntryException(SerializationInfo serializationInfo, StreamingContext streamingContext) :
            base(serializationInfo, streamingContext)
        {
            AssemblyName = serializationInfo.GetString(nameof(AssemblyName));
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            base.GetObjectData(serializationInfo, streamingContext);
            serializationInfo.AddValue(nameof(AssemblyName), AssemblyName);
        }
    }
}
