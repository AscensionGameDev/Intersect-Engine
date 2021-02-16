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

        /// <summary>
        /// Configures whether or not the level of an Npc is shown next to their name.
        /// </summary>
        public bool ShowLevelByName = false;

        /// <summary>
        /// Configures whether or not the type of Npc is displayed adove to their name as a sprite tag.
        /// [ Default is false ]
        /// </summary>
        public bool ShowNpcTags = false;

        /// <summary>
        /// NpcTags above the Npc's Name Labels, only works if ShowNpcTags = true
        /// [ Default is true ]
        /// </summary> 
        public bool NpcTagsOnTop = true;

        /// <summary>
        /// NpcTags under the Npc's Name Labels, only works if ShowNpcTags = true
        /// [ Default is false ]
        /// </summary>
        public bool NpcTagsOnBottom = false;

        /// <summary>
        /// Configures the AggressiveNpcTag sprite loaded from the misc resources folder (rec size 32x20):
        /// </summary>
        public string AggressiveNpcTag = "NpcTag_Aggressive.png";

        /// <summary>
        /// Configures the AttackWhenAttackedNpcTag sprite loaded from the misc resources folder (rec size 32x20):
        /// </summary>
        public string AttackWhenAttackedNpcTag = "NpcTag_AttackWhenAttacked.png";

        /// <summary>
        /// Configures the AttackOnSightNpcTag sprite loaded from the misc resources folder (rec size 32x20):
        /// </summary>
        public string AttackOnSightNpcTag = "NpcTag_AttackOnSight.png";

        /// <summary>
        /// Configures the GuardNpcTag sprite loaded from the misc resources folder (rec size 32x20):
        /// </summary>
        public string GuardNpcTag = "NpcTag_Guard.png";

        /// <summary>
        /// Configures the NeutralNpcTag sprite loaded from the misc resources folder (rec size 32x20):
        /// </summary>
        public string NeutralNpcTag = "NpcTag_Neutral.png";
    }

}
