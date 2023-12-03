using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
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
using Intersect.Reflection;
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

using MySqlConnector;
using LogLevel = Intersect.Logging.LogLevel;

namespace Intersect.Server.Database
{

    public static partial class DbInterface
    {

        /// <summary>
        /// This is our thread pool for handling game-loop database interactions.
        /// Min/Max Number of Threads & Idle Timeouts are set via server config.
        /// </summary>
        public static SmartThreadPool Pool = new(
                new STPStartInfo()
                {
                    ThreadPoolName = "DatabasePool",
                    IdleTimeout = Options.Instance.Processing.DatabaseThreadIdleTimeout,
                    MinWorkerThreads = Options.Instance.Processing.MinDatabaseThreads,
                    MaxWorkerThreads = Options.Instance.Processing.MaxDatabaseThreads
                }
            );

        private static string GameDbFilename => Path.Combine(ServerContext.ResourceDirectory, "gamedata.db");

        private static string LoggingDbFilename => Path.Combine(ServerContext.ResourceDirectory, "logging.db");

        private static string PlayersDbFilename => Path.Combine(ServerContext.ResourceDirectory, "playerdata.db");

        private static Logger _gameDatabaseLogger { get; set; }

        private static Logger _playerDatabaseLogger { get; set; }

        public static Dictionary<string, ServerVariableBase> ServerVariableEventTextLookup = new();

        public static Dictionary<string, PlayerVariableBase> PlayerVariableEventTextLookup = new();

        public static Dictionary<string, GuildVariableBase> GuildVariableEventTextLookup = new();

        public static Dictionary<string, UserVariableBase> UserVariableEventTextLookup = new();

        public static ConcurrentDictionary<Guid, ServerVariableBase> UpdatedServerVariables = new();

        private static List<MapGrid> mapGrids = new();

        public static GameContext CreateGameContext(
            bool readOnly = true,
            bool explicitLoad = false,
            bool lazyLoading = false,
            bool autoDetectChanges = false,
            QueryTrackingBehavior? queryTrackingBehavior = default
        ) => GameContext.Create(new DatabaseContextOptions
        {
            AutoDetectChanges = autoDetectChanges,
            ConnectionStringBuilder = Options.Instance.GameDatabase.Type.CreateConnectionStringBuilder(
                Options.Instance.GameDatabase,
                GameDbFilename
            ),
            DatabaseType = Options.Instance.GameDatabase.Type,
            ExplicitLoad = explicitLoad,
            KillServerOnConcurrencyException = Options.Instance.GameDatabase.KillServerOnConcurrencyException,
            LazyLoading = lazyLoading,
#if DEBUG
            LoggerFactory = new IntersectLoggerFactory(nameof(GameContext)),
#endif
            QueryTrackingBehavior = queryTrackingBehavior,
            ReadOnly = readOnly,
        });

        internal static LoggingContext CreateLoggingContext(
            bool readOnly = true,
            bool explicitLoad = false,
            bool lazyLoading = false,
            bool autoDetectChanges = false,
            QueryTrackingBehavior? queryTrackingBehavior = default
        ) => LoggingContext.Create(new DatabaseContextOptions
        {
            AutoDetectChanges = autoDetectChanges,
            ConnectionStringBuilder = Options.Instance.LoggingDatabase.Type.CreateConnectionStringBuilder(
                Options.Instance.LoggingDatabase,
                LoggingDbFilename
            ),
            DatabaseType = Options.Instance.LoggingDatabase.Type,
            ExplicitLoad = explicitLoad,
            KillServerOnConcurrencyException = Options.Instance.LoggingDatabase.KillServerOnConcurrencyException,
            LazyLoading = lazyLoading,
#if DEBUG
            LoggerFactory = new IntersectLoggerFactory(nameof(LoggingContext)),
#endif
            QueryTrackingBehavior = queryTrackingBehavior,
            ReadOnly = readOnly,
        });

