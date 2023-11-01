# Database Migrations

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
