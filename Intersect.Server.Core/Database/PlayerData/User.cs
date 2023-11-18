using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Logging;
using Intersect.Reflection;
using Intersect.Security;
using Intersect.Server.Core;
using Intersect.Server.Database.Logging.Entities;
using Intersect.Server.Database.PlayerData.Api;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Entities;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using Intersect.Server.Web.RestApi.Payloads;
using Intersect.Utilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VariableValue = Intersect.GameObjects.Switches_and_Variables.VariableValue;

// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Intersect.Server.Database.PlayerData
{

    [ApiVisibility(ApiVisibility.Restricted | ApiVisibility.Private)]
    public partial class User
    {
        private long _lastSave;
        private readonly object _lastSaveLock = new();

        private static readonly ConcurrentDictionary<Guid, User> OnlineUsers = new();

        [JsonIgnore][NotMapped] private readonly object mSavingLock = new();

        /// <summary>
        ///     Variables that have been updated for this account which need to be saved to the db
        /// </summary>
        [JsonIgnore]
        public ConcurrentDictionary<Guid, UserVariableBase> UpdatedVariables = new();

        public static int OnlineCount => OnlineUsers.Count;

        public static List<User> OnlineList => OnlineUsers.Values.ToList();

        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public Guid Id { get; private set; } = Guid.NewGuid();

        [Column(Order = 1)] public string Name { get; set; }

        [JsonIgnore] public string Salt { get; set; }

        [JsonIgnore] public string Password { get; set; }

        [Column(Order = 2)] public string Email { get; set; }

        [Column("Power")]
        [JsonIgnore]
        public string PowerJson
        {
            get => JsonConvert.SerializeObject(Power);
            set => Power = JsonConvert.DeserializeObject<UserRights>(value);
        }

        [NotMapped] public UserRights Power { get; set; }

        [JsonIgnore] public virtual List<Player> Players { get; set; } = new();

        [JsonIgnore] public virtual List<RefreshToken> RefreshTokens { get; set; } = new();

        public string PasswordResetCode { get; set; }

        [JsonIgnore] public DateTime? PasswordResetTime { get; set; }

        public DateTime? RegistrationDate { get; set; } = DateTime.UtcNow;

        private ulong mLoadedPlaytime { get; set; }

        public ulong PlayTimeSeconds
        {
            get =>
                mLoadedPlaytime +
                (ulong)(LoginTime != null ? DateTime.UtcNow - (DateTime)LoginTime : TimeSpan.Zero).TotalSeconds;

            set => mLoadedPlaytime = value;
        }

        /// <summary>
        ///     User Variable Values
        /// </summary>
        [JsonIgnore]
        public virtual List<UserVariable> Variables { get; set; } = new();

        [NotMapped] public DateTime? LoginTime { get; set; }

        public string LastIp { get; set; }

        public static User FindOnline(Guid id) => OnlineUsers.ContainsKey(id) ? OnlineUsers[id] : null;

        public static User FindOnline(string username) =>
            OnlineUsers.Values.FirstOrDefault(s => s.Name.ToLower().Trim() == username.ToLower().Trim());

        public static User FindOnlineFromEmail(string email) =>
            OnlineUsers.Values.FirstOrDefault(s => s.Email.ToLower().Trim() == email.ToLower().Trim());

        public static void Login(User user, string ip)
        {
            if (!OnlineUsers.ContainsKey(user.Id))
            {
                OnlineUsers.TryAdd(user.Id, user);
            }

            user.LastIp = ip;
        }

        public void TryLogout(bool softLogout = false)
        {
            //If we still have a character online (probably being held up in combat) then don't logout yet.
            foreach (var chr in Players)
            {
                if (Player.FindOnline(chr.Id) != null)
                {
                    return;
                }
            }

            if (!softLogout && OnlineUsers.ContainsKey(Id))
            {
                OnlineUsers.TryRemove(Id, out var removed);
            }
        }

        public static string GenerateSalt(ushort sizeInBits = 256)
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(sizeInBits >> 3));
        }

        public static string SaltPasswordHash(string passwordHash, string salt)
        {
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(passwordHash.ToUpperInvariant() + salt)));
        }

        public bool IsPasswordValid(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash) || string.IsNullOrWhiteSpace(Salt))
            {
                return false;
            }

            var saltedPasswordHash = SaltPasswordHash(passwordHash, Salt);

            return string.Equals(Password, saltedPasswordHash, StringComparison.Ordinal);
        }

        public bool TryChangePassword(string oldPassword, string newPassword) =>
            IsPasswordValid(oldPassword) && TrySetPassword(newPassword);

        public bool TrySetPassword(string passwordHash)
        {
            var salt = GenerateSalt();
            SaltPasswordHash(passwordHash, salt);

            Salt = salt;
            Password = SaltPasswordHash(passwordHash, salt);

            Save();

            return true;
        }

        public void AddCharacter(Player newCharacter)
        {
            if (newCharacter == null)
            {
                return;
            }

            //No passing in custom contexts here.. they may already have this user in the change tracker and things just get weird.
            //The cost of making a new context is almost nil.
            try
            {
                lock (mSavingLock)
                {
                    using var context = DbInterface.CreatePlayerContext(false);
                    context.Users.Update(this);

                    Players.Add(newCharacter);

                    Player.Validate(newCharacter);

                    context.ChangeTracker.DetectChanges();

                    context.StopTrackingUsersExcept(this);

                    //If we have a new character, intersect already generated the id.. which means the change tracker is gonna see them as modified and not added.. we need to manually set their state
                    context.Entry(newCharacter).State = EntityState.Added;

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to save user while adding character: {Name}");
                ServerContext.DispatchUnhandledException(
                    new Exception("Failed to save user, shutting down to prevent rollbacks!")
                );
            }
        }

        public void DeleteCharacter(Player deleteCharacter)
        {
            if (deleteCharacter == null)
            {
                return;
            }

            //No passing in custom contexts here.. they may already have this user in the change tracker and things just get weird.
            //The cost of making a new context is almost nil.
            try
            {
                lock (mSavingLock)
                {
                    using var context = DbInterface.CreatePlayerContext(false);

                    context.Users.Update(this);

                    Players.Remove(deleteCharacter);

                    context.ChangeTracker.DetectChanges();

                    context.StopTrackingUsersExcept(this);

                    context.Entry(deleteCharacter).State = EntityState.Deleted;

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to save user while deleting character: " + Name);
                ServerContext.DispatchUnhandledException(
                    new Exception("Failed to save user, shutting down to prevent rollbacks!")
                );
            }
        }

        public bool TryDelete()
        {
            //No passing in custom contexts here.. they may already have this user in the change tracker and things just get weird.
            //The cost of making a new context is almost nil.
            try
            {
                lock (mSavingLock)
                {
                    using var context = DbInterface.CreatePlayerContext(false);

                    context.Users.Remove(this);

                    context.ChangeTracker.DetectChanges();

                    context.StopTrackingUsersExcept(this);

                    context.Entry(this).State = EntityState.Deleted;

                    context.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to delete user: " + Name);
                ServerContext.DispatchUnhandledException(
                    new Exception("Failed to delete user, shutting down to prevent rollbacks!")
                );
                return false;
            }
        }

        public void Delete()
        {
            TryDelete();
        }

        public async Task SaveAsync(
            bool force = false,
            PlayerContext? playerContext = default,
            CancellationToken cancellationToken = default
        ) => Save(playerContext, force);

        public void SaveWithDebounce(long debounceMs = 5000)
        {
            lock (_lastSaveLock)
            {
                if (_lastSave < debounceMs + Timing.Global.MillisecondsUtc)
                {
                    Log.Debug("Skipping save due to debounce");
                    return;
                }
            }

            Save();
        }

        public UserSaveResult Save(bool force) => Save(force: force, create: false);

        public UserSaveResult Save(bool force = false, bool create = false) => Save(default, force, create);

#if DIAGNOSTIC
        private int _saveCounter = 0;
#endif

        private UserSaveResult Save(PlayerContext? playerContext, bool force = false, bool create = false)
        {
            lock (_lastSaveLock)
            {
                _lastSave = Timing.Global.MillisecondsUtc;
            }

#if DIAGNOSTIC
            var currentExecutionId = _saveCounter++;
#endif

            //No passing in custom contexts here.. they may already have this user in the change tracker and things just get weird.
            //The cost of making a new context is almost nil.
            var lockTaken = false;
            PlayerContext? createdContext = default;
            try
            {
                if (force)
                {
                    Monitor.Enter(mSavingLock);
                    lockTaken = true;
                }
                else
                {
                    Monitor.TryEnter(mSavingLock, 0, ref lockTaken);
                }

                if (!lockTaken)
                {
#if DIAGNOSTIC
                    Log.Debug($"Failed to take lock {Environment.StackTrace}");
#endif
                    return UserSaveResult.SkippedCouldNotTakeLock;
                }

#if DIAGNOSTIC
                Log.Debug($"DBOP-A Save({playerContext}, {force}, {create}) #{currentExecutionId} {Name} ({Id})");
#endif

                if (playerContext == null)
                {
                    createdContext = DbInterface.CreatePlayerContext(false);
                    playerContext = createdContext;
                }

                if (create)
                {
                    playerContext.Users.Add(this);
                }
                else
                {
                    // playerContext.Attach(this);
                    try
                    {
                        playerContext.Users.Update(this);
                    }
                    catch (InvalidOperationException invalidOperationException)
                    {
                        // ReSharper disable once ConstantConditionalAccessQualifier
                        // ReSharper disable once ConstantNullCoalescingCondition
                        if (invalidOperationException.Message?.Contains("Collection was modified") ?? false)
                        {
                            try
                            {
                                playerContext.Users.Update(this);
                                Log.Warn(invalidOperationException, $"Successfully recovered from {nameof(InvalidOperationException)}");
                            }
                            catch (Exception exception)
                            {
                                throw new AggregateException(
                                    $"Failed to recover from {nameof(InvalidOperationException)}",
                                    invalidOperationException,
                                    exception
                                );
                            }
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                playerContext.ChangeTracker.DetectChanges();

                playerContext.StopTrackingUsersExcept(this);

                if (UserBan != null)
                {
                    playerContext.Entry(UserBan).State = EntityState.Detached;
                }

                if (UserMute != null)
                {
                    playerContext.Entry(UserMute).State = EntityState.Detached;
                }

                playerContext.SaveChanges();

#if DIAGNOSTIC
                Log.Debug($"DBOP-B Save({playerContext}, {force}, {create}) #{currentExecutionId} {Name} ({Id})");
#endif

                return UserSaveResult.Completed;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var concurrencyErrors = new StringBuilder();
                foreach (var entry in ex.Entries)
                {
                    var type = entry.GetType().FullName;
                    concurrencyErrors.AppendLine($"Entry Type [{type} / {entry.State}]");
                    concurrencyErrors.AppendLine("--------------------");
                    concurrencyErrors.AppendLine($"Type: {entry.Entity.GetFullishName()}");

                    var proposedValues = entry.CurrentValues;
                    var databaseValues = entry.GetDatabaseValues();

                    var propertyNameColumnSize = proposedValues.Properties.Max(property => property.Name.Length);

                    foreach (var property in proposedValues.Properties)
                    {
                        concurrencyErrors.AppendLine(
                            $"\t{property.Name:propertyNameColumnSize} (Token: {property.IsConcurrencyToken}): Proposed: {proposedValues[property]}  Original Value: {entry.OriginalValues[property]}  Database Value: {(databaseValues != null ? databaseValues[property] : "null")}"
                        );
                    }

                    concurrencyErrors.AppendLine("");
                    concurrencyErrors.AppendLine("");
                }

                var suffix = string.Empty;
#if DIAGNOSTIC
                suffix = $"#{currentExecutionId}";
#endif
                Log.Error(ex, $"Jackpot! Concurrency Bug For {Name} in {(createdContext == default ? "Existing" : "Created")} Context {suffix}");
                Log.Error(concurrencyErrors.ToString());

#if DIAGNOSTIC
                Log.Debug($"DBOP-C Save({playerContext}, {force}, {create}) #{currentExecutionId} {Name} ({Id})");
#endif

                ServerContext.DispatchUnhandledException(
                    new Exception("Failed to save user, shutting down to prevent rollbacks!")
                );
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to save user: " + Name);

#if DIAGNOSTIC
                Log.Debug($"DBOP-C Save({playerContext}, {force}, {create}) #{currentExecutionId} {Name} ({Id})");
#endif
                ServerContext.DispatchUnhandledException(
                    new Exception("Failed to save user, shutting down to prevent rollbacks!")
                );
            }
            finally
            {
                if (lockTaken)
                {
                    createdContext?.Dispose();
                    Monitor.Exit(mSavingLock);
                }
            }

            return UserSaveResult.Failed;
        }

        [return: NotNullIfNotNull(nameof(user))]
        public static User? PostLoad(User? user, PlayerContext? playerContext = default)
        {
            if (user == default)
            {
                return user;
            }

            if (playerContext == default)
            {
                using var context = DbInterface.CreatePlayerContext();
                if (context == default)
                {
                    throw new InvalidOperationException();
                }

                // ReSharper disable once TailRecursiveCall
                return PostLoad(user, context);
            }

            var entityEntry = playerContext.Users.Attach(user);
            entityEntry.Collection(u => u.Variables).Load();

            return user;
        }

        public static Tuple<Client, User> Fetch(Guid userId)
        {
            var client = Globals.Clients.Find(queryClient => userId == queryClient?.User?.Id);

            return new Tuple<Client, User>(client, client?.User ?? FindById(userId));
        }

        public static Tuple<Client, User> Fetch(string userName)
        {
            var client = Globals.Clients.Find(queryClient => Entity.CompareName(userName, queryClient?.User?.Name));

            return new Tuple<Client, User>(client, client?.User ?? Find(userName));
        }

        public static User TryLogin(string username, string ptPassword)
        {
            var user = FindOnline(username);
            if (user != null)
            {
                var hashedPassword = SaltPasswordHash(ptPassword, user.Salt);
                if (string.Equals(user.Password, hashedPassword, StringComparison.Ordinal))
                {
                    return PostLoad(user);
                }
            }
            else
            {
                try
                {
                    using var context = DbInterface.CreatePlayerContext();
                    var salt = GetUserSalt(username);
                    if (!string.IsNullOrWhiteSpace(salt))
                    {
                        var pass = SaltPasswordHash(ptPassword, salt);
                        var queriedUser = QueryUserByNameAndPasswordShallow(context, username, pass);
                        return PostLoad(queriedUser, context);
                    }
                }
                catch (Exception exception)
                {
                    Log.Error(exception);
                }
            }

            return null;
        }

        public static User FindById(Guid userId)
        {
            using var playerContext = DbInterface.CreatePlayerContext();
            return FindById(userId, playerContext);
        }

        public static User FindById(Guid userId, PlayerContext playerContext)
        {
            if (userId == Guid.Empty)
            {
                return null;
            }

            var user = FindOnline(userId);

            if (user != null)
            {
                return user;
            }

            try
            {
                using var context = DbInterface.CreatePlayerContext();
                return QueryUserByIdShallow(playerContext, userId);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        public static User Find(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            var user = FindOnline(username);

            if (user != null)
            {
                return user;
            }

            try
            {
                using var context = DbInterface.CreatePlayerContext();
                var queriedUser = QueryUserByNameShallow(context, username);
                return queriedUser;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        public static User FindFromNameOrEmail(string nameOrEmail)
        {
            using var playerContext = DbInterface.CreatePlayerContext();
            return FindByNameOrEmail(nameOrEmail, playerContext);
        }

        public static User FindByNameOrEmail(string nameOrEmail, PlayerContext playerContext)
        {
            if (string.IsNullOrWhiteSpace(nameOrEmail))
            {
                return null;
            }

            var user = FindOnlineFromEmail(nameOrEmail);
            if (user != null)
            {
                return user;
            }

            user = FindOnline(nameOrEmail);
            if (user != null)
            {
                return user;
            }

            try
            {
                var queriedUser = QueryUserByNameOrEmailShallow(playerContext, nameOrEmail);
                return queriedUser;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        public static User FindByEmail(string email)
        {
            using var context = DbInterface.CreatePlayerContext();
            return FindByEmail(email);
        }

        public static User FindByEmail(string email, PlayerContext playerContext)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            var user = FindOnlineFromEmail(email);

            if (user != null)
            {
                return user;
            }

            try
            {
                return QueryUserByEmailShallow(playerContext, email);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        public static string GetUserSalt(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return null;
            }

            var user = FindOnline(userName);
            if (user != null)
            {
                return user.Salt;
            }

            try
            {
                using (var context = DbInterface.CreatePlayerContext())
                {
                    return SaltByName(context, userName);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        public static bool UserExists(string nameOrEmail)
        {
            if (string.IsNullOrWhiteSpace(nameOrEmail))
            {
                return false;
            }

            var user = FindOnlineFromEmail(nameOrEmail);
            if (user != null)
            {
                return true;
            }

            user = FindOnline(nameOrEmail);
            if (user != null)
            {
                return true;
            }

            try
            {
                using (var context = DbInterface.CreatePlayerContext())
                {
                    return AnyUserByNameOrEmail(context, nameOrEmail);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// </summary>
        public void Update()
        {
            if (UpdatedVariables.Count > 0)
            {
                Save();
                UpdatedVariables.Clear();
            }
        }

        /// <summary>
        ///     Returns a variable object given a user variable id
        /// </summary>
        /// <param name="id">Variable id</param>
        /// <param name="createIfNull">Creates this variable for the user if it hasn't been set yet</param>
        /// <returns></returns>
        public Variable GetVariable(Guid id, bool createIfNull = false)
        {
            foreach (var v in Variables)
            {
                if (v.VariableId == id)
                {
                    return v;
                }
            }

            if (createIfNull)
            {
                return CreateVariable(id);
            }

            return null;
        }

        /// <summary>
        ///     Creates a variable for this user with a given id if it doesn't already exist
        /// </summary>
        /// <param name="id">Variablke id</param>
        /// <returns></returns>
        private Variable CreateVariable(Guid id)
        {
            if (UserVariableBase.Get(id) == null)
            {
                return null;
            }

            var variable = new UserVariable(id);
            Variables.Add(variable);

            return variable;
        }

        /// <summary>
        ///     Gets the value of a account variable given a variable id
        /// </summary>
        /// <param name="id">Variable id</param>
        /// <returns></returns>
        public VariableValue GetVariableValue(Guid id)
        {
            var v = GetVariable(id, true);

            if (v == null)
            {
                return new VariableValue();
            }

            return v.Value;
        }

        /// <summary>
        ///     Starts all common events with a specified trigger for any character online of this account
        /// </summary>
        /// <param name="trigger">The common event trigger to run</param>
        /// <param name="command">The command which started this common event</param>
        /// <param name="param">Common event parameter</param>
        public void StartCommonEventsWithTriggerForAll(CommonEventTrigger trigger, string command, string param)
        {
            foreach (var plyr in Players)
            {
                if (Player.FindOnline(plyr.Id) != null)
                {
                    plyr.StartCommonEventsWithTrigger(trigger, command, param);
                }
            }
        }

        public static bool TryRegister(
            RegistrationActor actor,
            string username,
            string email,
            string password,
            [NotNullWhen(false)] out string error,
            [NotNullWhen(true)] out User user
        )
        {
            error = default;
            user = default;

            if (Options.BlockClientRegistrations)
            {
                error = Strings.Account.registrationsblocked;
                return false;
            }

            if (!FieldChecking.IsValidUsername(username, Strings.Regex.username))
            {
                error = Strings.Account.invalidname;
                return false;
            }

            if (!FieldChecking.IsWellformedEmailAddress(email, Strings.Regex.email))
            {
                error = Strings.Account.invalidemail;
                return false;
            }

            if (Ban.IsBanned(actor.IpAddress, out var message))
            {
                error = message;
                return false;
            }

            if (UserExists(username))
            {
                error = Strings.Account.exists;
                return false;
            }

            if (UserExists(email))
            {
                error = Strings.Account.emailexists;
                return false;
            }

            UserActivityHistory.LogActivity(
                Guid.Empty,
                Guid.Empty,
                actor.IpAddress.ToString(),
                actor.PeerType,
                UserActivityHistory.UserAction.Create,
                string.Empty
            );

            if (DbInterface.TryRegister(username, email, password, out user))
            {
                return true;
            }

            error = Strings.Account.UnknownError;
            return false;
        }

        public sealed record RegistrationActor(IPAddress IpAddress, UserActivityHistory.PeerType PeerType);

        #region Instance Variables

        [NotMapped]
        [ApiVisibility(ApiVisibility.Restricted | ApiVisibility.Private)]
        public bool IsBanned => Ban != null;

        [NotMapped]
        [ApiVisibility(ApiVisibility.Restricted | ApiVisibility.Private)]
        public bool IsMuted => Mute != null;

        [ApiVisibility(ApiVisibility.Restricted)]
        public Ban Ban
        {
            get => UserBan ?? IpBan;
            set => UserBan = value;
        }

        [ApiVisibility(ApiVisibility.Restricted)]
        public Mute Mute
        {
            get => UserMute ?? IpMute;
            set => UserMute = value;
        }

        [NotMapped] public Ban IpBan { get; set; }

        [NotMapped] public Mute IpMute { get; set; }

        [ApiVisibility(ApiVisibility.Restricted)]
        [NotMapped]
        public Ban UserBan { get; set; }

        [ApiVisibility(ApiVisibility.Restricted)]
        [NotMapped]
        public Mute UserMute { get; set; }

        #endregion

        #region Listing

        public static int Count()
        {
            using (var context = DbInterface.CreatePlayerContext())
            {
                return context.Users.Count();
            }
        }

        public static IList<User> List(
            string query,
            string sortBy,
            SortDirection sortDirection,
            int skip,
            int take,
            out int total
        )
        {
            try
            {
                using (var context = DbInterface.CreatePlayerContext())
                {
                    foreach (var user in Globals.OnlineList.Select(p => p.User))
                    {
                        if (user != default)
                        {
                            context.Entry(user).State = EntityState.Unchanged;
                        }
                    }

                    var compiledQuery = string.IsNullOrWhiteSpace(query)
                        ? context.Users.Include(p => p.Ban).Include(p => p.Mute) : context.Users.Where(
                            u => EF.Functions.Like(u.Name, $"%{query}%") || EF.Functions.Like(u.Email, $"%{query}%")
                        );

                    total = compiledQuery.Count();

                    switch (sortBy?.ToLower() ?? "")
                    {
                        case "email":
                            compiledQuery = sortDirection == SortDirection.Ascending
                                ? compiledQuery.OrderBy(u => u.Email.ToUpper())
                                : compiledQuery.OrderByDescending(u => u.Email.ToUpper());
                            break;
                        case "registrationdate":
                            compiledQuery = sortDirection == SortDirection.Ascending
                                ? compiledQuery.OrderBy(u => u.RegistrationDate)
                                : compiledQuery.OrderByDescending(u => u.RegistrationDate);
                            break;
                        case "playtime":
                            compiledQuery = sortDirection == SortDirection.Ascending
                                ? compiledQuery.OrderBy(u => u.PlayTimeSeconds)
                                : compiledQuery.OrderByDescending(u => u.PlayTimeSeconds);
                            break;
                        case "name":
                        default:
                            compiledQuery = sortDirection == SortDirection.Ascending
                                ? compiledQuery.OrderBy(u => u.Name.ToUpper())
                                : compiledQuery.OrderByDescending(u => u.Name.ToUpper());
                            break;
                    }

                    var users = compiledQuery.Skip(skip).Take(take).AsTracking().ToList();
                    return users;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                total = 0;
                return null;
            }
        }

        #endregion

        #region Compiled Queries

        private static readonly Func<PlayerContext, string, User> QueryUserByNameShallow = EF.CompileQuery(
                // ReSharper disable once SpecifyStringComparison
                (PlayerContext context, string username) => context.Users.Where(u => u.Name == username)
                    .Include(p => p.Ban)
                    .Include(p => p.Mute)
                    .Include(p => p.Players)
                    .AsSplitQuery()
                    .FirstOrDefault()
            ) ??
            throw new InvalidOperationException();

        private static readonly Func<PlayerContext, string, User> QueryUserByNameOrEmailShallow = EF.CompileQuery(
                // ReSharper disable once SpecifyStringComparison
                (PlayerContext context, string usernameOrEmail) => context.Users
                    .Where(u => u.Name == usernameOrEmail || u.Email == usernameOrEmail)
                    .Include(p => p.Ban)
                    .Include(p => p.Mute)
                    .Include(p => p.Players)
                    .AsSplitQuery()
                    .FirstOrDefault()
            ) ??
            throw new InvalidOperationException();

        private static readonly Func<PlayerContext, string, string, User> QueryUserByNameAndPasswordShallow =
            EF.CompileQuery(
                // ReSharper disable once SpecifyStringComparison
                (PlayerContext context, string username, string password) => context.Users
                    .Where(u => u.Name == username && u.Password == password)
                    .Include(p => p.Ban)
                    .Include(p => p.Mute)
                    .Include(p => p.Players)
                    .AsSplitQuery()
                    .FirstOrDefault()
            ) ??
            throw new InvalidOperationException();

        private static readonly Func<PlayerContext, Guid, User> QueryUserByIdShallow = EF.CompileQuery(
                (PlayerContext context, Guid id) => context.Users.Where(u => u.Id == id)
                    .Include(p => p.Ban)
                    .Include(p => p.Mute)
                    .Include(p => p.Players)
                    .AsSplitQuery()
                    .FirstOrDefault()
            ) ??
            throw new InvalidOperationException();

        private static readonly Func<PlayerContext, string, bool> AnyUserByNameOrEmail = EF.CompileQuery(
            // ReSharper disable once SpecifyStringComparison
            (PlayerContext context, string nameOrEmail) =>
                context.Users.Where(u => u.Name == nameOrEmail || u.Email == nameOrEmail).Any()
        );

        private static readonly Func<PlayerContext, string, string> SaltByName = EF.CompileQuery(
            // ReSharper disable once SpecifyStringComparison
            (PlayerContext context, string userName) =>
                context.Users.Where(u => u.Name == userName).Select(u => u.Salt).FirstOrDefault()
        );

        private static readonly Func<PlayerContext, string, User> QueryUserByEmailShallow = EF.CompileQuery(
                // ReSharper disable once SpecifyStringComparison
                (PlayerContext context, string email) => context.Users.Where(u => u.Email == email)
                    .Include(p => p.Ban)
                    .Include(p => p.Mute)
                    .Include(p => p.Players)
                    .AsSplitQuery()
                    .FirstOrDefault()
        ) ??
        throw new InvalidOperationException();

        #endregion
    }

}
