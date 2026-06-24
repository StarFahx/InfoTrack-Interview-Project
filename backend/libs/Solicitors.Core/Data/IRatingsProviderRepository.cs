namespace Solicitors.Core.Data;

public interface IRatingsProviderRepository
{
    IAsyncEnumerable<string> GetAllRatingsProvidersAsync();
}