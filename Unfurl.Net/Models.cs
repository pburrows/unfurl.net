using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace Unfurl.Net;

public class UnfurlOptions
{
    public Encoding? Encoding { get; set; }
    public NetworkCredential? Credentials { get; set; }
    public CancellationToken? CancellationToken { get; set; }
    public int? MaximumRedirects { get; set; }
    public string? UserAgent { get; set; }
    public bool LoadOEmbed { get; set; } = false;
    
    /// <summary>
    /// HttpClient to use for querying OEmbed content.
    /// If null, will create a new instance of HttpClient which has performance implications for high-volume services.
    /// </summary>
    public HttpClient? OEmbedHttpClient { get; set; }
}

public class UnfurlResult
{
    public string Url { get; set; } = null!;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public List<string>? Keywords { get; set; }
    public string? FavIcon { get; set; }
    public string? Author { get; set; }
    public string? ThemeColor { get; set; }
    public string? CanonicalUrl { get; set; }
    public OEmbedBase? OEmbed { get; set; }
    public XTwitterDetails? XTwitter { get; set; }
    public OpenGraphDetails? OpenGraph { get; set; }
    public string? Encoding { get; set; }
    public string? License { get; set; }
    public string? AppleTouchIcon { get; set; }
    public string? OEmbedLink { get; set; }
}

public class OEmbedBase
{
    public string Type { get; set; }
    public string Version { get; set; } = null!;
    public string? Title { get; set; }
    public string? AuthorName { get; set; }
    public string? AuthorUrl { get; set; }
    public string? ProviderName { get; set; }
    public string? ProviderUrl { get; set; }
    public int? CacheAge { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int? ThumbnailWidth { get; set; }
    public int? ThumbnailHeight { get; set; }
}

public class OEmbedPhoto : OEmbedBase
{
    public string Url { get; set; } = null!;
    public int Width { get; set; }
    public int Height { get; set; }
}

public class OEmbedVideo : OEmbedBase
{
    public string Html { get; set; } = null!;
    public int Width { get; set; }
    public int Height { get; set; }
}

public class OEmbedLink : OEmbedBase
{
}

public class OEmbedRich : OEmbedBase
{
    public string Html { get; set; } = null!;
    public int Width { get; set; }
    public int Height { get; set; }
}

public struct OEmbedTypes
{
    public const string Photo = "photo";
    public const string Video = "video";
    public const string Link = "link";
    public const string Rich = "rich";
}

public class XTwitterDetails
{
    public string? this[string propertyName]
    {
        get => GetType().GetProperty(propertyName)?.GetValue(this, null) as string;
        set => GetType().GetProperty(propertyName)?.SetValue(this, value, null);
    }

    public string? Card { get; set; }
    public string? Site { get; set; }
    public string? SiteId { get; set; }
    public string? Creator { get; set; }
    public string? CreatorId { get; set; }
    public string? Description { get; set; }
    public string? Title { get; set; }
    public string? Image { get; set; }
    public string? ImageAlt { get; set; }
    public string? Player { get; set; }
    public string? PlayerWidth { get; set; }
    public string? PlayerHeight { get; set; }
    public string? PlayerStream { get; set; }
}

public class OpenGraphDetails
{
    public string? Url { get; set; }
    public string? Title { get; set; }
    public string? Type { get; set; }
    public string? Image { get; set; }
    public string? Audio { get; set; }
    public string? Description { get; set; }
    public string? Determiner { get; set; }
    public string? Locale { get; set; }
    public string? LocaleAlternate { get; set; }
    public string? SiteName { get; set; }
    public string? Video { get; set; }
    public List<OpenGraphImage>? Images { get; set; }
    public List<OpenGraphVideo>? Videos { get; set; }
    public List<OpenGraphAudio>? Audios { get; set; }
    
    public string? ArticlePublishedTime { get; set; }
    public string? ArticleModifiedTime { get; set; }
    public string? ArticleExpirationTime { get; set; }
    public string? ArticleAuthor { get; set; }
    public string? ArticleSection { get; set; }
    public string? ArticleTags { get; set; }
    
    public string? BookAuthor { get; set; }
    public string? BookReleaseDate { get; set; }
    public string? BookIsbn { get; set; }
    public string? BookTags { get; set; }

    public string? this[string propertyName]
    {
        get => GetType().GetProperty(propertyName)?.GetValue(this, null) as string;
        set => GetType().GetProperty(propertyName)?.SetValue(this, value, null);
    }
}

public class OpenGraphImage
{
    public string? Url { get; set; }
    public string? SecureUrl { get; set; }
    public string? Type { get; set; }
    public string? Width { get; set; }
    public string? Height { get; set; }
    public string? Alt { get; set; }
}

public class OpenGraphAudio
{
    public string? Url { get; set; }
    public string? SecureUrl { get; set; }
    public string? Type { get; set; }
}

public class OpenGraphVideo
{
    public string? Url { get; set; }
    public string? Stream { get; set; }
    public string? Width { get; set; }
    public string? Height { get; set; }
    public string? Tags { get; set; }
    public string? SecureUrl { get; set; }
    public string? Type { get; set; }
}