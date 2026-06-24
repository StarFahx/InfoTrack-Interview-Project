namespace Solicitors.Core;

public interface IReadOnlyCitiesService
{
    Task<string[]> GetAllCitiesAsync(CancellationToken cancellationToken);
}