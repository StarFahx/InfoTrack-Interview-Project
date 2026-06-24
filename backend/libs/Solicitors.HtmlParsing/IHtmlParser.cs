namespace Solicitors.HtmlParsing;

using Models;

public interface IHtmlParser
{
    IEnumerable<IHtmlNode> ParseHtml(string html);
}