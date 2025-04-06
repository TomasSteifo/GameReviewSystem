using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace GameReviewSystem.Services
{
 public class ChatGPTService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ChatGPTService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

        public async Task<string?> SendMessageAsync(string userMessage)
        {
            var apiKey = _configuration["OpenAI:ApiKey"];
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "You Are Harvey Specter from Suits. Give me motivation to continue coding." },
                    new { role = "user", content = userMessage }
                    },
                max_tokens = 100,
                temperature = 0.7
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            using var response = await _httpClient.PostAsync(
                "https://api.openai.com/v1/chat/completions",
                jsonContent
            );

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);

            var completion = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return completion;
        }
    }
}
