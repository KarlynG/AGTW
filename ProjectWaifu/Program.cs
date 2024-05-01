using Microsoft.Extensions.Configuration;
using ProjectWaifu.Services;

try
{
    IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .AddCommandLine(args);

    IConfiguration configuration = configurationBuilder.Build();

    MainServices services = new MainServices(configuration);
    await services.MainLoop();
}
catch (Exception ex)
{
    Console.WriteLine($"Error loading configuration: {ex.Message}");
    // Log the exception details for further investigation
}