        /// <summary>
        /// Creates a game context to query. Best practice is to scope this within a using statement.
        /// </summary>
        /// <param name="readOnly">Defines whether or not the context should initialize with change tracking. If readonly is true then SaveChanges will not work.</param>
        /// <returns></returns>
        public static PlayerContext CreatePlayerContext(
            bool readOnly = true,
            bool explicitLoad = false,
            bool lazyLoading = false,
            bool autoDetectChanges = false,
            QueryTrackingBehavior? queryTrackingBehavior = default
        ) => PlayerContext.Create(new DatabaseContextOptions
        {
            AutoDetectChanges = autoDetectChanges,
            ConnectionStringBuilder = Options.Instance.PlayerDatabase.Type.CreateConnectionStringBuilder(
                Options.Instance.PlayerDatabase,
                PlayersDbFilename
            ),
            DatabaseType = Options.Instance.PlayerDatabase.Type,
            ExplicitLoad = explicitLoad,
            KillServerOnConcurrencyException = Options.Instance.PlayerDatabase.KillServerOnConcurrencyException,
            LazyLoading = lazyLoading,
#if DEBUG
            LoggerFactory = new IntersectLoggerFactory(nameof(PlayerContext)),
#endif
            QueryTrackingBehavior = queryTrackingBehavior,
            ReadOnly = readOnly,
        });

