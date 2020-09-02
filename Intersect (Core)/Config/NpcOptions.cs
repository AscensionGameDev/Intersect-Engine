namespace Intersect.Config
{

    /// <summary>
    /// Contains configurable options pertaining to the way Npcs are handled by the engine.
    /// </summary>
    public class NpcOptions
    {

        /// <summary>
        /// Configures whether or not Npcs are allowed to reset after moving out of a specified radius when starting to fight another entity.
        /// </summary>
        public bool AllowResetRadius = false;

        /// <summary>
        /// Configures the radius in which an NPC is allowed to move after starting to fight another entity.
        /// </summary>
        public int ResetRadius = 8;

        /// <summary>
        /// Configures whether or not the NPC is allowed to gain a new reset center point while it is still busy moving to its original reset point.
        /// NOTE: Can be used to allow the NPCs to be dragged far far away, as it constantly resets the center of its radius!!!
        /// </summary>

        public bool AllowNewResetLocationBeforeFinish = false;

        /// <summary>
        /// Configures whether or not the NPC should completely restore its vitals and statusses once it resets.
        /// </summary>
        public bool ResetVitalsAndStatusses = false;

    }

}
