using Microsoft.EntityFrameworkCore;

namespace Solicitors.Data.RepositorySetup.InMemory;

internal class InMemorySetupService(InMemoryDbSetupOptions config) : IRepoSetupService
{
    public void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseInMemoryDatabase(config.DbName);
    }
}