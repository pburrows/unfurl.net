using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Unfurl.Net;

public class Unfurler
{
    private readonly ILogger? logger = null;

    public Unfurler(ILogger? logger = null)
    {
        this.logger = logger;
    }

    public async Task<UnfurlResult> Unfurl(string url, UnfurlOptions? options = null)
    {
        var result = new UnfurlResult();
        result.Url = url;
        var doc = await GetDocument(url, options, result);
        if (doc == null)
        {
            return result;
        }

        var head = doc.DocumentNode.SelectSingleNode("//head");
        if (head == null)
        {
            return result;
        }

        ParseDetails(head, result);
        ParseXTwitterMetadata(head, result);
        ParseOgMetadata(head, result);
        await ParseOEmbedMetadata(result, options);

        return result;
    }

    private async Task ParseOEmbedMetadata(UnfurlResult result, UnfurlOptions? options = null)
    {
        if (options is null or { LoadOEmbed: false })
        {
            return; // make users positively opt-in to parsing oembed
        }

        if (string.IsNullOrEmpty(result.OEmbedLink))
        {
            return;
        }

        var client = options?.OEmbedHttpClient;
        if (client == null)
        {
            logger?.LogWarning("OEmbedHttpClient not set. You should either set a value for OEmbedHttpClient or turn off oEmbed parsing by setting LoadOEmbed = false");
            client = new HttpClient();
        }
        var oEmbedContent = await GetOembedContent(client, result.OEmbedLink!, options?.UserAgent ?? "Unfurl.Net/1.0");
        result.OEmbed = oEmbedContent;
    }

    private void ParseDetails(HtmlNode head, UnfurlResult result)
    {
        result.FavIcon = GetAttributeValue(head, "link[@rel='icon']", "href") ??
                         GetAttributeValue(head, "link[@rel='shortcut icon']", "href");
        result.License = GetAttributeValue(head, "link[@rel='license']", "href");
        result.CanonicalUrl = GetAttributeValue(head, "link[@rel='canonical']", "href");
        result.AppleTouchIcon = GetAttributeValue(head, "link[@rel='apple-touch-icon']", "href");
        result.OEmbedLink = GetAttributeValue(head, "link[@type='application/json+oembed']", "href") ??
                            GetAttributeValue(head, "link[@type='text/xml+oembed']", "href");
        if (result.OEmbedLink != null)
        {
            result.OEmbedLink = HttpUtility.HtmlDecode(result.OEmbedLink);
        }

        var titleNode = head.SelectSingleNode("title");
        if (titleNode != null)
        {
            result.Title = titleNode.InnerText;
        }

        result.Description = GetAttributeValue(head, "meta[@name='description']", "content");
        result.Author = GetAttributeValue(head, "meta[@name='author']", "content") ??
                        GetAttributeValue(head, "link[@rel='author']", "href");
        result.ThemeColor = GetAttributeValue(head, "meta[@name='theme_color']", "content");
        var keywords = GetAttributeValue(head, "meta[@name='keywords']", "content");
        if (!string.IsNullOrEmpty(keywords))
        {
            result.Keywords = keywords!.Split(',').Select(p => p.Trim()).ToList();
        }
    }

    private string? GetAttributeValue(HtmlNode parentNode, string xPath, string attributeName)
    {
        var node = parentNode.SelectSingleNode(xPath);
        if (node != null)
        {
            return node.GetAttributeValue(attributeName, null);
        }

        return null;
    }

    private void ParseXTwitterMetadata(HtmlNode head, UnfurlResult result)
    {
        result.XTwitter = new XTwitterDetails();
        foreach (var tag in Tags.XTwitterTags)
        {
            result.XTwitter[tag.Label] = GetAttributeValue(head, tag.XPath, "content");
        }
    }

    private void ParseOgMetadata(HtmlNode head, UnfurlResult result)
    {
        result.OpenGraph = new OpenGraphDetails();
        foreach (var tag in Tags.OpenGraphTags)
        {
            result.OpenGraph[tag.Label] = GetAttributeValue(head, tag.XPath, "content");
        }

        ParseOgImages(head, result);
        ParseOgVideos(head, result);
        ParseOgAudio(head, result);
    }

    private static void ParseOgAudio(HtmlNode head, UnfurlResult result)
    {
        if (string.IsNullOrEmpty(result.OpenGraph?.Audio)) return;
        // check for more image details
        var nodes = head.SelectNodes("meta[contains(@property, 'og:audio')]");
        if (nodes.Count <= 1) return;
        result.OpenGraph!.Audios = new List<OpenGraphAudio>();
        OpenGraphAudio? curAudio = null;
        foreach (var node in nodes)
        {
            var name = node.GetAttributeValue("property", "");
            switch (name)
            {
                case "og:audio":
                    curAudio = new OpenGraphAudio();
                    result.OpenGraph.Audios.Add(curAudio);
                    curAudio.Url = node.GetAttributeValue("content", null);
                    break;
                case "og:audio:secure_url":
                    if (curAudio != null) curAudio.SecureUrl = node.GetAttributeValue("content", null);
                    break;
                case "og:audio:type":
                    if (curAudio != null) curAudio.Type = node.GetAttributeValue("content", null);
                    break;
            }
        }
    }

