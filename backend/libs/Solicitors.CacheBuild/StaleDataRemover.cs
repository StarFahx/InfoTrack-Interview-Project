using Microsoft.Extensions.Options;
using Quartz;
using Solicitors.Core.Data;

namespace Solicitors.CacheBuild;

internal class StaleDataRemover(
    IOptions<ImportConfiguration> config,
    ISolicitorRepository repository) : IJob
{
    private readonly ImportConfiguration _config = config.Value;

    public Task Execute(IJobExecutionContext context)
    {
        return repository.RemoveStaleEntriesAsync(
            TimeSpan.FromMinutes(_config.StaleMinutes),
            context.CancellationToken);
    }
}