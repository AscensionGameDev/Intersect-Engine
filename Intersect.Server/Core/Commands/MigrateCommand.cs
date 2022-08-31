using Intersect.Config;
using Intersect.Logging;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Database;
using Intersect.Server.Database.GameData;
using Intersect.Server.Database.Logging;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Localization;

namespace Intersect.Server.Core.Commands;

internal sealed partial class MigrateCommand : ServerCommand
{
    public MigrateCommand() : base(Strings.Commands.Migrate)
    {
    }

    protected override void HandleValue(ServerContext context, ParserResult result)
    {
        var databases = new List<(Type, DatabaseOptions, string)>
        {
            (typeof(GameContext), Options.GameDb, Strings.Migration.gamedb),
            (typeof(PlayerContext), Options.PlayerDb, Strings.Migration.playerdb),
            (typeof(LoggingContext), Options.LoggingDb, Strings.Migration.LoggingDatabaseName)
        };

        Console.WriteLine();
        Console.WriteLine(Strings.Migration.selectdb);
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
        Console.WriteLine(Strings.Migration.cancel);

        // TODO: Remove > when moving to ReadKeyWait when console magic is ready
        Console.Write("> ");
        var input = Console.ReadLine();
        Console.WriteLine();

        if (!int.TryParse(input, out var selectedDatabaseIndex))
        {
            Console.WriteLine(Strings.Migration.migrationcancelled);
            return;
        }

        if (selectedDatabaseIndex < 1 || selectedDatabaseIndex > databases.Count)
        {
            Console.WriteLine(Strings.Migration.migrationcancelled);
            return;
        }

        var (selectedContextType, selectedOptions, selectedDatabaseName) = databases[selectedDatabaseIndex];

        var databaseTypes = new List<DatabaseType> { DatabaseType.Sqlite, DatabaseType.MySql };

        Console.WriteLine();
        Console.WriteLine(Strings.Migration.selectdbengine.ToString(selectedDatabaseName));
        foreach (var databaseType in databaseTypes)
        {
            Console.WriteLine(Strings.Migration.SelectDatabaseType.ToString(databaseType.GetName()));
        }

        Console.WriteLine();
        Console.WriteLine(Strings.Migration.cancel);

        // TODO: Remove > when moving to ReadKeyWait when console magic is ready
        Console.Write("> ");
        input = Console.ReadLine();
        Console.WriteLine();

        if (!int.TryParse(input, out var selectedDatabaseTypeIndex))
        {
            Console.WriteLine(Strings.Migration.migrationcancelled);
            return;
        }

        if (selectedDatabaseTypeIndex < 1 || selectedDatabaseTypeIndex > databaseTypes.Count)
        {
            Console.WriteLine(Strings.Migration.migrationcancelled);
            return;
        }

        var selectedDatabaseType = databaseTypes[selectedDatabaseTypeIndex - 1];
        if (selectedDatabaseType == selectedOptions.Type)
        {
            Console.WriteLine();
            Console.WriteLine(
                Strings.Migration.alreadyusingengine.ToString(selectedDatabaseName, selectedDatabaseType.GetName()));
            Console.WriteLine(Strings.Migration.migrationcancelled);
            return;
        }


        try
        {
            Task task;
            if (selectedContextType == typeof(GameContext))
            {
                task = DbInterface.Migrate<GameContext>(selectedOptions, selectedDatabaseType);
            }
            else if (selectedContextType == typeof(PlayerContext))
            {
                task = DbInterface.Migrate<PlayerContext>(selectedOptions, selectedDatabaseType);
            }
            else if (selectedContextType == typeof(LoggingContext))
            {
                task = DbInterface.Migrate<LoggingContext>(selectedOptions, selectedDatabaseType);
            }
            else
            {
                throw new InvalidOperationException();
            }

            task.ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }
        catch (Exception exception)
        {
            Log.Error(exception);
        }
    }
}
