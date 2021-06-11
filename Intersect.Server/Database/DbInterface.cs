using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Amib.Threading;
using Intersect.Collections;
using Intersect.Config;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Logging;
using Intersect.Logging.Output;
using Intersect.Models;
using Intersect.Server.Core;
using Intersect.Server.Database.GameData;
using Intersect.Server.Database.Logging;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Entities;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Maps;
using Intersect.Server.Networking;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using MySql.Data.MySqlClient;

namespace Intersect.Server.Database
{

    public static class DbInterface
    {

        /// <summary>
        /// This is our thread pool for handling game-loop database interactions.
        /// Min/Max Number of Threads & Idle Timeouts are set via server config.
        /// </summary>
        public static SmartThreadPool Pool = new SmartThreadPool(
                new STPStartInfo()
                {
                    ThreadPoolName = "DatabasePool",
                    IdleTimeout = Options.Instance.Processing.DatabaseThreadIdleTimeout,
                    MinWorkerThreads = Options.Instance.Processing.MinDatabaseThreads,
                    MaxWorkerThreads = Options.Instance.Processing.MaxDatabaseThreads
                }
            );

        private const string GameDbFilename = "resources/gamedata.db";

        private const string PlayersDbFilename = "resources/playerdata.db";

        private static Logger gameDbLogger { get; set; }

        private static Logger playerDbLogger { get; set; }

        public static Dictionary<string, ServerVariableBase> ServerVariableEventTextLookup = new Dictionary<string, ServerVariableBase>();

        public static Dictionary<string, PlayerVariableBase> PlayerVariableEventTextLookup = new Dictionary<string, PlayerVariableBase>();

        public static Dictionary<string, GuildVariableBase> GuildVariableEventTextLookup = new Dictionary<string, GuildVariableBase>();

        public static ConcurrentDictionary<Guid, ServerVariableBase> UpdatedServerVariables = new ConcurrentDictionary<Guid, ServerVariableBase>();

        private static List<MapGrid> mapGrids = new List<MapGrid>();

        public static long RegisteredPlayers
        {
            get
            {
                using (var context = CreatePlayerContext())
                {
                    return context.Players.Count();
                }
            }
        }

        /// <summary>
        /// Creates a game context to query. Best practice is to scope this within a using statement.
        /// </summary>
        /// <param name="readOnly">Defines whether or not the context should initialize with change tracking. If readonly is true then SaveChanges will not work.</param>
        /// <returns></returns>
        public static GameContext CreateGameContext(bool readOnly = true)
        {
            return new GameContext(
                CreateConnectionStringBuilder(Options.GameDb ?? throw new InvalidOperationException(), GameDbFilename),
                Options.GameDb.Type, readOnly, gameDbLogger, Options.GameDb.LogLevel
            );
        }

        /// <summary>
        /// Creates a game context to query. Best practice is to scope this within a using statement.
        /// </summary>
        /// <param name="readOnly">Defines whether or not the context should initialize with change tracking. If readonly is true then SaveChanges will not work.</param>
        /// <returns></returns>
        public static PlayerContext CreatePlayerContext(bool readOnly = true)
        {
            return new PlayerContext(
                    CreateConnectionStringBuilder(
                        Options.PlayerDb ?? throw new InvalidOperationException(), PlayersDbFilename
                    ), Options.PlayerDb.Type, readOnly, playerDbLogger, Options.PlayerDb.LogLevel
                );
        }

        public static void InitializeDbLoggers()
        {
            if (Options.GameDb.LogLevel > LogLevel.None)
            {
                gameDbLogger = new Logger(
                    new LogConfiguration
                    {
                        Tag = "GAMEDB",
                        LogLevel = Options.GameDb.LogLevel,
                        Outputs = ImmutableList.Create<ILogOutput>(
                            new FileOutput(Log.SuggestFilename(null, "gamedb"), LogLevel.Debug)
                        )
                    }
                );
            }

            if (Options.PlayerDb.LogLevel > LogLevel.None)
            {
                playerDbLogger = new Logger(
                    new LogConfiguration
                    {
                        Tag = "PLAYERDB",
                        LogLevel = Options.PlayerDb.LogLevel,
                        Outputs = ImmutableList.Create<ILogOutput>(
                            new FileOutput(Log.SuggestFilename(null, "playerdb"), LogLevel.Debug)
                        )
                    }
                );
            }
        }

        //Check Directories
        public static void CheckDirectories()
        {
            if (!Directory.Exists("resources"))
            {
                Directory.CreateDirectory("resources");
            }
        }

        //As of now Database writes only occur on player saving & when editors make game changes
        //Database writes are actually pretty rare. And even player saves are offloaded as tasks so
        //if delayed it won't matter much.
        //TODO: Options for saving frequency and number of backups to keep.
        public static void BackupDatabase()
        {
        }

        public static DbConnectionStringBuilder CreateConnectionStringBuilder(
            DatabaseOptions databaseOptions,
            string filename
        )
        {
            switch (databaseOptions.Type)
            {
                case DatabaseOptions.DatabaseType.SQLite:
                    return new SqliteConnectionStringBuilder($"Data Source={filename}");

                case DatabaseOptions.DatabaseType.MySQL:
                    return new MySqlConnectionStringBuilder
                    {
                        Server = databaseOptions.Server,
                        Port = databaseOptions.Port,
                        Database = databaseOptions.Database,
                        UserID = databaseOptions.Username,
                        Password = databaseOptions.Password
                    };

                default:
                    throw new ArgumentOutOfRangeException(nameof(databaseOptions.Type));
            }
        }

