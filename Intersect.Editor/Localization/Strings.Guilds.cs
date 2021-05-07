using Intersect.Editor.Maps;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;
using Intersect.Localization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Editor.Localization
{
    public static partial class Strings
    {
        public struct EventCreateGuild
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Cancel = @"Cancel";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString SelectVariable = @"Player Variable containing Guild Name:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Okay = @"Ok";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Title = @"Create Guild";

        }

        public struct EventGuildCommands
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString createguild = @"Create Guild [Player Variable {00} as name]";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString guildcreated = @"Guild created successfully.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString guildfailed = @"Guild failed to create.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString endcreateguild = @"End Create Guild";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString disbandguild = @"Disband Guild";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString guildisbanded = @"Guild disbanded successfully.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString guilddisbandfailed = @"Guild failed to disband.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString enddisbandguild = @"End Disband Guild";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString openbank = @"Open Guild Bank";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString setguildbankslots = @"Set Guild Bank Slots";
        }

        public struct EventGuildSetBankSlotsCountCommand
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Variable = @"Variable";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString PlayerVariable = @"Player Variable";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString ServerVariable = @"Global Variable";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString cancel = @"Cancel";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString okay = @"Ok";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString title = @"Set Guild Bank Slots Count";
        }

        public struct GuildConditional
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Title = @"In Guild With At Least Rank...";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Rank = @"Rank:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
             public static LocalizedString InGuildWithRank = @"Player is in Guild with at least rank: {00}";

        }

        public static string GetEventConditionalDesc(InGuildWithRank condition)
        {
             return Strings.GuildConditional.InGuildWithRank.ToString(Intersect.Options.Instance.Guild.Ranks[Math.Max(0, Math.Min(Intersect.Options.Instance.Guild.Ranks.Length -1, condition.Rank))].Title);
        }

    }
}
