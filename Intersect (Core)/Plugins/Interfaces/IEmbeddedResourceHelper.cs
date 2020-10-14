using System;

using JetBrains.Annotations;

using System.IO;
using System.Reflection;

namespace Intersect.Plugins.Interfaces
{
    /// <summary>
    /// Defines the API for accessing embedded resources.
    /// </summary>
    public interface IEmbeddedResourceHelper
    {
        /// <summary>
        /// Checks if a resource with the given name exists.
        /// </summary>
        /// <param name="resourceName">the name of the resource to check for</param>
        /// <returns>if <paramref name="resourceName"/> exists</returns>
        bool Exists([NotNull] string resourceName);

        /// <summary>
        /// Gets the information for the requested resource using inexact name matching as described by <see cref="Resolve(string)"/>.
        /// </summary>
        /// <param name="resourceName">the resource name to look for</param>
        /// <returns>the <see cref="ManifestResourceInfo"/> of the specified resource</returns>
        /// <exception cref="InvalidOperationException">if <see cref="Resolve(string)"/> does not throw an error but the resource cannot be found</exception>
        /// <see cref="Resolve(string)"/>
        [NotNull]
        ManifestResourceInfo GetInfo([NotNull] string resourceName);

        /// <summary>
        /// Opens a read stream for <paramref name="resourceName"/>.
        /// </summary>
        /// <param name="resourceName">the name of the resource to open for reading</param>
        /// <returns>the read stream for <paramref name="resourceName"/></returns>
        /// <see cref="Resolve(string)"/>
        [NotNull]
        Stream Read([NotNull] string resourceName);

        /// <summary>
        /// Resolves <paramref name="resourceName"/> to a fully-qualified resource name.
        ///
        /// <paramref name="resourceName"/> is used as "ends-with" matching
        ///   criteria, where the shortest fully-qualified resource name found
        ///   is returned as the resolved resource name.
        /// </summary>
        /// <param name="resourceName">the name of the resource to resolve</param>
        /// <returns>a fully-qualified resource name if any are found</returns>
        /// <exception cref="ArgumentNullException">if <see cref="string.IsNullOrWhiteSpace(string)"/> returns true for <paramref name="resourceName"/></exception>
        /// <exception cref="FileNotFoundException">if there are *no* resources that end with <paramref name="resourceName"/></exception>
        [NotNull]
        string Resolve([NotNull] string resourceName);

        /// <summary>
        /// Attempts to get the <see cref="ManifestResourceInfo"/> of the resource
        ///   identified by <paramref name="resourceName"/>, returning true if
        ///   successful and false otherwise, without throwing exceptions.
        /// </summary>
        /// <param name="resourceName">the name of the resource</param>
        /// <param name="resourceInfo">the output variable for the resource info</param>
        /// <returns>if the <see cref="ManifestResourceInfo"/> was fetched successfully</returns>
        /// <see cref="GetInfo(string)"/>
        bool TryGetInfo([NotNull] string resourceName, out ManifestResourceInfo resourceInfo);

        /// <summary>
        /// Attempts to get the read <see cref="Stream"/> of the resource
        ///   identified by <paramref name="resourceName"/>, returning true if
        ///   successful and false otherwise, without throwing exceptions.
        /// </summary>
        /// <param name="resourceName">the name of the resource</param>
        /// <param name="stream">the output variable for the resource stream</param>
        /// <returns>if a read <see cref="Stream"/> was opened successfully</returns>
        /// <see cref="Read(string)"/>
        bool TryRead([NotNull] string resourceName, out Stream stream);

        /// <summary>
        /// Attempts to resolve the fully-qualified version of <paramref name="resourceName"/>,
        ///   returning true if successful and false otherwise, without throwing exceptions.
        /// </summary>
        /// <param name="resourceName">the name of the resource to look for</param>
        /// <param name="manifestResourceName">the fully qualified resource name</param>
        /// <returns>if a resource was found that matches <paramref name="resourceName"/></returns>
        /// <see cref="Resolve(string)"/>
        bool TryResolve([NotNull] string resourceName, out string manifestResourceName);
    }
}
