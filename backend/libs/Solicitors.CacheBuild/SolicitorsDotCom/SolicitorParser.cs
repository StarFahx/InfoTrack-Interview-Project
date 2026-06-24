using System.Diagnostics.CodeAnalysis;
using Solicitors.Core.Misc;
using Solicitors.Core.Models.Imports;
using Solicitors.HtmlParsing;
using Solicitors.HtmlParsing.Models;

namespace Solicitors.CacheBuild.SolicitorsDotCom;

internal class SolicitorParser : ISolicitorParser
{
    private readonly HttpClient _client;
    private readonly IHtmlParser _htmlParser;

    public SolicitorParser(
        HttpClient client,
        IHtmlParser htmlParser)
    {
        _client = client;
        _client.BaseAddress = new Uri(Consts.BaseUrl);
        
        _htmlParser = htmlParser;
    }

    public async Task<SolicitorData[]> GetSolicitorsAsync(
        CancellationToken cancellationToken = default)
    {
        var builders = new Dictionary<string, SolicitorBuilder>();
        
        foreach (var location in Consts.Locations)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/conveyancing+{location}.html");
            httpRequest.Headers.Add("User-Agent", "Solicitors API Cache");
            var httpResponse = await _client.SendAsync(httpRequest, cancellationToken);
            var html = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            var nodes = _htmlParser.ParseHtml(html);
            IHtmlNode? mainNode = null;
            foreach (var node in nodes)
                if (node.TryGetByTagName("main", out mainNode))
                    break;

            if (TryGetResultSection(mainNode, out var results))
                ParseResultsSection(results, location, builders);
        }

        var batches = builders.Values
            .Select(builder => GetFullSolicitorDataAsync(builder, cancellationToken))
            .Batch(5);

        var solicitors = new List<SolicitorData?>();
        foreach (var batch in batches)
            solicitors.AddRange(await Task.WhenAll(batch));
        
