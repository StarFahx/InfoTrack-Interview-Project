using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Solicitors.Core.Data;
using Solicitors.Data.RepositorySetup;
using Solicitors.Data.RepositorySetup.InMemory;
using Solicitors.Data.RepositorySetup.Sqlite;

namespace Solicitors.Data;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class DIExtensions
{
    public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddScoped<SolicitorsRepository>()
            .AddScoped<ISolicitorRepository>(sp => sp.GetRequiredService<SolicitorsRepository>())
            .AddScoped<IReadOnlySolicitorRepository>(sp => sp.GetRequiredService<ISolicitorRepository>())
            .AddScoped<IReadOnlyCitiesRepository>(sp => sp.GetRequiredService<SolicitorsRepository>())
            .AddScoped<IRatingsProviderRepository>(sp => sp.GetRequiredService<SolicitorsRepository>())
            .AddSingleton(GetSetupService(configuration));
    }
    
    private static IRepoSetupService GetSetupService(IConfiguration configuration)
    {
        var options = ReadConfigForOptions(configuration);

        return options switch
        {
            InMemoryDbSetupOptions inMemOptions => new InMemorySetupService(inMemOptions),
            SqliteDbSetupOptions sqliteOptions => new SqliteSetupService(sqliteOptions),
            _ => throw new ArgumentException("Options type not supported", nameof(configuration))
        };
    }

    private static IRepoSetupOptions? ReadConfigForOptions(IConfiguration configuration)
    {
        var dbConfig = configuration.GetSection("Database");
        var dbType = dbConfig["Type"]?.ToLower();
        return dbType switch
        {
            "inmemory" => dbConfig.Get<InMemoryDbSetupOptions>(),
            "sqlite" => dbConfig.Get<SqliteDbSetupOptions>(),
            _ => null
        };
    }
}