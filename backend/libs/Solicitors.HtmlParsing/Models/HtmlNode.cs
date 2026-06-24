using System.Diagnostics.CodeAnalysis;

namespace Solicitors.HtmlParsing.Models;

internal class HtmlNode(string name, IHtmlAttribute[] attributes) 
    : IHtmlNode
{
    public virtual bool TryGetByTagName(
        string tagName, 
        [NotNullWhen(true)] out IHtmlNode? node, 
        bool noChildren = false)
    {
        node = null;
        if (tagName == name)
            node = this;

        return node is not null;
    }

    private string[] Classes
    {
        get
        {
            var classAttribute = attributes
                .Where(a => a is HtmlValueAttribute)
                .Select(a => (a as HtmlValueAttribute)!)
                .FirstOrDefault(a => a.Name.Equals("class", StringComparison.CurrentCultureIgnoreCase));

            return classAttribute is not null 
                ? classAttribute.Value.Substring(1, classAttribute.Value.Length - 2).Split(' ') 
                : [];
        }
    }

    public virtual bool TryGetByClass(
        string className, 
        [NotNullWhen(true)] out IHtmlNode? node, 
        bool noChildren = false)
    {
        node = null;
        
        if (Classes.Contains(className, StringComparer.InvariantCultureIgnoreCase))
            node = this;
        
        return node is not null;
    }

    public virtual bool TryGetByTagNameAndClass(
        string tagName, 
        string className,
        [NotNullWhen(true)] out IHtmlNode? node,
        bool noChildren = false)
    {
        node = null;
        if (tagName == name && Classes.Contains(className, StringComparer.InvariantCultureIgnoreCase))
            node = this;

        return node is not null;
    }

    public virtual bool TryGetText([NotNullWhen(true)] out string? text)
    {
        text = null;
        return false;
    }

    public virtual bool TryGetChildren([NotNullWhen(true)] out IEnumerable<IHtmlNode>? children)
    {
        children = null;
        return false;
    }

    public bool HasAttribute(string attributeName)
    {
        return attributes
            .Any(a => a.Name.Equals(attributeName, StringComparison.InvariantCultureIgnoreCase));
    }

    public bool TryGetAttributeValue(string attributeName, [NotNullWhen(true)] out string? attributeValue)
    {
        attributeValue = null;
        
        var matchingAttribute = attributes.FirstOrDefault(
            a => a.Name.Equals(attributeName, StringComparison.InvariantCultureIgnoreCase)
        );
        
        if (matchingAttribute is HtmlValueAttribute valueAttribute)
            attributeValue = valueAttribute.Value;

        return attributeValue is not null;
    }
}