using FluentAssertions;

namespace Unfurl.Net.Tests;

public class Tests
{
    [Fact]
    public async Task CanLoadATwitterCard()
    {
        var unfurler = new Unfurler();
        var url = "https://developer.x.com/en/docs/x-for-websites/cards/overview/markup";
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
        var results = await unfurler.Unfurl(url, new() {LoadOEmbed = true});

        results.Url.Should().Be(url);
        results.FavIcon.Should().Contain("favicon_32x32.png");
        results.CanonicalUrl.Should().Be("https://www.youtube.com/watch?v=Unzc731iCUY");
        results.Description.Should()
            .Be(
                "MIT How to Speak, IAP 2018Instructor: Patrick WinstonView the complete course: https://ocw.mit.edu/how_to_speakPatrick Winston&#39;s How to Speak talk has been a...");
        results.Keywords.Count.Should().BeGreaterThan(1);
        results.Keywords[0].Should().Be("Aloud");

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


        results.OEmbed.Should().NotBeNull();
        results.OEmbed!.Title.Should().StartWith("How to Speak");
        results.OEmbed!.Type.Should().Be(OEmbedTypes.Video);
        var video = results.OEmbed as OEmbedVideo;
        video.Should().NotBeNull();
        video!.Html.Should().StartWith("<");
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
        results.OpenGraph.Should().NotBeNull();
        results.OpenGraph!.Title.Should().Be("Alternative Hip-Hop");
    }

    [Fact]
    public async Task CanParseMsFormOembed()
    {
        var httpClient = new HttpClient();
        // https://youtu.be/5EI0OP7o8cM?si=Iu1qnqk8aXrkc_Bi
        var unfurler = new Unfurler();
        var url = "https://forms.office.com/r/YLPA60FDtJ";
        var results = await unfurler.Unfurl(url, new UnfurlOptions()
        {
            LoadOEmbed = true,
            MaximumRedirects = 2,
            OEmbedHttpClient = httpClient,
        });

        results.OEmbed.Should().NotBeNull();
    }

    [Fact]
    public async Task CanParseYouTubeShareLink()
    {
        var unfurler = new Unfurler();
        var url = "https://youtu.be/5EI0OP7o8cM?si=Iu1qnqk8aXrkc_Bi";
        var results = await unfurler.Unfurl(url, new UnfurlOptions
        {
            MaximumRedirects = 2,
            LoadOEmbed = true
        });

        results.OpenGraph.Should().NotBeNull();
        results.OpenGraph!.Title.Should().Be("If Beethoven Were A METAL Bassist...");
        results.OEmbed.Should().NotBeNull();
        results.OEmbed!.Title.Should().Be("If Beethoven Were A METAL Bassist...");
        results.OEmbed!.Type.Should().Be(OEmbedTypes.Video);
        var video = results.OEmbed as OEmbedVideo;
        video.Should().NotBeNull();
        video!.Html.Should().StartWith("<");
    }

    [Fact]
    public async Task DoesntParseOembedWhenToldNotTo()
    {
        var unfurler = new Unfurler();
        var url = "https://youtu.be/5EI0OP7o8cM?si=Iu1qnqk8aXrkc_Bi";
        var results = await unfurler.Unfurl(url, new UnfurlOptions() { LoadOEmbed = false, MaximumRedirects = 2 });

        results.OpenGraph.Should().NotBeNull();
        results.OpenGraph!.Title.Should().Be("If Beethoven Were A METAL Bassist...");
        results.OEmbed.Should().BeNull();
    }
}