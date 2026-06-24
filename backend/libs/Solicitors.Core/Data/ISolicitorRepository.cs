using Solicitors.Core.Models.Imports;

namespace Solicitors.Core.Data;

public interface ISolicitorRepository : IReadOnlySolicitorRepository
{
    Task EnsureCreatedAsync(CancellationToken cancellationToken);
    Task AddOrUpdateSolicitorAsync(SolicitorData solicitor, CancellationToken cancellationToken);
    Task RemoveStaleEntriesAsync(TimeSpan staleAge, CancellationToken cancellationToken);
}