        public static void InitializeDbLoggers()
        {
            if (Options.Instance.GameDatabase.LogLevel > LogLevel.None)
            {
                _gameDatabaseLogger = new Logger(
                    new LogConfiguration
                    {
                        Tag = "GAMEDB",
                        LogLevel = Options.Instance.GameDatabase.LogLevel,
                        Outputs = ImmutableList.Create<ILogOutput>(
                            new FileOutput(Log.SuggestFilename(null, "gamedb"), LogLevel.Debug)
                        )
                    }
                );
            }

            if (Options.Instance.PlayerDatabase.LogLevel > LogLevel.None)
            {
                _playerDatabaseLogger = new Logger(
                    new LogConfiguration
                    {
                        Tag = "PLAYERDB",
                        LogLevel = Options.Instance.PlayerDatabase.LogLevel,
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
            if (Directory.Exists(ServerContext.ResourceDirectory))
            {
                return;
            }

            if (ServerContext.IsDefaultResourceDirectory)
            {
                Directory.CreateDirectory(ServerContext.ResourceDirectory);
            }
            else
            {
                throw new DirectoryNotFoundException(
                    Path.Combine(Environment.CurrentDirectory, ServerContext.ResourceDirectory)
                );
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
                case DatabaseType.SQLite:
                    return new SqliteConnectionStringBuilder($"Data Source={filename}");

                case DatabaseType.MySQL:
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

        private static readonly MethodInfo _methodInfoProcessMigrations =
            typeof(DbInterface)
                .GetMethod(nameof(ProcessMigrations), BindingFlags.NonPublic | BindingFlags.Static);

        private static void ProcessMigrations<TContext>(TContext context)
            where TContext : IntersectDbContext<TContext>
        {
            if (!context.HasPendingMigrations)
            {
                Log.Verbose("No pending migrations, skipping...");
                return;
            }

            Log.Verbose($"Pending schema migrations for {typeof(TContext).Name}:\n\t{string.Join("\n\t", context.PendingSchemaMigrations)}");
            Log.Verbose($"Pending data migrations for {typeof(TContext).Name}:\n\t{string.Join("\n\t", context.PendingDataMigrationNames)}");

            var migrationScheduler = new MigrationScheduler<TContext>(context);
            Log.Verbose("Scheduling pending migrations...");
            migrationScheduler.SchedulePendingMigrations();

            Log.Verbose("Applying scheduled migrations...");
            migrationScheduler.ApplyScheduledMigrations();

            var remainingPendingSchemaMigrations = context.PendingSchemaMigrations.ToList();
            var processedSchemaMigrations =
                context.PendingSchemaMigrations.Where(migration => !remainingPendingSchemaMigrations.Contains(migration));

            context.OnSchemaMigrationsProcessed(processedSchemaMigrations.ToArray());
        }

        private static bool EnsureUpdated(IServerContext serverContext)
        {
            Log.Verbose("Creating game context...");
            using var gameContext = GameContext.Create(new DatabaseContextOptions
            {
                ConnectionStringBuilder = Options.Instance.GameDatabase.Type.CreateConnectionStringBuilder(
                    Options.Instance.GameDatabase,
                    GameDbFilename
                ),
                DatabaseType = Options.Instance.GameDatabase.Type,
                EnableDetailedErrors = true,
                EnableSensitiveDataLogging = true,
                LoggerFactory = new IntersectLoggerFactory(nameof(GameContext)),
            });

            Log.Verbose("Creating player context...");
            using var playerContext = PlayerContext.Create(new DatabaseContextOptions
            {
                ConnectionStringBuilder = Options.Instance.PlayerDatabase.Type.CreateConnectionStringBuilder(
                    Options.Instance.PlayerDatabase,
                    PlayersDbFilename
                ),
                DatabaseType = Options.Instance.PlayerDatabase.Type,
                EnableDetailedErrors = true,
                EnableSensitiveDataLogging = true,
                LoggerFactory = new IntersectLoggerFactory(nameof(PlayerContext)),
            });

            Log.Verbose("Creating logging context...");
            using var loggingContext = LoggingContext.Create(new DatabaseContextOptions
            {
                ConnectionStringBuilder = Options.Instance.LoggingDatabase.Type.CreateConnectionStringBuilder(
                    Options.Instance.LoggingDatabase,
                    LoggingDbFilename
                ),
                DatabaseType = Options.Instance.LoggingDatabase.Type,
                EnableDetailedErrors = true,
                EnableSensitiveDataLogging = true,
                LoggerFactory = new IntersectLoggerFactory(nameof(LoggingContext)),
            });

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

            var gameContextPendingMigrations = gameContext.PendingSchemaMigrations;
            var playerContextPendingMigrations = playerContext.PendingSchemaMigrations;
            var loggingContextPendingMigrations = loggingContext.PendingSchemaMigrations;

            var showMigrationWarning = (
                gameContextPendingMigrations.Any() && !gameContextPendingMigrations.Contains("20180905042857_Initial")
            ) || (
                playerContextPendingMigrations.Any() &&
                !playerContextPendingMigrations.Contains("20180927161502_InitialPlayerDb")
            ) || (
                loggingContextPendingMigrations.Any() &&
                !loggingContextPendingMigrations.Contains("20191118024649_RequestLogs")
            );

            if (showMigrationWarning)
            {
                if (serverContext.StartupOptions.MigrateAutomatically)
                {
                    Console.WriteLine(Strings.Database.MigratingAutomatically);
                    Log.Default.Write("Skipping user prompt for database migration...");
                }
                else
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

                        if (
                            !string.Equals(
                                input,
                                Strings.Database.upgradeexit.ToString().Trim(),
                                StringComparison.CurrentCultureIgnoreCase
                            )
                        )
                        {
                            continue;
                        }

                        Environment.Exit(1);

                        return false;
                    }
                }

                Console.WriteLine();
                Console.WriteLine(
                    "Please wait! Migrations can take several minutes, and even longer if you are using MySQL databases!"
                );
            }
            else
            {
                Console.WriteLine("No migrations pending, skipping...");
            }

            var contexts = new List<DbContext> { gameContext, playerContext, loggingContext };
            foreach (var context in contexts)
            {
                var contextType = context.GetType().FindGenericTypeParameters(typeof(IntersectDbContext<>)).First();
                _methodInfoProcessMigrations.MakeGenericMethod(contextType).Invoke(null, new object[] { context });
            }

            return true;
        }

        // Database setup, version checking
        internal static bool InitDatabase(IServerContext serverContext)
        {
            Console.WriteLine("Initializing database...");

            if (!EnsureUpdated(serverContext))
            {
                Console.Error.WriteLine("Database not updated.");
                return false;
            }

            Console.WriteLine("Loading game data...");

            LoadAllGameObjects();
            LoadTime();
            OnClassesLoaded();
            OnMapsLoaded();
            CacheServerVariableEventTextLookups();
            CachePlayerVariableEventTextLookups();
            CacheGuildVariableEventTextLookups();
            CacheUserVariableEventTextLookups();

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
            var user = User.FindByEmail(email);
            if (user != null)
            {
                return user.Name;
            }
            return null;
        }

        public static Player GetUserCharacter(User user, Guid playerId, bool explicitLoad = false)
        {
            if (user == default)
            {
                return default;
            }

            foreach (var player in user.Players)
            {
                if (player.Id != playerId)
                {
                    continue;
                }

                if (!explicitLoad)
                {
                    return player;
                }

                try
                {
                    using var playerContext = CreatePlayerContext(readOnly: true, explicitLoad: false);
                    player.LoadRelationships(playerContext);
                    _ = Player.Validate(player);
                }
                catch (Exception exception)
                {
                    Debugger.Break();
                    Log.Error(exception);
                    throw new Exception($"Error during explicit load of player {BitConverter.ToString(playerId.ToByteArray()).Replace("-", string.Empty)}", exception);
                }

                return player;
            }

            return null;
        }

        public static bool TryRegister(
            string username,
            string email,
            string password,
            [NotNullWhen(true)] out User? user
        )
        {
            try
            {
                var rawSaltData = RandomNumberGenerator.GetBytes(20);
                var rawSalt = Convert.ToBase64String(rawSaltData);
                var encodedSaltData = Encoding.UTF8.GetBytes(rawSalt);
                var saltData = SHA256.HashData(encodedSaltData);
                var salt = BitConverter.ToString(saltData).Replace("-", string.Empty);

                user = new User
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

                user.Save(create: true);
                return true;
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                user = default;
                return false;
            }
        }

        public static void CreateAccount(
            Client? client,
            string username,
            string password,
            string email,
            bool grantFirstUserAdmin = true
        )
        {
            var salt = User.GenerateSalt();
            var saltedPasswordHash = User.SaltPasswordHash(password, salt);

            var user = new User
            {
                Name = username,
                Email = email,
                Salt = salt,
                Password = saltedPasswordHash,
                Power = UserRights.None,
            };

            if (grantFirstUserAdmin && User.Count() == 0)
            {
                user.Power = UserRights.Admin;
            }

            user.Save(create: true);

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
                var type = (GameObjectType)value;
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
                case GameObjectType.UserVariable:
                    UserVariableBase.Lookup.Clear();

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
                            foreach (var anim in context.Animations) // TODO: fix "The data is NULL at ordinal 2"
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
                            foreach (var itm in context.Items.Include(i => i.EquipmentProperties))
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
                                MapController.Lookup.Set(map.Id, map);
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
                        case GameObjectType.UserVariable:
                            foreach (var psw in context.UserVariables)
                            {
                                UserVariableBase.Lookup.Set(psw.Id, psw);
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
                    dbObj = new MapController(predefinedid);

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
                    ((QuestBase)dbObj).StartEvent = (EventBase)AddGameObject(GameObjectType.Event);
                    ((QuestBase)dbObj).EndEvent = (EventBase)AddGameObject(GameObjectType.Event);
                    ((QuestBase)dbObj).StartEvent.CommonEvent = false;
                    ((QuestBase)dbObj).EndEvent.CommonEvent = false;

                    break;

                case GameObjectType.GuildVariable:
                    dbObj = new GuildVariableBase(predefinedid);

                    break;

                case GameObjectType.UserVariable:
                    dbObj = new UserVariableBase(predefinedid);

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
                            context.Maps.Add((MapController)dbObj);
                            MapController.Lookup.Set(dbObj.Id, dbObj);

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

                        case GameObjectType.UserVariable:
                            context.UserVariables.Add((UserVariableBase)dbObj);
                            UserVariableBase.Lookup.Set(dbObj.Id, dbObj);

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
                            //Delete all map events first
                            foreach (var evtId in ((MapController)gameObject).EventIds)
                            {
                                var evt = EventBase.Get(evtId);
                                if (evt != null)
                                {
                                    DeleteGameObject(evt);
                                }
                            }
                            context.Maps.Remove((MapController)gameObject);
                            MapController.Lookup.Delete(gameObject);

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
                        case GameObjectType.UserVariable:
                            context.UserVariables.Remove((UserVariableBase)gameObject);

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
                        {
                            if (gameObject is not ItemBase itemDescriptor)
                            {
                                throw new InvalidOperationException();
                            }

                            itemDescriptor.ValidateStatRanges();

                            if (itemDescriptor.EquipmentProperties?.DescriptorId == Guid.Empty)
                            {
                                context.Items_EquipmentProperties.Add(itemDescriptor.EquipmentProperties);
                            }
                            else
                            {
                                EquipmentProperties? deletedEquipmentProperties =
                                    context.Items_EquipmentProperties.FirstOrDefault(
                                        ep => ep.DescriptorId == itemDescriptor.Id
                                    );
                                if (deletedEquipmentProperties != default)
                                {
                                    context.Items_EquipmentProperties.Remove(deletedEquipmentProperties);
                                }
                            }

                            context.Items.Update(itemDescriptor);

                            break;
                        }
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
                            context.Maps.Update((MapController)gameObject);

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
                        case GameObjectType.UserVariable:
                            context.UserVariables.Update((UserVariableBase)gameObject);

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

            foreach (var map in MapController.Lookup)
            {
                ((MapController)map.Value).Initialize();
            }
        }

        private static void OnClassesLoaded()
        {
            if (ClassBase.Lookup.Count == 0)
            {
                Console.WriteLine(Strings.Database.noclasses);
                var cls = (ClassBase)AddGameObject(GameObjectType.Class);
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
                for (var i = 0; i < Enum.GetValues<Vital>().Length; i++)
                {
                    cls.BaseVital[i] = 20;
                }

                for (var i = 0; i < Enum.GetValues<Stat>().Length; i++)
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

        public static void CacheUserVariableEventTextLookups()
        {
            var lookup = new Dictionary<string, UserVariableBase>();
            var addedIds = new HashSet<string>();
            foreach (UserVariableBase variable in UserVariableBase.Lookup.Values)
            {
                if (!string.IsNullOrWhiteSpace(variable.TextId) && !addedIds.Contains(variable.TextId))
                {
                    lookup.Add(Strings.Events.UserVariable + "{" + variable.TextId + "}", variable);
                    addedIds.Add(variable.TextId);
                }
            }
            UserVariableEventTextLookup = lookup;
        }

        //Extra Map Helper Functions
        public static void CheckAllMapConnections()
        {
            var changed = false;
            foreach (MapController map in MapController.Lookup.Values)
            {
                CheckMapConnections(map, MapController.Lookup);
            }
        }

        public static bool CheckMapConnections(MapController map, DatabaseObjectLookup maps)
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
                foreach (var map in MapController.Lookup.Values)
                {
                    if (mapGrids.Count < 1)
                    {
                        mapGrids.Add(new MapGrid(map.Id, 0));
                        continue;
                    }

                    for (var y = 0; y < mapGrids.Count; y++)
                    {
                        if (!mapGrids[y].Contains(map.Id))
                        {
                            if (y != mapGrids.Count - 1)
                            {
                                continue;
                            }

                            mapGrids.Add(new MapGrid(map.Id, mapGrids.Count));
                        }

                        break;
                    }
                }

                foreach (MapController map in MapController.Lookup.Values)
                {
                    lock (map.GetMapLock())
                    {
                        var gridIndex = map.MapGrid;
                        var grid = mapGrids[gridIndex];
                        var surroundingMapIds = new List<Guid>();
                        var surroundingMaps = new List<MapController>();
                        for (var x = map.MapGridX - 1; x <= map.MapGridX + 1; x++)
                        {
                            for (var y = map.MapGridY - 1; y <= map.MapGridY + 1; y++)
                            {
                                if (x == map.MapGridX && y == map.MapGridY)
                                {
                                    continue;
                                }

                                if (x < grid.XMin || x >= grid.XMax || y < grid.YMin || y >= grid.YMax)
                                {
                                    continue;
                                }

                                if (grid.MapIdGrid[x, y] == Guid.Empty)
                                {
                                    continue;
                                }

                                var idFromGrid = grid.MapIdGrid[x, y];
                                surroundingMapIds.Add(idFromGrid);
                                if (MapController.TryGet(idFromGrid, out var mapOnGrid))
                                {
                                    surroundingMaps.Add(mapOnGrid);
                                }
                            }
                        }
                        map.SurroundingMapIds = surroundingMapIds.ToArray();
                        map.SurroundingMaps = surroundingMaps.ToArray();
                    }
                }

                for (var gridIndex = 0; gridIndex < mapGrids.Count; gridIndex++)
                {
                    PacketSender.SendMapGridToAll(gridIndex);
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
                return mapGrids.Any(mapGrid => mapGrid.Contains(id));
            }
        }

        //Map Folders
        private static void LoadMapFolders()
        {
            try
            {
                using (var context = CreateGameContext(readOnly: false))
                {
                    var mapFolders = context.MapFolders.OrderBy(f => f.Id).FirstOrDefault();
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
                    var time = context.Time.OrderBy(t => t.Id).FirstOrDefault();
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

        public static void HandleMigrationCommand()
        {
            var databases = new List<(Type, DatabaseOptions, string)>
            {
                (typeof(GameContext), Options.Instance.GameDatabase, Strings.Migration.GameDatabaseName),
                (typeof(PlayerContext), Options.Instance.PlayerDatabase, Strings.Migration.PlayerDatabaseName),
                (typeof(LoggingContext), Options.Instance.LoggingDatabase, Strings.Migration.LoggingDatabaseName)
            };

            Console.WriteLine();
            Console.WriteLine(Strings.Migration.SelectContext);
            Console.WriteLine();

            for (var databaseIndex = 0; databaseIndex < databases.Count; databaseIndex++)
            {
                var (contextType, options, databaseName) = databases[databaseIndex];
                var selectionNumber = databaseIndex + 1;
                var databaseTypeName = options.Type.GetName();
                Console.WriteLine(Strings.Migration.SelectDatabase.ToString(
                    selectionNumber,
                    databaseName,
                    databaseTypeName,
                    contextType == typeof(GameContext) ? Strings.Migration.SqliteRecommended : string.Empty
                ));
            }

            Console.WriteLine();
            Console.WriteLine(Strings.Migration.Cancel);

            // TODO: Remove > when moving to ReadKeyWait when console magic is ready
            Console.Write("> ");
            var input = Console.ReadLine();
            Console.WriteLine();

            if (!int.TryParse(input, out var selectedDatabaseIndex))
            {
                Console.WriteLine(Strings.Migration.MigrationCanceled);
                return;
            }

            if (selectedDatabaseIndex < 1 || selectedDatabaseIndex > databases.Count)
            {
                Console.WriteLine(Strings.Migration.MigrationCanceled);
                return;
            }

            var (selectedContextType, selectedOptions, selectedDatabaseName) = databases[selectedDatabaseIndex - 1];

            var databaseTypes = new List<DatabaseType> { DatabaseType.Sqlite, DatabaseType.MySql };

            Console.WriteLine();
            Console.WriteLine(Strings.Migration.SelectProvider.ToString(selectedDatabaseName));
            var databaseTypeIndex = 1;
            foreach (var databaseType in databaseTypes)
            {
                Console.WriteLine(
                    Strings.Migration.SelectDatabaseType.ToString(databaseTypeIndex, databaseType.GetName()));
                ++databaseTypeIndex;
            }

            Console.WriteLine();
            Console.WriteLine(Strings.Migration.Cancel);

            // TODO: Remove > when moving to ReadKeyWait when console magic is ready
            Console.Write("> ");
            input = Console.ReadLine();
            Console.WriteLine();

            if (!int.TryParse(input, out var selectedDatabaseTypeIndex))
            {
                Console.WriteLine(Strings.Migration.MigrationCanceled);
                return;
            }

            if (selectedDatabaseTypeIndex < 1 || selectedDatabaseTypeIndex > databaseTypes.Count)
            {
                Console.WriteLine(Strings.Migration.MigrationCanceled);
                return;
            }

            var selectedDatabaseType = databaseTypes[selectedDatabaseTypeIndex - 1];
            if (selectedDatabaseType == selectedOptions.Type)
            {
                Console.WriteLine();
                Console.WriteLine(
                    Strings.Migration.AlreadyUsingProvider.ToString(selectedDatabaseName,
                        selectedDatabaseType.GetName()));
                Console.WriteLine(Strings.Migration.MigrationCanceled);
                return;
            }

            try
            {
                Task task;
                if (selectedContextType == typeof(GameContext))
                {
                    task = Migrate<GameContext>(selectedOptions, selectedDatabaseType);
                }
                else if (selectedContextType == typeof(PlayerContext))
                {
                    task = Migrate<PlayerContext>(selectedOptions, selectedDatabaseType);
                }
                else if (selectedContextType == typeof(LoggingContext))
                {
                    task = Migrate<LoggingContext>(selectedOptions, selectedDatabaseType);
                }
                else
                {
                    throw new InvalidOperationException();
                }

                task.Wait();
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                throw;
            }
        }

        public static async Task Migrate<TContext>(DatabaseOptions fromDatabaseOptions, DatabaseType toDatabaseType)
            where TContext : IntersectDbContext<TContext>
        {
            string sqliteFileName;
            if (typeof(TContext) == typeof(GameContext))
            {
                sqliteFileName = GameDbFilename;
            }
            else if (typeof(TContext) == typeof(LoggingContext))
            {
                sqliteFileName = LoggingDbFilename;
            }
            else if (typeof(TContext) == typeof(PlayerContext))
            {
                sqliteFileName = PlayersDbFilename;
            }
            else
            {
                throw new InvalidOperationException();
            }

            var fromContextOptions = new DatabaseContextOptions
            {
                AutoDetectChanges = false,
                ConnectionStringBuilder =
                    fromDatabaseOptions.Type.CreateConnectionStringBuilder(fromDatabaseOptions, sqliteFileName),
                DatabaseType = fromDatabaseOptions.Type,
                ExplicitLoad = false,
                LazyLoading = false,
                LoggerFactory = default,
                QueryTrackingBehavior = default,
                ReadOnly = false
            };

            DatabaseOptions toDatabaseOptions;
            DatabaseContextOptions toContextOptions;
            switch (toDatabaseType)
            {
                case DatabaseType.MySql:
                    {
                        while (true)
                        {
                            Console.WriteLine(Strings.Migration.EnterConnectionStringParameters);

                            Console.Write(Strings.Migration.PromptHost.ToString(Strings.Migration.DefaultHost));
                            var host = Console.ReadLine()?.Trim();
                            if (string.IsNullOrWhiteSpace(host))
                            {
                                host = Strings.Migration.DefaultHost;
                            }

                            Console.Write(Strings.Migration.PromptPort.ToString(Strings.Migration.DefaultPortMySql));
                            var portString = Console.ReadLine()?.Trim();
                            if (string.IsNullOrWhiteSpace(portString))
                            {
                                portString = Strings.Migration.DefaultPortMySql;
                            }
                            var port = ushort.Parse(portString);

                            var contextName = typeof(TContext).Name.Replace("Context", "").ToLowerInvariant();
                            var version = typeof(DbInterface).Assembly.GetVersionName();
                            var defaultDatabase = Strings.Migration.DefaultDatabase.ToString(version, contextName);
                            Console.Write(Strings.Migration.PromptDatabase.ToString(defaultDatabase));
                            var database = Console.ReadLine()?.Trim();
                            if (string.IsNullOrWhiteSpace(database))
                            {
                                database = defaultDatabase;
                            }

                            Console.Write(Strings.Migration.PromptUsername.ToString(Strings.Migration.DefaultUsername));
                            var username = Console.ReadLine().Trim();
                            if (string.IsNullOrWhiteSpace(username))
                            {
                                username = Strings.Migration.DefaultUsername;
                            }

                            Console.Write(Strings.Migration.PromptPassword);
                            var password = GetPassword();

                            Console.WriteLine();
                            Console.WriteLine(Strings.Migration.MySqlConnecting);

                            toDatabaseOptions = new()
                            {
                                Type = toDatabaseType,
                                Server = host,
                                Port = port,
                                Database = database,
                                Username = username,
                                Password = password
                            };
                            toContextOptions = new()
                            {
                                ConnectionStringBuilder = toDatabaseType.CreateConnectionStringBuilder(
                                    toDatabaseOptions,
                                    default
                                ),
                                DatabaseType = toDatabaseType
                            };

                            try
                            {
                                await using var testContext = IntersectDbContext<TContext>.Create(toContextOptions);
                                break;
                            }
                            catch (Exception exception)
                            {
                                Log.Error(Strings.Migration.MySqlConnectionError.ToString(exception));
                                Console.WriteLine();
                                Console.WriteLine(Strings.Migration.MySqlTryAgain);
                                var input = Console.ReadLine();
                                var key = input.Length > 0 ? input[0] : ' ';
                                Console.WriteLine();

                                var shouldTryAgain = string.Equals(
                                    Strings.Migration.TryAgainCharacter,
                                    key.ToString(),
                                    StringComparison.Ordinal
                                );

                                if (shouldTryAgain)
                                {
                                    continue;
                                }

                                Log.Info(Strings.Migration.MigrationCanceled);
                                return;
                            }
                        }

                        break;
                    }

                case DatabaseType.Sqlite:
                    {
                        string dbFileName;
                        if (typeof(TContext).Extends<GameContext>())
                        {
                            dbFileName = GameDbFilename;
                        }
                        else if (typeof(TContext).Extends<PlayerContext>())
                        {
                            dbFileName = PlayersDbFilename;
                        }
                        else if (typeof(TContext).Extends<LoggingContext>())
                        {
                            dbFileName = LoggingDbFilename;
                        }
                        else
                        {
                            throw new InvalidOperationException($"Unsupported context type: {typeof(TContext).FullName}");
                        }

                        // Check if target SQLite file exists
                        if (File.Exists(dbFileName))
                        {
                            // If it does, check if it is OK to overwrite
                            Console.WriteLine();
                            Log.Error(Strings.Migration.DatabaseFileAlreadyExists.ToString(dbFileName));
                            var input = Console.ReadLine();
                            var key = input.Length > 0 ? input[0] : ' ';
                            Console.WriteLine();
                            if (key.ToString() != Strings.Migration.ConfirmCharacter)
                            {
                                Log.Info(Strings.Migration.MigrationCanceled);
                                return;
                            }

                            File.Delete(dbFileName);
                        }

                        toDatabaseOptions = new() { Type = toDatabaseType };
                        toContextOptions = new()
                        {
                            ConnectionStringBuilder = toDatabaseType.CreateConnectionStringBuilder(
                                toDatabaseOptions,
                                dbFileName
                            ),
                            DatabaseType = toDatabaseType
                        };

                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(toDatabaseType), toDatabaseType, null);
            }

            // Shut down server, start migration.
            Log.Info(Strings.Migration.StoppingServer);

            //This variable will end the server loop and save any pending changes
            ServerContext.Instance.DisposeWithoutExiting = true;
            ServerContext.Instance.RequestShutdown();

            while (ServerContext.Instance.IsRunning)
            {
                Thread.Sleep(100);
            }

            Log.Info(Strings.Migration.StartingMigration);
            var migrationService = new DatabaseTypeMigrationService();
            if (await migrationService.TryMigrate<TContext>(fromContextOptions, toContextOptions))
            {
                if (typeof(TContext).Extends<GameContext>())
                {
                    Options.Instance.GameDatabase = toDatabaseOptions;
                }
                else if (typeof(TContext).Extends<PlayerContext>())
                {
                    Options.Instance.PlayerDatabase = toDatabaseOptions;
                }
                else if (typeof(TContext).Extends<LoggingContext>())
                {
                    Options.Instance.LoggingDatabase = toDatabaseOptions;
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported context type: {typeof(TContext).FullName}");
                }

                Options.SaveToDisk();

                Log.Info(Strings.Migration.MigrationComplete);
                Bootstrapper.Context.WaitForConsole();
                ServerContext.Exit(0);
            }
            else
            {
                Log.Error($"Error migrating context type: {typeof(TContext).FullName}");
                ServerContext.Exit(1);
            }
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
    }
}
