# Unfurl.Net

A .net metadata scraper that supports X (nee Twitter) Cards and Open Graph.

## Installation

Unfurl.Net is published as a [NuGet package](https://www.nuget.org/packages/Unfurl.Net). Download from your favorite nuget package manager.

--or--

Install from the command line with:

```bash
dotnet add package Unfurl.Net
```

## Overview

Unfurl.Net is inspired by the [unfurl.js](https://github.com/jacktuck/unfurl) nodejs project which parses meta data from HTML content and turns it into a convenient object model for displaying URL previews and cards similar to how Facebook, Slack, Discord, or Twitter do. 

This library just parses the Html for OpenGraph and Twitter Card `<meta />` tags and turns it into an easy to use object. It does not actually build a visual representation of the Url. 

Unfurl.Net is a simple library that depends on the venerable [HtmlAgilityPack](https://github.com/zzzprojects/html-agility-pack) for its parsing. 

## Usage

Basic usage that loads open graph and X/Twitter cards:

```csharp
var unfurler = new Unfurler();
var url = "https://developer.x.com/en/docs/x-for-websites/cards/overview/markup";
var results = await unfurler.Unfurl(url);
```

Include oEmbed data:

```csharp
var httpClient = new HttpClient(); // <-- don't do this in production!
var unfurler = new Unfurler();
var url = "https://www.youtube.com/watch?v=5EI0OP7o8cM";
var results = await unfurler.Unfurl(url, new UnfurlOptions()
{
    LoadOEmbed = true,
    OEmbedHttpClient = httpClient,
});
```

Handle redirects and shortened links:

```csharp
var unfurler = new Unfurler();
var url = "https://www.youtube.com/watch?v=5EI0OP7o8cM";
var results = await unfurler.Unfurl(url, new UnfurlOptions()
{
    MaximumRedirects = 2,
});
```

All Options:

| Property          | Description                                                                                                                                                                                                                                               |
|-------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Encoding          | set the encoding of the target url. Default to null                                                                                                                                                                                                       |
| Credentials       | pass in custom network credentials if needed.                                                                                                                                                                                                             |
| CancellationToken | control cancellation of the network requests                                                                                                                                                                                                              |
| MaximumRedirects  | the number of redirects to follow from 301 or 302 responses.                                                                                                                                                                                              |
| UserAgent         | the user-agent string to pass as part of the request headers. Defaults to "Unfurl.Net/1.1"                                                                                                                                                                |
| LoadOEmbed        | whether or not to make the extra web-request to load any oEmbed content                                                                                                                                                                                   |
| OEmbedHttpClient  | Instance of [HttpClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-8.0) to use for querying oEmbed. If null, will create a new instance of HttpClient which has performance implications for high-volume services. |



## Roadmap

The current features are sufficient for my needs. But if there are any additional features you would like to see, please add an issue (or even submit a PR if you want) and I will work on adding them. 

Here are some features that could be added:

* Accept Html as a string or stream for parsing, and not just a Url that has to be downloaded. This will allow alternate methods of downloading Urls and Unfurl.Net can be used simply as the parser, not also as the Html downloader.