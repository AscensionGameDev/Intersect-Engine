using System;

namespace Intersect.Server.Database.Migration
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class RequiresMigrationAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of this attribute.
        /// </summary>
        /// <param name="id"> The required migration identifier. </param>
        public RequiresMigrationAttribute(string id)
        {
            Id = string.IsNullOrWhiteSpace(id) ? throw new ArgumentNullException(nameof(id)) : id;
        }

        /// <summary>
        /// The required migration identifier.
        /// </summary>
        public string Id { get; }
    }
}
