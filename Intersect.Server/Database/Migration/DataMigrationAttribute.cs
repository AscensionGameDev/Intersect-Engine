using System;

namespace Intersect.Server.Database.Migration
{
    /// <summary>
    /// Indicates that a class is a <see cref="DataMigration" /> and provides its identifier.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DataMigrationAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of this attribute.
        /// </summary>
        /// <param name="id"> The migration identifier. </param>
        public DataMigrationAttribute(string id)
        {
            Id = string.IsNullOrWhiteSpace(id) ? throw new ArgumentNullException(nameof(id)) : id;
        }

        /// <summary>
        /// The migration identifier.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// If the migration can be skipped.
        /// </summary>
        public bool Skippable { get; set; } = false;
    }
}
