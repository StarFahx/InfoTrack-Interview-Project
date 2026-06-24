namespace Solicitors.Data.RepositorySetup.InMemory;

public class InMemoryDbSetupOptions(string dbName) : IRepoSetupOptions
{
    public string DbName { get; } = dbName;
}