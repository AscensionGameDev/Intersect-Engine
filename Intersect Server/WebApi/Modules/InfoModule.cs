using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Server.Classes.Entities;
using Intersect.Server.Classes.General;
using Nancy;
using Newtonsoft.Json.Linq;

using ServerOptions = Intersect.Options;

namespace Intersect.Server.WebApi.Modules
{
    using Sync = Func<dynamic, Response>;

    public class InfoModule : ServerModule
    {
        public InfoModule() : base("/info")
        {
            Get("/", (Sync)Get_Root);
            Get("/config", (Sync) Get_Config);
            Get("/stats", (Sync)Get_Stats);
        }

        private Response Get_Root(dynamic parameters) => Response.AsJson(new
            {
                name = ServerOptions.GameName,
                port = ServerOptions.ServerPort,
            }
        );

        private Response Get_Config(dynamic parameters) => Response.AsJson(new
            {
                name = ServerOptions.GameName,
                port = ServerOptions.ServerPort,
                upnp = ServerOptions.UPnP,
                openPortChecker = ServerOptions.OpenPortChecker,
                player = new
                {
                    maxStat = ServerOptions.Player.MaxStat,
                    maxLevel = ServerOptions.Player.MaxLevel,
                    maxInventory = ServerOptions.Player.MaxInventory,
                    maxSpells = ServerOptions.Player.MaxSpells,
                    maxBank = ServerOptions.Player.MaxBank,
                    maxCharacters = ServerOptions.Player.MaxCharacters,
                    itemDropChance = ServerOptions.Player.ItemDropChance,
                    progressSavedMessages = ServerOptions.Player.ProgressSavedMessages
                },
                passability = new
                {
                    normal = ServerOptions.Passability.Normal,
                    safe = ServerOptions.Passability.Safe,
                    arena = ServerOptions.Passability.Arena
                },
                equipment = new
                {
                    weaponSlot = ServerOptions.Equipment.WeaponSlot,
                    shieldSlot = ServerOptions.Equipment.ShieldSlot,
                    slots = ServerOptions.Equipment.Slots?.ToArray(),
                    paperdoll = new
                    {
                        up = ServerOptions.Equipment.Paperdoll?.Up?.ToArray(),
                        down = ServerOptions.Equipment.Paperdoll?.Down?.ToArray(),
                        left = ServerOptions.Equipment.Paperdoll?.Left?.ToArray(),
                        right = ServerOptions.Equipment.Paperdoll?.Right?.ToArray()
                    },
                    toolTypes = ServerOptions.Equipment.ToolTypes?.ToArray()
                },
                combat = new
                {
                    regenTime = ServerOptions.Combat.RegenTime,
                    minAttackRate = ServerOptions.Combat.MinAttackRate,
                    maxAttackRate = ServerOptions.Combat.MaxAttackRate,
                    blockingSlow = ServerOptions.Combat.BlockingSlow,
                    critChance = ServerOptions.Combat.CritChance,
                    critMultiplier = ServerOptions.Combat.CritMultiplier,
                    maxDashSpeed = ServerOptions.Combat.MaxDashSpeed
                },
                map = new
                {
                    gameBorderStyle = ServerOptions.Map.GameBorderStyle,
                    itemSpawnTime = ServerOptions.Map.ItemSpawnTime,
                    itemDespawnTime = ServerOptions.Map.ItemDespawnTime,
                    zDimensionVisible = ServerOptions.Map.ZDimensionVisible,
                    mapWidth = ServerOptions.Map.MapWidth,
                    mapHeight = ServerOptions.Map.MapHeight,
                    tileWidth = ServerOptions.Map.TileWidth,
                    tileHeight = ServerOptions.Map.TileHeight
                }
            }
        );

        private Response Get_Stats(dynamic parameters) => Response.AsJson(new
            {
                uptime = Globals.System?.GetTimeMs() ?? -1,
                cps = Globals.Cps,
                connectedClients = Globals.Clients?.Count,
                onlineCount = Globals.OnlineList?.Count
            }
        );
    }
}
