using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Server.Utilities;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Intersect.Server.Entities;
using Intersect.Server.General;

namespace Intersect.Server.WebApi.Modules
{
    public class PlayerModule : ServerModule
    {
        public static Player AsPlayer(EntityInstance entity) => entity as Player;

        public static JObject ToJson(Player player)
        {
            if (player == null)
                return null;

            return new JObject
            {
                {"name", player.Name},
                {"sprite", player.Sprite},
                {"level", player.Level},
                {"experience", player.Exp},
                {"experiencetnl", player.ExperienceToNextLevel},
                {"classId", player.ClassId},
                {"class", ClassBase.Get(player.ClassId)?.Name},
                {"map", player.MapId},
                {"location", new JObject {
                    {"x", player.X},
                    {"y", player.Y},
                    {"z", player.Z}
                }},
                {"facing", player.Dir},
                {"isdead", player.Dead},
                {"vitals", new JObject(
                    Enum.GetValues(typeof(Vitals))
                        .OfType<Vitals>()
                        .ToList()
                        .FindAll(vital => vital != Vitals.VitalCount)
                        .Select(vital => new JProperty(vital.ToString().ToLower(), player.GetVital(vital)))
                        .ToArray<object>()
                )},
                {"maxVitals", new JObject(
                    Enum.GetValues(typeof(Vitals))
                        .OfType<Vitals>()
                        .ToList()
                        .FindAll(vital => vital != Vitals.VitalCount)
                        .Select(vital => new JProperty(vital.ToString().ToLower(), player.GetMaxVital(vital)))
                        .ToArray<object>()
                )},
                {"stats", new JObject(
                    Enum.GetValues(typeof(Stats))
                        .OfType<Stats>()
                        .ToList()
                        .FindAll(stat => stat != Stats.StatCount)
                        .Select(stat => new JProperty(stat.ToString().ToLower(), player.Stat?[(int)stat]?.Value()))
                        .ToArray<object>()
                )}
            };
        }

        public static List<Player> OnlinePlayers => Globals.OnlineList?.Select(AsPlayer).ToList();

        [NotNull]
        public static JObject GetOnlinePlayers(int page, int pageSize, string sortKey, bool ascending, params string[] filterKeys)
        {
            var pageCount = Math.Max(1, OnlinePlayers?.Count / Math.Max(pageSize, 1) ?? 1);
            var startPage = Math.Max(1, (page % pageCount + pageCount) % pageCount);
            var start = (startPage - 1) * pageSize;
            var count = Math.Min(pageSize, OnlinePlayers?.Count - start ?? 0);

            var result = new JObject
            {
                {"page", startPage},
                {"pageCount", pageCount},
                {"count", count},
                {"totalCount", OnlinePlayers?.Count},
                {"entries", new JArray(
                    SortHelper.OrderBy(OnlinePlayers?.Select(ToJson), player => player?[sortKey], ascending)?.ToList().GetRange(start, count)
                )}
            };

            return FilterPage(result, "name", filterKeys);
        }

        [NotNull]
        public static JObject FilterPage([NotNull] JObject page, string primaryKey, params string[] keys) => new JObject
        {
            {"page", page["page"]},
            {"pageCount", page["pageCount"]},
            {"count", page["count"]},
            {"totalCount", page["totalCount"]},
            {
                "entries", (
                    (keys?.Length ?? 0) == 0 ? page["entries"] : new JArray(
                        (page["entries"] as JArray)?
                        .ToList()
                        .Cast<JObject>()
                        .Select(
                            entry => new JObject(
                                entry?.Properties()?.ToList().FindAll(property =>
                                    string.Equals(primaryKey, property?.Name, StringComparison.InvariantCultureIgnoreCase)
                                    || (keys?.Contains(property?.Name) ?? false)
                                ).ToArray<object>()
                            )
                        )
                    )
                )
            }
        };

        public static JArray OnlinePlayersArray => new JArray(OnlinePlayers?.Select(ToJson));

        public PlayerModule() : base("/players")
        {
            Get("/{name}", parameters =>
            {
                var playerEntity = OnlinePlayers?.Find(entity => string.Equals(entity?.Name, parameters?.name,
                    StringComparison.InvariantCultureIgnoreCase));
                if (playerEntity == null)
                    return new JObject
                    {
                        {"error", $"No online player found with the name '{parameters?.name}'"}
                    };

                return ToJson(AsPlayer(playerEntity))?.ToString();
            });

            Get("/online", parameters => GetOnlinePlayers(
                Query<int>(page => Math.Max(1, page), 1, "page", "p"),
                Query<int>(page => Math.Max(10, page), 10, "pageSize", "ps"),
                Query<string>(value => string.IsNullOrWhiteSpace(value) ? null : value, "name", "sortKey", "by"),
                SortHelper.ToSortOrder(Query("asc", "sortOrder", "order")) == SortOrder.Ascending,
                Query<string>(value => string.IsNullOrWhiteSpace(value) ? null : value, null, "select", "keys")?.Split(',')
                ).ToString());
        }
    }
}
