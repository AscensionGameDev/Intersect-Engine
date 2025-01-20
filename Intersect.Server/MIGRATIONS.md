# Database Migrations

## Prerequisites
- Install `dotnet-ef` via `dotnet tool install --global dotnet-ef --version 7.*` ([official documentation](https://learn.microsoft.com/en-us/ef/core/cli/dotnet#installing-the-tools))
	- On Unix-like systems you might get an error like `zsh: no matches found: 7.*`, if so run: `dotnet tool install --global dotnet-ef --version 7.\*` (note the `\\` before the `*`)
	- If you get warnings like `The Entity Framework tools version '7.0.11' is older than that of the runtime '7.0.13'. Update the tools for the latest features and bug fixes. See https://aka.ms/AAc1fbw for more information.`
		you can run: `dotnet tool update --global dotnet-ef --version 7.0.13` or `dotnet tool update --global dotnet-ef --version 7.*` (mind the `*` on Unix-like systems)

## Supported Contexts
- Game (root `Game`, `GameContext` in code)
- Logging (root `Logging`, `LoggingContext` in code)
- Player (root `Player`, `PlayerContext` in code)

## Supported Database Providers
- MySQL (`MySql` in code)
- SQLite (`Sqlite` in code, see [Intersect.Server.Core/MIGRATIONS.md](../Intersect.Server.Core/MIGRATIONS.md) for more details)

## Generating Migrations

**_Do not omit the `--namespace` or `--output-dir` parameters, `dotnet ef` [still has a bug open at the time of writing](https://github.com/dotnet/efcore/issues/24339) and may not generate the `*ModelSnapshot.cs` files correctly if you don't_**

See the provider-specific sections for concrete examples, but the general command pattern is:

`dotnet ef migrations add <migration name> --context <provider><context full> --namespace Intersect.Server.Migrations.<provider>.<context root> --output-dir Migrations/<provider>/<context root>/ -- --databaseType <provider> [--connectionString "..."]`

### MariaDB/MySQL Migrations

**_Connection strings are required for generating MariaDB/MySQL migrations_**

**_MariaDB and MySQL are interchangeable as database providers but we refer to them both as `MySql` as far as migrations are concerned, please adhere to that convention when generating migrations_**

General command pattern is:

`dotnet ef migrations add <migration name> --context MySql<context> --namespace Intersect.Server.Migrations.MySql.<context root name> --output-dir Migrations/MySql/<context root name>/ -- --databaseType MySql --connectionString "..."`

Example:

`dotnet ef migrations add Net7Upgrade --context MySqlLoggingContext --namespace Intersect.Server.Migrations.MySql.Logging --output-dir Migrations/MySql/Logging/ -- --databaseType MySql --connectionString "Username=<username>;Password=<password>;Database=<a db that exists>"`

**Additionally, when trying to generate a migration on Windows you may need to include `Server=localhost` in your connection string.**

**_A connection string is required for generating MariaDB/MySQL migrations_**, if you fail to provide one (or provide an invalid one) it will crash and fail with something like the following:

`Unable to create a 'DbContext' of type 'MySqlGameContext'. For the different patterns supported at design time, see https://go.microsoft.com/fwlink/?linkid=851728`

<details>
	<summary>
		(Click to see full stacktrace when using <code>--verbose</code>)
	</summary>
	<code>
		Microsoft.EntityFrameworkCore.Design.OperationException: Unable to create an object of type 'MySqlPlayerContext'. For the different patterns supported at design time, see https://go.microsoft.com/fwlink/?linkid=851728
		---> MySqlConnector.MySqlException (0x80004005): Access denied for user ''@'172.25.0.1' (using password: NO)
			at MySqlConnector.Core.ServerSession.SwitchAuthenticationAsync(ConnectionSettings cs, String password, PayloadData payload, IOBehavior ioBehavior, CancellationToken cancellationToken) in /_/src/MySqlConnector/Core/ServerSession.cs:line 731
			at MySqlConnector.Core.ServerSession.ConnectAsync(ConnectionSettings cs, MySqlConnection connection, Int32 startTickCount, ILoadBalancer loadBalancer, Activity activity, IOBehavior ioBehavior, CancellationToken cancellationToken) in /_/src/MySqlConnector/Core/ServerSession.cs:line 573
			at MySqlConnector.Core.ConnectionPool.ConnectSessionAsync(MySqlConnection connection, String logMessage, Int32 startTickCount, Activity activity, IOBehavior ioBehavior, CancellationToken cancellationToken) in /_/src/MySqlConnector/Core/ConnectionPool.cs:line 403
			at MySqlConnector.Core.ConnectionPool.ConnectSessionAsync(MySqlConnection connection, String logMessage, Int32 startTickCount, Activity activity, IOBehavior ioBehavior, CancellationToken cancellationToken) in /_/src/MySqlConnector/Core/ConnectionPool.cs:line 408
			at MySqlConnector.Core.ConnectionPool.GetSessionAsync(MySqlConnection connection, Int32 startTickCount, Activity activity, IOBehavior ioBehavior, CancellationToken cancellationToken) in /_/src/MySqlConnector/Core/ConnectionPool.cs:line 98
			at MySqlConnector.Core.ConnectionPool.GetSessionAsync(MySqlConnection connection, Int32 startTickCount, Activity activity, IOBehavior ioBehavior, CancellationToken cancellationToken) in /_/src/MySqlConnector/Core/ConnectionPool.cs:line 128
			at MySqlConnector.MySqlConnection.CreateSessionAsync(ConnectionPool pool, Int32 startTickCount, Activity activity, Nullable`1 ioBehavior, CancellationToken cancellationToken) in /_/src/MySqlConnector/MySqlConnection.cs:line 929
			at MySqlConnector.MySqlConnection.OpenAsync(Nullable`1 ioBehavior, CancellationToken cancellationToken) in /_/src/MySqlConnector/MySqlConnection.cs:line 423
			at MySqlConnector.MySqlConnection.Open() in /_/src/MySqlConnector/MySqlConnection.cs:line 382
			at Microsoft.EntityFrameworkCore.ServerVersion.AutoDetect(String connectionString)
			at Intersect.Server.Database.IntersectDbContext`1.OnConfiguring(DbContextOptionsBuilder optionsBuilder) in /home/me/git/AscensionGameDev/engine/main/Intersect.Server/Database/IntersectDbContext.cs:line 72
			at Microsoft.EntityFrameworkCore.DbContext.get_ContextServices()
			at Microsoft.EntityFrameworkCore.DbContext.get_InternalServiceProvider()
			at Microsoft.EntityFrameworkCore.DbContext.get_ChangeTracker()
			at Intersect.Server.Database.IntersectDbContext`1..ctor(DatabaseContextOptions databaseContextOptions) in /home/me/git/AscensionGameDev/engine/main/Intersect.Server/Database/IntersectDbContext.Instantiation.cs:line 37
			at Intersect.Server.Database.PlayerData.PlayerContext..ctor(DatabaseContextOptions databaseContextOptions) in /home/me/git/AscensionGameDev/engine/main/Intersect.Server/Database/PlayerData/PlayerContext.cs:line 37
			at Intersect.Server.Database.PlayerData.MySqlPlayerContext..ctor(DatabaseContextOptions databaseContextOptions) in /home/me/git/AscensionGameDev/engine/main/Intersect.Server/Database/PlayerData/PlayerContext.cs:line 18
			at System.RuntimeMethodHandle.InvokeMethod(Object target, Void** arguments, Signature sig, Boolean isConstructor)
			at System.Reflection.ConstructorInvoker.Invoke(Object obj, IntPtr* args, BindingFlags invokeAttr)
			at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
			at Microsoft.Extensions.DependencyInjection.ActivatorUtilities.ConstructorMatcher.CreateInstance(IServiceProvider provider)
			at Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance(IServiceProvider provider, Type instanceType, Object[] parameters)
			at Microsoft.EntityFrameworkCore.Design.Internal.DbContextOperations.&lt;&gt;c__DisplayClass21_4.&lt;FindContextTypes&gt;b__13()
			--- End of inner exception stack trace ---
			at Microsoft.EntityFrameworkCore.Design.Internal.DbContextOperations.&lt;&gt;c__DisplayClass21_4.&lt;FindContextTypes&gt;b__13()
			at Microsoft.EntityFrameworkCore.Design.Internal.DbContextOperations.CreateContext(Func`1 factory)
			at Microsoft.EntityFrameworkCore.Design.Internal.DbContextOperations.CreateContext(String contextType)
			at Microsoft.EntityFrameworkCore.Design.Internal.MigrationsOperations.AddMigration(String name, String outputDir, String contextType, String namespace)
			at Microsoft.EntityFrameworkCore.Design.OperationExecutor.AddMigrationImpl(String name, String outputDir, String contextType, String namespace)
			at Microsoft.EntityFrameworkCore.Design.OperationExecutor.AddMigration.&lt;&gt;c__DisplayClass0_0.&lt;.ctor&gt;b__0()
			at Microsoft.EntityFrameworkCore.Design.OperationExecutor.OperationBase.&lt;&gt;c__DisplayClass3_0`1.&lt;Execute&gt;b__0()
			at Microsoft.EntityFrameworkCore.Design.OperationExecutor.OperationBase.Execute(Action action)
		Unable to create an object of type 'MySqlPlayerContext'. For the different patterns supported at design time, see https://go.microsoft.com/fwlink/?linkid=851728
	</code>
</details>
