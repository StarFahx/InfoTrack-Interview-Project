using Solicitors.Core.Models.Imports;

namespace Solicitors.CacheBuild;

internal interface ISolicitorParser
{
    Task<SolicitorData[]> GetSolicitorsAsync(
        CancellationToken cancellationToken = default);
}