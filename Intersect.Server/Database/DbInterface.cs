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
using System.Threading;
using System.Threading.Tasks;

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

using JetBrains.Annotations;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using MySql.Data.MySqlClient;

namespace Intersect.Server.Database
{

    public static class DbInterface
    {

        //Entity Framework Contexts are NOT threadsafe.. and using multiple contexts simultaneously doesn't work well due to us keeping players/maps/etc in memory.
        //This class interfaces with a single playerdb and gamedb context and makes sure requests for information are handled in a safe manner.

        private const string GameDbFilename = "resources/gamedata.db";

        private const string PlayersDbFilename = "resources/playerdata.db";

        [NotNull] private static readonly ConcurrentQueue<IdTrace> gameDbTraces = new ConcurrentQueue<IdTrace>();

        [NotNull] private static readonly ConcurrentQueue<IdTrace> playerDbTraces = new ConcurrentQueue<IdTrace>();

        private static Logger gameDbLogger;

        private static long gameDbSaveId = 0;

        private static long gameSavesWaiting = 0;

        private static List<MapGrid> mapGrids = new List<MapGrid>();

        private static object mGameDbLock = new object();

        private static object mPlayerDbLock = new object();

        private static Logger playerDbLogger;

        private static long playerDbSaveId = 0;

        private static long playerSavesWaiting = 0;

        private static Task sSavePlayerDbTask;

        private static PlayerContext sPlayerDb { get; set; }

        private static GameContext sGameDb { get; set; }

        public static long RegisteredPlayers
        {
            get
            {
                lock (mPlayerDbLock)
                {
                    return sPlayerDb.Players.Count();
                }
            }
        }

