using Microsoft.Extensions.Configuration;
using ProjectWaifu.Enums;
using ProjectWaifu.TwitterApi;

namespace ProjectWaifu.Services;

public class ProcessExecution
{
    public TwitterService _twitterService { get; set; }
    public ProcessExecution(IConfiguration configuration)
    {
        _twitterService = new TwitterService(configuration);
    }
    public async Task StartAsync(MenuOptions option)
    {
        switch (option)
        {
            case MenuOptions.PublishTweet:
                await PublishTweetMenu();
                // Handle the PublishTweet option
                break;
            case MenuOptions.Auto:
                await _twitterService.PublishAITweet();
                // Handle the Auto option
                break;
            default:
                await Console.Out.WriteLineAsync("\nInvalid option");
                break;
        }
    }
    public async Task PublishTweetMenu()
    {
        Console.Clear();
        MenuOptions[] menuOptions = { MenuOptions.Text, MenuOptions.Media, MenuOptions.Back };
        int selectedIndex = 0;
        while (true)
        {
            Console.Clear();
            for (int i = 0; i < menuOptions.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("> ");
                }
                else
                {
                    Console.Write("  ");
                }

                Console.WriteLine(menuOptions[i].GetDescription());
                Console.ResetColor();
            }

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex - 1 + menuOptions.Length) % menuOptions.Length;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex + 1) % menuOptions.Length;
                    break;
                case ConsoleKey.Enter:
                    if (menuOptions[selectedIndex] == MenuOptions.Exit)
                    {
                        Console.WriteLine("Exiting the program...");
                        return;
                    }

                    try
                    {
                        await PublishTweetFromConsole(menuOptions[selectedIndex]);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        Console.ReadKey();
                    }
                    break;
            }
            if ((keyInfo.Key == ConsoleKey.Enter && menuOptions[selectedIndex] == MenuOptions.Back))
                break;
        }
    }
    public async Task PublishTweetFromConsole(MenuOptions options)
    {
        if (options == MenuOptions.Back)
        {
            return;
        }
        Console.Clear();
        Console.WriteLine("Enter the tweet message:");
        string? tweetText = Console.ReadLine();
        if(string.IsNullOrEmpty(tweetText))
        {
            Console.WriteLine("Not valid.");
            return;
        }

        Console.WriteLine($"\nTweet message: \"{tweetText}\"");
        Console.WriteLine("Are you sure you want to publish this tweet? (y/n)");
        string? confirmation = Console.ReadLine();
        if (string.IsNullOrEmpty(confirmation))
        {
            Console.Clear();
            Console.WriteLine("Not valid.");
            return;
        }
        if (confirmation.ToLower() == "y")
        {
            try
            {
                if(options == MenuOptions.Text)
                {
                    await _twitterService.PublishTweetAsync(tweetText);
                }
                else
                {
                    await _twitterService.PublishTweetWithMediaAsync(tweetText);
                }
                Console.WriteLine("\nTweet published successfully! Press any key to continue.");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn error occurred while publishing the tweet: {ex.Message}");
                Console.ReadLine();
                Console.Clear();
            }
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Tweet publication canceled.");
        }
    }
}
