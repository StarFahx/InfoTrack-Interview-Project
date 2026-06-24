using Microsoft.AspNetCore.Mvc;
using Solicitors.Core;

namespace Solicitors.Api.Endpoints;

public static class RatingsProvidersEndpoints
{
    public static void MapRatingsProviders(this IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/ratingsProviders",
            (
                [FromServices] IRatingsProviderService service,
                CancellationToken cancellationToken
            ) => service.GetRatingsProvidersAsync(cancellationToken));
    }
}