using Intersect.Localization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Server.Localization
{
    public static partial class Strings
    {
        public static GuildsNamespace Guilds => Root.Guilds;


        public sealed class GuildsNamespace : LocaleNamespace
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString VariableNotString = @"The given guild name does not contain any text.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString VariableNotMatchLength = @"A guild name needs to be between {00} and {01} characters!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString VariableNoText = @"A guild name can not be empty!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString GuildNameInUse = @"Your chosen guild name is already in use!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString AlreadyInGuild = @"You are already in a guild!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString Welcome = @"Welcome to {00}!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString NotInGuild = @"You are not in a guild.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString NotReceivedInvite = @"You've not received any guild invites yet.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString NotAllowed = @"You do not have the permission to do this.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString InviteNotOnline = @"The player you're trying to invite is either not online or does not exist.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString InviteAlreadyInGuild = @"The player you're trying to invite is already in a guild or has a pending invite.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString InviteSent = @"You've invited {00} to join {01}!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString InviteDeclinedResponse = @"{00} has declined your request for them to join {01}.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString InviteDeclined = @"You have declined the request to join {00}.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString GuildLeaderLeave = @"A Guildmaster can not leave their own guild, please transfer ownership rights first!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString NoSuchPlayer = @"There is no such player in this guild.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString Promoted = @"{00} has been promoted to {01}!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString PromotionFailed = @"{00} can not be promoted any further.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString Demoted = @"{00} has been demoted to {01}!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString DemotionFailed = @"{00} can not be demoted any further.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString DisbandGuild = @"{00} has been disbanded!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] 
            public readonly LocalizedString deleteguildleader = @"You can not delete a character that is a guild {00}, please disband the guild or transfer ownership before trying again.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString guildcmd = @"/guild";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString guildchat = @"[{00}] {01}: {02}";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString Transferred = @"Guild ownership of {00} has been transferred from {01} to {02}!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString NotAllowedWithdraw = @"You do not have permission to withdraw from {00}'s guild bank!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString NotAllowedDeposit = @"You do not have permission to deposit items into {00}'s guild bank!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString NotAllowedSwap = @"You do not have permission to swap items around within {00}'s guild bank!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString RankLimit = @"Failed to join {00} because their guild is full!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString RankLimitResponse = @"This guild has already hit it's member limit for the rank of {00}. Promote or demote other members in order to make room for {01}.";
        }
    }
}
