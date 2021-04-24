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
        /// Configures whether or not the NPC should completely restore its vitals and statusses once it starts resetting.
        /// </summary>
        public bool ResetVitalsAndStatusses = false;

        /// <summary>
        /// Configures whether or not the NPCs health should continue to reset to full and clear statuses while working its way to the reset location
        /// </summary>
        public bool ContinuouslyResetVitalsAndStatuses = false;

        /// <summary>
        /// If true, a NPC can be attacked while they are resetting. Their new attacker will become a target if they are within the reset radius
        /// </summary>
        public bool AllowEngagingWhileResetting = false;

        /// <summary>
        /// Configures whether or not the level of an Npc is shown next to their name.
        /// </summary>
        public bool ShowLevelByName = false;

    }

}
