namespace Solicitors.CacheBuild;

public interface ISolicitorImporter
{
    Task RunFullImport(CancellationToken cancellationToken);
}