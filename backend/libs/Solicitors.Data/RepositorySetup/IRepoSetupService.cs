using Microsoft.EntityFrameworkCore;

namespace Solicitors.Data.RepositorySetup;

internal interface IRepoSetupService
{
    void OnConfiguring(DbContextOptionsBuilder optionsBuilder);
}