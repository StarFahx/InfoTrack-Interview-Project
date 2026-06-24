using System.Diagnostics.CodeAnalysis;

namespace Solicitors.HtmlParsing.Models;

public interface IHtmlNode
{
    /// <summary>
    /// Searches this node and its direct children for a matching tag name.
    /// </summary>
    /// <param name="tagName">The tag name to match.</param>
    /// <param name="node">If found, the node that matches the tag name.</param>
    /// <param name="noChildren">If true, only check this node, not its children.</param>
    /// <returns>True if this node, or any of its direct children, have a matching tag name; false otherwise.</returns>
    bool TryGetByTagName(
        string tagName, 
        [NotNullWhen(true)] out IHtmlNode? node, 
        bool noChildren = false);

    /// <summary>
    /// Searches this node and its direct children for a matching class.
    /// </summary>
    /// <param name="className">The class name to match.</param>
    /// <param name="node">If found, the node that matches the class name.</param>
    /// <param name="noChildren">If true, only check this node, not its children.</param>
    /// <returns>True if this node, or any of its direct children, have a matching class name; false otherwise.</returns>
    bool TryGetByClass(
        string className, 
        [NotNullWhen(true)] out IHtmlNode? node, 
        bool noChildren = false);

    /// <summary>
    /// Searches this node and its direct children for a matching tag name and matching class on the same node.
    /// </summary>
    /// <param name="tagName">The tag name to match.</param>
    /// <param name="className">The class name to match.</param>
    /// <param name="node">If found, the node that matches the tag name and class name.</param>
    /// <param name="noChildren">If true, only check this node, not its children.</param>
    /// <returns>True if this node, or any of its direct children, have a matching tag name and class name; false otherwise.</returns>
    bool TryGetByTagNameAndClass(
        string tagName, 
        string className, 
        [NotNullWhen(true)] out IHtmlNode? node, 
        bool noChildren = false);
    
    /// <summary>
    /// Checks this node (and *not* its children) to see if it is text content.
    /// </summary>
    /// <param name="text">If found, the text content.</param>
    /// <returns>True if the node is a text node, false otherwise.</returns>
    bool TryGetText([NotNullWhen(true)] out string? text);
    
    /// <summary>
    /// Checks this node to see if it has child elements.
    /// </summary>
    /// <param name="children">If found, the child elements.</param>
    /// <returns>True if the node has child elements, false otherwise.</returns>
    bool TryGetChildren([NotNullWhen(true)] out IEnumerable<IHtmlNode>? children);
    
    bool HasAttribute(string attributeName);

    bool TryGetAttributeValue(
        string attributeName,
        [NotNullWhen(true)] out string? attributeValue);
}