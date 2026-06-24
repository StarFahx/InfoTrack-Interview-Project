namespace Solicitors.CacheBuild;

public class ImportConfiguration
{
    public int StaleMinutes { get; set; }
    public int RemoveMinutes { get; set; }
    public int ImportMinutes { get; set; }
}