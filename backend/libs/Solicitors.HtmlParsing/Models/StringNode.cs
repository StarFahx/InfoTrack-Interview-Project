using System.Diagnostics.CodeAnalysis;

namespace Solicitors.HtmlParsing.Models;

internal class StringNode(string content) 
    : IHtmlNode
{
    public bool TryGetByTagName(
        string tagName,
        [NotNullWhen(true)] out IHtmlNode? node,
        bool noChildren = false)
    {
        node = null;
        return false;
    }

    public bool TryGetByClass(
        string className,
        [NotNullWhen(true)] out IHtmlNode? node,
        bool noChildren = false)
    {
        node = null;
        return false;
    }

    public bool TryGetByTagNameAndClass(
        string tagName, 
        string className, 
        [NotNullWhen(true)] out IHtmlNode? node,
        bool noChildren = false)
    {
        node = null;
        return false;
    }

    public bool TryGetText([NotNullWhen(true)] out string? text)
    {
        text = content;
        return !string.IsNullOrEmpty(text);
    }

    public bool TryGetChildren([NotNullWhen(true)] out IEnumerable<IHtmlNode>? children)
    {
        children = null;
        return false;
    }

    public bool HasAttribute(string attributeName) => false;

    public bool TryGetAttributeValue(
        string attributeName, 
        [NotNullWhen(true)] out string? attributeValue)
    {
        attributeValue = null;
        return false;
    }
}