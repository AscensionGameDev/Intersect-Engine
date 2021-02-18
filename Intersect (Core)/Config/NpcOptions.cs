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
        public bool AllowResetRadius { get; set; } = false;

        /// <summary>
        /// Configures the radius in which an NPC is allowed to move after starting to fight another entity.
        /// </summary>
        public int ResetRadius { get; set; } = 8;

        /// <summary>
        /// Configures whether or not the NPC is allowed to gain a new reset center point while it is still busy moving to its original reset point.
        /// NOTE: Can be used to allow the NPCs to be dragged far far away, as it constantly resets the center of its radius!!!
        /// </summary>
        public bool AllowNewResetLocationBeforeFinish { get; set; } = false;

        /// <summary>
        /// Configures whether or not the NPC should completely restore its vitals and statusses once it resets.
        /// </summary>
        public bool ResetVitalsAndStatusses { get; set; } = false;

        /// <summary>
        /// Configures whether or not the level of an Npc is shown next to their name.
        /// </summary>
        public bool ShowLevelByName { get; set; } = false;

        /// <summary>
        /// Configures whether or not to display Npc's Tags.
        /// NOTE: Npc Tag Sprites are always loaded from the "tags" resource folder (rec size 32x20).
        /// </summary>
        public bool Tags_Enable { get; set; } = false;

        /// <summary>
        /// Configures the position of the Npc Tags. Only works if ShowNpcTags = true.
        /// 0: NpcTags above the Npc's Name Label.
        /// 1: NpcTags under the Npc's Name Label.
        /// 2: NpcTags as prefix (left) of the Npc's Name Label.
        /// 3: NpcTags as suffix (right) of the Npc's Name Label.
        /// </summary>
        public int Tags_Position { get; set; } = 0;

        /// <summary>
        /// Configures the default tag icon sprite for AggressiveNpcTag.
        /// </summary>
        public string AggressiveTagIcon { get; set; } = "Aggressive.png";

        /// <summary>
        /// Configures the default tag icon sprite for AttackWhenAttacked.
        /// </summary>
        public string AttackWhenAttackedTagIcon { get; set; } = "AttackWhenAttacked.png";

        /// <summary>
        /// Configures the default tag icon sprite for AttackOnSight.
        /// </summary>
        public string AttackOnSightTagIcon { get; set; } = "AttackOnSight.png";

        /// <summary>
        /// Configures the default tag icon sprite for Guard.
        /// </summary>
        public string GuardTagIcon { get; set; } = "Guard.png";

        /// <summary>
        /// Configures default tag icon sprite for NeutralNpcTag.
        /// </summary>
        public string NeutralTagIcon { get; set; } = "Neutral.png";

        /// <summary>
        /// Configures which Npcs should have a Custom Npc Tag. Only works if ShowNpcTags = true.
        /// In order to change the default tag for a specific Npc, lets say, one named "Doe", add it's name to this string list
        /// then create a custom tag named "Doe.png" and place it inside the "tags" resource folder.
        /// </summary> 
        public string[] CustomTagIcons { get; set; } = { "Doe", "Monster", "Boss" };
    }

}