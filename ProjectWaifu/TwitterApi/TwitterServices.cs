namespace ProjectWaifu.TwitterApi;

using Microsoft.Extensions.Configuration;
using ProjectWaifu.AIServices;
using System.Threading.Tasks;
using Tweetinvi;

public class TwitterService
{
    private readonly IConfiguration _configuration;
    private readonly TwitterClient _twitterClient;
    private readonly TweetsV2Poster _tweetsV2Poster;

    public TwitterService(IConfiguration configuration)
    {
        _configuration = configuration;

        var consumerKey = _configuration["Twitter:ConsumerKey"];
        var consumerSecret = _configuration["Twitter:ConsumerSecret"];
        var accessToken = _configuration["Twitter:AccessToken"];
        var accessTokenSecret = _configuration["Twitter:AccessTokenSecret"];

        _twitterClient = new TwitterClient(consumerKey, consumerSecret, accessToken, accessTokenSecret);
        _tweetsV2Poster = new TweetsV2Poster(_twitterClient, new ImageGenerator(_configuration), new TextGeneration(_configuration));
    }

    public async Task PublishTweetAsync(string tweetText)
    {
        Console.Write("\nPublishing...\n");

        await _tweetsV2Poster.PostTweet(tweetText);
    }
    public async Task PublishTweetWithMediaAsync(string tweetText)
    {
        Console.Write("\nPublishing...\n");

        await _tweetsV2Poster.PostTweetWithMedia(tweetText);
    }
    public async Task PublishAITweet()
    {
        Console.Clear();
        Console.WriteLine("Use previous tweet as reference? Y or Enter for Yes");
        string? response = Console.ReadLine();
        var result = (string.IsNullOrEmpty(response) || response == "y");
        await _tweetsV2Poster.PostAIGeneratedTweet(result);
        Console.WriteLine("\nTweet Published successfully!\n");
    }
}
