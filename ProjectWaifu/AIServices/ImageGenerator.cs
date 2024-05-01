using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;

namespace ProjectWaifu.AIServices;

public class ImageGenerationResponse
{
    public string[] images { get; set; }
}

public class ImageGenerator
{
    private readonly IConfiguration _configuration;
    private readonly string? _endpoint = "";
    public ImageGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
        _endpoint = _configuration["SDWebUI:Endpoint"];
    }

    public async Task<string[]> GenerateImageAsync(string? prompt = "Cozy living room with armchair, floor lamp, bookshelf filled with books, potted plant, and a plate of homemade cookies on a coffee table. Warm lighting and comfortable knitted throw blanket")
    {
        using (var httpClient = new HttpClient())
        {
            #region image2text
            var requestPayload = new
            {
                prompt = $"{_configuration["SDWebUI:Prompt"]}, {prompt}",
                negative_prompt = $"{_configuration["SDWebUI:Negatives"]}",
                seed = -1,
                batch_size = 1,
                steps = 30,
                cfg_scale = 8.5,
                width = 1024,
                height = 1024
            };
            var requestJson = System.Text.Json.JsonSerializer.Serialize(requestPayload);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{_endpoint}/txt2img", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var imageResponse = System.Text.Json.JsonSerializer.Deserialize<ImageGenerationResponse>(responseJson);
            #endregion
            Console.WriteLine("Refining image...");
            #region image2image
            var image2imagePayload = new
            {
                prompt = $"{_configuration["SDWebUI:Prompt"]}, {prompt}",
                negative_prompt = $"{_configuration["SDWebUI:Negatives"]}",
                seed = -1,
                batch_size = 1,
                steps = 50,
                cfg_scale = 8.5,
                width = 768,
                height = 1344,
                denoising_strength = 1,
                resize_mode = 2,
                init_images = new[] { imageResponse == null ? "" : imageResponse.images[0] },
            };
            var image2imageJson = System.Text.Json.JsonSerializer.Serialize(image2imagePayload);
            var image2imageContent = new StringContent(image2imageJson, Encoding.UTF8, "application/json");

            var image2imageResponse = await httpClient.PostAsync($"{_endpoint}/img2img", image2imageContent);
            image2imageResponse.EnsureSuccessStatusCode();

            var responseImage2image = await image2imageResponse.Content.ReadAsStringAsync();
            var resultResponse = System.Text.Json.JsonSerializer.Deserialize<ImageGenerationResponse>(responseImage2image);

            #endregion
            return resultResponse == null ? [""] : resultResponse.images;
        }
    }
}