        // Database setup, version checking
        internal static bool InitDatabase(IServerContext serverContext)
        {
            using (var gameContext = CreateGameContext(readOnly: false))
            {

                using (var playerContext = CreatePlayerContext(readOnly:false))
                {

                    Logging.LoggingContext.Configure(
                        DatabaseOptions.DatabaseType.SQLite, Logging.LoggingContext.DefaultConnectionStringBuilder
                    );

                    ContextProvider.Add(Logging.LoggingContext.Create());

                    // We don't want anyone running the old migration tool accidentally
                    try
                    {
                        if (File.Exists("Intersect Migration Tool.exe"))
                        {
                            File.Delete("Intersect Migration Tool.exe");
                        }

                        if (File.Exists("Intersect Migration Tool.pdb"))
                        {
                            File.Delete("Intersect Migration Tool.pdb");
                        }

                        if (File.Exists("Intersect Migration Tool.mdb"))
                        {
                            File.Delete("Intersect Migration Tool.mdb");
                        }
                    }
                    catch
                    {
                        // ignored
                    }

                    var gameMigrations = gameContext.PendingMigrations;
                    var showGameMigrationWarning = gameMigrations.Any() && !gameMigrations.Contains("20180905042857_Initial");
                    var playerMigrations = playerContext.PendingMigrations;
                    var showPlayerMigrationWarning =
                        playerMigrations.Any() && !playerMigrations.Contains("20180927161502_InitialPlayerDb");

                    if (showGameMigrationWarning || showPlayerMigrationWarning)
                    {
                        Console.WriteLine();
                        Console.WriteLine(Strings.Database.upgraderequired);
                        Console.WriteLine(
                            Strings.Database.upgradebackup.ToString(Strings.Database.upgradeready, Strings.Database.upgradeexit)
                        );

                        Console.WriteLine();
                        while (true)
                        {
                            Console.Write("> ");
                            var input = Console.ReadLine().Trim();
                            if (input == Strings.Database.upgradeready.ToString().Trim())
                            {
                                break;
                            }
                            else if (input.ToLower() == Strings.Database.upgradeexit.ToString().Trim().ToLower())
                            {
                                Environment.Exit(1);

                                return false;
                            }
                        }

                        Console.WriteLine();
                        Console.WriteLine(
                            "Please wait! Migrations can take several minutes, and even longer if you are using MySQL databases!"
                        );
                    }

                    gameContext.Database.Migrate();
                    var remainingGameMigrations = gameContext.PendingMigrations;
                    var processedGameMigrations = new List<string>(gameMigrations);
                    foreach (var itm in remainingGameMigrations)
                    {
                        processedGameMigrations.Remove(itm);
                    }

                    gameContext.MigrationsProcessed(processedGameMigrations.ToArray());

                    playerContext.Database.Migrate();
                    var remainingPlayerMigrations = playerContext.PendingMigrations;
                    var processedPlayerMigrations = new List<string>(playerMigrations);
                    foreach (var itm in remainingPlayerMigrations)
                    {
                        processedPlayerMigrations.Remove(itm);
                    }

                    playerContext.MigrationsProcessed(processedPlayerMigrations.ToArray());
#if DEBUG
                    if (ServerContext.Instance.RestApi.Configuration.SeedMode)
                    {
                        playerContext.Seed();
                    }
#endif

                    try
                    {
                        using (var loggingContext = LoggingContext)
                        {
                            loggingContext.Database?.Migrate();
                        }
                    }
                    catch (Exception exception)
                    {
                        throw;
                    }

                    LoadAllGameObjects();
                    LoadTime();
                    OnClassesLoaded();
                    OnMapsLoaded();
                    CacheServerVariableEventTextLookups();
                    CachePlayerVariableEventTextLookups();
                    CacheGuildVariableEventTextLookups();
                }
            }

            return true;
        }

        public static Client GetPlayerClient(string username)
        {
            //Try to fetch a player entity by username, online or offline.
            //Check Online First
            lock (Globals.ClientLock)
            {
                foreach (var client in Globals.Clients)
                {
                    if (client.Entity != null && client.Name.ToLower() == username.ToLower())
                    {
                        return client;
                    }
                }
            }

            return null;
        }

        public static void SetPlayerPower(string username, UserRights power)
        {
            var user = User.Find(username);
            if (user != null)
            {
                user.Power = power;
                user.Save();
            }
            else
            {
                Console.WriteLine(Strings.Account.doesnotexist);
            }
        }

        public static bool SetPlayerPower(User user, UserRights power)
        {
            if (user != null)
            {
                user.Power = power;
                user.Save();

                return true;
            }
            else
            {
                Console.WriteLine(Strings.Account.doesnotexist);

                return false;
            }
        }

        //User Info
        public static bool AccountExists(string accountname)
        {
            return User.Find(accountname) != null;
        }

        public static string UsernameFromEmail(string email)
        {
            var user = User.FindFromEmail(email);
            if (user != null)
            {
                return user.Name;
            }
            return null;
        }

        public static Player GetUserCharacter(User user, Guid characterId)
        {
            if (user == null) return null;
            foreach (var character in user.Players)
            {
                if (character.Id == characterId)
                {
                    return character;
                }
            }

            return null;
        }

        public static void CreateAccount(
            Client client,
            string username,
            string password,
            string email
        )
        {
            var sha = new SHA256Managed();

            //Generate a Salt
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[20];
            rng.GetBytes(buff);
            var salt = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(Convert.ToBase64String(buff))))
                .Replace("-", "");

            var user = new User
            {
                Name = username,
                Email = email,
                Salt = salt,
                Password = User.SaltPasswordHash(password, salt),
                Power = UserRights.None,
            };

            if (User.Count() == 0)
            {
                user.Power = UserRights.Admin;
            }

            user.Save();

