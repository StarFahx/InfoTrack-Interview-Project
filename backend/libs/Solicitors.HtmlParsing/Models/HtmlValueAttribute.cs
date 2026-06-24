namespace Solicitors.HtmlParsing.Models;

internal class HtmlValueAttribute(string name, string value) : HtmlAttribute(name)
{
    public string Value { get; } = value;
}