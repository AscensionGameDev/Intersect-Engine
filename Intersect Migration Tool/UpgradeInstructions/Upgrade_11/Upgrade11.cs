using System;
using System.Collections.Generic;
using System.IO;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.Enums;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.GameObjects;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.GameObjects.Events;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.GameObjects.Maps;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.GameObjects.Maps.MapList;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.GameObjects.Switches_and_Variables;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.Models;
using Intersect.Migration.UpgradeInstructions.Upgrade_12;
using Mono.Data.Sqlite;
using Log = Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.Logging.Log;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_11
{
    public class User11
    {
        public int id;
        public string user;
        public string pass;
        public string salt;
        public string email;
        public int power;
    }

    public class Character11
    {
        public int id;
        public int userid;
        public string name;
        public int map;
        public int x;
        public int y;
        public int z;
        public int dir;
        public string sprite;
        public string face;
        public int classid;
        public int gender;
        public int level;
        public long exp;
        public int[] vitals;
        public int[] maxvitals;
        public int[] stats;
        public int statpoints;
        public int[] equipment;
        public long last_online;
    }

    public class Ban11
    {
        public int playerid;
        public long bantime;
        public int accountid;
        public long unbantime;
        public string ip;
        public string reason;
        public string banner;
    }

    public class Mute11
    {
        public int playerid;
        public long mutetime;
        public int accountid;
        public long unmutetime;
        public string ip;
        public string reason;
        public string muter;
    }

    public class Friend11
    {
        public int owner_id;
        public int friend_id;
    }

    public class PSwitch11
    {
        public int playerid;
        public int switchid;
        public bool value;
    }

    public class PVar11
    {
        public int playerid;
        public int variableid;
        public int value;
    }

    public class Quest11
    {
        public int playerid;
        public int questid;
        public int taskid;
        public int taskprogress;
        public bool completed;
    }

    public class Spell11
    {
        public int playerid;
        public int slot;
        public int spellid;
        public int spellcd;
    }

    public class Bag11
    {
        public int bagid;
        public int slots;
    }

    public class Inventory11
    {
        public int playerid;
        public int slot;
        public int itemnum;
        public int itemval;
        public int[] itemstats;
        public int item_bag_id;
    }

    public class Bank11
    {
        public int playerid;
        public int slot;
        public int itemnum;
        public int itemval;
        public int[] itemstats;
        public int item_bag_id;
    }

    public class BagItem11
    {
        public int bagid;
        public int slot;
        public int itemnum;
        public int itemval;
        public int[] itemstats;
        public int item_bag_id;
    }

    public class Hotbar11
    {
        public int playerid;
        public int slot;
        public int type;
        public int itemslot;
    }

    public class Upgrade11
    {
        //Database Variables
        private const string INFO_TABLE = "info";

        private const string DB_VERSION = "dbversion";

        //Log Table Constants
        private const string LOG_TABLE = "logs";

        //Ban Table Constants
        private const string BAN_TABLE = "bans";
        //Mute Table Constants
        private const string MUTE_TABLE = "mutes";
        //User Table Constants
        private const string USERS_TABLE = "users";
        //Character Table Constants
        private const string CHAR_TABLE = "characters";
        //Char Inventory Table Constants
        private const string CHAR_INV_TABLE = "char_inventory";
        //Char Spells Table Constants
        private const string CHAR_SPELL_TABLE = "char_spells";
        //Char Hotbar Table Constants
        private const string CHAR_HOTBAR_TABLE = "char_hotbar";
        //Char Bank Table Constants
        private const string CHAR_BANK_TABLE = "char_bank";
        //Char Switches Table Constants
        private const string CHAR_SWITCHES_TABLE = "char_switches";
        //Char Variables Table Constants
        private const string CHAR_VARIABLES_TABLE = "char_variables";
        //Char Quests Table Constants
        private const string CHAR_QUESTS_TABLE = "char_quests";
        //Char Friendss Table Constants
        private const string CHAR_FRIENDS_TABLE = "char_friends";
        //Bag Table Constants
        private const string BAGS_TABLE = "bags";
        //Bag Items Table Constants
        private const string BAG_ITEMS_TABLE = "bag_items";
        
        private const string MAP_ATTRIBUTES_TABLE = "map_attributes";
        private const string MAP_ATTRIBUTES_MAP_ID = "map_id";
        private const string MAP_ATTRIBUTES_DATA = "data";
        
        private const string MAP_LIST_DATA = "data";

        //Map Tiles Table
        private const string MAP_TILES_TABLE = "map_tiles";

        private const string MAP_TILES_MAP_ID = "map_id";
        private const string MAP_TILES_DATA = "data";

        //Map List Table Constants
        private const string MAP_LIST_TABLE = "map_list";
        //Time of Day Table Constants
        private const string TIME_TABLE = "time";
        private const string TIME_DATA = "data";

        //GameObject Table Constants
        private const string GAME_OBJECT_ID = "id";

        private const string GAME_OBJECT_DELETED = "deleted";
        private const string GAME_OBJECT_DATA = "data";

        private SqliteConnection _dbConnection;
        private object _dbLock = new object();

        private int craftsIndex = 1;
        private int craftTablesIndex = 1;

        Dictionary<GameObjectType,Dictionary<int,string>> objs = new Dictionary<GameObjectType, Dictionary<int, string>>();
        Dictionary<int,User11> Users = new Dictionary<int, User11>();
        private Dictionary<int, Character11> Characters = new Dictionary<int, Character11>();
        private List<Ban11> Bans = new List<Ban11>();
        private List<Mute11> Mutes = new List<Mute11>();
        private List<Friend11> Friends = new List<Friend11>();
        private List<PSwitch11> Switches = new List<PSwitch11>();
        private List<PVar11> Variables = new List<PVar11>();
        private List<Quest11> Quests = new List<Quest11>();
        private List<Spell11> Spells = new List<Spell11>();
        private List<Bag11> Bags = new List<Bag11>();
        private List<Inventory11> Items = new List<Inventory11>();
        private List<Bank11> Bank = new List<Bank11>();
        private List<BagItem11> BagItems = new List<BagItem11>();
        private List<Hotbar11> Hotbar = new List<Hotbar11>();

        public Upgrade11(SqliteConnection conn)
        {
            _dbConnection = conn;
        }

        public void Upgrade()
        {
            //Gotta Load and Save All Events
            ServerOptions.LoadOptions();
            LoadAllGameObjects();
            LoadTime();
            LoadMapFolders();

            objs.Remove(GameObjectType.Bench);
            objs[GameObjectType.Time].Add(0, TimeBase.GetTimeJson());

            //Time to load all player data into memory
            LoadUsers();
            LoadCharacters();
            LoadBans();
            LoadMutes();
            LoadFriends();
            LoadPSwitches();
            LoadPVariables();
            LoadQuests();
            LoadSpells();
            LoadBags();
            LoadInventories();
            LoadBanks();
            LoadBagItems();
            LoadHotbar();

            var up12 = new Upgrade12(objs, MapList.GetList(), Users, Characters, Bans, Mutes, Friends, Switches,Variables, Quests, Spells, Bags,Items,Bank,BagItems, Hotbar);
            up12.Upgrade();
        }

        private void LoadHotbar()
        {
            lock (_dbLock)
            {
                var query = "SELECT * from char_hotbar;";
                using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
                {
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var usr = new Hotbar11()
                            {
                                playerid = Convert.ToInt32(dataReader["char_id"]),
                                slot = Convert.ToInt32(dataReader["slot"]),
                                type = Convert.ToInt32(dataReader["type"]),
                                itemslot = Convert.ToInt32(dataReader["itemslot"]),
                            };
                            Hotbar.Add(usr);
                        }
                    }
                }
            }

        }

        private void LoadBagItems()
        {
            var commaSep = new char[1];
            commaSep[0] = ',';
            lock (_dbLock)
            {
                var query = "SELECT * from bag_items;";
                using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
                {
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var usr = new BagItem11()
                            {
                                bagid = Convert.ToInt32(dataReader["bag_id"]),
                                slot = Convert.ToInt32(dataReader["slot"]),
                                itemnum = Convert.ToInt32(dataReader["itemnum"]),
                                itemval = Convert.ToInt32(dataReader["itemval"]),
                                item_bag_id = Convert.ToInt32(dataReader["item_bag_id"]),
                            };
                            var statsString = dataReader["itemstats"].ToString();
                            var stats = statsString.Split(commaSep, StringSplitOptions.RemoveEmptyEntries);
                            usr.itemstats = new int[(int)Stats.StatCount];
                            for (var i = 0; i < (int)Stats.StatCount && i < stats.Length; i++)
                            {
                                usr.itemstats[i] = int.Parse(stats[i]);
                                if (usr.itemstats[i] > Options.MaxStatValue) usr.itemstats[i] = Options.MaxStatValue;
                            }
                            BagItems.Add(usr);
                        }
                    }
                }
            }
        }

        private void LoadBanks()
        {
            var commaSep = new char[1];
            commaSep[0] = ',';
            lock (_dbLock)
            {
                var query = "SELECT * from char_bank;";
                using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
                {
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var usr = new Bank11()
                            {
                                playerid = Convert.ToInt32(dataReader["char_id"]),
                                slot = Convert.ToInt32(dataReader["slot"]),
                                itemnum = Convert.ToInt32(dataReader["itemnum"]),
                                itemval = Convert.ToInt32(dataReader["itemval"]),
                                item_bag_id = Convert.ToInt32(dataReader["item_bag_id"]),
                            };
                            var statsString = dataReader["itemstats"].ToString();
                            var stats = statsString.Split(commaSep, StringSplitOptions.RemoveEmptyEntries);
                            usr.itemstats = new int[(int)Stats.StatCount];
                            for (var i = 0; i < (int)Stats.StatCount && i < stats.Length; i++)
                            {
                                usr.itemstats[i] = int.Parse(stats[i]);
                                if (usr.itemstats[i] > Options.MaxStatValue) usr.itemstats[i] = Options.MaxStatValue;
                            }
                            Bank.Add(usr);
                        }
                    }
                }
            }
        }

        private void LoadInventories()
        {
            var commaSep = new char[1];
            commaSep[0] = ',';
            lock (_dbLock)
            {
                var query = "SELECT * from char_inventory;";
                using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
                {
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var usr = new Inventory11()
                            {
                                playerid = Convert.ToInt32(dataReader["char_id"]),
                                slot = Convert.ToInt32(dataReader["slot"]),
                                itemnum = Convert.ToInt32(dataReader["itemnum"]),
                                itemval = Convert.ToInt32(dataReader["itemval"]),
                                item_bag_id = Convert.ToInt32(dataReader["item_bag_id"]),
                            };
                            var statsString = dataReader["itemstats"].ToString();
                            var stats = statsString.Split(commaSep, StringSplitOptions.RemoveEmptyEntries);
                            usr.itemstats = new int[(int)Stats.StatCount];
                            for (var i = 0; i < (int)Stats.StatCount && i < stats.Length; i++)
                            {
                                usr.itemstats[i] = int.Parse(stats[i]);
                                if (usr.itemstats[i] > Options.MaxStatValue) usr.itemstats[i] = Options.MaxStatValue;
                            }
                            Items.Add(usr);
                        }
                    }
                }
            }
        }

        private void LoadBags()
        {
            lock (_dbLock)
            {
                var query = "SELECT * from bags;";
                using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
                {
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var usr = new Bag11()
                            {
                                bagid = Convert.ToInt32(dataReader["bag_id"]),
                                slots = Convert.ToInt32(dataReader["slot_count"]),
                            };
                            Bags.Add(usr);
                        }
                    }
                }
            }
        }

        private void LoadSpells()
        {
            lock (_dbLock)
            {
                var query = "SELECT * from char_spells;";
                using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
                {
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var usr = new Spell11()
                            {
                                playerid = Convert.ToInt32(dataReader["char_id"]),
                                slot = Convert.ToInt32(dataReader["slot"]),
                                spellid = Convert.ToInt32(dataReader["spellnum"]),
                                spellcd = Convert.ToInt32(dataReader["spellcd"]),
                            };
                            Spells.Add(usr);
                        }
                    }
                }
            }
        }

        private void LoadQuests()
        {
            lock (_dbLock)
            {
                var query = "SELECT * from char_quests;";
                using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
                {
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var usr = new Quest11()
                            {
                                playerid = Convert.ToInt32(dataReader["char_id"]),
                                questid = Convert.ToInt32(dataReader["quest_id"]),
                                taskid = Convert.ToInt32(dataReader["task"]),
                                taskprogress = Convert.ToInt32(dataReader["task_progress"]),
                                completed = Convert.ToBoolean(dataReader["completed"])
                            };
                            Quests.Add(usr);
                        }
                    }
                }
            }
        }

        private void LoadPVariables()
        {
            lock (_dbLock)
            {
                var query = "SELECT * from char_variables;";
                using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
                {
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var usr = new PVar11()
                            {
                                playerid = Convert.ToInt32(dataReader["char_id"]),
                                variableid = Convert.ToInt32(dataReader["slot"]),
                                value = Convert.ToInt32(dataReader["val"])
                            };
                            Variables.Add(usr);
                        }
                    }
                }
            }
        }

        private void LoadPSwitches()
        {
            lock (_dbLock)
            {
                var query = "SELECT * from char_switches;";
                using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
                {
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var usr = new PSwitch11()
                            {
                                playerid = Convert.ToInt32(dataReader["char_id"]),
                                switchid = Convert.ToInt32(dataReader["slot"]),
                                value = Convert.ToBoolean(dataReader["val"])
                            };
                            Switches.Add(usr);
                        }
                    }
                }
            }
        }

        private void LoadFriends()
        {
            lock (_dbLock)
            {
                var query = "SELECT * from char_friends;";
                using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
                {
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var usr = new Friend11()
                            {
                                owner_id = Convert.ToInt32(dataReader["char_id"]),
                                friend_id = Convert.ToInt32(dataReader["friend_id"]),
                            };
                            Friends.Add(usr);
                        }
                    }
                }
            }
        }

        private void LoadMutes()
        {
            lock (_dbLock)
            {
                var query = "SELECT * from mutes;";
                using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
                {
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var usr = new Mute11()
                            {
                                playerid = Convert.ToInt32(dataReader["id"]),
                                mutetime = Convert.ToInt64(dataReader["time"]),
                                accountid = Convert.ToInt32(dataReader["user"]),
                                ip = dataReader["ip"].ToString(),
                                unmutetime = Convert.ToInt64(dataReader["duration"]),
                                reason = dataReader["reason"].ToString(),
                                muter = dataReader["banner"].ToString()
                            };
                            Mutes.Add(usr);
                        }
                    }
                }
            }
        }

        private void LoadBans()
        {
            lock (_dbLock)
            {
                var query = "SELECT * from bans;";
                using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
                {
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var usr = new Ban11()
                            {
                                playerid = Convert.ToInt32(dataReader["id"]),
                                bantime = Convert.ToInt64(dataReader["time"]),
                                accountid = Convert.ToInt32(dataReader["user"]),
                                ip = dataReader["ip"].ToString(),
                                unbantime = Convert.ToInt64(dataReader["duration"]),
                                reason = dataReader["reason"].ToString(),
                                banner = dataReader["banner"].ToString()
                            };
                            Bans.Add(usr);
                        }
                    }
                }
            }
        }

        private void LoadUsers()
        {
            lock (_dbLock)
            {
                var query = "SELECT * from users;";
                using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
                {
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var usr = new User11()
                            {
                                id = Convert.ToInt32(dataReader["id"]),
                                user = dataReader["user"].ToString(),
                                pass = dataReader["pass"].ToString(),
                                salt = dataReader["salt"].ToString(),
                                email = dataReader["email"].ToString(),
                                power = Convert.ToInt32(dataReader["power"])
                            };
                            Users.Add(usr.id,usr);
                        }
                    }
                }
            }
        }

        private void LoadCharacters()
        {
            var commaSep = new char[1];
            commaSep[0] = ',';
            lock (_dbLock)
            {
                var query = "SELECT * from characters where deleted = 0;";
                using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
                {
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var usr = new Character11()
                            {
                                id = Convert.ToInt32(dataReader["id"]),
                                userid = Convert.ToInt32(dataReader["user_id"]),
                                name = dataReader["name"].ToString(),
                                map = Convert.ToInt32(dataReader["map"]),
                                x = Convert.ToInt32(dataReader["x"]),
                                y = Convert.ToInt32(dataReader["y"]),
                                z = Convert.ToInt32(dataReader["z"]),
                                dir = Convert.ToInt32(dataReader["dir"]),
                                sprite = dataReader["sprite"].ToString(),
                                face = dataReader["face"].ToString(),
                                classid = Convert.ToInt32(dataReader["class"]),
                                gender = Convert.ToInt32(dataReader["gender"]),
                                level = Convert.ToInt32(dataReader["level"]),
                                exp = Convert.ToInt32(dataReader["exp"]),
                                statpoints = Convert.ToInt32(dataReader["statpoints"]),
                                last_online = Convert.ToInt64(dataReader["last_online"])
                            };

                            var maxVitalString = dataReader["maxvitals"].ToString();
                            var maxVitals = maxVitalString.Split(commaSep, StringSplitOptions.RemoveEmptyEntries);
                            usr.maxvitals = new int[(int)Vitals.VitalCount];
                            usr.vitals = new int[(int)Vitals.VitalCount];
                            for (var i = 0; i < (int)Vitals.VitalCount && i < maxVitals.Length; i++)
                            {
                                usr.maxvitals[i] = int.Parse(maxVitals[i]);
                            }
                            var vitalString = dataReader["vitals"].ToString();
                            var vitals = vitalString.Split(commaSep, StringSplitOptions.RemoveEmptyEntries);
                            for (var i = 0; i < (int)Vitals.VitalCount && i < vitals.Length; i++)
                            {
                                usr.vitals[i] = int.Parse(vitals[i]);
                            }
                            var statsString = dataReader["stats"].ToString();
                            var stats = statsString.Split(commaSep, StringSplitOptions.RemoveEmptyEntries);
                            usr.stats = new int[(int)Stats.StatCount];
                            for (var i = 0; i < (int)Stats.StatCount && i < stats.Length; i++)
                            {
                                usr.stats[i] = int.Parse(stats[i]);
                                if (usr.stats[i] > Options.MaxStatValue) usr.stats[i] = Options.MaxStatValue;
                            }
                            var equipmentString = dataReader["equipment"].ToString();
                            var equipment = equipmentString.Split(commaSep, StringSplitOptions.RemoveEmptyEntries);
                            usr.equipment = new int[(int)Options.EquipmentSlots.Count];
                            for (var i = 0; i < (int)Options.EquipmentSlots.Count && i < equipment.Length; i++)
                            {
                                usr.equipment[i] = int.Parse(equipment[i]);
                            }

                            Characters.Add(usr.id, usr);
                        }
                    }
                }
            }
        }

        //Game Object Saving/Loading
        private void LoadAllGameObjects()
        {
            foreach (var val in Enum.GetValues(typeof(GameObjectType)))
            {
                objs.Add((GameObjectType)val, new Dictionary<int, string>());
            }
            LoadGameObjects((GameObjectType)GameObjectType.Item);
            foreach (var val in Enum.GetValues(typeof(GameObjectType)))
            {
                if ((GameObjectType)val != GameObjectType.Time && (GameObjectType)val != GameObjectType.Item)
                {
                    LoadGameObjects((GameObjectType)val);
                }
            }
        }
        private static void ClearGameObjects(GameObjectType type)
        {
            switch (type)
            {
                case GameObjectType.Animation:
                    AnimationBase.Lookup.Clear();
                    break;
                case GameObjectType.Class:
                    ClassBase.Lookup.Clear();
                    break;
                case GameObjectType.Item:
                    ItemBase.Lookup.Clear();
                    break;
                case GameObjectType.Npc:
                    NpcBase.Lookup.Clear();
                    break;
                case GameObjectType.Projectile:
                    ProjectileBase.Lookup.Clear();
                    break;
                case GameObjectType.Quest:
                    QuestBase.Lookup.Clear();
                    break;
                case GameObjectType.Resource:
                    ResourceBase.Lookup.Clear();
                    break;
                case GameObjectType.Shop:
                    ShopBase.Lookup.Clear();
                    break;
                case GameObjectType.Spell:
                    SpellBase.Lookup.Clear();
                    break;
                case GameObjectType.Bench:
                    BenchBase.Lookup.Clear();
                    break;
                case GameObjectType.Map:
                    MapBase.Lookup.Clear();
                    break;
                case GameObjectType.CommonEvent:
                    EventBase.Lookup.Clear();
                    break;
                case GameObjectType.PlayerSwitch:
                    PlayerSwitchBase.Lookup.Clear();
                    break;
                case GameObjectType.PlayerVariable:
                    PlayerVariableBase.Lookup.Clear();
                    break;
                case GameObjectType.ServerSwitch:
                    ServerSwitchBase.Lookup.Clear();
                    break;
                case GameObjectType.ServerVariable:
                    ServerVariableBase.Lookup.Clear();
                    break;
                case GameObjectType.Tileset:
                    TilesetBase.Lookup.Clear();
                    break;
                case GameObjectType.Time:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void LoadGameObject(GameObjectType type, int index, byte[] data)
        {
            var dict = objs[type];
            switch (type)
            {
                case GameObjectType.Animation:
                    var anim = new AnimationBase(index);
                    anim.Load(data);
                    AnimationBase.Lookup.Set(index, anim);
                    dict.Add(index, anim.JsonData);
                    break;
                case GameObjectType.Bench:
                    var bench = new BenchBase(index);
                    bench.Load(data);
                    BenchBase.Lookup.Set(index, bench);
                    SaveGameObject(bench);
                    break;
                case GameObjectType.Class:
                    var cls = new ClassBase(index);
                    cls.Load(data);
                    ClassBase.Lookup.Set(index, cls);
                    dict.Add(index, cls.JsonData);
                    break;
                case GameObjectType.Item:
                    var itm = new ItemBase(index);
                    itm.Load(data);
                    ItemBase.Lookup.Set(index, itm);
                    dict.Add(index, itm.JsonData);
                    break;
                case GameObjectType.Npc:
                    var npc = new NpcBase(index);
                    npc.Load(data);
                    NpcBase.Lookup.Set(index, npc);
                    dict.Add(index, npc.JsonData);
                    break;
                case GameObjectType.Projectile:
                    var proj = new ProjectileBase(index);
                    proj.Load(data);
                    ProjectileBase.Lookup.Set(index, proj);
                    dict.Add(index, proj.JsonData);
                    break;
                case GameObjectType.Quest:
                    var qst = new QuestBase(index);
                    qst.Load(data);
                    QuestBase.Lookup.Set(index, qst);
                    dict.Add(index, qst.JsonData);
                    break;
                case GameObjectType.Resource:
                    var res = new ResourceBase(index);
                    res.Load(data);
                    ResourceBase.Lookup.Set(index, res);
                    dict.Add(index, res.JsonData);
                    break;
                case GameObjectType.Shop:
                    var shp = new ShopBase(index);
                    shp.Load(data);
                    ShopBase.Lookup.Set(index, shp);
                    dict.Add(index, shp.JsonData);
                    break;
                case GameObjectType.Spell:
                    var spl = new SpellBase(index);
                    spl.Load(data);
                    SpellBase.Lookup.Set(index, spl);
                    dict.Add(index, spl.JsonData);
                    break;
                case GameObjectType.Map:
                    var map = new MapBase(index, false);
                    MapBase.Lookup.Set(index, map);
                    map.Load(data);
                    map.TileData = GetMapTiles(index);
                    dict.Add(index, map.JsonData);
                    break;
                case GameObjectType.CommonEvent:
                    var buffer = new Intersect_Convert_Lib.ByteBuffer();
                    buffer.WriteBytes(data);
                    var evt = new EventBase(index, buffer, true);
                    EventBase.Lookup.Set(index, evt);
                    buffer.Dispose();
                    dict.Add(index, evt.JsonData);
                    break;
                case GameObjectType.PlayerSwitch:
                    var pswitch = new PlayerSwitchBase(index);
                    pswitch.Load(data);
                    PlayerSwitchBase.Lookup.Set(index, pswitch);
                    dict.Add(index, pswitch.JsonData);
                    break;
                case GameObjectType.PlayerVariable:
                    var pvar = new PlayerVariableBase(index);
                    pvar.Load(data);
                    PlayerVariableBase.Lookup.Set(index, pvar);
                    dict.Add(index, pvar.JsonData);
                    break;
                case GameObjectType.ServerSwitch:
                    var sswitch = new ServerSwitchBase(index);
                    sswitch.Load(data);
                    ServerSwitchBase.Lookup.Set(index, sswitch);
                    dict.Add(index, sswitch.JsonData);
                    break;
                case GameObjectType.ServerVariable:
                    var svar = new ServerVariableBase(index);
                    svar.Load(data);
                    ServerVariableBase.Lookup.Set(index, svar);
                    dict.Add(index, svar.JsonData);
                    break;
                case GameObjectType.Tileset:
                    var tset = new TilesetBase(index);
                    tset.Load(data);
                    TilesetBase.Lookup.Set(index, tset);
                    dict.Add(index, tset.JsonData);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void LoadGameObjects(GameObjectType gameObjectType)
        {
            if (gameObjectType == GameObjectType.CraftTables || gameObjectType == GameObjectType.Crafts) return;
            var nullIssues = "";
            lock (_dbLock)
            {
                var tableName = gameObjectType.GetTable();
                ClearGameObjects(gameObjectType);
                var query = "SELECT * from " + tableName + " WHERE " + GAME_OBJECT_DELETED + "=@" +
                            GAME_OBJECT_DELETED +
                            ";";
                using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DELETED, 0.ToString()));
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var index = Convert.ToInt32(dataReader[GAME_OBJECT_ID]);
                            if (dataReader[GAME_OBJECT_DATA].GetType() != typeof(DBNull))
                            {
                                LoadGameObject(gameObjectType, index, (byte[])dataReader[GAME_OBJECT_DATA]);
                            }
                            else
                            {
                                nullIssues += "Tried to load null value for index " + index + " of " + tableName +
                                              Environment.NewLine;
                            }
                        }
                    }
                }
            }
            if (nullIssues != "")
            {
                throw (new Exception("Tried to load one or more null game objects!" + Environment.NewLine +
                                     nullIssues));
            }
        }

        public IDatabaseObject AddGameObject(GameObjectType gameObjectType)
        {
            var insertQuery = $"INSERT into {gameObjectType.GetTable()} (" + GAME_OBJECT_DATA + ") VALUES (@" + GAME_OBJECT_DATA + ")" + "; SELECT last_insert_rowid();";
            var index = -1;
            using (var cmd = _dbConnection.CreateCommand())
            {
                cmd.CommandText = insertQuery;
                cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DATA, new byte[1]));
                index = (int)((long)cmd.ExecuteScalar());
            }
            if (index > -1)
            {
                IDatabaseObject dbObj = null;
                switch (gameObjectType)
                {
                    case GameObjectType.CraftTables:
                        var obje = new CraftingTableBase(index);
                        dbObj = obje;
                        CraftingTableBase.Lookup.Set(index, obje);
                        break;
                    case GameObjectType.Crafts:
                        var crft = new CraftBase(index);
                        dbObj = crft;
                        CraftBase.Lookup.Set(index, crft);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(gameObjectType), gameObjectType, null);
                }

                SaveGameObject(dbObj);
                return dbObj;
            }
            return null;
        }

        public void SaveGameObject(IDatabaseObject gameObject)
        {
            if (gameObject == null)
            {
                Log.Error("Attempted to persist null game object to the database.");
            }

            if (gameObject.Type == GameObjectType.Bench)
            {
                //Add any crafts
                var go = (BenchBase)gameObject;
                var newCrafts = new List<int>();
                var matchedId = -1;
                foreach (var craft in go.Crafts)
                {
                    var craftId = -1;
                    //check if we already have the craft anywhere
                    foreach (var exCraft in CraftBase.Lookup.Values)
                    {
                        var cft = (CraftBase) exCraft;
                        var match = true;
                        if (cft.Item != craft.Item) match = false;
                        if (cft.Time != craft.Time) match = false;

                        Dictionary<int,int> cftItems = new Dictionary<int, int>();
                        Dictionary<int,int> craftItems = new Dictionary<int, int>();
                        foreach (var item in cft.Ingredients)
                        {
                            if (cftItems.ContainsKey(item.Item))
                            {
                                cftItems[item.Item] += item.Quantity;
                            }
                            else
                            {
                                cftItems.Add(item.Item,item.Quantity);
                            }
                        }

                        foreach (var item in craft.Ingredients)
                        {
                            if (craftItems.ContainsKey(item.Item))
                            {
                                craftItems[item.Item] += item.Quantity;
                            }
                            else
                            {
                                craftItems.Add(item.Item, item.Quantity);
                            }
                        }

                        craftItems.Remove(-1);
                        cftItems.Remove(-1);

                        if (cftItems.Count == craftItems.Count)
                        {
                            foreach (var itm in cftItems.Keys)
                            {
                                if (!craftItems.ContainsKey(itm) || craftItems[itm] != cftItems[itm]) match = false;
                            }
                        }
                        else
                        {
                            match = false;
                        }

                        if (match)
                        {
                            //Found a duplicate ingredient!
                            matchedId = exCraft.Index;
                        }
                    }

                    //add the craft if needed
                    if (matchedId != -1)
                    {
                        newCrafts.Add(matchedId);
                    }
                    else
                    {
                        //Create new craft, copy over settings, save
                        var cft = new CraftBase(craftsIndex);
                        cft.Name = ItemBase.GetName(craft.Item);
                        cft.Time = craft.Time;
                        cft.Item = craft.Item;
                        cft.Ingredients = craft.Ingredients;
                        objs[GameObjectType.Crafts].Add(craftsIndex, cft.JsonData);
                        craftsIndex++;
                        newCrafts.Add(cft.Index);
                    }
                }

                //Create a table with the bench name and crafts
                var tbl = new CraftingTableBase(craftTablesIndex);
                tbl.Name = go.Name;
                tbl.Crafts = newCrafts;
                objs[GameObjectType.CraftTables].Add(craftTablesIndex, tbl.JsonData);
                craftTablesIndex++;
                return;
            }
            return;
            lock (_dbLock)
            {
                var insertQuery = "UPDATE " + gameObject.DatabaseTable + " set " + GAME_OBJECT_DELETED + "=@" +
                                  GAME_OBJECT_DELETED + "," + GAME_OBJECT_DATA + "=@" + GAME_OBJECT_DATA + " WHERE " +
                                  GAME_OBJECT_ID + "=@" + GAME_OBJECT_ID + ";";
                using (SqliteCommand cmd = new SqliteCommand(insertQuery, _dbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_ID, gameObject.Index));
                    cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DELETED, 0.ToString()));
                    if (gameObject.BinaryData != null)
                    {
                        cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DATA, gameObject.JsonData));
                    }
                    else
                    {
                        throw (new Exception("Tried to save a null game object (should be deleted instead?) Table: " +
                                             gameObject.DatabaseTable + " Id: " + gameObject.Index));
                    }
                    cmd.ExecuteNonQuery();
                }
            }
        }


        //Time
        private void LoadTime()
        {
            var query = $"SELECT * from {TIME_TABLE};";
            using (var cmd = new SqliteCommand(query, _dbConnection))
            {
                using (var dataReader = cmd.ExecuteReader())
                {
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            if (dataReader[TIME_DATA].GetType() != typeof(DBNull))
                            {
                                var data = (byte[])dataReader[TIME_DATA];
                                if (data.Length > 1)
                                {
                                    TimeBase.GetTimeBase().LoadTimeBase(data);
                                }
                            }
                        }
                    }
                }
            }
            var json = TimeBase.GetTimeJson();
        }

        //Map Folders
        private void LoadMapFolders()
        {
            var query = $"SELECT * from {MAP_LIST_TABLE};";
            using (var cmd = new SqliteCommand(query, _dbConnection))
            {
                using (var dataReader = cmd.ExecuteReader())
                {
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            if (dataReader[MAP_LIST_DATA].GetType() != typeof(DBNull))
                            {
                                var data = (byte[])dataReader[MAP_LIST_DATA];
                                if (data.Length > 1)
                                {
                                    var myBuffer = new ByteBuffer();
                                    myBuffer.WriteBytes(data);
                                    MapList.GetList().Load(myBuffer, MapBase.Lookup, true, true);
                                }
                            }
                        }
                    }
                }
            }
            foreach (var map in MapBase.Lookup)
            {
                if (MapList.GetList().FindMap(map.Value.Index) == null)
                {
                    MapList.GetList().AddMap(map.Value.Index, MapBase.Lookup);
                }
            }
        }

        public byte[] GetMapTiles(int index)
        {
            var nullIssues = "";
            var query = $"SELECT * from {MAP_TILES_TABLE} WHERE " + MAP_TILES_MAP_ID + "=@" + MAP_TILES_MAP_ID +
                        ";";
            using (var cmd = new SqliteCommand(query, _dbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_TILES_MAP_ID, index));
                using (var dataReader = cmd.ExecuteReader())
                {
                    if (dataReader.HasRows && dataReader.Read())
                    {
                        if (dataReader[MAP_TILES_DATA].GetType() != typeof(DBNull))
                        {
                            return (byte[])dataReader[MAP_TILES_DATA];
                        }
                    }
                    else
                    {
                        return new byte[Options.LayerCount * Options.MapWidth * Options.MapHeight * 13];
                    }
                }
            }
            return null;
        }
    }
}