namespace Solicitors.Data.RepositorySetup.Sqlite;

public class SqliteDbSetupOptions(string dbPath) : IRepoSetupOptions
{
    public string DbPath { get; } = dbPath;
}