    private static void ParseOgVideos(HtmlNode head, UnfurlResult result)
    {
        if (string.IsNullOrEmpty(result.OpenGraph?.Video)) return;
        // check for more image details
        var nodes = head.SelectNodes("meta[contains(@property, 'og:video')]");
        if (nodes.Count <= 1) return;
        result.OpenGraph!.Videos = new List<OpenGraphVideo>();
        OpenGraphVideo? curVideo = null;
        foreach (var node in nodes)
        {
            var name = node.GetAttributeValue("property", "");
            switch (name)
            {
                case "og:video":
                    curVideo = new OpenGraphVideo();
                    result.OpenGraph.Videos.Add(curVideo);
                    curVideo.Url = node.GetAttributeValue("content", null);
                    break;
                case "og:video:secure_url":
                    if (curVideo != null) curVideo.SecureUrl = node.GetAttributeValue("content", null);
                    break;
                case "og:video:stream":
                    if (curVideo != null) curVideo.Stream = node.GetAttributeValue("content", null);
                    break;
                case "og:video:type":
                    if (curVideo != null) curVideo.Type = node.GetAttributeValue("content", null);
                    break;
                case "og:video:width":
                    if (curVideo != null) curVideo.Width = node.GetAttributeValue("content", null);
                    break;
                case "og:video:height":
                    if (curVideo != null) curVideo.Height = node.GetAttributeValue("content", null);
                    break;
                case "og:video:tag":
                    if (curVideo != null) curVideo.Tags = node.GetAttributeValue("content", null);
                    break;
            }
        }
    }

    private static void ParseOgImages(HtmlNode head, UnfurlResult result)
    {
        if (string.IsNullOrEmpty(result.OpenGraph?.Image)) return;
        // check for more image details
        var nodes = head.SelectNodes("meta[contains(@property, 'og:image')]");
        if (nodes.Count <= 1) return;
        result.OpenGraph!.Images = new List<OpenGraphImage>();
        OpenGraphImage? curImage = null;
        foreach (var node in nodes)
        {
            var name = node.GetAttributeValue("property", "");
            switch (name)
            {
                case "og:image":
                    curImage = new OpenGraphImage();
                    result.OpenGraph.Images.Add(curImage);
                    curImage.Url = node.GetAttributeValue("content", null);
                    break;
                case "og:image:secure_url":
                    if (curImage != null) curImage.SecureUrl = node.GetAttributeValue("content", null);
                    break;
                case "og:image:type":
                    if (curImage != null) curImage.Type = node.GetAttributeValue("content", null);
                    break;
                case "og:image:width":
                    if (curImage != null) curImage.Width = node.GetAttributeValue("content", null);
                    break;
                case "og:image:height":
                    if (curImage != null) curImage.Height = node.GetAttributeValue("content", null);
                    break;
                case "og:image:alt":
                    if (curImage != null) curImage.Alt = node.GetAttributeValue("content", null);
                    break;
            }
        }
    }

    private async Task<HtmlDocument?> GetDocument(string url, UnfurlOptions? options, UnfurlResult result)
    {
        var web = new HtmlWeb();
        web.UserAgent = options?.UserAgent ?? "Unfurl.Net/1.0";
        web.MaxAutoRedirects = options?.MaximumRedirects;
        web.CaptureRedirect = options?.MaximumRedirects == null;

        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentNullException(nameof(url));
        }


        HtmlDocument? doc = null;
        if (options == null)
        {
            doc = await web.LoadFromWebAsync(url);
        }
        else
        {
            var uri = new Uri(url);
            doc = await web.LoadFromWebAsync(uri, (Encoding)options.Encoding!, (NetworkCredential)options.Credentials!,
                options.CancellationToken ?? CancellationToken.None);
        }

        result.Url = url;
        if (web.CaptureRedirect && web.ResponseUri != null)
        {
            result.Url = web.ResponseUri.ToString();
        }

