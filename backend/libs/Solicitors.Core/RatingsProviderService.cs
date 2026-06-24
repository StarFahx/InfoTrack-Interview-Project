using Solicitors.Core.Data;

namespace Solicitors.Core;

internal class RatingsProviderService(IRatingsProviderRepository repo) : IRatingsProviderService
{
    public async Task<string[]> GetRatingsProvidersAsync(CancellationToken cancellationToken)
    {
        return await repo.GetAllRatingsProvidersAsync().ToArrayAsync(cancellationToken);
    }
}