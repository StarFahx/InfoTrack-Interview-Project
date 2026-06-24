using Quartz;
using Solicitors.Core.Data;

namespace Solicitors.CacheBuild;

internal class ImportRunner(
    ISolicitorImporter importer,
    ISolicitorRepository repository) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await repository.EnsureCreatedAsync(context.CancellationToken);
        await importer.RunFullImport(context.CancellationToken);
    }
}