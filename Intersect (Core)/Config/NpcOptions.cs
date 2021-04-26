using Intersect.Enums;

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
		public bool AllowResetRadius
		{
			get;
			set;
		} = false;

		/// <summary>
		/// Configures the radius in which an NPC is allowed to move after starting to fight another entity.
		/// </summary>
		public int ResetRadius
		{
			get;
			set;
		} = 8;

		/// <summary>
		/// Configures whether or not the NPC is allowed to gain a new reset center point while it is still busy moving to its original reset point.
		/// NOTE: Can be used to allow the NPCs to be dragged far far away, as it constantly resets the center of its radius!!!
		/// </summary>
		public bool AllowNewResetLocationBeforeFinish
		{
			get;
			set;
		} = false;

		/// <summary>
		/// Configures whether or not the NPC should completely restore its vitals and statusses once it resets.
		/// </summary>
		public bool ResetVitalsAndStatusses
		{
			get;
			set;
		} = false;

		/// <summary>
		/// Configures whether or not the level of an Npc is shown next to their name.
		/// </summary>
		public bool ShowLevelByName
		{
			get;
			set;
		} = false;

		/// <summary>
		/// Configures whether or not to ONLY allow Default Tags for Npcs.
		/// NOTE: default tags won't be rendered on NPCs anymore.
		/// </summary>
		public bool DefaultTagsOnly
		{
			get;
			set;
		} = false;

		/// <summary>
		/// Configures the position of the Tags. Only works if ShowTags = true.
		/// </summary>
		public TagPosition TagPosition
		{
			get;
			set;
		} = TagPosition.Above;

		/// <summary>
		/// Configures the default tag sprite for Aggressive Npcs.
		/// </summary>
		public string AggressiveTagIcon
		{
			get;
			set;
		} = "Aggressive.png";

		/// <summary>
		/// Configures the default tag sprite for AttackWhenAttacked Npcs.
		/// </summary>
		public string AttackWhenAttackedTagIcon
		{
			get;
			set;
		} = "AttackWhenAttacked.png";

		/// <summary>
		/// Configures the default tag sprite for AttackOnSight Npcs.
		/// </summary>
		public string AttackOnSightTagIcon
		{
			get;
			set;
		} = "AttackOnSight.png";

		/// <summary>
		/// Configures the default tag sprite for Guard Npcs.
		/// </summary>
		public string GuardTagIcon
		{
			get;
			set;
		} = "Guard.png";

		/// <summary>
		/// Configures default tag sprite for Neutral Npcs.
		/// </summary>
		public string NeutralTagIcon
		{
			get;
			set;
		} = "Neutral.png";

	}
}
