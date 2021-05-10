namespace Intersect.Config.Guilds
{
    /// <summary>
    /// Name and options for individual guild ranks
    /// </summary>
    public class GuildRank
    {
        /// <summary>
        /// Name of this rank
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Maximum number of guild members with this title
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Permissions that this rank holds for the guild and the actions they can take
        /// </summary>
        public GuildPermissions Permissions { get; set; } = new GuildPermissions();

    }
}
