using Solicitors.Core.Data;
using Solicitors.Core.Misc;
using Solicitors.Core.Models;
using Solicitors.Core.Models.View;
using Solicitors.Core.Ordering;

namespace Solicitors.Core;

internal class SolicitorService(IReadOnlySolicitorRepository repo) : ISolicitorService
{
    public async Task<PaginationResponse<SolicitorSummary>> GetSolicitorSummariesAsync(
        Pagination pagination,
        string ratingsProvider,
        IFilter<Solicitor>? filter = null,
        IComparer<Solicitor>? ordering = null,
        CancellationToken cancellationToken = default)
    {
        var allSolicitors = repo.GetAllSolicitorsAsync();
        if (filter is not null)
            allSolicitors = allSolicitors.Where(filter.Filter);

        var matchingSolicitors = await allSolicitors.ToArrayAsync(cancellationToken);
        
        ordering ??= DefaultSolicitorDataOrdering.Instance;
        
        var results = matchingSolicitors
            .OrderBy(x => x, ordering)
            .Skip((int)((pagination.CurrentPage - 1) * pagination.PageSize))
            .Take((int)pagination.PageSize)
            .Select(x => new SolicitorSummary(x, ratingsProvider))
            .ToArray();

        return new PaginationResponse<SolicitorSummary>(matchingSolicitors.Length, results);
    }

    public async Task<SolicitorInfo?> GetSolicitorInfoByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var data = await repo.GetSolicitorByIdAsync(id, cancellationToken);
        if (data is null)
            return null;
        
        return new SolicitorInfo(data);
    }
}