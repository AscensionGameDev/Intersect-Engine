using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Database;

public static class MySqlDbContextExtensions
{
    public static void ConfigureOptionsBuilder(
        this IMySqlDbContext context,
        DbContextOptionsBuilder optionsBuilder,
        string connectionString
    )
    {
        optionsBuilder.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString),
            options => options
                .EnableRetryOnFailure(
                    5,
                    TimeSpan.FromSeconds(12),
                    default
                )
        );
    }
}