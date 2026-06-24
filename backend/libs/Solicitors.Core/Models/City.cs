namespace Solicitors.Core.Models;

public record City
{
    public Guid CityId { get; set; }
    public required string Name { get; set; }
    public DateTime LastModified { get; set; }

    public List<Solicitor> Solicitors { get; } = [];
}