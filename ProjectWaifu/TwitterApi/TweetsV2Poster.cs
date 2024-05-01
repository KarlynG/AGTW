using System.Text;
using Newtonsoft.Json;
using ProjectWaifu.AIServices;
using ProjectWaifu.Services;
using Tweetinvi;
using Tweetinvi.Core.Web;
using Tweetinvi.Models;

namespace ProjectWaifu.TwitterApi;

public class TweetsV2Poster
{
    private readonly ITwitterClient client;
    private readonly ImageGenerator _imageGenerator;
    private readonly TextGeneration _textGeneration;

    public TweetsV2Poster(ITwitterClient client, ImageGenerator imageGenerator, TextGeneration textGeneration)
    {
        this.client = client;
        _imageGenerator = imageGenerator;
        _textGeneration = textGeneration;
    }

    public Task<ITwitterResult> PostTweet(string text)
    {
        return client.Execute.AdvanceRequestAsync(
            (ITwitterRequest request) =>
            {
                var tweetRequest = new TweetV2PostRequest()
                {
                    Text = text,
                };
                var jsonBody = client.Json.Serialize(tweetRequest);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                request.Query.Url = "https://api.twitter.com/2/tweets";
                request.Query.HttpMethod = Tweetinvi.Models.HttpMethod.POST;
                request.Query.HttpContent = content;
            }
        );
    }
    public async Task PostAIGeneratedTweet(bool useLastTweet = true)
    {
        Console.WriteLine("\nGenerating Text...");
        var text = await _textGeneration.GetTweetText(useLastTweet);
        string resultString = text.Replace("\"", "");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Success!");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\nGenerating SD Prompt...\n");
        var prompt = await _textGeneration.GetSDPrompt(text);
        string resultPrompt = prompt.Replace("\"", "");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Success!");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\nPublishing tweet with media.\n");
        try
        {
            TweetService tweetService = new TweetService(); 
            await PostTweetWithMedia(resultString, resultPrompt);
            await tweetService.AddTweet(resultString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error publishing tweet: {ex.Message}");
            Console.ReadLine();
        }
    }
    public async Task<ITwitterResult> PostTweetWithMedia(string text, string? prompt = "")
    {
        Console.WriteLine("Generating Image...");
        var result = await _imageGenerator.GenerateImageAsync(prompt);
        // Convert the base64-encoded image data to a byte array
        byte[] imageData = Convert.FromBase64String(result[0]);

        // Upload the image to Twitter
        Console.WriteLine("Uploading image...");
        var uploadedPhoto = await client.Upload.UploadTweetImageAsync(imageData);
        Console.WriteLine("Making post on twitter...");
        return await client.Execute.AdvanceRequestAsync(
            (ITwitterRequest request) =>
            {
                var tweetRequest = new TweetV2PostRequestWithMedia()
                {
                    Text = text,
                    media = new mediaIDS()
                    {
                        media_ids = [uploadedPhoto.Id.ToString()]
                    }
                };
                var jsonBody = client.Json.Serialize(tweetRequest);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                request.Query.Url = "https://api.twitter.com/2/tweets";
                request.Query.HttpMethod = Tweetinvi.Models.HttpMethod.POST;
                request.Query.HttpContent = content;
            }
        );
    }
}

/// <summary>
/// There are a lot more fields according to:
/// https://developer.twitter.com/en/docs/twitter-api/tweets/manage-tweets/api-reference/post-tweets
/// but these are the ones we care about for our use case.
/// </summary>
public class TweetV2PostRequest
{
    /// <summary>
    /// The text of the tweet to post.
    /// </summary>
    [JsonProperty("text")]
    public string Text { get; set; } = string.Empty;
}
public class TweetV2PostRequestWithMedia
{
    [JsonProperty("text")]
    public string Text { get; set; } = string.Empty;
    public mediaIDS? media { get; set; }
}

public class mediaIDS
{
    public string[] media_ids { get; set; }
}