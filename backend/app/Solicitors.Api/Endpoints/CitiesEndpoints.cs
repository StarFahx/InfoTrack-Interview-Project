using Microsoft.AspNetCore.Mvc;
using Solicitors.Core;

namespace Solicitors.Api.Endpoints;

public static class CitiesEndpoints
{
    public static void MapCities(this IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/cities",
            (
                [FromServices] IReadOnlyCitiesService citiesService,
                CancellationToken cancellationToken
            ) => citiesService.GetAllCitiesAsync(cancellationToken));
    }
}