        result.Encoding = doc.Encoding.BodyName;
        return doc;
    }

    private async Task<OEmbedBase?> GetOembedContent(HttpClient httpClient, string url, string userAgent)
    {
        var uri = new Uri(url);
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
        var response = await httpClient.GetAsync(uri);
        var contentType = response.Content.Headers.ContentType.MediaType;
        var content = await response.Content.ReadAsStringAsync();
        switch (contentType.ToLower())
        {
            case "text/xml":
            case "text/xml+oembed":
                var xmlObj = XDocument.Parse(content);
                return ParseOEmbedXml(xmlObj);
            case "application/json":
            case "application/json+oembed":
                var obj = JObject.Parse(content);
                return ParseOEmbedJson(obj);
            default:
                return null;
        }
    }

    private OEmbedBase? ParseOEmbedJson(JObject obj)
    {
        var type = obj["type"]?.Value<string>();
        if (type == null)
        {
            return null;
        }

        OEmbedBase result;
        switch (type)
        {
            case OEmbedTypes.Video:
                var video = new OEmbedVideo();
                video.Type = OEmbedTypes.Video;
                video.Height = obj["height"]?.Value<int>() ?? 0;
                video.Width = obj["width"]?.Value<int>() ?? 0;
                video.Html = obj["html"]?.Value<string>() ?? "";
                result = video;
                break;
            case OEmbedTypes.Photo:
                var photo = new OEmbedPhoto();
                photo.Type = OEmbedTypes.Photo;
                photo.Height = obj["height"]?.Value<int>() ?? 0;
                photo.Width = obj["width"]?.Value<int>() ?? 0;
                photo.Url = obj["url"]?.Value<string>() ?? "";
                result = photo;
                break;
            case OEmbedTypes.Rich:
                var rich = new OEmbedRich();
                rich.Type = OEmbedTypes.Rich;
                rich.Height = obj["height"]?.Value<int>() ?? 0;
                rich.Width = obj["width"]?.Value<int>() ?? 0;
                rich.Html = obj["html"]?.Value<string>() ?? "";
                result = rich;
                break;
            case OEmbedTypes.Link:
            default:
                result = new OEmbedLink();
                result.Type = OEmbedTypes.Link;
                break;
        }

        result.Title = obj["title"]?.Value<string>(); 
        result.Version = obj["version"]?.Value<string>() ?? "1.0";
        result.AuthorName = obj["author_name"]?.Value<string>(); 
        result.AuthorUrl = obj["author_url"]?.Value<string>(); 
        result.ProviderName = obj["provider_name"]?.Value<string>(); 
        result.ProviderUrl = obj["provider_url"]?.Value<string>(); 
        result.CacheAge = obj["cache_age"]?.Value<int>(); //;
        result.ThumbnailUrl = obj["thumbnail_url"]?.Value<string>(); 
        result.ThumbnailHeight = obj["thumbnail_height"]?.Value<int>(); //;
        result.ThumbnailWidth = obj["thumbnail_width"]?.Value<int>(); //;

        return result;
    }

    private OEmbedBase? ParseOEmbedXml(XDocument doc)
    {
        var type = SafeXmlSelectValue(doc, "/oembed/type");
        if (type == null)
        {
            return null;
        }

        OEmbedBase result;
        switch (type)
        {
            case OEmbedTypes.Video:
                var video = new OEmbedVideo();
                video.Type = OEmbedTypes.Video;
                video.Height = SafeXmlSelectInt(doc, "/oembed/height") ?? 0;
                video.Width = SafeXmlSelectInt(doc, "/oembed/width") ?? 0;
                video.Html = SafeXmlSelectValue(doc, "/oembed/html") ?? "";
                result = video;
                break;
            case OEmbedTypes.Photo:
                var photo = new OEmbedPhoto();
                photo.Type = OEmbedTypes.Photo;
                photo.Height = SafeXmlSelectInt(doc, "/oembed/height") ?? 0;
                photo.Width = SafeXmlSelectInt(doc, "/oembed/width") ?? 0;
                photo.Url = SafeXmlSelectValue(doc, "/oembed/url") ?? "";
                result = photo;
                break;
            case OEmbedTypes.Rich:
                var rich = new OEmbedRich();
                rich.Type = OEmbedTypes.Rich;
                rich.Height = SafeXmlSelectInt(doc, "/oembed/height") ?? 0;
                rich.Width = SafeXmlSelectInt(doc, "/oembed/width") ?? 0;
                rich.Html = SafeXmlSelectValue(doc, "/oembed/html") ?? "";
                result = rich;
                break;
            case OEmbedTypes.Link:
            default:
                result = new OEmbedLink();
                result.Type = OEmbedTypes.Link;
                break;
        }

        result.Title = SafeXmlSelectValue(doc, "/oembed/title");
        result.Version = SafeXmlSelectValue(doc, "/oembed/version") ?? "1.0";
        result.AuthorName = SafeXmlSelectValue(doc, "/oembed/author_name");
        result.AuthorUrl = SafeXmlSelectValue(doc, "/oembed/author_url");
        result.ProviderName = SafeXmlSelectValue(doc, "/oembed/provider_name");
        result.ProviderUrl = SafeXmlSelectValue(doc, "/oembed/provider_url");
        result.CacheAge = SafeXmlSelectInt(doc, "/oembed/cache_age");
        result.ThumbnailUrl = SafeXmlSelectValue(doc, "/oembed/thumbnail_url");
        result.ThumbnailHeight = SafeXmlSelectInt(doc, "/oembed/thumbnail_height");
        result.ThumbnailWidth = SafeXmlSelectInt(doc, "/oembed/thumbnail_width");

        return result;
    }

    private string? SafeXmlSelectValue(XDocument doc, string xPath)
    {
        return doc.Document?.XPathSelectElement(xPath)?.Value;
    }

    private int? SafeXmlSelectInt(XDocument doc, string xPath)
    {
        var value = doc.Document?.XPathSelectElement(xPath)?.Value;
        if (value == null)
        {
            return null;
        }

        return Int32.Parse(value);
    }
}