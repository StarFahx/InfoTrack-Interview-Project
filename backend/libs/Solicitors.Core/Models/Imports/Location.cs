namespace Solicitors.Core.Models.Imports;

public record Location(string Address, string Phone, Rating[] LocationRatings)
{
    public string Address { get; } = Address;
    public string Phone { get; } = Phone;
    public Rating[] LocationRatings { get; } = LocationRatings;
}