namespace Solicitors.HtmlParsing;

using Models;

internal class HtmlParser : IHtmlParser
{
    private readonly string[] _voidElements =
    [
        "area",
        "base",
        "br",
        "col",
        "embed",
        "hr",
        "img",
        "input",
        "link",
        "meta",
        "source",
        "track",
        "wbr"
    ];
    
    public IEnumerable<IHtmlNode> ParseHtml(string html)
    {
        // wanted to do this in one pass but was quite time-consuming
        // so taking a little lesson from compiler development
        // and lex the text into tokens before parsing :)
        // deferred execution might help it still be "one pass"?
        var stringEnumerator = html.GetEnumerator();
        var tokens = LexHtml(stringEnumerator);
        var tokenEnumerator = tokens.GetEnumerator();
        return ParseHtml(tokenEnumerator);
    }
    
    private IEnumerable<string> LexHtml(CharEnumerator content)
    {
        var work = "";
        while (content.MoveNext())
        {
            if (content.Current == '<' && !string.IsNullOrWhiteSpace(work))
            {
                yield return work.Trim();
                work = "";
            }
        
            work += content.Current;
        
            if (content.Current == '>' && !string.IsNullOrWhiteSpace(work))
            {
                yield return work.Trim();
                work = "";
            }
        }
    }

    private IEnumerable<IHtmlNode> ParseHtml(IEnumerator<string> tokens)
    {
        while (tokens.MoveNext())
        {
            var token = tokens.Current;
            if (!token.StartsWith('<'))
            {
                yield return new StringNode(token);
                continue;
            }
        
            var noBraces = token.Substring(1, token.Length - 2);
            var split = noBraces.Split(' ');
            IHtmlAttribute[] attributes = [];
            if (split.Length > 1)
            {
                attributes = ParseAttributes(noBraces).ToArray();
            }

            if (IsVoidOrSelfClosingElement(token))
                yield return new HtmlNode(split.First(), attributes);
            else if (token.StartsWith("</"))
                yield break;
            else
            {
                var children = ParseHtml(tokens).ToArray();
                yield return new ParentHtmlNode(split.First(), attributes, children);
            }
        }
    }

    private bool IsVoidOrSelfClosingElement(string token)
    {
        if (token.StartsWith("<!") || token.EndsWith("/>"))
            return true;

        var tagName = token.Split(' ').First()[1..];
        if (tagName.EndsWith('>'))
            tagName = tagName[..^1];
        return _voidElements.Contains(tagName);
    }

    private IEnumerable<IHtmlAttribute> ParseAttributes(string tokenNoBraces)
    {
        var attributesString = string.Join(' ', tokenNoBraces.Split(' ').Skip(1));
        var attString = "";
        var inQuote = false;
        foreach (var c in attributesString)
        {
            if (!inQuote && c == ' ')
            {
                yield return ParseAttribute(attString);
                attString = "";
            }
            else
            {
                attString += c;
                if (c == '\"')
                    inQuote = !inQuote;
            }
        }
        
        yield return ParseAttribute(attString);
    }

    private IHtmlAttribute ParseAttribute(string attString)
    {
        var split = attString.Split('=');
        if (split.Length > 1)
            return new HtmlValueAttribute(split[0], string.Join('=', split.Skip(1)));
        else
            return new HtmlAttribute(attString);
    }
}