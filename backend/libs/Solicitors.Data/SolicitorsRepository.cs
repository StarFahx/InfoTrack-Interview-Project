using Microsoft.EntityFrameworkCore;
using Solicitors.Core.Data;
using Solicitors.Core.Models;
using Solicitors.Core.Models.Imports;
using Solicitors.Data.RepositorySetup;

namespace Solicitors.Data;

internal class SolicitorsRepository(IRepoSetupService setup)
    : DbContext, ISolicitorRepository, IReadOnlyCitiesRepository, IRatingsProviderRepository
{
    public DbSet<Solicitor> Solicitors { get; set; }
    public DbSet<SolicitorRating> SolicitorRatings { get; set; }
    public DbSet<SolicitorLocation> SolicitorLocations { get; set; }
    public DbSet<SolicitorLocationRating> SolicitorLocationRatings { get; set; }
    public DbSet<City> Cities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => setup.OnConfiguring(optionsBuilder);

    public Task EnsureCreatedAsync(CancellationToken cancellationToken)
        => Database.EnsureCreatedAsync(cancellationToken);

    public IAsyncEnumerable<Solicitor> GetAllSolicitorsAsync()
        => Solicitors
            .Include(solicitor => solicitor.Cities)
            .Include(solicitor => solicitor.Locations)
            .ThenInclude(location => location.LocationRatings)
            .Include(solicitor => solicitor.Ratings)
            .AsAsyncEnumerable();

    public Task<Solicitor?> GetSolicitorByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Solicitors
            .Include(solicitor => solicitor.Cities)
            .Include(solicitor => solicitor.Locations)
            .ThenInclude(location => location.LocationRatings)
            .Include(solicitor => solicitor.Ratings)
            .FirstOrDefaultAsync(x => x.SolicitorId == id, cancellationToken);

    public IAsyncEnumerable<string> GetCitiesAsync()
    {
        return Cities
            .Select(city => city.Name)
            .AsAsyncEnumerable();
    }

    public IAsyncEnumerable<string> GetAllRatingsProvidersAsync()
    {
        return SolicitorRatings
            .Select(rating => rating.Provider)
            .Distinct()
            .OrderBy(x => x == "Solicitors.com" ? 0 : 1)
            .ThenBy(x => x)
            .AsAsyncEnumerable();
    }

    public async Task AddOrUpdateSolicitorAsync(SolicitorData solicitor, CancellationToken cancellationToken)
    {
        var matchingSolicitor = await Solicitors
            .Include(existing => existing.Cities)
            .Include(existing => existing.Locations)
            .ThenInclude(location => location.LocationRatings)
            .Include(existing => existing.Ratings)
            .FirstOrDefaultAsync(
                item => item.Name == solicitor.Name, 
                cancellationToken: cancellationToken);

        if (matchingSolicitor is null)
            await AddNewSolicitorAsync(solicitor, cancellationToken);
        else
            await UpdateExistingSolicitorAsync(solicitor, matchingSolicitor, cancellationToken);

        await SaveChangesAsync(cancellationToken);
    }

    public Task RemoveStaleEntriesAsync(TimeSpan staleAge, CancellationToken cancellationToken)
    {
        var staleTime = DateTime.UtcNow - staleAge;
        
        Solicitors.RemoveRange(Solicitors.Where(x => x.LastModified < staleTime));
        SolicitorLocations.RemoveRange(SolicitorLocations.Where(x => x.LastModified < staleTime));
        SolicitorRatings.RemoveRange(SolicitorRatings.Where(x => x.LastModified < staleTime));
        SolicitorLocationRatings.RemoveRange(SolicitorLocationRatings.Where(x => x.LastModified < staleTime));
        Cities.RemoveRange(Cities.Where(x => x.LastModified < staleTime));

        return SaveChangesAsync(cancellationToken);
    }

    private async Task AddNewSolicitorAsync(
        SolicitorData solicitor, 
        CancellationToken cancellationToken)
    {
        var newSol = new Solicitor
        {
            Name = solicitor.Name,
            RelativeUrl = solicitor.UrlPath
        };
        UpdateSolicitorRootData(newSol, solicitor);
        Solicitors.Add(newSol);

        var cityTasks = solicitor.Cities
            .Distinct()
            .Select(async city =>
            {
                var matchingCity = await GetCityByNameAsync(city, cancellationToken);
                
                if (matchingCity is null)
                    AddNewCityToSolicitor(city, newSol);
                else
                    UpdateExistingCity(matchingCity, newSol);
            });
        
        await Task.WhenAll(cityTasks);

        foreach (var rating in solicitor.Ratings)
            AddNewRatingToSolicitor(rating, newSol);

        foreach (var location in solicitor.Offices)
            AddNewLocationToSolicitor(location, newSol);
    }

    private void UpdateSolicitorRootData(Solicitor solicitor, SolicitorData data)
    {
        solicitor.Name = data.Name;
        solicitor.RelativeUrl = data.UrlPath;
        solicitor.Phone = data.Phone;
        solicitor.Email = data.Email;
        solicitor.Website =  data.Website;
        solicitor.ShortDescription = data.ShortDescription;
        solicitor.LastModified = DateTime.UtcNow;
    }

    private void AddNewCityToSolicitor(string cityName, Solicitor solicitor)
    {
        var newCity = new City
        {
            Name = cityName
        };
        Cities.Add(newCity);
        UpdateExistingCity(newCity, solicitor);
    }

    private void AddNewRatingToSolicitor(Rating rating, Solicitor solicitor)
    {
        var newRating = new SolicitorRating
        {
            Value = rating.Value,
            Maximum = rating.MaxValue,
            Provider = rating.RatingProvider,
            Solicitor = solicitor,
            LastModified = DateTime.UtcNow
        };
        SolicitorRatings.Add(newRating);
        solicitor.Ratings.Add(newRating);
    }

    private void AddNewLocationToSolicitor(Location location, Solicitor solicitor)
    {
        var newLocation = new SolicitorLocation
        {
            Address = location.Address,
            Phone = location.Phone,
            Solicitor = solicitor,
            LastModified = DateTime.UtcNow
        };
        SolicitorLocations.Add(newLocation);
        solicitor.Locations.Add(newLocation);

        foreach (var rating in location.LocationRatings)
            AddNewRatingToLocation(rating, newLocation);
    }

    private void AddNewRatingToLocation(Rating rating, SolicitorLocation location)
    {
        var newLocRating = new SolicitorLocationRating
        {
            Value = rating.Value,
            Maximum = rating.MaxValue,
            Provider = rating.RatingProvider,
            SolicitorLocation = location,
            LastModified = DateTime.UtcNow
        };
        SolicitorLocationRatings.Add(newLocRating);
        location.LocationRatings.Add(newLocRating);
    }

    private Task UpdateExistingSolicitorAsync(
        SolicitorData solicitor,
        Solicitor matchingSolicitor,
        CancellationToken cancellationToken)
    {
        UpdateSolicitorRootData(matchingSolicitor, solicitor);
        var citiesUpdate = UpdateExistingCitiesAsync(solicitor, matchingSolicitor, cancellationToken);
        UpdateExistingRatings(solicitor, matchingSolicitor);
        UpdateExistingLocations(solicitor, matchingSolicitor);
        return citiesUpdate;
    }

    private async Task UpdateExistingCitiesAsync(
        SolicitorData solicitor,
        Solicitor matchingSolicitor,
        CancellationToken cancellationToken)
    {
        var lostCities = matchingSolicitor.Cities
            .Where(city => solicitor.Cities.All(cityName => cityName != city.Name))
            .ToArray();

        foreach (var city in lostCities)
        {
            matchingSolicitor.Cities.Remove(city);
            city.Solicitors.Remove(matchingSolicitor);
        }
        foreach (var cityName in solicitor.Cities.Distinct())
        {
            var matchingCity = matchingSolicitor.Cities
                .FirstOrDefault(city => cityName == city.Name);
            
            matchingCity ??= await GetCityByNameAsync(cityName, cancellationToken);

            if (matchingCity is null)
                AddNewCityToSolicitor(cityName, matchingSolicitor);
            else
                UpdateExistingCity(matchingCity, matchingSolicitor);
        }
    }
    
    private void UpdateExistingRatings(
        SolicitorData solicitor,
        Solicitor matchingSolicitor)
    {
        var lostRatings = matchingSolicitor.Ratings
            .Where(solicitorRating => solicitor.Ratings.All(rating => rating.RatingProvider != solicitorRating.Provider))
            .ToArray();

        foreach (var rating in lostRatings)
        {
            matchingSolicitor.Ratings.Remove(rating);
            SolicitorRatings.Remove(rating);
        }

        foreach (var rating in solicitor.Ratings)
        {
            var matchingRating = matchingSolicitor.Ratings
                .FirstOrDefault(solicitorRating => rating.RatingProvider == solicitorRating.Provider);
            
            if (matchingRating is null)
                AddNewRatingToSolicitor(rating, matchingSolicitor);
            else
                UpdateExistingRating(rating, matchingRating);
        }
    }

    private void UpdateExistingRating(Rating rating, SolicitorRating existingRating)
    {
        existingRating.Value = rating.Value;
        existingRating.Maximum = rating.MaxValue;
        existingRating.LastModified = DateTime.UtcNow;
    }
    
    private void UpdateExistingLocations(
        SolicitorData solicitor,
        Solicitor matchingSolicitor)
    {
        var lostLocations = matchingSolicitor.Locations
            .Where(location => solicitor.Offices.All(office => office.Address != location.Address))
            .ToArray();

        foreach (var location in lostLocations)
        {
            matchingSolicitor.Locations.Remove(location);
            SolicitorLocations.Remove(location);
        }

        foreach (var office in solicitor.Offices)
        {
            var location = matchingSolicitor.Locations
                    .FirstOrDefault(location => office.Address == location.Address);
            
            if (location is null)
                AddNewLocationToSolicitor(office, matchingSolicitor);
            else
                UpdateExistingLocation(office, location);
        }
    }

    private void UpdateExistingLocation(Location location, SolicitorLocation existingLocation)
    {
        existingLocation.Phone = location.Phone;
        existingLocation.LastModified = DateTime.UtcNow;
        
        var lostRatings = existingLocation.LocationRatings
            .Where(locationRating => location.LocationRatings.All(rating => locationRating.Provider != rating.RatingProvider))
            .ToArray();

        foreach (var rating in lostRatings)
        {
            existingLocation.LocationRatings.Remove(rating);
            SolicitorLocationRatings.Remove(rating);
        }

        foreach (var rating in location.LocationRatings)
        {
            var existingRating = existingLocation.LocationRatings
                .FirstOrDefault(locationRating => locationRating.Provider == rating.RatingProvider);

            if (existingRating is null)
                AddNewRatingToLocation(rating, existingLocation);
            else
                UpdateExistingLocationRating(rating, existingRating);
        }
    }

    private void UpdateExistingLocationRating(Rating rating, SolicitorLocationRating existingRating)
    {
        existingRating.Value = rating.Value;
        existingRating.Maximum = rating.MaxValue;
        existingRating.LastModified = DateTime.UtcNow;
    }

    private void UpdateExistingCity(City city, Solicitor solicitor)
    {
        city.Solicitors.Add(solicitor);
        solicitor.Cities.Add(city);
        city.LastModified = DateTime.UtcNow;
    }

    private Task<City?> GetCityByNameAsync(string cityName, CancellationToken cancellationToken)
    {
        return Cities
            .Include(city => city.Solicitors)
            .FirstOrDefaultAsync(
                city => city.Name == cityName,
                cancellationToken: cancellationToken);
    }
}