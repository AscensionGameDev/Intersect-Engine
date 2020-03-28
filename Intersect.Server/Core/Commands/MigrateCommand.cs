using System;

using Intersect.Config;
using Intersect.Logging;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Database;
using Intersect.Server.Localization;

namespace Intersect.Server.Core.Commands
{

    internal sealed class MigrateCommand : ServerCommand
    {

        public MigrateCommand() : base(Strings.Commands.Migrate)
        {
        }

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            var input = "";
            var selection = ' ';
            Console.WriteLine();
            Console.WriteLine(Strings.Migration.selectdb);
            Console.WriteLine();
            Console.WriteLine(
                Strings.Migration.selectgamedb.ToString(
                    Options.GameDb.Type == DatabaseOptions.DatabaseType.sqlite
                        ? Strings.Migration.currentlysqlite
                        : Strings.Migration.currentlymysql
                )
            );

            Console.WriteLine(
                Strings.Migration.selectplayerdb.ToString(
                    Options.PlayerDb.Type == DatabaseOptions.DatabaseType.sqlite
                        ? Strings.Migration.currentlysqlite
                        : Strings.Migration.currentlymysql
                )
            );

            Console.WriteLine();
            Console.WriteLine(Strings.Migration.cancel);

            // TODO: Remove > when moving to ReadKeyWait when console magic is ready
            Console.Write("> ");
            input = Console.ReadLine();
            selection = input.Length > 0 ? input[0] : ' ';
            Console.WriteLine();
            DatabaseOptions db;
            if (selection.ToString() == Strings.Migration.selectgamedbkey.ToString())
            {
                db = Options.GameDb;
            }
            else if (selection.ToString() == Strings.Migration.selectplayerdbkey.ToString())
            {
                db = Options.PlayerDb;
            }
            else
            {
                Console.WriteLine(Strings.Migration.migrationcancelled);

                return;
            }

            var dbString = db == Options.GameDb ? Strings.Migration.gamedb : Strings.Migration.playerdb;
            Console.WriteLine();
            Console.WriteLine(Strings.Migration.selectdbengine.ToString(dbString));
            Console.WriteLine(Strings.Migration.migratetosqlite);
            Console.WriteLine(Strings.Migration.migratetomysql);
            Console.WriteLine();
            Console.WriteLine(Strings.Migration.cancel);

            // TODO: Remove > when moving to ReadKeyWait when console magic is ready
            Console.Write("> ");
            input = Console.ReadLine();
            selection = input.Length > 0 ? input[0] : ' ';
            Console.WriteLine();
            var dbengine = DatabaseOptions.DatabaseType.sqlite;
            if (selection.ToString() != Strings.Migration.selectsqlitekey.ToString() &&
                selection.ToString() != Strings.Migration.selectmysqlkey.ToString())
            {
                Console.WriteLine(Strings.Migration.migrationcancelled);

                return;
            }

            if (selection.ToString() == Strings.Migration.selectmysqlkey.ToString())
            {
                dbengine = DatabaseOptions.DatabaseType.mysql;
            }

            if (db.Type == dbengine)
            {
                var engineString = dbengine == DatabaseOptions.DatabaseType.sqlite
                    ? Strings.Migration.sqlite
                    : Strings.Migration.mysql;

                Console.WriteLine();
                Console.WriteLine(Strings.Migration.alreadyusingengine.ToString(dbString, engineString));
                Console.WriteLine();
            }
            else
            {
                try
                {
                    DbInterface.Migrate(db, dbengine);
                }
                catch (Exception exception)
                {
                    Log.Error(exception);
                }
            }
        }

    }

}
