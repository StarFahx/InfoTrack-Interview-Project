using Solicitors.Core.Data;

namespace Solicitors.Core;

internal class ReadOnlyCitiesService(IReadOnlyCitiesRepository repo) : IReadOnlyCitiesService
{
    public async Task<string[]> GetAllCitiesAsync(CancellationToken cancellationToken)
    {
        var cities = repo.GetCitiesAsync();
        return await cities.ToArrayAsync(cancellationToken);
    }
}