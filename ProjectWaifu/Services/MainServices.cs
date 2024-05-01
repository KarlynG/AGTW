using Microsoft.Extensions.Configuration;
using ProjectWaifu.Enums;

namespace ProjectWaifu.Services;

public class MainServices
{
    public ProcessExecution _processExecution { get; set; }
    public MainServices(IConfiguration configuration)
    {
        _processExecution = new ProcessExecution(configuration);
    }
    public async Task MainLoop()
    {
        MenuOptions[] menuOptions = { MenuOptions.Auto, MenuOptions.PublishTweet, MenuOptions.Exit };
        int selectedIndex = 0;

        while (true)
        {
            DisplayMenu(menuOptions, selectedIndex);

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
                        await _processExecution.StartAsync(menuOptions[selectedIndex]);
                        Console.ReadKey();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        Console.ReadKey();
                    }
                    break;
            }
        }
    }
    /// <summary>
    /// Displays the menu options with the selected item highlighted.
    /// </summary>
    /// <param name="menuOptions">The array of menu options.</param>
    /// <param name="selectedIndex">The index of the currently selected option.</param>
    private void DisplayMenu(MenuOptions[] menuOptions, int selectedIndex)
    {
        Console.Clear();
        Console.WriteLine("Cool Console Menu");
        Console.WriteLine("------------------");

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
    }
}
