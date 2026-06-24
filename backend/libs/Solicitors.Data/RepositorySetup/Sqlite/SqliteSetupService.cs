using Microsoft.EntityFrameworkCore;

namespace Solicitors.Data.RepositorySetup.Sqlite;

internal class SqliteSetupService(SqliteDbSetupOptions config) : IRepoSetupService
{
    public void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={config.DbPath}");
    }
}