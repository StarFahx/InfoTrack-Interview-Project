namespace Solicitors.Core.Models.Imports;

public class SolicitorData
{
    public required string Name { get; init; }
    public required string UrlPath { get; init; }

    public string? ShortDescription { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? Website { get; init; }

    public string[] Cities { get; init; } = [];
    public Rating[] Ratings { get; init; } = [];
    public Location[] Offices { get; init; } = [];
}