        return solicitors
            .Where(x => x is not null)
            .Cast<SolicitorData>()
            .ToArray();
    }

    private bool TryGetResultSection(
        IHtmlNode? mainNode,
        [NotNullWhen(true)] out IEnumerable<IHtmlNode>? results)
    {
        results = null;
        
        // going one level at a time helps a little with performance here, at the cost of messier code
        return mainNode is not null
               && mainNode.TryGetByTagNameAndClass("div", "content-holder", out var contentHolderNode)
               && contentHolderNode.TryGetByTagNameAndClass("div", "container", out var containerNode)
               && containerNode.TryGetByTagNameAndClass("div", "content", out var contentNode)
               && contentNode.TryGetByTagNameAndClass("div", "result-section", out var resultSectionNode)
               && resultSectionNode.TryGetChildren(out results);
    }

    private void ParseResultsSection(
        IEnumerable<IHtmlNode> results,
        string location,
        Dictionary<string, SolicitorBuilder> builders)
    {
        foreach (var result in results)
        {
            if (result.TryGetByClass("result-item", out var resultItem, true)
                && TryParseToNewBuilder(resultItem, location, out var builder))
            {
                if (!builders.TryAdd(builder.Path, builder))
                    builders[builder.Path].AddBaseLocation(location);
            }
        }
    }

    private bool TryParseToNewBuilder(
        IHtmlNode resultItemNode,
        string baseLocation,
        [NotNullWhen(true)] out SolicitorBuilder? builder)
    {
        builder = null;
        if (!resultItemNode.TryGetByTagNameAndClass("a", "link-map", out var linkMapNode)
            || !linkMapNode.TryGetAttributeValue("href", out var uniqueUrl)
            || !resultItemNode.TryGetByTagNameAndClass("span", "h2", out var titleNode)
            || !titleNode.TryGetChildren(out var titleChildren))
            return false;

        string? name = null;
        foreach (var child in titleChildren)
            if (child.TryGetText(out name))
                break;

        if (name is null)
            return false;
        
        string? shortDesc = null;
        if (resultItemNode.TryGetByTagName("p", out var pNode)
            && pNode.TryGetChildren(out var pChildren))
            foreach (var child in pChildren)
                if (child.TryGetText(out shortDesc))
                    break;

        builder = new SolicitorBuilder(name, uniqueUrl, shortDesc, baseLocation);
        return true;
    }

    private async Task<SolicitorData?> GetFullSolicitorDataAsync(
        SolicitorBuilder builder,
        CancellationToken cancellationToken)
    {
        
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, builder.Path);
        httpRequest.Headers.Add("User-Agent", "Solicitors API Cache");
        var httpResponse = await _client.SendAsync(httpRequest, cancellationToken);
        var html = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
        var nodes = _htmlParser.ParseHtml(html);
        IHtmlNode? mainNode = null;
        foreach (var node in nodes)
            if (node.TryGetByTagName("main", out mainNode))
                break;

        if (mainNode is null || !mainNode.TryGetByTagNameAndClass("div", "content-block", out var contentBlockNode))
            return null;

        Location[] offices = [];
        if (contentBlockNode.TryGetByTagNameAndClass("div", "office-item", out var officeItemNode)
            && officeItemNode.TryGetChildren(out var officeItems))
        {
            offices = ParseOffices(officeItems);
        }

        Rating[] ratings = [];
        string? phone = null;
        string? email = null;
        string? website = null;
        if (mainNode.TryGetByTagNameAndClass("div", "side-holder", out var sidebarNode)
            && sidebarNode.TryGetChildren(out var sidebarChildren))
        {
            ParseSidebar(sidebarChildren, out phone, out email, out website, out ratings);
        }

        return new SolicitorData
        {
            Name = builder.Name,
            UrlPath = builder.Path,
            ShortDescription = builder.ShortDescription,
            Phone = phone,
            Email = email,
            Website = website,
            Ratings = ratings,
            Offices = offices,
            Cities = builder.BaseLocations
        };
    }

    private Location[] ParseOffices(IEnumerable<IHtmlNode> officeNodes)
        => officeNodes
            .Select(ParseOffice)
            .Where(x => x is not null)
            .Cast<Location>()
            .ToArray();

    private Location? ParseOffice(IHtmlNode officeNode)
    {
        if (!officeNode.TryGetChildren(out var officeChildren)
            || !officeNode.TryGetByTagName("address", out var addressNode)
            || !addressNode.TryGetChildren(out var addressLines))
            return null;

        Location? location = null;
        var address = "";
        foreach (var line in addressLines)
        {
            if (line.TryGetText(out var lineText))
                address += lineText;
            else if (line.TryGetByTagName("br", out _, true))
                address += "\n";
        }

        if (string.IsNullOrWhiteSpace(address))
            address = null;

        string? phone = null;
        foreach (var child in officeChildren)
        {
            if (child.TryGetByTagName("a", out var anchor)
                && anchor.TryGetAttributeValue("href", out var telString)
                && telString.StartsWith("\"tel:")
                && telString.Length > 5)
            {
                phone = telString.Substring(5, telString.Length - 6);
                break;
            }
        }

        if (address is not null && phone is not null)
            location = new Location(address, phone, []);

        return location;
    }

    private void ParseSidebar(
        IEnumerable<IHtmlNode> sidebarChildren,
        out string? phone,
        out string? email,
        out string? website,
        out Rating[] ratings)
    {
        List<Rating> ratingsList = [];
        phone = null;
        website = null;
        email = null;

        foreach (var node in sidebarChildren)
        {
            var isRevCount = node.TryGetByTagNameAndClass("div", "rev-count", out _, true);
            var isLinksHolder = node.TryGetByTagNameAndClass("div", "links-holder", out _, true);
            if ((!isRevCount && !isLinksHolder)
                || !node.TryGetChildren(out var children))
                continue;

            if (isLinksHolder)
            {
                if (node.TryGetByTagNameAndClass("a", "website", out var websiteNode)
                    && websiteNode.TryGetAttributeValue("href", out website))
                    website = website.Trim('"');

                if (node.TryGetByTagNameAndClass("a", "phone", out var phoneNode)
                    && phoneNode.TryGetAttributeValue("href", out var phoneStr)
                    && phoneStr.StartsWith("\"tel:")
                    && phoneStr.Length > 5)
                {
                    phone = phoneStr.Substring(5, phoneStr.Length - 6);
                }
            }

            var childrenArray = children as IHtmlNode[] ?? children.ToArray();
            foreach (var child in childrenArray.Where(x => x.TryGetByClass("rev-box", out _, true)))
            {
                if (child.TryGetAttributeValue("title", out var ratingString)
                    && TryParseRatingString(ratingString, out var rating, out var maxRating)
                    && child.TryGetByTagNameAndClass("img", "rev-logo", out var imgNode)
                    && imgNode.TryGetAttributeValue("src", out var imgSrc)
                    && TryParseRatingSrc(imgSrc, out var ratingProvider))
                {
                    ratingsList.Add(new Rating(rating, maxRating, ratingProvider, imgSrc));
                }
            }

            if (ratingsList.Count == 0)
            {
                foreach (var child in childrenArray.Where(x => !x.HasAttribute("class")))
                {
                    if (child.TryGetChildren(out var grandChildren))
                        foreach (var grandChild in grandChildren)
                            if (grandChild.TryGetText(out var textContent)
                                && TryParseOwnRatingString(textContent, out var rating))
                                ratingsList.Add(new Rating(rating, Consts.MaxRating, Consts.Name, Consts.IconPath));
                }
            }
        }

        ratings = ratingsList.ToArray();
    }
    

    bool TryParseRatingString(string ratingString, out decimal rating, out decimal maxRating)
    {
        rating = 0;
        maxRating = 0;
        var parts = ratingString.Trim('"').Split(" / ");
        if (parts.Length != 2)
            return false;

        return decimal.TryParse(parts[0], out rating) && decimal.TryParse(parts[1], out maxRating);
    }

    private bool TryParseRatingSrc(string imgSrc, out string provider)
    {
        provider = "";
        imgSrc = imgSrc.Trim('"');
        const string prefix = "/images/logo-";
        if (!imgSrc.StartsWith(prefix))
            return false;
        
        var providerAndExtension = imgSrc[prefix.Length..];
        provider = providerAndExtension.Split('.').First();
        return true;
    }

    bool TryParseOwnRatingString(string ratingString, out decimal rating)
    {
        rating = 0;
        const string prefix = "Average review score : ";
        if (!ratingString.StartsWith(prefix))
            return false;

        var ratingPart = ratingString[prefix.Length..];
        return decimal.TryParse(ratingPart, out rating);
    }
}