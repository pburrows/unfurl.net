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

## Roadmap

The current features are sufficient for my needs. But if there are any additional features you would like to see, please add an issue (or even submit a PR if you want) and I will work on adding them. 

Here are some features that could be added:

* Accept Html as a string or stream for parsing, and not just a Url that has to be downloaded. This will allow alternate methods of downloading Urls and Unfurl.Net can be used simply as the parser, not also as the Html downloader.
* Work with oEmbed data. oEmbed requires looking for the oEmbed tag (which unfurl.net does now) and then downloading the Json or Xml file referenced in that tag. At this point, I hadn't come across a lot of oEmbed tags, so I didn't implement that feature. But if this changes in the future, I will add that (or, again, feel free to request this feature in an issue.)