        public static void InitializeDbLoggers()
        {
            if (Options.GameDb.LogLevel > LogLevel.None)
            {
                gameDbLogger = new Logger(
                    new LogConfiguration
                    {
                        Tag = "GAMEDB",
                        Pretty = false,
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
                        Pretty = false,
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

        [NotNull]
        public static DbConnectionStringBuilder CreateConnectionStringBuilder(
            [NotNull] DatabaseOptions databaseOptions,
            [NotNull] string filename
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
        internal static bool InitDatabase([NotNull] ServerContext serverContext)
        {
            sGameDb = new GameContext(
                CreateConnectionStringBuilder(Options.GameDb ?? throw new InvalidOperationException(), GameDbFilename),
                Options.GameDb.Type, gameDbLogger, Options.GameDb.LogLevel
            );

            sPlayerDb = new PlayerContext(
                CreateConnectionStringBuilder(
                    Options.PlayerDb ?? throw new InvalidOperationException(), PlayersDbFilename
                ), Options.PlayerDb.Type, playerDbLogger, Options.PlayerDb.LogLevel
            );

            LoggingContext.Configure(
                DatabaseOptions.DatabaseType.SQLite, LoggingContext.DefaultConnectionStringBuilder
            );

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

            var gameMigrations = sGameDb.PendingMigrations;
            var showGameMigrationWarning = gameMigrations.Any() && !gameMigrations.Contains("20180905042857_Initial");
            var playerMigrations = sPlayerDb.PendingMigrations;
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

            sGameDb.Database.Migrate();
            var remainingGameMigrations = sGameDb.PendingMigrations;
            var processedGameMigrations = new List<string>(gameMigrations);
            foreach (var itm in remainingGameMigrations)
            {
                processedGameMigrations.Remove(itm);
            }

            sGameDb.MigrationsProcessed(processedGameMigrations.ToArray());

            sPlayerDb.Database.Migrate();
            var remainingPlayerMigrations = sPlayerDb.PendingMigrations;
            var processedPlayerMigrations = new List<string>(playerMigrations);
            foreach (var itm in remainingPlayerMigrations)
            {
                processedPlayerMigrations.Remove(itm);
            }

            sPlayerDb.MigrationsProcessed(processedPlayerMigrations.ToArray());
#if DEBUG
            if (ServerContext.Instance.RestApi.Configuration.SeedMode)
            {
                sPlayerDb.Seed();
            }
#endif

            if (serverContext.RestApi.Configuration.RequestLogging)
            {
                using (var loggingContext = LoggingContext.Create())
                {
                    loggingContext.Database?.Migrate();
                }
            }

            LoadAllGameObjects();
            LoadTime();
            OnClassesLoaded();
            OnMapsLoaded();
            SaveGameDatabase();

            return true;
        }

        public static Client GetPlayerClient([NotNull] string username)
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

        public static void SetPlayerPower([NotNull] string username, UserRights power)
        {
            var user = GetUser(username);
            if (user != null)
            {
                user.Power = power;
                SavePlayerDatabaseAsync();
            }
            else
            {
                Console.WriteLine(Strings.Account.doesnotexist);
            }
        }

        public static bool SetPlayerPower([CanBeNull] User user, UserRights power)
        {
            if (user != null)
            {
                user.Power = power;
                SavePlayerDatabaseAsync();

                return true;
            }
            else
            {
                Console.WriteLine(Strings.Account.doesnotexist);

                return false;
            }
        }

        //User Info
        public static bool AccountExists([NotNull] string accountname)
        {
            lock (mPlayerDbLock)
            {
                return sPlayerDb.Users.Any(
                    p => string.Equals(p.Name.Trim(), accountname.Trim(), StringComparison.CurrentCultureIgnoreCase)
                );
            }
        }

        public static string UsernameFromEmail([NotNull] string email)
        {
            lock (mPlayerDbLock)
            {
                return sPlayerDb.Users.FirstOrDefault(
                        p => string.Equals(p.Email.Trim(), email.Trim(), StringComparison.CurrentCultureIgnoreCase)
                    )
                    ?.Name;
            }
        }

        public static User GetUser([NotNull] string username)
        {
            lock (mPlayerDbLock)
            {
                return User.Find(username.Trim(), sPlayerDb)?.Load();
            }
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

        public static bool EmailInUse([NotNull] string email)
        {
            lock (mPlayerDbLock)
            {
                return sPlayerDb.Users.Any(
                    p => string.Equals(p.Email, email, StringComparison.CurrentCultureIgnoreCase)
                );
            }
        }

        public static bool CharacterNameInUse([NotNull] string name)
        {
            lock (mPlayerDbLock)
            {
                return sPlayerDb.Players.Any(
                    p => string.Equals(p.Name, name, StringComparison.CurrentCultureIgnoreCase)
                );
            }
        }

        public static Guid? GetCharacterId([NotNull] string name)
        {
            lock (mPlayerDbLock)
            {
                return sPlayerDb.Players.Where(
                        p => string.Equals(p.Name, name, StringComparison.CurrentCultureIgnoreCase)
                    )
                    ?.First()
                    ?.Id;
            }
        }

        public static Player GetPlayer(Guid playerId)
        {
            lock (mPlayerDbLock)
            {
                return Player.Load(playerId, sPlayerDb);
            }
        }

        public static Player GetPlayer([NotNull] string playerName)
        {
            lock (mPlayerDbLock)
            {
                return Player.Load(playerName, sPlayerDb);
            }
        }

        public static void CreateAccount(
            Client client,
            [NotNull] string username,
            [NotNull] string password,
            [NotNull] string email
        )
        {
            var sha = new SHA256Managed();

            //Generate a Salt
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[20];
            rng.GetBytes(buff);
            var salt = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(Convert.ToBase64String(buff))))
                .Replace("-", "");

            var rights = UserRights.None;
            if (RegisteredPlayers == 0)
            {
                rights = UserRights.Admin;
            }

            var user = new User
            {
                Name = username,
                Email = email,
                Salt = salt,
                Password = User.SaltPasswordHash(password, salt),
                Power = rights,
            };

            lock (mPlayerDbLock)
            {
                sPlayerDb.Users.Add(user);
            }

            client?.SetUser(user);
            SavePlayerDatabaseAsync();
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
            SavePlayerDatabaseAsync();
        }

        public static bool CheckPassword([NotNull] string username, [NotNull] string password)
        {
            lock (mPlayerDbLock)
            {
                // ReSharper disable once SpecifyStringComparison
                var user = sPlayerDb.Users.Where(p => p.Name.ToLower() == username.ToLower())
                    .Select(p => new {p.Password, p.Salt})
                    .FirstOrDefault();

                return user != null && User.SaltPasswordHash(password, user.Salt) == user.Password;
            }
        }

        public static UserRights CheckAccess([NotNull] string username)
        {
            lock (mPlayerDbLock)
            {
                // ReSharper disable once SpecifyStringComparison
                var user = User.Find(username);

                if (user != null)
                {
                    return user.Power;
                }

                return UserRights.None;
            }
        }

        public static bool LoadUser([NotNull] Client client, [NotNull] string username)
        {
            var user = GetUser(username);
            if (user != null)
            {
                client.SetUser(user);

                return true;
            }

            return false;
        }

        public static void AddCharacter(User usr, Player chr)
        {
            lock (mPlayerDbLock)
            {
                usr.Players.Add(chr);
                sPlayerDb.Add(chr);
            }

            SavePlayerDatabaseAsync();
        }

        public static void DeleteCharacter(User usr, Player chr)
        {
            lock (mPlayerDbLock)
            {
                usr.Players.Remove(chr);
                sPlayerDb.Remove<Player>(chr);
            }

            SavePlayerDatabaseAsync();
        }

        public static Bag GetBag(Guid bagId)
        {
            if (bagId == Guid.Empty)
            {
                return null;
            }

            lock (mPlayerDbLock)
            {
                var bag = Bag.GetBag(sPlayerDb, bagId);
                if (bag == null)
                {
                    return default;
                }

                bag.ValidateSlots();
                return bag;
            }
        }

        public static bool BagEmpty([NotNull] Bag bag)
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static void LoadGameObjects(GameObjectType gameObjectType)
        {
            ClearGameObjects(gameObjectType);
            lock (mGameDbLock)
            {
                switch (gameObjectType)
                {
                    case GameObjectType.Animation:
                        foreach (var anim in sGameDb.Animations)
                        {
                            AnimationBase.Lookup.Set(anim.Id, anim);
                        }

                        break;
                    case GameObjectType.Class:
                        foreach (var cls in sGameDb.Classes)
                        {
                            ClassBase.Lookup.Set(cls.Id, cls);
                        }

                        break;
                    case GameObjectType.Item:
                        foreach (var itm in sGameDb.Items)
                        {
                            ItemBase.Lookup.Set(itm.Id, itm);
                        }

                        break;
                    case GameObjectType.Npc:
                        foreach (var npc in sGameDb.Npcs)
                        {
                            NpcBase.Lookup.Set(npc.Id, npc);
                        }

                        break;
                    case GameObjectType.Projectile:
                        foreach (var proj in sGameDb.Projectiles)
                        {
                            ProjectileBase.Lookup.Set(proj.Id, proj);
                        }

                        break;
                    case GameObjectType.Quest:
                        foreach (var qst in sGameDb.Quests)
                        {
                            QuestBase.Lookup.Set(qst.Id, qst);
                        }

                        break;
                    case GameObjectType.Resource:
                        foreach (var res in sGameDb.Resources)
                        {
                            ResourceBase.Lookup.Set(res.Id, res);
                        }

                        break;
                    case GameObjectType.Shop:
                        foreach (var shp in sGameDb.Shops)
                        {
                            ShopBase.Lookup.Set(shp.Id, shp);
                        }

                        break;
                    case GameObjectType.Spell:
                        foreach (var spl in sGameDb.Spells)
                        {
                            SpellBase.Lookup.Set(spl.Id, spl);
                        }

                        break;
                    case GameObjectType.CraftTables:
                        foreach (var craft in sGameDb.CraftingTables)
                        {
                            CraftingTableBase.Lookup.Set(craft.Id, craft);
                        }

                        break;
                    case GameObjectType.Crafts:
                        foreach (var craft in sGameDb.Crafts)
                        {
                            CraftBase.Lookup.Set(craft.Id, craft);
                        }

                        break;
                    case GameObjectType.Map:
                        var maps = sGameDb.Maps.ToArray();
                        foreach (var map in maps)
                        {
                            MapInstance.Lookup.Set(map.Id, map);
                        }

                        break;
                    case GameObjectType.Event:
                        foreach (var evt in sGameDb.Events)
                        {
                            EventBase.Lookup.Set(evt.Id, evt);
                        }

                        break;
                    case GameObjectType.PlayerVariable:
                        foreach (var psw in sGameDb.PlayerVariables)
                        {
                            PlayerVariableBase.Lookup.Set(psw.Id, psw);
                        }

                        break;
                    case GameObjectType.ServerVariable:
                        foreach (var psw in sGameDb.ServerVariables)
                        {
                            ServerVariableBase.Lookup.Set(psw.Id, psw);
                        }

                        break;
                    case GameObjectType.Tileset:
                        foreach (var psw in sGameDb.Tilesets)
                        {
                            TilesetBase.Lookup.Set(psw.Id, psw);
                        }

                        break;
                    case GameObjectType.Time:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(gameObjectType), gameObjectType, null);
                }
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

                default:
                    throw new ArgumentOutOfRangeException(nameof(gameObjectType), gameObjectType, null);
            }

            return dbObj == null ? null : AddGameObject(gameObjectType, dbObj);
        }

        public static IDatabaseObject AddGameObject(GameObjectType gameObjectType, [NotNull] IDatabaseObject dbObj)
        {
            if (sGameDb == null)
            {
                throw new ArgumentNullException(nameof(sGameDb));
            }

            lock (mGameDbLock ?? throw new ArgumentNullException(nameof(mGameDbLock)))
            {
                switch (gameObjectType)
                {
                    case GameObjectType.Animation:
                        sGameDb.Animations.Add((AnimationBase) dbObj);
                        AnimationBase.Lookup.Set(dbObj.Id, dbObj);

                        break;

                    case GameObjectType.Class:
                        sGameDb.Classes.Add((ClassBase) dbObj);
                        ClassBase.Lookup.Set(dbObj.Id, dbObj);

                        break;

                    case GameObjectType.Item:
                        sGameDb.Items.Add((ItemBase) dbObj);
                        ItemBase.Lookup.Set(dbObj.Id, dbObj);

                        break;

                    case GameObjectType.Npc:
                        sGameDb.Npcs.Add((NpcBase) dbObj);
                        NpcBase.Lookup.Set(dbObj.Id, dbObj);

                        break;

                    case GameObjectType.Projectile:
                        sGameDb.Projectiles.Add((ProjectileBase) dbObj);
                        ProjectileBase.Lookup.Set(dbObj.Id, dbObj);

                        break;

                    case GameObjectType.Quest:
                        sGameDb.Quests.Add((QuestBase) dbObj);
                        QuestBase.Lookup.Set(dbObj.Id, dbObj);

                        break;

                    case GameObjectType.Resource:
                        sGameDb.Resources.Add((ResourceBase) dbObj);
                        ResourceBase.Lookup.Set(dbObj.Id, dbObj);

                        break;

                    case GameObjectType.Shop:
                        sGameDb.Shops.Add((ShopBase) dbObj);
                        ShopBase.Lookup.Set(dbObj.Id, dbObj);

                        break;

                    case GameObjectType.Spell:
                        sGameDb.Spells.Add((SpellBase) dbObj);
                        SpellBase.Lookup.Set(dbObj.Id, dbObj);

                        break;

                    case GameObjectType.CraftTables:
                        sGameDb.CraftingTables.Add((CraftingTableBase) dbObj);
                        CraftingTableBase.Lookup.Set(dbObj.Id, dbObj);

                        break;

                    case GameObjectType.Crafts:
                        sGameDb.Crafts.Add((CraftBase) dbObj);
                        CraftBase.Lookup.Set(dbObj.Id, dbObj);

                        break;

                    case GameObjectType.Map:
                        sGameDb.Maps.Add((MapInstance) dbObj);
                        MapInstance.Lookup.Set(dbObj.Id, dbObj);

                        break;

                    case GameObjectType.Event:
                        sGameDb.Events.Add((EventBase) dbObj);
                        EventBase.Lookup.Set(dbObj.Id, dbObj);

                        break;

                    case GameObjectType.PlayerVariable:
                        sGameDb.PlayerVariables.Add((PlayerVariableBase) dbObj);
                        PlayerVariableBase.Lookup.Set(dbObj.Id, dbObj);

                        break;

                    case GameObjectType.ServerVariable:
                        sGameDb.ServerVariables.Add((ServerVariableBase) dbObj);
                        ServerVariableBase.Lookup.Set(dbObj.Id, dbObj);

                        break;

                    case GameObjectType.Tileset:
                        sGameDb.Tilesets.Add((TilesetBase) dbObj);
                        TilesetBase.Lookup.Set(dbObj.Id, dbObj);

                        break;

                    case GameObjectType.Time:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(gameObjectType), gameObjectType, null);
                }
            }

            if (ServerContext.Instance.IsStarted)
            {
                SaveGameDatabase();
            }

            return dbObj;
        }

        public static void DeleteGameObject(IDatabaseObject gameObject)
        {
            lock (mGameDbLock)
            {
                switch (gameObject.Type)
                {
                    case GameObjectType.Animation:
                        sGameDb.Animations.Remove((AnimationBase) gameObject);

                        break;
                    case GameObjectType.Class:
                        sGameDb.Classes.Remove((ClassBase) gameObject);

                        break;
                    case GameObjectType.Item:
                        sGameDb.Items.Remove((ItemBase) gameObject);

                        break;
                    case GameObjectType.Npc:
                        sGameDb.Npcs.Remove((NpcBase) gameObject);

                        break;
                    case GameObjectType.Projectile:
                        sGameDb.Projectiles.Remove((ProjectileBase) gameObject);

                        break;
                    case GameObjectType.Quest:

                        if (((QuestBase) gameObject).StartEvent != null)
                        {
                            sGameDb.Events.Remove(((QuestBase) gameObject).StartEvent);
                            EventBase.Lookup.Delete(((QuestBase) gameObject).StartEvent);
                        }

                        if (((QuestBase) gameObject).EndEvent != null)
                        {
                            sGameDb.Events.Remove(((QuestBase) gameObject).EndEvent);
                            EventBase.Lookup.Delete(((QuestBase) gameObject).EndEvent);
                        }

                        foreach (var tsk in ((QuestBase) gameObject).Tasks)
                        {
                            if (tsk.CompletionEvent != null)
                            {
                                sGameDb.Events.Remove(tsk.CompletionEvent);
                                EventBase.Lookup.Delete(tsk.CompletionEvent);
                            }
                        }

                        sGameDb.Quests.Remove((QuestBase) gameObject);

                        break;
                    case GameObjectType.Resource:
                        sGameDb.Resources.Remove((ResourceBase) gameObject);

                        break;
                    case GameObjectType.Shop:
                        sGameDb.Shops.Remove((ShopBase) gameObject);

                        break;
                    case GameObjectType.Spell:
                        sGameDb.Spells.Remove((SpellBase) gameObject);

                        break;
                    case GameObjectType.CraftTables:
                        sGameDb.CraftingTables.Remove((CraftingTableBase) gameObject);

                        break;
                    case GameObjectType.Crafts:
                        sGameDb.Crafts.Remove((CraftBase) gameObject);

                        break;
                    case GameObjectType.Map:
                        sGameDb.Maps.Remove((MapInstance) gameObject);
                        MapInstance.Lookup.Delete(gameObject);

                        break;
                    case GameObjectType.Event:
                        sGameDb.Events.Remove((EventBase) gameObject);

                        break;
                    case GameObjectType.PlayerVariable:
                        sGameDb.PlayerVariables.Remove((PlayerVariableBase) gameObject);

                        break;
                    case GameObjectType.ServerVariable:
                        sGameDb.ServerVariables.Remove((ServerVariableBase) gameObject);

                        break;
                    case GameObjectType.Tileset:
                        sGameDb.Tilesets.Remove((TilesetBase) gameObject);

                        break;
                    case GameObjectType.Time:
                        break;
                }

                if (gameObject.Type.GetLookup().Values.Contains(gameObject))
                {
                    if (!gameObject.Type.GetLookup().Delete(gameObject))
                    {
                        throw new Exception();
                    }
                }
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
            }
        }

        //Extra Map Helper Functions
        public static void CheckAllMapConnections()
        {
            foreach (MapBase map in MapInstance.Lookup.Values)
            {
                CheckMapConnections(map, MapInstance.Lookup);
            }
        }

        public static void CheckMapConnections(MapBase map, DatabaseObjectLookup maps)
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
                SaveGameDatabase();
                PacketSender.SendMapToEditors(map.Id);
            }
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
                        map.SurroundingMaps.Clear();
                        var myGrid = map.MapGrid;
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
                                    map.SurroundingMaps.Add(mapGrids[myGrid].MyGrid[x, y]);
                                }
                            }
                        }
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
            lock (mGameDbLock)
            {
                var mapFolders = sGameDb.MapFolders.FirstOrDefault();
                if (mapFolders == null)
                {
                    sGameDb.MapFolders.Add(MapList.List);
                }
                else
                {
                    MapList.List = mapFolders;
                }
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

        //Time
        private static void LoadTime()
        {
            lock (mGameDbLock)
            {
                var time = sGameDb.Time.FirstOrDefault();
                if (time == null)
                {
                    sGameDb.Time.Add(TimeBase.GetTimeBase());
                }
                else
                {
                    TimeBase.SetStaticTime(time);
                }
            }

            Time.Init();
        }

        public static void SavePlayerDatabaseAsync()
        {
            if (sSavePlayerDbTask == null ||
                !(sSavePlayerDbTask.IsCompleted == false ||
                  sSavePlayerDbTask.Status == TaskStatus.Running ||
                  sSavePlayerDbTask.Status == TaskStatus.WaitingToRun ||
                  sSavePlayerDbTask.Status == TaskStatus.WaitingForActivation))
            {
                var trace = Environment.StackTrace;
                sSavePlayerDbTask = Task.Factory.StartNew(() => SavePlayerDatabase(trace));
            }
        }

        public static void SaveGameDatabase()
        {
            ++gameSavesWaiting;
            gameDbLogger?.Debug($"{gameSavesWaiting} saves queued.");

            if (gameDbTraces.Count > 1)
            {
                var builder = new StringBuilder();

                while (!gameDbTraces.IsEmpty)
                {
                    builder.AppendLine(
                        gameDbTraces.TryDequeue(out var dequeueTrace)
                            ? dequeueTrace.ToString()
                            : $"Error dequeuing trace ({gameDbTraces.Count} traces)."
                    );

                    builder.AppendLine();
                }

                gameDbLogger?.Debug($"{gameSavesWaiting} saves queued, traces:\n{builder}");
            }

            gameDbTraces.Enqueue(new IdTrace {Id = gameDbSaveId++, Trace = Environment.StackTrace});

            switch (gameSavesWaiting)
            {
                case var _ when gameSavesWaiting > 2:
                    Log.Debug($"Possible Game DB deadlock: {gameSavesWaiting} saves queued!");

                    break;

                case var _ when gameSavesWaiting > 8:
                    Log.Warn($"Probable Game DB deadlock: {gameSavesWaiting} saves queued!");

                    break;

                case var _ when gameSavesWaiting > 16:
                    Log.Error($"Detected Game DB deadlock: {gameSavesWaiting} saves queued!");

                    break;
            }

            lock (mGameDbLock)
            {
                var saved = false;
                var failures = 0;
                while (!saved)
                {
                    try
                    {
                        var elapsedMs = SaveDb(sGameDb);
                        saved = true;
                        gameDbLogger?.Debug($"Save took {elapsedMs}ms, {--gameSavesWaiting} saves queued.");
                        gameDbTraces.TryDequeue(out _);
                    }
                    catch (Exception ex)
                    {
                        if (ex is InvalidOperationException)
                        {
                            //Collection was modified?
                            //Loop and try to save again!
                            failures++;
                            if (failures > 10)
                            {
                                Console.WriteLine(
                                    "Error Saving Game Database! Server will shutdown in order to prevent potential rollback scenarios!"
                                );

                                Task.Factory.StartNew(
                                    () => Bootstrapper.OnUnhandledException(
                                        Thread.CurrentThread.Name, new UnhandledExceptionEventArgs(ex, true)
                                    )
                                );

                                gameDbLogger?.Error(
                                    ex,
                                    "Error Saving Game Database! Server will shutdown in order to prevent potential rollback scenarios! [Failures: " +
                                    failures +
                                    "]"
                                );

                                break;
                            }

                            //This should be a warning but I want to actually see it working in a real environment without people needing to change their configs for a few builds
                            //TODO change to .Warn
                            gameDbLogger?.Error(
                                ex,
                                "Collection was modified? while trying to save game db, will loop and try to save again! [Failures: " +
                                failures +
                                "]"
                            );
                        }
                        else
                        {
                            Console.WriteLine(
                                "Error Saving Game Database! Server will shutdown in order to prevent potential rollback scenarios!"
                            );

                            Task.Factory.StartNew(
                                () => Bootstrapper.OnUnhandledException(
                                    Thread.CurrentThread.Name, new UnhandledExceptionEventArgs(ex, true)
                                )
                            );

                            gameDbLogger?.Error(
                                ex,
                                "Error Saving Game Database! Server will shutdown in order to prevent potential rollback scenarios!"
                            );

                            break;
                        }
                    }
                }
            }
        }

        public static void SavePlayerDatabase(string trace)
        {
            ++playerSavesWaiting;
            var currentTrace = new IdTrace {Id = playerDbSaveId++, Trace = trace};
            playerDbLogger?.Debug($"{currentTrace.Id:00000}: {playerSavesWaiting} saves queued.");

            if (playerDbTraces.Count > 1)
            {
                var builder = new StringBuilder();

                while (!playerDbTraces.IsEmpty)
                {
                    builder.AppendLine(
                        playerDbTraces.TryDequeue(out var dequeueTrace)
                            ? dequeueTrace.ToString()
                            : $"{currentTrace.Id:00000}: Error dequeuing trace ({playerDbTraces.Count} traces)."
                    );

                    builder.AppendLine();
                }

                playerDbLogger?.Debug(
                    $"{currentTrace.Id:00000}: {playerSavesWaiting} saves queued, traces:\n{builder}"
                );
            }

            playerDbTraces.Enqueue(currentTrace);

            switch (playerSavesWaiting)
            {
                case var _ when playerSavesWaiting > 1:
                    Log.Debug(
                        $"{currentTrace.Id:00000}: Possible Player DB deadlock: {playerSavesWaiting} saves queued!"
                    );

                    break;

                case var _ when playerSavesWaiting > 4:
                    Log.Warn(
                        $"{currentTrace.Id:00000}: Probable Player DB deadlock: {playerSavesWaiting} saves queued!"
                    );

                    break;

                case var _ when playerSavesWaiting > 8:
                    Log.Error(
                        $"{currentTrace.Id:00000}: Detected Player DB deadlock: {playerSavesWaiting} saves queued!"
                    );

                    break;
            }

            lock (mPlayerDbLock)
            {
                var saved = false;
                var failures = 0;
                while (!saved)
                {
                    try
                    {
                        var elapsedMs = SaveDb(sPlayerDb);
                        saved = true;
                        playerDbLogger?.Debug(
                            $"{currentTrace.Id:00000}: Save took {elapsedMs}ms, {--playerSavesWaiting} saves queued."
                        );

                        if (playerDbTraces.TryPeek(out var peekTrace) && peekTrace.Id != currentTrace.Id)
                        {
                            playerDbLogger?.Debug(
                                $"{currentTrace.Id:00000}: Next save expected to complete was {peekTrace.Id:00000}, which is from a different call."
                            );
                        }

                        playerDbLogger?.Debug(
                            playerDbTraces.TryDequeue(out var dequeueTrace)
                                ? $"{dequeueTrace.Id:00000} ({currentTrace.Id:00000}): Save completed."
                                : $"{currentTrace.Id:00000}: Save complete but there are no available traces."
                        );
                    }
                    catch (Exception ex)
                    {
                        if (ex is InvalidOperationException)
                        {
                            //Collection was modified?
                            //Loop and try to save again!
                            failures++;
                            if (failures > 10)
                            {
                                Console.WriteLine(
                                    "Error Saving Player Database! Server will shutdown in order to prevent potential rollback scenarios!"
                                );

                                Task.Factory.StartNew(
                                    () => Bootstrapper.OnUnhandledException(
                                        Thread.CurrentThread.Name, new UnhandledExceptionEventArgs(ex, true)
                                    )
                                );

                                playerDbLogger?.Error(
                                    ex,
                                    "Error Saving Player Database! Server will shutdown in order to prevent potential rollback scenarios! [Failures: " +
                                    failures +
                                    "]"
                                );

                                break;
                            }

                            //This should be a warning but I want to actually see it working in a real environment without people needing to change their configs for a few builds
                            //TODO change to .Warn
                            playerDbLogger?.Error(
                                ex,
                                "Collection was modified? while trying to save player db, will loop and try to save again! [Failures: " +
                                failures +
                                "]"
                            );
                        }
                        else
                        {
                            Console.WriteLine(
                                "Error Saving Player Database! Server will shutdown in order to prevent potential rollback scenarios!"
                            );

                            Task.Factory.StartNew(
                                () => Bootstrapper.OnUnhandledException(
                                    Thread.CurrentThread.Name, new UnhandledExceptionEventArgs(ex, true)
                                )
                            );

                            playerDbLogger?.Error(
                                ex,
                                "Error Saving Player Database! Server will shutdown in order to prevent potential rollback scenarios!"
                            );

                            break;
                        }
                    }
                }
            }
        }

        private static long SaveDb(DbContext dbContext)
        {
            if (dbContext == null)
            {
                return -1;
            }

            var stopwatch = Stopwatch.StartNew();
            dbContext.SaveChanges();
            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
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

            lock (mGameDbLock)
            {
                lock (mPlayerDbLock)
                {
                    Console.WriteLine(Strings.Migration.startingmigration);
                    if (gameDb && newGameContext != null)
                    {
                        MigrateDbSet(sGameDb.Animations, newGameContext.Animations);
                        MigrateDbSet(sGameDb.Classes, newGameContext.Classes);
                        MigrateDbSet(sGameDb.CraftingTables, newGameContext.CraftingTables);
                        MigrateDbSet(sGameDb.Crafts, newGameContext.Crafts);
                        MigrateDbSet(sGameDb.Events, newGameContext.Events);
                        MigrateDbSet(sGameDb.Items, newGameContext.Items);
                        MigrateDbSet(sGameDb.MapFolders, newGameContext.MapFolders);
                        MigrateDbSet(sGameDb.Maps, newGameContext.Maps);
                        MigrateDbSet(sGameDb.Npcs, newGameContext.Npcs);
                        MigrateDbSet(sGameDb.Projectiles, newGameContext.Projectiles);
                        MigrateDbSet(sGameDb.Quests, newGameContext.Quests);
                        MigrateDbSet(sGameDb.Resources, newGameContext.Resources);
                        MigrateDbSet(sGameDb.Shops, newGameContext.Shops);
                        MigrateDbSet(sGameDb.Spells, newGameContext.Spells);
                        MigrateDbSet(sGameDb.ServerVariables, newGameContext.ServerVariables);
                        MigrateDbSet(sGameDb.PlayerVariables, newGameContext.PlayerVariables);
                        MigrateDbSet(sGameDb.Tilesets, newGameContext.Tilesets);
                        MigrateDbSet(sGameDb.Time, newGameContext.Time);
                        newGameContext.SaveChanges();
                        Options.GameDb = newOpts;
                        Options.SaveToDisk();
                    }
                    else if (!gameDb && newPlayerContext != null)
                    {
                        MigrateDbSet(sPlayerDb.Users, newPlayerContext.Users);
                        MigrateDbSet(sPlayerDb.Players, newPlayerContext.Players);
                        MigrateDbSet(sPlayerDb.Player_Friends, newPlayerContext.Player_Friends);
                        MigrateDbSet(sPlayerDb.Player_Spells, newPlayerContext.Player_Spells);
                        MigrateDbSet(sPlayerDb.Player_Variables, newPlayerContext.Player_Variables);
                        MigrateDbSet(sPlayerDb.Player_Hotbar, newPlayerContext.Player_Hotbar);
                        MigrateDbSet(sPlayerDb.Player_Quests, newPlayerContext.Player_Quests);
                        MigrateDbSet(sPlayerDb.Bags, newPlayerContext.Bags);
                        MigrateDbSet(sPlayerDb.Player_Items, newPlayerContext.Player_Items);
                        MigrateDbSet(sPlayerDb.Player_Bank, newPlayerContext.Player_Bank);
                        MigrateDbSet(sPlayerDb.Bag_Items, newPlayerContext.Bag_Items);
                        MigrateDbSet(sPlayerDb.Mutes, newPlayerContext.Mutes);
                        MigrateDbSet(sPlayerDb.Bans, newPlayerContext.Bans);
                        newPlayerContext.SaveChanges();
                        Options.PlayerDb = newOpts;
                        Options.SaveToDisk();
                    }
                }
            }

            Console.WriteLine(Strings.Migration.migrationcomplete);
            Bootstrapper.Context.ServerConsole.Wait(true);
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

        //External Context Access (Dangerous, not thread safe, you gotta get the lock before using the context else you're not gonna enjoy life.
        public static object GetPlayerContextLock()
        {
            return mPlayerDbLock;
        }

        public static PlayerContext GetPlayerContext()
        {
            return sPlayerDb;
        }

        private struct IdTrace
        {

            public long Id;

            public string Trace;

            public override string ToString()
            {
                return $"{Id:000000}: {Trace}";
            }

        }

    }

}
