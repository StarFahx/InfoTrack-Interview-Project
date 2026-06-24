using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Solicitors.Core.Ordering;

namespace Solicitors.Core;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class DIExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        return services
            .AddScoped<ISolicitorService, SolicitorService>()
            .AddScoped<IReadOnlySolicitorService>(sp => sp.GetRequiredService<ISolicitorService>())
            .AddScoped<IReadOnlyCitiesService, ReadOnlyCitiesService>()
            .AddScoped<IRatingsProviderService, RatingsProviderService>()
            .AddScoped<ISolicitorComparerFactory, SolicitorComparerFactory>();
    }
}