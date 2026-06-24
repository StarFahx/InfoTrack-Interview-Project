using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Solicitors.HtmlParsing;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class DIExtensions
{
    public static IServiceCollection AddHtmlParsing(this IServiceCollection services)
    {
        return services.AddSingleton<IHtmlParser, HtmlParser>();
    }
}