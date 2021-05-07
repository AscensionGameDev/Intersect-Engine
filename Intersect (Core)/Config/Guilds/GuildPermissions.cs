namespace Intersect.Config.Guilds
{
    public class GuildPermissions
    {
        /// <summary>
        /// Defines whether or not this guild member can invite new members
        /// </summary>
        public bool Invite { get; set; }

        /// <summary>
        /// Defines whether or not this guild member can kick members (under their rank of course)
        /// </summary>
        public bool Kick { get; set; }

        /// <summary>
        /// Defines whether or not this guild member can promote other guilds members to ranks below this one
        /// </summary>
        public bool Promote { get; set; }

        /// <summary>
        /// Defined whether or not this guild member can demote other members who are below this rank
        /// </summary>
        public bool Demote { get; set; }

        /// <summary>
        /// Determines whether or not guild members of this rank can withdraw items from the guild bank
        /// </summary>
        public bool BankRetrieve { get; set; }

        /// <summary>
        /// Determines whether or not guild members of this rank can deposit items into the guild bank
        /// </summary>
        public bool BankDeposit { get; set; }

        /// <summary>
        /// Determines whether or not guild members of this rank can move items around within the guild bank
        /// </summary>
        public bool BankMove { get; set; }
    }
}
