namespace Solicitors.Core;

public interface IRatingsProviderService
{
    Task<string[]> GetRatingsProvidersAsync(CancellationToken cancellationToken);
}