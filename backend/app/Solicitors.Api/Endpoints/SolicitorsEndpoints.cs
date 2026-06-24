using Microsoft.AspNetCore.Mvc;
using Solicitors.Core;
using Solicitors.Core.Misc;
using Solicitors.Core.Models;
using Solicitors.Core.Ordering;

namespace Solicitors.Api.Endpoints;

public static class SolicitorsEndpoints
{
    public static void MapSolicitors(this IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/solicitors", 
            (
                [FromServices] IReadOnlySolicitorService solicitorService,
                [FromServices] ISolicitorComparerFactory solicitorComparerFactory,
                CancellationToken cancellationToken,
                [FromQuery] uint pageNumber = 1,
                [FromQuery] uint pageSize = 20,
                [FromQuery] string? nameFilter = null,
                [FromQuery] decimal minRating = 0,
                [FromQuery] string[]? cities = null,
                [FromQuery] string ratingsProvider = "Solicitors.com",
                [FromQuery] OrderingType? orderingType = null) =>
            {
                IFilter<Solicitor>? filter = null;

                if (!string.IsNullOrEmpty(nameFilter))
                    filter = new WrapperFilter<Solicitor>(filter, solicitor => solicitor.Name.Contains(nameFilter, StringComparison.CurrentCultureIgnoreCase));
        
                if (cities is not null && cities.Length > 0)
                    filter = new WrapperFilter<Solicitor>(filter, solicitor => cities.Any(city => solicitor.Cities.Any(solCity => solCity.Name == city)));

                if (minRating > 0)
                    filter = new WrapperFilter<Solicitor>(
                        filter,
                        solicitor => solicitor.Ratings.Any(rating => rating.Provider == ratingsProvider && rating.Value / rating.Maximum >= minRating / 5.0m));

                var ordering = solicitorComparerFactory.GetComparer(orderingType, ratingsProvider);
        
                return solicitorService.GetSolicitorSummariesAsync(
                    new Pagination(pageNumber, pageSize),
                    ratingsProvider,
                    filter,
                    ordering,
                    cancellationToken: cancellationToken);
            }
        );

        app.MapGet(
            "/solicitors/{id:guid}",
            (
                [FromServices] IReadOnlySolicitorService solicitorService,
                [FromRoute] Guid id,
                CancellationToken cancellationToken
            ) => solicitorService.GetSolicitorInfoByIdAsync(id, cancellationToken)
        );
    }
}