using System.Runtime.Serialization;

namespace Intersect.Config
{
    public partial class InstancingOptions
    {
        /// <summary>
        ///  Intersect default for instance lives
        /// </summary>
        public const int DefaultInstanceLives = 3;

        /// <summary>
        /// Whether or not dieing in a shared instance "respawns" you at the instance entrance. Useful for dungeon implementations
        /// </summary>
        public bool SharedInstanceRespawnInInstance { get; }  = true;

        /// <summary>
        /// Whether a player that leaves a shared instance can come back in to that instance after leaving.
        /// </summary>
        public bool RejoinableSharedInstances { get; } = false;

        /// <summary>
        /// How many lives a party has in a shared instance, if enabled
        /// </summary>
        public int MaxSharedInstanceLives { get; } = DefaultInstanceLives;

        /// <summary>
        /// Whether or not all party members get booted out of an instance on lives reaching -1
        /// </summary>
        public bool BootAllFromInstanceWhenOutOfLives { get; } = true;

        /// <summary>
        /// Whether or not you lose experience on death in a shared instance
        /// </summary>
        public bool LoseExpOnInstanceDeath { get; } = false;

        /// <summary>
        /// Whether or not you regenerate mana on instance death
        /// </summary>
        public bool RegenManaOnInstanceDeath { get; } = false;

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            Validate();
        }

        public void Validate()
        {
        }
    }
}
