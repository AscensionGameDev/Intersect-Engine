using System;

namespace Intersect.Client.Framework.Storage
{
    /// <summary>
    /// Common implementation of the preferences serialization API.
    /// </summary>
    public abstract class PreferencesSerializer : IPreferencesSerializer
    {
        protected IGameContext GameContext { get; }

        protected PreferencesSerializer(IGameContext gameContext)
        {
            GameContext = gameContext ?? throw new ArgumentNullException(nameof(gameContext));
            
        }

        /// <inheritdoc />
        public bool Enabled { get; set; }

        /// <inheritdoc />
        public abstract bool Deserialize(IPreferences destinationPreferences);

        /// <inheritdoc />
        public abstract bool Serialize(IPreferences preferences);
    }
}
