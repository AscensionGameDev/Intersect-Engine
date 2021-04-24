using Intersect.Localization;
using Intersect.Server.Core.CommandParsing;

using Newtonsoft.Json;

namespace Intersect.Server.Localization
{

    public static partial class Strings
    {

        public sealed class CommandsNamespace : LocaleCommandNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocaleCommand Announcement = new LocaleCommand
            {
                Name = @"announcement",
                Description = @"Sends a global message to all users playing the game.",
                Help = @"sends a global message to all players"
            };

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocaleCommand Api = new LocaleCommand
            {
                Name = @"api",
                Description =
                    @"Sets the api access (enabled/disabled) of a selected account. true is enabled, false is disabled",
                Help = @"enables or disables api access for an account"
            };

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocaleCommand ApiGrant = new LocaleCommand
            {
                Name = @"apigrant",
                Description = @"Grants extra api access roles for a user. (Options: users.query, users.manage)",
                Help = @"enables extra api access roles for a account. (Options: users.query, users.manage)"
            };

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocaleCommand ApiRevoke = new LocaleCommand
            {
                Name = @"apirevoke",
                Description = @"Revokes extra api access roles for a account. (Options: users.query, users.manage)",
                Help = @"revokes extra api access roles for a account. (Options: users.query, users.manage)"
            };

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocaleCommand ApiRoles = new LocaleCommand
            {
                Name = @"apiroles",
                Description = @"Lists extra api access roles assigned to a user.",
                Help = @"lists extra api access roles for an account (Options: users.query, users.manage)"
            };

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly ArgumentsNamespace Arguments = new ArgumentsNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocaleCommand Ban = new LocaleCommand
            {
                Name = @"ban",
                Description = @"Bans a player from the server.",
                Help = @"bans a player from the server"
            };

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocalizedString banuser = @"console";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocalizedString commandinfo = @"/?";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocaleCommand Cps = new LocaleCommand
            {
                Name = @"cps",
                Description =
                    @"Prints the current CPS.",
                Help = @"prints the current server cps"
            };

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocaleCommand Exit = new LocaleCommand
            {
                Name = @"exit",
                Description = @"Closes down the server.",
                Help = @"closes the server"
            };

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocalizedString exited =
                @"Main server shutdown completed. If your server is stuck you may safely kill it.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocalizedString exiting =
                @"Server is now closing. Please wait while your game and player data is saved!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocalizedString ExperimentalFlagNotFound = @"Experimental flag '{00}' was not found!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocaleCommand Experiments = new LocaleCommand
            {
                Name = @"experiments",
                Description = @"Sets the enablement of an experimental feature. true is enabled, false is disabled",
                Help = @"enables or disables an experimental feature"
            };

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocalizedString FlagInfo = @"(flag)";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocaleCommand Help = new LocaleCommand
            {
                Name = @"help",
                Description = @"help",
                Help = @"displays list of available commands"
            };

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocalizedString invalid = @"Invalid /command.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocaleCommand Kick = new LocaleCommand
            {
                Name = @"kick",
                Description = @"Kicks a player from the server.",
                Help = @"kicks a player from the server"
            };

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocaleCommand Kill = new LocaleCommand
            {
                Name = @"kill",
                Description = @"Kills a player on the server.",
                Help = @"kills a player on the server"
            };

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocalizedString madeprivate =
                @"The server has now been made private and can only be accessed by admins. To change this use the makepublic command or edit the adminonly field in config.json";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocalizedString madepublic =
                @"The server has now been made public and can be accessed by all players. To change this use the makepublic command or edit the adminonly field in config.json";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocaleCommand MakePrivate = new LocaleCommand
            {
                Name = @"makeprivate",
                Description = @"Makes the server private and can only be accessed by admins.",
                Help = @"Makes the server private and can only be accessed by admins."
            };

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocaleCommand MakePublic = new LocaleCommand
            {
                Name = @"makepublic",
                Description = @"Makes the server public to all players.",
                Help = @"Makes the server public to all players."
            };

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocaleCommand Metrics = new LocaleCommand
            {
                Name = @"metrics",
                Description =
                    @"Enables or disables metrics collection.",
                Help = @"enables or disables metrics collection"
            };

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocaleCommand Migrate = new LocaleCommand
            {
                Name = @"migrate",
                Description = @"Walks you through migrating your player or game database between sqlite and mysql.",
                Help = @"walks you through migrating your player or game database between sqlite and mysql"
            };

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocaleCommand Mute = new LocaleCommand
            {
                Name = @"mute",
                Description = @"mutes a player preventing them from talking.",
                Help = @"mutes a player preventing them from talking"
            };

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocalizedString muteuser = @"console";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocaleCommand NetDebug = new LocaleCommand
            {
                Name = @"netdebug",
            };

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocaleCommand OnlineList = new LocaleCommand
            {
                Name = @"onlinelist",
                Help = @"shows all players online"
            };

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly CommandParsingNamespace Parsing = new CommandParsingNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocaleCommand Power = new LocaleCommand
            {
                Name = @"power",
                Description =
                    @"Sets the power or access of a selected account. Power 0 is regular user. Power 1 is in-game moderator. Power 2 is owner/designer and allows editor access.",
                Help = @"sets the administrative access of a user"
            };

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocaleCommand PowerAccount = new LocaleCommand
            {
                Name = @"poweracc",
                Description =
                    @"Sets the power or access of a selected account. Power 0 is regular user. Power 1 is in-game moderator. Power 2 is owner/designer and allows editor access.",
                Help = @"sets the administrative access of an account"
            };

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocalizedString RequiredInfo = @"(required)";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocaleCommand Unban = new LocaleCommand
            {
                Name = @"unban",
                Description = @"Unbans a player from the server.",
                Help = @"unbans a player from the server"
            };

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]            public readonly LocaleCommand Unmute = new LocaleCommand
            {
                Name = @"unmute",
                Description = @"unmutes a player allowing them to talk.",
                Help = @"unmutes a player allowing them to talk"
            };

