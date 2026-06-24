namespace Solicitors.HtmlParsing.Models;

internal class HtmlAttribute(string name) : IHtmlAttribute
{
    public string Name { get; } = name;
}