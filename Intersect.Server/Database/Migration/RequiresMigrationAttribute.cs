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
        public RequiresMigrationAttribute(string id, MigrationType migrationType)
        {
            Id = string.IsNullOrWhiteSpace(id) ? throw new ArgumentNullException(nameof(id)) : id;
            MigrationType = migrationType;
        }

        /// <summary>
        /// The required migration identifier.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The type of migration required.
        /// </summary>
        public MigrationType MigrationType { get; }
    }
}
