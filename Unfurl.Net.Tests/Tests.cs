using FluentAssertions;

namespace Unfurl.Net.Tests;

public class Tests
{
    [Fact]
    public async Task CanLoadATwitterCard()
    {
        var unfurler = new Unfurler();
        var url = "https://developer.twitter.com/en/docs/twitter-for-websites/cards/overview/markup";
        var results = await unfurler.Unfurl(url);

        results.Url.Should().Be(url);
        Assert.NotNull(results.XTwitter);
        results.Encoding.Should().Be("utf-8");
        results.XTwitter.Card.Should().Be("summary_large_image");
        results.Title.Trim().Should().Be("Cards markup | Docs | Twitter Developer Platform");
    }

    [Fact]
    public async Task CanParseAYouTubeLink()
    {
        var unfurler = new Unfurler();
        var url = "https://www.youtube.com/watch?v=Unzc731iCUY";
        var results = await unfurler.Unfurl(url);

        results.Url.Should().Be(url);
        results.FavIcon.Should().Contain("favicon_32x32.png");
        results.CanonicalUrl.Should().Be("https://www.youtube.com/watch?v=Unzc731iCUY");
        results.Description.Should()
            .Be(
                "MIT How to Speak, IAP 2018Instructor: Patrick WinstonView the complete course: https://ocw.mit.edu/how_to_speakPatrick Winston&#39;s How to Speak talk has been a...");
        results.Keywords.Count.Should().Be(1);
        results.Keywords[0].Should().Be("Aloud");
        results.OEmbedLink.Should()
            .Be(
                "https://www.youtube.com/oembed?format=json&amp;url=https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3DUnzc731iCUY");
        results.XTwitter.Card.Should().Be("player");
        results.XTwitter.Image.Should().Be("https://i.ytimg.com/vi/Unzc731iCUY/maxresdefault.jpg");
        results.XTwitter.Title.Should().Be("How to Speak");

        results.OpenGraph.SiteName.Should().Be("YouTube");
        results.OpenGraph.Image.Should().Be("https://i.ytimg.com/vi/Unzc731iCUY/maxresdefault.jpg");
        results.OpenGraph.Images.Count.Should()
            .Be(1); //[0].Url.Should().Be("https://i.ytimg.com/vi/Unzc731iCUY/maxresdefault.jpg");
        results.OpenGraph.Images[0].Url.Should().Be("https://i.ytimg.com/vi/Unzc731iCUY/maxresdefault.jpg");
        results.OpenGraph.Images[0].Height.Should().Be("720");
        results.OpenGraph.Description.Should()
            .Be(
                "MIT How to Speak, IAP 2018Instructor: Patrick WinstonView the complete course: https://ocw.mit.edu/how_to_speakPatrick Winston&#39;s How to Speak talk has been a...");
        results.OpenGraph.Type.Should().Be("video.other");
    }

    [Fact]
    public async Task CanParseSubstack()
    {
        var unfurler = new Unfurler();
        var url = "https://www.lennysnewsletter.com/p/good-strategy-bad-strategy-richard";
        var results = await unfurler.Unfurl(url);

        results.Url.Should().Be(url);
    }

    [Fact]
    public async Task CanParseSpotifyPlaylist()
    {
        var unfurler = new Unfurler();
        var url = "https://open.spotify.com/playlist/37i9dQZF1DWTggY0yqBxES?si=54fc0ceeec884e3e";
        var results = await unfurler.Unfurl(url);

        results.Url.Should().Be(url);
        results.OpenGraph.Title.Should().Be("Alternative Hip-Hop");
    }
}