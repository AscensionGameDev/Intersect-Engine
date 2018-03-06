using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Classes.Database.GameData
{
    public class GameContext : DbContext
    {

        private DatabaseUtils.DbProvider mConnection = DatabaseUtils.DbProvider.Sqlite;
        private string mConnectionString = @"Data Source=resources/gamedata.db";

        public GameContext()
        {

        }

        public GameContext(DatabaseUtils.DbProvider connection, string connectionString)
        {
            mConnection = connection;
            mConnectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch (mConnection)
            {
                case DatabaseUtils.DbProvider.Sqlite:
                    optionsBuilder.UseSqlite(mConnectionString);
                    break;
                case DatabaseUtils.DbProvider.MySql:
                    optionsBuilder.UseMySql(mConnectionString);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

    }
}
