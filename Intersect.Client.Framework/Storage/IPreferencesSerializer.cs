using System;

namespace Intersect.Client.Framework.Storage
{
    /// <summary>
    /// Declares the API for preference (de)serialization.
    /// </summary>
    public interface IPreferencesSerializer
    {
        /// <summary>
        /// If the serializer is enabled.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Deserialize preferences from a predetermined medium.
        /// </summary>
        /// <param name="destinationPreferences">the preferences instance to deserialize to</param>
        /// <returns>if deserialization was successful</returns>
        bool Deserialize(IPreferences destinationPreferences);

        /// <summary>
        /// Serialize preferences to a predetermined medium.
        /// </summary>
        /// <param name="preferences">the preferences to save</param>
        /// <returns>if serialization was succesful</returns>
        bool Serialize(IPreferences preferences);
    }
}
