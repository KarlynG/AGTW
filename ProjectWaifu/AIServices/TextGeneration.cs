using Microsoft.Extensions.Configuration;
using ProjectWaifu.Services;
using System.Text;

namespace ProjectWaifu.AIServices;

public class TextResponse
{
    public List<TextResponseChoice> choices { get; set; }
}
public class TextResponseChoice
{
    public int index { get; set; }
    public TextResponseMessage message { get; set; }
}
public class TextResponseMessage
{
    public string role { get; set; }
    public string content { get; set; }
}
public class TextGeneration
{
    private readonly IConfiguration _configuration;
    private readonly string? _endpoint = "";
    private readonly TweetService _tweetService;
    public TextGeneration(IConfiguration configuration)
    {
        _configuration = configuration;
        _endpoint = _configuration["TextGenWebUI:Endpoint"];
        _tweetService = new TweetService();
    }
    public async Task<string> GetTweetText(bool useLastTweet)
    {
        var result = await _tweetService.GetLatestTweet();
        string prompt;
        if (result == null)
        {
            prompt = "Give me your very first tweet. ";
        }
        else
        {
            prompt = "Craft your next tweet.";
            if(useLastTweet)
            {
                prompt += $"Keep in mind your previous tweet before doing so. Don't include dates in your response.\nPrevious tweet: {result.Message}\nTweet Date: {result.CreationDate}\nCurrent Date: {DateTime.Now}";
            }
        }
        prompt += "Remember that tweets need to be short. Don't include anything stable diffusion related.";
        using (var httpClient = new HttpClient())
        {
            var requestPayload = new
            {
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                mode = "chat-instruct",
                character = "Holo Neko-chan"
            };
            var requestJson = System.Text.Json.JsonSerializer.Serialize(requestPayload);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(_endpoint, content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var textResponse = System.Text.Json.JsonSerializer.Deserialize<TextResponse>(responseJson);
            var textResult = textResponse.choices.FirstOrDefault().message.content;
            return RemoveEndingTag(textResult);
        }
    }
    public async Task<string> GetSDPrompt(string current = "")
    {
        var prompt = """
                Give me a stable diffusion prompt based on this tweet. Stable diffusion prompts are separated by comas and describe a scene or an action. I only need the prompt, anything else in unnecessary.
                Prompt scenery reply example: bedroom, desktop, posters, pink room, plushies
                Prompt action reply example: one eye closed, tongue out, peace sign, smile
                """;
        prompt += $"\n Your current tweet you should based your prompt on: '{current}' \n Remember I only need the prompt. Only the prompt.";
        var result = await _tweetService.GetLatestTweet();
        if(result != null)
        {
            prompt += $"\n Keep in mind your last tweet before crafting the prompt. Last Tweet: \n '{result.Message}'";
        }
        using (var httpClient = new HttpClient())
        {
            var requestPayload = new
            {
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                mode = "chat-instruct",
                character = "Holo Neko-chan"
            };
            var requestJson = System.Text.Json.JsonSerializer.Serialize(requestPayload);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(_endpoint, content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var textResponse = System.Text.Json.JsonSerializer.Deserialize<TextResponse>(responseJson);
            var textResult = textResponse.choices.FirstOrDefault().message.content;
            return RemoveEndingTag(textResult);
        }
    }
    private string RemoveEndingTag(string input)
    {
        if (input.EndsWith("</s>"))
        {
            return input.Substring(0, input.Length - 4);
        }
        return input;
    }
}
