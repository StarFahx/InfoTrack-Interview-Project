using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;

namespace Solicitors.CacheBuild;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class DIExtensions
{
    public static IServiceCollection AddCacheBuild(this IServiceCollection services)
    {
        return services
            .AddScoped<ISolicitorParser, SolicitorsDotCom.SolicitorParser>()
            .AddScoped<ISolicitorImporter, SolicitorImporter>()
            .AddQuartz()
            .AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);
    }

    public static async Task UseCacheBuildAsync(this IServiceScope scope, CancellationToken cancellationToken)
    {
        var factory = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<ImportConfiguration>>().Value;
        var scheduler = await factory.GetScheduler(cancellationToken);

        var importJob = JobBuilder.Create<ImportRunner>()
            .WithIdentity("ImportJob", "ImportGroup")
            .Build();
        
        var importTrigger = TriggerBuilder.Create()
            .WithIdentity("ImportRunner", "ImportGroup")
            .StartNow()
            .WithSimpleSchedule(builder => builder
                .WithIntervalInMinutes(options.ImportMinutes)
                .RepeatForever())
            .Build();
        
        await scheduler.ScheduleJob(importJob, importTrigger, cancellationToken);

        var staleJob = JobBuilder.Create<StaleDataRemover>()
            .WithIdentity("StaleDataRemover", "RemoveGroup")
            .Build();

        var staleTrigger = TriggerBuilder.Create()
            .WithIdentity("StaleDataTrigger", "RemoveGroup")
            .StartNow()
            .WithSimpleSchedule(builder => builder
                .WithIntervalInMinutes(options.RemoveMinutes)
                .RepeatForever())
            .Build();
        
        await scheduler.ScheduleJob(staleJob, staleTrigger, cancellationToken);
    }
}