            public sealed class ArgumentsNamespace : LocaleNamespace
            {

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocaleArgument AccessBoolean = new LocaleArgument
                {
                    Name = @"access",
                    Description = @"whether or not to grant/revoke access"
                };

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocaleArgument AnnouncementMessage = new LocaleArgument
                {
                    Name = @"message",
                    Description = @"the message to send"
                };

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocaleArgument ApiRole = new LocaleArgument
                {
                    Name = @"role",
                    Description = @"role to grant or revoke (users.query or users.manage)"
                };

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocalizedString CpsLock = @"lock";

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocaleArgument CpsOperation = new LocaleArgument
                {
                    Name = @"operation",
                    Description = @"one of the following: status, lock, unlock"
                };

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocalizedString CpsStatus = @"status";

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocalizedString CpsUnlock = @"unlock";

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocaleArgument DurationBan = new LocaleArgument
                {
                    Name = @"duration",
                    Description = @"the duration to ban (in days)"
                };

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocaleArgument DurationMute = new LocaleArgument
                {
                    Name = @"duration",
                    Description = @"the duration to mute (in days)"
                };

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocaleArgument EnablementBoolean = new LocaleArgument
                {
                    Name = @"enablement",
                    Description = @"whether or not this is enabled"
                };

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocaleArgument Help = new LocaleArgument
                {
                    Name = @"help",
                    ShortName = 'h',
                    Description = @"shows this help message"
                };

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocaleArgument IpBan = new LocaleArgument
                {
                    Name = @"ip-ban",
                    Description = @"if it is an IP ban (true/false)"
                };

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocaleArgument IpMute = new LocaleArgument
                {
                    Name = @"ip-ban",
                    Description = @"if it is an IP mute (true/false)"
                };

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocalizedString MetricsDisable = @"disable";
                
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocaleArgument MetricsOperation = new LocaleArgument
                {
                    Name = @"operation",
                    Description = @"one of the following: disable, enable"
                };

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocalizedString MetricsEnable = @"enable";

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocaleArgument Power = new LocaleArgument
                {
                    Name = @"access",
                    Description = @"the access level to assign"
                };

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocaleArgument ReasonBan = new LocaleArgument
                {
                    Name = @"reason",
                    Description = @"the reason for the ban"
                };

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocaleArgument ReasonMute = new LocaleArgument
                {
                    Name = @"reason",
                    Description = @"the reason for the mute"
                };

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocaleArgument TargetApi = new LocaleArgument
                {
                    Name = @"account-name",
                    Description = @"the name of the acount to change the API access of"
                };

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocaleArgument TargetBan = new LocaleArgument
                {
                    Name = @"player-name",
                    Description = @"the name of the player to ban"
                };

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocaleArgument TargetExperimentalFeature = new LocaleArgument
                {
                    Name = @"experimental-feature-name-or-id",
                    Description = @"the Guid or name of the experimental feature to configure"
                };

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocaleArgument TargetKick = new LocaleArgument
                {
                    Name = @"player-name",
                    Description = @"the name of the player to kick"
                };

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocaleArgument TargetKill = new LocaleArgument
                {
                    Name = @"player-name",
                    Description = @"the name of the player to kill"
                };

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocaleArgument TargetMute = new LocaleArgument
                {
                    Name = @"player-name",
                    Description = @"the name of the player to mute"
                };

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocaleArgument TargetPower = new LocaleArgument
                {
                    Name = @"player-name",
                    Description = @"the name of the player to change the access of"
                };

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocaleArgument TargetPowerAccount = new LocaleArgument
                {
                    Name = @"account-name",
                    Description = @"the name of the acount to change the access of"
                };

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocaleArgument TargetUnban = new LocaleArgument
                {
                    Name = @"player-name",
                    Description = @"the name of the player to unban"
                };

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                public readonly LocaleArgument TargetUnmute = new LocaleArgument
                {
                    Name = @"player-name",
                    Description = @"the name of the player to unmute"
                };

            }

        }

    }

}
