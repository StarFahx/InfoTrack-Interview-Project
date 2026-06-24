namespace Solicitors.CacheBuild.SolicitorsDotCom;

internal class SolicitorBuilder
{
    private readonly HashSet<string> _baseLocations = [];

    public SolicitorBuilder(
        string name,
        string path,
        string? shortDesc,
        string baseLocation)
    {
        Name = name;
        ShortDescription = shortDesc;
        _baseLocations.Add(baseLocation);
        
        Path = path;
    }

    public void AddBaseLocation(string baseLocation)
    {
        _baseLocations.Add(baseLocation);
    }
    
    public string Path { get; }
    public string Name { get; }

    public string? ShortDescription { get; }

    public string[] BaseLocations => _baseLocations.ToArray();
}