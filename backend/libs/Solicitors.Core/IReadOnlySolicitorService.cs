using Solicitors.Core.Misc;
using Solicitors.Core.Models;
using Solicitors.Core.Models.View;

namespace Solicitors.Core;

public interface IReadOnlySolicitorService
{
    Task<PaginationResponse<SolicitorSummary>> GetSolicitorSummariesAsync(
        Pagination pagination,
        string ratingsProvider,
        IFilter<Solicitor>? filter = null,
        IComparer<Solicitor>? ordering = null,
        CancellationToken cancellationToken = default);
    
    Task<SolicitorInfo?> GetSolicitorInfoByIdAsync(Guid id, CancellationToken cancellationToken = default);
}