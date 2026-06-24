using Solicitors.Core.Models;

namespace Solicitors.Core.Data;

public interface IReadOnlySolicitorRepository
{
    IAsyncEnumerable<Solicitor> GetAllSolicitorsAsync();
    Task<Solicitor?> GetSolicitorByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IReadOnlyCitiesRepository
{
    IAsyncEnumerable<string> GetCitiesAsync();
}