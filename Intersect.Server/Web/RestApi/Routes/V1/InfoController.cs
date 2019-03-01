using System.Web.Http;
using Intersect.Server.General;

namespace Intersect.Server.Web.RestApi.Routes.V1
{
    [RoutePrefix("info")]
    public class InfoController : ApiController
    {
        [Route]
        [HttpGet]
        public object Default()
        {
            return new
            {
                name = Options.GameName,
                port = Options.ServerPort,
            };
        }

        [Route("config")]
        [HttpGet]
        public object Config()
        {
            return new
            {
                name = Options.GameName,
                port = Options.ServerPort,
                upnp = Options.UPnP,
                openPortChecker = Options.OpenPortChecker,
                player = new
                {
                    maxStat = Options.Player.MaxStat,
                    maxLevel = Options.Player.MaxLevel,
                    maxInventory = Options.Player.MaxInventory,
                    maxSpells = Options.Player.MaxSpells,
                    maxBank = Options.Player.MaxBank,
                    maxCharacters = Options.Player.MaxCharacters,
                    itemDropChance = Options.Player.ItemDropChance,
                },
                passability = new
                {
                    normal = Options.Passability.Normal,
                    safe = Options.Passability.Safe,
                    arena = Options.Passability.Arena
                },
                equipment = new
                {
                    weaponSlot = Options.Equipment.WeaponSlot,
                    shieldSlot = Options.Equipment.ShieldSlot,
                    slots = Options.Equipment.Slots?.ToArray(),
                    paperdoll = new
                    {
                        up = Options.Equipment.Paperdoll?.Up?.ToArray(),
                        down = Options.Equipment.Paperdoll?.Down?.ToArray(),
                        left = Options.Equipment.Paperdoll?.Left?.ToArray(),
                        right = Options.Equipment.Paperdoll?.Right?.ToArray()
                    },
                    toolTypes = Options.Equipment.ToolTypes?.ToArray()
                },
                combat = new
                {
                    regenTime = Options.Combat.RegenTime,
                    minAttackRate = Options.Combat.MinAttackRate,
                    maxAttackRate = Options.Combat.MaxAttackRate,
                    blockingSlow = Options.Combat.BlockingSlow,
                    maxDashSpeed = Options.Combat.MaxDashSpeed
                },
                map = new
                {
                    gameBorderStyle = Options.Map.GameBorderStyle,
                    itemSpawnTime = Options.Map.ItemSpawnTime,
                    itemDespawnTime = Options.Map.ItemDespawnTime,
                    zDimensionVisible = Options.Map.ZDimensionVisible,
                    width = Options.Map.Width,
                    height = Options.Map.Height,
                    tileWidth = Options.Map.TileWidth,
                    tileHeight = Options.Map.TileHeight
                }
            };
        }

        [Route("stats")]
        [HttpGet]
        public object Stats()
        {
            return new
            {
                uptime = Globals.Timing.TimeMs,
                cps = Globals.Cps,
                connectedClients = Globals.Clients?.Count,
                onlineCount = Globals.OnlineList?.Count
            };
        }
    }
}