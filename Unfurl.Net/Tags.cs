using System.Collections.Generic;

namespace Unfurl.Net;

internal class Tag
{
    public string XPath { get; set; } = null!;
    public string Label { get; set; } = null!;
}

internal class Tags
{
    // twitter tags   
    private static readonly Tag TwitterCard = new() { Label = "Card", XPath = "meta[@name='twitter:card']" };
    private static readonly Tag TwitterSite = new() { Label = "Site", XPath = "meta[@name='twitter:site']" };
    private static readonly Tag TwitterSiteId = new() { Label = "SiteId", XPath = "meta[@name='twitter:site:id']" };
    private static readonly Tag TwitterCreator = new() { Label = "Creator", XPath = "meta[@name='twitter:creator']" };
    private static readonly Tag TwitterCreatorId = new() { Label = "CreatorId", XPath = "meta[@name='twitter:creator:id']" };
    private static readonly Tag TwitterDescription = new() { Label = "Description", XPath = "meta[@name='twitter:description']" };
    private static readonly Tag TwitterTitle = new() { Label = "Title", XPath = "meta[@name='twitter:title']" };
    private static readonly Tag TwitterImage = new() { Label = "Image", XPath = "meta[@name='twitter:image']" };
    private static readonly Tag TwitterImageAlt = new() { Label = "ImageAlt", XPath = "meta[@name='twitter:image:alt']" };
    private static readonly Tag TwitterPlayer = new() { Label = "Player", XPath = "meta[@name='twitter:player']" };
    private static readonly Tag TwitterPlayerWidth = new() { Label = "PlayerWidth", XPath = "meta[@name='twitter:player:width']" };
    private static readonly Tag TwitterPlayerHeight = new() { Label = "PlayerHeight", XPath = "meta[@name='twitter:player:height']" };
    private static readonly Tag TwitterPlayerStream = new() { Label = "PlayerStream", XPath = "meta[@name='twitter:player:stream']" };
    // note: not including Twitter app cards right now. If there is a request for them, will add then.
    
    
    private static readonly Tag OgUrl = new() { Label = "Url", XPath = "meta[@property='og:url']" };
    private static readonly Tag OgTitle = new() { Label = "Title", XPath = "meta[@property='og:title']" };
    private static readonly Tag OgType = new() { Label = "Type", XPath = "meta[@property='og:type']" };
    private static readonly Tag OgImage = new() { Label = "Image", XPath = "meta[@property='og:image']" };
    private static readonly Tag OgAudio = new() { Label = "Audio", XPath = "meta[@property='og:audio']" };
    private static readonly Tag OgDescription = new() { Label = "Description", XPath = "meta[@property='og:description']" };
    private static readonly Tag OgDeterminer = new() { Label = "Determiner", XPath = "meta[@property='og:determiner']" };
    private static readonly Tag OgLocale = new() { Label = "Locale", XPath = "meta[@property='og:locale']" };
    private static readonly Tag OgLocaleAlternate = new() { Label = "LocaleAlternate", XPath = "meta[@property='og:locale:alternate']" };
    private static readonly Tag OgSiteName = new() { Label = "SiteName", XPath = "meta[@property='og:site_name']" };
    private static readonly Tag OgVideo = new() { Label = "Video", XPath = "meta[@property='og:video']" };
    
    private static readonly Tag OgArticlePublished = new() { Label = "ArticlePublishedTime", XPath = "meta[@property='article:published_time']" };
    private static readonly Tag OgArticleModified = new() { Label = "ArticleModifiedTime", XPath = "meta[@property='article:modified_time']" };
    private static readonly Tag OgArticleExpiration = new() { Label = "ArticleExpirationTime", XPath = "meta[@property='article:expiration_time']" };
    private static readonly Tag OgArticleAuthor = new() { Label = "ArticleAuthor", XPath = "meta[@property='article:author']" };
    private static readonly Tag OgArticleSection = new() { Label = "ArticleSection", XPath = "meta[@property='article:section']" };
    private static readonly Tag OgArticleTags = new() { Label = "ArticleTags", XPath = "meta[@property='article:tag']" };
    
    private static readonly Tag OgBookAuthor = new() { Label = "BookAuthor", XPath = "meta[@property='book:author']" };
    private static readonly Tag OgBookReleaseDate = new() { Label = "BookReleaseDate", XPath = "meta[@property='book:release_date']" };
    private static readonly Tag OgBookIsbn = new() { Label = "BookIsbn", XPath = "meta[@property='book:isbn']" };
    private static readonly Tag OgBookTags = new() { Label = "BookTags", XPath = "meta[@property='book:tags']" };

    public static readonly List<Tag> XTwitterTags = new()
    {
        TwitterCard, TwitterSite, TwitterSiteId, TwitterCreator, TwitterCreatorId, TwitterDescription, TwitterTitle, TwitterImage,
        TwitterImageAlt, TwitterPlayer, TwitterPlayerWidth, TwitterPlayerHeight, TwitterPlayerStream
    };
    public static readonly List<Tag> OpenGraphTags = new()
    {
        OgUrl, OgTitle, OgType, OgImage, OgAudio, OgDescription, OgDeterminer, OgLocale, OgLocaleAlternate, OgSiteName, OgVideo,
        OgArticleAuthor, OgArticleExpiration, OgArticleModified, OgArticlePublished, OgArticleSection, OgArticleTags,
        OgBookAuthor, OgBookReleaseDate, OgBookIsbn, OgBookTags
    };
    public static readonly List<Tag> OEmbedGraphTags = new();
}


/*
 * TODO: add support for google scholar citation meta tags:
 * see details: https://wiki.whatwg.org/wiki/MetaExtensions
 *
 * citation_author 	The name of an author of an academic paper 	 		
   citation_author_email 	The email address of the preceding author (identified using citation_author) of an academic paper 			
   citation_author_institution 	The name of an institution to which the preceding author (identified using citation_author) of an academic paper is affiliated 			
   citation_conference_title 	The title of the conference at which an academic paper is published 	 		
   citation_date 	The publication date of an academic paper 			
   citation_dissertation_institution 	The name of the institution which published an academic dissertation 	 		
   citation_doi 	The Digital Object Identifier of an academic paper 			
   citation_firstpage 	The first page of the journal in which an academic paper is published 	 		
   citation_fulltext_html_url 	The URL of the full text HTML version of an academic paper 			
   citation_isbn 	The ISBN of the book in which an academic paper is published 	 		
   citation_issn 	The ISSN of the journal in which an academic paper is published 	 		
   citation_issue 	The issue of the journal in which an academic paper is published 	 		
   citation_journal_abbrev 	The abbreviated title of the journal in which an academic paper is published 			
   citation_journal_title 	The title of the journal in which an academic paper is published 	 		
   citation_keywords 	A semicolon-separated list of keywords associated with an academic paper 			
   citation_language 	The language in which an academic paper is written, as an ISO 639-1 two-letter code 			
   citation_lastpage 	The last page of the journal in which an academic paper is published 	 		
   citation_pdf_url 	The URL of a PDF version of an academic paper 	 		
   citation_publication_date 	The publication date of an academic paper 	 		
   citation_publisher 	The name of the publisher of an academic paper 	The spec (as accessed 11-26-17) omits this meta name but refers to the meta name DC.publisher (see that on this page). 		
   citation_technical_report_institution 	The name of the institution which published an academic technical report or preprint 	 		
   citation_technical_report_number 	The identification number of an academic technical report or preprint 	 		
   citation_title 	The title of an academic paper 	 		
*/