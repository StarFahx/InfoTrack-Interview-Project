using System.Diagnostics.CodeAnalysis;

namespace Solicitors.HtmlParsing.Models;

internal class ParentHtmlNode(string tagName, IHtmlAttribute[] attributes, IHtmlNode[] children) 
    : HtmlNode(tagName, attributes)
{
    public override bool TryGetByTagName(
        string tagName, 
        [NotNullWhen(true)] out IHtmlNode? node, 
        bool noChildren = false)
    {
        node = null;
        if (!base.TryGetByTagName(tagName, out node, noChildren) && !noChildren)
        {
            var nodesToCheck = new Queue<IHtmlNode>(children);
            while (nodesToCheck.TryDequeue(out var child))
            {
                if (child.TryGetByTagName(tagName, out var childNode, true))
                {
                    node = childNode;
                    break;
                }

                if (child.TryGetChildren(out var grandChildren))
                    foreach (var grandChild in grandChildren)
                        nodesToCheck.Enqueue(grandChild);
            }
        }

        return node is not null;
    }

    public override bool TryGetByClass(
        string className, 
        [NotNullWhen(true)] out IHtmlNode? node,
        bool noChildren = false)
    {
        node = null;
        if (!base.TryGetByClass(className, out node, noChildren) && !noChildren)
        {
            var nodesToCheck = new Queue<IHtmlNode>(children);
            while (nodesToCheck.TryDequeue(out var child))
            {
                if (child.TryGetByClass(className, out var childNode, true))
                {
                    node = childNode;
                    break;
                }

                if (child.TryGetChildren(out var grandChildren))
                    foreach (var grandChild in grandChildren)
                        nodesToCheck.Enqueue(grandChild);
            }
        }

        return node is not null;
    }

    public override bool TryGetByTagNameAndClass(
        string tagName,
        string className,
        [NotNullWhen(true)] out IHtmlNode? node,
        bool noChildren = false)
    {
        node = null;
        if (!base.TryGetByTagNameAndClass(tagName, className, out node, noChildren) && !noChildren)
        {
            var nodesToCheck = new Queue<IHtmlNode>(children);
            while (nodesToCheck.TryDequeue(out var child))
            {
                if (child.TryGetByTagNameAndClass(tagName, className, out var childNode, true))
                {
                    node = childNode;
                    break;
                }

                if (child.TryGetChildren(out var grandChildren))
                    foreach (var grandChild in grandChildren)
                        nodesToCheck.Enqueue(grandChild);
            }
        }

        return node is not null;
    }

    public override bool TryGetChildren([NotNullWhen(true)] out IEnumerable<IHtmlNode>? children1)
    {
        children1 = children;
        return children.Length != 0;
    }
}