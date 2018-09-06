using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Intersect.Enums;
using Intersect.Migration.UpgradeInstructions.Upgrade_12;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Events;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Config;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Database.GameData;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Conditions;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Crafting;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Events;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Events.Commands;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Maps;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Maps.MapList;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Switches_and_Variables;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Utilities;
using Intersect.Server.Classes.Database.PlayerData;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using EventBase = Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Events.EventBase;
using EventCommandType = Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.GameObjects.Events.EventCommandType;
using EventMoveRoute = Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Events.EventMoveRoute;
using EventPage = Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Events.EventPage;
using ShopBase = Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.ShopBase;
using Intersect.Migration.UpgradeInstructions.Upgrade_11;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Database.PlayerData;
using Intersect.Server.Classes.Database.PlayerData.Characters;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_12
{
    public enum SwitchVariableTypes
    {
        PlayerSwitch = 0,
        PlayerVariable,
        ServerSwitch,
        ServerVariable,
    } 
    public class Upgrade12
    {
        Dictionary<GameObjectType, Dictionary<int, string>> objs = new Dictionary<GameObjectType, Dictionary<int, string>>();
        Dictionary<Guid,Dictionary<int,Guid>> questTaskIds = new Dictionary<Guid, Dictionary<int, Guid>>();
        private Dictionary<int, User11> OldUsers;
        private Dictionary<int, User> UserMap = new Dictionary<int, User>();
        private Dictionary<int, Character11> OldCharacters;
        private Dictionary<int, Player> CharacterMap = new Dictionary<int, Player>();
        private List<Ban11> OldBans = new List<Ban11>();
        private List<Mute11> OldMutes = new List<Mute11>();
        private List<Friend11> OldFriends = new List<Friend11>();
        private List<PSwitch11> OldSwitches = new List<PSwitch11>();
        private List<PVar11> OldVariables = new List<PVar11>();
        private List<Quest11> OldQuests = new List<Quest11>();
        private List<Spell11> OldSpells = new List<Spell11>();
        private List<Bag11> OldBags = new List<Bag11>();
        private List<Inventory11> OldItems = new List<Inventory11>();
        private List<Bank11> OldBanks = new List<Bank11>();
        private List<BagItem11> OldBagItems = new List<BagItem11>();
        private List<Hotbar11> OldHotbar = new List<Hotbar11>();
        private Dictionary<int, Bag> BagMap = new Dictionary<int, Bag>();
        private Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.GameObjects.Maps.MapList.MapList oldMapList;
        private PlayerContext sPlayerDb;
        private GameContext sGameDb;
        private const string GameDbFilename = "resources/gamedata.db";
        private const string PlayersDbFilename = "resources/playerdata.db";
        private long _tc = DateTime.Now.ToBinary();
        private bool playerDbSqlite = true;
        private bool gameDbSqlite = true;

        private long TimeCreated
        {
            get
            {
                _tc++;
                return _tc;
            }
        }

        public Upgrade12(Dictionary<Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.Enums.GameObjectType, Dictionary<int, string>> up11objs, Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.GameObjects.Maps.MapList.MapList mapList,
            Dictionary<int,User11> oldUsers, Dictionary<int,Character11> oldCharacters, List<Ban11> oldBans, List<Mute11> oldMutes, List<Friend11> oldFriends, List<PSwitch11> oldSwitches, List<PVar11> oldVariables,
            List<Quest11> oldQuests, List<Spell11> oldSpells, List<Bag11> oldBags, List<Inventory11> oldItems, List<Bank11> oldBanks, List<BagItem11> oldBagItems, List<Hotbar11> oldHotbar)
        {

            if (File.Exists(GameDbFilename)) File.Delete(GameDbFilename);
            if (File.Exists(PlayersDbFilename)) File.Delete(PlayersDbFilename);

            foreach (var val in up11objs)
            {
                var typ = GameObjectType.Event;
                if (val.Key.ToString() != "CommonEvent")
                    typ = (GameObjectType)Enum.Parse(typeof(GameObjectType), val.Key.ToString());
                objs.Add(typ,val.Value);
            }

            oldMapList = mapList;
            OldUsers = oldUsers;
            OldCharacters = oldCharacters;
            OldBans = oldBans;
            OldMutes = oldMutes;
            OldFriends = oldFriends;
            OldSwitches = oldSwitches;
            OldVariables = oldVariables;
            OldQuests = oldQuests;
            OldSpells = oldSpells;
            OldBags = oldBags;
            OldItems = oldItems;
            OldBanks = oldBanks;
            OldBagItems = oldBagItems;
            OldHotbar = oldHotbar;

            //TODO: On screen prompts to see if they want to use sqlite or mysql...
            //We will officially recommend sqlite for BOTH since the api will be available... but this will be their call. Going back and forth will be difficult to say the least.

            //MySql Connection Settings
            var playerDbMySqlConnInfo = new MySqlDbConnInfo();
            var gameDbMySqlConnInfo = new MySqlDbConnInfo();



            //Connect to new player database
            if (playerDbSqlite)
            {
                sPlayerDb = new PlayerContext(DatabaseUtils.DbProvider.Sqlite, $"Data Source={PlayersDbFilename}");
            }
            else
            {
                sPlayerDb = new PlayerContext(DatabaseUtils.DbProvider.MySql, $"server={playerDbMySqlConnInfo.Server}:{playerDbMySqlConnInfo.Port};database={playerDbMySqlConnInfo.Database};user={playerDbMySqlConnInfo.User};password={playerDbMySqlConnInfo.Password}");
            }
            sPlayerDb.Database.Migrate();

            if (gameDbSqlite)
            {
                sGameDb = new GameContext(DatabaseUtils.DbProvider.Sqlite, $"Data Source={GameDbFilename}");
            }
            else
            {
                sGameDb = new GameContext(DatabaseUtils.DbProvider.MySql, $"server={gameDbMySqlConnInfo.Server}:{gameDbMySqlConnInfo.Port};database={gameDbMySqlConnInfo.Database};user={gameDbMySqlConnInfo.User};password={gameDbMySqlConnInfo.Password}");
            }
            sGameDb.Database.Migrate();
        }

        public void Upgrade()
        {
            Options._options = new Options();
            Options.ServerPort = Upgrade_11.Intersect_Convert_Lib.Options.ServerPort;
            Options._options._gameName = Upgrade_11.Intersect_Convert_Lib.Options.GameName;
            Options.Player.MaxStat = Upgrade_11.Intersect_Convert_Lib.Options.MaxStatValue;
            Options.Player.MaxLevel = Upgrade_11.Intersect_Convert_Lib.Options.MaxLevel;
            Options.Player.MaxInventory = Upgrade_11.Intersect_Convert_Lib.Options.MaxInvItems;
            Options.Player.MaxSpells = Upgrade_11.Intersect_Convert_Lib.Options.MaxPlayerSkills;
            Options.Player.MaxBank = Upgrade_11.Intersect_Convert_Lib.Options.MaxBankSlots;
            Options.Player.MaxCharacters = Upgrade_11.Intersect_Convert_Lib.Options.MaxCharacters;
            Options.Player.ItemDropChance = Upgrade_11.Intersect_Convert_Lib.Options.ItemDropChance;
            Options.Equipment.WeaponSlot = Upgrade_11.Intersect_Convert_Lib.Options.WeaponIndex;
            Options.Equipment.ShieldSlot = Upgrade_11.Intersect_Convert_Lib.Options.ShieldIndex;

            Options._options.EquipmentOpts.Slots = Upgrade_11.Intersect_Convert_Lib.Options.EquipmentSlots;

            Options._options.EquipmentOpts.Paperdoll.Up.Clear();
            Options._options.EquipmentOpts.Paperdoll.Up.Add("Player");
            Options._options.EquipmentOpts.Paperdoll.Up.AddRange(Upgrade_11.Intersect_Convert_Lib.Options.PaperdollOrder[0].ToArray());

            Options._options.EquipmentOpts.Paperdoll.Down.Clear();
            Options._options.EquipmentOpts.Paperdoll.Down.Add("Player");
            Options._options.EquipmentOpts.Paperdoll.Down.AddRange(Upgrade_11.Intersect_Convert_Lib.Options.PaperdollOrder[1].ToArray());

            Options._options.EquipmentOpts.Paperdoll.Left.Clear();
            Options._options.EquipmentOpts.Paperdoll.Left.Add("Player");
            Options._options.EquipmentOpts.Paperdoll.Left.AddRange(Upgrade_11.Intersect_Convert_Lib.Options.PaperdollOrder[2].ToArray());

            Options._options.EquipmentOpts.Paperdoll.Right.Clear();
            Options._options.EquipmentOpts.Paperdoll.Right.Add("Player");
            Options._options.EquipmentOpts.Paperdoll.Right.AddRange(Upgrade_11.Intersect_Convert_Lib.Options.PaperdollOrder[3].ToArray());
            
            Options._options.EquipmentOpts.ToolTypes = Upgrade_11.Intersect_Convert_Lib.Options.ToolTypes;
            Options._options._animatedSprites = Upgrade_11.Intersect_Convert_Lib.Options.AnimatedSprites;
            Options.Passability.Passable = Upgrade_11.Intersect_Convert_Lib.Options.PlayerPassable;
            Options.Combat.MinAttackRate = Upgrade_11.Intersect_Convert_Lib.Options.MinAttackRate;
            Options.Combat.MaxAttackRate = Upgrade_11.Intersect_Convert_Lib.Options.MaxAttackRate;
            Options.Combat.BlockingSlow = (int)Upgrade_11.Intersect_Convert_Lib.Options.BlockingSlow;
            Options.Combat.MaxDashSpeed = Upgrade_11.Intersect_Convert_Lib.Options.MaxDashSpeed;
            Options.Map.GameBorderStyle = Upgrade_11.Intersect_Convert_Lib.Options.GameBorderStyle;
            Options.Map.ZDimensionVisible = Upgrade_11.Intersect_Convert_Lib.Options.ZDimensionVisible;
            Options.Map.Width = Upgrade_11.Intersect_Convert_Lib.Options.MapWidth;
            Options.Map.TileWidth = Upgrade_11.Intersect_Convert_Lib.Options.TileWidth;
            Options.Map.Height = Upgrade_11.Intersect_Convert_Lib.Options.MapHeight;
            Options.Map.TileHeight = Upgrade_11.Intersect_Convert_Lib.Options.TileHeight;
            Options.Player.ProgressSavedMessages = false;
            Options._options._apiPort = Options.ServerPort;

            File.WriteAllText("resources/config.json", JsonConvert.SerializeObject(Options._options, Formatting.Indented));


            //Generate Quest Task Ids
            GenerateQuestTaskIds(objs[GameObjectType.Quest]);

            ImportTilesets(objs[GameObjectType.Tileset]);
            ImportAnimations(objs[GameObjectType.Animation]);
            ImportPlayerSwitches(objs[GameObjectType.PlayerSwitch]);
            ImportPlayerVariables(objs[GameObjectType.PlayerVariable]);
            ImportServerSwitches(objs[GameObjectType.ServerSwitch]);
            ImportServerVariables(objs[GameObjectType.ServerVariable]);
            ImportTime(objs[GameObjectType.Time][0]);
            ImportCrafts(objs[GameObjectType.Crafts]);
            ImportCraftTables(objs[GameObjectType.CraftTables]);
            ImportProjectiles(objs[GameObjectType.Projectile]);
            ImportResources(objs[GameObjectType.Resource]);
            ImportShops(objs[GameObjectType.Shop]);
            ImportClasses(objs[GameObjectType.Class]);
            ImportItems(objs[GameObjectType.Item]);
            ImportSpells(objs[GameObjectType.Spell]);
            ImportNpcs(objs[GameObjectType.Npc]);
            ImportEvents(objs[GameObjectType.Event]);
            ImportQuests(objs[GameObjectType.Quest]);
            ImportMaps(objs[GameObjectType.Map]);
            ImportMapFolders();

            //--------------------------------------------------------------

            //Player Data!!

            //--------------------------------------------------------------

            ImportUsers();
            ImportCharacters();
            ImportBans();
            ImportMutes();
            ImportFriends();
            ImportPSwitches();
            ImportPVariables();
            ImportPQuests();
            ImportPSpells();
            ImportBags();
            ImportPItems();
            ImportBankItems();
            ImportBagItems();
            ImportHotbar();

            sGameDb.SaveChanges();
            sPlayerDb.SaveChanges();
            if (gameDbSqlite) sGameDb.Database.ExecuteSqlCommand("VACUUM;");
            if (playerDbSqlite) sPlayerDb.Database.ExecuteSqlCommand("VACUUM;");
            sGameDb.SaveChanges();
            sPlayerDb.SaveChanges();
            Console.WriteLine("Done converting to new db! Hit any key to continue");
            Console.ReadKey();
            Environment.Exit(-1);
        }

        #region "PlayerData"
        private void ImportHotbar()
        {
            foreach (var bn in OldHotbar)
            {
                if (CharacterMap.ContainsKey(bn.playerid))
                {
                    var player = CharacterMap[bn.playerid];
                    var slot = player.Hotbar[bn.slot];
                    slot.Type = bn.type;
                    slot.ItemSlot = bn.itemslot;
                }
            }
        }
        private void ImportBagItems()
        {
            foreach (var bn in OldBagItems)
            {
                if (BagMap.ContainsKey(bn.bagid) && GetGuid(GameObjectType.Item, bn.itemnum) != Guid.Empty)
                {
                    var slot = BagMap[bn.bagid].Slots[bn.slot];
                    slot.ItemId = GetGuid(GameObjectType.Item, bn.itemnum);
                    slot.Quantity = bn.itemval;
                    slot.StatBoost = bn.itemstats;
                    var item = sGameDb.Items.Find(GetGuid(GameObjectType.Item, bn.itemnum));
                    if (item.ItemType == ItemTypes.Bag && bn.bagid != bn.item_bag_id)
                    {
                        if (BagMap.ContainsKey(bn.item_bag_id))
                        {
                            slot.Bag = BagMap[bn.item_bag_id];
                        }
                    }
                }
            }
        }
        private void ImportBankItems()
        {
            foreach (var bn in OldBanks)
            {
                if (CharacterMap.ContainsKey(bn.playerid) && GetGuid(GameObjectType.Item, bn.itemnum) != Guid.Empty)
                {
                    var player = CharacterMap[bn.playerid];
                    var slot = player.Bank[bn.slot];
                    slot.ItemId = GetGuid(GameObjectType.Item, bn.itemnum);
                    slot.Quantity = bn.itemval;
                    slot.StatBoost = bn.itemstats;
                    var item = sGameDb.Items.Find(GetGuid(GameObjectType.Item, bn.itemnum));
                    if (item.ItemType == ItemTypes.Bag)
                    {
                        if (BagMap.ContainsKey(bn.item_bag_id))
                        {
                            slot.Bag = BagMap[bn.item_bag_id];
                        }
                    }
                }
            }
        }
        private void ImportPItems()
        {
            foreach (var bn in OldItems)
            {
                if (CharacterMap.ContainsKey(bn.playerid) && GetGuid(GameObjectType.Item, bn.itemnum) != Guid.Empty)
                {
                    var player = CharacterMap[bn.playerid];
                    var slot = player.Items[bn.slot];
                    slot.ItemId = GetGuid(GameObjectType.Item, bn.itemnum);
                    slot.Quantity = bn.itemval;
                    slot.StatBoost = bn.itemstats;
                    var item = sGameDb.Items.Find(GetGuid(GameObjectType.Item, bn.itemnum));
                    if (item.ItemType == ItemTypes.Bag)
                    {
                        if (BagMap.ContainsKey(bn.item_bag_id))
                        {
                            slot.Bag = BagMap[bn.item_bag_id];
                        }
                    }
                }
            }
        }
        private void ImportBags()
        {
            foreach (var bn in OldBags)
            {
                var bag = new Bag(bn.slots);
                bag.Id = Guid.NewGuid();
                BagMap.Add(bn.bagid,bag);
                sPlayerDb.Bags.Add(bag);
            }
        }
        private void ImportPSpells()
        {
            foreach (var bn in OldSpells)
            {
                if (CharacterMap.ContainsKey(bn.playerid) && GetGuid(GameObjectType.Spell, bn.spellid) != Guid.Empty)
                {
                    var player = CharacterMap[bn.playerid];
                    var slot = player.Spells[bn.slot];
                    slot.SpellId = GetGuid(GameObjectType.Spell, bn.spellid);
                    slot.SpellCd = bn.spellcd;
                }
            }
        }
        private void ImportPQuests()
        {
            foreach (var bn in OldQuests)
            {
                if (CharacterMap.ContainsKey(bn.playerid) && GetGuid(GameObjectType.Quest, bn.questid) != Guid.Empty)
                {
                    var fr = new Quest(GetGuid(GameObjectType.Quest, bn.questid));
                    fr.TaskId = GetQuestTaskId(GetGuid(GameObjectType.Quest, bn.questid), bn.taskid);
                    fr.TaskProgress = bn.taskprogress;
                    fr.Completed = bn.completed;
                    CharacterMap[bn.playerid].Quests.Add(fr);
                }
            }
        }
        private void ImportPVariables()
        {
            foreach (var bn in OldVariables)
            {
                if (CharacterMap.ContainsKey(bn.playerid) && GetGuid(GameObjectType.PlayerVariable, bn.variableid) != Guid.Empty)
                {
                    var fr = new Variable(GetGuid(GameObjectType.PlayerVariable, bn.variableid));
                    fr.Value = bn.value;
                    CharacterMap[bn.playerid].Variables.Add(fr);
                }
            }
        }
        private void ImportPSwitches()
        {
            foreach (var bn in OldSwitches)
            {
                if (CharacterMap.ContainsKey(bn.playerid) && GetGuid(GameObjectType.PlayerSwitch,bn.switchid) != Guid.Empty)
                {
                    var fr = new Switch(GetGuid(GameObjectType.PlayerSwitch, bn.switchid));
                    fr.Value = bn.value;
                    CharacterMap[bn.playerid].Switches.Add(fr);
                }
            }
        }
        private void ImportFriends()
        {
            foreach (var bn in OldFriends)
            {
                if (CharacterMap.ContainsKey(bn.owner_id) && CharacterMap.ContainsKey(bn.friend_id))
                {
                    var fr = new Friend();
                    fr.Owner = CharacterMap[bn.owner_id];
                    fr.Target = CharacterMap[bn.friend_id];
                    sPlayerDb.Character_Friends.Add(fr);
                }
            }
        }
        private void ImportMutes()
        {
            foreach (var bn in OldMutes)
            {
                if (UserMap.ContainsKey(bn.accountid))
                {
                    var usr = UserMap[bn.accountid];
                    var mute = new Mute();
                    mute.Muter = bn.muter;
                    mute.Ip = bn.ip;
                    mute.EndTime = DateTime.FromBinary(bn.unmutetime);
                    mute.StartTime = DateTime.FromBinary(bn.mutetime);
                    mute.Player = usr;
                    mute.Reason = bn.reason;
                    sPlayerDb.Mutes.Add(mute);
                }
            }
        }
        private void ImportBans()
        {
            foreach (var bn in OldBans)
            {
                if (UserMap.ContainsKey(bn.accountid))
                {
                    var usr = UserMap[bn.accountid];
                    var ban = new Ban();
                    ban.Banner = bn.banner;
                    ban.Ip = bn.ip;
                    ban.EndTime = DateTime.FromBinary(bn.unbantime);
                    ban.StartTime = DateTime.FromBinary(bn.bantime);
                    ban.Player = usr;
                    ban.Reason = bn.reason;
                    sPlayerDb.Bans.Add(ban);
                }
            }
        }
        private void ImportCharacters()
        {
            foreach (var usr in OldCharacters)
            {
                var old = usr.Value;
                var pl = new Player();
                pl.FixLists();
                pl.Id = Guid.NewGuid();
                CharacterMap.Add(usr.Key, pl);

                pl.Name = old.name;
                pl.MapId = GetGuid(GameObjectType.Map, old.map);
                pl.X = old.x;
                pl.Y = old.y;
                pl.Z = old.z;
                pl.Dir = old.dir;
                pl.Sprite = old.sprite;
                pl.Face = old.face;
                pl.ClassId = GetGuid(GameObjectType.Class, old.classid);
                pl.Gender = (Gender)old.gender;
                pl.Level = old.level;
                pl.Exp = old.exp;
                pl._vital = old.vitals;

                //TODO: Math here in order to decouple player stats from class base stats (yuck!)
                pl.BaseStat = old.stats;
                pl.StatPoints = old.statpoints;
                pl.Equipment = old.equipment;
                pl.LastOnline = DateTime.FromBinary(old.last_online);

                if (UserMap.ContainsKey(old.userid))
                {
                    UserMap[old.userid].Characters.Add(pl);
                }
            }
        }
        private void ImportUsers()
        {
            foreach (var usr in OldUsers)
            {
                var newUsr = new User();
                newUsr.Id = Guid.NewGuid();
                UserMap.Add(usr.Key,newUsr);
                newUsr.Name = usr.Value.user;
                newUsr.Access = (Access) usr.Value.power;
                newUsr.Password = usr.Value.pass;
                newUsr.Salt = usr.Value.salt;
                newUsr.Email = usr.Value.email;
                sPlayerDb.Users.Add(newUsr);
            }
        }
        #endregion

        #region "GameData"
        private void ImportMapFolders()
        {
            var mapList = new MapList();
            ImportMapListItems(mapList.Items, oldMapList.Items);
            sGameDb.MapFolders.Add(mapList);
        }
        private void ImportMapListItems(List<MapListItem> addTo, List<Upgrade_11.Intersect_Convert_Lib.GameObjects.Maps.MapList.MapListItem> addFrom)
        {
            foreach (var itm in addFrom)
            {
                if (itm.Type == 0) //Folder
                {
                    var folder = (Upgrade_11.Intersect_Convert_Lib.GameObjects.Maps.MapList.MapListFolder) itm;
                    var newFolder = new MapListFolder();
                    newFolder.Name = folder.Name;
                    newFolder.FolderId = Guid.NewGuid();
                    ImportMapListItems(newFolder.Children.Items,folder.Children.Items);
                    addTo.Add(newFolder);
                }
                else if (itm.Type == 1) //Map
                {
                    var map = (Upgrade_11.Intersect_Convert_Lib.GameObjects.Maps.MapList.MapListMap)itm;
                    var newMap = new MapListMap();
                    newMap.Name = map.Name;
                    newMap.MapId = GetGuid(GameObjectType.Map, map.MapNum);
                    newMap.Type = 1;
                    var mapFromDb = sGameDb.Maps.Find(newMap.MapId);
                    if (mapFromDb != null)
                    {
                        newMap.TimeCreated = mapFromDb.TimeCreated;
                        addTo.Add(newMap);
                    }
                }
            }
        }
        private Guid GetQuestTaskId(Guid questId, int taskId)
        {
            if (questTaskIds.ContainsKey(questId))
            {
                if (questTaskIds[questId].ContainsKey(taskId))
                    return questTaskIds[questId][taskId];
            }
            return Guid.Empty;
        }
        private void GenerateQuestTaskIds(Dictionary<int, string> jsns)
        {
            questTaskIds.Clear();
            foreach (var qst in jsns)
            {
                var data = JObject.Parse(qst.Value);
                var qstId = GetGuid(GameObjectType.Quest, qst.Key);
                questTaskIds.Add(qstId,new Dictionary<int, Guid>());
                var tasks = JArray.Parse(data["Tasks"].ToString());
                foreach (var task in tasks)
                {
                    var id = int.Parse(task["Id"].ToString());
                    questTaskIds[qstId].Add(id,Guid.NewGuid());
                }

            }
        }
        private void ImportQuests(Dictionary<int, string> jsns)
        {
            foreach (var jsn in jsns)
            {
                var data = JObject.Parse(jsn.Value);
                var itm = new QuestBase(Guid.Parse(data["Guid"].ToString()));
                itm.TimeCreated = TimeCreated;

                itm.Name = data["Name"].ToString();

                itm.BeforeDescription = data["BeforeDesc"].ToString();
                itm.EndDescription = data["EndDesc"].ToString();

                var endEventId = Guid.NewGuid();
                itm.EndEventId = endEventId;
                ImportEvent(endEventId, data["EndEvent"].ToString(), Guid.Empty, -1, -1, null);

                itm.InProgressDescription = data["InProgressDesc"].ToString();
                itm.LogAfterComplete = Convert.ToBoolean(int.Parse(data["LogAfterComplete"].ToString()));
                itm.LogBeforeOffer = Convert.ToBoolean(int.Parse(data["LogBeforeOffer"].ToString()));
                itm.Quitable = Convert.ToBoolean(int.Parse(data["Quitable"].ToString()));
                itm.Repeatable = Convert.ToBoolean(int.Parse(data["Repeatable"].ToString()));
                itm.Requirements = ImportConditionLists(data["Requirements"].ToString());
                itm.StartDescription = data["StartDesc"].ToString();

                var startEventId = Guid.NewGuid();
                itm.StartEventId = startEventId;
                ImportEvent(startEventId, data["StartEvent"].ToString(), Guid.Empty, -1, -1, null);

                var tasks = JArray.Parse(data["Tasks"].ToString());
                foreach (var task in tasks)
                {
                    var tsk = new QuestBase.QuestTask(GetQuestTaskId(itm.Id,int.Parse(task["Id"].ToString())));
                    tsk.Objective = (QuestObjective) int.Parse(task["Objective"].ToString());
                    if (tsk.Objective == QuestObjective.GatherItems)
                    {
                        tsk.TargetId = GetGuid(GameObjectType.Item, int.Parse(task["Data1"].ToString()));
                        tsk.Quantity = int.Parse(task["Data2"].ToString());
                    }
                    else if (tsk.Objective == QuestObjective.KillNpcs)
                    {
                        tsk.TargetId = GetGuid(GameObjectType.Npc, int.Parse(task["Data1"].ToString()));
                        tsk.Quantity = int.Parse(task["Data2"].ToString());
                    }
                    tsk.Description = task["Desc"].ToString();

                    var completionEventId = Guid.NewGuid();
                    tsk.CompletionEventId = completionEventId;
                    ImportEvent(completionEventId, task["CompletionEvent"].ToString(), Guid.Empty, -1, -1, null);

                    itm.Tasks.Add(tsk);
                }

                sGameDb.Quests.Add(itm);
            }
        }
        private void ImportMaps(Dictionary<int, string> jsns)
        {
            foreach (var jsn in jsns)
            {
                var data = JObject.Parse(jsn.Value);
                var itm = new MapInstance(Guid.Parse(data["Guid"].ToString()));
                itm.TimeCreated = TimeCreated;
                itm.Name = data["Name"].ToString();

                //Tiledata
                var tileData = Convert.FromBase64String(data["TileData"].ToString());
                var bf = new ByteBuffer();
                var bfNew = new ByteBuffer();
                bf.WriteBytes(tileData);
                itm.Layers = new TileArray[Options.LayerCount];
                for (var i = 0; i < Options.LayerCount; i++)
                {
                    itm.Layers[i].Tiles = new Tile[Options.MapWidth, Options.MapHeight];
                    for (var x = 0; x < Options.MapWidth; x++)
                    {
                        for (var y = 0; y < Options.MapHeight; y++)
                        {
                            bfNew.WriteGuid(GetGuid(GameObjectType.Tileset, bf.ReadInteger()));
                            bfNew.WriteInteger(bf.ReadInteger());
                            bfNew.WriteInteger(bf.ReadInteger());
                            bfNew.WriteByte(bf.ReadByte());
                        }
                    }
                }
                itm.TileData = Compression.CompressPacket(bfNew.ToArray());

                itm.Up = GetGuid(GameObjectType.Map, int.Parse(data["Up"].ToString()));
                itm.Down = GetGuid(GameObjectType.Map, int.Parse(data["Down"].ToString()));
                itm.Left = GetGuid(GameObjectType.Map, int.Parse(data["Left"].ToString()));
                itm.Right = GetGuid(GameObjectType.Map, int.Parse(data["Right"].ToString()));
                itm.Revision = int.Parse(data["Revision"].ToString());

                //Attributes
                var attributes = JArray.Parse(data["Attributes"].ToString());
                for (var x = 0; x < Options.MapWidth; x++)
                {
                    for (var y = 0; y < Options.MapHeight; y++)
                    {
                        if (string.IsNullOrEmpty(attributes[x][y].ToString()))
                        {
                            itm.Attributes[x, y] = null;
                        }
                        else
                        {
                            var value = int.Parse(attributes[x][y]["Value"].ToString());
                            var data1 = int.Parse(attributes[x][y]["Data1"].ToString());
                            var data2 = int.Parse(attributes[x][y]["Data2"].ToString());
                            var data3 = int.Parse(attributes[x][y]["Data3"].ToString());
                            var data4 = attributes[x][y]["Data4"].ToString();
                            Intersect_Convert_Lib.GameObjects.Maps.Attribute att = Intersect_Convert_Lib.GameObjects.Maps.Attribute.CreateAttribute((MapAttributes)value);
                            switch ((MapAttributes) value)
                            {
                                case MapAttributes.Walkable:
                                    att = null;
                                    break;
                                case MapAttributes.Blocked:
                                    att.Type = MapAttributes.Blocked;
                                    break;
                                case MapAttributes.Item:
                                    att.Type = MapAttributes.Item;
                                    att.Item.ItemId = GetGuid(GameObjectType.Item, data1);
                                    att.Item.Quantity = data2;
                                    break;
                                case MapAttributes.ZDimension:
                                    att.Type = MapAttributes.ZDimension;
                                    att.ZDimension.GatewayTo = data1;
                                    att.ZDimension.BlockedLevel = data2;
                                    break;
                                case MapAttributes.NpcAvoid:
                                    att.Type = MapAttributes.NpcAvoid;
                                    break;
                                case MapAttributes.Warp:
                                    att.Type = MapAttributes.Warp;
                                    att.Warp.MapId = GetGuid(GameObjectType.Map, data1);
                                    att.Warp.X = data2;
                                    att.Warp.Y = data3;
                                    att.Warp.Direction = (WarpDirection)int.Parse(data4);
                                    break;
                                case MapAttributes.Sound:
                                    att.Type = MapAttributes.Sound;
                                    att.Sound.File = data4;
                                    att.Sound.Distance = data1;
                                    break;
                                case MapAttributes.Resource:
                                    att.Type = MapAttributes.Resource;
                                    att.Resource.ResourceId = GetGuid(GameObjectType.Resource, data1);
                                    att.Resource.SpawnLevel = data2;
                                    break;
                                case MapAttributes.Animation:
                                    att.Type = MapAttributes.Animation;
                                    att.Animation.AnimationId = GetGuid(GameObjectType.Animation, data1);
                                    break;
                                case MapAttributes.GrappleStone:
                                    att.Type = MapAttributes.GrappleStone;
                                    break;
                                case MapAttributes.Slide:
                                    att.Type = MapAttributes.Slide;
                                    att.Slide.Direction = (byte) data1;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            itm.Attributes[x, y] = att;
                        }
                    }
                }


                //Lights
                itm.Lights = JsonConvert.DeserializeObject<List<LightBase>>(data["Lights"].ToString());


                //Events
                var events = JObject.Parse(data["Events"].ToString());
                Dictionary<int, Guid> mapEvents = new Dictionary<int, Guid>();
                mapEvents.Add(-1,Guid.Empty);
                foreach (var evt in events)
                {
                    var id = Guid.NewGuid();
                    mapEvents.Add(int.Parse(evt.Key),id);
                    itm.EventIds.Add(id);
                }

                foreach (var evt in events)
                {
                    var evtData = JObject.Parse(evt.Value.ToString());
                    var x = int.Parse(evtData["SpawnX"].ToString());
                    var y = int.Parse(evtData["SpawnY"].ToString());
                    ImportEvent(mapEvents[int.Parse(evt.Key)], evt.Value.ToString(), itm.Id, x, y, mapEvents);
                }

                //Spawns
                var spawns = JArray.Parse(data["Spawns"].ToString());
                foreach (var spawn in spawns)
                {
                    var spn = new NpcSpawn();
                    spn.NpcId = GetGuid(GameObjectType.Npc, int.Parse(spawn["NpcNum"].ToString()));
                    spn.X = int.Parse(spawn["X"].ToString());
                    spn.Y = int.Parse(spawn["Y"].ToString());
                    spn.Direction = (NpcSpawnDirection)(int.Parse(spawn["Dir"].ToString()) + 1);
                    itm.Spawns.Add(spn);
                }

                itm.Music = data["Music"].ToString();
                itm.Sound = data["Sound"].ToString();
                itm.IsIndoors = bool.Parse(data["IsIndoors"].ToString());
                itm.Panorama = data["Panorama"].ToString();
                itm.Fog = data["Fog"].ToString();
                itm.FogXSpeed = int.Parse(data["FogXSpeed"].ToString());
                itm.FogYSpeed = int.Parse(data["FogYSpeed"].ToString());
                itm.FogTransparency = int.Parse(data["FogTransparency"].ToString());
                itm.RHue = int.Parse(data["RHue"].ToString());
                itm.GHue = int.Parse(data["GHue"].ToString());
                itm.BHue = int.Parse(data["BHue"].ToString());
                itm.AHue = int.Parse(data["AHue"].ToString());
                itm.Brightness = int.Parse(data["Brightness"].ToString());
                itm.ZoneType = (MapZones)int.Parse(data["ZoneType"].ToString());
                itm.PlayerLightSize = int.Parse(data["PlayerLightSize"].ToString());
                itm.PlayerLightIntensity = byte.Parse(data["PlayerLightIntensity"].ToString());
                itm.PlayerLightExpand = float.Parse(data["PlayerLightExpand"].ToString());
                itm.PlayerLightColor.A = byte.Parse(data["PlayerLightColor"]["A"].ToString());
                itm.PlayerLightColor.R = byte.Parse(data["PlayerLightColor"]["R"].ToString());
                itm.PlayerLightColor.G = byte.Parse(data["PlayerLightColor"]["G"].ToString());
                itm.PlayerLightColor.B = byte.Parse(data["PlayerLightColor"]["B"].ToString());
                itm.OverlayGraphic = data["OverlayGraphic"].ToString();



                sGameDb.Maps.Add(itm);
            }
        }
        private string FixEventText(string input)
        {
            //Have to accept a numeric parameter after each of the following (player switch/var and server switch/var)
            MatchCollection matches = Regex.Matches(input, Regex.Escape(@"\pv" + " ([0-9]+)"));
            foreach (Match m in matches)
            {
                if (m.Success)
                {
                    int id = Convert.ToInt32(m.Groups[1].Value);
                    input = input.Replace(@"\pv " + id, @"\pv{" + id + "}");
                }
            }
            matches = Regex.Matches(input, Regex.Escape(@"\ps" + " ([0-9]+)"));
            foreach (Match m in matches)
            {
                if (m.Success)
                {
                    int id = Convert.ToInt32(m.Groups[1].Value);
                    input = input.Replace(@"\ps " + id, @"\ps{" + id + "}");
                }
            }
            matches = Regex.Matches(input, Regex.Escape(@"\gv" + " ([0-9]+)"));
            foreach (Match m in matches)
            {
                if (m.Success)
                {
                    int id = Convert.ToInt32(m.Groups[1].Value);
                    input = input.Replace(@"\gv " + id, @"\gv{" + id + "}");
                }
            }
            matches = Regex.Matches(input, Regex.Escape(@"\gs" + " ([0-9]+)"));
            foreach (Match m in matches)
            {
                if (m.Success)
                {
                    int id = Convert.ToInt32(m.Groups[1].Value);
                    input = input.Replace(@"\gs " + id, @"\gs{" + id + "}");
                }
            }
            return input;
        }
        private Dictionary<Guid, List<EventCommand>> ImportCommandLists(string json, Dictionary<int, Guid> mapEvents)
        {
            var importLists = new List<Guid>();
            var commandLists = new Dictionary<Guid, List<EventCommand>>();
            var listIds = new List<Guid>();
            importLists.Add(Guid.Empty);

            var lists = JArray.Parse(json);
            for (int i = 0; i < lists.Count; i++)
            {
                var id = i == 0 ? Guid.Empty : Guid.NewGuid();
                listIds.Add(id);
            }

            for (int i = 0; i < lists.Count; i++)
            {
                var id = listIds[i];
                var commands = JArray.Parse(lists[i]["Commands"].ToString());
                EventCommand newCmd = null;
                var newCmds = new List<EventCommand>();
                foreach (var cmd in commands)
                {
                    newCmd = null;
                    var ints = JsonConvert.DeserializeObject<int[]>(cmd["Ints"].ToString());
                    var strs = JsonConvert.DeserializeObject<string[]>(cmd["Strs"].ToString());
                    switch ((Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.GameObjects.Events.EventCommandType) int.Parse(cmd["Type"].ToString()))
                    {
                        case EventCommandType.Null:
                            break;
                        case EventCommandType.ShowText:
                            var aCmd = new ShowTextCommand();
                            aCmd.Text = FixEventText(strs[0]);
                            aCmd.Face = strs[1];
                            newCmd = aCmd;
                            break;
                        case EventCommandType.ShowOptions:
                            var bCmd = new ShowOptionsCommand();
                            bCmd.Text = FixEventText(strs[0]);
                            bCmd.Options[0]  = FixEventText(strs[1]);
                            bCmd.Options[1] = FixEventText(strs[2]);
                            bCmd.Options[2] = FixEventText(strs[3]);
                            bCmd.Options[3] = FixEventText(strs[4]);
                            bCmd.Face = strs[5];
                            bCmd.BranchIds[0] = listIds[ints[0]];
                            bCmd.BranchIds[1] = listIds[ints[1]];
                            bCmd.BranchIds[2] = listIds[ints[2]];
                            bCmd.BranchIds[3] = listIds[ints[3]];
                            importLists.Add(bCmd.BranchIds[0]);
                            importLists.Add(bCmd.BranchIds[1]);
                            importLists.Add(bCmd.BranchIds[2]);
                            importLists.Add(bCmd.BranchIds[3]);
                            bCmd.Text = strs[0];
                            newCmd = bCmd;
                            break;
                        case EventCommandType.AddChatboxText:
                            var cCmd = new AddChatboxTextCommand();
                            cCmd.Text = FixEventText(strs[0]);
                            cCmd.Color = strs[1];
                            cCmd.Channel = (ChatboxChannel)ints[0];
                            newCmd = cCmd;
                            break;
                        case EventCommandType.SetSwitch:
                            var dCmd = new SetSwitchCommand();
                            var switchType = (SwitchVariableTypes)ints[0];
                            if (switchType == SwitchVariableTypes.PlayerSwitch)
                            {
                                dCmd.SwitchType = SwitchTypes.PlayerSwitch;
                                dCmd.SwitchId = GetGuid(GameObjectType.PlayerSwitch, ints[1]);
                            }
                            else
                            {
                                dCmd.SwitchType = SwitchTypes.ServerSwitch;
                                dCmd.SwitchId = GetGuid(GameObjectType.ServerSwitch, ints[1]);
                            }
                                
                            dCmd.Value = Convert.ToBoolean(ints[2]);
                            newCmd = dCmd;
                            break;
                        case EventCommandType.SetVariable:
                            var eCmd = new SetVariableCommand();
                            var variableType = (SwitchVariableTypes) ints[0];

                            if (variableType == SwitchVariableTypes.PlayerVariable)
                            {
                                eCmd.VariableType = VariableTypes.PlayerVariable;
                                eCmd.VariableId = GetGuid(GameObjectType.PlayerVariable, ints[1]);
                            }
                            else
                            {
                                eCmd.VariableType = VariableTypes.ServerVariable;
                                eCmd.VariableId = GetGuid(GameObjectType.ServerVariable, ints[1]);
                            }
                            eCmd.ModType = (VariableMods) ints[2];
                            eCmd.Value = ints[3];
                            eCmd.HighValue = ints[4];
                            newCmd = eCmd;
                            break;
                        case EventCommandType.SetSelfSwitch:
                            var fCmd = new SetSelfSwitchCommand();
                            fCmd.SwitchId = ints[0];
                            fCmd.Value = Convert.ToBoolean(ints[1]);
                            newCmd = fCmd;
                            break;
                        case EventCommandType.ConditionalBranch:
                            var gCmd = new ConditionalBranchCommand();
                            gCmd.Condition = ImportCondition(cmd.ToString());
                            gCmd.BranchIds[0] = listIds[ints[4]];
                            gCmd.BranchIds[1] = listIds[ints[5]];
                            importLists.Add(gCmd.BranchIds[0]);
                            importLists.Add(gCmd.BranchIds[1]);
                            newCmd = gCmd;
                            break;
                        case EventCommandType.ExitEventProcess:
                            var hCmd = new ExitEventProcessingCommand();
                            newCmd = hCmd;
                            break;
                        case EventCommandType.Label:
                            var iCmd = new LabelCommand();
                            iCmd.Label = strs[0];
                            newCmd = iCmd;
                            break;
                        case EventCommandType.GoToLabel:
                            var jCmd = new GoToLabelCommand();
                            jCmd.Label = strs[0];
                            newCmd = jCmd;
                            break;
                        case EventCommandType.StartCommonEvent:
                            var kCmd = new StartCommmonEventCommand();
                            kCmd.EventId = GetGuid(GameObjectType.Event, ints[0]);
                            newCmd = kCmd;
                            break;
                        case EventCommandType.RestoreHp:
                            newCmd = new RestoreHpCommand();
                            break;
                        case EventCommandType.RestoreMp:
                            newCmd = new RestoreMpCommand();
                            break;
                        case EventCommandType.LevelUp:
                            newCmd = new LevelUpCommand();
                            break;
                        case EventCommandType.GiveExperience:
                            var lCmd = new GiveExperienceCommand();
                            lCmd.Exp = ints[0];
                            newCmd = lCmd;
                            break;
                        case EventCommandType.ChangeLevel:
                            var mCmd = new ChangeLevelCommand();
                            mCmd.Level = ints[0];
                            newCmd = mCmd;
                            break;
                        case EventCommandType.ChangeSpells:
                            var nCmd = new ChangeSpellsCommand();
                            nCmd.Add = !Convert.ToBoolean(ints[0]);
                            nCmd.SpellId = GetGuid(GameObjectType.Spell, ints[1]);
                            nCmd.BranchIds[0] = listIds[ints[4]];
                            nCmd.BranchIds[1] = listIds[ints[5]];
                            importLists.Add(nCmd.BranchIds[0]);
                            importLists.Add(nCmd.BranchIds[1]);
                            newCmd = nCmd;
                            break;
                        case EventCommandType.ChangeItems:
                            var oCmd = new ChangeItemsCommand();
                            oCmd.Add = !Convert.ToBoolean(ints[0]);
                            oCmd.ItemId = GetGuid(GameObjectType.Item, ints[1]);
                            oCmd.Quantity = ints[2];
                            oCmd.BranchIds[0] = listIds[ints[4]];
                            oCmd.BranchIds[1] = listIds[ints[5]];
                            importLists.Add(oCmd.BranchIds[0]);
                            importLists.Add(oCmd.BranchIds[1]);
                            newCmd = oCmd;
                            break;
                        case EventCommandType.ChangeSprite:
                            var pCmd = new ChangeSpriteCommand();
                            pCmd.Sprite = strs[0];
                            newCmd = pCmd;
                            break;
                        case EventCommandType.ChangeFace:
                            var qCmd = new ChangeFaceCommand();
                            qCmd.Face = strs[0];
                            newCmd = qCmd;
                            break;
                        case EventCommandType.ChangeGender:
                            var rCmd = new ChangeGenderCommand();
                            rCmd.Gender = (Gender)ints[0];
                            newCmd = rCmd;
                            break;
                        case EventCommandType.SetAccess:
                            var sCmd = new SetAccessCommand();
                            sCmd.Access = (Access) ints[0];
                            newCmd = sCmd;
                            break;
                        case EventCommandType.WarpPlayer:
                            var tCmd = new WarpCommand();
                            tCmd.MapId = GetGuid(GameObjectType.Map, ints[0]);
                            tCmd.X = ints[1];
                            tCmd.Y = ints[2];
                            tCmd.Direction = (WarpDirection)ints[3];
                            newCmd = tCmd;
                            break;
                        case EventCommandType.SetMoveRoute:
                            var uCmd = new SetMoveRouteCommand();
                            uCmd.Route = ImportMoveRoute(cmd["Route"].ToString(),mapEvents);
                            newCmd = uCmd;
                            break;
                        case EventCommandType.WaitForRouteCompletion:
                            var vCmd = new WaitForRouteCommand();
                            vCmd.TargetId = Guid.Empty;
                            if (mapEvents != null && mapEvents.ContainsKey(ints[0]))
                                vCmd.TargetId = mapEvents[ints[0]];
                            newCmd = vCmd;
                            break;
                        case EventCommandType.HoldPlayer:
                            newCmd = new HoldPlayerCommand();
                            break;
                        case EventCommandType.ReleasePlayer:
                            newCmd = new ReleasePlayerCommand();
                            break;
                        case EventCommandType.SpawnNpc:
                            var wCmd = new SpawnNpcCommand();
                            wCmd.NpcId = GetGuid(GameObjectType.Npc, ints[0]);
                            if (ints[1] == 0) //Tile Spawn
                            {
                                wCmd.MapId = GetGuid(GameObjectType.Map, ints[2]);
                                wCmd.X = ints[3];
                                wCmd.Y = ints[4];
                                wCmd.Dir = (byte)ints[5];
                            }
                            else //Npc Spawn
                            {
                                wCmd.EntityId = Guid.Empty;
                                if (mapEvents != null && mapEvents.ContainsKey(ints[2]))
                                    wCmd.EntityId = mapEvents[ints[2]];
                                wCmd.X = ints[3];
                                wCmd.Y = ints[4];
                                wCmd.Dir = (byte) ints[5];
                            }
                            newCmd = wCmd;
                            break;
                        case EventCommandType.PlayAnimation:
                            var xCmd = new PlayAnimationCommand();
                            xCmd.AnimationId = GetGuid(GameObjectType.Animation, ints[0]);
                            if (ints[1] == 0) //Tile Spawn
                            {
                                xCmd.MapId = GetGuid(GameObjectType.Map, ints[2]);
                                xCmd.X = ints[3];
                                xCmd.Y = ints[4];
                                xCmd.Dir = (byte)ints[5];
                            }
                            else //Npc Spawn
                            {
                                xCmd.EntityId = Guid.Empty;
                                if (mapEvents != null && mapEvents.ContainsKey(ints[2]))
                                    xCmd.EntityId = mapEvents[ints[2]];
                                xCmd.X = ints[3];
                                xCmd.Y = ints[4];
                                xCmd.Dir = (byte)ints[5];
                            }
                            newCmd = xCmd;
                            break;
                        case EventCommandType.PlayBgm:
                            var yCmd = new PlayBgmCommand();
                            yCmd.File = strs[0];
                            newCmd = yCmd;
                            break;
                        case EventCommandType.FadeoutBgm:
                            newCmd = new FadeoutBgmCommand();
                            break;
                        case EventCommandType.PlaySound:
                            var zCmd = new PlaySoundCommand();
                            zCmd.File = strs[0];
                            newCmd = zCmd;
                            break;
                        case EventCommandType.StopSounds:
                            newCmd = new StopSoundsCommand();
                            break;
                        case EventCommandType.Wait:
                            var aaCmd = new WaitCommand();
                            aaCmd.Time = ints[0];
                            newCmd = aaCmd;
                            break;
                        case EventCommandType.OpenBank:
                            newCmd = new OpenBankCommand();
                            break;
                        case EventCommandType.OpenShop:
                            var bbCmd = new OpenShopCommand();
                            bbCmd.ShopId = GetGuid(GameObjectType.Shop, ints[0]);
                            newCmd = bbCmd;
                            break;
                        case EventCommandType.OpenCraftingBench:
                            var ccCmd = new OpenCraftingTableCommand();
                            ccCmd.CraftingTableId = GetGuid(GameObjectType.CraftTables, ints[0]);
                            newCmd = ccCmd;
                            break;
                        case EventCommandType.SetClass:
                            var ddCmd = new SetClassCommand();
                            ddCmd.ClassId = GetGuid(GameObjectType.Class, ints[0]);
                            newCmd = ddCmd;
                            break;
                        case EventCommandType.DespawnNpc:
                            newCmd = new DespawnNpcCommand();
                            break;
                        case EventCommandType.StartQuest:
                            var eeCmd = new StartQuestCommand();
                            eeCmd.QuestId = GetGuid(GameObjectType.Quest, ints[0]);
                            eeCmd.Offer = Convert.ToBoolean(ints[1]);
                            eeCmd.BranchIds[0] = listIds[ints[4]];
                            eeCmd.BranchIds[1] = listIds[ints[5]];
                            importLists.Add(eeCmd.BranchIds[0]);
                            importLists.Add(eeCmd.BranchIds[1]);
                            newCmd = eeCmd;
                            break;
                        case EventCommandType.CompleteQuestTask:
                            var ffCmd = new CompleteQuestTaskCommand();
                            ffCmd.QuestId = GetGuid(GameObjectType.Quest, ints[0]);
                            ffCmd.TaskId = GetQuestTaskId(ffCmd.QuestId, ints[1]);
                            newCmd = ffCmd;
                            break;
                        case EventCommandType.EndQuest:
                            var ggCmd = new EndQuestCommand();
                            ggCmd.QuestId = GetGuid(GameObjectType.Quest, ints[0]);
                            ggCmd.SkipCompletionEvent = Convert.ToBoolean(ints[1]);
                            newCmd = ggCmd;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (newCmd != null)
                    {
                        newCmds.Add(newCmd);
                    }
                }
                commandLists.Add(id,newCmds);
            }

            foreach (var list in commandLists.Keys.ToArray())
            {
                if (!importLists.Contains(list))
                    commandLists.Remove(list);
            }

            return commandLists;
        }
        private EventMoveRoute ImportMoveRoute(string json, Dictionary<int, Guid> mapEvents)
        {
            var route = new EventMoveRoute();
            var data = JObject.Parse(json);

            var actions = JArray.Parse(data["Actions"].ToString());
            foreach (var action in actions)
            {
                var act = new MoveRouteAction();
                var type = (MoveRouteEnum) int.Parse(action["Type"].ToString());
                EventGraphic graphic = null;
                if (!string.IsNullOrEmpty(action["Graphic"].ToString()))
                {
                    graphic = new EventGraphic();
                    graphic.Filename = action["Graphic"]["Filename"].ToString();
                    graphic.Height = int.Parse(action["Graphic"]["Height"].ToString());
                    graphic.Type = (EventGraphicType)int.Parse(action["Graphic"]["Type"].ToString());
                    graphic.Width = int.Parse(action["Graphic"]["Width"].ToString());
                    graphic.X = int.Parse(action["Graphic"]["X"].ToString());
                    graphic.Y = int.Parse(action["Graphic"]["Y"].ToString());
                }
                var animationId = GetGuid(GameObjectType.Animation, int.Parse(action["AnimationIndex"].ToString()));
                act.Type = type;
                if (type == MoveRouteEnum.SetGraphic)
                {
                    act.Graphic = graphic;
                }
                else if (type == MoveRouteEnum.SetAnimation)
                {
                    act.AnimationId = animationId;
                }
                route.Actions.Add(act);
            }

            route.IgnoreIfBlocked = bool.Parse(data["IgnoreIfBlocked"].ToString());
            route.RepeatRoute = bool.Parse(data["RepeatRoute"].ToString());

            var target = int.Parse(data["Target"].ToString());
            route.Target = Guid.Empty;

            if (mapEvents != null && mapEvents.ContainsKey(target))
                route.Target = mapEvents[target];

            return route;
        }
        private Condition ImportCondition(string json)
        {
            var data = JObject.Parse(json);
            Condition cnd = null;
            var ints = JsonConvert.DeserializeObject<int[]>(data["Ints"].ToString());
            var strs = JsonConvert.DeserializeObject<string[]>(data["Strs"].ToString());

            switch (ints[0])
            {
                case 0:
                    var aCnd = new PlayerSwitchCondition();
                    aCnd.SwitchId = GetGuid(GameObjectType.PlayerSwitch, ints[1]);
                    aCnd.Value = Convert.ToBoolean(ints[2]);
                    cnd = aCnd;
                    break;
                case 1:
                    var bCnd = new PlayerVariableCondition();
                    bCnd.VariableId = GetGuid(GameObjectType.PlayerVariable, ints[1]);
                    bCnd.Comparator = (VariableComparators) ints[2];
                    bCnd.Value = ints[3];
                    cnd = bCnd;
                    break;
                case 2:
                    var cCnd = new ServerSwitchCondition();
                    cCnd.SwitchId = GetGuid(GameObjectType.ServerSwitch, ints[1]);
                    cCnd.Value = Convert.ToBoolean(ints[2]);
                    cnd = cCnd;
                    break;
                case 3:
                    var dCnd = new ServerVariableCondition();
                    dCnd.VariableId = GetGuid(GameObjectType.ServerVariable, ints[1]);
                    dCnd.Comparator = (VariableComparators)ints[2];
                    dCnd.Value = ints[3];
                    cnd = dCnd;
                    break;
                case 4:
                    var eCnd = new HasItemCondition();
                    eCnd.ItemId = GetGuid(GameObjectType.Item, ints[1]);
                    eCnd.Quantity = ints[2];
                    cnd = eCnd;
                    break;
                case 5:
                    var fCnd = new ClassIsCondition();
                    fCnd.ClassId = GetGuid(GameObjectType.Class, ints[1]);
                    cnd = fCnd;
                    break;
                case 6:
                    var gCnd = new KnowsSpellCondition();
                    gCnd.SpellId = GetGuid(GameObjectType.Spell, ints[1]);
                    cnd = gCnd;
                    break;
                case 7:
                    var hCnd = new LevelOrStatCondition();
                    if (ints[3] == 0)
                    {
                        hCnd.ComparingLevel = true;
                    }
                    else
                    {
                        hCnd.Stat = (Stats) (ints[3] - 1);
                    }
                    hCnd.Comparator = (VariableComparators)ints[1];
                    hCnd.Value = ints[2];
                    cnd = hCnd;
                    break;
                case 8:
                    var iCnd = new SelfSwitchCondition();
                    iCnd.SwitchIndex = ints[1];
                    iCnd.Value = Convert.ToBoolean(ints[2]);
                    cnd = iCnd;
                    break;
                case 9:
                    var jCnd = new PowerIsCondition();
                    jCnd.Power = (byte) ints[1];
                    cnd = jCnd;
                    break;
                case 10:
                    var kCnd = new TimeBetweenCondition();
                    kCnd.Ranges[0] = ints[1];
                    kCnd.Ranges[1] = ints[2];
                    cnd = kCnd;
                    break;
                case 11:
                    var lCnd = new CanStartQuestCondition();
                    lCnd.QuestId = GetGuid(GameObjectType.Quest, ints[1]);
                    cnd = lCnd;
                    break;
                case 12:
                    var mCnd = new QuestInProgressCondition();
                    mCnd.QuestId = GetGuid(GameObjectType.Quest, ints[1]);
                    mCnd.Progress = (QuestProgress) ints[2];
                    mCnd.TaskId = GetQuestTaskId(mCnd.QuestId, ints[3]);
                    cnd = mCnd;
                    break;
                case 13:
                    var nCnd = new QuestCompletedCondition();
                    nCnd.QuestId = GetGuid(GameObjectType.Quest, ints[1]);
                    cnd = nCnd;
                    break;
                case 14: //We're wiping out the on player death condition since it never worked.. but we don't want to return null and break stuff.. so replacing with dummy stuff
                    var oCnd = new PlayerSwitchCondition();
                    oCnd.SwitchId = Guid.Empty;
                    oCnd.Value = true;
                    cnd = oCnd;
                    break;
                case 15:
                    cnd = new NoNpcsOnMapCondition();
                    break;
                case 16:
                    var pCnd = new GenderIsCondition();
                    pCnd.Gender = (Gender) ints[1];
                    cnd = pCnd;
                    break;
            }

            return cnd;
        }
        private ConditionLists ImportConditionLists(string json)
        {
            var list = new ConditionLists();
            var data = JObject.Parse(json);

            var lists = JArray.Parse(data["Lists"].ToString());
            foreach (var lst in lists)
            {
                var cndList = new ConditionList();
                cndList.Name = lst["Name"].ToString();
                var cnds = JArray.Parse(lst["Conditions"].ToString());
                foreach (var cnd in cnds)
                {
                    var importCnd = ImportCondition(cnd.ToString());
                    if (importCnd != null)
                        cndList.Conditions.Add(importCnd);
                }
                list.Lists.Add(cndList);
            }
            
            return list;
        }
        private void ImportEventPage(EventBase evt, string json, Dictionary<int, Guid> mapEvents)
        {
            var data = JObject.Parse(json);
            var page = new EventPage();
            page.AnimationId = GetGuid(GameObjectType.Animation, int.Parse(data["Animation"].ToString()));
            page.CommandLists = ImportCommandLists(data["CommandLists"].ToString(), mapEvents);
            page.ConditionLists = ImportConditionLists(data["ConditionLists"].ToString());
            page.Description = data["Desc"].ToString();
            page.DirectionFix = Convert.ToBoolean(int.Parse(data["DirectionFix"].ToString()));
            page.DisablePreview = Convert.ToBoolean(int.Parse(data["DisablePreview"].ToString()));
            page.FaceGraphic = data["FaceGraphic"].ToString();
            page.Graphic.Filename = data["Graphic"]["Filename"].ToString();
            page.Graphic.Height = int.Parse(data["Graphic"]["Height"].ToString());
            page.Graphic.Type = (EventGraphicType)int.Parse(data["Graphic"]["Type"].ToString());
            page.Graphic.Width = int.Parse(data["Graphic"]["Width"].ToString());
            page.Graphic.X = int.Parse(data["Graphic"]["X"].ToString());
            page.Graphic.Y = int.Parse(data["Graphic"]["Y"].ToString());
            page.HideName = Convert.ToBoolean(int.Parse(data["HideName"].ToString()));
            page.InteractionFreeze = Convert.ToBoolean(int.Parse(data["InteractionFreeze"].ToString()));
            page.Layer = (EventRenderLayer)int.Parse(data["Layer"].ToString());
            page.Movement.Frequency = (EventMovementFrequency)int.Parse(data["MovementFreq"].ToString());
            page.Movement.Speed = (EventMovementSpeed)int.Parse(data["MovementSpeed"].ToString());
            page.Movement.Type = (EventMovementType)int.Parse(data["MovementType"].ToString());

            page.Movement.Route = ImportMoveRoute(data["MoveRoute"].ToString(), mapEvents);
            
            page.Passable = Convert.ToBoolean(int.Parse(data["Passable"].ToString()));
            if (evt.CommonEvent)
            {
                page.CommonTrigger = (CommonEventTrigger)int.Parse(data["Trigger"].ToString());
            }
            else
            {
                page.Trigger = (EventTrigger)int.Parse(data["Trigger"].ToString());
            }
            page.TriggerCommand = data["TriggerCommand"].ToString();
            page.TriggerVal = Guid.Empty;
            page.WalkingAnimation = Convert.ToBoolean(int.Parse(data["WalkingAnimation"].ToString()));

            evt.Pages.Add(page);
        }
        private void ImportEvent(EventBase evt, string json, Dictionary<int, Guid> mapEvents)
        {
            var data = JObject.Parse(json);
            evt.Name = data["Name"].ToString();
            evt.TimeCreated = TimeCreated;
            evt.Global = Convert.ToBoolean(int.Parse(data["IsGlobal"].ToString()));

            var pages = JArray.Parse(data["MyPages"].ToString());

            foreach (var page in pages)
            {
                ImportEventPage(evt, page.ToString(), mapEvents);
            }

            sGameDb.Events.Add(evt);
        }
        private void ImportEvent(Guid id, string json, Guid mapId, int mapX, int mapY, Dictionary<int,Guid> mapEvents)
        {
            var itm = new EventBase(id,mapId,mapX,mapY);
            itm.Pages.Clear();
            ImportEvent(itm,json,mapEvents);
        }
        private void ImportEvent(Guid id, string json)
        {
            var itm = new EventBase(id, true);
            ImportEvent(itm,json,null);
        }
        private void ImportEvents(Dictionary<int, string> jsns)
        {
            foreach (var jsn in jsns)
            {
                if (GetGuid(GameObjectType.Event,jsn.Key) != Guid.Empty)
                    ImportEvent(GetGuid(GameObjectType.Event,jsn.Key),jsn.Value);
            }
        }
        private void ImportNpcs(Dictionary<int, string> jsns)
        {
            foreach (var jsn in jsns)
            {
                var data = JObject.Parse(jsn.Value);
                var itm = new NpcBase(Guid.Parse(data["Guid"].ToString()));
                itm.TimeCreated = TimeCreated;

                itm.Name = data["Name"].ToString();

                //AggroList
                var aggro = JsonConvert.DeserializeObject<int[]>(data["AggroList"].ToString());
                foreach (var agg in aggro)
                {
                    if (GetGuid(GameObjectType.Npc, agg) != Guid.Empty)
                    {
                        itm.AggroList.Add(GetGuid(GameObjectType.Npc, agg));
                    }
                }

                itm.AttackAllies = bool.Parse(data["AttackAllies"].ToString());
                itm.AttackAnimationId = GetGuid(GameObjectType.Animation, int.Parse(data["AttackAnimation"].ToString()));

                //Behavior handled in switch statement below

                itm.CritChance = int.Parse(data["CritChance"].ToString());
                itm.Damage = int.Parse(data["Damage"].ToString());
                itm.DamageType = int.Parse(data["DamageType"].ToString());

                var drops = JArray.Parse(data["Drops"].ToString());
                foreach (var drp in drops)
                {
                    var itemid = GetGuid(GameObjectType.Item, int.Parse(drp["ItemNum"].ToString()));
                    if (itemid != Guid.Empty)
                    {
                        var drop = new NpcDrop();
                        drop.ItemId = itemid;
                        drop.Chance = int.Parse(drp["Chance"].ToString());
                        drop.Quantity = int.Parse(drp["Amount"].ToString());
                        itm.Drops.Add(drop);
                    }
                }

                itm.Experience = int.Parse(data["Experience"].ToString());
                itm.Level = int.Parse(data["Level"].ToString());

                itm.MaxVital = JsonConvert.DeserializeObject<int[]>(data["MaxVital"].ToString());

                itm.NpcVsNpcEnabled = bool.Parse(data["NpcVsNpcEnabled"].ToString());

                itm.Scaling = int.Parse(data["Scaling"].ToString());
                itm.ScalingStat = int.Parse(data["ScalingStat"].ToString());
                itm.SightRange = int.Parse(data["SightRange"].ToString());
                itm.SpawnDuration = int.Parse(data["SpawnDuration"].ToString());
                itm.SpellFrequency = int.Parse(data["SpellFrequency"].ToString());

                var spells = JsonConvert.DeserializeObject<int[]>(data["Spells"].ToString());
                foreach (var spl in spells)
                {
                    var spellId = GetGuid(GameObjectType.Spell,spl);
                    if (spellId != Guid.Empty)
                    {
                        itm.Spells.Add(spellId);
                    }
                }

                itm.Sprite = data["Sprite"].ToString();

                itm.Stats = JsonConvert.DeserializeObject<int[]>(data["Stat"].ToString());

                var behavior = (NpcBehavior) int.Parse(data["Behavior"].ToString());
                ConditionList condList;
                LevelOrStatCondition cond;
                switch (behavior)
                {
                    case NpcBehavior.AttackWhenAttacked:
                        break;
                    case NpcBehavior.AttackOnSight:
                        itm.Aggressive = true;
                        break;
                    case NpcBehavior.Friendly:
                        condList = new ConditionList();
                        condList.Name = "Friendly";
                        cond = new LevelOrStatCondition();
                        cond.ComparingLevel = true;
                        cond.Comparator = VariableComparators.Less;
                        cond.Value = 0;
                        condList.Conditions.Add(cond);
                        itm.PlayerCanAttackConditions.Lists.Add(condList);

                        //Player can't attack because level always >= 0.
                        //Npc is not aggressive so it won't attack player on sight.
                        break;
                    case NpcBehavior.Guard:
                        condList = new ConditionList();
                        condList.Name = "Guard";
                        cond = new LevelOrStatCondition();
                        cond.ComparingLevel = true;
                        cond.Comparator = VariableComparators.GreaterOrEqual;
                        cond.Value = 0;
                        condList.Conditions.Add(cond);
                        itm.PlayerFriendConditions.Lists.Add(condList);

                        //Player cannot attack npc if the npc is a protector.. and the npc will
                        //act like a guard because it will be a protector as long as level >= 0 which is always.
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }


                sGameDb.Npcs.Add(itm);
            }
        }
        private void ImportSpells(Dictionary<int, string> jsns)
        {
            foreach (var jsn in jsns)
            {
                var data = JObject.Parse(jsn.Value);
                var itm = new SpellBase(Guid.Parse(data["Guid"].ToString()));
                itm.TimeCreated = TimeCreated;

                itm.Name = data["Name"].ToString();
                itm.CastAnimationId = GetGuid(GameObjectType.Animation, int.Parse(data["CastAnimation"].ToString()));
                itm.CastDuration = int.Parse(data["CastDuration"].ToString());

                itm.CastingRequirements = ImportConditionLists(data["CastingReqs"].ToString());

                itm.Combat.CastRange = int.Parse(data["CastRange"].ToString());
                itm.CooldownDuration = int.Parse(data["CooldownDuration"].ToString());
                //Cost Variable wasn't used and shouldn't have been a member of SpellBase
                itm.Combat.CritChance = int.Parse(data["CritChance"].ToString());
                itm.Combat.DamageType = int.Parse(data["DamageType"].ToString());
                //DataX handled in switch statement below
                itm.Description = data["Desc"].ToString();
                itm.Combat.Friendly = Convert.ToBoolean(int.Parse(data["Friendly"].ToString()));
                itm.HitAnimationId = GetGuid(GameObjectType.Animation, int.Parse(data["HitAnimation"].ToString()));
                itm.Combat.HitRadius = int.Parse(data["HitRadius"].ToString());
                itm.Icon = data["Pic"].ToString();
                itm.Combat.ProjectileId = GetGuid(GameObjectType.Projectile, int.Parse(data["Projectile"].ToString()));
                itm.Combat.Scaling = int.Parse(data["Scaling"].ToString());
                itm.Combat.ScalingStat = int.Parse(data["ScalingStat"].ToString());

                itm.SpellType = (SpellTypes)byte.Parse(data["SpellType"].ToString());

                itm.Combat.StatDiff = JsonConvert.DeserializeObject<int[]>(data["StatDiff"].ToString());
                itm.Combat.TargetType = (SpellTargetTypes)int.Parse(data["TargetType"].ToString());
                itm.VitalCost = JsonConvert.DeserializeObject<int[]>(data["VitalCost"].ToString());
                itm.Combat.VitalDiff = JsonConvert.DeserializeObject<int[]>(data["VitalDiff"].ToString());

                switch ((SpellTypes) int.Parse(data["SpellType"].ToString()))
                {
                    case SpellTypes.CombatSpell:
                        itm.Combat.HoTDoT = Convert.ToBoolean(int.Parse(data["Data1"].ToString()));
                        itm.Combat.Duration = int.Parse(data["Data2"].ToString());
                        itm.Combat.Effect = (StatusTypes) int.Parse(data["Data3"].ToString());
                        itm.Combat.HotDotInterval = int.Parse(data["Data4"].ToString());
                        itm.Combat.TransformSprite = data["Data5"].ToString();
                        break;
                    case SpellTypes.Warp:
                        itm.Warp.MapId = GetGuid(GameObjectType.Map, int.Parse(data["Data1"].ToString()));
                        itm.Warp.X = int.Parse(data["Data2"].ToString());
                        itm.Warp.Y = int.Parse(data["Data3"].ToString());
                        itm.Warp.Dir = byte.Parse(data["Data4"].ToString());
                        break;
                    case SpellTypes.WarpTo:
                        break;
                    case SpellTypes.Dash:
                        itm.Dash.IgnoreMapBlocks = Convert.ToBoolean(int.Parse(data["Data1"].ToString()));
                        itm.Dash.IgnoreActiveResources = Convert.ToBoolean(int.Parse(data["Data2"].ToString()));
                        itm.Dash.IgnoreInactiveResources = Convert.ToBoolean(int.Parse(data["Data3"].ToString()));
                        itm.Dash.IgnoreZDimensionAttributes = Convert.ToBoolean(int.Parse(data["Data4"].ToString()));
                        break;
                    case SpellTypes.Event:
                        itm.EventId = GetGuid(GameObjectType.Event, int.Parse(data["Data1"].ToString()));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                sGameDb.Spells.Add(itm);
            }
        }
        private void ImportItems(Dictionary<int, string> jsns)
        {
            foreach (var jsn in jsns)
            {
                var data = JObject.Parse(jsn.Value);
                var itm = new ItemBase(Guid.Parse(data["Guid"].ToString()));
                itm.TimeCreated = TimeCreated;

                itm.Name = data["Name"].ToString();
                itm.AnimationId = GetGuid(GameObjectType.Animation, int.Parse(data["Animation"].ToString()));
                itm.AttackAnimationId = GetGuid(GameObjectType.Animation, int.Parse(data["AttackAnimation"].ToString()));
                itm.Bound = Convert.ToBoolean(int.Parse(data["Bound"].ToString()));
                itm.CritChance = int.Parse(data["CritChance"].ToString());
                itm.Damage = int.Parse(data["Damage"].ToString());
                itm.DamageType = int.Parse(data["DamageType"].ToString());

                //DataX values handled in switch statement below

                itm.Description = data["Desc"].ToString();
                itm.FemalePaperdoll = data["FemalePaperdoll"].ToString();

                itm.ItemType = (ItemTypes)int.Parse(data["ItemType"].ToString());

                itm.MalePaperdoll = data["MalePaperdoll"].ToString();
                itm.Icon = data["Pic"].ToString();
                itm.Price = int.Parse(data["Price"].ToString());
                itm.ProjectileId = GetGuid(GameObjectType.Projectile, int.Parse(data["Projectile"].ToString()));
                itm.Scaling = int.Parse(data["Scaling"].ToString());
                itm.ScalingStat = int.Parse(data["ScalingStat"].ToString());
                itm.Speed = int.Parse(data["Speed"].ToString());
                itm.Stackable = Convert.ToBoolean(int.Parse(data["Stackable"].ToString()));
                itm.StatGrowth = int.Parse(data["StatGrowth"].ToString());
                itm.StatsGiven = JsonConvert.DeserializeObject<int[]>(data["StatsGiven"].ToString());
                itm.Tool = int.Parse(data["Tool"].ToString());

                switch ((ItemTypes)int.Parse(data["ItemType"].ToString()))
                {
                    case ItemTypes.None:
                        break;
                    case ItemTypes.Equipment:
                        itm.EquipmentSlot = int.Parse(data["Data1"].ToString());
                        itm.Effect.Type = (EffectType)int.Parse(data["Data2"].ToString());
                        itm.Effect.Percentage = int.Parse(data["Data3"].ToString());
                        itm.TwoHanded = Convert.ToBoolean(int.Parse(data["Data4"].ToString()));
                        break;
                    case ItemTypes.Consumable:
                        itm.Consumable.Type = (ConsumableType) int.Parse(data["Data1"].ToString());
                        itm.Consumable.Value = int.Parse(data["Data2"].ToString());
                        break;
                    case ItemTypes.Currency:
                        break;
                    case ItemTypes.Spell:
                        itm.SpellId = GetGuid(GameObjectType.Spell, int.Parse(data["Data1"].ToString()));
                        break;
                    case ItemTypes.Event:
                        itm.EventId = GetGuid(GameObjectType.Event, int.Parse(data["Data1"].ToString()));
                        break;
                    case ItemTypes.Bag:
                        itm.SlotCount = int.Parse(data["Data1"].ToString());
                        break;
                }
                itm.UsageRequirements = ImportConditionLists(data["UseReqs"].ToString());

                sGameDb.Items.Add(itm);
            }
        }
        private void ImportClasses(Dictionary<int, string> jsns)
        {
            foreach (var jsn in jsns)
            {
                var data = JObject.Parse(jsn.Value);
                var itm = new ClassBase(Guid.Parse(data["Guid"].ToString()));
                itm.TimeCreated = TimeCreated;

                itm.Name = data["Name"].ToString();

                itm.AttackAnimationId = GetGuid(GameObjectType.Animation, int.Parse(data["AttackAnimation"].ToString()));
                itm.BaseExp = int.Parse(data["BaseExp"].ToString());
                itm.BasePoints = int.Parse(data["BasePoints"].ToString());
                itm.BaseStat = JsonConvert.DeserializeObject<int[]>(data["BaseStat"].ToString());
                itm.BaseVital = JsonConvert.DeserializeObject<int[]>(data["BaseVital"].ToString());
                itm.CritChance = int.Parse(data["CritChance"].ToString());
                itm.Damage = int.Parse(data["Damage"].ToString());
                itm.DamageType = int.Parse(data["DamageType"].ToString());
                itm.ExpIncrease = int.Parse(data["ExpIncrease"].ToString());
                itm.IncreasePercentage = int.Parse(data["IncreasePercentage"].ToString());

                var items = JArray.Parse(data["Items"].ToString());
                foreach (var im in items)
                {
                    var itemid = GetGuid(GameObjectType.Item, int.Parse(im["ItemNum"].ToString()));
                    if (itemid != Guid.Empty)
                    {
                        var cItem = new ClassItem();
                        cItem.Id = itemid;
                        cItem.Quantity = int.Parse(im["Amount"].ToString());
                        itm.Items.Add(cItem);
                    }
                }

                itm.Locked = Convert.ToBoolean(int.Parse(data["Locked"].ToString()));
                itm.PointIncrease = int.Parse(data["PointIncrease"].ToString());
                itm.Scaling = int.Parse(data["Scaling"].ToString());
                itm.ScalingStat = int.Parse(data["ScalingStat"].ToString());
                itm.SpawnDir = int.Parse(data["SpawnDir"].ToString());
                itm.SpawnMapId = GetGuid(GameObjectType.Map,int.Parse(data["SpawnMap"].ToString()));
                itm.SpawnX = int.Parse(data["SpawnX"].ToString());
                itm.SpawnY = int.Parse(data["SpawnY"].ToString());

                var spells = JArray.Parse(data["Spells"].ToString());
                foreach (var sp in spells)
                {
                    var spellId = GetGuid(GameObjectType.Spell, int.Parse(sp["SpellNum"].ToString()));
                    if (spellId != Guid.Empty)
                    {
                        var cItem = new ClassSpell();
                        cItem.Id = spellId;
                        cItem.Level = int.Parse(sp["Level"].ToString());
                        itm.Spells.Add(cItem);
                    }
                }

                itm.Sprites = JsonConvert.DeserializeObject<List<ClassSprite>>(data["Sprites"].ToString());
                itm.StatIncrease = JsonConvert.DeserializeObject<int[]>(data["StatIncrease"].ToString());
                itm.VitalIncrease = JsonConvert.DeserializeObject<int[]>(data["VitalIncrease"].ToString());
                itm.VitalRegen = JsonConvert.DeserializeObject<int[]>(data["VitalRegen"].ToString());


                sGameDb.Classes.Add(itm);
            }
        }
        private void ImportShops(Dictionary<int, string> jsns)
        {
            foreach (var jsn in jsns)
            {
                var data = JObject.Parse(jsn.Value);
                var itm = new ShopBase(Guid.Parse(data["Guid"].ToString()));
                itm.TimeCreated = TimeCreated;

                itm.Name = data["Name"].ToString();
                
                //Buying Items
                var buyingItems = JArray.Parse(data["BuyingItems"].ToString());
                foreach (var bItem in buyingItems)
                {
                    var sItem = new ShopItem(GetGuid(GameObjectType.Item, int.Parse(bItem["ItemNum"].ToString())), GetGuid(GameObjectType.Item, int.Parse(bItem["CostItemNum"].ToString())), int.Parse(bItem["CostItemVal"].ToString()));
                    itm.BuyingItems.Add(sItem);
                }

                itm.BuyingWhitelist = bool.Parse(data["BuyingWhitelist"].ToString());
                itm.DefaultCurrencyId = GetGuid(GameObjectType.Item, int.Parse(data["DefaultCurrency"].ToString()));

                //Selling Items
                var sellingItems = JArray.Parse(data["SellingItems"].ToString());
                foreach (var bItem in sellingItems)
                {
                    var sItem = new ShopItem(GetGuid(GameObjectType.Item, int.Parse(bItem["ItemNum"].ToString())), GetGuid(GameObjectType.Item, int.Parse(bItem["CostItemNum"].ToString())), int.Parse(bItem["CostItemVal"].ToString()));
                    itm.SellingItems.Add(sItem);
                }

                sGameDb.Shops.Add(itm);
            }
        }
        private void ImportResources(Dictionary<int, string> jsns)
        {
            foreach (var jsn in jsns)
            {
                var data = JObject.Parse(jsn.Value);
                var itm = new ResourceBase(Guid.Parse(data["Guid"].ToString()));
                itm.TimeCreated = TimeCreated;

                itm.Name = data["Name"].ToString();
                itm.AnimationId = GetGuid(GameObjectType.Animation, int.Parse(data["Animation"].ToString()));

                var drops = JArray.Parse(data["Drops"].ToString());
                foreach (var drp in drops)
                {
                    var itemid = GetGuid(GameObjectType.Item, int.Parse(drp["ItemNum"].ToString()));
                    if (itemid != Guid.Empty)
                    {
                        var drop = new ResourceBase.ResourceDrop();
                        drop.ItemId = itemid;
                        drop.Chance = int.Parse(drp["Chance"].ToString());
                        drop.Quantity = int.Parse(drp["Amount"].ToString());
                        itm.Drops.Add(drop);
                    }
                }

                itm.Exhausted.Graphic = data["EndGraphic"].ToString();
                itm.HarvestingRequirements = ImportConditionLists(data["HarvestingReqs"].ToString());
                itm.Initial.Graphic = data["InitialGraphic"].ToString();
                itm.MaxHp = int.Parse(data["MaxHp"].ToString());
                itm.MinHp = int.Parse(data["MinHp"].ToString());
                itm.SpawnDuration = int.Parse(data["SpawnDuration"].ToString()) * 1000;
                itm.Tool = int.Parse(data["Tool"].ToString());
                itm.WalkableAfter = bool.Parse(data["WalkableAfter"].ToString());
                itm.WalkableBefore = bool.Parse(data["WalkableBefore"].ToString());

                sGameDb.Resources.Add(itm);
            }
        }
        private void ImportProjectiles(Dictionary<int, string> jsns)
        {
            foreach (var jsn in jsns)
            {
                var data = JObject.Parse(jsn.Value);
                var itm = new ProjectileBase(Guid.Parse(data["Guid"].ToString()));
                itm.TimeCreated = TimeCreated;

                itm.Name = data["Name"].ToString();

                itm.AmmoItemId = GetGuid(GameObjectType.Item, int.Parse(data["Ammo"].ToString()));
                itm.AmmoRequired = int.Parse(data["AmmoRequired"].ToString());

                var animations = JArray.Parse(data["Animations"].ToString());
                foreach (var anim in animations)
                {
                    var projAnim = new ProjectileAnimation(GetGuid(GameObjectType.Animation, int.Parse(anim["Animation"].ToString())), int.Parse(anim["SpawnRange"].ToString()), bool.Parse(anim["AutoRotate"].ToString()));
                    itm.Animations.Add(projAnim);
                }

                itm.Delay = int.Parse(data["Delay"].ToString());
                itm.GrappleHook = bool.Parse(data["GrappleHook"].ToString());
                //Homing never existed.. and prob never will
                itm.IgnoreActiveResources = bool.Parse(data["IgnoreActiveResources"].ToString());
                itm.IgnoreExhaustedResources = bool.Parse(data["IgnoreExhaustedResources"].ToString());
                itm.IgnoreMapBlocks = bool.Parse(data["IgnoreMapBlocks"].ToString());
                itm.IgnoreZDimension = bool.Parse(data["IgnoreZDimension"].ToString());
                itm.Knockback = int.Parse(data["Knockback"].ToString());
                itm.Quantity = int.Parse(data["Quantity"].ToString());
                itm.Range = int.Parse(data["Range"].ToString());
                itm.SpawnLocations = JsonConvert.DeserializeObject<Location[,]>(data["SpawnLocations"].ToString());
                itm.Speed = int.Parse(data["Speed"].ToString());

                sGameDb.Projectiles.Add(itm);
            }
        }
        private void ImportCraftTables(Dictionary<int, string> jsns)
        {
            foreach (var jsn in jsns)
            {
                var data = JObject.Parse(jsn.Value);
                var itm = new CraftingTableBase(Guid.Parse(data["Guid"].ToString()));
                itm.TimeCreated = TimeCreated;

                itm.Name = data["Name"].ToString();

                var crafts = JsonConvert.DeserializeObject<int[]>(data["Crafts"].ToString());
                foreach (var craft in crafts)
                {
                    itm.Crafts.Add(GetGuid(GameObjectType.Crafts, craft));
                }
                

                sGameDb.CraftingTables.Add(itm);
            }
        }
        private void ImportCrafts(Dictionary<int, string> jsns)
        {
            foreach (var jsn in jsns)
            {
                var data = JObject.Parse(jsn.Value);
                var itm = new CraftBase(Guid.Parse(data["Guid"].ToString()));
                itm.TimeCreated = TimeCreated;

                itm.Name = data["Name"].ToString();

                itm.ItemId = GetGuid(GameObjectType.Item, int.Parse(data["Item"].ToString()));
                itm.Time = int.Parse(data["Time"].ToString());
                var ingredients = JArray.Parse(data["Ingredients"].ToString());
                foreach (var i in ingredients)
                {
                    var ci = new CraftIngredient(GetGuid(GameObjectType.Item, int.Parse(i["Item"].ToString())), int.Parse(i["Quantity"].ToString()));
                    itm.Ingredients.Add(ci);
                }

                sGameDb.Crafts.Add(itm);
            }
        }
        private void ImportTilesets(Dictionary<int, string> jsns)
        {
            foreach (var jsn in jsns)
            {
                var data = JObject.Parse(jsn.Value);
                var itm = new TilesetBase(Guid.Parse(data["Guid"].ToString()));
                itm.Name = data["Name"].ToString();
                itm.TimeCreated = TimeCreated;
                sGameDb.Tilesets.Add(itm);
            }
        }
        private void ImportAnimations(Dictionary<int, string> jsns)
        {
            foreach (var jsn in jsns)
            {
                var data = JObject.Parse(jsn.Value);
                var itm = new AnimationBase(Guid.Parse(data["Guid"].ToString()));
                itm.TimeCreated = TimeCreated;

                itm.Name = data["Name"].ToString();

                //Lower
                itm.Lower.FrameCount = int.Parse(data["LowerAnimFrameCount"].ToString());
                itm.Lower.FrameSpeed = int.Parse(data["LowerAnimFrameSpeed"].ToString());
                itm.Lower.LoopCount = int.Parse(data["LowerAnimLoopCount"].ToString());
                itm.Lower.DisableRotations = bool.Parse(data["DisableLowerRotations"].ToString());
                itm.Lower.Sprite = data["LowerAnimSprite"].ToString();
                itm.Lower.XFrames = int.Parse(data["LowerAnimXFrames"].ToString());
                itm.Lower.YFrames = int.Parse(data["LowerAnimYFrames"].ToString());
                itm.Lower.Lights = JsonConvert.DeserializeObject<LightBase[]>(data["LowerLights"].ToString());

                itm.Sound = data["Sound"].ToString();

                //Upper
                itm.Upper.FrameCount = int.Parse(data["UpperAnimFrameCount"].ToString());
                itm.Upper.FrameSpeed = int.Parse(data["UpperAnimFrameSpeed"].ToString());
                itm.Upper.LoopCount = int.Parse(data["UpperAnimLoopCount"].ToString());
                itm.Upper.DisableRotations = bool.Parse(data["DisableUpperRotations"].ToString());
                itm.Upper.Sprite = data["UpperAnimSprite"].ToString();
                itm.Upper.XFrames = int.Parse(data["UpperAnimXFrames"].ToString());
                itm.Upper.YFrames = int.Parse(data["UpperAnimYFrames"].ToString());
                itm.Upper.Lights = JsonConvert.DeserializeObject<LightBase[]>(data["UpperLights"].ToString());

                sGameDb.Animations.Add(itm);
            }
        }
        private void ImportPlayerSwitches(Dictionary<int, string> jsns)
        {
            foreach (var jsn in jsns)
            {
                var data = JObject.Parse(jsn.Value);
                var itm = new PlayerSwitchBase(Guid.Parse(data["Guid"].ToString()))
                {
                    TimeCreated = TimeCreated,

                    Name = data["Name"].ToString(),

                    TextId = jsn.Key.ToString()
                };

                sGameDb.PlayerSwitches.Add(itm);
            }
        }
        private void ImportPlayerVariables(Dictionary<int, string> jsns)
        {
            foreach (var jsn in jsns)
            {
                var data = JObject.Parse(jsn.Value);
                var itm = new PlayerVariableBase(Guid.Parse(data["Guid"].ToString()))
                {
                    TimeCreated = TimeCreated,

                    Name = data["Name"].ToString(),

                    TextId = jsn.Key.ToString()
                };

                sGameDb.PlayerVariables.Add(itm);
            }
        }
        private void ImportServerSwitches(Dictionary<int, string> jsns)
        {
            foreach (var jsn in jsns)
            {
                var data = JObject.Parse(jsn.Value);
                var itm = new ServerSwitchBase(Guid.Parse(data["Guid"].ToString()))
                {
                    TimeCreated = TimeCreated,

                    Name = data["Name"].ToString(),

                    TextId = jsn.Key.ToString(),
                    Value = bool.Parse(data["Value"].ToString())
                };
                sGameDb.ServerSwitches.Add(itm);
            }
        }
        private void ImportServerVariables(Dictionary<int, string> jsns)
        {
            foreach (var jsn in jsns)
            {
                var data = JObject.Parse(jsn.Value);
                var itm = new ServerVariableBase(Guid.Parse(data["Guid"].ToString()))
                {
                    TimeCreated = TimeCreated,

                    Name = data["Name"].ToString(),

                    TextId = jsn.Key.ToString(),

                    Value = int.Parse(data["Value"].ToString())
                };

                sGameDb.ServerVariables.Add(itm);
            }
        }
        private void ImportTime(string jsn)
        {
            var data = JObject.Parse(jsn);
            var tb = TimeBase.GetTimeBase();

            tb.DaylightHues = JsonConvert.DeserializeObject<Color[]>(data["RangeColors"].ToString());
            tb.RangeInterval = int.Parse(data["RangeInterval"].ToString());
            tb.Rate = float.Parse(data["Rate"].ToString());
            tb.SyncTime = bool.Parse(data["SyncTime"].ToString());


            sGameDb.Add(tb);
        }
        #endregion
        private Guid GetGuid(GameObjectType typ, int index)
        {
            if (objs.ContainsKey(typ))
            {
                if (objs[typ].ContainsKey(index))
                {
                    var jobj = JObject.Parse(objs[typ][index]);
                    var guid = Guid.Parse(jobj["Guid"].ToString());
                    return guid;
                }
            }
            return Guid.Empty;
        }
    }

    public class MySqlDbConnInfo
    {
        public string Server;
        public string User;
        public string Password;
        public int Port;
        public string Database;
    }
}
