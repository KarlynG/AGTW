# Project Waifu
Project Waifu is a console app that generates and publishes posts on Twitter, simulating the presence of a cute anime girl. 
By leveraging advanced language models and image generation techniques, Project Waifu creates engaging content that mimics the style and persona of 
an anime character.

## Features

- AI-generated text content using state-of-the-art language models
- AI-generated images that complement the generated text
- Seamless integration with the Twitter API for automated posting
- Persistence of previous tweets using SQLite for maintaining context
- Flexibility to create manual or fully AI-generated posts
- Customizable persona for the anime girl character

## Installation
**1. Clone the repository**
   ```
   git clone https://github.com/yourusername/project-waifu.git
   ```
**2. Install the required dependencies**
   ```
   cd project-waifu
   dotnet restore
   ```
**3. Set up the necessary API crendentials:**
  - Create a Twitter Developer account and obtain API keys and access tokens.
  - Set up the oobabooga/text-generation-webui and AUTOMATIC1111/stable-diffusion-webui APIs locally.
    
**4. Configure the application:**
  - Open appsettings.json and provide the required API credentials and configurations
**5. Run the application:**
    ```
    dotnet run
    ```

## Usage

1. Launch the application and choose between manual or AI-generated posting mode.
2. For manual posting:
  - Enter the desired text content for the tweet
  - Optionally, provide an image or let the AI generate one based on the text
3. For AI-generated posting:
  - The application will generate text content based on the anime girl's persona and previous tweets
  - An image will be automatically generated to accompany the text
4. Review the generated content and confirm the posting to Twitter.
5. The application will save the posted tweet in the SQLite database for future reference and context.

## Technologies Used
- .NET 8
- SQLite for data persistence

## Acknowledgements

- [oobabooga/text-generation-webui](https://github.com/oobabooga/text-generation-webui)
- [AUTOMATIC1111/stable-diffusion-webui](https://github.com/AUTOMATIC1111/stable-diffusion-webui)
- [Twitter API Documentation](https://developer.twitter.com/en/docs/twitter-api/tweets/manage-tweets/introduction)