            client?.SetUser(user);
        }

        public static void ResetPass(User user, string password)
        {
            var sha = new SHA256Managed();

            //Generate a Salt
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[20];
            rng.GetBytes(buff);
            var salt = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(Convert.ToBase64String(buff))))
                .Replace("-", "");

            user.Salt = salt;
            user.Password = User.SaltPasswordHash(password, salt);
            user.Save();
        }

        public static bool BagEmpty(Bag bag)
        {
            for (var i = 0; i < bag.Slots.Count; i++)
            {
                if (bag.Slots[i] != null)
                {
                    var item = ItemBase.Get(bag.Slots[i].ItemId);
                    if (item != null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        //Game Object Saving/Loading
        private static void LoadAllGameObjects()
        {
            foreach (var value in Enum.GetValues(typeof(GameObjectType)))
            {
                Debug.Assert(value != null, "value != null");
                var type = (GameObjectType) value;
                if (type == GameObjectType.Time)
                {
                    continue;
                }

                LoadGameObjects(type);
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
                case GameObjectType.CraftTables:
                    CraftingTableBase.Lookup.Clear();

                    break;
                case GameObjectType.Crafts:
                    CraftBase.Lookup.Clear();

                    break;
                case GameObjectType.Map:
                    MapBase.Lookup.Clear();

                    break;
                case GameObjectType.Event:
                    EventBase.Lookup.Clear();

                    break;
                case GameObjectType.PlayerVariable:
                    PlayerVariableBase.Lookup.Clear();

                    break;
                case GameObjectType.ServerVariable:
                    ServerVariableBase.Lookup.Clear();

                    break;
                case GameObjectType.Tileset:
                    TilesetBase.Lookup.Clear();

                    break;
                case GameObjectType.Time:
                    break;
                case GameObjectType.GuildVariable:
                    GuildVariableBase.Lookup.Clear();

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static void LoadGameObjects(GameObjectType gameObjectType)
        {
            ClearGameObjects(gameObjectType);
            try
            {
                using (var context = CreateGameContext(readOnly: true))
                {
                    switch (gameObjectType)
                    {
                        case GameObjectType.Animation:
                            foreach (var anim in context.Animations)
                            {
                                AnimationBase.Lookup.Set(anim.Id, anim);
                            }

                            break;
                        case GameObjectType.Class:
                            foreach (var cls in context.Classes)
                            {
                                ClassBase.Lookup.Set(cls.Id, cls);
                            }

                            break;
                        case GameObjectType.Item:
                            foreach (var itm in context.Items)
                            {
                                ItemBase.Lookup.Set(itm.Id, itm);
                            }

                            break;
                        case GameObjectType.Npc:
                            foreach (var npc in context.Npcs)
                            {
                                NpcBase.Lookup.Set(npc.Id, npc);
                            }

                            break;
                        case GameObjectType.Projectile:
                            foreach (var proj in context.Projectiles)
                            {
                                ProjectileBase.Lookup.Set(proj.Id, proj);
                            }

                            break;
                        case GameObjectType.Quest:
                            foreach (var qst in context.Quests)
                            {
                                QuestBase.Lookup.Set(qst.Id, qst);
                            }

                            break;
                        case GameObjectType.Resource:
                            foreach (var res in context.Resources)
                            {
                                ResourceBase.Lookup.Set(res.Id, res);
                            }

                            break;
                        case GameObjectType.Shop:
                            foreach (var shp in context.Shops)
                            {
                                ShopBase.Lookup.Set(shp.Id, shp);
                            }

                            break;
                        case GameObjectType.Spell:
                            foreach (var spl in context.Spells)
                            {
                                SpellBase.Lookup.Set(spl.Id, spl);
                            }

                            break;
                        case GameObjectType.CraftTables:
                            foreach (var craft in context.CraftingTables)
                            {
                                CraftingTableBase.Lookup.Set(craft.Id, craft);
                            }

                            break;
                        case GameObjectType.Crafts:
                            foreach (var craft in context.Crafts)
                            {
                                CraftBase.Lookup.Set(craft.Id, craft);
                            }

                            break;
                        case GameObjectType.Map:
                            foreach (var map in context.Maps)
                            {
                                MapInstance.Lookup.Set(map.Id, map);
                                if (Options.Instance.MapOpts.Layers.DestroyOrphanedLayers)
                                {
                                    map.DestroyOrphanedLayers();
                                }
                            }

                            break;
                        case GameObjectType.Event:
                            foreach (var evt in context.Events)
                            {
                                EventBase.Lookup.Set(evt.Id, evt);
                            }

                            break;
                        case GameObjectType.PlayerVariable:
                            foreach (var psw in context.PlayerVariables)
                            {
                                PlayerVariableBase.Lookup.Set(psw.Id, psw);
                            }

                            break;
                        case GameObjectType.ServerVariable:
                            foreach (var psw in context.ServerVariables)
                            {
                                ServerVariableBase.Lookup.Set(psw.Id, psw);
                            }

                            break;
                        case GameObjectType.Tileset:
                            foreach (var psw in context.Tilesets)
                            {
                                TilesetBase.Lookup.Set(psw.Id, psw);
                            }

                            break;
                        case GameObjectType.Time:
                            break;
                        case GameObjectType.GuildVariable:
                            foreach (var psw in context.GuildVariables)
                            {
                                GuildVariableBase.Lookup.Set(psw.Id, psw);
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(gameObjectType), gameObjectType, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public static IDatabaseObject AddGameObject(GameObjectType gameObjectType)
        {
            return AddGameObject(gameObjectType, Guid.Empty);
        }

        public static IDatabaseObject AddGameObject(GameObjectType gameObjectType, Guid predefinedid)
        {
            if (predefinedid == Guid.Empty)
            {
                predefinedid = Guid.NewGuid();
            }

            IDatabaseObject dbObj = null;
            switch (gameObjectType)
            {
                case GameObjectType.Animation:
                    dbObj = new AnimationBase(predefinedid);

                    break;
                case GameObjectType.Class:
                    dbObj = new ClassBase(predefinedid);

                    break;
                case GameObjectType.Item:
                    dbObj = new ItemBase(predefinedid);

                    break;
                case GameObjectType.Npc:
                    dbObj = new NpcBase(predefinedid);

                    break;
                case GameObjectType.Projectile:
                    dbObj = new ProjectileBase(predefinedid);

                    break;
                case GameObjectType.Resource:
                    dbObj = new ResourceBase(predefinedid);

                    break;
                case GameObjectType.Shop:
                    dbObj = new ShopBase(predefinedid);

                    break;
                case GameObjectType.Spell:
                    dbObj = new SpellBase(predefinedid);

                    break;
                case GameObjectType.CraftTables:
                    dbObj = new CraftingTableBase(predefinedid);

                    break;
                case GameObjectType.Crafts:
                    dbObj = new CraftBase(predefinedid);

                    break;
                case GameObjectType.Map:
                    dbObj = new MapInstance(predefinedid);

                    break;
                case GameObjectType.Event:
                    dbObj = new EventBase(predefinedid);

                    break;
                case GameObjectType.PlayerVariable:
                    dbObj = new PlayerVariableBase(predefinedid);

                    break;
                case GameObjectType.ServerVariable:
                    dbObj = new ServerVariableBase(predefinedid);

                    break;
                case GameObjectType.Tileset:
                    dbObj = new TilesetBase(predefinedid);

                    break;
                case GameObjectType.Time:
                    break;

                case GameObjectType.Quest:
                    dbObj = new QuestBase(predefinedid);
                    ((QuestBase) dbObj).StartEvent = (EventBase) AddGameObject(GameObjectType.Event);
                    ((QuestBase) dbObj).EndEvent = (EventBase) AddGameObject(GameObjectType.Event);
                    ((QuestBase) dbObj).StartEvent.CommonEvent = false;
                    ((QuestBase) dbObj).EndEvent.CommonEvent = false;

                    break;

                case GameObjectType.GuildVariable:
                    dbObj = new GuildVariableBase(predefinedid);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameObjectType), gameObjectType, null);
            }

            return dbObj == null ? null : AddGameObject(gameObjectType, dbObj);
        }

        public static IDatabaseObject AddGameObject(GameObjectType gameObjectType, IDatabaseObject dbObj)
        {
            try
            {
                using (var context = CreateGameContext(readOnly: false))
                {

                    switch (gameObjectType)
                    {
                        case GameObjectType.Animation:
                            context.Animations.Add((AnimationBase)dbObj);
                            AnimationBase.Lookup.Set(dbObj.Id, dbObj);

                            break;

                        case GameObjectType.Class:
                            context.Classes.Add((ClassBase)dbObj);
                            ClassBase.Lookup.Set(dbObj.Id, dbObj);

                            break;

                        case GameObjectType.Item:
                            context.Items.Add((ItemBase)dbObj);
                            ItemBase.Lookup.Set(dbObj.Id, dbObj);

                            break;
                        case GameObjectType.Npc:
                            context.Npcs.Add((NpcBase)dbObj);
                            NpcBase.Lookup.Set(dbObj.Id, dbObj);

                            break;

                        case GameObjectType.Projectile:
                            context.Projectiles.Add((ProjectileBase)dbObj);
                            ProjectileBase.Lookup.Set(dbObj.Id, dbObj);

                            break;

                        case GameObjectType.Quest:
                            context.Quests.Add((QuestBase)dbObj);
                            QuestBase.Lookup.Set(dbObj.Id, dbObj);

                            break;

                        case GameObjectType.Resource:
                            context.Resources.Add((ResourceBase)dbObj);
                            ResourceBase.Lookup.Set(dbObj.Id, dbObj);

                            break;

                        case GameObjectType.Shop:
                            context.Shops.Add((ShopBase)dbObj);
                            ShopBase.Lookup.Set(dbObj.Id, dbObj);

                            break;

                        case GameObjectType.Spell:
                            context.Spells.Add((SpellBase)dbObj);
                            SpellBase.Lookup.Set(dbObj.Id, dbObj);

                            break;

                        case GameObjectType.CraftTables:
                            context.CraftingTables.Add((CraftingTableBase)dbObj);
                            CraftingTableBase.Lookup.Set(dbObj.Id, dbObj);

                            break;

                        case GameObjectType.Crafts:
                            context.Crafts.Add((CraftBase)dbObj);
                            CraftBase.Lookup.Set(dbObj.Id, dbObj);

                            break;

                        case GameObjectType.Map:
                            context.Maps.Add((MapInstance)dbObj);
                            MapInstance.Lookup.Set(dbObj.Id, dbObj);

                            break;

                        case GameObjectType.Event:
                            context.Events.Add((EventBase)dbObj);
                            EventBase.Lookup.Set(dbObj.Id, dbObj);

                            break;

                        case GameObjectType.PlayerVariable:
                            context.PlayerVariables.Add((PlayerVariableBase)dbObj);
                            PlayerVariableBase.Lookup.Set(dbObj.Id, dbObj);

                            break;

                        case GameObjectType.ServerVariable:
                            context.ServerVariables.Add((ServerVariableBase)dbObj);
                            ServerVariableBase.Lookup.Set(dbObj.Id, dbObj);

                            break;

                        case GameObjectType.Tileset:
                            context.Tilesets.Add((TilesetBase)dbObj);
                            TilesetBase.Lookup.Set(dbObj.Id, dbObj);

                            break;

                        case GameObjectType.Time:
                            break;

                        case GameObjectType.GuildVariable:
                            context.GuildVariables.Add((GuildVariableBase)dbObj);
                            GuildVariableBase.Lookup.Set(dbObj.Id, dbObj);

                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(gameObjectType), gameObjectType, null);
                    }

                    context.ChangeTracker.DetectChanges();
                    context.Entry(dbObj).State = EntityState.Added;
                    context.SaveChanges();
                }

                return dbObj;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public static void DeleteGameObject(IDatabaseObject gameObject)
        {
            try
            {
                using (var context = CreateGameContext(readOnly: false))
                {
                    switch (gameObject.Type)
                    {
                        case GameObjectType.Animation:
                            context.Animations.Remove((AnimationBase)gameObject);

                            break;
                        case GameObjectType.Class:
                            context.Classes.Remove((ClassBase)gameObject);

                            break;
                        case GameObjectType.Item:
                            context.Items.Remove((ItemBase)gameObject);

                            break;
                        case GameObjectType.Npc:
                            context.Npcs.Remove((NpcBase)gameObject);

                            break;
                        case GameObjectType.Projectile:
                            context.Projectiles.Remove((ProjectileBase)gameObject);

                            break;
                        case GameObjectType.Quest:

                            if (((QuestBase)gameObject).StartEvent != null)
                            {
                                context.Events.Remove(((QuestBase)gameObject).StartEvent);
                                context.Entry(((QuestBase)gameObject).StartEvent).State = EntityState.Deleted;
                                EventBase.Lookup.Delete(((QuestBase)gameObject).StartEvent);
                            }

                            if (((QuestBase)gameObject).EndEvent != null)
                            {
                                context.Events.Remove(((QuestBase)gameObject).EndEvent);
                                context.Entry(((QuestBase)gameObject).EndEvent).State = EntityState.Deleted;
                                EventBase.Lookup.Delete(((QuestBase)gameObject).EndEvent);
                            }

                            foreach (var tsk in ((QuestBase)gameObject).Tasks)
                            {
                                if (tsk.CompletionEvent != null)
                                {
                                    context.Events.Remove(tsk.CompletionEvent);
                                    context.Entry(tsk.CompletionEvent).State = EntityState.Deleted;
                                    EventBase.Lookup.Delete(tsk.CompletionEvent);
                                }
                            }

                            context.Quests.Remove((QuestBase)gameObject);

                            break;
                        case GameObjectType.Resource:
                            context.Resources.Remove((ResourceBase)gameObject);

                            break;
                        case GameObjectType.Shop:
                            context.Shops.Remove((ShopBase)gameObject);

                            break;
                        case GameObjectType.Spell:
                            context.Spells.Remove((SpellBase)gameObject);

                            break;
                        case GameObjectType.CraftTables:
                            context.CraftingTables.Remove((CraftingTableBase)gameObject);

                            break;
                        case GameObjectType.Crafts:
                            context.Crafts.Remove((CraftBase)gameObject);

                            break;
                        case GameObjectType.Map:
                            context.Maps.Remove((MapInstance)gameObject);
                            MapInstance.Lookup.Delete(gameObject);

                            break;
                        case GameObjectType.Event:
                            context.Events.Remove((EventBase)gameObject);

                            break;
                        case GameObjectType.PlayerVariable:
                            context.PlayerVariables.Remove((PlayerVariableBase)gameObject);

                            break;
                        case GameObjectType.ServerVariable:
                            context.ServerVariables.Remove((ServerVariableBase)gameObject);

                            break;
                        case GameObjectType.Tileset:
                            context.Tilesets.Remove((TilesetBase)gameObject);

                            break;
                        case GameObjectType.Time:
                            break;
                        case GameObjectType.GuildVariable:
                            context.GuildVariables.Remove((GuildVariableBase)gameObject);

                            break;
                    }

                    if (gameObject.Type.GetLookup().Values.Contains(gameObject))
                    {
                        if (!gameObject.Type.GetLookup().Delete(gameObject))
                        {
                            throw new Exception();
                        }
                    }

                    context.ChangeTracker.DetectChanges();
                    context.Entry(gameObject).State = EntityState.Deleted;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public static void SaveGameObject(IDatabaseObject gameObject)
        {
            try
            {
                using (var context = CreateGameContext(readOnly: false))
                {

                    switch (gameObject.Type)
                    {
                        case GameObjectType.Animation:
                            context.Animations.Update((AnimationBase)gameObject);

                            break;
                        case GameObjectType.Class:
                            context.Classes.Update((ClassBase)gameObject);

                            break;
                        case GameObjectType.Item:
                            context.Items.Update((ItemBase)gameObject);

                            break;
                        case GameObjectType.Npc:
                            context.Npcs.Update((NpcBase)gameObject);

                            break;
                        case GameObjectType.Projectile:
                            context.Projectiles.Update((ProjectileBase)gameObject);

                            break;
                        case GameObjectType.Quest:

                            if (((QuestBase)gameObject).StartEvent != null)
                            {
                                context.Events.Update(((QuestBase)gameObject).StartEvent);
                            }

                            if (((QuestBase)gameObject).EndEvent != null)
                            {
                                context.Events.Update(((QuestBase)gameObject).EndEvent);
                            }

                            foreach (var tsk in ((QuestBase)gameObject).Tasks)
                            {
                                if (tsk.CompletionEvent != null)
                                {
                                    context.Events.Update(tsk.CompletionEvent);
                                }
                            }

                            context.Quests.Update((QuestBase)gameObject);

                            break;
                        case GameObjectType.Resource:
                            context.Resources.Update((ResourceBase)gameObject);

                            break;
                        case GameObjectType.Shop:
                            context.Shops.Update((ShopBase)gameObject);

                            break;
                        case GameObjectType.Spell:
                            context.Spells.Update((SpellBase)gameObject);

                            break;
                        case GameObjectType.CraftTables:
                            context.CraftingTables.Update((CraftingTableBase)gameObject);

                            break;
                        case GameObjectType.Crafts:
                            context.Crafts.Update((CraftBase)gameObject);

                            break;
                        case GameObjectType.Map:
                            context.Maps.Update((MapInstance)gameObject);

                            break;
                        case GameObjectType.Event:
                            context.Events.Update((EventBase)gameObject);

                            break;
                        case GameObjectType.PlayerVariable:
                            context.PlayerVariables.Update((PlayerVariableBase)gameObject);

                            break;
                        case GameObjectType.ServerVariable:
                            context.ServerVariables.Update((ServerVariableBase)gameObject);

                            break;
                        case GameObjectType.Tileset:
                            context.Tilesets.Update((TilesetBase)gameObject);

                            break;
                        case GameObjectType.Time:
                            break;
                        case GameObjectType.GuildVariable:
                            context.GuildVariables.Update((GuildVariableBase)gameObject);

                            break;
                    }

                    context.ChangeTracker.DetectChanges();
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        //Post Loading Functions
        private static void OnMapsLoaded()
        {
            if (MapBase.Lookup.Count == 0)
            {
                Console.WriteLine(Strings.Database.nomaps);
                AddGameObject(GameObjectType.Map);
            }

            GenerateMapGrids();
            LoadMapFolders();
            CheckAllMapConnections();

            foreach (var map in MapInstance.Lookup)
            {
                ((MapInstance) map.Value).Initialize();
            }
        }

        private static void OnClassesLoaded()
        {
            if (ClassBase.Lookup.Count == 0)
            {
                Console.WriteLine(Strings.Database.noclasses);
                var cls = (ClassBase) AddGameObject(GameObjectType.Class);
                cls.Name = Strings.Database.Default;
                var defaultMale = new ClassSprite()
                {
                    Sprite = "Base_Male.png",
                    Gender = Gender.Male
                };

                var defaultFemale = new ClassSprite()
                {
                    Sprite = "Base_Female.png",
                    Gender = Gender.Female
                };

                cls.Sprites.Add(defaultMale);
                cls.Sprites.Add(defaultFemale);
                for (var i = 0; i < (int) Vitals.VitalCount; i++)
                {
                    cls.BaseVital[i] = 20;
                }

                for (var i = 0; i < (int) Stats.StatCount; i++)
                {
                    cls.BaseStat[i] = 20;
                }
                SaveGameObject(cls);
            }
        }

        public static void CachePlayerVariableEventTextLookups()
        {
            var lookup = new Dictionary<string, PlayerVariableBase>();
            var addedIds = new HashSet<string>();
            foreach (PlayerVariableBase variable in PlayerVariableBase.Lookup.Values)
            {
                if (!string.IsNullOrWhiteSpace(variable.TextId) && !addedIds.Contains(variable.TextId))
                {
                    lookup.Add(Strings.Events.playervar + "{" + variable.TextId + "}", variable);
                    lookup.Add(Strings.Events.playerswitch + "{" + variable.TextId + "}", variable);
                    addedIds.Add(variable.TextId);
                }
            }
            PlayerVariableEventTextLookup = lookup;
        }

        public static void CacheServerVariableEventTextLookups()
        {
            var lookup = new Dictionary<string, ServerVariableBase>();
            var addedIds = new HashSet<string>();
            foreach (ServerVariableBase variable in ServerVariableBase.Lookup.Values)
            {
                if (!string.IsNullOrWhiteSpace(variable.TextId) && !addedIds.Contains(variable.TextId))
                {
                    lookup.Add(Strings.Events.globalvar + "{" + variable.TextId + "}", variable);
                    lookup.Add(Strings.Events.globalswitch + "{" + variable.TextId + "}", variable);
                    addedIds.Add(variable.TextId);
                }
            }
            ServerVariableEventTextLookup = lookup;
        }

        public static void CacheGuildVariableEventTextLookups()
        {
            var lookup = new Dictionary<string, GuildVariableBase>();
            var addedIds = new HashSet<string>();
            foreach (GuildVariableBase variable in GuildVariableBase.Lookup.Values)
            {
                if (!string.IsNullOrWhiteSpace(variable.TextId) && !addedIds.Contains(variable.TextId))
                {
                    lookup.Add(Strings.Events.guildvar + "{" + variable.TextId + "}", variable);
                    addedIds.Add(variable.TextId);
                }
            }
            GuildVariableEventTextLookup = lookup;
        }

        //Extra Map Helper Functions
        public static void CheckAllMapConnections()
        {
            var changed = false;
            foreach (MapInstance map in MapInstance.Lookup.Values)
            {
                CheckMapConnections(map, MapInstance.Lookup);
            }
        }

        public static bool CheckMapConnections(MapInstance map, DatabaseObjectLookup maps)
        {
            var updated = false;
            if (!maps.Keys.Contains(map.Up) && map.Up != Guid.Empty)
            {
                map.Up = Guid.Empty;
                updated = true;
            }

            if (!maps.Keys.Contains(map.Down) && map.Down != Guid.Empty)
            {
                map.Down = Guid.Empty;
                updated = true;
            }

            if (!maps.Keys.Contains(map.Left) && map.Left != Guid.Empty)
            {
                map.Left = Guid.Empty;
                updated = true;
            }

            if (!maps.Keys.Contains(map.Right) && map.Right != Guid.Empty)
            {
                map.Right = Guid.Empty;
                updated = true;
            }

            if (updated)
            {
                SaveGameObject(map);
                PacketSender.SendMapToEditors(map.Id);
                return true;
            }

            return false;
        }

        public static void GenerateMapGrids()
        {
            lock (mapGrids)
            {
                mapGrids.Clear();
                foreach (var map in MapInstance.Lookup.Values)
                {
                    if (mapGrids.Count == 0)
                    {
                        mapGrids.Add(new MapGrid(map.Id, 0));
                    }
                    else
                    {
                        for (var y = 0; y < mapGrids.Count; y++)
                        {
                            if (!mapGrids[y].HasMap(map.Id))
                            {
                                if (y != mapGrids.Count - 1)
                                {
                                    continue;
                                }

                                mapGrids.Add(new MapGrid(map.Id, mapGrids.Count));

                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }

                foreach (MapInstance map in MapInstance.Lookup.Values)
                {
                    lock (map.GetMapLock())
                    {
                        var myGrid = map.MapGrid;
                        var surroundingMapIds = new List<Guid>();
                        var surroundingMaps = new List<MapInstance>();
                        for (var x = map.MapGridX - 1; x <= map.MapGridX + 1; x++)
                        {
                            for (var y = map.MapGridY - 1; y <= map.MapGridY + 1; y++)
                            {
                                if (x == map.MapGridX && y == map.MapGridY)
                                {
                                    continue;
                                }

                                if (x >= mapGrids[myGrid].XMin &&
                                    x < mapGrids[myGrid].XMax &&
                                    y >= mapGrids[myGrid].YMin &&
                                    y < mapGrids[myGrid].YMax &&
                                    mapGrids[myGrid].MyGrid[x, y] != Guid.Empty)
                                {
                                    
                                    surroundingMapIds.Add(mapGrids[myGrid].MyGrid[x, y]);
                                    surroundingMaps.Add(MapInstance.Get(mapGrids[myGrid].MyGrid[x, y]));
                                }
                            }
                        }
                        map.SurroundingMapIds = surroundingMapIds.ToArray();
                        map.SurroundingMaps = surroundingMaps.ToArray();
                    }
                }

                for (var i = 0; i < mapGrids.Count; i++)
                {
                    PacketSender.SendMapGridToAll(i);
                }
            }
        }

        public static MapGrid GetGrid(int index)
        {
            lock (mapGrids)
            {
                return mapGrids[index];
            }
        }

        public static bool GridsContain(Guid id)
        {
            lock (mapGrids)
            {
                return mapGrids.Any(g => g.HasMap(id));
            }
        }

        //Map Folders
        private static void LoadMapFolders()
        {
            try
            {
                using (var context = CreateGameContext(readOnly: false))
                {
                    var mapFolders = context.MapFolders.FirstOrDefault();
                    if (mapFolders == null)
                    {
                        context.MapFolders.Add(MapList.List);
                        context.ChangeTracker.DetectChanges();
                        context.SaveChanges();
                    }
                    else
                    {
                        MapList.List = mapFolders;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            foreach (var map in MapBase.Lookup)
            {
                if (MapList.List.FindMap(map.Value.Id) == null)
                {
                    MapList.List.AddMap(map.Value.Id, map.Value.TimeCreated, MapBase.Lookup);
                }
            }

            MapList.List.PostLoad(MapBase.Lookup, true, true);
            PacketSender.SendMapListToAll();
        }

        public static void SaveMapList()
        {
            try
            {
                using (var context = CreateGameContext(readOnly: false))
                {
                    context.MapFolders.Update(MapList.List);
                    context.ChangeTracker.DetectChanges();
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        //Time
        private static void LoadTime()
        {
            try
            {
                using (var context = CreateGameContext(readOnly: false))
                {
                    var time = context.Time.FirstOrDefault();
                    if (time == null)
                    {
                        context.Time.Add(TimeBase.GetTimeBase());
                        context.ChangeTracker.DetectChanges();
                        context.SaveChanges();
                    }
                    else
                    {
                        TimeBase.SetStaticTime(time);
                    }
                }
                Time.Init();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public static void SaveTime()
        {
            try
            {
                using (var context = CreateGameContext(readOnly: false))
                {
                    context.Time.Update(TimeBase.GetTimeBase());
                    context.ChangeTracker.DetectChanges();
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public static void SaveUpdatedServerVariables()
        {
            if (UpdatedServerVariables.Count > 0)
            {
                using (var context = CreateGameContext(readOnly: false))
                {
                    foreach (var variable in UpdatedServerVariables)
                    {
                        var serverVar = variable.Value;
                        if (serverVar != null)
                        {
                            context.ServerVariables.Update(variable.Value);
                        }
                        UpdatedServerVariables.TryRemove(variable.Key, out ServerVariableBase obj);
                    }
                    context.SaveChanges();
                }
            }
        }

        //Migration Code
        public static void Migrate(DatabaseOptions orig, DatabaseOptions.DatabaseType convType)
        {
            var gameDb = orig == Options.GameDb;
            PlayerContext newPlayerContext = null;
            GameContext newGameContext = null;
            var newOpts = new DatabaseOptions
            {
                Type = DatabaseOptions.DatabaseType.SQLite
            };

            //MySql Creds
            string host, user, pass, database;
            ushort port;
            var dbConnected = false;

            switch (convType)
            {
                case DatabaseOptions.DatabaseType.MySQL:
                    while (!dbConnected)
                    {
                        Console.WriteLine(Strings.Migration.entermysqlinfo);
                        Console.Write(Strings.Migration.mysqlhost);
                        host = Console.ReadLine()?.Trim() ?? "localhost";
                        Console.Write(Strings.Migration.mysqlport);
                        var portinput = Console.ReadLine()?.Trim();
                        if (string.IsNullOrWhiteSpace(portinput))
                        {
                            portinput = "3306";
                        }

                        port = ushort.Parse(portinput);
                        Console.Write(Strings.Migration.mysqldatabase);
                        database = Console.ReadLine()?.Trim();
                        Console.Write(Strings.Migration.mysqluser);
                        user = Console.ReadLine().Trim();
                        Console.Write(Strings.Migration.mysqlpass);
                        pass = GetPassword();

                        Console.WriteLine();
                        Console.WriteLine(Strings.Migration.mysqlconnecting);
                        var connectionStringBuilder = CreateConnectionStringBuilder(
                            new DatabaseOptions
                            {
                                Type = DatabaseOptions.DatabaseType.MySQL,
                                Server = host,
                                Port = port,
                                Database = database,
                                Username = user,
                                Password = pass
                            }, null
                        );

                        try
                        {
                            if (gameDb)
                            {
                                newGameContext = new GameContext(
                                    connectionStringBuilder, DatabaseOptions.DatabaseType.MySQL
                                );

                                if (newGameContext.IsEmpty())
                                {
                                    newGameContext.Database.EnsureDeleted();
                                    newGameContext.Database.Migrate();
                                }
                                else
                                {
                                    Console.WriteLine(Strings.Migration.mysqlnotempty);

                                    return;
                                }
                            }
                            else
                            {
                                newPlayerContext = new PlayerContext(
                                    connectionStringBuilder, DatabaseOptions.DatabaseType.MySQL
                                );

                                if (newPlayerContext.IsEmpty())
                                {
                                    newPlayerContext.Database.EnsureDeleted();
                                    newPlayerContext.Database.Migrate();
                                }
                                else
                                {
                                    Console.WriteLine(Strings.Migration.mysqlnotempty);

                                    return;
                                }
                            }

                            newOpts.Type = DatabaseOptions.DatabaseType.mysql;
                            newOpts.Server = host;
                            newOpts.Port = port;
                            newOpts.Database = database;
                            newOpts.Username = user;
                            newOpts.Password = pass;

                            dbConnected = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(Strings.Migration.mysqlconnectionerror.ToString(ex));
                            Console.WriteLine();
                            Console.WriteLine(Strings.Migration.mysqltryagain);
                            var input = Console.ReadLine();
                            var key = input.Length > 0 ? input[0] : ' ';
                            Console.WriteLine();
                            if (!string.Equals(
                                Strings.Migration.tryagaincharacter, key.ToString(), StringComparison.Ordinal
                            ))
                            {
                                Console.WriteLine(Strings.Migration.migrationcancelled);

                                return;
                            }
                        }
                    }

                    break;

                case DatabaseOptions.DatabaseType.SQLite:
                    //If the file exists make sure it is safe to delete
                    var dbExists = gameDb && File.Exists(GameDbFilename) || !gameDb && File.Exists(PlayersDbFilename);
                    if (dbExists)
                    {
                        Console.WriteLine();
                        var filename = gameDb ? GameDbFilename : PlayersDbFilename;
                        Console.WriteLine(Strings.Migration.sqlitealreadyexists.ToString(filename));
                        var input = Console.ReadLine();
                        var key = input.Length > 0 ? input[0] : ' ';
                        Console.WriteLine();
                        if (key.ToString() != Strings.Migration.overwritecharacter)
                        {
                            Console.WriteLine(Strings.Migration.migrationcancelled);

                            return;
                        }
                    }

                    if (gameDb)
                    {
                        newGameContext = new GameContext(
                            CreateConnectionStringBuilder(
                                new DatabaseOptions
                                {
                                    Type = DatabaseOptions.DatabaseType.SQLite
                                }, GameDbFilename
                            ), DatabaseOptions.DatabaseType.SQLite
                        );

                        newGameContext.Database.EnsureDeleted();
                        newGameContext.Database.Migrate();
                    }
                    else
                    {
                        newPlayerContext = new PlayerContext(
                            CreateConnectionStringBuilder(
                                new DatabaseOptions
                                {
                                    Type = DatabaseOptions.DatabaseType.SQLite
                                }, PlayersDbFilename
                            ), DatabaseOptions.DatabaseType.SQLite
                        );

                        newPlayerContext.Database.EnsureDeleted();
                        newPlayerContext.Database.Migrate();
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(convType));
            }

            //Shut down server, start migration.
            Console.WriteLine(Strings.Migration.stoppingserver);

            //This variable will end the server loop and save any pending changes
            ServerContext.Instance.RequestShutdown();

            while (ServerContext.Instance.IsRunning)
            {
                System.Threading.Thread.Sleep(100);
            }


            Console.WriteLine(Strings.Migration.startingmigration);
            if (gameDb && newGameContext != null)
            {
                using (var context = CreateGameContext(readOnly: false))
                {
                    MigrateDbSet(context.Animations, newGameContext.Animations);
                    MigrateDbSet(context.Classes, newGameContext.Classes);
                    MigrateDbSet(context.CraftingTables, newGameContext.CraftingTables);
                    MigrateDbSet(context.Crafts, newGameContext.Crafts);
                    MigrateDbSet(context.Events, newGameContext.Events);
                    MigrateDbSet(context.Items, newGameContext.Items);
                    MigrateDbSet(context.MapFolders, newGameContext.MapFolders);
                    MigrateDbSet(context.Maps, newGameContext.Maps);
                    MigrateDbSet(context.Npcs, newGameContext.Npcs);
                    MigrateDbSet(context.Projectiles, newGameContext.Projectiles);
                    MigrateDbSet(context.Quests, newGameContext.Quests);
                    MigrateDbSet(context.Resources, newGameContext.Resources);
                    MigrateDbSet(context.Shops, newGameContext.Shops);
                    MigrateDbSet(context.Spells, newGameContext.Spells);
                    MigrateDbSet(context.ServerVariables, newGameContext.ServerVariables);
                    MigrateDbSet(context.PlayerVariables, newGameContext.PlayerVariables);
                    MigrateDbSet(context.Tilesets, newGameContext.Tilesets);
                    MigrateDbSet(context.Time, newGameContext.Time);
                    newGameContext.ChangeTracker.DetectChanges();
                    newGameContext.SaveChanges();
                    newGameContext.Dispose();
                    newGameContext = null;
                }
                Options.GameDb = newOpts;
                Options.SaveToDisk();
            }
            else if (!gameDb && newPlayerContext != null)
            {
                using (var context = CreatePlayerContext(readOnly: false))
                {
                    MigrateDbSet(context.Users, newPlayerContext.Users);
                    MigrateDbSet(context.Players, newPlayerContext.Players);
                    MigrateDbSet(context.Player_Friends, newPlayerContext.Player_Friends);
                    MigrateDbSet(context.Player_Spells, newPlayerContext.Player_Spells);
                    MigrateDbSet(context.Player_Variables, newPlayerContext.Player_Variables);
                    MigrateDbSet(context.Player_Hotbar, newPlayerContext.Player_Hotbar);
                    MigrateDbSet(context.Player_Quests, newPlayerContext.Player_Quests);
                    MigrateDbSet(context.Bags, newPlayerContext.Bags);
                    MigrateDbSet(context.Player_Items, newPlayerContext.Player_Items);
                    MigrateDbSet(context.Player_Bank, newPlayerContext.Player_Bank);
                    MigrateDbSet(context.Bag_Items, newPlayerContext.Bag_Items);
                    MigrateDbSet(context.Mutes, newPlayerContext.Mutes);
                    MigrateDbSet(context.Bans, newPlayerContext.Bans);
                    newPlayerContext.ChangeTracker.DetectChanges();
                    newPlayerContext.SaveChanges();
                    newPlayerContext.Dispose();
                    newPlayerContext = null;
                }
                Options.PlayerDb = newOpts;
                Options.SaveToDisk();

            }

            Console.WriteLine(Strings.Migration.migrationcomplete);
            Bootstrapper.Context.ConsoleService.Wait(true);
            Environment.Exit(0);
        }

        private static void MigrateDbSet<T>(DbSet<T> oldDbSet, DbSet<T> newDbSet) where T : class
        {
            foreach (var itm in oldDbSet)
            {
                newDbSet.Add(itm);
            }
        }

        //Code taken from Stackoverflow on 9/20/2018
        //Answer by Dai and Damian Leszczyński - Vash
        //https://stackoverflow.com/questions/3404421/password-masking-console-application
        public static string GetPassword()
        {
            var pwd = "";
            while (true)
            {
                var i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 1)
                    {
                        pwd = pwd.Remove(pwd.Length - 2, 1);
                        Console.Write("\b \b");
                    }
                }
                else if (i.KeyChar != '\u0000'
                ) // KeyChar == '\u0000' if the key pressed does not correspond to a printable character, e.g. F1, Pause-Break, etc
                {
                    pwd = pwd + i.KeyChar;
                    Console.Write("*");
                }
            }

            return pwd;
        }

        private static readonly ContextProvider ContextProvider = new ContextProvider();

        public static ILoggingContext LoggingContext => ContextProvider.Access<ILoggingContext, LoggingContextInterface>();

    }

}
