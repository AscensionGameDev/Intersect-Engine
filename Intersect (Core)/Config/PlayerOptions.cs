using Intersect.Enums;

namespace Intersect.Config
{
	public class PlayerOptions
	{
		/// <summary>
		/// A percentage between 0 and 100 which determines the chance in which they will lose any given item in their inventory when killed.
		/// </summary>
		public int ItemDropChance
		{
			get;
			set;
		} = 0;

		/// <summary>
		/// Number of bank slots a player has.
		/// </summary>
		public int MaxBank
		{
			get;
			set;
		} = 100;

		/// <summary>
		/// Number of characters an account may create.
		/// </summary>
		public int MaxCharacters
		{
			get;
			set;
		} = 1;

		/// <summary>
		/// Number of inventory slots a player has.
		/// </summary>
		public int MaxInventory
		{
			get;
			set;
		} = 35;

		/// <summary>
		/// Max level a player can achieve.
		/// </summary>
		public int MaxLevel
		{
			get;
			set;
		} = 100;

		/// <summary>
		/// Number of spell slots a player has.
		/// </summary>
		public int MaxSpells
		{
			get;
			set;
		} = 35;

		/// <summary>
		/// The highest value a single stat can be for a player.
		/// </summary>
		public int MaxStat
		{
			get;
			set;
		} = 255;

		/// <summary>
		/// How long a player must wait before sending a trade/party/friend request after the first as been denied.
		/// </summary>
		public int RequestTimeout
		{
			get;
			set;
		} = 300000;

		/// <summary>
		/// Distance (in tiles) between players in which a trade offer can be sent and accepted.
		/// </summary>
		public int TradeRange
		{
			get;
			set;
		} = 6;

		/// <summary>
		/// Unlinks the timers for combat and movement to facilitate complex combat (e.g. kiting)
		/// </summary>
		public bool AllowCombatMovement
		{
			get;
			set;
		} = true;

		/// <summary>
		/// Configures whether or not the level of a player is shown next to their name.
		/// </summary>
		public bool ShowLevelByName
		{
			get;
			set;
		} = false;

		/// <summary>
		/// Configures whether or not to display Player's Tags.
		/// NOTE: Npc Tag Sprites are always loaded from the "tags" resource folder.
		/// *Recommended sizes: 32x16, 64x32, 128x64 and so on [2:1 px]
		/// </summary>
		public bool ShowTags
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
		/// Configures which Players should have a custom Tag. Only works if ShowTags = true.
		/// In order to set a custom tag for a specific Player, lets say, one named "Rick",
		/// add it's name to this string list, then create a custom tag named "Player_Rick.png"
		/// and place it inside the "tags" resource folder.
		/// </summary>
		public string[] CustomTagIcons
		{
			get;
			set;
		} = {
			"Aru",
			"Bobby",
			"Rick"
		};

	}
}
