using Intersect.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Client.Localization
{
    public static partial class Strings
    {
        public struct Guilds
        {
            public static LocalizedString Guild = @"Guild";

            public static LocalizedString guildtip = "Send {00} an invite to your guild.";

            public static LocalizedString Invite = @"Invite";

            public static LocalizedString NotInGuild = @"You are not currently in a guild!";

            public static LocalizedString InviteMemberTitle = @"Invite Player";

            public static LocalizedString InviteMemberPrompt = @"Who would you like to invite to {00}?";

            public static LocalizedString InviteRequestTitle = @"Guild Invite";

            public static LocalizedString InviteRequestPrompt = @"{00} would like to invite you to join their guild, {01}. Do you accept?";

            public static LocalizedString Leave = "Leave";

            public static LocalizedString LeaveTitle = @"Leave Guild";

            public static LocalizedString LeavePrompt = @"Are you sure you would like to leave your guild?";

            public static LocalizedString Promote = @"Promote to {00}";

            public static LocalizedString Demote = @"Demote to {00}";

            public static LocalizedString Kick = @"Kick";

            public static LocalizedString PM = @"PM";

            public static LocalizedString Transfer = @"Transfer";

            public static LocalizedString OnlineListEntry = @"[{00}] {01} - {02}";

            public static LocalizedString OfflineListEntry = @"[{00}] {01} - {02}";

            public static LocalizedString Tooltip = @"Lv. {00} {01}";

            public static LocalizedString KickTitle = @"Kick Guild Member";

            public static LocalizedString KickPrompt = @"Are you sure you would like to kick {00}?";

            public static LocalizedString PromoteTitle = @"Promote Guild Member";

            public static LocalizedString PromotePrompt = @"Are you sure you would like promote {00} to rank {01}?";

            public static LocalizedString DemoteTitle = @"Demote Guild Member";

            public static LocalizedString DemotePrompt = @"Are you sure you would like to demote {00} to rank {01}?";

            public static LocalizedString TransferTitle = @"Transfer Guild";

            public static LocalizedString TransferPrompt = @"This action will completely transfer all ownership of your guild to {00} and you will lose your rank of {01}. If you are sure you want to hand over your guild enter '{02}' below.";

            public static LocalizedString Bank = @"{00} Guild Bank";

            public static LocalizedString NotAllowedWithdraw = @"You do not have permission to withdraw from {00}'s guild bank!";

            public static LocalizedString NotAllowedDeposit = @"You do not have permission to deposit items into {00}'s guild bank!";

            public static LocalizedString NotAllowedSwap = @"You do not have permission to swap items around within {00}'s guild bank!";
        }
    }
}
