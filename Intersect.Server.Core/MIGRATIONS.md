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
- MySQL (`MySql` in code, see [Intersect.Server/MIGRATIONS.md](../Intersect.Server/MIGRATIONS.md) for more details)
- SQLite (`Sqlite` in code)

## Generating Migrations

**_When running migrations in Intersect.Server.Core do not omit `--startup-project ../Intersect.Server/`_**

**_Do not omit the `--namespace` or `--output-dir` parameters, `dotnet ef` [still has a bug open at the time of writing](https://github.com/dotnet/efcore/issues/24339) and may not generate the `*ModelSnapshot.cs` files correctly if you don't_**

See the provider-specific sections for concrete examples, but the general command pattern is:

`dotnet ef migrations add <migration name> --context <provider><context full> --namespace Intersect.Server.Migrations.<provider>.<context root> --output-dir Migrations/<provider>/<context root>/ -- --databaseType <provider> [--connectionString "..."]`

### SQLite Migrations

General command pattern is:

`dotnet ef migrations add <migration name> --startup-project ../Intersect.Server/ --context Sqlite<context full> --namespace Intersect.Server.Migrations.Sqlite.<context root> --output-dir Migrations/Sqlite/<context root>/ -- --databaseType Sqlite`

Example:

`dotnet ef migrations add Net7Upgrade --startup-project ../Intersect.Server/ --context SqlitePlayerContext --namespace Intersect.Server.Migrations.Sqlite.Player `
