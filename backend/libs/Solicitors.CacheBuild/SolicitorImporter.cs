using Solicitors.Core.Data;
using Solicitors.Core.Models.Imports;

namespace Solicitors.CacheBuild;

internal class SolicitorImporter(
    ISolicitorParser parser,
    ISolicitorRepository repository) : ISolicitorImporter
{
    public async Task RunFullImport(CancellationToken cancellationToken)
    {
        foreach (var solicitor in await parser.GetSolicitorsAsync(cancellationToken))
        {
           await RunImport(solicitor, cancellationToken); 
        }
    }

    private Task RunImport(SolicitorData solicitor, CancellationToken cancellationToken) 
        => repository.AddOrUpdateSolicitorAsync(solicitor, cancellationToken);
}